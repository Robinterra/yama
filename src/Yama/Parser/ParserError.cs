using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ParserError : IParseTreeNode, IIndexNode, ICompileNode
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get;
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
            this.GetAllChilds = new List<IParseTreeNode>();
        }

        public ParserError(IdentifierToken token)
        {
            this.Token = token;
            this.OutputNode = new ParserSyntaxError(token.Text, token);
            this.AllTokens = new() { token };
            this.GetAllChilds = new List<IParseTreeNode>();
        }

        public ParserError(IdentifierToken token, string errorMessage, params IdentifierToken[] tokens)
        {
            this.Token = token;
            this.OutputNode = new ParserSyntaxError(errorMessage, token);
            this.AllTokens = new() { token };
            this.AllTokens.AddRange(tokens);
            this.GetAllChilds = new List<IParseTreeNode>();
        }

        public ParserError(IdentifierToken token, string errorMessage, List<IParseTreeNode> childs, params IdentifierToken[] tokens)
        {
            this.Token = token;
            this.OutputNode = new ParserSyntaxError(errorMessage, token);
            this.AllTokens = new() { token };
            this.AllTokens.AddRange(tokens);
            this.GetAllChilds = childs;
        }

        #endregion ctor

        #region methods

        public virtual IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            return new ParserError(request.Token);
        }

        public bool Indezieren ( RequestParserTreeIndezieren request )
        {
            return request.Index.CreateError(this);
        }

        public bool Compile ( RequestParserTreeCompile request)
        {
            return false;
        }

        #endregion methods
    }
}