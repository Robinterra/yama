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

    }
}