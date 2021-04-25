using Yama.Parser;

namespace Yama.Index
{
    public interface IIndexTypeSafety
    {

        bool CheckExecute ( RequestTypeSafety request );

    }
}