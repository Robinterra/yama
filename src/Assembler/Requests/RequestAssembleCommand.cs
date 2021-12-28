using System.Collections.Generic;
using System.IO;
using Yama.Parser;

namespace Yama.Assembler
{
    public class RequestAssembleCommand
    {

        #region get/set

        public IParseTreeNode Node
        {
            get;
            set;
        }

        public Assembler Assembler
        {
            get;
            set;
        }

        public Stream Stream
        {
            get;
            set;
        }

        public bool WithMapper
        {
            get;
            set;
        }

        public List<ICommand> Result
        {
            get;
            set;
        } = new List<ICommand>();

        public uint Position
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestAssembleCommand(IParseTreeNode node, Assembler assembler, Stream stream, bool withMapper, uint position)
        {
            this.Node = node;
            this.Assembler = assembler;
            this.Stream = stream;
            this.WithMapper = withMapper;
            this.Position = position;
        }

        #endregion ctor

    }
}