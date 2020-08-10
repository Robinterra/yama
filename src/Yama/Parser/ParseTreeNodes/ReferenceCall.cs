using System;
using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;

namespace Yama.Parser
{
    public class ReferenceCall : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        public SyntaxToken Token
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

        public ReferenceCall (  )
        {

        }

        public ReferenceCall ( int prio )
        {
            this.Prio = prio;
        }
        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( !this.CheckValidTokens ( token ) ) return null;

            ReferenceCall result = new ReferenceCall (  );

            token.Node = result;

            result.Token = token;

            return result;
        }

        private bool CheckValidTokens(SyntaxToken token)
        {
            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.This) return true;
            if (token.Kind == SyntaxKind.Base) return true;

            return false;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (parent is IndexVariabelnReference varref) return this.RefComb(varref);
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            container.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        private bool RefComb(IndexVariabelnReference varref)
        {
            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            string moderesult = "default";
            if (mode == "set") moderesult = mode;
            if (mode == "point") moderesult = mode;
            if (mode == "setpoint") moderesult = mode;
            if (this.Reference.Deklaration is IndexMethodDeklaration) moderesult = "methode";

            CompileReferenceCall compileReference = new CompileReferenceCall();

            compileReference.Compile(compiler, this, moderesult);

            return true;
        }

        #endregion methods

    }
}