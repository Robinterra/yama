using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class WhileKey : IParseTreeNode, IContainer
    {

        #region get/set

        public IParseTreeNode Condition
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

                if (this.Condition != null) result.Add ( this.Condition );
                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.While ) return null;
            if ( parser.Peek ( token, 1 ).Kind != SyntaxKind.OpenKlammer ) return null;

            WhileKey key = new WhileKey (  );
            key.Token = token;
            token.Node = key;

            SyntaxToken conditionkind = parser.Peek ( token, 1 );

            IParseTreeNode rule = parser.GetRule<ContainerExpression>();

            key.Condition = rule.Parse(parser, conditionkind);

            if (key.Condition == null) return null;

            key.Condition.Token.ParentNode = key;

            SyntaxToken Statementchild = parser.Peek ( ((ContainerExpression)key.Condition).Ende, 1);

            key.Statement = parser.ParseCleanToken(Statementchild);

            if (key.Statement == null) return null;

            key.Statement.Token.ParentNode = key;

            return key;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            this.Statement.Indezieren(index, parent);
            this.Condition.Indezieren(index, parent);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return true;
        }

        #endregion methods

    }
}