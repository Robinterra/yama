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

        IdentifierToken Token
        {
            get;
            set;
        }

        // -----------------------------------------------

        List<IParseTreeNode> GetAllChilds
        {
            get;
        }

        // -----------------------------------------------

        List<IdentifierToken> AllTokens
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        IParseTreeNode Parse ( Request.RequestParserTreeParser request );

        // -----------------------------------------------

        bool Indezieren ( Request.RequestParserTreeIndezieren request );

        // -----------------------------------------------

        bool Compile ( Request.RequestParserTreeCompile request );

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --