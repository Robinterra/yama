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

            definition.Commands.Add(new T3ImmediateCommand ("adc", "T3Immediate", 0xF14, 4));
            definition.Commands.Add(new T3ImmediateCommand ("add", "T3Immediate", 0xF20, 4));
            definition.Commands.Add(new T3ImmediateCommand ("and", "T3Immediate", 0xF00, 4));

            return true;
        }

        // -----------------------------------------------


        private bool T3RegisterDefinitionen(AssemblerDefinition definition)
        {
            definition.Formats.Add(new T3RegisterFormat());

            definition.Commands.Add(new T3RegisterCommand ("adc", "T3Register", 0xEB4, 4));
            definition.Commands.Add(new T3RegisterCommand ("add", "T3Register", 0xEB0, 4));
            definition.Commands.Add(new T3RegisterCommand ("and", "T3Register", 0xEA0, 4));
            

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
            for (uint i = 0; i <= 12; i++)
            {
                definition.Registers.Add(new Register(string.Format("r{0}", i), i));
            }

            definition.Registers.Add(new Register("sp", 13));
            definition.Registers.Add(new Register("lr", 14));
            definition.Registers.Add(new Register("pc", 15));

            return true;
        }

        // -----------------------------------------------

        public Assembler GenerateAssembler()
        {
            Assembler assembler = new Assembler();
            assembler.Definition = new AssemblerDefinition();

            this.T3ImmediateDefinitionen ( assembler.Definition );
            this.T3RegisterDefinitionen ( assembler.Definition );
            this.GenerateRegister ( assembler.Definition );

            return assembler;
        }

        // -----------------------------------------------

        #endregion ARM-T32 Definition

        // -----------------------------------------------
    }
}