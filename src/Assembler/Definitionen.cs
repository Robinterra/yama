using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Assembler.ARMT32;
using Yama.Assembler.Commands.AVR.Asm;
using Yama.Assembler.Definitions;
using Yama.Assembler.Runtime;
using Yama.Lexer;
using Yama.Parser;
using Command1Register = Yama.Assembler.Runtime.Command1Register;
using Command2Register = Yama.Assembler.Runtime.Command2Register;

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
            layer.ParserMembers.Add(new DataNode());
            layer.ParserMembers.Add(new WordNode());
            layer.ParserMembers.Add(new SpaceNode());
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
            layer.ParserMembers.Add(new PointerNode());
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

        public Parser.Parser GetParser(ParserInputData inputData)
        {
            Parser.Parser p = new Parser.Parser (this.GetParserRules(), this.GetBasicLexer(inputData.InputStream), inputData);
            p.ErrorNode = new ParserError();

            return p;
        }

        // -----------------------------------------------

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
            rules.Add ( new KeyWord ( ".data", IdentifierKind.Base ) );
            rules.Add ( new KeyWord ( ".datalist", IdentifierKind.Base ) );
            rules.Add ( new KeyWord ( ".word", IdentifierKind.Base ) );
            rules.Add ( new KeyWord ( ".space", IdentifierKind.Base ) );
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
            rules.Add ( new Punctuation ( new ZeichenKette ( "*" ), IdentifierKind.StarToken ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "+" ), IdentifierKind.PlusToken ) );
            //rules.Add ( new Punctuation ( new ZeichenKette ( "\n" ), IdentifierKind.EndOfCommand ) );
            rules.Add ( new Whitespaces (  ) );
            rules.Add ( new Text ( new ZeichenKette ( "\"" ), new ZeichenKette ( "\"" ), escape ) );
            rules.Add ( new Text ( new ZeichenKette ( "\'" ), new ZeichenKette ( "\'" ), escape ) );
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit ( false ), new Underscore (  ) } ) );

            return rules;
        }

        // -----------------------------------------------

        public Lexer.Lexer GetBasicLexer(Stream stream)
        {
            Lexer.Lexer lexer = new Lexer.Lexer(stream);

            lexer.LexerTokens.AddRange(this.GetLexerRules());

            return lexer;
        }

        // -----------------------------------------------

        #endregion LexerDefintion

        // -----------------------------------------------

        public Assembler? GenerateAssembler(Assembler? assembler, string name)
        {
            if (assembler is null) return null;

            List<IAssemblerDefinition> definitions = new List<IAssemblerDefinition>
            {
                new ArmT32Definition(),
                new RuntimeDefinition(),
                new ArmA32Definition(),
                new Arm2x86Def(),
            };

            IAssemblerDefinition? definition = definitions.Find(t=>t.Name == name);
            if (definition is null) return null;

            return definition.SetupDefinition(assembler);
        }

        // -----------------------------------------------

        #region AVR Definition

        // -----------------------------------------------

        private bool AvrFormat1Def(AssemblerDefinition definition)
        {
            definition.Formats.Add(new AvrFormat1());

            definition.Commands.Add(new AvrCommand2Register("adc", "Format1", 0x7, 2));
            definition.Commands.Add(new AvrCommand2Register("add", "Format1", 0x3, 2));
            definition.Commands.Add(new AvrCommand2Register("and", "Format1", 0x8, 2));

            return true;
        }

        // -----------------------------------------------

        #endregion AVR Definition
    }
}