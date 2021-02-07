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

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.Break ) return null;

            BreakKey key = new BreakKey (  );
            token.Node = key;
            key.Token = token;

            return key;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.JumpTo.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods

    }
}