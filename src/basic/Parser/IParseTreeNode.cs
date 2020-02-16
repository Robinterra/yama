namespace LearnCsStuf.Basic
{
    public interface IParseTreeNode
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        SyntaxToken Token
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        IParseTreeNode Parse ( Parser parser, SyntaxToken token );

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --