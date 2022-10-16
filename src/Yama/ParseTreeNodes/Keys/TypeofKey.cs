using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class TypeofKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

        #region get/set

        public IdentifierToken TypeToken
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                return result;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IndexVariabelnReference? TypeRef
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public TypeofKey ()
        {
            this.Token = new();
            this.Ende = new();
            this.TypeToken = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Typeof ) return null;

            IdentifierToken? openBrackettoken = request.Parser.Peek ( request.Token, 1 );
            if (openBrackettoken is null) return new ParserError(request.Token, $"Expectet a open Bracket '<' after a 'typeof' Keyword {request.Token.Kind}");
            if ( openBrackettoken.Kind != IdentifierKind.Operator && openBrackettoken.Text == "<" ) return new ParserError(openBrackettoken, $"Expectet a open Bracket '<' after Keyword 'typeof' and not a {openBrackettoken.Kind}", request.Token);

            TypeofKey key = new TypeofKey ();
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);
            key.AllTokens.Add(openBrackettoken);

            IdentifierToken? wordToken = request.Parser.Peek ( openBrackettoken, 1 );
            if (wordToken is null) return new ParserError(request.Token, $"Expectet a begin of a type name after '<'", openBrackettoken);
            if (wordToken.Kind != IdentifierKind.Word) return new ParserError(request.Token, $"Expectet a begin of a type name after '<' and not '{wordToken.Text}'", openBrackettoken, wordToken);
            key.TypeToken = wordToken;
            key.AllTokens.Add(wordToken);

            IdentifierToken? closeBrackettoken = request.Parser.Peek ( wordToken, 1 );
            if (closeBrackettoken is null) return new ParserError(wordToken, $"Expectet a close Bracket '>' after a 'type' on a typeof {wordToken.Text}", request.Token, openBrackettoken);
            if ( closeBrackettoken.Kind != IdentifierKind.Operator && closeBrackettoken.Text == ">" ) return new ParserError(request.Token, $"Expectet a close Bracket '>' after Keyword 'typeof' and not a {closeBrackettoken.Kind}", wordToken, closeBrackettoken, openBrackettoken);
            key.AllTokens.Add(closeBrackettoken);
            key.Ende = closeBrackettoken;

            return key;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.TypeToken is null) return false;

            IndexVariabelnReference type = new IndexVariabelnReference(this, this.TypeToken.Text);

            container.VariabelnReferences.Add(type);

            this.TypeRef = type;

            IndexVariabelnReference typeInfo = new IndexVariabelnReference(this, "TypeInfo");

            container.VariabelnReferences.Add(typeInfo);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            Compiler.Compiler compiler = request.Compiler;

            if (this.TypeRef is null) return true;
            if (this.TypeRef.Deklaration is not IndexKlassenDeklaration ikd) return true;
            if (ikd.ReflectionData is null) return compiler.AddError($"The Class '{ikd.Name}' is not Reflectionable", this);

            CompileReferenceCall referenceCall = new CompileReferenceCall();
            referenceCall.CompileData(compiler, this, ikd.ReflectionData.JumpPointName!);

            return true;
        }

        #endregion methods

    }
}