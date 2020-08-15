using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Container : IParseTreeNode, IContainer
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> Statements
        {
            get;
            set;
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
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return this.Statements;
            }
        }

        public IndexContainer IndexContainer { get; private set; }

        #endregion get/set

        #region  ctor

        public Container (  )
        {

        }

        public Container ( IdentifierKind begin, IdentifierKind ende )
        {
            this.BeginKind = begin;
            this.EndeKind = ende;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != this.BeginKind ) return null;

            IdentifierToken kind = parser.FindEndTokenWithoutParse ( token, this.EndeKind, this.BeginKind );

            if ( kind == null ) return null;

            if ( kind.Node != null ) return null;

            Container expression = new Container (  );

            expression.Token = token;
            expression.Ende = kind;

            token.Node = expression;
            kind.Node = expression;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens ( token.Position + 1, kind.Position );

            parser.Repleace ( token, kind.Position );

            if ( nodes == null ) return null;

            expression.Statements = nodes;

            foreach ( IParseTreeNode node in nodes )
            {
                node.Token.ParentNode = expression;
            }

            return expression;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (parent is IndexNamespaceDeklaration dek) return this.NamespaceIndezi(index, dek);
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexContainer indexContainer = new IndexContainer();
            indexContainer.Use = this;
            container.Containers.Add(indexContainer);
            this.IndexContainer = indexContainer;

            foreach (IParseTreeNode node in this.Statements)
            {
                node.Indezieren(index, indexContainer);
            }

            return true;
        }

        private bool NamespaceIndezi(Index.Index index, IndexNamespaceDeklaration dek)
        {
            foreach (IParseTreeNode node in this.Statements)
            {
                node.Indezieren(index, dek);
            }

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            foreach (IParseTreeNode nodes in this.Statements)
            {
                nodes.Compile(compiler, mode);
            }

            return true;
        }

        #endregion methods

    }
}