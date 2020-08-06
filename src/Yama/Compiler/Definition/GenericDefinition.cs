using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler.Atmega328p
{

    public class GenericDefinition : IProcessorDefinition
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Name
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int CalculationBytes
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int AdressBytes
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int ZeroRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int ArbeitsRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int AblageRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<CompileAlgo> Algos
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<string> AviableRegisters
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string JumpHelperPattern
        {
            get;
            set;
        }

        // -----------------------------------------------

        [System.Text.Json.Serialization.JsonIgnore]
        public int CurrentArbeitsRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        [System.Text.Json.Serialization.JsonIgnore]
        public int CurrentAblageRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        [System.Text.Json.Serialization.JsonIgnore]
        public int JumpCounter
        {
            get;
            set;
        }

        // -----------------------------------------------

        [System.Text.Json.Serialization.JsonIgnore]
        public List<string> RegisterUses
        {
            get;
            set;
        }

        // -----------------------------------------------

        [System.Text.Json.Serialization.JsonIgnore]
        public Compiler Compiler
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool BeginNeueMethode(List<string> registersUses)
        {
            this.CurrentArbeitsRegister = this.ArbeitsRegister;
            this.CurrentAblageRegister = this.AblageRegister;
            this.RegisterUses = registersUses;

            return true;
        }

        // -----------------------------------------------

        public bool ParaClean()
        {
            this.CurrentArbeitsRegister = this.ArbeitsRegister;

            return true;
        }

        // -----------------------------------------------

        public string GenerateJumpPointName()
        {
            string result = string.Format(this.JumpHelperPattern, this.JumpCounter);

            this.JumpCounter++;

            return result;
        }

        // -----------------------------------------------

        public Dictionary<string,string> KeyMapping(IRegisterQuery query)
        {
            throw new NotImplementedException();
        }

        // -----------------------------------------------

        public string PostKeyReplace(IRegisterQuery query)
        {
            //if (query.Key == "[PUSHREG]") return this.PushReg(query);
            //if (query.Key == "[POPREG]") return this.PopReg(query);

            return null;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}