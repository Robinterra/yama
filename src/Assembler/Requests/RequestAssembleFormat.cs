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

        public ConditionMode Condition
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

        public uint Stype
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

        public RequestAssembleFormat()
        {
            this.Condition = ConditionMode.Always;
        }
    }

    public enum ConditionMode
    {
        None,
        Never,
        Equal,
        NotEqual,
        UnsignedLessThan,
        UnsignedLessThanOrEqual,
        UnsignedGreaterThanOrEqual,
        UnsignedGreaterThan,
        SignedGreaterThanOrEqual,
        SignedLessThan,
        SignedGreaterThan,
        SignedLessThanOrEqual,
        Always,
        SkipNext,
        FullBits
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