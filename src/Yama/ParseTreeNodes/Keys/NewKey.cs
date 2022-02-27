using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class NewKey : IParseTreeNode, IEndExpression, IContainer
    {

        #region vars

        private IdentifierToken? ende;

        #endregion vars

        #region get/set

        public IndexVariabelnReference? Reference
        {
            get;
            set;
        }

        public List<IParseTreeNode> Parameters
        {
            get;
            set;
        }

        public IParseTreeNode? Zuweisung
        {
            get;
            set;
        }

        public IdentifierToken? Definition
        {
            get;
            set;
        }

        public CompileReferenceCall CtorCall
        {
            get;
        } = new CompileReferenceCall();

        public CompileExecuteCall FunctionExecute
        {
            get;
        } = new CompileExecuteCall();

        public IdentifierToken Token
        {
            get;
            set;
        }

        public GenericCall? GenericDefintion
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.GenericDefintion != null) result.Add(this.GenericDefintion);
                result.AddRange ( this.Parameters );
                if (this.Zuweisung != null) result.Add ( this.Zuweisung );

                return result;
            }
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.ende is null) return this.Token;

                return this.ende;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public NewKey ()
        {
            this.Parameters = new List<IParseTreeNode>();
            this.AllTokens = new List<IdentifierToken> ();
            this.Token = new IdentifierToken();
        }

        #endregion ctor

        #region methods

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

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.New ) return null;

            NewKey newKey = new NewKey();
            newKey.Token = request.Token;
            newKey.AllTokens.Add ( request.Token );

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;

            newKey.Definition = token;
            newKey.AllTokens.Add ( token );
            if ( !this.CheckHashValidOperator( token )) return null;

            IdentifierToken? beginkind = request.Parser.Peek ( newKey.Definition, 1 );
            if ( beginkind is null ) return null;

            beginkind = this.TryParseGeneric(request, newKey, beginkind);
            if ( beginkind is null ) return null;

            if (beginkind.Kind != IdentifierKind.OpenBracket) return null;

            IdentifierToken? endToken = request.Parser.FindEndToken ( beginkind, IdentifierKind.CloseBracket, IdentifierKind.OpenBracket );
            if ( endToken is null ) return null;

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens ( beginkind.Position + 1, endToken.Position, true );
            if (nodes is null) return null;

            newKey.Parameters = nodes;

            newKey.ende = endToken;
            newKey.AllTokens.Add(endToken);
            newKey.AllTokens.Add(beginkind);

            return newKey;
        }

        private IdentifierToken? TryParseGeneric(RequestParserTreeParser request, NewKey deklaration, IdentifierToken token)
        {
            GenericCall? genericRule = request.Parser.GetRule<GenericCall>();
            if (genericRule is null) return token;

            IParseTreeNode? node = request.Parser.TryToParse ( genericRule, token );
            if (node is not GenericCall genericCall) return token;

            deklaration.GenericDefintion = genericCall;

            return request.Parser.Peek(genericCall.Ende, 1);
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            //if (parent is IndexVariabelnReference varref) return this.RefComb(varref);
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Definition is null) return request.Index.CreateError(this);

            IndexMethodReference methodReference = new IndexMethodReference(this, this.Token.Text);

            foreach (IParseTreeNode node in this.Parameters)
            {
                node.Indezieren(request);

                IndexVariabelnReference? parRef = container.VariabelnReferences.LastOrDefault();
                if (parRef is null) return request.Index.CreateError(this);

                methodReference.Parameters.Add(parRef);
            }

            container.MethodReferences.Add(methodReference);

            IndexVariabelnReference typeDeklaration = new IndexVariabelnReference(this, this.Definition.Text);

            if (this.GenericDefintion != null)
            {
                typeDeklaration.GenericDeklaration = this.GenericDefintion;
                this.GenericDefintion.Indezieren(request);
            }
            container.VariabelnReferences.Add(typeDeklaration);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, this.Token.Text);
            reference.Deklaration = typeDeklaration;
            if (this.GenericDefintion != null) reference.GenericDeklaration = this.GenericDefintion;

            typeDeklaration.ParentCall = reference;
            typeDeklaration.VariabelnReferences.Add(reference);
            this.Reference = reference;
            methodReference.CallRef = reference;

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Reference is null) return false;

            List<IParseTreeNode> copylist = this.Parameters;
            copylist.Reverse();

            IParseTreeNode? dek = null;

            int parasCount = 0;

            foreach (IParseTreeNode par in copylist )
            {
                dek = par;
                if (par is EnumartionExpression b) dek = b.ExpressionParent;
                if (dek == null) continue;

                dek.Compile(request);

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(request.Compiler, null, "default");

                parasCount++;
            }

            this.CtorCall.Compile(request.Compiler, this.Reference, "methode");

            parasCount++;

            this.FunctionExecute.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        #endregion methods

    }
}