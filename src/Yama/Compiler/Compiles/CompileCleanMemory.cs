using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileCleanMemory : ICompileRoot
    {
        private SSACompileLine line;

        #region get/set

        public List<string> AssemblyCommands
        {
            get;
            set;
        }

        public List<string> PostAssemblyCommands
        {
            get;
            set;
        }

        public IParseTreeNode? Node
        {
            get;
            set;
        }

        public bool IsUsed
        {
            get
            {
                return this.OwnerVarsToClear.Count > 0;
            }
        }

        public CompileAlgo? Algo
        {
            get;
            set;
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        }

        public List<SSAVariableMap> OwnerVarsToClear
        {
            get;
        }

        #endregion get/set

        #region ctor

        public CompileCleanMemory()
        {
            this.PrimaryKeys = new();
            this.PostAssemblyCommands = new();
            this.AssemblyCommands = new();
            this.OwnerVarsToClear = new();
            this.Algo = new CompileAlgo()
            {
                Name = "CleanMemory"
            };

            SSACompileLine line = new SSACompileLine(this);
            line.FlowTask = ProgramFlowTask.CleanMemoryEnd;
            this.line = line;
        }

        #endregion ctor

        #region methods

        public bool Compile(Compiler compiler, ReturnKey returnKey)
        {
            this.Node = returnKey;

            if (compiler.ContainerMgmt.CurrentMethod is null) return false;
            CompileContainer currentMethode = compiler.ContainerMgmt.CurrentMethod;

            foreach (KeyValuePair<string, SSAVariableMap> keyvarmap in currentMethode.VarMapper)
            {
                SSAVariableMap varmap = keyvarmap.Value;
                if (varmap.Kind != SSAVariableMap.VariableType.OwnerReference) continue;
                if (varmap.Value != SSAVariableMap.LastValue.Unknown && varmap.Value != SSAVariableMap.LastValue.NotNull) continue;
                if (varmap.First.TryToClean) continue;
                if (varmap.TryToClean) continue;

                this.OwnerVarsToClear.Add(new SSAVariableMap(varmap));
            }

            if (!this.IsUsed) return true;

            this.CallDectors(compiler, returnKey, true);

            compiler.AddSSALine(line);

            return true;
        }

        public bool Compile(Compiler compiler, IfKey ifKey, bool isInIfStatementDefined = false)
        {
            this.Node = ifKey;

            if (compiler.ContainerMgmt.CurrentMethod is null) return false;
            if (compiler.ContainerMgmt.CurrentContainer is null) return false;
            CompileContainer currentMethode = compiler.ContainerMgmt.CurrentMethod;
            CompileContainer currentContainer = compiler.ContainerMgmt.CurrentContainer;

            if (currentContainer.HasReturn) return true;
            SSACompileLine? firstLine = currentContainer.Lines.FirstOrDefault();
            if (firstLine is null) return true;

            foreach (KeyValuePair<string, SSAVariableMap> keyvarmap in currentMethode.VarMapper)
            {
                SSAVariableMap varmap = keyvarmap.Value;
                if (varmap.Kind != SSAVariableMap.VariableType.OwnerReference) continue;
                if (varmap.Value != SSAVariableMap.LastValue.Unknown && varmap.Value != SSAVariableMap.LastValue.NotNull) continue;
                if (varmap.First.TryToClean) continue;
                if (isInIfStatementDefined && !currentContainer.Deklarations.Any(t=>t.Key == varmap.Key)) continue;

                this.OwnerVarsToClear.Add(new SSAVariableMap(varmap));
            }

            if (!this.IsUsed) return true;

            this.CallDectors(compiler, ifKey);

            compiler.AddSSALine(line);

            return true;
        }

        public bool Compile(Compiler compiler, ForKey ifKey)
        {
            this.Node = ifKey;

            if (compiler.ContainerMgmt.CurrentMethod is null) return false;
            if (compiler.ContainerMgmt.CurrentLoop is null) return false;
            CompileContainer currentMethode = compiler.ContainerMgmt.CurrentMethod;
            CompileContainer currentContainer = compiler.ContainerMgmt.CurrentLoop;

            if (currentContainer.HasReturn) return true;
            SSACompileLine? firstLine = currentContainer.Lines.FirstOrDefault();
            if (firstLine is null) return true;

            foreach (KeyValuePair<string, SSAVariableMap> keyvarmap in currentMethode.VarMapper)
            {
                SSAVariableMap varmap = keyvarmap.Value;
                if (varmap.Kind != SSAVariableMap.VariableType.OwnerReference) continue;
                if (varmap.Value != SSAVariableMap.LastValue.Unknown && varmap.Value != SSAVariableMap.LastValue.NotNull) continue;
                if (varmap.TryToClean) continue;
                if (!currentContainer.Deklarations.Any(t=>t.Key == varmap.Key)) continue;

                this.OwnerVarsToClear.Add(new SSAVariableMap(varmap));
            }

            if (!this.IsUsed) return true;

            this.CallDectors(compiler, ifKey, true);

            compiler.AddSSALine(line);

            return true;
        }

        public bool Compile(Compiler compiler, MethodeDeclarationNode declarationNode, IndexVariabelnDeklaration thisVar)
        {
            if (declarationNode.Deklaration is null) return true;

            IndexKlassenDeklaration? klasse = declarationNode.Deklaration.Klasse;
            if (klasse is null) return true;

            foreach (IndexPropertyDeklaration propDek in klasse.IndexProperties)
            {
                if (propDek.Use.BorrowingToken is not null) continue;
                if (propDek.Type.Deklaration is not IndexKlassenDeklaration ikd) continue;
                if (ikd.MemberModifier != ClassMemberModifiers.None) continue;

                IMethode? dector = ikd.Methods.FirstOrDefault();
                if (dector is null)
                {
                    compiler.AddError($"type '{ikd.Name} has no dector", declarationNode);

                    continue;
                }

                CompileSprungPunkt sprungPunkt = new CompileSprungPunkt();

                CompileReferenceCall thisRef = new CompileReferenceCall();
                if (!thisRef.GetVariableCompile(compiler, thisVar, declarationNode)) continue;

                CompileReferenceCall referenceCallNullCheck = new CompileReferenceCall();
                referenceCallNullCheck.Compile(compiler, new IndexVariabelnReference(declarationNode, propDek.Name) { Deklaration = propDek }, "point");
                SSACompileArgument arg = compiler.ContainerMgmt.StackArguments.Peek();

                CompileJumpWithCondition jumpWithCondition = new CompileJumpWithCondition();
                jumpWithCondition.Compile(compiler, sprungPunkt, "isZero");

                compiler.ContainerMgmt.StackArguments.Push(arg);
                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(compiler, null, "copy");

                IndexVariabelnReference reference = new IndexVariabelnReference(declarationNode, "~");
                reference.Deklaration = dector;

                CompileReferenceCall compileReference = new CompileReferenceCall();
                compileReference.CompilePoint0(compiler);

                CompileReferenceCall operatorCall = new CompileReferenceCall();
                operatorCall.Compile(compiler, reference, "funcref");

                CompileExecuteCall functionExecute = new CompileExecuteCall();
                functionExecute.Compile(compiler, null);

                sprungPunkt.Compile(compiler, declarationNode);
            }

            return true;
        }

        public bool Compile(Compiler compiler, WhileKey ifKey)
        {
            this.Node = ifKey;

            if (compiler.ContainerMgmt.CurrentMethod is null) return false;
            if (compiler.ContainerMgmt.CurrentContainer is null) return false;
            CompileContainer currentMethode = compiler.ContainerMgmt.CurrentMethod;
            CompileContainer currentContainer = compiler.ContainerMgmt.CurrentContainer;

            if (currentContainer.HasReturn) return true;
            SSACompileLine? firstLine = currentContainer.Lines.FirstOrDefault();
            if (firstLine is null) return true;

            foreach (KeyValuePair<string, SSAVariableMap> keyvarmap in currentMethode.VarMapper)
            {
                SSAVariableMap varmap = keyvarmap.Value;
                if (varmap.Kind != SSAVariableMap.VariableType.OwnerReference) continue;
                if (varmap.Value != SSAVariableMap.LastValue.Unknown && varmap.Value != SSAVariableMap.LastValue.NotNull) continue;
                if (varmap.TryToClean) continue;
                if (!currentContainer.Deklarations.Contains(varmap)) continue;

                this.OwnerVarsToClear.Add(new SSAVariableMap(varmap));
            }

            if (!this.IsUsed) return true;

            this.CallDectors(compiler, ifKey, true);

            compiler.AddSSALine(line);

            return true;
        }

        private bool CallDectors(Compiler compiler, IParseTreeNode node, bool canBeDominated = false)
        {
            foreach (SSAVariableMap map in this.OwnerVarsToClear)
            {
                if (map.Deklaration.Type.Deklaration is not IndexKlassenDeklaration ikd)
                {
                    compiler.AddError($"type of variable {map.Key} Unknown", node);

                    continue;
                }
                IMethode? dector = ikd.Methods.FirstOrDefault();
                if (dector is null)
                {
                    compiler.AddError($"type '{ikd.Name} has no dector", node);

                    continue;
                }

                CompileSprungPunkt sprungPunkt = new CompileSprungPunkt();
                if (!canBeDominated) sprungPunkt.CleanMemoryLocation = line;
                if (!canBeDominated) sprungPunkt.CleanMemoryUseErkenner = map;

                if (map.Value == SSAVariableMap.LastValue.Unknown)
                {
                    map.OrgMap.Value = SSAVariableMap.LastValue.NotNull;

                    CompileReferenceCall referenceCallNullCheck = new CompileReferenceCall();
                    if (!canBeDominated) referenceCallNullCheck.CleanMemoryLocation = line;
                    if (!canBeDominated) referenceCallNullCheck.CleanMemoryUseErkenner = map;
                    if (!referenceCallNullCheck.GetVariableCompile(compiler, map.Deklaration, node)) continue;

                    CompileJumpWithCondition jumpWithCondition = new CompileJumpWithCondition();
                    if (!canBeDominated) jumpWithCondition.CleanMemoryLocation = line;
                    if (!canBeDominated) jumpWithCondition.CleanMemoryUseErkenner = map;
                    jumpWithCondition.Compile(compiler, sprungPunkt, "isZero");
                }

                CompileReferenceCall referenceCall = new CompileReferenceCall();
                if (!canBeDominated) referenceCall.CleanMemoryLocation = line;
                if (!canBeDominated) referenceCall.CleanMemoryUseErkenner = map;
                if (!referenceCall.GetVariableCompile(compiler, map.Deklaration, node)) continue;

                CompilePushResult compilePushResult = new CompilePushResult();
                if (!canBeDominated) compilePushResult.CleanMemoryLocation = line;
                if (!canBeDominated) compilePushResult.CleanMemoryUseErkenner = map;
                compilePushResult.Compile(compiler, null, "copy");

                IndexVariabelnReference reference = new IndexVariabelnReference(node, "~");
                reference.Deklaration = dector;

                CompileReferenceCall compileReference = new CompileReferenceCall();
                if (!canBeDominated) compileReference.CleanMemoryLocation = line;
                if (!canBeDominated) compileReference.CleanMemoryUseErkenner = map;
                compileReference.CompilePoint0(compiler);

                CompileReferenceCall operatorCall = new CompileReferenceCall();
                if (!canBeDominated) operatorCall.CleanMemoryLocation = line;
                if (!canBeDominated) operatorCall.CleanMemoryUseErkenner = map;
                operatorCall.Compile(compiler, reference, "funcref");

                CompileExecuteCall functionExecute = new CompileExecuteCall();
                if (!canBeDominated) functionExecute.CleanMemoryLocation = line;
                if (!canBeDominated) functionExecute.CleanMemoryUseErkenner = map;
                functionExecute.Compile(compiler, null);

                if (map.Value == SSAVariableMap.LastValue.Unknown)
                {
                    sprungPunkt.Compile(compiler, node);
                }

                //map.OrgMap.Value = SSAVariableMap.LastValue.NeverCall;
                //map.OrgMap.MutableState = SSAVariableMap.VariableMutableState.NotMutable;
                //map.OrgMap.Kind = SSAVariableMap.VariableType.BorrowingReference;
                if (!canBeDominated)
                {
                    map.OrgMap.First.TryToClean = true;
                    map.OrgMap.TryToClean = true;
                }
            }

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            return true;
        }

        #endregion methods
    }
}