using System;
using System.Linq;
using System.Collections.Generic;
using Yama.Index;
using System.IO;
using System.Text.Json;
using System.Text;

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

        public int WorkingRegisterStart
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int PlaceToKeepRegisterStart
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int PlaceToKeepRegisterLast
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int WorkingRegisterLast
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int ResultRegister
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

        public List<AdvancedKeyReplaces> AdvancedKeyReplaces
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
        public int CurrentWorkingRegister
        {
            get;
            set;
        }

        // -----------------------------------------------

        [System.Text.Json.Serialization.JsonIgnore]
        public int CurrentPlaceToKeepRegister
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
        public int VariabelCounter
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

        [System.Text.Json.Serialization.JsonIgnore]
        public RegisterAllocater Allocater
        {
            get;
            set;
        } = new RegisterAllocater();

        // -----------------------------------------------

        public int FramePointer
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

        public int GetNextFreeRegister()
        {
            int result = this.CurrentPlaceToKeepRegister;

            if (result < this.PlaceToKeepRegisterStart) return -1;

            int bytes = this.AdressBytes / this.CalculationBytes;

            this.CurrentPlaceToKeepRegister -= bytes;

            return result;
        }

        // -----------------------------------------------

        public bool BeginNewMethode(List<string> registersUses)
        {
            this.CurrentWorkingRegister = this.WorkingRegisterStart;
            this.CurrentPlaceToKeepRegister = this.PlaceToKeepRegisterLast;
            this.RegisterUses = registersUses;
            /*this.Allocater = new RegisterAllocater();
            this.Allocater.ResultRegister = new RegisterMap(this.RegisterUses[this.ResultRegister], this.ResultRegister);

            for (int i = this.PlaceToKeepRegisterLast; i >= this.PlaceToKeepRegisterStart; i -= 1)
            {
                this.Allocater.RegisterMaps.Add(new RegisterMap(this.RegisterUses[i], i));
            }*/

            return true;
        }

        // -----------------------------------------------

        public bool ParaClean()
        {
            this.CurrentWorkingRegister = this.WorkingRegisterStart;

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
            if (query.Key.Name == "[SSAPOP]") return this.SsaPop(query);
            if (query.Key.Name == "[SSAPUSH]") return this.SsaPush(query);
            if (query.Kategorie == "funcref") return this.FuncRef(query);
            if (query.Kategorie == "setref") return this.FuncRef(query);
            if (query.Kategorie == "point0") return this.Point0(query);
            if (query.Key.Name == "[REG]") return this.RegisterQuery(query);
            if (query.Key.Name == "[NAME]") return this.NameQuery(query);
            if (query.Key.Name == "[PARA]") return this.MethodePara(query);
            if (query.Key.Name == "[NUMCONST]") return this.MethodeNumConst(query);
            if (query.Key.Name == "[JUMPTO]") return this.JumpToQuery(query);
            if (query.Key.Name == "[PROPERTY]") return this.PropertyQuery(query);
            if (query.Key.Name == "[NAMEDATACALL]") return this.NameCallQuery(query);
            if (query.Key.Name == "[DATACONTAINER]") return this.DataContainerQuery(query);

            return this.MakeAdvanedKeyReplaces(query);
        }

        // -----------------------------------------------

        private Dictionary<string, string> SsaPush(IRegisterQuery query)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!(query.Value is RequestSSAArgument request)) return result;

            SSACompileArgument arg = new SSACompileArgument(request.Target);

            this.Compiler.ContainerMgmt.StackArguments.Push(arg);

            request.Target.HasReturn = true;

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string, string> DataContainerQuery(IRegisterQuery query)
        {
            List<DataHold> dataHolds = this.Compiler.ContainerMgmt.CurrentContainer.DataHolds;

            StringBuilder builder = new StringBuilder();

            CompileAlgo algo = this.Compiler.GetAlgo("wordMarker", "default");
            if (algo == null) return null;

            bool isfirst = true;

            foreach (DataHold dataHold in dataHolds)
            {
                if (!isfirst) builder.AppendLine();

                string cmd = algo.AssemblyCommands.FirstOrDefault();

                cmd = cmd.Replace("[JUMPTO]", dataHold.JumpPoint);
                cmd = cmd.Replace("[DATA]", dataHold.DatenValue);

                builder.Append(cmd);

                isfirst = false;
            }

            return new Dictionary<string, string> { { query.Key.Name, builder.ToString() }};
        }

        // -----------------------------------------------

        private Dictionary<string, string> NameCallQuery(IRegisterQuery query)
        {
            Index.Index index = query.Uses.GetIndex;

            string pointer = this.Compiler.ContainerMgmt.AddDataCall(query.Value.ToString(), this.Compiler);

            return new Dictionary<string, string> { { query.Key.Name, pointer }};
        }

        // -----------------------------------------------

        private Dictionary<string, string> Point0(IRegisterQuery query)
        {
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }

            string keypattern = "[PROPERTY[{0}]]";
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add( string.Format(keypattern, 0), string.Format(keyPattern.Pattern, 0) );

            return result;
        }

        private Dictionary<string, string> FuncRef(IRegisterQuery query)
        {
            string keypattern = "[PROPERTY[{0}]]";
            bool isset = query.Kategorie == "setref";
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }
            Dictionary<string,string> result = new Dictionary<string,string>();
            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            int counter = 0;
            int count = 0;

            if (!(query.Value is IMethode dek)) return null;

            foreach (IMethode a in dek.Klasse.Methods)
            {
                if (a is IndexMethodDeklaration vardek) count = 1;
                if (a is IndexVektorDeklaration vekdek) count = 2;
                if (a is IndexPropertyGetSetDeklaration getsetdek) count = 2;

                counter += count;

                if (a.Name != dek.Name) continue;
                if (isset) count = 1;

                counter = counter - count;
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

        private Dictionary<string, string> MakeAdvanedKeyReplaces(IRegisterQuery query)
        {
            List<AdvancedKeyReplaces> foundedList = this.AdvancedKeyReplaces.Where(t=>t.Key == query.Key.Name).ToList();
            if (foundedList.Count == 0) return null;

            foreach (AdvancedKeyReplaces keymap in foundedList)
            {
                if (!this.AdvancedKeyReplacesIsOk(keymap)) continue;

                return new Dictionary<string, string> { { keymap.Key, keymap.Value } };
            }

            return null;
        }

        // -----------------------------------------------

        private bool AdvancedKeyReplacesIsOk(AdvancedKeyReplaces keymap)
        {
            if (keymap.Defines == null) return true;
            if (keymap.Defines.Count == 0) return true;
            if (keymap.Defines.Any(t=>this.Compiler.Defines.Contains(t))) return true;

            return false;
        }

        // -----------------------------------------------

        public string PostKeyReplace(IRegisterQuery query)
        {
            if (query.Key.Name == "[PUSHREG]") return this.PushReg(query);
            if (query.Key.Name == "[POPREG]") return this.PopReg(query);
            if (query.Key.Name == "[VARCOUNT]") return this.VarCountQuery(query);
            if (query.Key.Name == "[stackpos]") return this.StackPositionQuery(query);
            if (query.Key.Name == "[stackcount]") return this.StackCountQuery(query);
            if (query.Key.Name == "[virtuelRegister]") return this.StackCountQuery(query);

            this.Compiler.AddError("Post Key not supported");

            return null;
        }

        private string StackCountQuery(IRegisterQuery query)
        {
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == "[VARCOUNT]");
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }

            return string.Format( keyPattern.Pattern, ((int)query.Value) * this.AdressBytes );
        }

        private string StackPositionQuery(IRegisterQuery query)
        {
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == "[VARCOUNT]");
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }

            int position = (int)query.Value;
            if (this.Name == "arm-t32") position -= 1;

            return string.Format( keyPattern.Pattern, (position) * this.AdressBytes );
        }

        // -----------------------------------------------

        #region KeyMapping

        // -----------------------------------------------

        private Dictionary<string, string> NameQuery(IRegisterQuery query)
        {
            return new Dictionary<string, string> { { query.Key.Name, query.Value.ToString() }};
        }

        // -----------------------------------------------

        private Dictionary<string, string> PropertyQuery(IRegisterQuery query)
        {
            string keypattern = "[PROPERTY[{0}]]";
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }
            Dictionary<string,string> result = new Dictionary<string,string>();
            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            int counter = 0;

            if (!(query.Value is IndexPropertyDeklaration dek)) return null;

            if (dek.Klasse.IsMethodsReferenceMode) counter += 1;

            foreach (IParent a in dek.Klasse.IndexProperties)
            {
                if (!(a is IndexPropertyDeklaration vardek)) continue;

                counter++;

                if (a.Name != dek.Name) continue;

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

        private Dictionary<string,string> JumpToQuery(IRegisterQuery query)
        {
            if (query.Key.Values != null && query.Key.Values.Count > 0) return this.JumpToQueryWithValues(query);
            if (!(query.Value is CompileSprungPunkt t))
                return null;

            t.Add(this.Compiler, t);

            return new Dictionary<string, string> { { query.Key.Name, t.JumpPointName } };
        }

        // -----------------------------------------------

        private Dictionary<string, string> JumpToQueryWithValues(IRegisterQuery query)
        {
            string pattern = "[JUMPTO[{0}]]";
            int duration = Convert.ToInt32(query.Key.Values[0]);
            Dictionary<string, string> result = new Dictionary<string, string>();

            for (int i=0; i < duration; i++)
            {
                result.Add(string.Format(pattern, i), this.GenerateJumpPointName());
            }

            return result;
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
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : (this.AdressBytes / this.CalculationBytes);

            for (int i = 0; i < duration; i++ )
            {
                int registerStart = this.CurrentWorkingRegister;
                this.CurrentWorkingRegister += bytes;

                if (registerStart > this.WorkingRegisterLast)
                { this.Compiler.AddError("Arbeitsregister voll Ausgelastet"); return null; }

                string reg = this.AviableRegisters[registerStart];

                result.Add(string.Format(keypattern, i), reg);

                //if (!this.RegisterUses.Contains(reg)) this.RegisterUses.Add(reg);
            }

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> SsaPop(IRegisterQuery query)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!(query.Value is RequestSSAArgument request)) return result;

            string countStr = query.Key.Values.FirstOrDefault();
            int count = Convert.ToInt32(countStr);

            for (int i = 0; i < count; i++)
            {
                try
                {
                    SSACompileArgument arg = this.Compiler.ContainerMgmt.StackArguments.Pop();

                    request.Target.AddArgument(arg);
                }
                catch
                {
                    return result;
                }
            }

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> RegisterQuery(IRegisterQuery query)
        {
            string keypattern = "[REG[{0}]]";
            Dictionary<string,string> result = new Dictionary<string,string>();

            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : (this.AdressBytes / this.CalculationBytes);

            int einzelbyte = bytes / duration;

            this.CurrentPlaceToKeepRegister -= bytes - 1;
            for (int i = 0; i < duration; i++ )
            {
                int registerStart = this.CurrentPlaceToKeepRegister;
                this.CurrentPlaceToKeepRegister += einzelbyte;

                if (registerStart < this.PlaceToKeepRegisterStart ) { this.Compiler.AddError("Ablageregister voll Ausgelastet"); return null; }

                string reg = this.AviableRegisters[registerStart];

                result.Add( string.Format(keypattern, i), reg );

                if (!this.RegisterUses.Contains(reg)) this.RegisterUses.Add(reg);
            }
            this.CurrentPlaceToKeepRegister -= bytes + 1;

            return result;
        }

        // -----------------------------------------------

        private Dictionary<string,string> VarQuery(IRegisterQuery query)
        {
            return new Dictionary<string, string>();

            /*string keypattern = "[VAR[{0}]]";
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }
            Dictionary<string,string> result = new Dictionary<string,string>();
            int duration = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[0]) : 1;
            int bytes = query.Key.Values.Count >= 2 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            int counter = 0;
            int skip = bytes / duration;

            foreach (IParent a in query.Uses.Deklarationen)
            {
                if (!(a is IndexVariabelnDeklaration vardek)) continue;

                counter++;

                if (this.VariabelCounter < counter) this.VariabelCounter = counter;

                if (a.Name != query.Value.ToString()) continue;

                counter = counter - 1;
                counter = counter * bytes;

                for (int i = 0; i < duration; i++ )
                {
                    result.Add( string.Format(keypattern, i), string.Format(keyPattern.Pattern, counter + i + skip) );
                }

                return  result;
            }

            return null;*/
        }

        // -----------------------------------------------

        private string VarCountQuery(IRegisterQuery query)
        {
            GenericDefinitionKeyPattern keyPattern = this.KeyPatterns.FirstOrDefault(t=>t.Key == query.Key.Name);
            if (keyPattern == null) { this.Compiler.AddError(string.Format("Missing Keypattern {0}", query.Key.Name)); return null; }

            /*Dictionary<string,string> result = new Dictionary<string,string>();
            int bytes = query.Key.Values.Count >= 1 ? Convert.ToInt32(query.Key.Values[1]) : this.AdressBytes;

            int counter = 0;

            foreach (IParent a in query.Uses.Deklarationen)
            {
                if (!(a is IndexVariabelnDeklaration vardek)) continue;

                counter++;
            }*/

            return string.Format( keyPattern.Pattern, ((int)query.Value) * this.AdressBytes );
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

        public bool LoadExtensions(List<FileInfo> allFilesinUse)
        {

            foreach (FileInfo file in allFilesinUse)
            {
                FileInfo extFile = new FileInfo(Path.ChangeExtension(file.FullName, ".json"));

                if (!extFile.Exists) continue;

                if (!this.LoadExtension(extFile)) return this.Compiler.AddError(string.Format("Extension {0} konnte nicht geladen werden" , extFile.FullName));
            }

            return true;
        }

        // -----------------------------------------------

        private bool LoadExtension(FileInfo file)
        {
            if ( file.Extension != ".json" ) return false;

            List<GenericDefinition> definition = null;

            using ( FileStream stream = file.OpenRead (  ) )

            definition = JsonSerializer.DeserializeAsync<List<GenericDefinition>> ( stream ).Result;

            GenericDefinition correctDefinition = definition.FirstOrDefault ( t=>t.Name == this.Name );

            if (correctDefinition == null)
            {
                if (this.Compiler != null) return this.Compiler.AddError("Keine Extensionserweiterung für diese Definition verfügbar.");
                else return false;
            }


            this.KeyPatterns.AddRange(correctDefinition.KeyPatterns);

            this.AdvancedKeyReplaces.AddRange(correctDefinition.AdvancedKeyReplaces);

            this.Algos.AddRange(correctDefinition.Algos);

            return true;
        }

        // -----------------------------------------------

        public string GetRegister(int reg)
        {
            return this.AviableRegisters[reg];
        }

        // -----------------------------------------------

        #endregion PostKeys

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}