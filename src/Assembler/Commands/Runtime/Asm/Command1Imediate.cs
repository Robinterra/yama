using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class Command1Imediate : ICommand
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
        public int MaxSize { get; }
        public bool IsCmp
        {
            get;
        }

        public IParseTreeNode Node
        {
            get;
            set;
        }

        public ConditionMode Condition
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Command1Imediate(string key, IFormat format, uint id, int size, int maxsize, bool iscmp = false, ConditionMode condition = ConditionMode.Always)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.MaxSize = maxsize;
            this.IsCmp = iscmp;
            this.Condition = condition;
        }

        public Command1Imediate(Command1Imediate t, IParseTreeNode node, List<byte> bytes)
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
            if (!(request.Node is CommandWith2ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.NumberToken) return false;

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Condition = this.Condition;
            if (!this.IsCmp)
                assembleFormat.RegisterDestionation = request.GetRegister(t.Argument0.Token.Text);
            else
                assembleFormat.RegisterInputLeft = request.GetRegister(t.Argument0.Token.Text);

            assembleFormat.Immediate = Convert.ToUInt32(t.Argument1.Token.Value);

            if (!Format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new Command1Imediate(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith2ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.NumberToken) return false;

            uint valueOfImmediate = Convert.ToUInt32(t.Argument1.Token.Value);
            if (valueOfImmediate > this.MaxSize) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}