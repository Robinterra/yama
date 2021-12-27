using System;
using System.Collections.Generic;

namespace Yama.Compiler
{
    public class ContainerManagment
    {

        #region get/set

        public CompileContainer? RootContainer
        {
            get;
            set;
        }

        public CompileContainer? CurrentContainer
        {
            get
            {
                if (this.ContainerStack.Count == 0) return null;

                return this.ContainerStack.Peek();
            }
        }

        public Stack<CompileContainer> ContainerStack
        {
            get;
            set;
        } = new Stack<CompileContainer>();

        public Stack<CompileContainer> LoopStack
        {
            get;
            set;
        } = new Stack<CompileContainer>();

        public Stack<SSACompileArgument> StackArguments
        {
            get;
            set;
        } = new Stack<SSACompileArgument>();

        public List<CompileContainer> Methods
        {
            get;
            set;
        } = new List<CompileContainer>();

        public CompileContainer? CurrentMethod
        {
            get;
            set;
        }

        public CompileContainer? CurrentLoop
        {
            get
            {
                if (this.LoopStack.Count == 0) return null;

                return this.LoopStack.Peek();
            }
        }

        #endregion get/set

        #region methods

        public string AddDataCall(string jumpPoint, Compiler compiler)
        {
            if (this.CurrentContainer is null) return string.Empty;

            return this.CurrentContainer.AddDataCall(jumpPoint, compiler);
        }

        public bool AddNewMethode(CompileContainer compileContainer)
        {
            this.Methods.Add(compileContainer);
            this.CurrentMethod = compileContainer;
            this.StackArguments.Clear();

            return true;
        }

        #endregion methods

    }
}