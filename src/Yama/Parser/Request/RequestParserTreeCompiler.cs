namespace Yama.Parser.Request
{
    public class RequestParserTreeCompile
    {

        #region get/set

        public Compiler.Compiler Compiler
        {
            get;
            set;
        }

        public string Mode
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestParserTreeCompile(Compiler.Compiler compiler)
        {
            this.Compiler = compiler;
            this.Mode = "default";
        }

        public RequestParserTreeCompile(Compiler.Compiler compiler, string mode)
        {
            this.Compiler = compiler;
            this.Mode = mode;
        }

        #endregion ctor

    }
}