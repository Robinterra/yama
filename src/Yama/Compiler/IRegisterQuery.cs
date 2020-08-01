using Yama.Index;

namespace Yama.Compiler
{
    public interface IRegisterQuery
    {

        string Key
        {
            get;
            set;
        }

        object Value
        {
            get;
            set;
        }

        string Kategorie
        {
            get;
            set;
        }

        ValidUses Uses
        {
            get;
            set;
        }

    }
}