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

        #endregion get/set

        #region ctor

        public TrueFalseKey (  )
        {

        }

        public TrueFalseKey ( int prio )
        {
            this.Prio = prio;
        }
        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            bool isok = token.Kind == IdentifierKind.True;

            if ( !isok ) isok = token.Kind == IdentifierKind.False;

            if ( !isok ) return null;

            TrueFalseKey result = new TrueFalseKey (  );

            token.Node = result;

            result.Token = token;

            return result;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = "bool";
            container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            Number number = new Number();
            if (this.Token.Text == "true") number.Token = new IdentifierToken { Value = 0xff };
            if (this.Token.Text == "false") number.Token = new IdentifierToken { Value = 0 };

            CompileNumConst numConst = new CompileNumConst();
            numConst.Compile(compiler, number, mode);

            return true;
        }

        #endregion methods

    }
}