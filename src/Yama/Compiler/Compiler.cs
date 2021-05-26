using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yama.Index;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.Compiler
{
    public class Compiler
    {

        #region get/set

        public CompileHeader Header
        {
            get;
            set;
        } = new CompileHeader();

        public ContainerManagment ContainerMgmt
        {
            get;
            set;
        } = new ContainerManagment();

        public List<ICompileRoot> toRemove = new List<ICompileRoot>();

        public bool IsLoopHeaderBegin
        {
            get;
            set;
        }

        public FileInfo OutputFile
        {
            get;
            set;
        }

        public StreamWriter Writer
        {
            get;
            set;
        }

        public int OrderCounter
        {
            get;
            set;
        }

        public IProcessorDefinition Definition
        {
            get;
            set;
        }

        public List<ICompileRoot> AssemblerSequence
        {
            get;
            set;
        } = new List<ICompileRoot>();
        public MethodeDeclarationNode MainFunction { get; internal set; }
        public List<CompilerError> Errors { get; set; } = new List<CompilerError>();

        public List<string> Defines { get; set; }
        public Optimize OptimizeLevel
        {
            get;
            set;
        }
        public List<ICompileRoot> DataSequence { get; internal set; } = new List<ICompileRoot>();
        public string LastVariableCall { get; internal set; }
        public IndexVariabelnDeklaration CurrentThis { get; internal set; }
        public IndexVariabelnDeklaration CurrentBase { get; internal set; }
        public List<CompileContainer> Containers { get; private set; } = new List<CompileContainer>();

        public List<SSACompileLine> SSALines
        {
            get;
            set;
        } = new List<SSACompileLine>();

        public bool IsOutputIRCode
        {
            get
            {
                return this.IRCodeStream != null;
            }
        }

        public StreamWriter IRCodeStream
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public CompileAlgo GetAlgo(string algoName, string mode)
        {
            CompileAlgo result = this.Definition.Algos.FirstOrDefault(a=> a.Name == algoName && a.Mode == mode);

            if (result != null) return result;

            this.AddError(string.Format("Der Algorithmus {0} mit dem Modus {1} konnte nicht gefunden werden!", algoName, mode));

            return null;
        }

        public SSACompileLine AddSSALine(SSACompileLine line)
        {
            line.Order = this.OrderCounter;
            this.OrderCounter += 1;

            if (line.Algo == null)
            {
                /*line.LoopContainer = this.ContainerMgmt.CurrentContainer;

                //this.ContainerMgmt.CurrentContainer.Lines.Add(line);
                this.SSALines.Add(line);
                this.ContainerMgmt.CurrentMethod.Lines.Add(line);*/

                return line;
            }

            if (!line.Algo.CanBeDominatet)
            {
                //this.ContainerMgmt.CurrentContainer.Lines.Add(line);
                this.SSALines.Add(line);
                this.ContainerMgmt.CurrentMethod.Lines.Add(line);

                return line;
            }

            foreach (SSACompileLine existLine in this.ContainerMgmt.CurrentContainer.Lines)
            {
                if (!existLine.Algo.Equals(line.Algo)) continue;
                if (existLine.Arguments.Count != line.Arguments.Count) continue;
                if (!this.CheckArgumentsEqual(existLine, line)) continue;

                return existLine;
            }

            this.ContainerMgmt.CurrentMethod.Lines.Add(line);
            this.SSALines.Add(line);

            return line;
        }

        private bool CheckArgumentsEqual(SSACompileLine existLine, SSACompileLine line)
        {
            int count = 0;
            foreach (SSACompileArgument argNew in line.Arguments)
            {
                SSACompileArgument orgArg = existLine.Arguments[count];

                if (argNew.Mode != orgArg.Mode) return false;
                if (argNew.Mode == SSACompileArgumentMode.Reference)
                    if (!argNew.Reference.Equals(orgArg.Reference)) return false;
            }

            return true;
        }

        public bool BeginNewMethode(List<string> registerInUse, CompileContainer compileContainer, ValidUses uses)
        {
            if (this.ContainerMgmt.CurrentMethod != null) this.PopContainer();

            this.Definition.BeginNewMethode(registerInUse);
            this.ContainerMgmt.AddNewMethode(compileContainer);
            compileContainer.StackVarMapper.Push(new Dictionary<string, SSAVariableMap>());
            this.SetNewContainer(compileContainer, uses);
            compileContainer.RegistersUses = registerInUse;

            CompileContainer container = new CompileContainer();
            container.Begin = compileContainer.Begin;
            container.Ende = compileContainer.Ende;
            this.PushContainer(container, null);

            return true;
        }

        public bool AddLine(RequestAddLine request)
        {
            if (request.Dictionaries != null)
            {
                foreach(KeyValuePair<string, string> pair in request.Dictionaries)
                {
                    request.AssemblyCode = request.AssemblyCode.Replace(pair.Key, pair.Value);
                }
            }

            if (request.PostReplaces != null)
            {
                foreach(KeyValuePair<string, string> pair in request.PostReplaces)
                {
                    request.AssemblyCode = request.AssemblyCode.Replace(pair.Key, pair.Value);
                }
            }

            if (this.Writer != null) this.Writer.WriteLine(request.AssemblyCode);

            if (request.AddToList) request.Root.AssemblyCommands.Add(request.AssemblyCode);

            return true;
        }

        public bool Compilen(List<IParseTreeNode> nodes)
        {
            if (this.Definition == null) return this.AddError("No assembler defintion for translating found");
            this.Definition.Compiler = this;

            this.SetNewContainer(new CompileContainer(), null);
            this.ContainerMgmt.CurrentMethod = this.ContainerMgmt.CurrentContainer;

            this.Header.Compile(this, this.MainFunction);

            if (!this.GenerateInheritanceDataStructure(nodes)) return false;

            Parser.Request.RequestParserTreeCompile request = new Parser.Request.RequestParserTreeCompile(this);

            if (!this.GenerateIRCode(nodes, request)) return false;

            if (!this.OptimizeIRCode()) return false;

            if (!this.DoAllocateRegisters()) return false;

            return this.TranslateIRCodeToAssemblerSequence();
        }

        private bool OptimizeIRCode()
        {
            List<SSACompileLine> canBeRemove = new List<SSACompileLine>();

            foreach (SSACompileLine line in this.SSALines)
            {
                if (line.Owner is CompileJumpTo)
                {
                    SSACompileArgument arg = line.Arguments.FirstOrDefault();
                    if (arg == null) continue;

                    if (arg.Mode != SSACompileArgumentMode.JumpReference) continue;
                    if (arg.CompileReference.Line.Order == line.Order + 1) canBeRemove.Add(line);
                }
            }

            foreach (SSACompileLine line in canBeRemove)
            {
                this.SSALines.Remove(line);
            }

            return true;
        }

        private bool GenerateIRCode(List<IParseTreeNode> nodes, RequestParserTreeCompile request)
        {
            foreach (IParseTreeNode node in nodes)
            {
                if (!node.Compile(request)) this.AddError("One error orrcured: generate ir code", node);
            }

            return this.Errors.Count == 0;
        }

        private bool GenerateInheritanceDataStructure(List<IParseTreeNode> nodes)
        {
            foreach (IParseTreeNode node in nodes)
            {
                if (!(node is KlassenDeklaration k)) continue;
                if (!k.Deklaration.IsMethodsReferenceMode) continue;

                k.Deklaration.DataRef = k.compile;
                k.compile.Data = new DataObject();
                k.compile.Data.Mode = DataMode.JumpPointListe;
                k.compile.Compile(this, k, "datalist");
            }

            return this.Errors.Count == 0;
        }

        private bool DoAllocateRegisters()
        {
            foreach (CompileContainer methode in this.ContainerMgmt.Methods)
            {
                methode.DoAllocate(this);
            }

            return this.Errors.Count == 0;
        }

        private bool TranslateIRCodeToAssemblerSequence()
        {
            if (this.OutputFile != null)
                if (this.OutputFile.Exists) this.OutputFile.Delete();

            //this.AssemblerSequence.AddRange(this.DataSequence);

            try
            {
                if (this.OutputFile != null)
                    this.Writer = new StreamWriter(this.OutputFile.OpenWrite());
            }
            catch (Exception e) { return this.AddError(string.Format("The outputfile for the assembler sequence can not be created. {0}", e.Message)); }

            foreach (SSACompileLine line in this.SSALines)
            {
                if (this.IsOutputIRCode) this.AddIrLineToStream(line);

                if (!line.Owner.InFileCompilen(this)) this.AddError("One error orrcured: translate ir code to assembler");
            }

            if (this.IsOutputIRCode) this.IRCodeStream.Write("-- [EOF] --");

            foreach (ICompileRoot root in this.DataSequence)
            {
                if (!root.InFileCompilen(this)) this.AddError("One error orrcured: data sequence");
            }

            this.AssemblerSequence.AddRange(this.DataSequence);

            foreach (ICompileRoot root in this.toRemove )
            {
                this.AssemblerSequence.Remove(root);
            }

            if (this.OutputFile != null)
                this.Writer.Close();

            return this.Errors.Count == 0;
        }

        int emptyStrings = 0;
        private bool AddIrLineToStream(SSACompileLine line)
        {
            if (line.ReplaceLine != null) return true;

            StringBuilder result = new StringBuilder();

            if (line.Owner is CompileFunktionsDeklaration fd) return this.AddIrLineFunktionsDeklaration(fd);

            if (line.Owner is CompileFunktionsEnde fe) return this.AddIrLineFunktionsEnde(fe);

            if (line.Owner is CompileReferenceCall rc && rc.Node != null) return this.AddIrLineReferenceCall(rc, line);

            result.AppendFormat("{3}{0}: {1}{2}", line.Order, line.Owner.Algo.Name, line.Owner.Algo.Mode, new string(' ', emptyStrings));

            foreach (SSACompileArgument arg in line.Arguments)
            {
                if (arg.Mode == SSACompileArgumentMode.Const) result.AppendFormat(" #0x{0:x}", arg.Const);
                if (arg.Mode == SSACompileArgumentMode.Reference) result.AppendFormat(" {0}", arg.Reference.Order);
                if (arg.Mode == SSACompileArgumentMode.Variable) result.AppendFormat(" {0}", arg.Variable.Reference.Order);
                if (arg.Mode == SSACompileArgumentMode.JumpReference) result.AppendFormat(" {0}", arg.CompileReference.Line.Order);
            }

            this.IRCodeStream.WriteLine(result);

            return true;
        }

        private bool AddIrLineReferenceCall(CompileReferenceCall rc, SSACompileLine line)
        {
            string result = string.Format("{2}{0}: {1} (  )", line.Order, rc.Node.Token.Text, new string(' ', emptyStrings));

            this.IRCodeStream.WriteLine(result);

            return true;
        }

        private bool AddIrLineFunktionsEnde(CompileFunktionsEnde fe)
        {
            emptyStrings -= 4;

            this.IRCodeStream.WriteLine("}");
            this.IRCodeStream.WriteLine();

            return true;
        }

        private bool AddIrLineFunktionsDeklaration(CompileFunktionsDeklaration fd)
        {
            string result = string.Format("{0} (  )\n{{", fd.Node.Token.Text);

            this.IRCodeStream.WriteLine();
            this.IRCodeStream.WriteLine(result);

            emptyStrings += 4;

            return true;
        }

        public bool AddError(string msg, IParseTreeNode node = null)
        {
            CompilerError error = new CompilerError();
            error.Msg = msg;
            error.Use = node;

            this.Errors.Add(error);

            return false;
        }

        public bool SetNewContainer(CompileContainer compileContainer, ValidUses uses)
        {
            this.ContainerMgmt.RootContainer = compileContainer;
            this.PushContainer(compileContainer, uses);

            this.Definition.VariabelCounter = 0;

            return true;
        }

        public bool PushContainer(CompileContainer compileContainer, ValidUses uses, bool isloop = false)
        {
            this.ContainerMgmt.ContainerStack.Push(compileContainer);
            if (isloop) this.ContainerMgmt.LoopStack.Push(compileContainer);

            this.Containers.Add(compileContainer);

            if (this.ContainerMgmt.CurrentMethod != null) this.ContainerMgmt.CurrentMethod.BeginNewContainerVars();

            if (uses == null) return true;

            foreach (IParent parent in uses.Deklarationen)
            {
                if (!(parent is IndexVariabelnDeklaration dek)) continue;
                if (this.ContainerMgmt.CurrentMethod.VarMapper.ContainsKey(dek.Name)) continue;

                SSAVariableMap map = new SSAVariableMap();
                map.Key = dek.Name;
                map.Deklaration = dek;

                this.ContainerMgmt.CurrentMethod.VarMapper.Add(dek.Name, map);
            }

            return true;
        }

        public bool PopContainer()
        {
            if (this.ContainerMgmt.ContainerStack.Count == 0) return true;

            CompileContainer container = this.ContainerMgmt.ContainerStack.Pop();
            Dictionary<string, SSAVariableMap> containerMaps = this.ContainerMgmt.CurrentMethod.PopVarMap();

            bool isloop = false;

            CompileContainer loop = this.ContainerMgmt.CurrentLoop;
            if (loop != null) if (isloop = loop.Equals(container)) this.ContainerMgmt.LoopStack.Pop();

            if (this.ContainerMgmt.CurrentContainer == null) return true;

            this.ContainerMgmt.CurrentContainer.DataHolds.AddRange(container.DataHolds);

            if (this.ContainerMgmt.CurrentMethod == null) return true;

            if (containerMaps == null) return true;

            Dictionary<string, SSAVariableMap> parentVarMap = this.ContainerMgmt.CurrentMethod.VarMapper;

            foreach (KeyValuePair<string, SSAVariableMap> conMap in containerMaps)
            {
                this.PopContainerIterationContainerMap(loop, isloop, parentVarMap, conMap);
            }

            foreach (SSACompileLine line in container.PhiSetNewVar)
            {
                if (line.ReplaceLine == null) continue;
                if (line.ReplaceLine.PhiMap.Count == 1) continue;
                if (!this.CheckPhiMaps(line, line.PhiMap)) continue;

                line.ReplaceLine = null;
                if (line.Owner is CompileReferenceCall t) t.IsUsed = true;
                line.HasReturn = true;
            }

            foreach (SSACompileLine line in container.PhiSetNewVar)
            {
                if (line.ReplaceLine == null) continue;

                line.ReplaceLine.PhiMap.Add(line);
                line.ReplaceLine.Calls.AddRange(line.Calls);
            }

            return true;
        }

        private bool PopContainerIterationContainerMap(CompileContainer loop, bool isloop, Dictionary<string, SSAVariableMap> parentVarMap, KeyValuePair<string, SSAVariableMap> conMap)
        {
            if (!parentVarMap.ContainsKey(conMap.Key)) return false;
            if (conMap.Value.Reference == null) return false;

            if (isloop)
            {
                conMap.Value.Reference.LoopContainer = loop;
                conMap.Value.Reference.Calls.Add(loop.LoopLine);
            }

            if (parentVarMap[conMap.Key].Reference == null || conMap.Value.Reference.Equals(parentVarMap[conMap.Key].Reference))
            {
                parentVarMap[conMap.Key].Reference = conMap.Value.Reference;

                return false;
            }

            //if (containerMaps[orgMap.Key].Reference.ReplaceLine != null) this.CheckForCopy(containerMaps[orgMap.Key].Reference);

            SSAVariableMap orig = parentVarMap[conMap.Key];
            foreach (SSACompileLine line in conMap.Value.AllSets)
            {
                orig.Reference.PhiMap.AddRange(line.PhiMap);
                line.PhiMap.AddRange(orig.Reference.PhiMap);
                foreach (SSACompileLine phi in line.PhiMap)
                {
                    orig.Reference.Calls.AddRange(phi.Calls);
                    phi.Calls.AddRange(orig.Reference.Calls);
                }
            }

            //
            //orig.Reference.PhiMap.AddRange(conMap.Value.Reference.PhiMap);

            return true;
        }

        public bool ComeLeftBeforeRight(SSACompileLine line, SSACompileLine right)
        {
            foreach (SSACompileLine alls in this.SSALines)
            {
                if (alls.Equals(line)) return true;
                if (alls.Equals(right)) return false;
            }

            return false;
        }

        private bool CheckPhiMaps(SSACompileLine line, List<SSACompileLine> phiMap)
        {
            foreach (SSACompileLine mustcomebefore in phiMap)
            {
                if (this.ComeLeftBeforeRight(mustcomebefore, line)) continue;

                return false;
            }

            return true;
        }

        #endregion methods
    }
}