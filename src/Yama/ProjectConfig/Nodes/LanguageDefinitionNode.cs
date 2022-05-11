using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.ProjectConfig.Nodes
{

    public class LanguageDefinitionNode : IDeserialize
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

        public LanguageDefinitionNode (  )
        {
            this.Token = new();
            this.ValueToken = new();
            this.AllTokens = new List<IdentifierToken>();
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool Deserialize(RequestDeserialize request)
        {
            if (this.ValueToken.Value == null) return false;

            string? path = this.ValueToken.Value.ToString();
            if (path == null) return false;
            if (request.Project.Directory == null) return false;

            FileInfo directory = new FileInfo(Path.Combine( request.Project.Directory.FullName, path));

            request.Project.LanguageDefinitions.Add(directory);

            return true;
        }

        // -----------------------------------------------

        public IParseTreeNode? Parse(RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;
            if (request.Token.Text.ToLower() != "languageDefinition") return null;

            SourcePathsNode result = new SourcePathsNode();
            result.AllTokens.Add(request.Token);
            result.Token = request.Token;

            IdentifierToken? token = request.Parser.Peek(result.Token, 1);
            if (token is null) return null;
            if (token.Kind != IdentifierKind.DoublePoint) return new ParserError(token, $"Expectet a ':' and not a {token.Kind}", result.AllTokens.ToArray());
            result.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token is null) return null;
            if (token.Kind != IdentifierKind.Text) return new ParserError(token, $"Expectet a \"Text\" and not a {token.Kind}", result.AllTokens.ToArray());
            result.AllTokens.Add(token);
            result.ValueToken = token;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}