using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;

namespace Yama.Parser
{
    public class EnumDeklaration : IParseTreeNode//, IPriority
    {

        #region get/set

        public IndexEnumDeklaration Deklaration
        {
            get;
            set;
        }

        public IdentifierToken AccessDefinition
        {
            get;
            set;
        }

        public IdentifierToken ClassDefinition
        {
            get;
            set;
        }

        public IParseTreeNode Statement
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

                result.Add ( this.Statement );

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

        public EnumDeklaration()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public EnumDeklaration(ParserLayer nextLayer)
        {
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

        private IdentifierToken MakeAccessValid( Parser parser, IdentifierToken token, EnumDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            EnumDeklaration deklaration = new EnumDeklaration();

            IdentifierToken token = this.MakeAccessValid(request.Parser, request.Token, deklaration);

            if ( !this.CheckHashValidClass ( token ) ) return null;

            deklaration.ClassDefinition = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );

            deklaration.Statement = request.Parser.ParseCleanToken(token, this.NextLayer);

            if (deklaration.Statement == null) return null;

            return deklaration;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexNamespaceDeklaration dek)) return request.Index.CreateError(this, "Kein Namespace als Parent dieser Klasse");

            IndexEnumDeklaration deklaration = new IndexEnumDeklaration();
            deklaration.Name = this.Token.Text;
            deklaration.Use = this;

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