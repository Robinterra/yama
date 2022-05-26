using Yama.Index;
using System.Collections.Generic;
using Yama.Lexer;
using System.Linq;
using Yama.Compiler;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class VariabelDeklaration : IParseTreeNode, IIndexNode, ICompileNode, IPriority, IContainer
    {

        #region get/set

        public IndexVariabelnDeklaration? Deklaration
        {
            get;
            set;
        }

        public IParseTreeNode? TypeDefinition
        {
            get;
            set;
        }

        public GenericCall? GenericDefintion
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public bool IsInMethodeDeklaration
        {
            get;
        }

        public int Prio
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.TypeDefinition is not null) result.Add ( this.TypeDefinition );
                if (this.GenericDefintion != null) result.Add ( this.GenericDefintion );

                return result;
            }
        }

        public CompileReferenceCall Compilen
        {
            get;
            set;
        } = new CompileReferenceCall();

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public VariabelDeklaration ( int prio, bool isinmethoddeklaration = false )
        {
            this.Token = new();
            this.IsInMethodeDeklaration = isinmethoddeklaration;
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
            this.Ende = new();
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidChild ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Word) return true;

            return false;
        }

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Char) return true;
            if (token.Kind == IdentifierKind.Byte) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;
            if (token.Kind == IdentifierKind.Float32Bit) return true;
            if (token.Kind == IdentifierKind.Void) return true;

            return false;
        }

        public IParseTreeNode? Parse ( RequestParserTreeParser request )
        {
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            //IdentifierToken? lexerLeft = request.Parser.Peek ( request.Token, -1 );
            //if (lexerLeft is null) return null;
            //if ( this.CheckHashValidChild ( lexerLeft ) /*&& !this.CheckAusnahmen ( lexerLeft )*/ ) return null;

            VariabelDeklaration node = new VariabelDeklaration ( this.Prio );

            IdentifierToken? lexerRight = request.Parser.Peek ( request.Token, 1 );
            if (lexerRight is null) return null;

            lexerRight = this.TryParseGeneric(request, node, lexerRight);
            if (lexerRight is null) return null;
            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            node.Token = lexerRight;
            node.AllTokens.Add(lexerRight);
            node.Ende = lexerRight;

            ReferenceCall callRule = request.Parser.GetRule<ReferenceCall>();

            node.TypeDefinition = request.Parser.TryToParse ( callRule, request.Token );
            if ( node.TypeDefinition is null ) return null;

            if (!IsInMethodeDeklaration) return node;

            IdentifierToken? optionalComma = request.Parser.Peek ( lexerRight, 1 );
            if (optionalComma is null) return node;
            if (optionalComma.Kind != IdentifierKind.Comma) return node;

            node.AllTokens.Add(optionalComma);
            node.Ende = optionalComma;

            return node;
        }

        private IdentifierToken? TryParseGeneric(RequestParserTreeParser request, VariabelDeklaration deklaration, IdentifierToken token)
        {
            GenericCall genericRule = request.Parser.GetRule<GenericCall>();

            GenericCall? genericCall = request.Parser.TryToParse(genericRule, token);
            if (genericCall is null) return token;

            deklaration.GenericDefintion = genericCall;

            return request.Parser.Peek(genericCall.Ende, 1);
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);

            IndexVariabelnDeklaration? reference = this.Deklaration;

            if (reference is null)
            {
                if (this.TypeDefinition is not IIndexNode typeNode) return request.Index.CreateError(this);

                typeNode.Indezieren(request);

                IndexVariabelnReference? type = container.VariabelnReferences.LastOrDefault();
                if (type is null) return request.Index.CreateError(this);

                reference = new IndexVariabelnDeklaration(this, this.Token.Text, type);

                if (this.GenericDefintion != null)
                {
                    reference.GenericDeklaration = this.GenericDefintion;
                    this.GenericDefintion.Indezieren(request);
                }

                this.Deklaration = reference;
            }

            container.VariabelnDeklarations.Add(reference);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (request.Mode != "set") return true;
            if (this.Deklaration is null) return false;

            IndexVariabelnReference varref = new IndexVariabelnReference(this, this.Token.Text);
            varref.Deklaration = this.Deklaration;
            varref.Use = this;
            varref.ParentUsesSet = this.Deklaration.ParentUsesSet;

            this.Compilen.Compile(request.Compiler, varref, request.Mode);

            return true;
        }

        #endregion methods
    }
}