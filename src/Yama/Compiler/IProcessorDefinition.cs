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

        List<CompileAlgo> Algos
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        bool BeginNeuRegister();

        List<string> ZielRegister(IRegisterQuery query);

        bool ParaClean();

        #endregion methods

    }

}