using System;
using System.Collections.Generic;
using System.Linq;

namespace Yama.Compiler.Definition
{

    public class RegisterAllocater
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------
        public List<RegisterMap> RegisterMaps
        {
            get;
            set;
        } = new List<RegisterMap>();

        // -----------------------------------------------

        /*public RegisterMap ResultRegister
        {
            get;
            set;
        }*/

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

        public GenericDefinition? Defintion
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

            return true;
        }

        // -----------------------------------------------

        public RegisterMap? GetReferenceRegister(SSACompileLine? line, SSACompileLine from)
        {
            if (from == line && this.CheckLastSet(from)) return from.LastSet!.RegisterMap;
            if (this.Defintion is null) return null;
            if (line is null) return null;

            line = this.GetOriginal(line);
            if (line is null) return null;

            if (line.RegisterMap is not null && line.RegisterMap.Line is not null)
            {
                if (line.Equals(line.RegisterMap.Line)) return line.RegisterMap;
            }
            SSACompileLine? phimap = line.PhiMap.Find(t=>t.RegisterMap != null);
            if (phimap != null && phimap.RegisterMap!.Line!.FindEquals(line)) return phimap.RegisterMap;

            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode != RegisterUseMode.Used) continue;
                if (map.Line is null) continue;
                if (!map.Line.FindEquals(line)) continue;

                this.CheckToFreeRegister(map, line, from);

                this.LastVirtuell = false;

                return map;
            }

            foreach (RegisterMap map in this.VirtuellRegister)
            {
                if (map.Mode != RegisterUseMode.Used) continue;
                if (map.Line is null) continue;
                if (!map.Line.FindEquals(line)) continue;

                this.CheckToFreeRegister(map, line, from);

                string name = this.LastVirtuell ? this.Defintion.GetRegister(this.Defintion.PlaceToKeepRegisterStart) : this.Defintion.GetRegister(this.Defintion.PlaceToKeepRegisterLast);
                RegisterMap res = new RegisterMap(name, map.RegisterId, RegisterType.Stack, RegisterUseMode.UsedAblage);
                res.Line = map.Line;
                this.LastVirtuell = !this.LastVirtuell;

                return res;
            }

            this.LastVirtuell = false;

            return null;
        }

        public bool FreeLoops(CompileContainer loopContainer, SSACompileLine line)
        {
            if (line.Owner is not CompileFreeLoop freeLoop) return false;

            foreach (SSACompileLine phi in freeLoop.Phis)
            {
                SSACompileArgument? phiArg = phi.Arguments.FirstOrDefault(t=>t.Reference is not null && t.Reference.RegisterMap is not null);
                if (phiArg is null) continue;
                if (phiArg.Reference!.RegisterMap!.Mode != RegisterUseMode.Used) continue;
                if (phiArg.Reference.RegisterMap.Line != phiArg.Reference) continue;

                phiArg.Reference.RegisterMap.Line = phi;

                if (phiArg.Reference.Order < phi.GreateOrder) continue;

                phiArg.Reference.RegisterMap.Mode = RegisterUseMode.Free;
            }

            return true;
        }

        private SSACompileLine? GetOriginal(SSACompileLine line)
        {
            if (line == null) return null;
            if (line.ReplaceLine == null) return line;

            return this.GetOriginal(line.ReplaceLine);
        }

        private bool CheckLastSet(SSACompileLine line)
        {
            SSACompileLine? lastSet = line.LastSet;
            if (lastSet is null) return false;

            RegisterMap? map = lastSet.RegisterMap;
            if (map is null) return false;

            if (map.Mode != RegisterUseMode.Used) return false;

            return map.Line == lastSet;
        }

        public bool ExistAllocation(SSACompileLine line)
        {
            if (this.CheckLastSet(line)) return true;

            SSACompileLine? phimap = line.PhiMap.Find(t=>t.RegisterMap != null);
            if (phimap != null && phimap.RegisterMap!.Line!.FindEquals(line)) return true;

            return false;
        }

        // -----------------------------------------------

        private bool CheckToFreeRegister(RegisterMap map, SSACompileLine line, SSACompileLine from)
        {
            if (this.HasALoopContainer(map)) return true;
            if (map.Line is null) return false;

            SSACompileLine checkCalls = map.Line;
            if (!checkCalls.Equals(line)) checkCalls = line;

            if (checkCalls.Equals(from))
            {
                if (checkCalls.Calls.Count == 0) map.Line.PhiMap.Remove(line);
                if (map.Line.PhiMap.Count == 0) map.Mode = RegisterUseMode.Free;

                return true;
            }

            int greatOrder = checkCalls.GreateOrder;

            if (greatOrder > from.Order) return true;

            map.Mode = RegisterUseMode.Free;

            return true;
        }

        private bool HasALoopContainer(RegisterMap map)
        {
            if (map.Line is null) return false;

            if (map.Line.LoopContainer != null && map.Line.LoopContainer.LoopLine is not null) return this.IsUsedAfterBeginOfLoop(map.Line.LoopContainer.LoopLine, map.Line.Calls);

            foreach (SSACompileLine line in map.Line.PhiMap)
            {
                if (line.LoopContainer != null) return true;
            }

            return false;
        }

        // -----------------------------------------------

        private bool IsUsedAfterBeginOfLoop(SSACompileLine loopLine, List<SSACompileLine> calls)
        {
            if (calls.Count == 0) return false;

            SSACompileArgument? arg = loopLine.Arguments.FirstOrDefault();
            if (arg == null) return true;
            if (arg.Reference is null) return false;

            List<SSACompileLine> find = calls.Where(t=>t.Order > arg.Reference.Order).ToList();
            if (find.Count > 1) return true;

            SSACompileLine? lastMatch = find.FirstOrDefault();
            if (lastMatch == null) return false;
            if (lastMatch.Order != loopLine.Order) return true;

            calls.Remove(lastMatch);

            return false;
        }

        // -----------------------------------------------

        public RegisterMap GetNextFreeRegister(SSACompileLine line)
        {
            this.LastVirtuell = false;

            this.MakeUnusedToFree(line);

            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode == RegisterUseMode.Free) return map;
            }

            foreach (RegisterMap map in this.VirtuellRegister)
            {
                if (map.Mode == RegisterUseMode.Free) return map;
            }

            string name = this.Defintion!.GetRegister(this.Defintion.ResultRegister);
            int id = this.VirtuellRegister.Count + 1;
            RegisterMap mapneu = new RegisterMap(name, id, RegisterType.Stack, RegisterUseMode.Free);

            this.VirtuellRegister.Add(mapneu);

            return mapneu;
        }

        private bool MakeUnusedToFree(SSACompileLine line)
        {
            foreach (RegisterMap map in this.RegisterMaps)
            {
                if (map.Mode != RegisterUseMode.Used) continue;
                if (map.Line is null) continue;

                this.CheckToFreeRegister(map, map.Line, line);
            }

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------
    }
}