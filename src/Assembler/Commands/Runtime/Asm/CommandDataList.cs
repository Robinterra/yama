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
        } = new byte[0];

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

        public uint Condition
        {
            get;
            set;
        }

        public uint Subtraction
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CommandDataList(uint subtraction)
        {
            this.Key = string.Empty;
            this.Format = string.Empty;
            this.Node = new ParserError();
            this.Subtraction = subtraction;
        }

        public CommandDataList(CommandDataList t, IParseTreeNode node, List<byte> bytes, uint subtraction)
        {
            this.Key = t.Key;
            this.Format = t.Format;
            this.CommandId = t.CommandId;
            this.Size = t.Size;
            this.Node = node;
            this.Data = bytes.ToArray();
            this.Subtraction = subtraction;
        }

        #endregion ctor

        public bool Assemble(RequestAssembleCommand request)
        {
            if (!(request.Node is DataNode t)) return false;

            List<byte> daten = new List<byte>();

            foreach (Lexer.IdentifierToken token in t.Arguments)
            {
                if (token.Kind == Lexer.IdentifierKind.NumberToken)
                {
                    daten.AddRange(BitConverter.GetBytes((int)token.Value!));
                    continue;
                }

                JumpPointMapper? map = request.Assembler.GetJumpPoint(token.Text);
                uint target = 4;
                if (map != null) target = map.Adresse;
                daten.AddRange(BitConverter.GetBytes(target - this.Subtraction));
            }

            if (request.WithMapper) request.Result.Add(new CommandDataList(this, request.Node, daten, this.Subtraction));
            request.Stream.Write(daten.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (!(request.Node is DataNode t)) return false;
            if (t.AllTokens[0].Text != ".datalist") return false;

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