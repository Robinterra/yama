using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class Runtime
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public uint MemorySize
        {
            get;
            set;
        } = 1000000;

        // -----------------------------------------------

        public uint[] Register
        {
            get;
            set;
        } = new uint[16];

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

        public uint Command
        {
            get;
            set;
        }

        // -----------------------------------------------

        public uint Condition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public uint A
        {
            get;
            set;
        }

        // -----------------------------------------------
        public uint B
        {
            get;
            set;
        }

        // -----------------------------------------------
        public uint C
        {
            get;
            set;
        }

        // -----------------------------------------------

        public FileInfo Input
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool IsStepMode
        {
            get;
            set;
        }

        // -----------------------------------------------

        public byte[] Memory
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<ICommand> Commands
        {
            get;
            set;
        } = new List<ICommand>();

        // -----------------------------------------------

        public bool IsRun
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Runtime()
        {
            this.Init();
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        private void Init()
        {
            this.Commands.Add(new AdcRegisterCommand());
            this.Commands.Add(new AddImediateCommand());
            this.Commands.Add(new AddRegisterCommand());
            this.Commands.Add(new AndRegisterCommand());
            this.Commands.Add(new AslImediateCommand());
            this.Commands.Add(new AslRegisterCommand());
            this.Commands.Add(new ASRImediateCommand());
            this.Commands.Add(new ASRRegisterCommand());
            this.Commands.Add(new BConditionCommand());
            this.Commands.Add(new BxRegisterCommand());
            this.Commands.Add(new BlxRegisterCommand());
            this.Commands.Add(new CmpImediateCommand());
            this.Commands.Add(new CmpRegisterCommand());
            this.Commands.Add(new DivRegisterCommand());
            this.Commands.Add(new EorRegisterCommand());
            this.Commands.Add(new ExecRegisterCommand());
            this.Commands.Add(new LdrCommand());
            this.Commands.Add(new MovImediateCommand());
            this.Commands.Add(new MovRegisterCommand());
            this.Commands.Add(new MulRegisterCommand());
            this.Commands.Add(new OrrRegisterCommand());
            this.Commands.Add(new PopCommand());
            this.Commands.Add(new PushCommand());
            this.Commands.Add(new StrCommand());
            this.Commands.Add(new SubImediateCommand());
            this.Commands.Add(new SubRegisterCommand());
        }

        // -----------------------------------------------

        public bool Execute ()
        {
            this.ReadFile();

            this.IsRun = true;

            while (this.IsRun)
            {
                this.MakeCommand();

                this.DebugEvent();

                this.RunCommand();

                this.EndPhase();
            }

            Console.WriteLine("Program Exit with {0} Returncode", this.Register[12]);
            Environment.Exit((int)this.Register[12]);

            return true;
        }

        private bool ReadFile()
        {
            this.Memory = new byte[this.MemorySize];

            Array.Copy(File.ReadAllBytes(this.Input.FullName), this.Memory, this.Input.Length);

            this.Register[15] = 0;
            this.Register[13] = this.MemorySize - 4;
            this.Register[12] = (uint)this.Input.Length;

            return true;
        }

        private bool DebugEvent()
        {
            if (this.IsStepMode) this.StepPrint();

            return true;
        }

        private void StepPrint()
        {
            this.IsStepMode = false;
            Console.WriteLine("--------------");

            uint cmd = this.Command;
            foreach (ICommand command in this.Commands)
            {
                if (command.CommandId != cmd) continue;

                Console.Write("CMD: {0}, ", command.GetType().Name);
                break;
            }
            Console.WriteLine("A: {0}, B: {1}, C: {2}", this.A, this.B, this.C);

            for (int registers = 0; registers < 13; registers++)
            {
                Console.Write("r{0}: {1}, ", registers, this.Register[registers]);
            }
            Console.WriteLine();

            Console.Write("SP: {2}, LR: {1}, PC: {0}", this.Register[15], this.Register[14], this.Register[13]);

            this.DebugReader();
        }

        private bool DebugReader()
        {
            bool notContinue = true;

            while (notContinue)
            {
                Console.Write("> ");
                string cmd = Console.ReadLine();

                if (cmd == "continue")
                {
                    notContinue = false;

                    continue;
                }
                if (cmd == "step")
                {
                    notContinue = false;
                    this.IsStepMode = true;

                    continue;
                }
            }

            return true;
        }

        private bool MakeCommand()
        {
            uint adresse = this.Register[15];
            byte three = this.Memory[adresse];
            byte two = this.Memory[adresse + 1];
            byte one = this.Memory[adresse + 2];
            byte command = this.Memory[adresse + 3];
            this.Command = command;

            return this.ChooseCorrectFormat(command, one, two, three);
        }

        private bool RunCommand()
        {
            if (!this.CheckCondition()) return true;

            uint cmd = this.Command;

            foreach (ICommand command in this.Commands)
            {
                if (command.CommandId != cmd) continue;

                return command.Execute(this);
            }

            Console.WriteLine("Warning Command {0:x} not found", cmd);

            return false;
        }

        private bool CheckCondition()
        {
            if (this.Condition == 0) return true;
            if (this.Condition == 1) return this.Zero;
            if (this.Condition == 2) return !this.Zero;
            if (this.Condition == 3) return (!this.Zero) && (!this.Carry);
            if (this.Condition == 4) return (this.Zero) || (!this.Carry);
            if (this.Condition == 5) return (!this.Zero) && (this.Carry);
            if (this.Condition == 6) return (this.Zero) || (this.Carry);
            if (this.Condition == 15) this.Register[15] += 4;

            return true;
        }

        private bool EndPhase()
        {
            this.Register[15] += 4;

            return true;
        }

        // -----------------------------------------------

        private bool ChooseCorrectFormat(byte command, byte one, byte two, byte three)
        {
            byte format = (byte)(command >> 4);
            if (format == 5) return this.Format1(one, two, three);
            if (format == 1) return this.Format2(one, two, three);
            if (format == 2) return this.Format3(one, two, three);
            if (format == 4) return this.Format3(one, two, three);
            if (format == 0xf) return this.Format1(one, two, three);
            if (command == 0x32) return this.Format3(one, two, three);
            if (command == 0x31) return this.Format1(one, two, three);
            if (command == 0x30) return this.Format1(one, two, three);

            return false;
        }

        private bool Format3(byte one, byte two, byte three)
        {
            this.Condition = (uint) one >> 4;
            this.A = (uint) one & 0x0f;
            this.B = 0;
            this.C = (uint) two << 8 | three;

            return true;
        }

        private bool Format2(byte one, byte two, byte three)
        {
            this.Condition = (uint) one >> 4;
            this.A = (uint) one & 0x0f;
            this.B = ((uint) two & 0xf0) >> 4;
            this.C = (uint) two & 0x0F << 8 | three;

            return true;
        }

        private bool Format1(byte one, byte two, byte three)
        {
            this.Condition = (uint) one >> 4;
            this.A = (uint) one & 0x0f;
            this.B = ((uint) two & 0xf0) >> 4;
            this.C = (uint) two & 0x0f;

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}