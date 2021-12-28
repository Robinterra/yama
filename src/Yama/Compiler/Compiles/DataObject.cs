using System.Collections.Generic;
using System.Text;

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

    }

    public enum DataMode
    {
        None,
        Text,
        JumpPointListe,
        Int
    }
}