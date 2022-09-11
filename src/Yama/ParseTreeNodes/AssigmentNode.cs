using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class AssigmentNode : IParseTreeNode, IIndexNode, ICompileNode, IContainer, IParentNode
    {

        #region get/set

        public IParseTreeNode? ChildNode
        {
            get;
            set;
        }

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

                if (this.ChildNode != null) result.Add ( this.ChildNode );

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

        public IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        private ParserLayer expressionLayer;

        #endregion get/set

        #region ctor

        public AssigmentNode (ParserLayer expressionLayer)
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.expressionLayer = expressionLayer;
            this.Token = new();
            this.Ende = new();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Operator ) return null;
            if ( request.Token.Text != "=" ) return null;

            AssigmentNode node = new AssigmentNode(this.expressionLayer);
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            IdentifierToken? token = request.Parser.Peek ( request.Token, 1 );
            if (token is null) return null;

            node.ChildNode = request.Parser.ParseCleanToken ( token, this.expressionLayer, false );

            node.Ende = token;
            if (node.ChildNode is IContainer container) node.Ende = container.Ende;

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.ChildNode is not IIndexNode rightNode) return request.Index.CreateError(this);
            if (this.LeftNode is not IIndexNode leftNode) return request.Index.CreateError(this);

            leftNode.Indezieren(request);
            IndexVariabelnReference? varref = container.VariabelnReferences.LastOrDefault();

            rightNode.Indezieren(request);
            IndexVariabelnReference? indexVariabelnReference = container.VariabelnReferences.LastOrDefault();

            if (varref is null) return false;
            if (indexVariabelnReference is null) return false;
            if (leftNode is ConditionalCompilationNode) return true;

            request.Index.IndexTypeSafeties.Add(new IndexSetValue(varref, indexVariabelnReference));

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.ChildNode is not ICompileNode rightNode) return false;
            if (this.LeftNode is not ICompileNode leftnode) return false;

            this.LefNodeIsStruct(leftnode, rightNode, request);

            rightNode.Compile(request);

            if (request.StructLeftNode is not null)
            {
                request.StructLeftNode = null;

                return true;
            }

            return leftnode.Compile(new RequestParserTreeCompile ( request.Compiler, "set" ));
        }

        private bool LefNodeIsStruct(ICompileNode leftnode, ICompileNode rightNode, RequestParserTreeCompile request)
        {
            if (leftnode is VariabelDeklaration vd)
            {
                if (vd.Deklaration is not null)
                {
                    if (vd.Deklaration.Type.Deklaration is IndexKlassenDeklaration kdi)
                    {
                        if (kdi.MemberModifier == ClassMemberModifiers.Struct)
                        {
                            request.StructLeftNode = vd.Deklaration;

                            return true;
                        }
                    }
                }
            }
            if (leftnode is not ReferenceCall rc) return false;
            if (rc.Reference is null) return false;
            if (rc.Reference.Deklaration is not IndexVariabelnDeklaration ivd) return false;
            if (ivd.Type.Deklaration is not IndexKlassenDeklaration ikd) return false;
            if (ikd.MemberModifier != ClassMemberModifiers.Struct) return false;

            request.StructLeftNode = ivd;

            return true;
        }

        #endregion methods
    }
}