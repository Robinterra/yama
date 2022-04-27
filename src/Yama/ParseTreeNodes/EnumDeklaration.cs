using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;

namespace Yama.Parser
{
    public class EnumDeklaration : IParseTreeNode//, IPriority
    {

        #region get/set

        public IndexEnumDeklaration? Deklaration
        {
            get;
            set;
        }

        public IdentifierToken? AccessDefinition
        {
            get;
            set;
        }

        public IdentifierToken? ClassDefinition
        {
            get;
            set;
        }

        public IParseTreeNode? Statement
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Statement is not null) result.Add ( this.Statement );

                return result;
            }
        }

        public ParserLayer NextLayer
        {
            get;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public EnumDeklaration(ParserLayer nextLayer)
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.NextLayer = nextLayer;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidName ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;

            return false;
        }

        private bool CheckHashValidClass ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Enum) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Public) return true;
            if (token.Kind == IdentifierKind.Private) return true;

            return false;
        }

        private IdentifierToken? MakeAccessValid( Parser parser, IdentifierToken token, EnumDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            EnumDeklaration deklaration = new EnumDeklaration(this.NextLayer);

            IdentifierToken? token = this.MakeAccessValid(request.Parser, request.Token, deklaration);
            if (token is null) return null;
            if ( !this.CheckHashValidClass ( token ) ) return null;

            deklaration.ClassDefinition = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return new ParserError(request.Token, $"Can not find a Name after a enum deklaration", request.Token);
            if ( !this.CheckHashValidName ( token ) ) return new ParserError(token, $"Expectet a name for the enum and not a '{token.Text}'", token, request.Token);

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return new ParserError(request.Token, $"Can not find a '{{' after the enum name", deklaration.Token);

            deklaration.Statement = request.Parser.ParseCleanToken(token, this.NextLayer);
            if (deklaration.Statement is null) return new ParserError(request.Token, $"Can not find a enum Statement after the enum name", deklaration.Token);

            return deklaration;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexNamespaceDeklaration dek) return request.Index.CreateError(this, "Kein Namespace als Parent dieses Enums");
            if (this.Statement is null) return request.Index.CreateError(this);

            IndexEnumDeklaration deklaration = new IndexEnumDeklaration(this, this.Token.Text);

            this.Deklaration = deklaration;

            dek.EnumDeklarationen.Add(deklaration);
            foreach (IParseTreeNode node in this.Statement.GetAllChilds)
            {
                node.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, deklaration));
            }

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return true;
        }

        #endregion methods
    }
}