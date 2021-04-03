using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class GenericCall : IParseTreeNode, IEndExpression, IContainer
    {

        #region get/set

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public List<IParseTreeNode> ParametersNodes
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

                result.Add ( this.LeftNode );
                result.AddRange ( this.ParametersNodes );

                return result;
            }
        }

        public IdentifierKind BeginZeichen
        {
            get;
            set;
        }

        public IdentifierKind EndeZeichen
        {
            get;
            set;
        }
        public IdentifierToken Ende
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public GenericCall ( int prio )
        {
            this.Prio = prio;
        }

        public GenericCall ( IdentifierKind begin, IdentifierKind end, int prio )
            : this ( prio )
        {
            this.BeginZeichen = begin;
            this.EndeZeichen = end;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.BeginZeichen ) return null;

            MethodeCallNode node = new MethodeCallNode ( this.Prio );

            IdentifierToken steuerToken = request.Parser.FindEndToken ( request.Token, this.EndeZeichen, this.BeginZeichen );

            if ( steuerToken == null ) return null;

            IdentifierToken left = request.Parser.Peek ( request.Token, -1 );

            if ( left.Kind == IdentifierKind.Operator ) return null;
            if ( left.Kind == IdentifierKind.NumberToken ) return null;
            if ( left.Kind == IdentifierKind.OpenBracket ) return null;
            if ( left.Kind == IdentifierKind.BeginContainer ) return null;
            if ( left.Kind == IdentifierKind.OpenSquareBracket ) return null;
            if ( left.Kind == IdentifierKind.EndOfCommand ) return null;
            if ( left.Kind == IdentifierKind.Comma ) return null;

            node.LeftNode = request.Parser.ParseCleanToken ( left );

            node.ParametersNodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, steuerToken.Position, true );

            node.Token = request.Token;
            node.Token.Node = node;

            node.LeftNode.Token.ParentNode = node;
            node.Ende = steuerToken;

            steuerToken.ParentNode = node;
            steuerToken.Node = node;

            foreach ( IParseTreeNode n in node.ParametersNodes )
            {
                n.Token.ParentNode = node;
            }

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return true;
        }

        #endregion methods
    }
}