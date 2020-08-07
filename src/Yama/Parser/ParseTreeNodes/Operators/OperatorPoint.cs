using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class OperatorPoint : IParseTreeNode//, IPriority
    {

        #region get/set

        public IndexMethodReference Reference
        {
            get;
            set;
        }

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public IParseTreeNode RightNode
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        public int Prio
        {
            get;
        }

        public bool IsANonStatic
        {
            get
            {
                if (this.RightNode is ReferenceCall rc)
                    if (rc.Reference.Deklaration is IndexMethodDeklaration dek) return true;

                return false;
            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                result.Add ( this.LeftNode );
                result.Add ( this.RightNode );

                return result;
            }
        }

        #endregion get/set

        #region ctor

        public OperatorPoint ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            return token.Kind == SyntaxKind.Point;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Point ) return null;
            if ( !this.CheckHashValidOperator ( token ) ) return null;

            OperatorPoint node = new OperatorPoint ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );

            node.RightNode = parser.ParseCleanToken ( parser.Peek ( token, 1 ) );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            if (!this.LeftNode.Indezieren(index, parent)) return false;

            IndexVariabelnReference reference = container.VariabelnReferences.Last();

            this.RightNode.Indezieren(index, reference);

            //container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            if (mode == "set") { CompileMovResult movResultRight = new CompileMovResult(); movResultRight.Compile(compiler, null, "default"); }

            this.LeftNode.Compile(compiler, "default");

            string moderesult = "point";
            if (mode == "set") moderesult = "setpoint";
            if (mode == "methode")
            {
                moderesult = mode;
                if (this.RightNode is ReferenceCall rc)
                    if (rc.Reference.Deklaration is IndexMethodDeklaration dek)
                        this.CompileNonStaticCall(compiler, "default", dek);
            }

            this.RightNode.Compile(compiler, moderesult);

            return true;
        }

        private bool CompileNonStaticCall(Compiler.Compiler compiler, string mode, IndexMethodDeklaration methdek)
        {
            bool isok = methdek.Type == MethodeType.Methode;
            if (!isok) isok = methdek.Type == MethodeType.Property;
            if (!isok) return false;

            CompileMovResult movResultRight = new CompileMovResult();

            movResultRight.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods
    }
}