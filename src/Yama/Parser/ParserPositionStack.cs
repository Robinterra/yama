using System.Collections.Generic;

namespace Yama.Parser
{
    public struct ParserPositionStack
    {

        public int Position;

        public int Start;

        public int Max;

        public List<IParseTreeNode> PossibleParentsTemp;

        public ParserPositionStack(Parser parser, List<IParseTreeNode> possibleParents)
        {
            this.Position = parser.Position;
            this.Start = parser.Start;
            this.Max = parser.Max;
            this.PossibleParentsTemp = possibleParents;
        }

        public bool SetToParser(Parser parser)
        {
            parser.Position = this.Position;
            parser.Start = this.Start;
            parser.Max = this.Max;
            parser.possibleParents = this.PossibleParentsTemp;

            return true;
        }

    }
}