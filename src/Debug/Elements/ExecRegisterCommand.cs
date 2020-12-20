using System;
using System.Collections.Generic;
using System.Text;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class ExecRegisterCommand : ICommand
    {

        public uint CommandId
        {
            get;
            set;
        } = 0xFF;

        public bool Execute(Runtime runtime)
        {

            if (runtime.Register[runtime.A] == 1)
            {
                System.Console.Write("{0}", runtime.Register[1]);

                return true;
            }

            if (runtime.Register[runtime.A] == 2)
            {
                string input = System.Console.ReadLine();
                if (!int.TryParse(input, out int number))
                {
                    System.Console.WriteLine("Failed to Parse int, default is 0");
                    number = 0;
                }

                runtime.Register[12] = (uint)number;

                return true;
            }

            if (runtime.Register[runtime.A] == 3)
            {
                runtime.IsRun = false;
                runtime.Register[12] = runtime.Register[1];

                return true;
            }

            if (runtime.Register[runtime.A] == 4)
            {
                uint adresse = runtime.Register[1];

                uint length = BitConverter.ToUInt32(runtime.Memory, (int)adresse);

                string printtext = Encoding.UTF8.GetString(runtime.Memory, (int)adresse + 4, (int)length);

                Console.Write(printtext);

                return true;
            }
            if (runtime.Register[runtime.A] == 5)
            {
                string text = Console.ReadLine();

                byte[] data = Encoding.UTF8.GetBytes(text);

                byte[] result = new byte[data.Length + 4];

                Array.Copy(data, 0, result, 4, data.Length);

                byte[] length = BitConverter.GetBytes(data.Length);

                Array.Copy(length, 0, result, 0, 4);

                runtime.AddObjectToMemory(result, out uint adresse);

                runtime.Register[12] = adresse;

                return true;
            }

            if (runtime.Register[runtime.A] == 6) return WindowsFormsMapper.Instance.Execute(runtime);

            return true;
        }

    }
}