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

        public string AddDataCall(string jumpPoint, Compiler compiler)
        {
            return this.CurrentContainer.AddDataCall(jumpPoint, compiler);
        }
    }
}