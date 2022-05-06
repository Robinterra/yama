using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NamespaceKey : IParseTreeNode, IIndexNode, ICompileNode
    {

        #region get/set

        public IParseTreeNode? Statement
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Statement is not null) result.Add ( this.Statement );

                return result;
            }
        }

        public ParserLayer NextLayer
        {
            get;
        }

        public IndexNamespaceDeklaration? Deklaration
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public NamespaceKey(ParserLayer nextLayer)
        {
            this.NextLayer = nextLayer;
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Namespace ) return null;

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if ( token is null ) return null;

            NamespaceKey key = new NamespaceKey ( this.NextLayer );
            key.AllTokens.Add ( request.Token );

            IdentifierToken? keyNamenToken = request.Parser.Peek ( request.Token, 1 );

            if ( keyNamenToken is null ) return null;
            key.Token = keyNamenToken;
            key.AllTokens.Add ( keyNamenToken );

            IdentifierToken? statementchild = request.Parser.Peek ( keyNamenToken, 1);
            if ( statementchild is null ) return null;

            key.Statement = request.Parser.ParseCleanToken(statementchild, this.NextLayer, false);

            if (key.Statement == null) return null;
            if ( token.Kind != IdentifierKind.Text ) return new ParserError(request.Token, $"Wrong Syntax for a namespace. Expected: 'namespace \"YourNameSpaceName\"' and not 'namespace {token.Text}'", token);

            return key;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if ( this.Token.Value is null ) return false;

            string? name = this.Token.Value.ToString ();
            if ( name is null ) return false;

            IndexNamespaceDeklaration dek = new IndexNamespaceDeklaration(this, name);

            dek = request.Index.NamespaceAdd(dek);
            this.Deklaration = dek;

            if (this.Token.Info is not null) dek.OriginKeys.Add(this.Token.Info.Origin);

            if ( this.Statement is not IIndexNode statementNode ) return false;
            statementNode.Indezieren(new RequestParserTreeIndezieren(request.Index, dek));

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            //this.Statement.Compile(compiler, mode);

            return true;
        }

        #endregion methods

    }
}