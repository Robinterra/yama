using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class EnumKeyValue : IParseTreeNode
    {

        #region get/set

        public IndexEnumEntryDeklaration Deklaration
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        public SyntaxToken Value
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

        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public EnumKeyValue (  )
        {

        }

        #endregion ctor

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            EnumKeyValue node = new EnumKeyValue();

            node.Token = token;

            token = parser.Peek ( token, 1 );

            if (token == null) return null;
            if ( token.Kind != SyntaxKind.NumberToken ) return null;

            node.Value = token;

            node.Token.Node = node;
            node.Value.Node = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexEnumDeklaration dek)) return index.CreateError(this);

            IndexEnumEntryDeklaration deklaration = new IndexEnumEntryDeklaration();
            deklaration.Name = this.Token.Text;
            deklaration.Value = this.Value;
            deklaration.Use = this;

            this.Deklaration = deklaration;

            dek.Entries.Add(deklaration);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return true;
        }
    }
}