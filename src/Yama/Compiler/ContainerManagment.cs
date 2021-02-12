using System;
using System.Collections.Generic;

namespace Yama.Compiler
{
    public class ContainerManagment
    {

        public CompileContainer RootContainer
        {
            get;
            set;
        }

        public CompileContainer CurrentContainer
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

        public CompileContainer CurrentMethod
        {
            get;
            set;
        }

        public CompileContainer CurrentLoop
        {
            get
            {
                if (this.LoopStack.Count == 0) return null;

                return this.LoopStack.Peek();
            }
        }

        public string AddDataCall(string jumpPoint, Compiler compiler)
        {
            return this.CurrentContainer.AddDataCall(jumpPoint, compiler);
        }

        public bool AddNewMethode(CompileContainer compileContainer)
        {
            this.Methods.Add(compileContainer);
            this.CurrentMethod = compileContainer;
            this.StackArguments.Clear();

            return true;
        }
    }
}