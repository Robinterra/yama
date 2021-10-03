using System.Collections.Generic;
using System.Linq;
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

        public List<IdentifierToken> AllTokens
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

        public List<IDeserialize> Childs
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public PackageGroupNode ( ParserLayer layer )
        {
            this.AllTokens = new List<IdentifierToken>();
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
            request.Package = package;

            foreach ( IDeserialize child in this.Childs )
            {
                child.Deserialize ( request );
            }

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
            result.AllTokens.Add(request.Token);
            result.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(result.Token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.DoublePoint) return null;
            result.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.BeginContainer) return null;
            result.AllTokens.Add(token);
            IdentifierToken begin = token;

            token = request.Parser.FindEndToken ( token, IdentifierKind.CloseContainer, IdentifierKind.BeginContainer );
            if (token == null) return null;

            result.AllTokens.Add ( token );
            result.Ende = token;

            request.Parser.ActivateLayer ( this.Layer );

            List<IParseTreeNode> nodes = request.Parser.ParseCleanTokens ( begin.Position + 1, result.Ende.Position );
            result.Childs = nodes.Cast<IDeserialize>().ToList();

            request.Parser.VorherigesLayer ();

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}