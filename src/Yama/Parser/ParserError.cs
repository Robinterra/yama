using System.Collections.Generic;
using Yama.Index;
using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ParserError : IParseTreeNode
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new ();
            }
        }

        public IOutputNode OutputNode
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ParserError ()
        {
            this.Token = new();
            this.OutputNode = new ParserSyntaxError("error", new IdentifierToken());
            this.AllTokens = new List<IdentifierToken> ();
        }

        public ParserError(IdentifierToken token)
        {
            this.Token = token;
            this.OutputNode = new ParserSyntaxError(token.Text, token);
            this.AllTokens = new() { token };
        }

        public ParserError(IdentifierToken token, string errorMessage, params IdentifierToken[] tokens)
        {
            this.Token = token;
            this.OutputNode = new ParserSyntaxError(errorMessage, token);
            this.AllTokens = new() { token };
            this.AllTokens.AddRange(tokens);
        }

        #endregion ctor

        #region methods

        public virtual IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            return new ParserError(request.Token);
        }

        public bool Indezieren ( Request.RequestParserTreeIndezieren request )
        {
            return request.Index.CreateError(this);
        }

        public bool Compile ( Request.RequestParserTreeCompile request)
        {
            return false;
        }

        #endregion methods
    }
}