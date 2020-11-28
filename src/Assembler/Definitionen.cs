using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Assembler.ARMT32;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler
{
    public class Definitionen
    {

        // -----------------------------------------------

        #region ParserDefinition

        // -----------------------------------------------

        private ParserLayer GetMainLayer(ParserLayer argumentLayer)
        {
            ParserLayer layer = new ParserLayer("main");

            //layer.ParserMembers.Add(new BedingtesAssemblen());
            layer.ParserMembers.Add(new JumpPointMarker());
            layer.ParserMembers.Add(new CommandWithList());
            layer.ParserMembers.Add(new CommandWith3ArgsNode(argumentLayer));
            layer.ParserMembers.Add(new CommandWith2ArgsNode(argumentLayer));
            layer.ParserMembers.Add(new CommandWith1ArgNode(argumentLayer));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer GetArgLayer()
        {
            ParserLayer layer = new ParserLayer("argumentLayer");

            //layer.ParserMembers.Add(new BedingtesAssemblen());
            layer.ParserMembers.Add(new ArgumentNode());
            layer.ParserMembers.Add(new SquareArgumentNode());

            return layer;
        }

        // -----------------------------------------------

        private List<ParserLayer> GetParserRules (  )
        {
            List<ParserLayer> parserRules = new List<ParserLayer>();

            ParserLayer arglayer = this.GetArgLayer();
            ParserLayer mainlayer = this.GetMainLayer(arglayer);
            parserRules.Add(arglayer);
            parserRules.Add(mainlayer);

            return parserRules;
        }

        // -----------------------------------------------

        public Parser.Parser GetParser(FileInfo file)
        {
            Parser.Parser p = new Parser.Parser ( file, this.GetParserRules(), this.GetBasicLexer() );
            p.ErrorNode = new ParserError();

            return p;
        }

        #endregion ParserDefinition

        // -----------------------------------------------

        #region LexerDefintion

        // -----------------------------------------------

        private List<ILexerToken> GetLexerRules()
        {
            List<ILexerToken> rules = new List<ILexerToken>();

            Escaper escape = new Escaper ( new ZeichenKette ( "\\" ), new List<Replacer>
            {
                new Replacer ( new ZeichenKette ( "\\" ), "\\" ),
                new Replacer ( new ZeichenKette ( "0" ), "\0" ),
                new Replacer ( new ZeichenKette ( "n" ), "\n" ),
                new Replacer ( new ZeichenKette ( "r" ), "\r" ),
                new Replacer ( new ZeichenKette ( "t" ), "\t" ),
                new Replacer ( new ZeichenKette ( "\"" ), "\"" ),
                new Replacer ( new ZeichenKette ( "\'" ), "\'" ),
            } );
            rules.Add ( new Comment ( new ZeichenKette ( "/*" ), new ZeichenKette ( "*/" ) ) );
            rules.Add ( new Comment ( new ZeichenKette ( "//" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Comment ( new ZeichenKette ( "." ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Digit (  ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "(" ), IdentifierKind.OpenBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ")" ), IdentifierKind.CloseBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "{" ), IdentifierKind.BeginContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "}" ), IdentifierKind.CloseContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "[" ), IdentifierKind.OpenSquareBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "]" ), IdentifierKind.CloseSquareBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "." ), IdentifierKind.Point ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "," ), IdentifierKind.Comma ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ":" ), IdentifierKind.DoublePoint ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "#" ), IdentifierKind.Hash ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "=" ), IdentifierKind.Gleich ) );
            //rules.Add ( new Punctuation ( new ZeichenKette ( "\n" ), IdentifierKind.EndOfCommand ) );
            rules.Add ( new Whitespaces (  ) );
            rules.Add ( new Text ( new ZeichenKette ( "\"" ), new ZeichenKette ( "\"" ), escape ) );
            rules.Add ( new Text ( new ZeichenKette ( "\'" ), new ZeichenKette ( "\'" ), escape ) );
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );

            return rules;
        }

        // -----------------------------------------------

        public Lexer.Lexer GetBasicLexer()
        {
            Lexer.Lexer lexer = new Lexer.Lexer();

            lexer.LexerTokens = this.GetLexerRules();

            return lexer;
        }

        // -----------------------------------------------

        #endregion LexerDefintion

        // -----------------------------------------------

        #region ARM-T32 Definition

        // -----------------------------------------------

        private bool T3ImmediateDefinitionen(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T3ImmediateFormat());
            definition.Formats.Add(new T2LDRSPFormat());
            definition.Formats.Add(new T3LdrRegisterFormat());
            definition.Formats.Add(new ConstFormat());
            definition.Formats.Add(new T2BranchImmediateFormat());
            definition.Formats.Add(new T1InputOutputSmallFormat());

            definition.Commands.Add(new T3ImmediateCommand ("adc", "T3Immediate", 0xF14, 4));
            definition.Commands.Add(new T3ImmediateCommand ("add", "T3Immediate", 0xF10, 4));
            definition.Commands.Add(new T3ImmediateCommand ("sub", "T3Immediate", 0xF1A, 4));
            definition.Commands.Add(new T3ImmediateCommand ("sbc", "T3Immediate", 0xF16, 4));
            //definition.Commands.Add(new T3ImmediateCommand ("mul", "T3Immediate", 0xFB0, 4));
            definition.Commands.Add(new T3ImmediateCommand ("and", "T3Immediate", 0xF00, 4));
            definition.Commands.Add(new T2RegisterImmediateCommand("cmp", "T3Immediate", 0xF1B, 4));
            definition.Commands.Add(new T3ImmediateCommand("eor", "T3Immediate", 0xF08, 4));
            definition.Commands.Add(new T3ImmediateCommand("orr", "T3Immediate", 0xF04, 4));

            definition.Commands.Add(new T2LdrArrayRegisterCommand("ldr", "T2LdrSp", 0x13, 2, 0x7, 4, true));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("ldr", "T1InputOutputSmall", 0xD, 2, 0x7, 4));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("ldr", "T3LdrRegister", 0xF8D, 4, 0xe, 1));
            definition.Commands.Add(new T2LdrConstCommand("ldr", "T3LdrRegister", 0xF85, 9));
            definition.Commands.Add(new T2LdrJumpCommand("ldr", "T3LdrRegister", 0xF85, 9));

            definition.Commands.Add(new T2LdrArrayRegisterCommand("str", "T2LdrSp", 0x12, 2, 0x7, 4, true));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("str", "T1InputOutputSmall", 0xC, 2, 0x7, 4));
            definition.Commands.Add(new T2LdrArrayRegisterCommand("str", "T3LdrRegister", 0xF8C, 4, 0xe, 1));

            definition.Commands.Add(new T2RegisterImmediateCommand("mov", "T3Immediate", 0xF04, 4));

            return true;
        }

        // -----------------------------------------------

        private bool T1RegisterDefinition(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T1SmallRegisterFormat());

            definition.Commands.Add(new T1RegistersCommand("cmp", "T1Register", 0x45, 2));
            definition.Commands.Add(new T1RegistersCommand("mov", "T1Register", 0x46, 2));

            return true;
        }

        // -----------------------------------------------


        private bool T3RegisterDefinitionen(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T3RegisterFormat());
            definition.Formats.Add(new T2RegisterListFormat());

            definition.Commands.Add(new T3RegisterCommand ("adc", "T3Register", 0xEB4, 4));
            definition.Commands.Add(new T3RegisterCommand ("add", "T3Register", 0xEB0, 4));
            definition.Commands.Add(new T3RegisterCommand ("sbc", "T3Register", 0xEB6, 4));
            definition.Commands.Add(new T3RegisterCommand ("sub", "T3Register", 0xEBA, 4));
            definition.Commands.Add(new T3RegisterCommand ("mul", "T3Register", 0xFB0, 4, true));
            definition.Commands.Add(new T3RegisterCommand ("and", "T3Register", 0xEA0, 4));
            definition.Commands.Add(new T3RegisterCommand ("eor", "T3Register", 0xEA8, 4));
            definition.Commands.Add(new T3RegisterCommand ("orr", "T3Register", 0xEA4, 4));
            definition.Commands.Add(new T3StackOneRegisterCommand ("pop", "T3LdrRegister", 0xF85, 4));
            definition.Commands.Add(new T1RegisterListeCommand ("pop", "T2RegisterList", 0xE8B, 4, 12));
            definition.Commands.Add(new T3StackOneRegisterCommand ("push", "T3LdrRegister", 0xF84, 4));
            definition.Commands.Add(new T1RegisterListeCommand ("push", "T2RegisterList", 0xE92, 4, 12));


            return true;
        }

        // -----------------------------------------------

        private bool BranchesDefinitionen(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T4BigBranchFormat());
            definition.Formats.Add(new T1BranchRegisterFormat());
            definition.Formats.Add(new T1BranchConditionFormat());

            definition.Commands.Add(new T4BranchCommand("b", "T2BranchImmediate", 0x1C, 2, 0x3FF));
            //definition.Commands.Add(new T4BranchCommand("b", "T4BigBranch", 0, 4));
            definition.Commands.Add(new T1RegisterCommand("blx", "T1BLX", 0x08F, 2));
            definition.Commands.Add(new T1RegisterCommand("bx", "T1BLX", 0x08E, 2));
            definition.Commands.Add(new T4BranchCommand("bge", "T1BranchCondition", 0xDA, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("blt", "T1BranchCondition", 0xDB, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bgt", "T1BranchCondition", 0xDC, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("ble", "T1BranchCondition", 0xDD, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("bne", "T1BranchCondition", 0xD1, 2, 0xFF));
            definition.Commands.Add(new T4BranchCommand("beq", "T1BranchCondition", 0xD0, 2, 0xFF));

            return true;
        }

        // -----------------------------------------------

        private bool T3asrDefinition(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T3AsrImmediateFormat());
            definition.Formats.Add(new T3AsrRegisterFormat());

            definition.Commands.Add(new T3ImmediateCommand ("asr", "T3AsrIm", 0xEA4, 4));
            definition.Commands.Add(new T3RegisterCommand ("asr", "T3AsrRe", 0xFA4, 4));

            return true;
        }

        // -----------------------------------------------

        private bool GenerateRegister(AssemblerDefinition definition)
        {
            for (uint i = 0; i <= 15; i++)
            {
                definition.Registers.Add(new Register(string.Format("r{0}", i), i));
            }

            definition.Registers.Add(new Register("sp", 13));
            definition.Registers.Add(new Register("lr", 14));
            definition.Registers.Add(new Register("pc", 15));

            return true;
        }

        // -----------------------------------------------

        private Assembler GenerateArmT32Assembler(Assembler assembler)
        {
            assembler.Definition = new AssemblerDefinition();
            assembler.Definition.ProgramCounterIncress = 4;
            assembler.Definition.CommandEntitySize = 2;

            this.T3ImmediateDefinitionen ( assembler.Definition );
            this.T3RegisterDefinitionen ( assembler.Definition );
            this.T3asrDefinition ( assembler.Definition );
            this.T1RegisterDefinition ( assembler.Definition );
            this.BranchesDefinitionen ( assembler.Definition );
            this.GenerateRegister ( assembler.Definition );

            return assembler;
        }

        // -----------------------------------------------

        public Assembler GenerateAssembler(Assembler assembler, string name)
        {
            if (name == "arm-t32") return this.GenerateArmT32Assembler(assembler);

            return null;
        }

        // -----------------------------------------------

        #endregion ARM-T32 Definition

        // -----------------------------------------------
    }
}