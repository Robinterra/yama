namespace Yama.Assembler
{

    public interface IAssemblerDefinition
    {

        string Name
        {
            get;
        }

        Assembler SetupDefinition(Assembler assembler);

        uint GetCondition(ConditionMode condition);
    }

}