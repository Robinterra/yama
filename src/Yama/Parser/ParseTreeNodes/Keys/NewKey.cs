using System.Collections.Generic;
using System.Linq;
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

        public IdentifierToken Definition
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

                if (this.Parameters != null) result.AddRange ( this.Parameters );
                if (this.Zuweisung != null) result.Add ( this.Zuweisung );

                return result;
            }
        }

        public IdentifierToken Ende { get; set; }

        #endregion get/set

        #region methods

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Char) return true;
            if (token.Kind == IdentifierKind.Byte) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;
            if (token.Kind == IdentifierKind.Float32Bit) return true;
            if (token.Kind == IdentifierKind.Void) return true;

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.New ) return null;

            NewKey newKey = new NewKey();
            newKey.Token = token;
            newKey.Definition = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidOperator( newKey.Definition )) return null;

            IdentifierToken beginkind = parser.Peek ( newKey.Definition, 1 );

            IdentifierToken endToken = parser.FindEndToken ( beginkind, IdentifierKind.CloseBracket, IdentifierKind.OpenBracket );

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
            List<IParseTreeNode> copylist = this.Parameters;
            //copylist.Reverse();
            IParseTreeNode dek = null;

            int parasCount = 0;

            foreach (IParseTreeNode par in copylist )
            {
                dek = par;
                if (par is EnumartionExpression b) dek = b.ExpressionParent;
                if (dek == null) continue;

                dek.Compile(compiler, mode);

                CompilePushResult compilePushResult = new CompilePushResult();
                compilePushResult.Compile(compiler, null, "default");

                parasCount++;
            }

            this.CtorCall.Compile(compiler, this.Reference, "methode");

            parasCount++;

            this.FunctionExecute.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods

    }
}