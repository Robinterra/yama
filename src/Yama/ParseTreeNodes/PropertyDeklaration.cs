using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;
using Yama.Compiler;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class PropertyDeklaration : IParseTreeNode, IIndexNode, ICompileNode//, IPriority
    {
        private ParserLayer layer;

        #region get/set

        public IndexPropertyDeklaration? Deklaration
        {
            get;
            set;
        }

        public IdentifierToken? AccessDefinition
        {
            get;
            set;
        }

        public IdentifierToken? ZusatzDefinition
        {
            get;
            set;
        }

        public IdentifierToken? TypeDefinition
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

                if (this.GenericDefintion is not null) result.Add(this.GenericDefintion);

                return result;
            }
        }

        public GenericCall? GenericDefintion
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken? BorrowingToken
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public PropertyDeklaration(ParserLayer layer)
        {
            this.AllTokens = new List<IdentifierToken> ();
            this.layer = layer;
            this.Token = new();
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
            if (token.Kind == IdentifierKind.New) return true;

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
            if (token.Kind == IdentifierKind.This) return true;
            if (token.Kind == IdentifierKind.Implicit) return true;
            if (token.Kind == IdentifierKind.Explicit) return true;
            if (token.Kind == IdentifierKind.Void) return true;

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
            if (token.Kind == IdentifierKind.OperatorKey) return true;

            return false;
        }

        private IdentifierToken? MakeAccessValid( Parser parser, IdentifierToken token, PropertyDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        private IdentifierToken? MakeZusatzValid( Parser parser, IdentifierToken token, PropertyDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        public IParseTreeNode? Parse ( RequestParserTreeParser request )
        {
            PropertyDeklaration deklaration = new PropertyDeklaration(this.layer);

            IdentifierToken? token = this.MakeAccessValid( request.Parser, request.Token, deklaration );
            if (token is null) return null;

            token = this.MakeZusatzValid ( request.Parser, token, deklaration );
            if (token is null) return null;

            token = this.TryParseBorrwoing(request.Parser, token, deklaration);
            if (token is null) return null;
            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return null;

            token = this.TryParseGeneric(request, deklaration, token);
            if (token is null) return null;
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if ( token is null ) return null;
            if (token.Kind != IdentifierKind.EndOfCommand) return null;

            deklaration.AllTokens.Add(token);

            return deklaration;
        }

        private IdentifierToken? TryParseGeneric(RequestParserTreeParser request, PropertyDeklaration deklaration, IdentifierToken token)
        {
            GenericCall genericRule = request.Parser.GetRule<GenericCall>();

            GenericCall? genericCall = request.Parser.TryToParse(genericRule, token);
            if (genericCall is null) return token;

            deklaration.GenericDefintion = genericCall;

            return request.Parser.Peek(genericCall.Ende, 1);
        }

        private IdentifierToken? TryParseBorrwoing(Parser parser, IdentifierToken token, PropertyDeklaration node)
        {
            if (token.Kind != IdentifierKind.Operator) return token;
            if (token.Text != "&") return token;

            node.BorrowingToken = token;
            node.AllTokens.Add(token);

            IdentifierToken? nextToken = parser.Peek(token, 1);
            return nextToken;
        }

        public MethodeType GetMethodeType()
        {
            MethodeType type = MethodeType.Property;

            if (this.ZusatzDefinition != null)
            {
                if (this.ZusatzDefinition.Kind == IdentifierKind.Static) type = MethodeType.Static;
            }

            return type;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexKlassenDeklaration klasse) return request.Index.CreateError(this);
            if (this.TypeDefinition is null) return request.Index.CreateError(this);

            IndexVariabelnReference varref = new IndexVariabelnReference(this, this.TypeDefinition.Text);

            IndexPropertyDeklaration deklaration = new IndexPropertyDeklaration(this, this.Token.Text, MethodeType.Property, varref);

            this.Deklaration = deklaration;

            if (this.ZusatzDefinition != null)
            {
                if (this.ZusatzDefinition.Kind == IdentifierKind.Static) deklaration.Zusatz = MethodeType.Static;
            }

            if (this.GenericDefintion is not null)
            {
                deklaration.GenericDeklaration = this.GenericDefintion;
                this.GenericDefintion.Indezieren(new RequestParserTreeIndezieren(request.Index, deklaration.GetContainer));
            }

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == IdentifierKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Zusatz = this.GetMethodeType();

            this.AddMethode(klasse, deklaration);

            //this.SetStatement.Indezieren(index, deklaration);
            //this.GetStatement.Indezieren(index, deklaration);

            return true;
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexPropertyDeklaration deklaration)
        {
            if (deklaration.Zusatz == MethodeType.Static)
            {
                klasse.IndexStaticProperties.Add(deklaration);

                return true;
            }

            klasse.IndexProperties.Add(deklaration);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Deklaration is null) return false;
            if (this.Deklaration.Zusatz != MethodeType.Static) return true;

            CompileData compile = new CompileData(DataMode.Int);
            compile.JumpPointName = this.Deklaration.AssemblyName;
            compile.Compile(request.Compiler, this);

            return true;
        }

        /*private bool CompileGetFunktion(Compiler.Compiler compiler, string mode)
        {
            compiler.Definition.BeginNeueMethode(this.RegisterInUseGet);

            this.CompileContainerGet.Begin = new CompileSprungPunkt();
            this.CompileContainerGet.Ende = new CompileSprungPunkt();
            compiler.SetNewContainer(this.CompileContainerGet);

            this.FunktionsDeklarationCompileGet.Compile(compiler, this, mode);

            foreach(IndexVariabelnDeklaration node in this.Deklaration.ParametersGet)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.CompileIndexNode(compiler, node, "get");
            }

            compiler.Definition.ParaClean();

            this.CompileContainer.Begin.Compile(compiler, this, mode);

            this.Statement.Compile(compiler, mode);

            this.CompileContainer.Ende.Compile(compiler, this, mode);

            this.FunktionsEndeCompile.Compile(compiler, this, mode);

            return true;
        }*/

        #endregion methods
    }
}