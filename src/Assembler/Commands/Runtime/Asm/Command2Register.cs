using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class Command2Register : ICommand
    {
        private RegisterMode Rm;

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
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Command2Register(string key, IFormat format, uint id, int size, RegisterMode rm = RegisterMode.Destitination_Left)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Rm = rm;
        }

        public Command2Register(Command2Register t, IParseTreeNode node, List<byte> bytes)
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
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.Word) return false;

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            if (this.Rm == RegisterMode.Destitination_Left)
            {
                assembleFormat.RegisterInputLeft = request.GetRegister(t.Argument1.Token.Text);
                assembleFormat.RegisterDestionation = request.GetRegister(t.Argument0.Token.Text);
            }
            if (this.Rm == RegisterMode.Destitination_Right)
            {
                assembleFormat.RegisterInputRight = request.GetRegister(t.Argument1.Token.Text);
                assembleFormat.RegisterDestionation = request.GetRegister(t.Argument0.Token.Text);
            }
            if (this.Rm == RegisterMode.Left_Right)
            {
                assembleFormat.RegisterInputRight = request.GetRegister(t.Argument0.Token.Text);
                assembleFormat.RegisterInputLeft = request.GetRegister(t.Argument1.Token.Text);
            }

            if (!Format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new Command2Register(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith2ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.Word) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }

        public enum RegisterMode
        {
            Destitination_Right,
            Destitination_Left,
            Left_Right
        }

    }
}