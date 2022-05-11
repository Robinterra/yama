using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NullKey : IParseTreeNode, IIndexNode, ICompileNode
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public CompileNumConst NumConst
        {
            get;
            set;
        } = new CompileNumConst();

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

        public NullKey (  )
        {
            this.Token = new IdentifierToken();
            this.AllTokens = new List<IdentifierToken> ();
        }

        public NullKey ( int prio ) : this()
        {
            this.Prio = prio;
        }

        #endregion ctor

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Null ) return null;

            NullKey node = new NullKey();
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, "int");

            container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            this.Token.Value = 0;

            this.NumConst.Compile(request.Compiler, new Number { Token = this.Token }, request.Mode);

            return true;
        }
    }
}