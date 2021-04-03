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

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.ConditionalCompilation ) return null;

            ConditionalCompilationNode node = new ConditionalCompilationNode { Token = request.Token };

            node.Token.Node = node;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Token.Text.Contains("#defalgo"))
            {
                CompileRegionDefAlgo regionDefAlgo = new CompileRegionDefAlgo();

                return regionDefAlgo.Compile(request.Compiler, this, request.Mode);
            }
            if (this.Token.Text.Contains("#region asm")) return this.RegionAsm.Compile(request.Compiler, this, request.Mode);

            return true;
        }
    }
}