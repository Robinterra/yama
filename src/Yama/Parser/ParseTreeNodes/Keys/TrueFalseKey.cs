using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class TrueFalseKey : IParseTreeNode, IPriority
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                return result;
            }
        }
        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public TrueFalseKey (  )
        {

        }

        public TrueFalseKey ( int prio )
        {
            this.Prio = prio;
        }
        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            bool isok = token.Kind == SyntaxKind.True;

            if ( !isok ) isok = token.Kind == SyntaxKind.False;

            if ( !isok ) return null;

            TrueFalseKey result = new TrueFalseKey (  );

            token.Node = result;

            result.Token = token;

            return result;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            Number number = new Number();
            if (this.Token.Text == "true") number.Token = new SyntaxToken { Value = 0xff };
            if (this.Token.Text == "false") number.Token = new SyntaxToken { Value = 0 };

            CompileNumConst numConst = new CompileNumConst();
            numConst.Compile(compiler, number, mode);

            return true;
        }

        #endregion methods

    }
}