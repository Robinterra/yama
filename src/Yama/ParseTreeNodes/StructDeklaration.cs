using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class StructDeklaration : IParseTreeNode, IIndexNode, ICompileNode//, IPriority
    {

        #region get/set

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

        public StructDeklaration(ParserLayer nextLayer)
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

            return false;
        }

        private bool CheckHashValidClass ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Struct) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Public) return true;
            if (token.Kind == IdentifierKind.Private) return true;

            return false;
        }

        private IdentifierToken? MakeAccessValid( Parser parser, IdentifierToken token, StructDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        public IParseTreeNode? Parse ( RequestParserTreeParser request )
        {
            StructDeklaration deklaration = new StructDeklaration(this.NextLayer);

            IdentifierToken? token = this.MakeAccessValid(request.Parser, request.Token, deklaration);
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

            deklaration.Statement = request.Parser.ParseCleanToken(token, this.NextLayer, false);

            if (deklaration.Statement is null) return new ParserError(deklaration.Token, $"Can not find a '{{' after the class name", request.Token, deklaration.ClassDefinition);

            return deklaration;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexNamespaceDeklaration dek) return request.Index.CreateError(this, "Kein Namespace als Parent dieser Klasse");
            if (this.Statement is null) return request.Index.CreateError(this);

            IndexKlassenDeklaration deklaration = new IndexKlassenDeklaration(this, this.Token.Text);
            deklaration.MemberModifier = ClassMemberModifiers.Struct;
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
                if (m.Klasse is null) return false;
                if (!m.Klasse.Equals(this.Deklaration)) continue;

                if (m.Use is not ICompileNode compileNode) continue;
                compileNode.Compile(request);
            }

            foreach (IndexPropertyDeklaration m in this.Deklaration.IndexProperties)
            {
                if (m.Klasse is null) continue;
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