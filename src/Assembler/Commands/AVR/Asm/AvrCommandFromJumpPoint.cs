﻿using System;
using System.Collections.Generic;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler.Commands.AVR.Asm
{

    public class AvrCommandFromJumpPoint : ICommand
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

        public uint Condition
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public AvrCommandFromJumpPoint ( string key, string format, uint id, int size, uint condition = 0 )
        {
            this.Key = key;
            this.Format = format;
            this.CommandId = id;
            this.Size = size;
            this.Condition = condition;
        }

        public AvrCommandFromJumpPoint ( AvrCommandFromJumpPoint t, IParseTreeNode node, List<byte> bytes )
        {
            this.Key = t.Key;
            this.Format = t.Format;
            this.CommandId = t.CommandId;
            this.Size = t.Size;
            this.Node = node;
            this.Data = bytes.ToArray ();
        }

        #endregion ctor

        public bool Assemble ( RequestAssembleCommand request )
        {
            if ( request.Node.Token.Text.ToLower () != this.Key.ToLower () ) return false;
            if ( !(request.Node is CommandWith1ArgNode t) ) return false;
            if ( t.Argument0.Token.Kind != Lexer.IdentifierKind.Word ) return false;

            JumpPointMapper map = request.Assembler.GetJumpPoint ( t.Argument0.Token.Value.ToString () );
            if ( map == null ) return false;

            IFormat format = request.Assembler.GetFormat ( this.Format );
            RequestAssembleFormat assembleFormat = new RequestAssembleFormat ();
            assembleFormat.Command = this.CommandId;
            assembleFormat.Arguments.Add ( map.Adresse );
            assembleFormat.Arguments.Add ( this.Condition );

            if ( !format.Assemble ( assembleFormat ) ) return false;

            if ( request.WithMapper ) request.Result.Add ( new AvrCommandFromJumpPoint ( this, request.Node, assembleFormat.Result ) );
            request.Stream.Write ( assembleFormat.Result.ToArray () );

            return true;
        }

        public bool Identify ( RequestIdentify request )
        {
            if ( request.Node.Token.Text.ToLower () != this.Key.ToLower () ) return false;
            if ( !(request.Node is CommandWith1ArgNode t) ) return false;
            if ( t.Argument0.Token.Kind != Lexer.IdentifierKind.Word ) return false;

            return true;
        }

        public bool DisAssemble ( RequestDisAssembleCommand request )
        {
            throw new NotImplementedException ();
        }

    }

}