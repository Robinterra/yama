using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class TrueFalseKey : IParseTreeNode, IPriority
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

        public TrueFalseKey (  )
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        public TrueFalseKey ( int prio ) : this()
        {
            this.Prio = prio;
        }
        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            bool isok = request.Token.Kind == IdentifierKind.True;
            if ( !isok ) isok = request.Token.Kind == IdentifierKind.False;
            if ( !isok ) return null;

            TrueFalseKey result = new TrueFalseKey (  );

            result.Token = request.Token;
            result.AllTokens.Add(request.Token);

            return result;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, "bool");
            container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            Number number = new Number();
            if (this.Token.Text == "true") number.Token = new IdentifierToken { Value = 0xff };
            if (this.Token.Text == "false") number.Token = new IdentifierToken { Value = 0 };

            CompileNumConst numConst = new CompileNumConst();
            numConst.Compile(request.Compiler, number, request.Mode);

            return true;
        }

        #endregion methods

    }
}