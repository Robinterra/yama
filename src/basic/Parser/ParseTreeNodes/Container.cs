using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Container : IParseTreeNode
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> Statements
        {
            get;
            set;
        }

        public SyntaxKind BeginKind
        {
            get;
            set;
        }

        public SyntaxKind EndeKind
        {
            get;
            set;
        }

        public SyntaxToken Ende
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return this.Statements;
            }
        }

        #endregion get/set

        #region  ctor

        public Container (  )
        {

        }

        public Container ( SyntaxKind begin, SyntaxKind ende )
        {
            this.BeginKind = begin;
            this.EndeKind = ende;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != this.BeginKind ) return null;

            SyntaxToken kind = parser.FindEndToken ( token, this.EndeKind, this.BeginKind );

            if ( kind == null ) return null;

            if ( kind.Node != null ) return null;

            Container expression = new Container (  );

            expression.Token = token;
            expression.Ende = kind;

            token.Node = expression;
            kind.Node = expression;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens ( token.Position + 1, kind.Position );

            parser.Repleace ( token, kind.Position );

            if ( nodes == null ) return null;

            expression.Statements = nodes;

            return expression;
        }

        #endregion methods

    }
}