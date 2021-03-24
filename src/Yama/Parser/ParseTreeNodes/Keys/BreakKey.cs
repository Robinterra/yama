using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class BreakKey : IParseTreeNode
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

        public CompileJumpTo JumpTo
        {
            get;
            set;
        } = new CompileJumpTo() { Point = PointMode.LoopEnde };

        #endregion get/set

        #region ctor

        public BreakKey()
        {
            
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Break ) return null;

            BreakKey key = new BreakKey (  );
            key.Token = request.Token;
            key.Token.Node = key;

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            this.JumpTo.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        #endregion methods

    }
}