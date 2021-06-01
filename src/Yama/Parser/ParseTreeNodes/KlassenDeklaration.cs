using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class KlassenDeklaration : IParseTreeNode//, IPriority
    {

        #region get/set

        public CompileData compile
        {
            get;
            set;
        } = new CompileData();

        public IndexKlassenDeklaration Deklaration
        {
            get;
            set;
        }

        public IdentifierToken AccessDefinition
        {
            get;
            set;
        }

        public IdentifierToken ZusatzDefinition
        {
            get;
            set;
        }

        public IdentifierToken ClassDefinition
        {
            get;
            set;
        }

        public IParseTreeNode GenericDefintion
        {
            get;
            set;
        }

        public IdentifierToken InheritanceBase
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

        public int Prio
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.GenericDefintion != null) result.Add ( this.GenericDefintion );
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

        private bool CheckHashValidName ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Operator) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Char) return true;
            if (token.Kind == IdentifierKind.Byte) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;
            if (token.Kind == IdentifierKind.Float32Bit) return true;

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
        private bool CheckHashValidTypeDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Char) return true;
            if (token.Kind == IdentifierKind.Byte) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;
            if (token.Kind == IdentifierKind.Float32Bit) return true;

            return false;
        }

        private bool CheckHashValidClass ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Class) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Public) return true;
            if (token.Kind == IdentifierKind.Private) return true;

            return false;
        }

        private bool CheckHashValidZusatzDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Static) return true;
            if (token.Kind == IdentifierKind.Primitive) return true;

            return false;
        }

        private IdentifierToken MakeAccessValid( Parser parser, IdentifierToken token, KlassenDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        private IdentifierToken MakeInheritanceBase( Parser parser, IdentifierToken token, KlassenDeklaration deklaration)
        {
            if (token.Kind != IdentifierKind.DoublePoint) return token;

            token.Node = deklaration;

            token = parser.Peek(token, 1);

            if (token.Kind != IdentifierKind.Word) return token;

            token.Node = deklaration;

            deklaration.InheritanceBase = token;

            return parser.Peek(token, 1);
        }

        private IdentifierToken MakeZusatzValid( Parser parser, IdentifierToken token, KlassenDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) )
            {
                deklaration.InheritanceBase = new IdentifierToken(IdentifierKind.Word, token.Position, token.Line, token.Column, "object", "object");

                return token;
            }

            deklaration.ZusatzDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            KlassenDeklaration deklaration = new KlassenDeklaration();

            IdentifierToken token = this.MakeAccessValid(request.Parser, request.Token, deklaration);

            token = this.MakeZusatzValid ( request.Parser, token, deklaration );

            if ( !this.CheckHashValidClass ( token ) ) return null;

            deklaration.ClassDefinition = token;

            token = request.Parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;
            deklaration.Token = token;

            token = request.Parser.Peek ( token, 1 );

            token = this.TryParseGeneric ( request, deklaration, token );

            token = this.MakeInheritanceBase ( request.Parser, token, deklaration );

            deklaration.Statement = request.Parser.ParseCleanToken(token, this.NextLayer);

            if (deklaration.Statement == null) return null;

            return this.CleanUp(deklaration);
        }

        private IdentifierToken TryParseGeneric(RequestParserTreeParser request, KlassenDeklaration deklaration, IdentifierToken token)
        {
            GenericCall genericRule = request.Parser.GetRule<GenericCall>();
            if (genericRule == null) return token;

            IParseTreeNode node = genericRule.Parse(new RequestParserTreeParser(request.Parser, token));
            if (!(node is GenericCall genericCall)) return token;

            deklaration.GenericDefintion = genericCall;

            return request.Parser.Peek(genericCall.Ende, 1);
        }

        private KlassenDeklaration CleanUp(KlassenDeklaration deklaration)
        {
            deklaration.Statement.Token.ParentNode = deklaration;

            if (deklaration.GenericDefintion != null)
            {
                deklaration.GenericDefintion.Token.ParentNode = deklaration;
            }

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

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexNamespaceDeklaration dek)) return request.Index.CreateError(this, "Kein Namespace als Parent dieser Klasse");

            IndexKlassenDeklaration deklaration = new IndexKlassenDeklaration();
            deklaration.Name = this.Token.Text;
            deklaration.Use = this;

            if (this.GenericDefintion != null) this.GenericDefintion.Indezieren(new RequestParserTreeIndezieren(request.Index, deklaration));

            if (this.InheritanceBase != null) deklaration.InheritanceBase = new IndexVariabelnReference { Name = this.InheritanceBase.Text, Use = this };

            this.Deklaration = deklaration;

            dek.KlassenDeklarationen.Add(deklaration);
            foreach (IParseTreeNode node in this.Statement.GetAllChilds)
            {
                node.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, deklaration));
            }

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            foreach(IMethode m in this.Deklaration.StaticMethods)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(request);
            }

            foreach(IndexMethodDeklaration m in this.Deklaration.Operators)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(request);
            }

            foreach(IMethode m in this.Deklaration.Methods)
            {
                if (this.Deklaration.IsMethodsReferenceMode) this.AddAssemblyName(compile, m);

                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(request);
            }

            foreach(IndexPropertyDeklaration m in this.Deklaration.IndexProperties)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(request);
            }

            foreach(IndexPropertyDeklaration m in this.Deklaration.IndexStaticProperties)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(request);
            }

            foreach(IndexMethodDeklaration m in this.Deklaration.Ctors)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(request);
            }

            foreach(IndexMethodDeklaration m in this.Deklaration.DeCtors)
            {
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                m.Use.Compile(request);
            }

            return true;
        }

        private bool AddAssemblyName(CompileData compile, IMethode m)
        {
            if (m is IndexMethodDeklaration md) compile.Data.JumpPoints.Add(md.AssemblyName);
            if (m is IndexPropertyGetSetDeklaration pgsd)
            {
                compile.Data.JumpPoints.Add(pgsd.AssemblyNameGetMethode);
                compile.Data.JumpPoints.Add(pgsd.AssemblyNameSetMethode);
            }
            if (m is IndexVektorDeklaration vd)
            {
                compile.Data.JumpPoints.Add(vd.AssemblyNameGetMethode);
                compile.Data.JumpPoints.Add(vd.AssemblyNameSetMethode);
            }

            return true;
        }

        #endregion methods
    }
}