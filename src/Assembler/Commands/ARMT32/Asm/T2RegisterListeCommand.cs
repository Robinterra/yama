using System;
using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class T1RegisterListeCommand : ICommand
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
        public int Max { get; }
        public IParseTreeNode Node
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public T1RegisterListeCommand(string key, string format, uint id, int size, int max)
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Max = max;
        }

        public T1RegisterListeCommand(T1RegisterListeCommand t, IParseTreeNode node, List<byte> bytes)
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
            if (t.Arguments.Count == 0) return request.Assembler.IsOptimizeActive;

            IFormat format = request.Assembler.GetFormat(this.Format);
            RequestAssembleFormat assembleFormat = new RequestAssembleFormat();
            assembleFormat.Command = this.CommandId;

            uint registerlist = 0;
            foreach (Lexer.IdentifierToken token in t.Arguments)
            {
                if (token.Kind != Lexer.IdentifierKind.Word) return false;

                registerlist |= (uint) 1 << (int)request.Assembler.GetRegister(token.Text);
            }

            assembleFormat.Arguments.Add(request.Assembler.GetRegister("r13"));
            assembleFormat.Arguments.Add(registerlist);

            if (!format.Assemble(assembleFormat)) return false;

            if (request.WithMapper) request.Result.Add(new T1RegisterListeCommand(this, request.Node, assembleFormat.Result));
            request.Stream.Write(assembleFormat.Result.ToArray());

            return true;
        }

        public bool Identify(RequestIdentify request)
        {
            if (request.Node.Token.Text.ToLower() != this.Key.ToLower()) return false;
            if (!(request.Node is CommandWithList t)) return false;
            if (t.Arguments.Count == 0)
            {
                request.Size = 0;
                if (!request.Assembler.IsOptimizeActive) return false;
            }

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