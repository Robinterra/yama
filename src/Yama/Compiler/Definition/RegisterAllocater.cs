using System;
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

        public bool Init(GenericDefinition def)
        {
            for (int i = def.WorkingRegisterStart; i <= def.WorkingRegisterLast; i++)
            {
                RegisterMap map = new RegisterMap(def.GetRegister(i), i, RegisterUseMode.Free);
            }

            /*for (int i = def.PlaceToKeepRegisterStart; i <= def.PlaceToKeepRegisterLast; i++)
            {
                RegisterMap map = new RegisterMap(def.GetRegister(i), i, RegisterUseMode.Ablage);
            }*/

            /*for (int i = 0; i <= 5; i++)
            {
                RegisterMap map = new RegisterMap(RegisterType.Stack, RegisterUseMode.Free);
            }*/

            return true;
        }

        // -----------------------------------------------

        public RegisterMap GetReferenceRegister(SSACompileLine line, SSACompileLine from)
        {
            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode != RegisterUseMode.Used) continue;

                if (!line.Equals(map.Line)) continue;

                this.CheckToFreeRegister(map, from);

                return map;
            }

            return null;
        }

        // -----------------------------------------------

        private bool CheckToFreeRegister(RegisterMap map, SSACompileLine from)
        {
            for (int i = 0; i < map.Line.Calls.Count; i++)
            {
                if (!from.Equals(map.Line.Calls[i])) continue;

                if (i == map.Line.Calls.Count - 1) map.Mode = RegisterUseMode.Free;

                return true;
            }

            return false;
        }

        // -----------------------------------------------

        public RegisterMap GetNextFreeRegister()
        {
            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode == RegisterUseMode.Free) return map;
            }

            return null;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------
    }
}