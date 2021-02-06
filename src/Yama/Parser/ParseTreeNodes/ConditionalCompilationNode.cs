using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ConditionalCompilationNode : IParseTreeNode
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
                return new List<IParseTreeNode> (  );
            }
        }

        public CompileRegionAsm RegionAsm
        {
            get;
            set;
        } = new CompileRegionAsm();

        #endregion get/set

        #region ctor

        public ConditionalCompilationNode (  )
        {

        }

        #endregion ctor

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.ConditionalCompilation ) return null;

            ConditionalCompilationNode node = new ConditionalCompilationNode { Token = token };

            token.Node = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            if (this.Token.Text.Contains("#defalgo"))
            {
                CompileRegionDefAlgo regionDefAlgo = new CompileRegionDefAlgo();

                return regionDefAlgo.Compile(compiler, this, mode);;
            }
            if (this.Token.Text.Contains("#region asm")) return this.RegionAsm.Compile(compiler, this, mode);;

            return true;
        }
    }
}