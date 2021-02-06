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
                return this.ContainerStack.Peek();
            }
        }

        public Stack<CompileContainer> ContainerStack
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
        }

        public CompileContainer CurrentMethod
        {
            get;
            set;
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