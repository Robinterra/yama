using System;
using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;

namespace Yama.Parser
{
    public class ReferenceCall : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        public SyntaxToken Token
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
        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public ReferenceCall (  )
        {

        }

        public ReferenceCall ( int prio )
        {
            this.Prio = prio;
        }
        #endregion ctor

        #region methods

        /*private bool ParseVector ( Parser parser )
        {
            SyntaxToken checkToken = parser.Peek ( this.Token, 1 );

            if ( checkToken == null ) return true;
            if ( checkToken.Kind != SyntaxKind.EckigeKlammerAuf ) return true;

            this.Vectoren = new List<IParseTreeNode>();

            while ( checkToken.Kind == SyntaxKind.EckigeKlammerAuf )
            {
                SyntaxToken endToken = parser.FindEndToken ( checkToken, SyntaxKind.EckigeKlammerZu, SyntaxKind.EckigeKlammerAuf );

                if ( endToken == null ) return false;

                IParseTreeNode vectorData = parser.ParseCleanToken ( parser.Peek ( checkToken, 1 ) );//parser.ParseCleanTokens ( checkToken.Position + 1, endToken.Position );

                if ( vectorData == null ) return false;

                checkToken.Node = this;

                endToken.Node = this;

                vectorData.Token.ParentNode = this;

                this.Vectoren.Add ( vectorData );

                checkToken = parser.Peek ( endToken, 1 );

                if ( checkToken == null ) return false;
            }

            return true;
        }*/

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Word ) return null;

            ReferenceCall result = new ReferenceCall (  );

            token.Node = result;

            result.Token = token;

            //int counter = 1;

            /*while ( this.ParseVector ( parser, result, ref counter ) )
            {

            }

            this.ParseGenericType ( parser, result, ref counter );*/

            //this.ParseParameters ( parser, result, counter );

            return result;
        }

        /*private bool ParseParameters(Parser parser, ReferenceCall parent, int count)
        {
            SyntaxToken checkToken = parser.Peek ( this.Token, count );

            if ( checkToken == null ) return true;

            Container container = new Container(SyntaxKind.OpenKlammer, SyntaxKind.CloseKlammer);

            IParseTreeNode node = container.Parse ( parser, checkToken );

            if (node == null) return false;

            node.Token.ParentNode = parent;

            parent.FunctionCall = node;

            return true;
        }*/

        #endregion methods

    }
}