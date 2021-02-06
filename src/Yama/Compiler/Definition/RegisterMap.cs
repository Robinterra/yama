namespace Yama.Compiler.Definition
{

    public class RegisterMap
    {

        public string Name
        {
            get;
            set;
        }

        public RegisterUseMode Mode
        {
            get;
            set;
        }

        public RegisterType Type
        {
            get;
            set;
        }

        public int RegisterId
        {
            get;
            set;
        }

        public SSACompileLine Line
        {
            get;
            set;
        }

        public RegisterMap(RegisterType type, RegisterUseMode mode)
        {
            this.Type = type;
            this.Mode = mode;
        }

        public RegisterMap(string name, int id, RegisterUseMode mode)
        {
            this.Name = name;
            this.RegisterId = id;
            this.Type = RegisterType.Register;
            this.Mode = mode;
        }
    }

    public enum RegisterUseMode
    {
        Free,
        Ablage,
        UsedAblage,
        Used
    }

    public enum RegisterType
    {
        None,
        Register,

        Stack
    }
}