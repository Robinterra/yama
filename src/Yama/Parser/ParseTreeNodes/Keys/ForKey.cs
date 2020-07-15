using System.Collections.Generic;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ForKey : IParseTreeNode, IContainer
    {

        #region get/set

        public IParseTreeNode Deklaration
        {
            get;
            set;
        }

        public IParseTreeNode Condition
        {
            get;
            set;
        }
        public IParseTreeNode Inkrementation
        {
            get;
            set;
        }
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

        public SyntaxToken Ende
        {
            get
            {
                return (this.Statement is IContainer t) ? t.Ende : this.Statement.Token;
            }
            set
            {

            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Deklaration != null) result.Add ( this.Deklaration );
                if (this.Condition != null) result.Add ( this.Condition );
                if (this.Inkrementation != null) result.Add ( this.Inkrementation );
                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.For ) return null;
            if ( parser.Peek ( token, 1 ).Kind != SyntaxKind.OpenKlammer ) return null;

            ForKey key = new ForKey (  );
            key.Token = token;
            token.Node = key;

            SyntaxToken conditionkind = parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(SyntaxKind.OpenKlammer, SyntaxKind.CloseKlammer);

            IParseTreeNode klammer = rule.Parse(parser, conditionkind);

            if (klammer == null) return null;
            if (!(klammer is Container t)) return null;
            if (t.Statements.Count != 3) return null;

            t.Token.ParentNode = key;

            key.Deklaration = t.Statements[0];
            key.Condition = t.Statements[1];
            key.Inkrementation = t.Statements[2];

            SyntaxToken Statementchild = parser.Peek ( t.Ende, 1);

            key.Statement = parser.ParseCleanToken(Statementchild);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;

            return key;
        }

        #endregion methods

    }
}