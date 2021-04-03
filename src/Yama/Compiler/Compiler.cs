using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Index;
using Yama.Parser;

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

            if (this.Writer != null)
                this.Writer.WriteLine(request.AssemblyCode);

            if (request.AddToList) request.Root.AssemblyCommands.Add(request.AssemblyCode);

            return true;
        }

        public bool Compilen(List<IParseTreeNode> nodes)
        {
            if (this.Definition == null) return this.AddError("Keine definition zur Übersetzung in Assembler gesetzt");
            this.Definition.Compiler = this;

            this.SetNewContainer(new CompileContainer(), null);
            this.ContainerMgmt.CurrentMethod = this.ContainerMgmt.CurrentContainer;

            this.Header.Compile(this, this.MainFunction);

            this.MakeInheritance(nodes);

            if (this.Errors.Count != 0) return false;

            Parser.Request.RequestParserTreeCompile request = new Parser.Request.RequestParserTreeCompile(this);

            foreach (IParseTreeNode node in nodes)
            {
                if (!node.Compile(request)) this.AddError("Beim assembelieren ist in einer Klasse ein Fehler aufgetreten", node);
            }

            if (this.Errors.Count != 0) return false;

            this.DoAllocate();

            if (this.Errors.Count != 0) return false;

            this.RunAssmblerSequence();

            return this.Errors.Count == 0;
        }

        private bool MakeInheritance(List<IParseTreeNode> nodes)
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

            return true;
        }

        private bool DoAllocate()
        {
            foreach (CompileContainer methode in this.ContainerMgmt.Methods)
            {
                methode.DoAllocate(this);
            }

            return true;
        }

        private bool RunAssmblerSequence()
        {
            if (this.OutputFile != null)
                if (this.OutputFile.Exists) this.OutputFile.Delete();

            //this.AssemblerSequence.AddRange(this.DataSequence);

            try
            {
                if (this.OutputFile != null)
                    this.Writer = new StreamWriter(this.OutputFile.OpenWrite());
            }
            catch (Exception e) { return this.AddError(string.Format("Die Datei in der Assemblercode geschrieben werden sollte konnte nicht angelegt werden. {0}", e.Message)); }

            foreach (SSACompileLine line in this.SSALines)
            {
                if (!line.Owner.InFileCompilen(this)) this.AddError("Beim Schreiben der Assemblersequence ist ein Fehler aufgetreten");
            }

            foreach (ICompileRoot root in this.DataSequence)
            {
                if (!root.InFileCompilen(this)) this.AddError("Beim Schreiben der Assemblersequence ist ein Fehler aufgetreten");
            }

            this.AssemblerSequence.AddRange(this.DataSequence);

            foreach (ICompileRoot root in this.toRemove )
            {
                this.AssemblerSequence.Remove(root);
            }

            if (this.OutputFile != null)
                this.Writer.Close();

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
                if (!parentVarMap.ContainsKey(conMap.Key)) continue;
                if (conMap.Value.Reference == null) continue;

                if (isloop)
                {
                    conMap.Value.Reference.LoopContainer = loop;
                    conMap.Value.Reference.Calls.Add(loop.LoopLine);
                }

                if (parentVarMap[conMap.Key].Reference == null || conMap.Value.Reference.Equals(parentVarMap[conMap.Key].Reference))
                {
                    parentVarMap[conMap.Key].Reference = conMap.Value.Reference;

                    continue;
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