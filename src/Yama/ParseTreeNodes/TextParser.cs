using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class TextParser : IParseTreeNode, IIndexNode, ICompileNode, IPriority
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
            this.Token = new IdentifierToken();
            this.AllTokens = new List<IdentifierToken> ();
        }

        public TextParser ( int prio ) : this()
        {
            this.Prio = prio;
        }

        #endregion ctor

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Text ) return null;

            TextParser node = new TextParser();

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            return node;
        }

        public bool Indezieren( RequestParserTreeIndezieren request )
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, "string");

            container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            CompileData compile = new CompileData(DataMode.Text);
            compile.Data.Text = this.Token.Text;
            compile.Compile(request.Compiler, this);

            CompileReferenceCall referenceCall = new CompileReferenceCall();
            referenceCall.CompileData(request.Compiler, this, compile.JumpPointName!);

            return true;
        }
    }
}