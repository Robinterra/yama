using Yama.Index;

namespace Yama.Compiler
{
    public class DefaultRegisterQuery : IRegisterQuery
    {

        public AlgoKeyCall Key
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public string Kategorie
        {
            get;
            set;
        }

        public ValidUses Uses
        {
            get;
            set;
        }

    }
}