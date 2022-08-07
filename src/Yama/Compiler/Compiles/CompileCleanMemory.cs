using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileCleanMemory : ICompileRoot
    {

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

                this.OwnerVarsToClear.Add(new SSAVariableMap(varmap));
            }

            if (!this.IsUsed) return true;

            this.CallDectors(compiler, returnKey);

            return true;
        }

        private bool CallDectors(Compiler compiler, ReturnKey returnKey)
        {
            foreach (SSAVariableMap map in this.OwnerVarsToClear)
            {
                if (map.Deklaration.Type.Deklaration is not IndexKlassenDeklaration ikd)
                {
                    compiler.AddError($"type of variable {map.Key} Unknown", returnKey);

                    continue;
                }
                IMethode? dector = ikd.Methods.FirstOrDefault();
                if (dector is null)
                {
                    compiler.AddError($"type '{ikd.Name} has no dector", returnKey);

                    continue;
                }

                CompileSprungPunkt sprungPunkt = new CompileSprungPunkt();

                if (map.Value == SSAVariableMap.LastValue.Unknown)
                {
                    map.OrgMap.Value = SSAVariableMap.LastValue.NotNull;

                    CompileReferenceCall referenceCallNullCheck = new CompileReferenceCall();
                    if (!referenceCallNullCheck.GetVariableCompile(compiler, map.Deklaration, returnKey)) continue;

                    CompileJumpWithCondition jumpWithCondition = new CompileJumpWithCondition();
                    jumpWithCondition.Compile(compiler, sprungPunkt, "isZero");
                }

                CompileReferenceCall referenceCall = new CompileReferenceCall();
                if (!referenceCall.GetVariableCompile(compiler, map.Deklaration, returnKey)) continue;

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(compiler, null, "copy");

                IndexVariabelnReference reference = new IndexVariabelnReference(returnKey, "~");
                reference.Deklaration = dector;

                CompileReferenceCall compileReference = new CompileReferenceCall();
                compileReference.CompilePoint0(compiler);
                CompileReferenceCall operatorCall = new CompileReferenceCall();
                operatorCall.Compile(compiler, reference, "funcref");

                CompileExecuteCall functionExecute = new CompileExecuteCall();
                functionExecute.Compile(compiler, null);

                if (map.Value == SSAVariableMap.LastValue.Unknown)
                {
                    sprungPunkt.Compile(compiler, returnKey);
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