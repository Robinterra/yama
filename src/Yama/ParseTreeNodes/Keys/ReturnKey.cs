using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ReturnKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {
        private ParserLayer expressionLayer;

        #region get/set

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

                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        public CompileJumpTo JumpTo
        {
            get;
            set;
        } = new CompileJumpTo() { Point = PointMode.RootEnde };

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Ende
        {
            get;
            private set;
        }

        #endregion get/set

        #region ctor

        public ReturnKey (ParserLayer expressionLayer)
        {
            this.expressionLayer = expressionLayer;
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Ende = this.Token;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Return ) return null;

            ReturnKey result = new ReturnKey(this.expressionLayer);
            result.Token = request.Token;
            result.AllTokens.Add ( request.Token );

            //IdentifierToken? ende = request.Parser.FindAToken(request.Token, IdentifierKind.EndOfCommand);
            //if (ende is null) return new ParserError(request.Token, "Expectet a ';' after the return statement");

            IdentifierToken? token = request.Parser.Peek(request.Token, 1);
            if (token is null) return null;

            IParseTreeNode? node = request.Parser.ParseCleanToken(token, this.expressionLayer, true);
            result.Statement = node;
            if (node is not IContainer con) return null;

            IdentifierToken? semikolon = request.Parser.Peek(con.Ende, 1);
            if (semikolon is null) return null;
            if (semikolon.Kind != IdentifierKind.EndOfCommand) return null;
            result.AllTokens.Add(semikolon);
            result.Ende = semikolon;

            return result;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is not IIndexNode indexNode) return request.Index.CreateError(this);

            indexNode.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Statement is not ICompileNode compileNode) return false;
            if (request.Compiler.ContainerMgmt.CurrentMethod is null) return true;
            CompileContainer currentMethode = request.Compiler.ContainerMgmt.CurrentMethod;

            this.LefNodeIsStruct(currentMethode.ReturnType, request);
            compileNode.Compile(request);
            request.StructLeftNode = null;

            CompileMovReg movReg = new CompileMovReg();
            SSACompileLine? moveRegLine = movReg.Compile(request.Compiler, this);
            if (moveRegLine is null) return false;

            SSACompileLine?[] tryToCleans = new SSACompileLine[currentMethode.VarMapper.Count];

            CompileCleanMemory cleanMemory = new CompileCleanMemory();
            cleanMemory.Compile(request.Compiler, this, tryToCleans);

            request.Compiler.AssemblerSequence.Add(movReg);
            request.Compiler.AddSSALine(moveRegLine);

            this.JumpTo.Compile(request.Compiler, null, request.Mode);

            if (request.Compiler.ContainerMgmt.CurrentContainer is null) return true;

            request.Compiler.ContainerMgmt.CurrentContainer.HasReturn = true;

            int i = -1;
            foreach (KeyValuePair<string, SSAVariableMap> varilabeMap in request.Compiler.ContainerMgmt.CurrentMethod.VarMapper)
            {
                i = i + 1;

                varilabeMap.Value.Reference = null;
                if (varilabeMap.Value.Kind != SSAVariableMap.VariableType.OwnerReference) continue;

                varilabeMap.Value.Value = SSAVariableMap.LastValue.NeverCall;
                varilabeMap.Value.MutableState = SSAVariableMap.VariableMutableState.NotMutable;
                varilabeMap.Value.First.TryToClean = tryToCleans[i];
            }

            return true;
        }

        private bool LefNodeIsStruct(SSAVariableMap? returnValue, RequestParserTreeCompile request)
        {
            if (returnValue is null) return false;
            if (returnValue.Deklaration.Type.Deklaration is not IndexKlassenDeklaration ikd) return false;
            if (ikd.MemberModifier != ClassMemberModifiers.Struct) return false;

            IndexVariabelnDeklaration dek = new IndexVariabelnDeklaration(this, returnValue.Key, new IndexVariabelnReference(this, returnValue.Key) { Deklaration = ikd });
            request.StructLeftNode = dek;

            return true;
        }

        #endregion methods

    }
}