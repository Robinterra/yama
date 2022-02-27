using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public string? Tag
        {
            get;
            set;
        }

        public CompileRegionAsm RegionAsm
        {
            get;
            set;
        } = new CompileRegionAsm();

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ConditionalCompilationNode (  )
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.ConditionalCompilation ) return null;

            ConditionalCompilationNode node = new ConditionalCompilationNode();

            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            if (!node.Token.Text.Contains("#tag")) return node;

            request.Parser.MethodTag.Add ( node );

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(node.Token.Text));
            Lexer.Lexer lexer = new Lexer.Lexer(stream);
            lexer.LexerTokens.Add(new Lexer.Text(new ZeichenKette("("), new ZeichenKette(")"), null));

            IdentifierToken? token = lexer.NextFindMatch();
            if (token is null) return null;
            if (token.Value is null) return null;

            node.Tag = token.Value.ToString();

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