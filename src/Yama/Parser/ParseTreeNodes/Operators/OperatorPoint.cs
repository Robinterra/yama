using System;
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

        public IdentifierToken Token
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
                {
                    if (rc.Reference.Deklaration is IndexMethodDeklaration dek)
                        return dek.Type != MethodeType.Static;

                    if (rc.Reference.Deklaration is IndexVektorDeklaration vd)
                        return vd.Type != MethodeType.VektorStatic;

                    if (rc.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsd)
                        return pgsd.Type != MethodeType.PropertyStaticGetSet;
                }

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

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            return token.Kind == IdentifierKind.Point;
        }

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.Point ) return null;
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
            if (this.RightNode is ReferenceCall rct)
                if (rct.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsdek)
                    if (!this.CompileNonStaticCallt(compiler, mode, pgsdek)) return true;

            this.CompileLeftNodeIfNotStaticClass(compiler, mode);

            if (this.RightNode is ReferenceCall rctu)
                if (rctu.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsdek)
                    this.CompileNonStaticCall(compiler, mode, pgsdek);

            string moderesult = "point";
            if (mode == "set") moderesult = "setpoint";

            if (mode == "methode")
            {
                moderesult = mode;
                if (this.RightNode is ReferenceCall rc)
                    if (rc.Reference.Deklaration is IndexMethodDeklaration dek)
                        this.CompileNonStaticCall(compiler, "default", dek);
            }

            if (mode == "vektorcall" || mode == "setvektorcall")
            {
                moderesult = mode;
                if (this.RightNode is ReferenceCall rc)
                    if (rc.Reference.Deklaration is IndexVektorDeklaration dek)
                        this.CompileNonStaticCall(compiler, "default", dek);
            }

            this.RightNode.Compile(compiler, moderesult);

            return true;
        }

        private bool CompileLeftNodeIfNotStaticClass(Compiler.Compiler compiler, string mode)
        {
            if (this.RightNode is ReferenceCall rct)
            {
                if (rct.Reference.Deklaration is IndexMethodDeklaration t)
                    if (t.Type == MethodeType.Static) return true;

                if (rct.Reference.Deklaration is IndexVektorDeklaration vd)
                    if (vd.Type == MethodeType.VektorStatic) return true;

                if (rct.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsd)
                    if (pgsd.Type == MethodeType.VektorStatic) return true;
            }

            return this.LeftNode.Compile(compiler, "default");
        }

        private bool CompileNonStaticCall(Compiler.Compiler compiler, string mode, IndexMethodDeklaration methdek)
        {
            bool isok = methdek.Type == MethodeType.Methode;
            if (!isok) isok = methdek.Type == MethodeType.Property;
            if (!isok) return false;

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "copy");

            return true;
        }

        private bool CompileNonStaticCall(Compiler.Compiler compiler, string mode, IndexVektorDeklaration methdek)
        {
            if (methdek.Type == MethodeType.VektorStatic) return true;

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            return true;
        }

        private bool CompileNonStaticCall(Compiler.Compiler compiler, string mode, IndexPropertyGetSetDeklaration methdek)
        {
            if (methdek.Type == MethodeType.PropertyStaticGetSet) return true;

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            return true;
        }

        private bool CompileNonStaticCallt(Compiler.Compiler compiler, string mode, IndexPropertyGetSetDeklaration methdek)
        {
            if (methdek.Type == MethodeType.PropertyStaticGetSet) return true;

            if (mode == "default" || mode == "set")
            {
                if ("default" == mode) mode = "point";

                VektorCall call = new VektorCall(5);
                call.LeftNode = this;
                call.ParametersNodes = new List<IParseTreeNode>();

                call.Compile(compiler, mode);

                return false;
            }

            return true;
        }

        #endregion methods
    }
}