namespace Yama.Debug
{
    public class AndImediateCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0x16;

        public bool Execute(Runtime runtime)
        {

            runtime.Register[runtime.A] = runtime.Register[runtime.B] & runtime.C;

            return true;
        }

    }
}