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

        public List<IMapper> Mappers
        {
            get;
            set;
        } = new List<IMapper>();

        public ExecRegisterCommand()
        {
            this.Mappers.Add(new InputOutputMapper());
            this.Mappers.Add(new GuiMapper());
        }

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
                string printtext = runtime.GetStringFromRegister(1);

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

            if (runtime.Register[runtime.A] == 7) return this.ExecuteConsoleStuff(runtime);

            uint id = runtime.Register[runtime.A];

            foreach (IMapper mapper in this.Mappers)
            {
                if (mapper.Id != id) continue;

                return mapper.Execute(runtime);
            }

            return true;
        }

        private bool ExecuteConsoleStuff(Runtime runtime)
        {
            if (runtime.Register[1] == 1)
            {
                Console.SetCursorPosition((int)runtime.Register[2], (int)runtime.Register[3]);

                return true;
            }

            if (runtime.Register[1] == 2)
            {
                Console.Write("\x1B[38;5;{0}m", runtime.Register[2]);

                return true;
            }

            if (runtime.Register[1] == 3)
            {
                Console.BackgroundColor = (ConsoleColor)runtime.Register[2];

                return true;
            }

            if (runtime.Register[1] == 4)
            {
                ConsoleKeyInfo key = Console.ReadKey();

                runtime.Register[12] = (uint)key.KeyChar;

                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(" ");
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

                return true;
            }

            return true;
        }
    }
}