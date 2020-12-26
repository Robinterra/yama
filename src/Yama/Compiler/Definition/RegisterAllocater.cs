using System.Collections.Generic;

namespace Yama.Compiler.Definition
{

    public class RegisterAllocater
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public Stack<VarCallRegister> VarCallStack
        {
            get;
            set;
        } = new Stack<VarCallRegister>();

        // -----------------------------------------------

        public List<RegisterMap> RegisterMaps
        {
            get;
            set;
        } = new List<RegisterMap>();

        // -----------------------------------------------

        public RegisterMap ResultRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public RegisterMap GetNextFreeOrVariableUseRegister(string variablename)
        {
            for (int i = this.RegisterMaps.Count - 1; i >= 0; i -= 1)
            {
                RegisterMap map = this.RegisterMaps[i];
                if (map.IsUsed == RegisterUseMode.Free) return map;
                if (map.IsUsed != RegisterUseMode.Ablage) return null;
                if (map.VariableName == variablename) return map;
            }

            return null;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------
    }
}