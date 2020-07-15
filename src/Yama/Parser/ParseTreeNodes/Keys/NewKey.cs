using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class NewKey : IParseTreeNode
    {

        #region get/set

        public IParseTreeNode Parameters
        {
            get;
            set;
        }

        public IParseTreeNode Zuweisung
        {
            get;
            set;
        }

        public SyntaxToken Definition
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

                if (this.Parameters != null) result.Add ( this.Parameters );
                if (this.Zuweisung != null) result.Add ( this.Zuweisung );

                return result;
            }
        }

        #endregion get/set

        #region methods

        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;
            if (token.Kind == SyntaxKind.Void) return true;

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.New ) return null;

            NewKey newKey = new NewKey();
            newKey.Token = token;
            newKey.Definition = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidOperator( newKey.Definition )) return null;

            SyntaxToken conditionkind = parser.Peek ( newKey.Definition, 1 );

            newKey.Parameters = parser.ParseCleanToken(conditionkind);

            if (newKey.Parameters == null) return null;

            newKey.Token.Node = newKey;
            newKey.Definition.Node = newKey;
            newKey.Parameters.Token.ParentNode = newKey;

            return newKey;
        }

        #endregion methods

    }
}