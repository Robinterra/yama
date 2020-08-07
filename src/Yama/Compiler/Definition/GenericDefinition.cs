using System;
using System.Linq;
using System.Collections.Generic;
using Yama.Index;

namespace Yama.Compiler.Definition
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

        public int MaxAblageRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int MaxArbeitsRegister
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

        public int ZuweisungsRegister
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

        public List<GenericDefinitionKeyPattern> KeyPatterns
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
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == "JUMPHELPER");
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", "JUMPHELPER")); return null; }

            string result = string.Format(keyPattern.Pattern, this.JumpCounter);

            this.JumpCounter++;

            return result;
        }

        // -----------------------------------------------

        public Dictionary<string,string> KeyMapping(IRegisterQuery query)
        {
            if (query.Key.Values == null) query.Key.Values = new List<string>();

            if (query.Key.Name == "[VAR]") return this.VarQuery(query);
            if (query.Key.Name == "[REG]") return this.RegisterQuery(query);
            if (query.Key.Name == "[NAME]") return this.NameQuery(query);
            if (query.Key.Name == "[REGPOP]") return this.MethodeRegPop(query);
            if (query.Key.Name == "[PARA]") return this.MethodePara(query);
            if (query.Key.Name == "[NUMCONST]") return this.MethodeNumConst(query);
            if (query.Key.Name == "[JUMPTO]") return this.JumpToQuery(query);

            return null;
        }

        // -----------------------------------------------

        public string PostKeyReplace(IRegisterQuery query)
        {
            if (query.Key.Name == "[PUSHREG]") return this.PushReg(query);
            if (query.Key.Name == "[POPREG]") return this.PopReg(query);

            return null;
        }

        // -----------------------------------------------

        #region KeyMapping

        // -----------------------------------------------

        private Dictionary<string, string> NameQuery(IRegisterQuery query)
        {
            return new Dictionary<string, string> { { query.Key.Name, query.Value.ToString() }};
        }

        // -----------------------------------------------

        private Dictionary<string,string> JumpToQuery(IRegisterQuery query)
        {
            if (!(query.Value is CompileSprungPunkt t)) return null;

            t.Add(this.Compiler, t);

            return new Dictionary<string, string> { { query.Key.Name, t.JumpPointName } };
        }

        // -----------------------------------------------

        private Dictionary<string,string> MethodeNumConst(IRegisterQuery query)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }
            string keypattern = "[NUMCONST[{0}]]";

            int wert = (int)query.Value;
            int vorlage = 0xFF;
            int pattern = 0;
            int bitcounter = 0;
            int counter = 0;

            int durations = this.AdressBytes / this.CalculationBytes;

            for (int j = 0; j < this.CalculationBytes; j++)
            {
                pattern = pattern << 8;

                pattern += vorlage;
            }

            for (int i = 0; i < durations; i++)
            {
                int resultInt = wert & pattern;

                resultInt = resultInt >> bitcounter;

                result.Add(string.Format(keypattern, counter), string.Format(keyPattern.Pattern, resultInt));

                pattern = pattern << this.CalculationBytes * 8;

                bitcounter += this.CalculationBytes * 8;
                counter++;
            }

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> MethodePara(IRegisterQuery query)
        {
            string keypattern = "[PARA[{0}]]";
            Dictionary<string, string> result = new Dictionary<string, string>();

            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            for (int i = 0; i < duration; i++ )
            {
                int registerStart = this.CurrentArbeitsRegister;
                this.CurrentArbeitsRegister += bytes;

                if (registerStart > this.MaxArbeitsRegister) { this.Compiler.AddError("Arbeitsregister voll Ausgelastet"); return null; }

                result.Add(string.Format(keypattern, i), this.AviableRegisters[registerStart]);
            }

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> MethodeRegPop(IRegisterQuery query)
        {
            string keypattern = "[REGPOP[{0}]]";
            Dictionary<string, string> result = new Dictionary<string, string>();

            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            for (int i = 0; i < duration; i++ )
            {
                this.CurrentAblageRegister -= bytes;
                int registerStart = this.CurrentAblageRegister;

                if (registerStart < this.AblageRegister) { this.Compiler.AddError("Ablageregister voll Ausgelastet"); return null; }

                result.Add(string.Format(keypattern, i), this.AviableRegisters[registerStart]);
            }

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> RegisterQuery(IRegisterQuery query)
        {
            string keypattern = "[REG[{0}]]";
            Dictionary<string,string> result = new Dictionary<string,string>();

            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            for (int i = 0; i < duration; i++ )
            {
                int registerStart = this.CurrentAblageRegister;
                this.CurrentAblageRegister += bytes;

                if (registerStart > this.MaxAblageRegister ) { this.Compiler.AddError("Ablageregister voll Ausgelastet"); return null; }

                string reg = this.AviableRegisters[registerStart];

                result.Add( string.Format(keypattern, i), reg );

                if (!this.RegisterUses.Contains(reg)) this.RegisterUses.Add(reg);
            }

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> VarQuery(IRegisterQuery query)
        {
            string keypattern = "[VAR[{0}]]";
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }
            Dictionary<string,string> result = new Dictionary<string,string>();
            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            int counter = 0;

            foreach (IParent a in query.Uses.Deklarationen)
            {
                if (!(a is IndexVariabelnDeklaration vardek)) continue;

                counter++;

                if (a.Name != query.Value.ToString()) continue;

                counter = counter - 1;
                counter = counter * bytes;

                for (int i = 0; i < duration; i++ )
                {
                    result.Add( string.Format(keypattern, i), string.Format(keyPattern.Pattern, counter + i) );
                }

                return  result;
            }

            return null;
        }

        // -----------------------------------------------

        #endregion KeyMapping

        // -----------------------------------------------

        #region PostKeys

        // -----------------------------------------------

        private string PopReg(IRegisterQuery query)
        {
            if (!(query.Value is List<string> t)) return null;

            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }
            string result = string.Empty;

            foreach(string reg in t)
            {
                result = string.Format(keyPattern.Pattern, reg, result);
            }

            return result;
        }

        // -----------------------------------------------

        private string PushReg(IRegisterQuery query)
        {
            if (!(query.Value is List<string> t)) return null;

            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }
            string result = string.Empty;

            foreach(string reg in t)
            {
                result = string.Format(keyPattern.Pattern, reg, result);
            }

            return result;
        }

        // -----------------------------------------------

        #endregion PostKeys

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}