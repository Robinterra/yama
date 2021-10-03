using Yama.Index;
using System.Collections.Generic;
using Yama.Lexer;
using System.Linq;
using Yama.Compiler;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class VariabelDeklaration : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnDeklaration Deklaration
        {
            get;
            set;
        }

        public IParseTreeNode TypeDefinition
        {
            get;
            set;
        }

        public GenericCall GenericDefintion
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
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

                result.Add ( this.TypeDefinition );
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

        #endregion get/set

        #region ctor

        public VariabelDeklaration ( int prio )
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidChild ( IdentifierToken token )
        {
            if (token == null) return false;

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

        public IParseTreeNode Parse ( RequestParserTreeParser request )
        {
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            IdentifierToken lexerLeft = request.Parser.Peek ( request.Token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) /*&& !this.CheckAusnahmen ( lexerLeft )*/ ) return null;

            VariabelDeklaration node = new VariabelDeklaration ( this.Prio );

            IdentifierToken lexerRight = request.Parser.Peek ( request.Token, 1 );
            if (lexerRight == null) return null;

            lexerRight = this.TryParseGeneric(request, node, lexerRight);

            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            node.Token = lexerRight;
            node.AllTokens.Add(lexerRight);

            IParseTreeNode callRule = request.Parser.GetRule<ReferenceCall>();

            node.TypeDefinition = request.Parser.TryToParse ( callRule, request.Token );

            if ( node.TypeDefinition == null ) return null;

            return node;
        }

        private IdentifierToken TryParseGeneric(RequestParserTreeParser request, VariabelDeklaration deklaration, IdentifierToken token)
        {
            GenericCall genericRule = request.Parser.GetRule<GenericCall>();
            if (genericRule == null) return token;

            IParseTreeNode node = genericRule.Parse(new RequestParserTreeParser(request.Parser, token));
            if (!(node is GenericCall genericCall)) return token;

            deklaration.GenericDefintion = genericCall;

            return request.Parser.Peek(genericCall.Ende, 1);
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            IndexVariabelnDeklaration reference = this.Deklaration;

            if (reference == null)
            {
                reference = new IndexVariabelnDeklaration();
                reference.Use = this;
                reference.Name = this.Token.Text;
                if (this.GenericDefintion != null)
                {
                    reference.GenericDeklaration = this.GenericDefintion;
                    this.GenericDefintion.Indezieren(request);
                }

                this.TypeDefinition.Indezieren(request);
                IndexVariabelnReference type = container.VariabelnReferences.LastOrDefault();

                if (type == null) return request.Index.CreateError(this);

                reference.Type = type;

                this.Deklaration = reference;
            }

            container.VariabelnDeklarations.Add(reference);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (request.Mode == "set")
            {
                IndexVariabelnReference varref = new IndexVariabelnReference();
                varref.Deklaration = this.Deklaration;
                varref.Use = this;
                varref.ParentUsesSet = this.Deklaration.ParentUsesSet;

                this.Compilen.Compile(request.Compiler, varref, request.Mode);
            }

            return true;
        }

        #endregion methods
    }
}