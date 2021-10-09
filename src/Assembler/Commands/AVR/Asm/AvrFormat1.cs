﻿using System;

namespace Yama.Assembler.Commands.AVR.Asm
{

    public class AvrFormat1 : IFormat
    {
        public string Name
        {
            get
            {
                return "Format1";
            }
        }

        public bool Assemble(RequestAssembleFormat request)
        {
            if (request.Arguments.Count != 2) return false;

            uint firstFragment = (request.Command << 10) & 0xfC00;
            firstFragment |= ( request.Arguments[0] << 4 ) & 0x01F0;
            firstFragment |= ( request.Arguments[1] ) & 0x000F;
            firstFragment |= ( request.Arguments[1] << 9 ) & 0x0200;

            byte[] tmp = BitConverter.GetBytes ( firstFragment );
            request.Result.Add ( tmp[0] );
            request.Result.Add ( tmp[1] );

            return true;
        }

        public bool DisAssemble(RequestDisAssembleFormat request)
        {
            return true;
        }
    }

}