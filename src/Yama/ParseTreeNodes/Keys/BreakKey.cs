using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class BreakKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {

        #region get/set

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

                return result;
            }
        }

        public CompileJumpTo JumpTo
        {
            get;
            set;
        } = new CompileJumpTo() { Point = PointMode.LoopEnde };

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public BreakKey()
        {
            this.Ende = this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Break ) return null;

            BreakKey key = new BreakKey (  );
            key.Ende = key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken? maybeSemikolon = request.Parser.Peek(request.Token, 1);
            if (maybeSemikolon is null) return key;
            if (maybeSemikolon.Kind != IdentifierKind.EndOfCommand) return key;

            key.AllTokens.Add(maybeSemikolon);
            key.Ende = maybeSemikolon;

            return key;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            CompileContainer? currentMethod = request.Compiler.ContainerMgmt.CurrentMethod;
            if (currentMethod is null) return false;

            CompileContainer? currentLoop = request.Compiler.ContainerMgmt.CurrentLoop;
            if (currentLoop is null) return false;

            CompileCleanMemory cleanMemory = new CompileCleanMemory();
            if (currentLoop.CurrentNode is ForKey fk) cleanMemory.Compile(request.Compiler, fk);
            if (currentLoop.CurrentNode is WhileKey wk) cleanMemory.Compile(request.Compiler, wk);

            foreach (KeyValuePair<string, SSAVariableMap> varMap in currentMethod.VarMapper)
            {
                if (varMap.Value.Reference is null) continue;

                varMap.Value.LoopBranchReferencesForPhis.Add(varMap.Value.Reference);
            }

            currentLoop.LastContinueOrBreakMaps.Add(currentMethod.VarMapper);

            this.JumpTo.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        #endregion methods

    }
}