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

        public List<byte> Result
        {
            get;
            set;
        } = new List<byte>();
        public bool Sonder { get; set; }
        public bool Sonder2 { get; set; }
    }
}