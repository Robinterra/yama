using System.Collections.Generic;
using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.ProjectConfig.Nodes
{
    public interface IDeserialize : IParseTreeNode
    {

        bool Deserialize (RequestDeserialize request);

    }
}