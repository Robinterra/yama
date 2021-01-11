using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWord : ICommand
    {

        #region get/set

        public string Key
        {
            get;
        }

        public string Format
        {
            get;
        }

        public uint CommandId
        {
            get;
            set;
        }
        public byte[] Data
        {
            get;
            set;
        }

        public ICompileRoot CompileElement
        {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }
        public IParseTreeNode Node
        {
            get;
            set;
        }
        public uint Condition { get; set; }

        #endregion get/set

        #region ctor

        public CommandWord()
        {

        }

        public CommandWord(CommandWord t, IParseTreeNode node, List<byte> bytes)
        {
            this.Key = t.Key;
            this.Format = t.Format;
            this.CommandId = t.CommandId;
            this.Size = t.Size;
            this.Node = node;
            this.Data = bytes.ToArray();
        }

        #endregion ctor

        public bool Assemble(RequestAssembleCommand request)
        {
            if (!(request.Node is WordNode t)) return false;

            List<byte> daten = new List<byte>();

            this.Size = 4;

            byte[] data = BitConverter.GetBytes((uint)t.Data.Value);

            daten.AddRange(data);

            if (request.WithMapper) request.Result.Add(new CommandWord(this, request.Node, daten));
            request.Stream.Write(daten.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (!(request.Node is WordNode t)) return false;

            this.Size = 4;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}