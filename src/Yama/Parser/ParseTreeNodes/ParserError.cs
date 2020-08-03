using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ParserError : IParseTreeNode
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
                return null;
            }
        }

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            ParserError result = new ParserError ();

            result.Token = token;

            token.Node = result;

            return result;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            return index.CreateError(this);
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return false;
        }
    }
}