using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler.Atmega328p
{

    public class Atmega328pDefinition : IProcessorDefinition
    {

        #region get/set

        public string Name
        {
            get;
            set;
        } = "atmega328p";

        public int CalculationBytes
        {
            get;
            set;
        } = 1;

        public int AdressBytes
        {
            get;
            set;
        } = 2;

        public int ZeroRegister
        {
            get;
            set;
        } = 0;

        public int ArbeitsRegister
        {
            get;
            set;
        } = 2;

        public int AblageRegister
        {
            get;
            set;
        } = 16;

        public int CurrentArbeitsRegister
        {
            get;
            set;
        } = 0;

        public int CurrentAblageRegister
        {
            get;
            set;
        } = 0;

        #endregion get/set

        public bool BeginNeuRegister()
        {
            this.CurrentArbeitsRegister = this.ArbeitsRegister;
            this.CurrentAblageRegister = this.AblageRegister;

            return true;
        }

        public List<string> ZielRegister(IRegisterQuery query)
        {
            if (query.Key == "[VAR]") return this.VarQuery(query);
            if (query.Key == "[REG]") return this.RegisterQuery(query);
            if (query.Key == "[METHODEREFCALL]") return this.MethodeRefCallQuery(query);
            if (query.Key == "[NAME]") return new List<string> { query.Value.ToString() };

            return null;
        }

        private List<string> MethodeRefCallQuery(IRegisterQuery query)
        {
            List<string> result = new List<string>();

            result.Add(string.Format("lo8(gs({0}))", query.Value));
            result.Add(string.Format("hi8(gs({0}))", query.Value));

            return result;
        }

        private List<string> RegisterQuery(IRegisterQuery query)
        {
            string resultPattern = "r{0}";
            List<string> result = new List<string>();

            int registerStart = this.CurrentArbeitsRegister;
            this.CurrentArbeitsRegister += this.AdressBytes;

            if (registerStart >= 26) return null;

            result.Add(string.Format(resultPattern, registerStart));
            result.Add(string.Format(resultPattern, registerStart + 1));

            return result;
        }

        private List<string> VarQuery(IRegisterQuery query)
        {
            string resultPattern = "Y+{0}";
            List<string> result = new List<string>();

            int counter = 0;

            foreach (IParent a in query.Uses.Deklarationen)
            {
                if (!(a is IndexVariabelnDeklaration vardek)) continue;

                counter++;

                if (a.Name != query.Value.ToString()) continue;

                result.Add(string.Format(resultPattern, counter * 2 - 1));
                result.Add(string.Format(resultPattern, counter * 2));
            }

            return result;
        }
    }

}