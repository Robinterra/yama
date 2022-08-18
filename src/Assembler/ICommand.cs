using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler
{
    public interface ICommand
    {
        string Key
        {
            get;
        }

        /*string Format
        {
            get;
        }*/

        uint CommandId
        {
            get;
            set;
        }

        int Size
        {
            get;
            set;
        }

        byte[] Data
        {
            get;
            set;
        }

        IParseTreeNode Node
        {
            get;
            set;
        }

        bool Assemble(RequestAssembleCommand request);

        bool Identify(RequestIdentify request);
        bool DisAssemble(RequestDisAssembleCommand request);

    }
}