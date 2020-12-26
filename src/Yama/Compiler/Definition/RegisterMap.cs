namespace Yama.Compiler.Definition
{

    public class RegisterMap
    {

        public string Name
        {
            get;
            set;
        }

        public string VariableName
        {
            get;
            set;
        }

        public RegisterUseMode IsUsed
        {
            get;
            set;
        }
        public int RegisterId
        {
            get;
            set;
        }

        public RegisterMap(string name, int id)
        {
            this.Name = name;
            this.RegisterId = id;
        }
    }

    public enum RegisterUseMode
    {
        Free,
        Ablage,
        Temp,
        Used
    }
}