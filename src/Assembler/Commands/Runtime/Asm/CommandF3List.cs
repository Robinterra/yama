using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class CommandF3List : ICommand
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
        public int Max { get; }
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CommandF3List(string key, IFormat format, uint id, int size, int max)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Max = max;
        }

        public CommandF3List(CommandF3List t, IParseTreeNode node, List<byte> bytes)
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
            if (!(request.Node is CommandWithList t)) return false;

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;

            uint registerlist = 0;
            foreach (Lexer.IdentifierToken token in t.Arguments)
            {
                if (token.Kind != Lexer.IdentifierKind.Word) return false;

                registerlist |= (uint) 1 << (int)request.Assembler.GetRegister(token.Text);
            }

            assembleFormat.RegisterDestionation = request.GetRegister("sp");
            assembleFormat.Immediate = registerlist;

            if (!Format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new CommandF3List(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWithList t)) return false;
            if (t.Arguments.Count == 0) return false;

            foreach (Lexer.IdentifierToken token in t.Arguments)
            {
                if (token.Kind != Lexer.IdentifierKind.Word) return false;

                if (request.Assembler.GetRegister(token.Text) > this.Max) return false;
            }

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}