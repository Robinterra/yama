using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public interface IProcessorDefinition
    {

        #region get/set

        string Name
        {
            get;
            set;
        }

        int CalculationBytes
        {
            get;
            set;
        }

        int AdressBytes
        {
            get;
            set;
        }

        #endregion get/set

        List<string> ZielRegister(IRegisterQuery query);
    }

}