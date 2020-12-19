using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class TextParser : IParseTreeNode, IPriority
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
                return new List<IParseTreeNode> (  );
            }
        }

        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public TextParser (  )
        {

        }

        public TextParser ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.Text ) return null;

            TextParser node = new TextParser { Token = token };

            token.Node = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            CompileData compile = new CompileData();
            compile.Compile(compiler, this);

            CompileReferenceCall referenceCall = new CompileReferenceCall();
            referenceCall.CompileData(compiler, this, compile.JumpPointName);

            return true;
        }
    }
}