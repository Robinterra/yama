using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Container : IParseTreeNode, IContainer
    {

        private IdentifierToken? ende;

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> Statements
        {
            get;
        }

        public IdentifierKind BeginKind
        {
            get;
            set;
        }

        public IdentifierKind EndeKind
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.ende is null) return this.Token;

                return this.ende;
            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return this.Statements;
            }
        }

        public IndexContainer? IndexContainer
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region  ctor

        public Container (  )
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.Token = new();
            this.Statements = new();
        }

        public Container ( IdentifierKind begin, IdentifierKind ende ) : this()
        {
            this.BeginKind = begin;
            this.EndeKind = ende;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != this.BeginKind ) return null;

            IdentifierToken? kind = request.Parser.FindEndTokenWithoutParse ( request.Token, this.EndeKind, this.BeginKind );
            if ( kind is null ) return null;
            if ( kind.Node is not null ) return null;

            Container expression = new Container (  );

            expression.Token = request.Token;
            expression.AllTokens.Add ( request.Token );
            expression.ende = kind;
            expression.AllTokens.Add(expression.Ende);

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens ( request.Token.Position + 1, kind.Position );

            request.Parser.Repleace ( request.Token, kind.Position );

            if ( nodes is null ) return null;

            expression.Statements.AddRange(nodes);

            return expression;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is IndexNamespaceDeklaration dek) return this.NamespaceIndezi(request);
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);

            IndexContainer indexContainer = new IndexContainer();
            indexContainer.Use = this;
            container.Containers.Add(indexContainer);
            this.IndexContainer = indexContainer;

            foreach (IParseTreeNode node in this.Statements)
            {
                node.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, indexContainer));
            }

            return true;
        }

        private bool NamespaceIndezi(Request.RequestParserTreeIndezieren request)
        {
            foreach (IParseTreeNode node in this.Statements)
            {
                node.Indezieren(request);
            }

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            foreach (IParseTreeNode nodes in this.Statements)
            {
                nodes.Compile(request);
            }

            return true;
        }

        #endregion methods

    }
}