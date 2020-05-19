using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ReferenceCall : IParseTreeNode, IPriority
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> Parameters
        {
            get;
            set;
        }

        public List<IParseTreeNode> Vectoren
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if ( this.Vectoren != null ) result.AddRange(this.Vectoren);
                if ( this.Parameters != null ) result.AddRange(this.Parameters);

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

        private bool ParseVector ( Parser parser )
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
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Word ) return null;

            ReferenceCall result = new ReferenceCall (  );

            token.Node = result;

            result.Token = token;

            if ( !result.ParseVector ( parser ) ) return null;

            if ( !result.ParseParameters ( parser ) ) return null;

            return result;
        }

        private bool ParseParameters(Parser parser)
        {
            SyntaxToken checkToken = parser.Peek ( this.Token, 1 );

            if ( checkToken == null ) return true;
            if ( checkToken.Kind != SyntaxKind.OpenKlammer ) return true;

            SyntaxToken endToken = parser.FindEndToken ( checkToken, SyntaxKind.CloseKlammer, SyntaxKind.OpenKlammer );

            if ( endToken == null ) return false;

            List<IParseTreeNode> parametersData = parser.ParseCleanTokens ( checkToken.Position + 1, endToken.Position );//parser.ParseCleanTokens ( checkToken.Position + 1, endToken.Position );

            if ( parametersData == null ) return false;

            checkToken.Node = this;

            endToken.Node = this;

            foreach (IParseTreeNode data in parametersData)
            {
                data.Token.ParentNode = this;
            }

            this.Parameters = parametersData;

            return true;
        }

        #endregion methods

    }
}