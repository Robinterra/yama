using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class PointIdentifier : IParseTreeNode, IIndexNode, ICompileNode, IContainer//, IPriority
    {

        #region get/set

        public IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        public IParseTreeNode? RightNode
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public bool IsANonStatic
        {
            get
            {
                if (this.RightNode is not ReferenceCall rc) return false;
                if (rc.Reference is null) return false;

                if (rc.Reference.Deklaration is IndexMethodDeklaration dek)
                    return dek.Type != MethodeType.Static;

                if (rc.Reference.Deklaration is IndexVektorDeklaration vd)
                    return vd.Type != MethodeType.VektorStatic;

                if (rc.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsd)
                    return pgsd.Type != MethodeType.PropertyStaticGetSet;

                return false;
            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.LeftNode != null) result.Add ( this.LeftNode );
                if (this.RightNode != null) result.Add ( this.RightNode );

                return result;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public PointIdentifier ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Ende = new IdentifierToken();
        }

        #endregion ctor

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
            if (token.Kind == IdentifierKind.This) return true;

            return false;
        }

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;
            if (token.Kind != IdentifierKind.Point ) return null;

            PointIdentifier node = new PointIdentifier();
            node.Token = token;
            node.AllTokens.Add(token);

            ReferenceCall referenceRule = request.Parser.GetRule<ReferenceCall>();

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return null;
            if ( !this.CheckHashValidOperator ( token ) ) return null;

            node.LeftNode = request.Parser.TryToParse(referenceRule, request.Token);
            if (node.LeftNode is null) return null;

            node.RightNode = request.Parser.TryToParse(this, token);
            if (node.RightNode is IContainer container)
            {
                node.Ende = container.Ende;

                return node;
            }

            node.RightNode = request.Parser.TryToParse(referenceRule, token);
            node.Ende = token;

            if (node.RightNode is null) return null;

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (this.LeftNode is not IIndexNode leftNode) return request.Index.CreateError(this);
            if (this.RightNode is not IIndexNode rightNode) return request.Index.CreateError(this);

            if (!leftNode.Indezieren(request)) return request.Index.CreateError(this);

            IndexVariabelnReference? reference = null;
            if (request.Parent is IndexContainer container) reference = container.VariabelnReferences.LastOrDefault();
            if (request.Parent is IndexVariabelnReference varRef) reference = varRef.VariabelnReferences.LastOrDefault();
            if (reference is null) return request.Index.CreateError(this);

            rightNode.Indezieren(new RequestParserTreeIndezieren(request.Index, reference));

            //container.VariabelnReferences.Add(reference);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.RightNode is not ICompileNode rightNode) return false;

            if (this.RightNode is ReferenceCall rct)
            {
                if (rct.Reference is null) return false;

                if (rct.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsdek)
                    if (!this.CompileNonStaticCallt(request.Compiler, request.Mode, pgsdek)) return true;
            }

            this.CompileLeftNodeIfNotStaticClass(request.Compiler, request.Mode);

            if (request.Mode == "copy") return true;

            if (this.RightNode is ReferenceCall rctu)
            {
                if (rctu.Reference is null) return false;

                if (rctu.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsdek)
                    this.CompileNonStaticCall(request.Compiler, request.Mode, pgsdek);


                if (rctu.Reference.Deklaration is IndexMethodDeklaration dek) request.Mode = "methode";

                if (rctu.Reference.Deklaration is IndexVektorDeklaration vdek) request.Mode = request.Mode == "point" || request.Mode == "vektorcall" ? "vektorcall" : "setvektorcall";
            }

            string moderesult = "point";
            if (request.Mode == "set") moderesult = "setpoint";

            if (request.Mode == "methode")
            {
                if (this.RightNode is ReferenceCall rc)
                {
                    moderesult = request.Mode;

                    if (rc.Reference is null) return false;

                    if (rc.Reference.Deklaration is IndexMethodDeklaration dek) this.CompileNonStaticCall(request.Compiler, "default", dek);
                }
            }

            if (request.Mode == "vektorcall" || request.Mode == "setvektorcall")
            {
                if (this.RightNode is ReferenceCall rc)
                {
                    moderesult = request.Mode;

                    if (rc.Reference is null) return false;

                    if (rc.Reference.Deklaration is IndexVektorDeklaration dek)
                        this.CompileNonStaticCall(request.Compiler, "default", dek);
                }
            }

            rightNode.Compile(new RequestParserTreeCompile(request.Compiler, moderesult));

            return true;
        }

        private bool CompileLeftNodeIfNotStaticClass(Compiler.Compiler compiler, string mode)
        {
            if (this.LeftNode is not ICompileNode leftNode) return false;

            if (this.RightNode is ReferenceCall rct)
            {
                if (rct.Reference is null) return false;

                if (rct.Reference.Deklaration is IndexMethodDeklaration t)
                    if (t.Type == MethodeType.Static) return true;

                if (rct.Reference.Deklaration is IndexVektorDeklaration vd)
                    if (vd.Type == MethodeType.VektorStatic) return true;

                if (rct.Reference.Deklaration is IndexPropertyGetSetDeklaration pgsd)
                    if (pgsd.Type == MethodeType.VektorStatic) return true;

                if (rct.Reference.Deklaration is IndexPropertyDeklaration klu)
                    if (klu.Zusatz == MethodeType.Static) return true;
            }

            return leftNode.Compile(new RequestParserTreeCompile(compiler, mode == "point" ? mode : "default"));
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

                VektorCall call = new VektorCall(5, new ParserLayer("a"));
                call.Token = this.Token;
                call.LeftNode = this;

                call.Compile(new RequestParserTreeCompile(compiler, mode));

                return false;
            }

            return true;
        }

        #endregion methods
    }
}