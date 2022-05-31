namespace Yama.Debug
{

    public class StatusRegister
    {

        // -----------------------------------------------

        public bool Negative
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool Carry
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool Zero
        {
            get;
            set;
        }

        // -----------------------------------------------

        public uint CurrentState
        {
            get
            {
                uint result = 0;

                if (this.Negative) result |= 0x8;
                if (this.Zero) result |= 0x4;
                if (this.Carry) result |= 0x2;

                return result;
            }
        }

        // -----------------------------------------------

    }

}