using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NewKey : IParseTreeNode, IEndExpression, IContainer
    {

        #region get/set

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        public List<IParseTreeNode> Parameters
        {
            get;
            set;
        }

        public IParseTreeNode Zuweisung
        {
            get;
            set;
        }

        public SyntaxToken Definition
        {
            get;
            set;
        }

        public CompileReferenceCall CtorCall
        {
            get;
            set;
        } = new CompileReferenceCall();

        public CompileExecuteCall FunctionExecute
        {
            get;
            set;
        } = new CompileExecuteCall();

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

                if (this.Parameters != null) result.AddRange ( this.Parameters );
                if (this.Zuweisung != null) result.Add ( this.Zuweisung );

                return result;
            }
        }

        public SyntaxToken Ende { get; set; }

        #endregion get/set

        #region methods

        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;
            if (token.Kind == SyntaxKind.Void) return true;

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.New ) return null;

            NewKey newKey = new NewKey();
            newKey.Token = token;
            newKey.Definition = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidOperator( newKey.Definition )) return null;

            SyntaxToken beginkind = parser.Peek ( newKey.Definition, 1 );

            SyntaxToken endToken = parser.FindEndToken ( beginkind, SyntaxKind.CloseBracket, SyntaxKind.OpenBracket );

            if ( endToken == null ) return null;

            newKey.Parameters = parser.ParseCleanTokens ( beginkind.Position + 1, endToken.Position, true );

            if (newKey.Parameters == null) return null;

            newKey.Token.Node = newKey;
            newKey.Definition.Node = newKey;
            newKey.Ende = endToken;
            endToken.ParentNode = newKey;
            endToken.Node = newKey;
            beginkind.ParentNode = newKey;
            beginkind.Node = newKey;

            foreach ( IParseTreeNode n in newKey.Parameters )
            {
                n.Token.ParentNode = newKey;
            }

            return newKey;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            //if (parent is IndexVariabelnReference varref) return this.RefComb(varref);
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            foreach (IParseTreeNode node in this.Parameters)
            {
                node.Indezieren(index, container);
            }

            IndexVariabelnReference typeDeklaration = new IndexVariabelnReference();
            typeDeklaration.Use = this;
            typeDeklaration.Name = this.Definition.Text;
            container.VariabelnReferences.Add(typeDeklaration);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            reference.Deklaration = typeDeklaration;
            typeDeklaration.ParentCall = reference;
            typeDeklaration.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            for (int i = this.Parameters.Count; i > 0; i-- )
            {
                this.Parameters[i - 1].Compile(compiler, mode);

                CompileMovResult movResultRight = new CompileMovResult();

                movResultRight.Compile(compiler, null, mode);
            }

            this.CtorCall.Compile(compiler, this.Reference, "methode");

            for (int i = 0; i < this.Parameters.Count; i++)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.Compile(compiler, null);
            }

            this.FunctionExecute.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods

    }
}