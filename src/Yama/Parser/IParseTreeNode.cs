using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
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

        List<IParseTreeNode> GetAllChilds
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        IParseTreeNode Parse ( Parser parser, SyntaxToken token );

        // -----------------------------------------------

        bool Indezieren(Index.Index index, IParent parent);

        // -----------------------------------------------

        bool Compile ( Compiler.Compiler compiler, string mode = "default" );

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --