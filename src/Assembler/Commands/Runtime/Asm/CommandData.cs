using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class CommandData : ICommand
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

        public CommandData()
        {

        }

        public CommandData(CommandData t, IParseTreeNode node, List<byte> bytes)
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
            if (!(request.Node is DataNode t)) return false;

            List<byte> daten = new List<byte>();
            this.Size = t.Data.Text.Length + 4;
            byte[] data = System.Text.Encoding.UTF8.GetBytes(t.Data.Value.ToString());
            daten.AddRange(BitConverter.GetBytes(data.Length));
            daten.AddRange(data);

            if (request.WithMapper) request.Result.Add(new CommandData(this, request.Node, daten));
            request.Stream.Write(daten.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (!(request.Node is DataNode t)) return false;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(t.Data.Value.ToString());
            this.Size = data.Length + 4;
            request.IsData = true;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}