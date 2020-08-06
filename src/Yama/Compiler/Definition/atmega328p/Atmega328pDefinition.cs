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

        // -----------------------------------------------

        public int ZeroRegister
        {
            get;
            set;
        } = 0;

        // -----------------------------------------------

        public int ArbeitsRegister
        {
            get;
            set;
        } = 2;

        // -----------------------------------------------

        public int JumpCounter
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int AblageRegister
        {
            get;
            set;
        } = 16;

        // -----------------------------------------------

        public int CurrentArbeitsRegister
        {
            get;
            set;
        } = 0;

        // -----------------------------------------------

        public int CurrentAblageRegister
        {
            get;
            set;
        } = 0;

        // -----------------------------------------------

        public List<CompileAlgo> Algos
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<string> RegisterUses
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Compiler Compiler
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Atmega328pDefinition()
        {
            this.Algos = this.CompileAlgos();
        }

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

        public string PostKeyReplace(IRegisterQuery query)
        {
            if (query.Key == "[PUSHREG]") return this.PushReg(query);
            if (query.Key == "[POPREG]") return this.PopReg(query);

            return null;
        }

        // -----------------------------------------------

        public Dictionary<string,string> KeyMapping(IRegisterQuery query)
        {
            if (query.Key == "[VAR]") return this.VarQuery(query);
            if (query.Key == "[REG]") return this.RegisterQuery(query);
            if (query.Key == "[NAME]") return new Dictionary<string, string> { { query.Key, query.Value.ToString() }};
            if (query.Key == "[REGPOP]") return this.MethodeRegPop(query);
            if (query.Key == "[PARA]") return this.MethodePara(query);
            if (query.Key == "[NUMCONST]") return this.MethodeNumConst(query);
            if (query.Key == "[JUMPTO]") return this.JumpToQuery(query);

            return null;
        }

        // -----------------------------------------------

        public string GenerateJumpPointName()
        {
            string result = string.Format("JUMPHELPER_{0}", this.JumpCounter);

            this.JumpCounter++;

            return result;
        }

        // -----------------------------------------------

        #region keymapping

        // -----------------------------------------------

        private Dictionary<string,string> JumpToQuery(IRegisterQuery query)
        {
            if (!(query.Value is CompileSprungPunkt t)) return null;

            t.Add(this.Compiler, t);

            return new Dictionary<string, string> { { query.Key, t.JumpPointName } };
        }

        // -----------------------------------------------

        private Dictionary<string,string> MethodeNumConst(IRegisterQuery query)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string string_pattern = "0x{0:X}";
            string keypattern = "[NUMCONST[{0}]]";

            int wert = (int)query.Value;
            int pattern = 0xFF;
            int bitcounter = 0;
            int counter = 0;

            for (int i = 0; i < this.AdressBytes; i++)
            {
                int resultInt = wert & pattern;

                resultInt = resultInt >> bitcounter;

                result.Add(string.Format(keypattern, counter), string.Format(string_pattern, resultInt));

                pattern = pattern << 8;

                bitcounter += 8;
                counter++;
            }

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> MethodePara(IRegisterQuery query)
        {
            string resultPattern = "r{0}";
            string keypattern = "[PARA[{0}]]";
            Dictionary<string, string> result = new Dictionary<string, string>();

            int registerStart = this.CurrentArbeitsRegister;
            this.CurrentArbeitsRegister += this.AdressBytes;

            if (registerStart >= this.AblageRegister) return null;

            result.Add(string.Format(keypattern, 0), string.Format(resultPattern, registerStart));
            result.Add(string.Format(keypattern, 1), string.Format(resultPattern, registerStart + 1));

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> MethodeRegPop(IRegisterQuery query)
        {
            string resultPattern = "r{0}";
            Dictionary<string, string> result = new Dictionary<string, string>();

            this.CurrentAblageRegister -= this.AdressBytes;
            int registerStart = this.CurrentAblageRegister;

            if (registerStart < this.ArbeitsRegister) return null;

            result.Add(query.Key ,string.Format(resultPattern, registerStart));

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> RegisterQuery(IRegisterQuery query)
        {
            string resultPattern = "r{0}";
            Dictionary<string,string> result = new Dictionary<string,string>();

            int registerStart = this.CurrentAblageRegister;
            this.CurrentAblageRegister += this.AdressBytes;

            if (registerStart >= 24) return null;

            string regOne = string.Format(resultPattern, registerStart);
            string regTwo = string.Format(resultPattern, registerStart + 1);

            result.Add( "[REG[0]]", regOne );
            result.Add( "[REG[1]]", regTwo );

            if (!this.RegisterUses.Contains(regOne)) this.RegisterUses.Add(regOne);
            if (!this.RegisterUses.Contains(regTwo)) this.RegisterUses.Add(regTwo);

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> VarQuery(IRegisterQuery query)
        {
            string resultPattern = "Y+{0}";
            Dictionary<string,string> result = new Dictionary<string,string>();

            int counter = 0;

            foreach (IParent a in query.Uses.Deklarationen)
            {
                if (!(a is IndexVariabelnDeklaration vardek)) continue;

                counter++;

                if (a.Name != query.Value.ToString()) continue;

                result.Add( "[VAR[0]]", string.Format(resultPattern, counter * 2 - 1) );
                result.Add( "[VAR[1]]", string.Format(resultPattern, counter * 2) );

                return  result;
            }

            return null;
        }

        // -----------------------------------------------

        #endregion keymapping

        // -----------------------------------------------

        #region postkeyreplace

        // -----------------------------------------------


        private string PopReg(IRegisterQuery query)
        {
            if (!(query.Value is List<string> t)) return null;

            string pattern = "POP {0}\n{1}";
            string result = string.Empty;

            foreach(string reg in t)
            {
                result = string.Format(pattern, reg, result);
            }

            return result;
        }

        // -----------------------------------------------

        private string PushReg(IRegisterQuery query)
        {
            if (!(query.Value is List<string> t)) return null;

            string pattern = "{1}PUSH {0}\n";
            string result = string.Empty;

            foreach(string reg in t)
            {
                result = string.Format(pattern, reg, result);
            }

            return result;
        }

        // -----------------------------------------------

        #endregion postkeyreplace

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

        #region definitions

        // -----------------------------------------------

        private CompileAlgo CreateAlgoReferenceCallGet()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ReferenceCall";
            result.Mode = "default";
            result.Description = "Der aufruf einer ganz normalen Variabel";
            result.Keys.Add("[VAR]");
            result.AssemblyCommands.Add("ldd r24,[VAR[0]]");
            result.AssemblyCommands.Add("ldd r25,[VAR[1]]");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoHeader()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "CompileHeader";
            result.Mode = "default";
            result.Description = "Das setzen des Headers";
            result.AssemblyCommands.Add("__SP_H__ = 0x3e");
            result.AssemblyCommands.Add( "__SP_L__ = 0x3d");
            result.AssemblyCommands.Add("__SREG__ = 0x3f");
            result.AssemblyCommands.Add("__tmp_reg__ = 0");
            result.AssemblyCommands.Add("__zero_reg__ = 1");
            result.AssemblyCommands.Add(".global    main");
            result.AssemblyCommands.Add(".type main, @function");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoReferenceCallSet()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ReferenceCall";
            result.Mode = "set";
            result.Description = "Das setzen einer ganz normalen Variabel";
            result.Keys.Add("[VAR]");
            result.AssemblyCommands.Add("std [VAR[0]],r24");
            result.AssemblyCommands.Add("std [VAR[1]],r25");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoExecutionCall()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ExecuteCall";
            result.Mode = "default";
            result.Description = "Aufruf einer Funktion";
            result.AssemblyCommands.Add("movw r30, r24");
            result.AssemblyCommands.Add("icall");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoFunktionsDeklaration()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "FunktionsDeklaration";
            result.Mode = "default";
            result.Description = "Die Deklaration einer Funktion";
            result.Keys.Add("[NAME]");
            result.PostKeys.Add("[PUSHREG]");
            result.AssemblyCommands.Add("[NAME]:");
            result.AssemblyCommands.Add("[PUSHREG]");
            result.AssemblyCommands.Add("push r28");
            result.AssemblyCommands.Add("push r29");
            result.AssemblyCommands.Add("in r28,__SP_L__");
            result.AssemblyCommands.Add("in r29,__SP_H__");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoFunktionsEnde()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "FunktionsEnde";
            result.Mode = "default";
            result.Description = "Das ende einer Funktion";
            result.PostKeys.Add("[POPREG]");
            result.AssemblyCommands.Add("adiw r28, 6");
            result.AssemblyCommands.Add("in __tmp_reg__,__SREG__");
            result.AssemblyCommands.Add("cli");
            result.AssemblyCommands.Add("out __SP_H__,r29");
            result.AssemblyCommands.Add("out __SREG__,__tmp_reg__");
            result.AssemblyCommands.Add("out __SP_L__,r28");
            result.AssemblyCommands.Add("pop r29");
            result.AssemblyCommands.Add("pop r28");
            result.AssemblyCommands.Add("[POPREG]");
            result.AssemblyCommands.Add("ret");

            return result;
        }

        // -----------------------------------------------
        private CompileAlgo CreateAlgoMoveResult()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "MovResult";
            result.Mode = "default";
            result.Description = "Das verschieben eines Ergebnisses";
            result.Keys.Add("[REG]");
            result.AssemblyCommands.Add("movw [REG[0]], r24");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoUsePara()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "UsePara";
            result.Mode = "default";
            result.Description = "Ein Registry Cache Entry als Parameter nutzen";
            result.Keys.Add("[REGPOP]");
            result.Keys.Add("[PARA]");
            result.AssemblyCommands.Add("movw [PARA[0]],[REGPOP]");

            return result;
        }

        // -----------------------------------------------

        
        private CompileAlgo CreateAlgoUseParaGet()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "UsePara";
            result.Mode = "get";
            result.Description = "Ein Parameter in seine Variabel laden";
            result.Keys.Add("[VAR]");
            result.Keys.Add("[PARA]");
            result.AssemblyCommands.Add("std [VAR[0]],[PARA[0]]");
            result.AssemblyCommands.Add("std [VAR[1]],[PARA[1]]");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoNumberConst()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "NumConst";
            result.Mode = "default";
            result.Description = "Die Konstante in das Register laden";
            result.Keys.Add("[NUMCONST]");
            result.AssemblyCommands.Add("ldi r24,[NUMCONST[0]]");
            result.AssemblyCommands.Add("ldi r25,[NUMCONST[1]]");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoReferenceCallMethode()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ReferenceCall";
            result.Mode = "methode";
            result.Description = "Der aufruf einer ganz normalen Methode";
            result.Keys.Add("[NAME]");
            result.AssemblyCommands.Add("ldi r24,hi8(gs([NAME]))");
            result.AssemblyCommands.Add("ldi r25,lo8(gs([NAME]))");

            return result;
        }

        // -----------------------------------------------
        
        private CompileAlgo CreateAlgoSprungPunkt()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "SprungPunkt";
            result.Mode = "default";
            result.Description = "Das setzen eines sprungounktes";
            result.AssemblyCommands.Add("[NAME]:");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoJumpToDefault()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "JumpTo";
            result.Mode = "default";
            result.Description = "Springt zum ziel punkt";
            result.Keys.Add("[JUMPTO]");
            result.AssemblyCommands.Add("rjmp [JUMPTO]");

            return result;
        }

        // -----------------------------------------------
        private List<CompileAlgo> CompileAlgos()
        {
            List<CompileAlgo> result = new List<CompileAlgo>();

            result.Add(this.CreateAlgoReferenceCallGet());
            result.Add(this.CreateAlgoReferenceCallMethode());
            result.Add(this.CreateAlgoReferenceCallSet());
            result.Add(this.CreateAlgoMoveResult());
            result.Add(this.CreateAlgoExecutionCall());
            result.Add(this.CreateAlgoFunktionsDeklaration());
            result.Add(this.CreateAlgoFunktionsEnde());
            result.Add(this.CreateAlgoUsePara());
            result.Add(this.CreateAlgoUseParaGet());
            result.Add(this.CreateAlgoNumberConst());
            result.Add(this.CreateAlgoHeader());
            result.Add(this.CreateAlgoJumpToDefault());
            result.Add(this.CreateAlgoSprungPunkt());

            return result;
        }

        // -----------------------------------------------

        #endregion definitions

        // -----------------------------------------------
    }

}