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

        public string? GetData()
        {
            if (this.Mode == DataMode.Int) return string.Format("0x{0:x}", this.IntValue);
            if (this.Mode == DataMode.Text) return this.Text;
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
                this.TypeArt = this.GetTypeArt(property);

                if (this.TypeArt != 3) return;
                if (property.Klasse is null) return;
                this.ClassData = property.Klasse.ReflectionData;
            }

            #endregion ctor

            public bool Compile(Compiler compiler, IParseTreeNode parent)
            {
                this.NameData.Compile(compiler, parent);

                return true;
            }

            private int GetTypeArt(IndexPropertyDeklaration property)
            {
                if (property.Klasse is null) return 0;
                if (property.Klasse.Name == "int") return 0;
                if (property.Klasse.Name == "string") return 1;
                if (property.Klasse.Name == "bool") return 2;
                if (property.Klasse.IsMethodsReferenceMode) return 3;

                return 0;
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