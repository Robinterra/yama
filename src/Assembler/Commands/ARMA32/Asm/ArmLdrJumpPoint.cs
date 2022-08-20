using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Runtime
{
    public class ArmLdrJumpPoint : ICommand
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

        private IFormat bformat;

        public uint CommandId
        {
            get;
            set;
        }

        private uint bId;

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

        public ArmLdrJumpPoint(string key, IFormat ldrFormat, uint ldrId, IFormat bFormat, uint bId, int size)
        {
            this.Node = new ParserError();
            this.Key = key;
            this.Format = ldrFormat;
            this.bformat = bFormat;
            this.CommandId = ldrId;
            this.bId = bId;
            this.Size = size;
        }

        public ArmLdrJumpPoint(ArmLdrJumpPoint t, IParseTreeNode node, List<byte> bytes)
        {
            this.Key = t.Key;
            this.Format = t.Format;
            this.CommandId = t.CommandId;
            this.Size = t.Size;
            this.Node = node;
            this.Data = bytes.ToArray();
            this.bformat = t.bformat;
            this.bId = t.bId;
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
            assembleFormat.RegisterDestionation = request.GetRegister(t.Argument0.Token.Text);
            assembleFormat.RegisterInputLeft = request.GetRegister("pc");
            if (!Format.Assemble(assembleFormat)) return false;

            assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.bId;
            assembleFormat.Immediate = 1;
            if (!bformat.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new ArmLdrJumpPoint(this, request.Node, assembleFormat.Result));

            JumpPointMapper? map = request.Assembler.GetJumpPoint(t.Argument1.Token.Text);
            if (map == null) return false;

            byte[] tmp = BitConverter.GetBytes ( map.Adresse - 4 );
            assembleFormat.Result.Add ( tmp[0] );
            assembleFormat.Result.Add ( tmp[1] );
            assembleFormat.Result.Add ( tmp[2] );
            assembleFormat.Result.Add ( tmp[3] );

            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWith2ArgsNode t)) return false;
            if (t.Argument0.Token.Kind != Lexer.IdentifierKind.Word) return false;
            if (t.Argument1 is SquareArgumentNode s) return false;
            if (t.Argument1.Token.Kind != Lexer.IdentifierKind.Word) return false;

            return true;
        }

        public bool DisAssemble(RequestDisAssembleCommand request)
        {
            throw new NotImplementedException();
        }
    }
}