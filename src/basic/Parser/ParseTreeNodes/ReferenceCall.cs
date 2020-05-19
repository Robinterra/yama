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

        public List<IParseTreeNode> Vectoren
        {
            get;
        } = new List<IParseTreeNode>();

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                result.AddRange(this.Vectoren);

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

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Word ) return null;

            ReferenceCall result = new ReferenceCall (  );

            token.Node = result;

            result.Token = token;

            SyntaxToken checkToken = parser.Peek ( token, 1 );

            if ( checkToken == null ) return result;
            if ( checkToken.Kind != SyntaxKind.EckigeKlammerAuf ) return result;

            while ( checkToken.Kind == SyntaxKind.EckigeKlammerAuf )
            {
                SyntaxToken endToken = parser.FindEndToken ( checkToken, SyntaxKind.EckigeKlammerZu, SyntaxKind.EckigeKlammerAuf );

                if ( endToken == null ) return null;

                IParseTreeNode vectorData = parser.ParseCleanToken ( parser.Peek ( checkToken, 1 ) );//parser.ParseCleanTokens ( checkToken.Position + 1, endToken.Position );

                if ( vectorData == null ) return null;

                checkToken.Node = result;

                endToken.Node = result;

                vectorData.Token.ParentNode = result;

                result.Vectoren.Add ( vectorData );

                checkToken = parser.Peek ( endToken, 1 );

                if ( checkToken == null ) return result;
            }

            return result;
        }

        #endregion methods

    }
}