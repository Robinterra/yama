using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class KlassenDeklaration : IParseTreeNode, IIndexNode, ICompileNode//, IPriority
    {

        #region get/set

        public CompileData VirtualClassData
        {
            get;
        } = new CompileData(DataMode.JumpPointListe);

        public CompileData ReflectionClassData
        {
            get;
        } = new CompileData(DataMode.Reflection);

        public IndexKlassenDeklaration? Deklaration
        {
            get;
            set;
        }

        public IdentifierToken? AccessDefinition
        {
            get;
            set;
        }

        public IdentifierToken? MemberModifier
        {
            get;
            set;
        }

        public IdentifierToken? ClassDefinition
        {
            get;
            set;
        }

        public GenericCall? GenericDefintion
        {
            get;
            set;
        }

        public IdentifierToken? InheritanceBase
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

                if (this.GenericDefintion is not null) result.Add ( this.GenericDefintion );
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

        public KlassenDeklaration(ParserLayer nextLayer)
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.NextLayer = nextLayer;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidName ( IdentifierToken token )
        {
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

        private IdentifierToken? MakeAccessValid( Parser parser, IdentifierToken token, KlassenDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        private IdentifierToken? MakeInheritanceBase( Parser parser, IdentifierToken? token, KlassenDeklaration deklaration)
        {
            if (token is null) return null;
            if (token.Kind != IdentifierKind.DoublePoint) return token;

            deklaration.AllTokens.Add(token);
            token = parser.Peek(token, 1);

            if (token is null) return null;
            if (token.Kind != IdentifierKind.Word) return token;

            deklaration.InheritanceBase = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        private IdentifierToken? MakeZusatzValid( Parser parser, IdentifierToken token, KlassenDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) )
            {
                deklaration.InheritanceBase = new IdentifierToken(IdentifierKind.Word, token.Position, token.Line, token.Column, "object", "object");
                deklaration.AllTokens.Add(deklaration.InheritanceBase);

                return token;
            }

            deklaration.MemberModifier = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        public IParseTreeNode? Parse ( RequestParserTreeParser request )
        {
            KlassenDeklaration deklaration = new KlassenDeklaration(this.NextLayer);

            IdentifierToken? token = this.MakeAccessValid(request.Parser, request.Token, deklaration);
            if (token is null) return null;

            token = this.MakeZusatzValid ( request.Parser, token, deklaration );
            if (token is null) return null;
            if ( !this.CheckHashValidClass ( token ) ) return null;

            deklaration.ClassDefinition = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return new ParserError(deklaration.ClassDefinition, $"Can not find a Name after a class deklaration", request.Token);
            if ( !this.CheckHashValidName ( token ) ) return new ParserError(token, $"Expectet a name for the class and not a '{token.Text}'", token, request.Token);

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return new ParserError(deklaration.Token, $"Can not find a '{{' after the class name", request.Token, deklaration.ClassDefinition);

            token = this.TryParseGeneric ( request, deklaration, token );

            token = this.MakeInheritanceBase ( request.Parser, token, deklaration );
            if (token is null) return new ParserError(deklaration.Token, $"Can not find a '{{' after the class name", request.Token, deklaration.ClassDefinition);

            deklaration.Statement = request.Parser.ParseCleanToken(token, this.NextLayer, false);

            if (deklaration.Statement is null) return new ParserError(deklaration.Token, $"Can not find a '{{' after the class name", request.Token, deklaration.ClassDefinition);

            return deklaration;
        }

        private IdentifierToken? TryParseGeneric(RequestParserTreeParser request, KlassenDeklaration deklaration, IdentifierToken token)
        {
            GenericCall genericRule = request.Parser.GetRule<GenericCall>();

            GenericCall? genericCall = request.Parser.TryToParse ( genericRule, token );
            if (genericCall is null) return token;

            deklaration.GenericDefintion = genericCall;

            return request.Parser.Peek(genericCall.Ende, 1);
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexNamespaceDeklaration dek) return request.Index.CreateError(this, "Kein Namespace als Parent dieser Klasse");
            if (this.Statement is null) return request.Index.CreateError(this);

            if (this.Token.Text == "object") this.InheritanceBase = null;
            IndexKlassenDeklaration deklaration = new IndexKlassenDeklaration(this, this.Token.Text);

            if ( this.MemberModifier != null )
            {
                if ( this.MemberModifier.Kind == IdentifierKind.Static ) deklaration.MemberModifier = ClassMemberModifiers.Static;
                if ( this.MemberModifier.Kind == IdentifierKind.Primitive ) deklaration.MemberModifier = ClassMemberModifiers.Primitive;
            }

            if (this.GenericDefintion != null)
            {
                this.GenericDefintion.Indezieren(new RequestParserTreeIndezieren(request.Index, deklaration));
                deklaration.GenericDeklaration = this.GenericDefintion;
            }

            if (this.InheritanceBase != null) deklaration.InheritanceBase = new IndexVariabelnReference(this, this.InheritanceBase.Text);

            this.Deklaration = deklaration;

            dek.KlassenDeklarationen.Add(deklaration);
            foreach (IParseTreeNode node in this.Statement.GetAllChilds)
            {
                if (node is not IIndexNode indexNode) continue;

                indexNode.Indezieren(new RequestParserTreeIndezieren(request.Index, deklaration));
            }

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Deklaration is null) return false;
            if (this.ReflectionClassData.Data.Refelection is not null) this.ReflectionClassData.Data.Refelection.VirtuelClassData = this.VirtualClassData;

            foreach (IMethode m in this.Deklaration.StaticMethods)
            {
                if (m.Klasse is null) return false;
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }

            foreach (IndexMethodDeklaration m in this.Deklaration.Operators)
            {
                if (m.Klasse is null) return false;
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }

            foreach (IMethode m in this.Deklaration.Methods)
            {
                if (this.Deklaration.IsMethodsReferenceMode) this.AddAssemblyName(this.VirtualClassData, m);
                if (m.Klasse is null) return false;
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }

            foreach (IndexPropertyDeklaration m in this.Deklaration.IndexProperties)
            {
                if (m.Klasse is null) continue;
                if (this.Deklaration.IsMethodsReferenceMode) this.AddPropteryToRefelection(m);
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }

            foreach (IndexPropertyDeklaration m in this.Deklaration.IndexStaticProperties)
            {
                if (m.Klasse is null) continue;
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }

            foreach (IndexMethodDeklaration m in this.Deklaration.Ctors)
            {
                if (m.Klasse is null) return false;
                if (m.Parameters.Count == 1 && this.ReflectionClassData.Data.Refelection is not null) this.ReflectionClassData.Data.Refelection.EmptyCtor = m;
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }

            /*foreach (IndexMethodDeklaration m in this.Deklaration.DeCtors)
            {
                if (m.Klasse is null) return false;
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }*/

            return true;
        }

        private bool AddPropteryToRefelection(IndexPropertyDeklaration m)
        {
            if (this.ReflectionClassData.Data.Refelection is null) return true;

            this.ReflectionClassData.Data.Refelection.AddProperty(m);

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