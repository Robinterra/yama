using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ReturnKey : IParseTreeNode
    {

        #region get/set

        public IParseTreeNode Statement
        {
            get;
            set;
        }

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

                result.Add ( this.Statement );

                return result;
            }
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Return ) return null;

            SyntaxToken ende = parser.FindAToken(token, SyntaxKind.EndOfCommand);

            List<IParseTreeNode> nodes = parser.ParseCleanTokens(token.Position + 1, ende.Position);
            IParseTreeNode node = null;

            if ( nodes == null ) return null;
            if ( nodes.Count > 1 ) return null;
            if ( nodes.Count == 1 ) node = nodes[0];

            ReturnKey result = new ReturnKey();

            result.Statement = node;
            result.Token = token;
            token.Node = result;
            if ( node != null ) node.Token.ParentNode = result;

            return result;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            this.Statement.Indezieren(index, parent);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return true;
        }

        #endregion methods

    }
}