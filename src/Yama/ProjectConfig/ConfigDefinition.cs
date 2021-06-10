using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Lexer;
using Yama.Parser;
using Yama.ProjectConfig.Nodes;

namespace Yama.ProjectConfig
{
    public class ConfigDefinition
    {

        // -----------------------------------------------

        #region Lexer

        // -----------------------------------------------

        public List<ILexerToken> GetLexerRules()
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
            rules.Add ( new Comment ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ":" ), IdentifierKind.DoublePoint ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "{" ), IdentifierKind.BeginContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "}" ), IdentifierKind.CloseContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "[" ), IdentifierKind.OpenSquareBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "]" ), IdentifierKind.CloseSquareBracket ) );
            rules.Add ( new Digit (  ) );
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

        #endregion Lexer

        // -----------------------------------------------

        #region Parser

        // -----------------------------------------------

        private ParserLayer RootLayer(ParserLayer packagelayer)
        {
            ParserLayer parserLayer = new ParserLayer("root");

            parserLayer.ParserMembers.Add(new SourcePathsNode());

            return parserLayer;
        }

        // -----------------------------------------------

        private ParserLayer PackageLayer()
        {
            ParserLayer parserLayer = new ParserLayer("package");



            return parserLayer;
        }

        // -----------------------------------------------

        private List<ParserLayer> GetParserRules (  )
        {
            List<ParserLayer> parserRules = new List<ParserLayer>();

            ParserLayer packagelayer = this.PackageLayer();
            ParserLayer rootLayer = this.RootLayer(packagelayer);
            parserRules.Add(packagelayer);
            parserRules.Add(rootLayer);

            return parserRules;
        }

        // -----------------------------------------------

        private bool Parse(List<IParseTreeNode> nodes)
        {
            System.IO.FileInfo file = null;

            Parser.Parser p = new Parser.Parser ( file, this.GetParserRules(), this.GetBasicLexer() );
            ParserLayer startlayer = p.ParserLayers.FirstOrDefault(t=>t.Name == "root");

            p.ErrorNode = new ParserError (  );

            if (!p.Parse(startlayer)) return this.PrintingErrors(p);

            IParseTreeNode node = p.ParentContainer;

            nodes.AddRange(node.GetAllChilds);

            return true;
        }

        // -----------------------------------------------

        #endregion Parser

        // -----------------------------------------------

        #region PrintErrors

        // -----------------------------------------------

        private bool PrintingErrors(Parser.Parser p)
        {
            foreach ( IParseTreeNode error in p.ParserErrors )
            {
                IdentifierToken token = error.Token;

                if (token.Kind == IdentifierKind.Unknown) token = error.Token.ParentNode.Token;

                p.PrintSyntaxError ( token, token.Text );
            }

            return false;
        }

        // -----------------------------------------------

        #endregion PrintErrors

        // -----------------------------------------------

    }
}