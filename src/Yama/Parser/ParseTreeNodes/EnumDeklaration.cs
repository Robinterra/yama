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

        public List<IdentifierKind> Ausnahmen
        {
            get;
        }
        public ParserLayer NextLayer { get; }

        #endregion get/set

        #region ctor

        public EnumDeklaration()
        {

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

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            EnumDeklaration deklaration = new EnumDeklaration();

            IdentifierToken token = this.MakeAccessValid(request.Parser, request.Token, deklaration);

            if ( !this.CheckHashValidClass ( token ) ) return null;

            deklaration.ClassDefinition = token;

            token = request.Parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = request.Parser.Peek ( token, 1 );

            deklaration.Statement = request.Parser.ParseCleanToken(token, this.NextLayer);

            if (deklaration.Statement == null) return null;

            return this.CleanUp(deklaration);
        }

        private EnumDeklaration CleanUp(EnumDeklaration deklaration)
        {
            deklaration.Statement.Token.ParentNode = deklaration;

            if (deklaration.AccessDefinition != null)
            {
                deklaration.AccessDefinition.Node = deklaration;
                deklaration.AccessDefinition.ParentNode = deklaration;
            }
            deklaration.ClassDefinition.Node = deklaration;
            deklaration.ClassDefinition.ParentNode = deklaration;
            deklaration.Token.Node = deklaration;

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