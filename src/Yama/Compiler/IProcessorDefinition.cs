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

        List<string> RegisterUses
        {
            get;
            set;
        }

        // -----------------------------------------------

        Compiler Compiler
        {
            get;
            set;
        }

        // -----------------------------------------------


        #endregion get/set

        #region methods

        bool BeginNeueMethode(List<string> registersUses);

        string PostKeyReplace(IRegisterQuery key);

        Dictionary<string,string> KeyMapping(IRegisterQuery query);

        bool ParaClean();
        string GenerateJumpPointName();

        #endregion methods

    }

}