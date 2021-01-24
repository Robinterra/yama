using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class T2LdrPointerCommand : ICommand
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
        public int MaxRegister { get; }
        public uint Teiler { get; }
        public bool IsSpTwo { get; }
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public T2LdrPointerCommand(string key, string format, uint id, int size, int maxregister, uint teiler, bool issptwo = false)
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.MaxRegister = maxregister;
            this.Teiler = teiler;
            this.IsSpTwo = issptwo;
        }

        public T2LdrPointerCommand(T2LdrPointerCommand t, IParseTreeNode node, List<byte> bytes)
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
            if (!(t.Argument1 is PointerNode s)) return false;
            if (request.Assembler.GetRegister(t.Argument0.Token.Text) > this.MaxRegister) return false;
            JumpPointMapper map = request.Assembler.GetJumpPoint(s.Token.Value.ToString());

            if (map == null) return false;
            uint target = request.Assembler.BuildJumpSkipper(request.Position, map.Adresse, (uint)this.Size, true);
            if ((request.Position & 0x3) != 0 && target == 2) target = 4;

            IFormat format = request.Assembler.GetFormat(this.Format);
            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument0.Token.Text));
            assembleFormat.Arguments.Add(request.Assembler.GetRegister("pc"));
            assembleFormat.Arguments.Add(target);

            if (!format.Assemble(assembleFormat))
                return false;

            if (request.WithMapper) request.Result.Add(new T2LdrPointerCommand(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith2ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (!(t.Argument1 is PointerNode s)) return false;
            if (request.Assembler.GetRegister(t.Argument0.Token.Text) > this.MaxRegister) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}