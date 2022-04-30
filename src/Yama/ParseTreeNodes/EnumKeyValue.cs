using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class EnumKeyValue : IParseTreeNode, IIndexNode, ICompileNode
    {

        #region get/set

        public IndexEnumEntryDeklaration? Deklaration
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken? Value
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
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            EnumKeyValue node = new EnumKeyValue();

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            IdentifierToken? equalToken = request.Parser.Peek ( request.Token, 1 );
            if ( equalToken is null ) return new ParserError(request.Token, $"Expectet a = after the '{request.Token.Text}'");
            if ( equalToken.Kind != IdentifierKind.Operator ) return new ParserError(equalToken, $"Expectet a = after the '{request.Token.Text}' and not a {equalToken.Kind.ToString()}", request.Token);
            if (equalToken.Text != "=") return new ParserError(equalToken, $"Expectet a = after the '{request.Token.Text}' and not a {equalToken.Kind.ToString()}", request.Token);

            node.AllTokens.Add(equalToken);

            IdentifierToken? token = request.Parser.Peek ( equalToken, 1 );
            if ( token is null ) return new ParserError(equalToken, $"Expectet a number after the '{request.Token.Text} ='", request.Token);
            if ( token.Kind != IdentifierKind.NumberToken ) return new ParserError(token, $"Expectet a number after the '{request.Token.Text} =' and not a {token.Kind.ToString()}", request.Token, equalToken);

            node.Value = token;
            node.AllTokens.Add(token);

            token = request.Parser.Peek ( equalToken, 1 );
            if ( token is null ) return node;
            if (token.Kind != IdentifierKind.Comma) return node;

            node.AllTokens.Add(token);

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexEnumDeklaration dek) return request.Index.CreateError(this);
            if (this.Value is null) return request.Index.CreateError(this);

            IndexEnumEntryDeklaration deklaration = new IndexEnumEntryDeklaration(this, this.Token.Text, this.Value);

            this.Deklaration = deklaration;

            dek.Entries.Add(deklaration);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            return true;
        }
    }
}