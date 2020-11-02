namespace Yama.Assembler
{
    public interface IFormat
    {
        string Name
        {
            get;
            set;
        }

        bool Assemble(RequestAssembleFormat request);

        bool DisAssemble(RequestDisAssembleFormat request);
    }
}