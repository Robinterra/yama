using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class GenericCall : IParseTreeNode, IEndExpression, IContainer
    {

        #region get/set

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

                return result;
            }
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        public IdentifierToken Begin
        {
            get;
            set;
        }

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public GenericCall (  )
        {

        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Operator ) return null;
            if ( request.Token.Text != "<" ) return null;

            GenericCall genericCall = new GenericCall();

            genericCall.Begin = request.Token;

            IdentifierToken token = request.Parser.Peek(request.Token, 1);
            if (token == null) return null;
            if (!this.ValidNameTokenKind(token.Kind)) return null;

            genericCall.Token = token;
            token = request.Parser.Peek(token, 1);

            if (token.Kind != IdentifierKind.Operator) return null;
            if (token.Text != ">") return null;

            genericCall.Ende = token;

            return this.CleanUp(genericCall);
        }

        private bool ValidNameTokenKind(IdentifierKind kind)
        {
            if (kind == IdentifierKind.Word) return true;
            if (kind == IdentifierKind.Int32Bit) return true;
            if (kind == IdentifierKind.Boolean) return true;
            if (kind == IdentifierKind.Char) return true;
            if (kind == IdentifierKind.Byte) return true;
            if (kind == IdentifierKind.Int16Bit) return true;
            if (kind == IdentifierKind.Int64Bit) return true;
            if (kind == IdentifierKind.Float32Bit) return true;

            return false;
        }

        private IParseTreeNode CleanUp(GenericCall genericCall)
        {
            genericCall.Token.Node = genericCall;

            genericCall.Begin.Node = genericCall;

            genericCall.Ende.Node = genericCall;

            return genericCall;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is IndexKlassenDeklaration idk) return this.IndezKlassenGeneric(request, idk);
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            container.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        private bool IndezKlassenGeneric(RequestParserTreeIndezieren request, IndexKlassenDeklaration idk)
        {


            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return true;
        }

        #endregion methods
    }
}