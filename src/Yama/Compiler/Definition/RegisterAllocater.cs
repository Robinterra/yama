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

        public List<RegisterMap> VirtuellRegister
        {
            get;
            set;
        } = new List<RegisterMap>();

        public bool LastVirtuell
        {
            get;
            set;
        }

        public GenericDefinition Defintion
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
            this.Defintion = def;
            this.RegisterMaps.Clear();
            this.VirtuellRegister = new List<RegisterMap>();
            this.LastVirtuell = false;

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

                this.LastVirtuell = false;

                return map;
            }

            foreach (RegisterMap map in this.VirtuellRegister)
            {
                if (map.Mode != RegisterUseMode.Used) continue;

                if (!map.Line.FindEquals(line)) continue;

                this.CheckToFreeRegister(map, line, from);

                RegisterMap res = new RegisterMap(RegisterType.Stack, RegisterUseMode.UsedAblage);
                res.Name = this.LastVirtuell ? this.Defintion.GetRegister(this.Defintion.PlaceToKeepRegisterStart) : this.Defintion.GetRegister(this.Defintion.PlaceToKeepRegisterLast);
                res.RegisterId = map.RegisterId;
                this.LastVirtuell = !this.LastVirtuell;

                return map;
            }

            this.LastVirtuell = false;

            return null;
        }

        public bool FreeLoops(CompileContainer loopContainer, SSACompileLine line)
        {
            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode != RegisterUseMode.Used) continue;
                if (map.Line.LoopContainer == null) continue;
                if (!map.Line.LoopContainer.Equals(loopContainer)) continue;

                this.CheckLoopForFree(map, line);
                map.Mode = RegisterUseMode.Free;
            }

            return true;
        }

        private bool CheckLoopForFree(RegisterMap map, SSACompileLine line)
        {
            for (int i = 0; i < map.Line.Calls.Count; i++)
            {
                if (!line.Equals(map.Line.Calls[i])) continue;

                if (i != map.Line.Calls.Count - 1) return true;

                map.Mode = RegisterUseMode.Free;

                return true;
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

            int greatOrder = -1;

            for (int i = 0; i < checkCalls.Calls.Count; i++)
            {
                if (greatOrder < checkCalls.Calls[i].Order) greatOrder = checkCalls.Calls[i].Order;
                //if (!from.Equals(checkCalls.Calls[i])) continue;

                //if (i != checkCalls.Calls.Count - 1) return true;

                //map.Line.PhiMap.Remove(line);
                //if (map.Line.PhiMap.Count == 0) map.Mode = RegisterUseMode.Free;
            }

            if (greatOrder > from.Order) return true;

            map.Mode = RegisterUseMode.Free;

            return true;
        }

        // -----------------------------------------------

        public RegisterMap GetNextFreeRegister()
        {
            this.LastVirtuell = false;

            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode == RegisterUseMode.Free) return map;
            }

            foreach (RegisterMap map in this.VirtuellRegister)
            {
                if (map.Mode == RegisterUseMode.Free) return map;
            }

            RegisterMap mapneu = new RegisterMap(RegisterType.Stack, RegisterUseMode.Free);
            mapneu.RegisterId = this.VirtuellRegister.Count + 1;
            mapneu.Name = this.Defintion.GetRegister(this.Defintion.ResultRegister);
            this.VirtuellRegister.Add(mapneu);

            return mapneu;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------
    }
}