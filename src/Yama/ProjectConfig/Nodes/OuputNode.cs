using System.Collections.Generic;
using System.IO;
using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.ProjectConfig.Nodes
{
    public class OutputNode : IDeserialize
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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public OutputNode (  )
        {
            this.AllTokens = new List<IdentifierToken>();
            this.Token = new();
            this.ValueToken = new();
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
            if (this.ValueToken.Value == null) return false;

            string? path = this.ValueToken.Value.ToString();
            if (path == null) return false;

            FileInfo directory = new FileInfo(path);

            request.Project.OutputFile = directory;

            return true;
        }

        // -----------------------------------------------

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            throw new System.NotImplementedException();
        }

        // -----------------------------------------------

        public IParseTreeNode? Parse(RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;
            if (request.Token.Text.ToLower() != "out") return null;

            OutputNode result = new OutputNode();
            result.AllTokens.Add(request.Token);
            result.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(result.Token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.DoublePoint) return null;
            result.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Text) return null;
            result.AllTokens.Add(token);
            result.ValueToken = token;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}