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
        public uint Condition { get; set; }

        #endregion get/set

        #region ctor

        public CommandData()
        {
            this.Key = string.Empty;
            this.Format = string.Empty;
            this.Node = new ParserError();
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
            if (t.Data.Kind == Lexer.IdentifierKind.NumberToken) return this.AssembleNumberData(request);

            List<byte> daten = new List<byte>();

            string? datenstring = null;
            if (t.Data.Value is not null) datenstring = t.Data.Value.ToString();
            if (datenstring is null) return false;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(datenstring);

            int orgsize = data.Length;

            this.Size = data.Length + 4;
            int temp = this.Size & 0x3;
            if (temp >= 0) this.Size = this.Size + 4;
            this.Size = this.Size ^ temp;

            for (int i = 0; i < 4 - (temp == 0 ? 0 : temp); i++)
            {
                datenstring = datenstring + "\0";
            }

            data = System.Text.Encoding.UTF8.GetBytes(datenstring);

            daten.AddRange(BitConverter.GetBytes(orgsize));
            daten.AddRange(data);

            if (request.WithMapper) request.Result.Add(new CommandData(this, request.Node, daten));
            request.Stream.Write(daten.ToArray());

            return true;
        }

        private bool AssembleNumberData(RequestAssembleCommand request)
        {
            if (!(request.Node is DataNode t)) return false;

            List<byte> daten = new List<byte>();

            this.Size = 4;

            uint datenValue = this.GetDatenValue(request, t);

            byte[] data = BitConverter.GetBytes(datenValue);

            daten.AddRange(data);

            if (request.WithMapper) request.Result.Add(new CommandData(this, request.Node, daten));
            request.Stream.Write(daten.ToArray());

            return true;
        }

        private uint GetDatenValue(RequestAssembleCommand request, DataNode t)
        {
            if (t.Data.Kind == Lexer.IdentifierKind.NumberToken) return Convert.ToUInt32(t.Data.Value);
            if (t.Data.Kind != Lexer.IdentifierKind.Word) return 0;

            string? datenstring = null;
            if (t.Data.Value is not null) datenstring = t.Data.Value.ToString();
            if (datenstring is null) return 0;

            JumpPointMapper? map = request.Assembler.GetJumpPoint(datenstring);
            if (map != null) return map.Adresse;

            request.Assembler.Errors.Add(request.Node);

            return 0;
        }

        public bool Identify(RequestIdentify request)
        {
            if (!(request.Node is DataNode t)) return false;
            if (t.AllTokens[0].Text != ".data") return false;
            if (t.Data.Kind == Lexer.IdentifierKind.NumberToken)
            {
                this.Size = 4;
                request.IsData = true;

                return true;
            }

            string? datenstring = null;
            if (t.Data.Value is not null) datenstring = t.Data.Value.ToString();
            if (datenstring is null) return false;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(datenstring);

            this.Size = data.Length + 4;
            int temp = this.Size & 0x3;
            if (temp >= 0) this.Size = this.Size + 4;
            this.Size = this.Size ^ temp;

            request.IsData = true;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}