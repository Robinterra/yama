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
            this.RegisterMaps.Clear();

            for (int i = def.WorkingRegisterStart; i <= def.WorkingRegisterLast; i++)
            {
                RegisterMap map = new RegisterMap(def.GetRegister(i), i, RegisterUseMode.Free);

                this.RegisterMaps.Add(map);
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
            line = this.GetOriginal(line);

            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode != RegisterUseMode.Used) continue;

                if (!map.Line.FindEquals(line)) continue;

                this.CheckToFreeRegister(map, line, from);

                return map;
            }

            return null;
        }

        public bool FreeLoops(CompileContainer loopContainer)
        {
            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode != RegisterUseMode.Used) continue;
                if (map.Line.LoopContainer == null) continue;
                if (!map.Line.LoopContainer.Equals(loopContainer)) continue;

                map.Mode = RegisterUseMode.Free;
            }

            return true;
        }

        private SSACompileLine GetOriginal(SSACompileLine line)
        {
            if (line.ReplaceLine == null) return line;

            return this.GetOriginal(line.ReplaceLine);
        }

        public bool ExistAllocation(SSACompileLine line)
        {
            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode != RegisterUseMode.Used) continue;

                if (map.Line.FindEquals(line)) return true;
            }

            return false;
        }

        // -----------------------------------------------

        private bool CheckToFreeRegister(RegisterMap map, SSACompileLine line, SSACompileLine from)
        {
            if (map.Line.LoopContainer != null) return true;

            SSACompileLine checkCalls = map.Line;
            if (!checkCalls.Equals(line)) checkCalls = line;

            if (checkCalls.Equals(from))
            {
                if (checkCalls.Calls.Count == 0) map.Line.PhiMap.Remove(line);
                if (map.Line.PhiMap.Count == 0) map.Mode = RegisterUseMode.Free;

                return true;
            }

            for (int i = 0; i < checkCalls.Calls.Count; i++)
            {
                if (!from.Equals(checkCalls.Calls[i])) continue;

                if (i != checkCalls.Calls.Count - 1) return true;

                map.Line.PhiMap.Remove(line);
                if (map.Line.PhiMap.Count == 0) map.Mode = RegisterUseMode.Free;

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