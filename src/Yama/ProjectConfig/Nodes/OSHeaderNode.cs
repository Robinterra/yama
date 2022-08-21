using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.ProjectConfig.Nodes
{

    public class OSHeaderNode : IDeserialize
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

        public OSHeaderNode (  )
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

            string? osHeader = this.ValueToken.Value.ToString();
            if (osHeader is null) return false;
            if (!Enum.TryParse<Project.OSHeader>(osHeader, out Project.OSHeader osheader)) return false;

            request.Project.OsHeader = osheader;

            return true;
        }

        // -----------------------------------------------

        public IParseTreeNode? Parse(RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;
            if (request.Token.Text.ToLower() != "osheader") return null;

            OSHeaderNode result = new OSHeaderNode();
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