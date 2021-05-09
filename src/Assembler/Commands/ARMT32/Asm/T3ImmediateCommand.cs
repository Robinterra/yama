using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class T3ImmediateCommand : ICommand
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
        public int Mode { get; private set; }
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public T3ImmediateCommand(string key, string format, uint id, int size, int mode = -1)
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Mode = mode;
        }

        public T3ImmediateCommand(T3ImmediateCommand t, IParseTreeNode node, List<byte> bytes)
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
            if (!(request.Node is CommandWith3ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument2.Token.Kind != Lexer.IdentifierKind.NumberToken) return false;
            if (this.CanBeIgnore(t, request)) return true;

            IFormat format = request.Assembler.GetFormat(this.Format);
            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            if (this.Mode != -1) assembleFormat.Arguments.Add((uint)this.Mode);
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument0.Token.Text));
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument1.Token.Text));
            assembleFormat.Arguments.Add(Convert.ToUInt32(t.Argument2.Token.Value));

            if (!format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new T3ImmediateCommand(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        private bool CanBeIgnore(CommandWith3ArgsNode t, RequestAssembleCommand request)
        {
            if (!request.Assembler.IsOptimizeActive) return false;
            if ((int)t.Argument2.Token.Value != 0) return false;
            if (t.Argument0.Token.Value.ToString() != t.Argument1.Token.Value.ToString()) return false;
            if (!this.IsKeyValid()) return false;

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith3ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument2.Token.Kind != Lexer.IdentifierKind.NumberToken) return false;

            if (!request.Assembler.IsOptimizeActive) return true;
            if ((int)t.Argument2.Token.Value != 0) return true;
            if (t.Argument0.Token.Value.ToString() != t.Argument1.Token.Value.ToString()) return true;
            if (!this.IsKeyValid()) return true;

            request.Size = 0;

            return true;
        }

        private bool IsKeyValid()
        {
            if (this.Key == "add") return true;
            if (this.Key == "sub") return true;

            return false;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}