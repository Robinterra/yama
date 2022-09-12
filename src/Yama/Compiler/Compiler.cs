using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Yama.Index;
using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
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

        public FileInfo? OutputFile
        {
            get;
            set;
        }

        public StreamWriter? Writer
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

        public MethodeDeclarationNode? MainFunction
        {
            get;
            set;
        }

        public List<CompilerError> Errors
        {
            get;
            set;
        } = new List<CompilerError>();

        public List<string> Defines
        {
            get;
            set;
        }

        public Optimize OptimizeLevel
        {
            get;
            set;
        }

        public List<ICompileRoot> DataSequence
        {
            get;
            set;
        } = new List<ICompileRoot>();

        public string? LastVariableCall
        {
            get;
            set;
        }

        public IndexVariabelnDeklaration? CurrentThis
        {
            get;
            set;
        }

        public IndexVariabelnDeklaration? CurrentBase
        {
            get;
            set;
        }

        public List<CompileContainer> Containers
        {
            get;
            set;
        } = new List<CompileContainer>();

        public List<SSACompileLine> SSALines
        {
            get;
            set;
        } = new List<SSACompileLine>();

        public StreamWriter? IRCodeStream
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Compiler(IProcessorDefinition definition, List<string> defines)
        {
            this.Defines = defines;
            this.Definition = definition;
        }

        #endregion ctor

        #region methods

        public CompileAlgo? GetAlgo(string algoName, string mode)
        {
            if (this.Definition.Algos is null) return null;

            CompileAlgo? result = this.Definition.Algos.FirstOrDefault(a=> a.Name == algoName && a.Mode == mode);
            if (result != null) return result;

            this.AddError(string.Format("Der Algorithmus {0} mit dem Modus {1} konnte nicht gefunden werden!", algoName, mode));

            return null;
        }

        public SSACompileLine GetLatestSSALine()
        {
            SSACompileLine? result = this.SSALines.LastOrDefault();
            if (result is null) throw new NullReferenceException();

            return result;
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
                if (this.ContainerMgmt.CurrentMethod is null) return line;
                this.ContainerMgmt.CurrentMethod.Lines.Add(line);
                this.ContainerMgmt.CurrentContainer?.Lines.Add(line);

                return line;
            }

            if (this.ContainerMgmt.CurrentContainer is null) return line;
            foreach (SSACompileLine existLine in this.ContainerMgmt.CurrentContainer.Lines)
            {
                if (!existLine.Algo.Equals(line.Algo)) continue;
                if (existLine.Arguments.Count != line.Arguments.Count) continue;
                if (!this.CheckArgumentsEqual(existLine, line)) continue;

                return existLine;
            }

            if (this.ContainerMgmt.CurrentMethod is null) return line;
            this.ContainerMgmt.CurrentMethod.Lines.Add(line);
            this.ContainerMgmt.CurrentContainer.Lines.Add(line);
            this.SSALines.Add(line);

            return line;
        }

        public bool EndCurrentMethod()
        {
            this.PopContainer();
            this.PopContainer();

            this.ContainerMgmt.CurrentMethod = null;

            return true;
        }

        private bool CheckArgumentsEqual(SSACompileLine existLine, SSACompileLine line)
        {
            int count = 0;
            foreach (SSACompileArgument argNew in line.Arguments)
            {
                SSACompileArgument orgArg = existLine.Arguments[count];

                if (argNew.Mode != orgArg.Mode) return false;
                if (argNew.Mode == SSACompileArgumentMode.Reference)
                {
                    if (argNew.Reference is null) return false;
                    if (!argNew.Reference.Equals(orgArg.Reference)) return false;
                }
            }

            return true;
        }

        public bool BeginNewMethode(List<string> registerInUse, CompileContainer compileContainer, ValidUses uses)
        {
            //if (this.ContainerMgmt.CurrentMethod != null) this.PopContainer();

            this.Definition.BeginNewMethode(registerInUse);
            this.ContainerMgmt.AddNewMethode(compileContainer);
            compileContainer.StackVarMapper.Push(new Dictionary<string, SSAVariableMap>());
            this.SetNewContainer(compileContainer, uses);
            compileContainer.RegistersUses = registerInUse;

            CompileContainer container = new CompileContainer();
            container.Begin = compileContainer.Begin;
            container.Ende = compileContainer.Ende;
            this.PushContainer(container, null);

            foreach (KeyValuePair<string, SSAVariableMap> map in compileContainer.VarMapper)
            {
                if (map.Value.IsStruct is null) continue;
                if (!map.Value.Deklaration.IsMutable) continue;
                if (map.Value.Key == "Result" && compileContainer.ReturnType?.IsStruct is not null) continue;
                if (map.Value.Key == "this") continue;

                compileContainer.VarStructs.Add(map.Value);
            }

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

        public bool Compilen(IEnumerable<ICompileNode> nodes)
        {
            if (this.Definition == null) return this.AddError("No assembler defintion for translating found");
            if (this.MainFunction is null) return false;
            this.Definition.Compiler = this;

            this.SetNewContainer(new CompileContainer(), null);
            this.ContainerMgmt.CurrentMethod = this.ContainerMgmt.CurrentContainer;

            this.Header.Compile(this, this.MainFunction);

            if (!this.GenerateInheritanceDataStructure(nodes)) return false;

            RequestParserTreeCompile request = new RequestParserTreeCompile(this);

            if (!this.GenerateIRCode(nodes, request)) return false;

            if (!this.OptimizeIRCode()) return false;

            if (!this.DoAllocateRegisters()) return false;

            return this.TranslateIRCodeToAssemblerSequence();
        }

        private bool OptimizeIRCode()
        {
            if (this.OptimizeLevel != Optimize.SSA) return false;

            for (int i = this.SSALines.Count - 1; i >= 0; i--)
            {
                SSACompileLine phi = this.SSALines[i];
                if (phi.FlowTask != ProgramFlowTask.Phi) continue;
                if (phi.IsUsed) continue;

                phi.Arguments.ForEach(t=>
                {
                    t.Reference?.RemoveCall(phi);
                });
            }

            OptimizeIRCode optimizeIRCode = new OptimizeIRCode(this.SSALines);

            return optimizeIRCode.Run();
        }

        private bool GenerateIRCode(IEnumerable<ICompileNode> nodes, RequestParserTreeCompile request)
        {
            foreach (ICompileNode node in nodes)
            {
                if (!node.Compile(request)) this.AddError("One error orrcured: generate ir code", (IParseTreeNode)node);
            }

            return this.Errors.Count == 0;
        }

        private bool GenerateInheritanceDataStructure(IEnumerable<ICompileNode> nodes)
        {
            foreach (ICompileNode node in nodes)
            {
                if (node is not KlassenDeklaration k) continue;
                if (k.Deklaration is null) continue;
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
            if (this.IRCodeStream is null) return false;
            if (this.OutputFile != null)
                if (this.OutputFile.Exists) this.OutputFile.Delete();

            //this.AssemblerSequence.AddRange(this.DataSequence);

            //if (this.IRCodeStream != null) this.PrintIr();

            try
            {
                if (this.OutputFile != null)
                    this.Writer = new StreamWriter(this.OutputFile.OpenWrite());
            }
            catch (Exception e) { return this.AddError(string.Format("The outputfile for the assembler sequence can not be created. {0}", e.Message)); }

            IRCodePrintStream iRCodePrintStream = new IRCodePrintStream(this.IRCodeStream);

            foreach (SSACompileLine line in this.SSALines)
            {
                iRCodePrintStream.Add(line);

                if (!line.Owner.InFileCompilen(this)) this.AddError("One error orrcured: translate ir code to assembler");
            }

            foreach (ICompileRoot root in this.DataSequence)
            {
                if (!root.InFileCompilen(this)) this.AddError("One error orrcured: data sequence");
            }

            this.AssemblerSequence.AddRange(this.DataSequence);

            foreach (ICompileRoot root in this.toRemove )
            {
                this.AssemblerSequence.Remove(root);
            }

            if (this.OutputFile != null && this.Writer is not null)
                this.Writer.Close();

            return this.Errors.Count == 0;
        }

        public bool AddError(string msg, IParseTreeNode? node = null)
        {
            CompilerError error = node is null ? new(new SimpleErrorOut(msg)) : new(node, msg);

            this.Errors.Add(error);

            return false;
        }

        public bool SetNewContainer(CompileContainer compileContainer, ValidUses? uses)
        {
            this.ContainerMgmt.RootContainer = compileContainer;
            this.PushContainer(compileContainer, uses);

            this.Definition.VariabelCounter = 0;

            return true;
        }

        public Dictionary<string, SSAVariableMap> GetCopyOfCurrentContext()
        {
            if (this.ContainerMgmt.CurrentMethod is null) throw new NullReferenceException();

            return this.ContainerMgmt.CurrentMethod.GetCopyOfCurrentContext();
        }

        public bool PushContainer(CompileContainer compileContainer, ValidUses? uses, bool isloop = false)
        {
            if (isloop)
            {
                CompilePhi phis = new CompilePhi();
                phis.CompileBeginLoop(this, compileContainer.CurrentNode!);

                this.ContainerMgmt.ContainerStack.Push(compileContainer);

                this.ContainerMgmt.LoopStack.Push(compileContainer);
            }
            else this.ContainerMgmt.ContainerStack.Push(compileContainer);

            this.Containers.Add(compileContainer);

            if (this.ContainerMgmt.CurrentMethod != null) this.ContainerMgmt.CurrentMethod.BeginNewKontextPath();

            if (uses is null) return true;

            foreach (IParent parent in uses.Deklarationen)
            {
                if (!(parent is IndexVariabelnDeklaration dek)) continue;
                if (this.ContainerMgmt.CurrentMethod is null) continue;

                string name = dek.Name == "return" ? "Result" : dek.Name;
                if (this.ContainerMgmt.CurrentMethod.VarMapper.ContainsKey(name)) continue;

                SSAVariableMap map = new SSAVariableMap(dek);

                this.ContainerMgmt.CurrentMethod.VarMapper.Add(name, map);
            }

            return true;
        }

        public bool PopContainer()
        {
            if (this.ContainerMgmt.ContainerStack.Count == 0) return true;

            CompileContainer container = this.ContainerMgmt.ContainerStack.Pop();
            if (this.ContainerMgmt.CurrentMethod is null) return false;

            Dictionary<string, SSAVariableMap>? containerMaps = this.ContainerMgmt.CurrentMethod.PopVarMap();

            if (this.ContainerMgmt.CurrentContainer == null) return true;

            this.ContainerMgmt.CurrentContainer.DataHolds.AddRange(container.DataHolds);

            if (this.ContainerMgmt.CurrentMethod == null) return true;

            if (containerMaps == null) return true;

            Dictionary<string, SSAVariableMap> parentVarMap = this.ContainerMgmt.CurrentMethod.VarMapper;
            if (parentVarMap.Equals(containerMaps)) return true;

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

        public bool PopContainerForLoops(List<SSACompileLine> phis)
        {
            if (this.ContainerMgmt.ContainerStack.Count == 0) return false;
            CompileContainer? loop = this.ContainerMgmt.CurrentLoop;
            if (loop is null) return true;
            if (!loop.Equals(this.ContainerMgmt.CurrentContainer)) return true;

            this.ContainerMgmt.LoopStack.Pop();

            CompileContainer container = this.ContainerMgmt.ContainerStack.Pop();
            if (container.HasReturned) return true;
            if (this.ContainerMgmt.CurrentMethod is null) return true;

            Dictionary<string, SSAVariableMap>? containerMaps = this.ContainerMgmt.CurrentMethod.PopVarMap();
            if (containerMaps is null) return true;

            SSACompileLine? firstLine = container.Lines.FirstOrDefault();
            if (firstLine is null) return true;

            Dictionary<string, SSAVariableMap> parentVarMap = this.ContainerMgmt.CurrentMethod.VarMapper;

            foreach (KeyValuePair<string, SSAVariableMap> varibaleMap in containerMaps)
            {
                if (varibaleMap.Value.TryToClean is not null) continue;
                if (varibaleMap.Value.Reference is null) continue;
                if (!parentVarMap.ContainsKey(varibaleMap.Key)) continue;

                SSAVariableMap currentVarMap = parentVarMap[varibaleMap.Key];
                //if (!currentVarMap.ArgumentsCalls.Any(t=>t.Reference is not null && t.Reference.Order > firstLine.Order) && !varibaleMap.Value.AllSets.Any(t=>t.Order > firstLine.Order)) continue;
                if (currentVarMap.Reference is null) continue;

                SSACompileLine phiLoop = currentVarMap.Reference;
                if (phiLoop.FlowTask != ProgramFlowTask.Phi) continue;

                SSACompileLine? fixReplace = container.PhiSetNewVar.FirstOrDefault(t=>t.Owner is CompileReferenceCall && t.Arguments.Any(q=>q.Reference == phiLoop));
                if (fixReplace is not null)
                {
                    fixReplace.ReplaceLine = null;
                    fixReplace.HasReturn = true;
                    ((CompileReferenceCall)fixReplace.Owner).IsUsed = true;
                }

                CompilePhi compilePhi = new CompilePhi();
                compilePhi.CompileLoopEndPhis(this, phiLoop, phis, varibaleMap.Value, currentVarMap);
            }

            return true;
        }

        public IEnumerable<KeyValuePair<string, SSAVariableMap>> PopContainerAndReturnVariableMapperForIfs()
        {
            if (this.ContainerMgmt.ContainerStack.Count == 0) yield break;

            CompileContainer container = this.ContainerMgmt.ContainerStack.Pop();
            if (container.HasReturned) yield break;
            if (this.ContainerMgmt.CurrentMethod is null) yield break;

            Dictionary<string, SSAVariableMap>? containerMaps = this.ContainerMgmt.CurrentMethod.PopVarMap();
            if (containerMaps is null) yield break;

            SSACompileLine? firstLine = container.Lines.FirstOrDefault();
            if (firstLine is null) yield break;

            foreach (KeyValuePair<string, SSAVariableMap> varibaleMap in containerMaps)
            {
                if (varibaleMap.Value.Reference is null) continue;

                varibaleMap.Value.PhiOnlyValueChecking = varibaleMap.Value.Reference.Order < firstLine.Order;

                yield return varibaleMap;
            }
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