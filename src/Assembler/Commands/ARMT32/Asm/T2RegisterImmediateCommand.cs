using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class T2RegisterImmediateCommand : ICommand
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

        public int Max
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public T2RegisterImmediateCommand(string key, string format, uint id, int size, int max)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Max = max;
        }

        public T2RegisterImmediateCommand(T2RegisterImmediateCommand t, IParseTreeNode node, List<byte> bytes)
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

            IFormat? format = request.Assembler.GetFormat(this.Format);
            if (format is null) return false;

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            if (this.Key == "cmp") assembleFormat.Arguments.Add(0xf);
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument0.Token.Text));
            if (this.Key != "cmp") assembleFormat.Arguments.Add(0xf);
            assembleFormat.Arguments.Add(Convert.ToUInt32(t.Argument1.Token.Value));

            if (!format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new T2RegisterImmediateCommand(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith2ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.NumberToken) return false;
            if (Convert.ToInt32(t.Argument1.Token.Value) > this.Max) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}