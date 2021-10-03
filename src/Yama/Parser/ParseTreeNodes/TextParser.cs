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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public TextParser (  )
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public TextParser ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Text ) return null;

            TextParser node = new TextParser();

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            return node;
        }

        public bool Indezieren( Request.RequestParserTreeIndezieren request )
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = "string";

            container.VariabelnReferences.Add(reference);

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