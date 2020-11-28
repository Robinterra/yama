namespace Yama.Assembler
{
    public class Register
    {
        public Register(string name, uint binaryid)
        {
            this.Name = name;
            this.BinaryId = binaryid;
        }

        public string Name
        {
            get;
            set;
        }

        public uint BinaryId
        {
            get;
            set;
        }
    }
}