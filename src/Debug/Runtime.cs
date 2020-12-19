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

        public uint StepCounter
        {
            get;
            set;
        }

        // -----------------------------------------------
        public List<Assembler.ICommand> Sequence
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<uint> BreakAdresses
        {
            get;
            set;
        } = new List<uint>();

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

            this.DebugReader();

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
            if (this.IsStepMode) return this.DebugReader();

            if (this.BreakAdresses.Contains(this.Register[15]))
            {
                Console.WriteLine("Breakpoint at {0:x} reached", this.Register[15]);
                return this.DebugReader();
            }

            return true;
        }

        private void StepPrint()
        {
            this.IsStepMode = false;
            Console.WriteLine("--------------");

            this.PrintCmd();

            Console.WriteLine("A: {0:x}, B: {1:x}, C: {2:x}", this.A, this.B, this.C);

            for (int registers = 0; registers < 13; registers++)
            {
                Console.Write("r{0:x}: {1:x}, ", registers, this.Register[registers]);
            }
            Console.WriteLine();

            Console.WriteLine("SP: {2:x}, LR: {1:x}, PC: {0:x}", this.Register[15], this.Register[14], this.Register[13]);
        }

        private bool PrintCmd()
        {
            if (this.Sequence != null) return this.PrintCmdFromSequence(this.Register[15] >> 2);

            uint cmd = this.Command;

            foreach (ICommand command in this.Commands)
            {
                if (command.CommandId != cmd) continue;

                Console.Write("CMD: {0}, ", command.GetType().Name);
                break;
            }

            return true;
        }

        private bool PrintCmdFromSequence(uint index)
        {
            if (this.Sequence.Count <= index) return this.WriteError("Out of bounds");

            IParseTreeNode node = this.FindSequence((int)index);
            if (node == null) return true;

            Console.Write("{0}: {1} ", node.Token.Line, node.Token.Text);

            foreach (IParseTreeNode child in node.GetAllChilds)
            {
                Console.Write("{0}, ", child.Token.Text);
            }

            Console.WriteLine();

            return true;
        }

        private IParseTreeNode FindSequence(int index)
        {
            int counter = 0;
            foreach (Assembler.ICommand command in this.Sequence)
            {
                if (counter == index) return command.Node;

                counter += command.Size >> 2;
            }

            return null;
        }

        private bool DebugReader()
        {
            if (this.StepCounter > 0)
            {
                this.StepCounter--;
                return true;
            }

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
                if (cmd.Contains("step "))
                {
                    notContinue = false;
                    this.IsStepMode = true;

                    string[] spliter = cmd.Split(' ');
                    uint.TryParse(spliter[1], out uint result);

                    this.StepCounter = result;

                    continue;
                }
                if (cmd.Contains("print")) this.PrintCmds(cmd);
                if (cmd.Contains("break")) this.CreateBreaks(cmd);
                //if (cmd == "inspect") this.Inspect();

                if (cmd == "stat") this.StepPrint();
                if (cmd.Contains("mem")) this.PrintMem(cmd);
                if (cmd.Contains("edit")) this.EditMem(cmd);
            }

            return true;
        }

        private bool PrintCmds(string cmd)
        {
            string[] daten = cmd.Split(' ');
            if (daten.Length != 2) return false;
            if (!uint.TryParse(daten[1], out uint count)) return this.WriteError("Argument Number can not be parse");
            if (this.Sequence == null) return this.WriteError("only sequence mode supprt");

            uint start = this.Register[15] >> 2;
            for (uint i = 0; i < count; i++)
            {
                this.PrintCmdFromSequence(start + i);
            }

            return true;
        }

        private bool EditMem(string cmd)
        {
            string[] daten = cmd.Split(' ');
            if (daten.Length != 3) return false;
            if (!uint.TryParse(daten[1], System.Globalization.NumberStyles.HexNumber, null, out uint adresse)) return this.WriteError("Argument Number can not be parse");
            if (!uint.TryParse(daten[2], System.Globalization.NumberStyles.HexNumber, null, out uint value)) return this.WriteError("Argument Number can not be parse");
            if (value > 0xff) return this.WriteError("The Value is to big");

            this.Memory[adresse] = (byte)value;

            return true;
        }

        private bool Inspect()
        {
            Console.Write("inspect> ");
            string cmd = Console.ReadLine();

            return true;
        }

        private bool PrintMem(string cmd)
        {
            string[] daten = cmd.Split(' ');
            if (daten.Length != 3) return this.WriteError("Missing Parameters");
            if (!uint.TryParse(daten[1], System.Globalization.NumberStyles.HexNumber, null, out uint adresse)) return this.WriteError("Argument Number can not be parse");
            if (!uint.TryParse(daten[2], System.Globalization.NumberStyles.HexNumber, null, out uint length)) return this.WriteError("Argument Number can not be parse");
            length = length << 4;

            Console.WriteLine("0x{0:x8}  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F", adresse);
            for (uint lineCounter = 0; lineCounter < length >> 4; lineCounter++)
            {
                this.PrintLine(adresse + (lineCounter << 4));
            }

            return true;
        }

        private bool WriteError(string v)
        {
            Console.Error.WriteLine(v);

            return false;
        }

        private bool PrintLine(uint v)
        {
            Console.Write("0x{0:x8} ", v);

            for (uint i = 0; i < 0x10; i++)
            {
                if (v + i >= this.Memory.Length) continue;

                Console.Write("{0:x2} ", this.Memory[v + i]);
            }

            Console.WriteLine();

            return true;
        }

        private bool CreateBreaks(string cmd)
        {
            string[] daten = cmd.Split(' ');
            if (daten.Length != 2) return this.WriteError("Missing Parameters");

            if (this.Sequence == null)
            {
                if (!uint.TryParse(daten[1], System.Globalization.NumberStyles.HexNumber, null, out uint pointadres)) return this.WriteError("Argument Number can not be parse");

                this.BreakAdresses.Add(pointadres);

                return true;
            }
            if (!uint.TryParse(daten[1], out uint line)) return this.WriteError("Argument Number can not be parse");

            uint counter = 0;
            foreach (Assembler.ICommand command in this.Sequence)
            {
                counter +=(uint) command.Size >> 2;
                if (command.Node.Token.Line != line) continue;

                counter -= (uint) command.Size >> 2;
                Console.WriteLine("Breakpoint at 0x{0:x} created", counter << 2);
                this.BreakAdresses.Add(counter << 2);

                return true;
            }

            return this.WriteError("breakpoint can not be created");
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