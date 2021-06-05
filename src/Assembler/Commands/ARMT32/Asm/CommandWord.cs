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

            uint datenValue = this.GetDatenValue(request, t);

            byte[] data = BitConverter.GetBytes(datenValue);

            daten.AddRange(data);

            if (request.WithMapper) request.Result.Add(new CommandWord(this, request.Node, daten));
            request.Stream.Write(daten.ToArray());

            return true;
        }

        private uint GetDatenValue(RequestAssembleCommand request, WordNode t)
        {
            if (t.Data.Kind == Lexer.IdentifierKind.NumberToken) return Convert.ToUInt32(t.Data.Value);
            if (t.Data.Kind != Lexer.IdentifierKind.Word) return 0;

            JumpPointMapper map = request.Assembler.GetJumpPoint(t.Data.Value.ToString());
            if (map == null)
            {
                request.Assembler.Errors.Add(request.Node);

                return 0;
            }

            uint result = map.Adresse;

            if (t.AdditionNumberToken != null && result != 0)
            {
                result = (uint)(result + ((int)t.AdditionNumberToken.Value));
            }

            return result;
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