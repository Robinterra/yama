using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{
    public class RequestParserTreeCompile
    {

        #region get/set

        public Compiler Compiler
        {
            get;
            set;
        }

        public string Mode
        {
            get;
            set;
        }

        public bool StructDefiniton
        {
            get;
            set;
        }

        public IndexVariabelnDeklaration? StructLeftNode
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestParserTreeCompile(Compiler compiler)
        {
            this.Compiler = compiler;
            this.Mode = "default";
        }

        public RequestParserTreeCompile(Compiler compiler, string mode)
        {
            this.Compiler = compiler;
            this.Mode = mode;
        }

        public RequestParserTreeCompile(Compiler compiler, string mode, bool structDef)
        {
            this.Compiler = compiler;
            this.Mode = mode;
            this.StructDefiniton = structDef;
        }

        #endregion ctor

    }
}