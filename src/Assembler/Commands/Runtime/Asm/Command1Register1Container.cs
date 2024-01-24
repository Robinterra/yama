using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class Command1Register1Container : ICommand
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
        public int MaxRegister { get; }
        public uint Teiler { get; }
        public bool IsSpTwo { get; }
        public IParseTreeNode Node
        {
            get;
            set;
        }

        public int? ImmediateOverride
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Command1Register1Container(string key, IFormat format, uint id, int size, int maxregister, uint teiler, bool issptwo = false, int? immediateOverride = null)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.MaxRegister = maxregister;
            this.Teiler = teiler;
            this.IsSpTwo = issptwo;
            this.ImmediateOverride = immediateOverride;
        }

        public Command1Register1Container(Command1Register1Container t, IParseTreeNode node, List<byte> bytes)
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
            if (!(t.Argument1 is SquareArgumentNode s)) return false;
            if (request.Assembler.GetRegister(t.Argument0.Token.Text) > this.MaxRegister) return false;
            if (s.Number is null) return false;
            if (Convert.ToUInt32(s.Number.Value) % this.Teiler != 0) return false;

            uint immediate = Convert.ToUInt32(s.Number.Value) / this.Teiler;
            if (this.ImmediateOverride.HasValue) immediate = (uint)this.ImmediateOverride.Value;

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.RegisterDestionation = request.GetRegister(t.Argument0.Token.Text);
            assembleFormat.RegisterInputLeft = request.GetRegister(s.Token.Text);
            assembleFormat.Immediate = immediate;

            if (!Format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new Command1Register1Container(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith2ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (!(t.Argument1 is SquareArgumentNode s)) return false;
            if (request.Assembler.GetRegister(t.Argument0.Token.Text) > this.MaxRegister) return false;
            if (s.Number == null)
            {
                s.Number = new Lexer.IdentifierToken();
                s.Number.Kind = Lexer.IdentifierKind.NumberToken;
                s.Number.Value = (uint)0;
            }
            if (Convert.ToUInt32(s.Number.Value) % this.Teiler != 0) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}