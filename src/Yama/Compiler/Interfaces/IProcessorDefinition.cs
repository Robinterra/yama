using System.Collections.Generic;
using System.IO;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public interface IProcessorDefinition
    {

        #region get/set

        string? Name
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

        List<CompileAlgo>? Algos
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

        Compiler? Compiler
        {
            get;
            set;
        }

        // -----------------------------------------------

        int VariabelCounter
        {
            get;
            set;
        }

        // -----------------------------------------------


        #endregion get/set

        #region methods

        bool BeginNewMethode(List<string> registersUses);

        int GetNextFreeRegister();

        string? PostKeyReplace(IRegisterQuery key);

        Dictionary<string,string>? KeyMapping(IRegisterQuery query);

        bool ParaClean();
        string? GenerateJumpPointName();
        bool LoadExtensions(List<FileInfo> allFilesinUse);
        string GetRegister(int reg);

        #endregion methods

    }

}