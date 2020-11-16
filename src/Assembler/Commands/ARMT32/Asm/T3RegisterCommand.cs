using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class T3RegisterCommand : ICommand
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

        #endregion get/set

        #region ctor

        public T3RegisterCommand(string key, string format, uint id, int size)
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
        }

        public T3RegisterCommand(T3RegisterCommand t, IParseTreeNode node, List<byte> bytes)
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
            if (t.Argument2.Token.Kind != Lexer.IdentifierKind.Word) return false;

            IFormat format = request.Assembler.GetFormat(this.Format);
            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument0.Token.Text));
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument1.Token.Text));
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument2.Token.Text));

            if (!format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new T3RegisterCommand(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith3ArgsNode t)) return false;
            if (t.Argument2.Token.Kind != Lexer.IdentifierKind.Word) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}