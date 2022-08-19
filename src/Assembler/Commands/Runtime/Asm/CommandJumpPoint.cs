using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class CommandJumpPoint : ICommand
    {

        #region get/set

        public string Key
        {
            get;
        }

        public IFormat Format
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
        public uint Maximum { get; private set; }
        public uint Condition
        {
            get;
            set;
        }
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CommandJumpPoint(string key, IFormat format, uint id, int size, uint max, uint condition = 0)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Maximum = max;
            this.Condition = condition;
        }

        public CommandJumpPoint(CommandJumpPoint t, IParseTreeNode node, List<byte> bytes)
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
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith1ArgNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;

            string? datenstring = null;
            if (t.Argument0.Token.Value is not null) datenstring = t.Argument0.Token.Value.ToString();
            if (datenstring is null) return false;

            JumpPointMapper? map = request.Assembler.GetJumpPoint(datenstring);
            if (map == null) return false;

            uint target = request.Assembler.BuildJumpSkipper(request.Position, map.Adresse, (uint)this.Size, true);

            if ((target & 0x80000000) != 0x80000000)
            {
                if (target > this.Maximum) return false;
            }
            else
            {
                uint smaleentity = (~this.Maximum) + 1;
                if (target < smaleentity) return false;
            }

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Condition = this.Condition;
            assembleFormat.Immediate = target >> 2;

            if (!Format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new CommandJumpPoint(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith1ArgNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}