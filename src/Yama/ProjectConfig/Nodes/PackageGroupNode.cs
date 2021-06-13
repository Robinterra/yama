using System.Collections.Generic;
using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.ProjectConfig.Nodes
{

    public class PackageGroupNode: IDeserialize, IContainer
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

        public IdentifierToken Ende
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

        public List<IdentifierToken> SupportTokens
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ParserLayer Layer
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public PackageGroupNode ( ParserLayer layer )
        {
            this.SupportTokens = new List<IdentifierToken>();
            this.Layer = layer;
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
            Package package = new Package ();
            request.Project.Packages.Add ( package );

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
            if (request.Token.Text.ToLower() != "package") return null;

            PackageGroupNode result = new PackageGroupNode ( this.Layer );
            result.SupportTokens.Add(request.Token);
            result.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(result.Token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.DoublePoint) return null;
            result.SupportTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.BeginContainer) return null;
            result.SupportTokens.Add(token);
            IdentifierToken begin = token;

            token = request.Parser.FindEndToken ( token, IdentifierKind.CloseContainer, IdentifierKind.BeginContainer );
            if (token == null) return null;

            result.SupportTokens.Add ( token );
            result.Ende = token;

            request.Parser.ActivateLayer ( this.Layer );

            List<IParseTreeNode> nodes = request.Parser.ParseCleanTokens ( begin.Position + 1, result.Ende.Position );

            request.Parser.VorherigesLayer ();

            return this.CleanUp(result);
        }

        // -----------------------------------------------

        private IParseTreeNode CleanUp(PackageGroupNode node)
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