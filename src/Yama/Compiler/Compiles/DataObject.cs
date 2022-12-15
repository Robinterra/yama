using System.Collections.Generic;
using System.Text;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{
    public class DataObject
    {

        public List<string> JumpPoints
        {
            get;
            set;
        } = new List<string>();

        public DataMode Mode
        {
            get;
            set;
        }

        public RefelectionData? Refelection
        {
            get;
            set;
        }

        public int IntValue
        {
            get;
            set;
        }

        public string? Text
        {
            get;
            set;
        }

        public DataObject(DataMode mode, string? text = null)
        {
            if (text is not null) this.Text = $"\"{text}\"";
            this.Mode = mode;
        }

        public string? GetData(Compiler compiler)
        {
            if (this.Mode == DataMode.Int) return string.Format("0x{0:x}", this.IntValue);
            if (this.Mode == DataMode.Text) return this.Text;
            if (this.Mode == DataMode.Reflection && this.Refelection is not null) return this.Refelection.GetData(compiler);
            if (this.Mode != DataMode.JumpPointListe) return null;

            StringBuilder builder = new StringBuilder();

            bool isfirst = true;
            foreach ( string data in this.JumpPoints )
            {
                if (!isfirst) builder.Append(",");
                isfirst = false;
                builder.Append(data);
            }

            return builder.ToString();
        }

        public class RefelectionData
        {

            #region get/set

            public string Name
            {
                get;
            }

            public CompileData NameData
            {
                get;
            }

            public CompileData? VirtuelClassData
            {
                get;
                set;
            }

            public IndexMethodDeklaration? EmptyCtor
            {
                get;
                set;
            }

            public List<ReflectionProperty> Properties
            {
                get;
            }

            #endregion get/set

            #region ctor

            public RefelectionData(string name)
            {
                this.Name = name;
                this.Properties = new List<ReflectionProperty>();
                this.NameData = new CompileData(DataMode.Text, name);
            }

            #endregion ctor

            #region methods

            public bool Compile(Compiler compiler, IParseTreeNode parent)
            {
                this.NameData.Compile(compiler, parent);

                foreach (ReflectionProperty property in this.Properties)
                {
                    property.Compile(compiler, parent);
                }

                return true;
            }

            public bool AddProperty(IndexPropertyDeklaration property)
            {
                this.Properties.Add(new ReflectionProperty(property));

                return true;
            }

            public string? GetData(Compiler compiler)
            {
                if (this.VirtuelClassData is null) return null;
                if (this.EmptyCtor is null) 
                {
                    //compiler.AddError("Expectet a Empty Ctor", this.VirtuelClassData.Node);
                    return null;
                }

                StringBuilder builder = new StringBuilder();
                builder.Append($"{this.VirtuelClassData.JumpPointName},");
                builder.Append($"{this.NameData.JumpPointName},");
                builder.Append($"{this.EmptyCtor.AssemblyName},");
                builder.Append($"{this.Properties.Count}");

                int count = 1;
                foreach (ReflectionProperty refelectionData in this.Properties)
                {
                    refelectionData.GetData(builder, count);

                    count = count + 1;
                }

                return builder.ToString();
            }

            #endregion methods

        }

        public class ReflectionProperty
        {

            #region get/set

            public string Name
            {
                get;
            }

            public int Position
            {
                get;
            }

            public int TypeArt
            {
                get;
            }

            public CompileData NameData
            {
                get;
            }

            public CompileData? ClassData
            {
                get;
            }

            #endregion get/set

            #region ctor

            public ReflectionProperty(IndexPropertyDeklaration property)
            {

                this.Name = property.Name;
                this.NameData = new CompileData(DataMode.Text, this.Name);
                this.Position = property.Position;

                if (property.Type is null) return;
                if (property.Type.Deklaration is not IndexKlassenDeklaration ikd) return;
                this.TypeArt = this.GetTypeArt(ikd);

                if (this.TypeArt != 3) return;
                this.ClassData = ikd.ReflectionData;
            }

            #endregion ctor

            public bool Compile(Compiler compiler, IParseTreeNode parent)
            {
                this.NameData.Compile(compiler, parent);

                return true;
            }

            private int GetTypeArt(IndexKlassenDeklaration klasse)
            {
                if (klasse.Name == "int") return 0;
                if (klasse.Name == "String") return 1;
                if (klasse.Name == "List") return 4;
                if (klasse.Name == "bool") return 2;
                if (klasse.IsMethodsReferenceMode) return 3;

                return 0;
            }

            public bool GetData(StringBuilder builder, int count)
            {
                builder.Append($",{this.NameData.JumpPointName}");
                builder.Append($",{this.Position}");
                builder.Append($",{this.TypeArt}");
                builder.Append(this.ClassData is null ? ",0" : $",{this.ClassData.JumpPointName}");

                return true;
            }
        }

    }

    public enum DataMode
    {
        None,
        Text,
        JumpPointListe,
        Int,
        Reflection
    }
}