using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Number : IParseTreeNode, IPriority
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

        public Number (  )
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.Token = new();
        }

        public Number ( int prio ) : this()
        {
            this.Prio = prio;
        }

        #endregion ctor

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.NumberToken ) return null;

            Number node = new Number();
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = "int";

            container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            this.NumConst.Compile(request.Compiler, this, request.Mode);

            return true;
        }
    }
}