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
    }
}