using Yama.Parser;

namespace Yama.Compiler
{
    public class CompilerError
    {

        public IParseTreeNode Use
        {
            get;
            set;
        }
        public string Msg { get; set; }
    }
}