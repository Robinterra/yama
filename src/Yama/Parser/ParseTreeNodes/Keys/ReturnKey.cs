using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ReturnKey : IParseTreeNode
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

                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        public CompileJumpTo JumpTo
        {
            get;
            set;
        } = new CompileJumpTo() { Point = PointMode.RootEnde };

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ReturnKey ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Return ) return null;

            IdentifierToken? ende = request.Parser.FindAToken(request.Token, IdentifierKind.EndOfCommand);
            if (ende is null) return null;

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens(request.Token.Position + 1, ende.Position);
            IParseTreeNode? node = null;

            if ( nodes is null ) return null;
            if ( nodes.Count != 1 ) return null;
            node = nodes[0];

            ReturnKey result = new ReturnKey();

            result.Statement = node;
            result.Token = request.Token;
            result.AllTokens.Add ( request.Token );

            return result;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is null) return request.Index.CreateError(this);

            this.Statement.Indezieren(request);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Statement is null) return false;

            this.Statement.Compile(request);

            if (this.Statement is ReferenceCall)
            {
                CompileMovReg movReg = new CompileMovReg();
                movReg.Compile(request.Compiler, this);
            }

            this.JumpTo.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        #endregion methods

    }
}