using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NullKey : IParseTreeNode
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public CompileNumConst NumConst
        {
            get;
            set;
        } = new CompileNumConst();

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode> (  );
            }
        }

        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public NullKey (  )
        {

        }

        public NullKey ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Null ) return null;

            NullKey node = new NullKey { Token = token };

            token.Node = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.Token.Value = 0;

            this.NumConst.Compile(compiler, new Number { Token = this.Token }, mode);

            return true;
        }
    }
}