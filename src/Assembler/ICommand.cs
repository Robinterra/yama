using System.Collections.Generic;
using Yama.Compiler;

namespace Yama.Assembler
{
    public interface ICommand
    {
        string Key
        {
            get;
            set;
        }

        string Format
        {
            get;
            set;
        }

        byte[] Data
        {
            get;
            set;
        }

        ICompileRoot CompileElement
        {
            get;
            set;
        }

        bool Assemble(RequestAssembleCommand request);

        bool DisAssemble(RequestDisAssembleCommand request);

    }
}