using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;

namespace Yama.Parser
{
    public class KlassenDeklaration : IParseTreeNode//, IPriority
    {

        #region get/set

        public IndexKlassenDeklaration Deklaration
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

        public SyntaxToken ClassDefinition
        {
            get;
            set;
        }

        public IParseTreeNode GenericDefintion
        {
            get;
            set;
        }

        public SyntaxToken InheritanceBase
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

                result.Add ( this.Statement );

                return result;
            }
        }

        public List<SyntaxKind> Ausnahmen
        {
            get;
        }
        public ParserLayer NextLayer { get; }

        #endregion get/set

        #region ctor

        public KlassenDeklaration()
        {

        }

        public KlassenDeklaration(ParserLayer nextLayer)
        {
            this.NextLayer = nextLayer;
        }

        public KlassenDeklaration ( int prio )
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
            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;

            return false;
        }

        private bool CheckHashValidClass ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Class) return true;

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

            return false;
        }

        private SyntaxToken MakeAccessValid( Parser parser, SyntaxToken token, KlassenDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        private SyntaxToken MakeInheritanceBase( Parser parser, SyntaxToken token, KlassenDeklaration deklaration)
        {
            if (token.Kind != SyntaxKind.DoublePoint) return token;

            token.Node = deklaration;

            token = parser.Peek(token, 1);

            if (token.Kind != SyntaxKind.Word) return token;

            token.Node = deklaration;

            deklaration.InheritanceBase = token;

            return parser.Peek(token, 1);
        }

        private SyntaxToken MakeZusatzValid( Parser parser, SyntaxToken token, KlassenDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {

            KlassenDeklaration deklaration = new KlassenDeklaration();

            token = this.MakeAccessValid(parser, token, deklaration);

            token = this.MakeZusatzValid ( parser, token, deklaration );

            if ( !this.CheckHashValidClass ( token ) ) return null;

            deklaration.ClassDefinition = token;

            token = parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = parser.Peek ( token, 1 );

            token = this.MakeInheritanceBase ( parser, token, deklaration );

            deklaration.Statement = parser.ParseCleanToken(token, this.NextLayer);

            if (deklaration.Statement == null) return null;

            return this.CleanUp(deklaration);
        }

        private KlassenDeklaration CleanUp(KlassenDeklaration deklaration)
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
            deklaration.ClassDefinition.Node = deklaration;
            deklaration.ClassDefinition.ParentNode = deklaration;
            deklaration.Token.Node = deklaration;

            return deklaration;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexNamespaceDeklaration dek)) return index.CreateError(this, "Kein Namespace als Parent dieser Klasse");

            IndexKlassenDeklaration deklaration = new IndexKlassenDeklaration();
            deklaration.Name = this.Token.Text;
            deklaration.Use = this;

            if (this.InheritanceBase != null) deklaration.InheritanceBase = new IndexVariabelnReference { Name = this.InheritanceBase.Text, Use = this };

            this.Deklaration = deklaration;

            dek.KlassenDeklarationen.Add(deklaration);
            foreach (IParseTreeNode node in this.Statement.GetAllChilds)
            {
                node.Indezieren(index, deklaration);
            }

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            foreach(IndexMethodDeklaration m in this.Deklaration.StaticMethods)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(compiler, mode);
            }

            foreach(IndexMethodDeklaration m in this.Deklaration.Operators)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(compiler, mode);
            }

            foreach(IndexMethodDeklaration m in this.Deklaration.Methods)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(compiler, mode);
            }

            foreach(IndexPropertyDeklaration m in this.Deklaration.IndexProperties)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(compiler, mode);
            }

            foreach(IndexMethodDeklaration m in this.Deklaration.Ctors)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(compiler, mode);
            }

            foreach(IndexMethodDeklaration m in this.Deklaration.DeCtors)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(compiler, mode);
            }

            foreach(IndexVaktorDeklaration m in this.Deklaration.VektorDeclaration)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(compiler, mode);
            }

            return true;
        }

        #endregion methods
    }
}