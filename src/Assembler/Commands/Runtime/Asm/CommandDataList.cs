using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class CommandDataList : ICommand
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

        public CommandDataList()
        {

        }

        public CommandDataList(CommandDataList t, IParseTreeNode node, List<byte> bytes)
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

            foreach (Lexer.IdentifierToken token in t.Arguments)
            {
                JumpPointMapper map = request.Assembler.GetJumpPoint(token.Text);
                uint target = 4;
                if (map != null) target = map.Adresse;
                daten.AddRange(BitConverter.GetBytes(target - 4));
            }

            if (request.WithMapper) request.Result.Add(new CommandDataList(this, request.Node, daten));
            request.Stream.Write(daten.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (!(request.Node is DataNode t)) return false;
            if (t.SupportTokens[0].Text != ".datalist") return false;

            this.Size = t.Arguments.Count << 2;
            request.IsData = true;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}