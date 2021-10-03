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

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken Value
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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public EnumKeyValue (  )
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            EnumKeyValue node = new EnumKeyValue();

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            IdentifierToken token = request.Parser.Peek ( request.Token, 1 );

            if ( token == null ) return null;
            if ( token.Kind != IdentifierKind.NumberToken ) return null;

            node.Value = token;
            node.AllTokens.Add(token);

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexEnumDeklaration dek)) return request.Index.CreateError(this);

            IndexEnumEntryDeklaration deklaration = new IndexEnumEntryDeklaration();
            deklaration.Name = this.Token.Text;
            deklaration.Value = this.Value;
            deklaration.Use = this;

            this.Deklaration = deklaration;

            dek.Entries.Add(deklaration);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return true;
        }
    }
}