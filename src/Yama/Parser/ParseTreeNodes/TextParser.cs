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

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Text ) return null;

            TextParser node = new TextParser { Token = request.Token };

            node.Token.Node = node;

            return node;
        }

        public bool Indezieren( Request.RequestParserTreeIndezieren request )
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            CompileData compile = new CompileData();
            compile.Data = new DataObject();
            compile.Data.Mode = DataMode.Text;
            compile.Data.Text = this.Token.Text;
            compile.Compile(request.Compiler, this);

            CompileReferenceCall referenceCall = new CompileReferenceCall();
            referenceCall.CompileData(request.Compiler, this, compile.JumpPointName);

            return true;
        }
    }
}