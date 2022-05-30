namespace Yama.Debug
{

    public class MrsCommand : ICommand
    {
        public uint CommandId
        {
            get
            {
                return 0x42;
            }
        }

        public bool Execute(Runtime runtime)
        {
            runtime.Register[runtime.A] = runtime.StatusRegister.CurrentState;

            return true;
        }
    }

}