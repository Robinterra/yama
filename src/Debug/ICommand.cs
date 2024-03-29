using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public interface ICommand
    {

        uint CommandId
        {
            get;
        }

        bool Execute(Runtime runtime);

    }
}