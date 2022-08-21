using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class Command3Register : ICommand
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

        private uint stype;

        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Command3Register(string key, IFormat format, uint id, int size, uint stype = 0)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.stype = stype;
        }

        public Command3Register(Command3Register t, IParseTreeNode node, List<byte> bytes)
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
            if (t.Argument2.Token.Kind != Lexer.IdentifierKind.Word) return false;

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.RegisterDestionation = request.GetRegister(t.Argument0.Token.Text);
            assembleFormat.RegisterInputLeft = request.GetRegister(t.Argument1.Token.Text);
            assembleFormat.RegisterInputRight = request.GetRegister(t.Argument2.Token.Text);
            assembleFormat.Stype = stype;

            if (!Format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new Command3Register(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith3ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument2.Token.Kind != Lexer.IdentifierKind.Word) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}