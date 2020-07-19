using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;

namespace Yama.Parser
{
    public class FunktionsDeklaration : IParseTreeNode//, IPriority
    {

        #region get/set

        public IndexMethodDeklaration Deklaration
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

        public IParseTreeNode GenericDefintion
        {
            get;
            set;
        }

        public List<IParseTreeNode> Parameters
        {
            get;
            set;
        }

        public IParseTreeNode Statement
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

                result.AddRange ( this.Parameters );
                result.Add ( this.Statement );

                return result;
            }
        }

        public List<SyntaxKind> Ausnahmen
        {
            get;
        }

        #endregion get/set

        #region ctor

        public FunktionsDeklaration()
        {

        }

        public FunktionsDeklaration ( int prio )
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

        private SyntaxToken MakeAccessValid( Parser parser, SyntaxToken token, FunktionsDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        private SyntaxToken MakeZusatzValid( Parser parser, SyntaxToken token, FunktionsDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {

            FunktionsDeklaration deklaration = new FunktionsDeklaration();

            token = this.MakeAccessValid(parser, token, deklaration);

            token = this.MakeZusatzValid ( parser, token, deklaration );

            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;

            token = parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(SyntaxKind.OpenKlammer, SyntaxKind.CloseKlammer);

            if ( token == null ) return null;

            IParseTreeNode klammer = rule.Parse(parser, token);

            if (klammer == null) return null;
            if (!(klammer is Container t)) return null;

            t.Token.ParentNode = deklaration;
            deklaration.Parameters = t.Statements;

            SyntaxToken Statementchild = parser.Peek ( t.Ende, 1);

            deklaration.Statement = parser.ParseCleanToken(Statementchild);

            if (deklaration.Statement == null) return null;

            return this.CleanUp(deklaration);
        }

        private FunktionsDeklaration CleanUp(FunktionsDeklaration deklaration)
        {
            deklaration.Statement.Token.ParentNode = deklaration;

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

            IndexMethodDeklaration deklaration = new IndexMethodDeklaration();
            deklaration.Use = this;
            deklaration.Name = this.Token.Text;
            deklaration.ReturnValue = new IndexVariabelnReference { Name = this.TypeDefinition.Text, Use = this };
            this.Deklaration = deklaration;

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == SyntaxKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Type = this.GetMethodeType();

            IndexContainer container = new IndexContainer();
            deklaration.Container = container;

            foreach (IParseTreeNode par in this.Parameters)
            {
                if (!par.Indezieren(index, container)) continue;

                deklaration.Parameters.Add(container.VariabelnDeklarations.Last());
            }

            this.AddMethode(klasse, deklaration);

            this.Statement.Indezieren(index, container);

            return true;
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexMethodDeklaration deklaration)
        {
            if (deklaration.Type == MethodeType.Ctor) klasse.Ctors.Add(deklaration);
            if (deklaration.Type == MethodeType.DeCtor) klasse.DeCtors.Add(deklaration);
            if (deklaration.Type == MethodeType.Operator) klasse.Operators.Add(deklaration);
            if (deklaration.Type == MethodeType.Methode) klasse.Methods.Add(deklaration);
            if (deklaration.Type == MethodeType.Static) klasse.StaticMethods.Add(deklaration);
            if (deklaration.Type == MethodeType.Implicit) klasse.Operators.Add(deklaration);
            if (deklaration.Type == MethodeType.Explicit) klasse.Operators.Add(deklaration);

            return true;
        }

        #endregion methods
    }
}