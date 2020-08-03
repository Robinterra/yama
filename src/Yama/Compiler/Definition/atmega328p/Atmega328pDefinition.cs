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

        #endregion get/set

        // -----------------------------------------------

        public Atmega328pDefinition()
        {
            this.Algos = this.CompileAlgos();
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool BeginNeuRegister()
        {
            this.CurrentArbeitsRegister = this.ArbeitsRegister;
            this.CurrentAblageRegister = this.AblageRegister;

            return true;
        }

        // -----------------------------------------------

        public bool ParaClean()
        {
            this.CurrentArbeitsRegister = this.ArbeitsRegister;

            return true;
        }

        // -----------------------------------------------

        public List<string> ZielRegister(IRegisterQuery query)
        {
            if (query.Key == "[VAR]") return this.VarQuery(query);
            if (query.Key == "[REG]") return this.RegisterQuery(query);
            if (query.Key == "[METHODEREFCALL]") return this.MethodeRefCallQuery(query);
            if (query.Key == "[NAME]") return new List<string> { query.Value.ToString() };
            if (query.Key == "[REGPOP]") return this.MethodeRegPop(query);
            if (query.Key == "[PARA]") return this.MethodePara(query);
            if (query.Key == "[NUMCONST]") return this.MethodeNumConst(query);

            return null;
        }

        // -----------------------------------------------

        private List<string> MethodeNumConst(IRegisterQuery query)
        {
            List<string> result = new List<string>();
            string string_pattern = "0x{0:X}";

            int wert = (int)query.Value;
            int pattern = 0xFF;
            int bitcounter = 0;

            for (int i = 0; i < this.AdressBytes; i++)
            {
                int resultInt = wert & pattern;

                resultInt = resultInt >> bitcounter;

                result.Add(string.Format(string_pattern, resultInt));

                pattern = pattern << 8;

                bitcounter += 8;
            }

            return result;
        }

        // -----------------------------------------------

        private List<string> MethodePara(IRegisterQuery query)
        {
            string resultPattern = "r{0}";
            List<string> result = new List<string>();

            int registerStart = this.ArbeitsRegister;
            this.ArbeitsRegister += this.AdressBytes;

            if (registerStart >= 26) return null;

            result.Add(string.Format(resultPattern, registerStart));

            return result;
        }

        // -----------------------------------------------

        private List<string> MethodeRegPop(IRegisterQuery query)
        {
            string resultPattern = "r{0}";
            List<string> result = new List<string>();

            this.AblageRegister -= this.AdressBytes;
            int registerStart = this.AblageRegister;

            if (registerStart >= 26) return null;

            result.Add(string.Format(resultPattern, registerStart));

            return result;
        }

        // -----------------------------------------------

        private List<string> MethodeRefCallQuery(IRegisterQuery query)
        {
            List<string> result = new List<string>();

            result.Add(string.Format("lo8(gs({0}))", query.Value));
            result.Add(string.Format("hi8(gs({0}))", query.Value));

            return result;
        }

        // -----------------------------------------------

        private List<string> RegisterQuery(IRegisterQuery query)
        {
            string resultPattern = "r{0}";
            List<string> result = new List<string>();

            int registerStart = this.AblageRegister;
            this.AblageRegister += this.AdressBytes;

            if (registerStart >= 26) return null;

            result.Add(string.Format(resultPattern, registerStart));
            result.Add(string.Format(resultPattern, registerStart + 1));

            return result;
        }

        // -----------------------------------------------

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
            result.AssemblyCommands.Add("ldd r24,[VAR]");
            result.AssemblyCommands.Add("ldd r25,[VAR]");

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
            result.AssemblyCommands.Add("std [VAR],r24");
            result.AssemblyCommands.Add("std [VAR],r25");

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
            result.AssemblyCommands.Add("[NAME]:");
            result.AssemblyCommands.Add("push r16");
            result.AssemblyCommands.Add("push r17");
            result.AssemblyCommands.Add("push r18");
            result.AssemblyCommands.Add("push r19");
            result.AssemblyCommands.Add("push r20");
            result.AssemblyCommands.Add("push r21");
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
            result.AssemblyCommands.Add("adiw r28, 6");
            result.AssemblyCommands.Add("in __tmp_reg__,__SREG__");
            result.AssemblyCommands.Add("cli");
            result.AssemblyCommands.Add("out __SP_H__,r29");
            result.AssemblyCommands.Add("out __SREG__,__tmp_reg__");
            result.AssemblyCommands.Add("out __SP_L__,r28");
            result.AssemblyCommands.Add("pop r29");
            result.AssemblyCommands.Add("pop r28");
            result.AssemblyCommands.Add("pop r21");
            result.AssemblyCommands.Add("pop r20");
            result.AssemblyCommands.Add("pop r19");
            result.AssemblyCommands.Add("pop r18");
            result.AssemblyCommands.Add("pop r17");
            result.AssemblyCommands.Add("pop r16");
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
            result.AssemblyCommands.Add("movw [REG], r24");

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
            result.AssemblyCommands.Add("movw [PARA],[REGPOP]");

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
            result.AssemblyCommands.Add("ldi 24,[NUMCONST]");
            result.AssemblyCommands.Add("ldi 25,[NUMCONST]");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoReferenceCallMethode()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ReferenceCall";
            result.Mode = "methode";
            result.Description = "Der aufruf einer ganz normalen Methode";
            result.Keys.Add("[METHODEREFCALL]");
            result.AssemblyCommands.Add("ldi r24,[METHODEREFCALL]");
            result.AssemblyCommands.Add("ldi r25,[METHODEREFCALL]");

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
            result.Add(this.CreateAlgoNumberConst());

            return result;
        }

        // -----------------------------------------------

        #endregion definitions

        // -----------------------------------------------
    }

}