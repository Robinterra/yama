using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class BedingtesCompilierenParser : IParseTreeNode
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
                return new List<IParseTreeNode> (  );
            }
        }

        #endregion get/set

        #region ctor

        public BedingtesCompilierenParser (  )
        {

        }

        #endregion ctor

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.BedingtesCompilieren ) return null;

            BedingtesCompilierenParser node = new BedingtesCompilierenParser { Token = token };

            token.Node = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            if (!this.Token.Text.Contains("#region asm")) return true;

            compiler.Writer.WriteLine(this.Token.Value.ToString());

            return true;
        }
    }
}