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

        public Command1Imediate(string key, string format, uint id, int size, uint condition = 0)
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
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

            IFormat format = request.Assembler.GetFormat(this.Format);
            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Arguments.Add(this.Condition);
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument0.Token.Text));
            assembleFormat.Arguments.Add(0);
            assembleFormat.Arguments.Add(Convert.ToUInt32(t.Argument1.Token.Value));

            if (!format.Assemble(assembleFormat)) return false;

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

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}