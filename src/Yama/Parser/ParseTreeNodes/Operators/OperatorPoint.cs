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

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Point ) return null;
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            OperatorPoint node = new OperatorPoint ( this.Prio );
            node.Token = request.Token;
            node.Token.Node = node;

            node.LeftNode = request.Parser.ParseCleanToken ( request.Parser.Peek ( request.Token, -1 ) );

            node.RightNode = request.Parser.ParseCleanToken ( request.Parser.Peek ( request.Token, 1 ) );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            if (!this.LeftNode.Indezieren(request)) return false;

            IndexVariabelnReference reference = container.VariabelnReferences.LastOrDefault();
            if (reference == null) return request.Index.CreateError(this);

            this.RightNode.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, reference));

            //container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.RightNode is ReferenceCall rct)
                if (rct.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsdek)
                    if (!this.CompileNonStaticCallt(request.Compiler, request.Mode, pgsdek)) return true;

            this.CompileLeftNodeIfNotStaticClass(request.Compiler, request.Mode);

            if (this.RightNode is ReferenceCall rctu)
                if (rctu.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsdek)
                    this.CompileNonStaticCall(request.Compiler, request.Mode, pgsdek);

            string moderesult = "point";
            if (request.Mode == "set") moderesult = "setpoint";

            if (request.Mode == "methode")
            {
                moderesult = request.Mode;
                if (this.RightNode is ReferenceCall rc)
                    if (rc.Reference.Deklaration is IndexMethodDeklaration dek)
                        this.CompileNonStaticCall(request.Compiler, "default", dek);
            }

            if (request.Mode == "vektorcall" || request.Mode == "setvektorcall")
            {
                moderesult = request.Mode;
                if (this.RightNode is ReferenceCall rc)
                    if (rc.Reference.Deklaration is IndexVektorDeklaration dek)
                        this.CompileNonStaticCall(request.Compiler, "default", dek);
            }

            this.RightNode.Compile(new Request.RequestParserTreeCompile(request.Compiler, moderesult));

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

                if (rct.Reference.Deklaration is IndexPropertyDeklaration klu)
                    if (klu.Zusatz == MethodeType.Static) return true;
            }

            return this.LeftNode.Compile(new Request.RequestParserTreeCompile(compiler, "default"));
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
            compilePushResult.Compile(compiler, null, "copy");

            return true;
        }

        private bool CompileNonStaticCall(Compiler.Compiler compiler, string mode, IndexPropertyGetSetDeklaration methdek)
        {
            if (methdek.Type == MethodeType.PropertyStaticGetSet) return true;

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "copy");

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

                call.Compile(new Request.RequestParserTreeCompile(compiler, mode));

                return false;
            }

            return true;
        }

        #endregion methods
    }
}