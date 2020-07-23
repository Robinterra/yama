using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;

namespace Yama.Parser
{
    public class PropertyDeklaration : IParseTreeNode//, IPriority
    {
        private ParserLayer layer;

        #region get/set

        public IndexPropertyDeklaration Deklaration
        {
            get;
            set;
        }

        public SyntaxToken AccessDefinition
        {
            get;
            set;
        }

        public SyntaxToken ZusatzDefinition
        {
            get;
            set;
        }

        public SyntaxToken TypeDefinition
        {
            get;
            set;
        }

        public IParseTreeNode GetStatement
        {
            get;
            set;
        }

        public IParseTreeNode SetStatement
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        public int Prio
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                result.Add ( this.GetStatement );
                result.Add ( this.SetStatement );

                return result;
            }
        }

        public List<SyntaxKind> Ausnahmen
        {
            get;
        }

        #endregion get/set

        #region ctor

        public PropertyDeklaration()
        {

        }

        public PropertyDeklaration(ParserLayer layer)
        {
            this.layer = layer;
        }

        public PropertyDeklaration ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidName ( SyntaxToken token )
        {
            if (token == null) return false;

            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Operator) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;
            if (token.Kind == SyntaxKind.New) return true;

            return false;
        }

        /*private bool CheckAusnahmen ( SyntaxToken token )
        {
            if (token == null) return false;

            foreach ( SyntaxKind op in this.Ausnahmen )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }*/
        private bool CheckHashValidTypeDefinition ( SyntaxToken token )
        {
            if (token == null) return false;

            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;
            if (token.Kind == SyntaxKind.This) return true;
            if (token.Kind == SyntaxKind.Implicit) return true;
            if (token.Kind == SyntaxKind.Explicit) return true;
            if (token.Kind == SyntaxKind.Void) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Public) return true;
            if (token.Kind == SyntaxKind.Private) return true;

            return false;
        }

        private bool CheckHashValidZusatzDefinition ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Static) return true;
            if (token.Kind == SyntaxKind.OperatorKey) return true;

            return false;
        }

        private SyntaxToken MakeAccessValid( Parser parser, SyntaxToken token, PropertyDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        private SyntaxToken MakeZusatzValid( Parser parser, SyntaxToken token, PropertyDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {

            PropertyDeklaration deklaration = new PropertyDeklaration();

            token = this.MakeAccessValid(parser, token, deklaration);

            token = this.MakeZusatzValid ( parser, token, deklaration );

            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;

            token = parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(SyntaxKind.BeginContainer, SyntaxKind.CloseContainer);

            if ( token == null ) return null;

            IParseTreeNode klammer = rule.Parse(parser, token);

            if (klammer == null) return null;
            if (!(klammer is Container t)) return null;

            if (klammer.GetAllChilds.Count != 2) return null;

            deklaration.SetStatement = klammer.GetAllChilds[0];
            deklaration.GetStatement = klammer.GetAllChilds[1];

            t.Token.ParentNode = deklaration;

            if (deklaration.SetStatement == null) return null;
            if (deklaration.GetStatement == null) return null;

            return this.CleanUp(deklaration);
        }

        private PropertyDeklaration CleanUp(PropertyDeklaration deklaration)
        {
            deklaration.SetStatement.Token.ParentNode = deklaration;
            deklaration.GetStatement.Token.ParentNode = deklaration;

            if (deklaration.AccessDefinition != null)
            {
                deklaration.AccessDefinition.Node = deklaration;
                deklaration.AccessDefinition.ParentNode = deklaration;
            }
            if (deklaration.ZusatzDefinition != null)
            {
                deklaration.ZusatzDefinition.Node = deklaration;
                deklaration.ZusatzDefinition.ParentNode = deklaration;
            }
            deklaration.TypeDefinition.Node = deklaration;
            deklaration.TypeDefinition.ParentNode = deklaration;
            deklaration.Token.Node = deklaration;

            return deklaration;
        }

        public MethodeType GetMethodeType()
        {
            MethodeType type = MethodeType.Methode;

            if (this.ZusatzDefinition != null)
            {
                if (this.ZusatzDefinition.Kind == SyntaxKind.Static) type = MethodeType.Static;
                if (this.ZusatzDefinition.Kind == SyntaxKind.Operator)
                {
                    type = MethodeType.Operator;
                    if (this.TypeDefinition.Kind == SyntaxKind.Explicit) type = MethodeType.Explicit;
                    if (this.TypeDefinition.Kind == SyntaxKind.Implicit) type = MethodeType.Implicit;
                    if (this.TypeDefinition.Kind == SyntaxKind.This)
                    {
                        if (this.Token.Kind == SyntaxKind.New) type = MethodeType.Ctor;
                        if (this.Token.Text == "~") type = MethodeType.DeCtor;
                    }
                }
            }

            return type;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexKlassenDeklaration klasse)) return index.CreateError(this);

            IndexPropertyDeklaration deklaration = new IndexPropertyDeklaration();
            deklaration.Use = this;
            deklaration.Name = this.Token.Text;
            deklaration.Type = new IndexVariabelnReference { Name = this.TypeDefinition.Text, Use = this };
            this.Deklaration = deklaration;

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == SyntaxKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Zusatz = this.GetMethodeType();

            this.AddMethode(klasse, deklaration);

            this.SetStatement.Indezieren(index, deklaration);
            this.GetStatement.Indezieren(index, deklaration);

            return true;
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexPropertyDeklaration deklaration)
        {
            klasse.IndexProperties.Add(deklaration);

            return true;
        }

        #endregion methods
    }
}