using System.Collections.Generic;
using System.IO;
using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.ProjectConfig.Nodes
{
    public class StartNamespaceNode : IDeserialize
    {
        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierToken Token
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode>();
            }
        }

        // -----------------------------------------------

        public IdentifierToken ValueToken
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IdentifierToken> SupportTokens
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public StartNamespaceNode (  )
        {
            this.SupportTokens = new List<IdentifierToken>();
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool Compile(RequestParserTreeCompile request)
        {
            throw new System.NotImplementedException();
        }

        // -----------------------------------------------

        public bool Deserialize(RequestDeserialize request)
        {
            string text = this.ValueToken.Value.ToString();

            request.Project.StartNamespace = text;

            return true;
        }

        // -----------------------------------------------

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            throw new System.NotImplementedException();
        }

        // -----------------------------------------------

        public IParseTreeNode Parse(RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;
            if (request.Token.Text.ToLower() != "startnamespace") return null;

            StartNamespaceNode result = new StartNamespaceNode();
            result.SupportTokens.Add(request.Token);
            result.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(result.Token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.DoublePoint) return null;
            result.SupportTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Text) return null;
            result.SupportTokens.Add(token);
            result.ValueToken = token;

            return this.CleanUp(result);
        }

        // -----------------------------------------------

        private IParseTreeNode CleanUp(StartNamespaceNode node)
        {
            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}