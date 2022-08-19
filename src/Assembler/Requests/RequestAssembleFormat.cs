using System.Collections.Generic;

namespace Yama.Assembler
{
    public class RequestAssembleFormat
    {
        public uint Command
        {
            get;
            set;
        }

        public List<uint> Arguments
        {
            get;
            set;
        } = new List<uint>();

        public uint RegisterDestionation
        {
            get;
            set;
        }

        public uint Condition
        {
            get;
            set;
        }

        public uint RegisterInputLeft
        {
            get;
            set;
        }

        public uint RegisterInputRight
        {
            get;
            set;
        }

        public uint Immediate
        {
            get;
            set;
        }

        public List<byte> Result
        {
            get;
            set;
        } = new List<byte>();
        public bool Sonder { get; set; }
        public bool Sonder2 { get; set; }
    }

    public enum ArgumentMode
    {
        None,
        RegisterDestionation,
        RegisterInputLeft,
        RegisterInputRight,
        Condition
    }
}