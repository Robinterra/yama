using System.Collections.Generic;
using System.IO;
using System.Text;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class ConditionalCompilationNode : IParseTreeNode, IIndexNode, ICompileNode, IContainer
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
                if (this.AssigmentNode is not null) result.Add(this.AssigmentNode);

                return result;
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

        public IParseTreeNode? AssigmentNode
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.AssigmentNode is IContainer con) return con.Ende;

                return this.Token;
            }
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

            if (node.Token.Text.Contains("#defalgo")) return this.FinishDefAlgo(request, node);
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

        private IParseTreeNode? FinishDefAlgo(RequestParserTreeParser request, ConditionalCompilationNode node)
        {
            IdentifierToken? assigmentToken = request.Parser.Peek(request.Token, 1);
            if (assigmentToken is null) return node;
            if (assigmentToken.Kind != IdentifierKind.Operator) return node;
            if (assigmentToken.Text != "=") return node;

            IParseTreeNode? assigmentRule = request.Parser.GetRule<AssigmentNode>();
            if (assigmentRule is null) return null;
            IParseTreeNode? assimgentNode = request.Parser.TryToParse(assigmentRule, assigmentToken);
            if (assimgentNode is not IParentNode parent) return null;

            node.AssigmentNode = assimgentNode;
            request.Parser.SetChild(parent, node);

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (this.AssigmentNode is IIndexNode indexNode) return indexNode.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.AssigmentNode is ICompileNode compileNode) compileNode.Compile(request);

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