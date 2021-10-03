using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class DataNode : IParseTreeNode
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
                return new List<IParseTreeNode>();
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Data
        {
            get;
            set;
        }

        public List<IdentifierToken> Arguments
        {
            get;
            set;
        } = new List<IdentifierToken>();

        #endregion get/set

        #region ctor

        public DataNode ()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        public bool Compile(Parser.Request.RequestParserTreeCompile request)
        {
            return true;
        }
    
        public bool Indezieren(Parser.Request.RequestParserTreeIndezieren request)
        {
            return true;
        }

        public IParseTreeNode Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Base) return null;

            DataNode node = new DataNode();
            node.AllTokens.Add(request.Token);

            IdentifierToken token = request.Parser.Peek(request.Token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Word) return null;

            node.Token = token;
            node.AllTokens.Add(token);
            token = request.Parser.Peek(token, 1);

            if (token == null) return null;
            if (token.Kind != IdentifierKind.Gleich) return null;
            node.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token.Kind == IdentifierKind.Text) node.Data = token;
            else if (token.Kind == IdentifierKind.NumberToken) node.Data = token;
            else if (!this.TryParseList(request.Parser, token, node)) return null;

            return node;
        }

        private bool TryParseList(Parser.Parser parser, IdentifierToken token, DataNode deklaration)
        {
            if (token.Kind != IdentifierKind.BeginContainer) return false;
            deklaration.AllTokens.Add(token);
            token = parser.Peek(token, 1);

            while (token.Kind == IdentifierKind.Word)
            {
                deklaration.Arguments.Add(token);
                deklaration.AllTokens.Add(token);

                token = parser.Peek(token, 1);

                if (token.Kind != IdentifierKind.Comma) continue;

                deklaration.AllTokens.Add(token);

                token = parser.Peek(token, 1);
            }

            if (token.Kind != IdentifierKind.CloseContainer) return false;
            deklaration.AllTokens.Add(token);

            return true;
        }
    }
}