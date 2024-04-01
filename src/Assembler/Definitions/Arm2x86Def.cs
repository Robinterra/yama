using Yama.Assembler.ARMA32;
using Yama.Assembler.ARMT32;
using Yama.Assembler.Runtime;
using Yama.Parser;

namespace Yama.Assembler.Definitions
{
    public class Arm2x86Def : IAssemblerDefinition
    {
        public string Name => "arm2x86";

        public uint GetCondition(ConditionMode condition)
        {
            throw new NotImplementedException();
        }

        public Assembler SetupDefinition(Assembler assembler)
        {
            assembler.Definition = new AssemblerDefinition();
            assembler.Definition.Arm2x86 = this;
            return assembler;
        }

        public string Translate(string register)
        {
            switch (register.ToLower())
            {
                case "r12": return "eax";
                case "r0": return "ebx";
                case "r1": return "ecx";
                case "r2": return "edx";
                case "r3": return "esi";
                case "r4": return "edi";
                case "fp": return "ebp";
                case "sp": return "esp";
                default: throw new NotSupportedException($"Register '{register}' not supported");
            }
        }
    }

    public class BranchesArm2x86
    {
        public Arm2x86Def Def { get; }

        public BranchesArm2x86(Arm2x86Def def)
        {
            this.Def = def;
        }

        public bool Translate(TextWriter writer, IParseTreeNode node)
        {
            if (node is CommandWith2ArgsNode c2r) return this.Cmp(writer, c2r);
            if (node is not CommandWith1ArgNode cw) return false;
            switch (cw.Token.Text.ToLower())
            {
                case "b": return this.B(writer, cw);
                case "bx": return this.Bx(writer, cw);
                case "blt": return this.Blt(writer, cw);
                case "bgt": return this.Bgt(writer, cw);
                case "ble": return this.Ble(writer, cw);
                case "bge": return this.Bge(writer, cw);
                case "beq": return this.Beq(writer, cw);
                case "svc": return this.Svc(writer, cw);
                default: return false;
            }
        }

        private bool Svc(TextWriter writer, CommandWith1ArgNode cw)
        {
            writer.WriteLine($"int 0x80");
            return true;
        }

        private bool Cmp(TextWriter writer, CommandWith2ArgsNode c2r)
        {
            if (c2r.Token.Text.ToLower() != "cmp") return false;

            string reg1 = this.Def.Translate(c2r.Argument0.Token.Text);
            string reg2 = this.Def.Translate(c2r.Argument1.Token.Text);
            writer.WriteLine($"cmp {reg1}, {reg2}");

            return true;
        }

        private bool Beq(TextWriter writer, CommandWith1ArgNode cw)
        {
            string label = cw.Argument0.Token.Text;
            writer.WriteLine($"je {label}");
            return true;
        }

        private bool Bge(TextWriter writer, CommandWith1ArgNode cw)
        {
            string label = cw.Argument0.Token.Text;
            writer.WriteLine($"jge {label}");
            return true;
        }

        private bool Ble(TextWriter writer, CommandWith1ArgNode cw)
        {
            string label = cw.Argument0.Token.Text;
            writer.WriteLine($"jle {label}");
            return true;
        }

        private bool Bgt(TextWriter writer, CommandWith1ArgNode cw)
        {
            string label = cw.Argument0.Token.Text;
            writer.WriteLine($"jg {label}");
            return true;
        }

        private bool Blt(TextWriter writer, CommandWith1ArgNode cw)
        {
            string label = cw.Argument0.Token.Text;
            writer.WriteLine($"jl {label}");
            return true;
        }

        private bool Bx(TextWriter writer, CommandWith1ArgNode cw)
        {
            string reg = this.Def.Translate(cw.Argument0.Token.Text);
            writer.WriteLine($"jmp {reg}");
            return true;
        }

        private bool B(TextWriter writer, CommandWith1ArgNode cw)
        {
            string label = cw.Argument0.Token.Text;
            writer.WriteLine($"jmp {label}");
            return true;
        }
    }

    public class PushPopArm2x86
    {
        public Arm2x86Def Def { get; }

        public PushPopArm2x86(Arm2x86Def def)
        {
            this.Def = def;
        }

        public bool Translate(TextWriter writer, IParseTreeNode node)
        {
            if (node is not CommandWithList cw) return false;
            switch (cw.Token.Text.ToLower())
            {
                case "push": return this.Push(writer, cw);
                case "pop": return this.Pop(writer, cw);
                default: return false;
            }
        }

        private bool Pop(TextWriter writer, CommandWithList cw)
        {
            foreach (var arg in cw.Arguments)
            {
                string reg = this.Def.Translate(arg.Text);
                writer.WriteLine($"pop {reg}");
            }
            return true;
        }

        private bool Push(TextWriter writer, CommandWithList cw)
        {
            foreach (var arg in cw.Arguments)
            {
                string reg = this.Def.Translate(arg.Text);
                writer.WriteLine($"push {reg}");
            }
            return true;
        }
    }

    public class MemoryAccessArm2x86
    {
        public Arm2x86Def Def { get; }

        public MemoryAccessArm2x86(Arm2x86Def def)
        {
            this.Def = def;
        }

        public bool Translate(TextWriter writer, IParseTreeNode node)
        {
            if (node is not CommandWith2ArgsNode cw) return false;
            switch (cw.Token.Text.ToLower())
            {
                case "ldr": return this.Ldr(writer, cw);
                case "str": return this.Str(writer, cw);
                case "mov": return this.Mov(writer, cw);
                default: return false;
            }
        }

        private bool Mov(TextWriter writer, CommandWith2ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            if (cw.Argument1.Token.Kind == Lexer.IdentifierKind.NumberToken)
            {
                writer.WriteLine($"mov {dest}, {cw.Argument1.Token.Text}");
                return true;
            }
            string src = this.Def.Translate(cw.Argument1.Token.Text);
            writer.WriteLine($"mov {dest}, {src}");
            return true;
        }

        private bool Str(TextWriter writer, CommandWith2ArgsNode cw)
        {
            if (cw.Argument1 is not SquareArgumentNode s) return false;

            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string srcReg = this.Def.Translate(s.Token.Text);
            if (s.Number == null)
            {
                s.Number = new Lexer.IdentifierToken();
                s.Number.Kind = Lexer.IdentifierKind.NumberToken;
                s.Number.Value = (uint)0;
            }
            uint number = Convert.ToUInt32(s.Number.Value);
            writer.WriteLine($"mov [{srcReg}+{number}], dword {dest}");
            return true;
        }

        private bool Ldr(TextWriter writer, CommandWith2ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);

            if (cw.Argument1 is not SquareArgumentNode s)
            {
                if (cw.Argument1.Token.Kind != Lexer.IdentifierKind.Word) return false;

                writer.WriteLine($"lea {dest}, [{cw.Argument1.Token.Text}]");

                return true;
            }

            string srcReg = this.Def.Translate(s.Token.Text);
            if (s.Number == null)
            {
                s.Number = new Lexer.IdentifierToken();
                s.Number.Kind = Lexer.IdentifierKind.NumberToken;
                s.Number.Value = (uint)0;
            }
            uint number = Convert.ToUInt32(s.Number.Value);
            writer.WriteLine($"mov {dest}, dword [{srcReg}+{number}]");
            return true;
        }
    }

    public class ArimeticsArm2x86
    {
        public Arm2x86Def Def { get; }

        public ArimeticsArm2x86(Arm2x86Def def)
        {
            this.Def = def;
        }

        public bool Translate(TextWriter writer, IParseTreeNode node)
        {
            if (node is not CommandWith3ArgsNode cw) return false;
            switch (cw.Token.Text.ToLower())
            {
                case "add": return this.Add(writer, cw);
                case "sub": return this.Sub(writer, cw);
                case "mul": return this.Mul(writer, cw);
                case "div": return this.Div(writer, cw);
                case "lsl": return this.Lsl(writer, cw);
                case "lsr": return this.Lsr(writer, cw);
                case "and": return this.And(writer, cw);
                case "eor": return this.Eor(writer, cw);
                case "orr": return this.Orr(writer, cw);
                default: return false;
            }
        }

        private bool Orr(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != dest) writer.WriteLine($"mov {dest}, {src1}");
            writer.WriteLine($"or {dest}, {src2}");

            return true;
        }

        private bool Eor(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != dest) writer.WriteLine($"mov {dest}, {src1}");
            writer.WriteLine($"xor {dest}, {src2}");

            return true;
        }

        private bool And(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != dest) writer.WriteLine($"mov {dest}, {src1}");
            writer.WriteLine($"and {dest}, {src2}");

            return true;
        }

        private bool Lsr(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = cw.Argument2.Token.Kind == Lexer.IdentifierKind.NumberToken ? cw.Argument2.Token.Text.ToString() : this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != dest) writer.WriteLine($"mov {dest}, {src1}");
            writer.WriteLine($"shr {dest}, {src2}");

            return true;
        }

        private bool Lsl(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = cw.Argument2.Token.Kind == Lexer.IdentifierKind.NumberToken ? cw.Argument2.Token.Text.ToString() : this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != dest) writer.WriteLine($"mov {dest}, {src1}");
            writer.WriteLine($"shl {dest}, {src2}");

            return true;
        }

        private bool Div(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != "eax") writer.WriteLine($"mov eax, {src1}");
            //writer.WriteLine($"cdq");
            writer.WriteLine($"idiv {src2}");
            if (dest != "eax") writer.WriteLine($"mov {dest}, eax");

            return true;
        }

        private bool Mul(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != "eax") writer.WriteLine($"mov eax, {src1}");
            writer.WriteLine($"imul {src2}");
            if (dest != "eax") writer.WriteLine($"mov {dest}, eax");

            return true;
        }

        private bool Sub(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != dest) writer.WriteLine($"mov {dest}, {src1}");
            writer.WriteLine($"sub {dest}, {src2}");

            return true;
        }

        private bool Add(TextWriter writer, CommandWith3ArgsNode cw)
        {
            string dest = this.Def.Translate(cw.Argument0.Token.Text);
            string src1 = this.Def.Translate(cw.Argument1.Token.Text);
            string src2 = this.Def.Translate(cw.Argument2.Token.Text);

            if (src1 != dest) writer.WriteLine($"mov {dest}, {src1}");
            writer.WriteLine($"add {dest}, {src2}");

            return true;
        }
    }
}