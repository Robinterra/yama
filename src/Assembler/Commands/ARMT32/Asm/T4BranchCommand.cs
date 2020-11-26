using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class T4BranchCommand : ICommand
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
        public uint Maximum { get; }
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public T4BranchCommand(string key, string format, uint id, int size, uint max)
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Maximum = max;
        }

        public T4BranchCommand(T4BranchCommand t, IParseTreeNode node, List<byte> bytes)
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
            if (!(request.Node is CommandWith1ArgNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;

            JumpPointMapper map = request.Assembler.GetJumpPoint(t.Argument0.Token.Value.ToString());
            if (map == null) return false;

            uint target = request.Assembler.BuildJumpSkipper(request.Position, map.Adresse, (uint)this.Size);

            if ((target & 0x80000000) != 0x80000000)
            {
                if (target > this.Maximum) return false;
            }
            else
            {
                uint smaleentity = (~this.Maximum) + 1;
                if (target < smaleentity) return false;
            }

            //target = target >> this.BitSkipper;

            IFormat format = request.Assembler.GetFormat(this.Format);

            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Arguments.Add(target);

            if (!format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new T4BranchCommand(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith1ArgNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}