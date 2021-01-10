using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class T2LdrJumpCommand : ICommand
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
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public T2LdrJumpCommand(string key, string format, uint id, int size)
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
        }

        public T2LdrJumpCommand(T2LdrJumpCommand t, IParseTreeNode node, List<byte> bytes)
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

            JumpPointMapper map = request.Assembler.GetJumpPoint(t.Argument1.Token.Text);

            if (map == null)
                return false;

            IFormat format = request.Assembler.GetFormat(this.Format);
            IFormat branch = request.Assembler.GetFormat("T2BranchImmediate");
            IFormat constFormat = request.Assembler.GetFormat("Const");

            uint valuePosition = (request.Position & 0x3) == 0 ? (uint)2 : 4;
            uint valueSkip = request.Assembler.BuildJumpSkipper(request.Position + 4, request.Position + 4 + 2 + 4, 2);

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Arguments.Add(request.Assembler.GetRegister(t.Argument0.Token.Text));
            assembleFormat.Arguments.Add(0xF);
            assembleFormat.Arguments.Add(valuePosition);

            RequestAssembleFormat assembleFormatBranch = new RequestAssembleFormat();
            assembleFormatBranch.Command = 0x1C;
            assembleFormatBranch.Result = assembleFormat.Result;
            assembleFormatBranch.Arguments.Add(valueSkip);

            RequestAssembleFormat assembleFormatConst = new RequestAssembleFormat();
            assembleFormatConst.Command = map.Adresse;
            assembleFormatConst.Result = assembleFormat.Result;

            if (!format.Assemble(assembleFormat)) return false;

            if (!branch.Assemble(assembleFormatBranch)) return false;

            if (!constFormat.Assemble(assembleFormatConst)) return false;

            if (request.WithMapper) request.Result.Add(new T2LdrJumpCommand(this, request.Node, assembleFormat.Result));
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
    }
}