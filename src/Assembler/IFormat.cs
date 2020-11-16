namespace Yama.Assembler
{
    public interface IFormat
    {
        string Name
        {
            get;
        }

        bool Assemble(RequestAssembleFormat request);

        bool DisAssemble(RequestDisAssembleFormat request);
    }
}