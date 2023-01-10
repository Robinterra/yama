using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileData : ICompileRoot
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "Data";

        public CompileAlgo? Algo
        {
            get;
            set;
        }

        public List<ICompileRoot> Calls
        {
            get;
            set;
        } = new List<ICompileRoot>();

        public string? JumpPointName
        {
            get;
            set;
        }

        public DataObject Data
        {
            get;
        }

        public List<string> AssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public IParseTreeNode? Node
        {
            get;
            set;
        }

        private bool? remember = null;
        public bool IsUsed
        {
            get
            {
                if (this.UseHandler is null) return true;
                if (this.remember is not null) return (bool)this.remember;

                this.remember = this.UseHandler.IsInUse(20);

                return (bool)this.remember;
            }
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        } = new();

        public List<string> PostAssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public IndexMethodDeklaration? UseHandler
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CompileData(DataMode mode, string? text = null)
        {
            this.Data = new DataObject(mode, text);
        }

        #endregion ctor

        #region methods

        public bool PreparationJumpPoint(Compiler compiler)
        {
            if (string.IsNullOrEmpty(this.JumpPointName)) this.JumpPointName = compiler.Definition.GenerateJumpPointName();

            return true;
        }

        public bool Compile(Compiler compiler, IParseTreeNode parent, string mode = "default")
        {
            this.Node = parent;
            compiler.DataSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            if (string.IsNullOrEmpty(this.JumpPointName)) this.JumpPointName = compiler.Definition.GenerateJumpPointName();
            if (this.Data.Mode != DataMode.Reflection) return true;
            if (this.Data.Refelection is null) return true;

            this.Data.Refelection.Compile(compiler, parent);

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            if (!this.IsUsed) return true;

            foreach (string str in this.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, false));
            }

            for (int i = 0; i < this.Algo!.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], null, new Dictionary<string, string> { { "[NAME]", this.JumpPointName! }, {"[DATA]", this.Data.GetData(compiler)!} }));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str));
            }

            return true;
        }

        #endregion methods

    }

}