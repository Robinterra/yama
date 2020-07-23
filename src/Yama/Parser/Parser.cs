using System;
using System.IO;
using System.Collections.Generic;
using Yama.Lexer;
using System.Linq;

namespace Yama.Parser
{
    public class Parser
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        private int grosstePrio = -1;
        private List<IParseTreeNode> possibleParents;

        // -----------------------------------------------

        #endregion vars

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public int Position
        {
            get;
            private set;
        }
        public int Start { get; private set; }

        // -----------------------------------------------

        public FileInfo Fileinfo
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Stream InputStream
        {
            get;
            set;
        }

        // -----------------------------------------------

        private Lexer.Lexer Tokenizer
        {
            get;
            set;
        }

        // -----------------------------------------------

        private List<SyntaxToken> CleanTokens
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<SyntaxToken> SyntaxErrors
        {
            get;
            set;
        }

        public Container ParentContainer
        {
            get;
            private set;
        }

        // -----------------------------------------------

        public int Max
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IParseTreeNode ErrorNode
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> ParserErrors
        {
            get;
        } = new List<IParseTreeNode> ();

        // -----------------------------------------------

        public List<ParserLayer> ParserLayers
        {
            get;
        } = new List<ParserLayer>();

        // -----------------------------------------------

        public Stack<ParserLayer> ParserStack
        {
            get;
            set;
        } = new Stack<ParserLayer>();

        // -----------------------------------------------

        public ParserLayer CurrentLayer
        {
            get
            {
                try
                {
                    return this.ParserStack.Peek();
                }
                catch
                {
                    return this.ParserLayers.FirstOrDefault();
                }
            }
        }

        // -----------------------------------------------

        public List<IParseTreeNode> CurrentParserMembers
        {
            get
            {
                return this.CurrentLayer.ParserMembers;
            }
        }

        // -----------------------------------------------

        public SyntaxToken Current
        {
            get
            {
                if (this.CleanTokens.Count <= this.Position) return null;

                SyntaxToken result = this.CleanTokens[this.Position];
                result.Position = this.Position;

                return result;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        private Parser ( List<ParserLayer> layers, Lexer.Lexer lexer )
        {
            this.SyntaxErrors = new List<SyntaxToken> (  );
            this.ParserLayers = layers;
            this.Tokenizer = lexer;
        }

        // -----------------------------------------------

        public Parser ( FileInfo file, List<ParserLayer> layers, Lexer.Lexer lexer )
            : this ( layers, lexer )
        {
            this.Fileinfo = file;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public List<IParseTreeNode> ParseCleanTokens(SyntaxToken left, int start, int position)
        {
            if (left.Node == null) return this.ParseCleanTokens(start, position);

            List<IParseTreeNode> result = new List<IParseTreeNode>();

            SyntaxToken token = this.GetParent(left);

            if (token != null) result.Add(token.Node);
            else result.Add(left.Node);

            return result;
        }

        // -----------------------------------------------

        public IParseTreeNode GetRule<T>() where T : IParseTreeNode
        {
            foreach (ParserLayer layer in this.ParserLayers)
            {
                foreach ( IParseTreeNode rule in layer.ParserMembers )
                {
                    if ( rule is T ) return rule;
                }
            }

            return null;
        }

        // -----------------------------------------------

        public bool PrintSyntaxError(SyntaxToken token, string msg, string nexterrormsg = "Syntax error")
        {
            //if (token.Kind != SyntaxKind.Unknown) return false;

            this.SyntaxErrors.Add ( token );

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( "{4}({0},{1}): {5} - {3} \"{2}\"", token.Line, token.Column, token.Text, msg, this.Fileinfo.FullName, nexterrormsg );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        public SyntaxToken GetParent ( SyntaxToken token )
        {
            if ( token.ParentNode == null ) return token;

            return this.GetParent ( token.ParentNode.Token );
        }

        // -----------------------------------------------

        private bool CheckTokens()
        {
            this.Tokenizer.Daten = this.InputStream;
            this.CleanTokens = new List<SyntaxToken>();

            foreach (SyntaxToken token in this.Tokenizer)
            {
                if (token.Kind == SyntaxKind.Whitespaces) continue;
                if (token.Kind == SyntaxKind.Comment) continue;
                if (token.Kind == SyntaxKind.Unknown && this.PrintSyntaxError ( token, "unkown char" )) continue;

                this.CleanTokens.Add ( token );
                //Console.Write ( token.Kind.ToString() + " : " );
                //Console.WriteLine ( token.Value );
            }

            return this.SyntaxErrors.Count == 0;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> ParseCleanTokens ( int von, int bis, bool withlostchilds = false )
        {
            if (this.CleanTokens.Count == 0) return null;
            if ( von == bis ) return new List<IParseTreeNode>();
            if ( von >= bis ) return null;
            if (bis > this.Max) return null;

            int currentpos = this.Position;
            this.Position = von;
            int orgstart = this.Start;
            this.Start = von;
            int tempmax = this.Max;
            this.Max = bis;
            List<IParseTreeNode> possibleParentsTemp = this.possibleParents;

            this.possibleParents = new List<IParseTreeNode>();
            List<IParseTreeNode> nodeParents = new List<IParseTreeNode>();

            bool isok = true;
            bool vorgangOhneNeueNodes = true;
            while ( isok )
            {
                IParseTreeNode node = this.ParsePrimary ( this.Max );

                if ( node != null ) vorgangOhneNeueNodes = false;

                isok = node != null;

                this.possibleParents.Add ( node );

                this.NextToken (  );

                if ( isok ) continue;
                if ( vorgangOhneNeueNodes ) continue;

                this.Position = von;
                isok = true;
                vorgangOhneNeueNodes = true;
            }

            /*foreach ( IParseTreeNode node in this.possibleParents )
            {
                if ( node == null ) continue;
                if ( node.Token.ParentNode != null ) continue;

                nodeParents.Add ( node );
            }*/

            nodeParents = this.GetCleanNodeParents();

            this.Position = currentpos;
            this.Max = tempmax;
            this.Start = orgstart;
            this.possibleParents = possibleParentsTemp;

            return nodeParents;
        }

        private List<IParseTreeNode> GetCleanNodeParents()
        {
            if (this.Max - this.Start < 0) return new List<IParseTreeNode>();

            List<SyntaxToken> unclean = this.CleanTokens.GetRange(this.Start, this.Max - this.Start);
            List<IParseTreeNode> result = new List<IParseTreeNode>();
            List<IParseTreeNode> clean = new List<IParseTreeNode>();

            foreach ( SyntaxToken token in unclean )
            {
                if (token == null) continue;
                if (token.Node == null) continue;
                if (clean.Contains(token.Node)) continue;

                clean.Add(token.Node);
            }

            foreach ( IParseTreeNode node in clean )
            {
                if ( node.Token.ParentNode != null ) continue;

                result.Add ( node );
            }

            return result;
        }

        private IParseTreeNode ParsePrimary ( int max )
        {
            int pos = this.Position;

            while ( this.Position < max )
            {
                if ( this.Current.Node == null )
                {
                    IParseTreeNode node = this.ParseSteuerTokens ( this.Current );
                    if ( node != null ) return node;
                }

                this.NextToken (  );
            }

            this.Position = pos;

            for ( int i = this.GetGrosstePrio (  ); i > -1; i-- )
            {
                pos = this.Position;

                IParseTreeNode node = this.ParsePrimaryPrioSystem ( max, i );

                this.Position = pos;

                if ( node != null ) return node;
            }

            while ( this.Position < max )
            {
                if ( this.Current.Node == null )
                {
                    return this.SyntaxErrorToken ( this.Current );
                }

                this.NextToken (  );
            }

            return null;
        }

        // -----------------------------------------------

        private IParseTreeNode ParsePrimaryPrioSystem ( int max, int prio )
        {
            while ( this.Position < max )
            {
                if ( this.Current.Node == null )
                {
                    IParseTreeNode node = this.ParsePrioSystem ( this.Current, prio );
                    if ( node != null ) return node;
                }

                this.NextToken (  );
            }

            return null;
        }

        // -----------------------------------------------

        public bool NextToken (  )
        {
            this.Position++;
            return true;
        }

        // -----------------------------------------------

        private IParseTreeNode GetNodeFromToken ( SyntaxToken token )
        {
            if ( token.ParentNode != null ) return this.GetNodeFromToken ( token.ParentNode.Token );

            return token.Node;
        }

        // -----------------------------------------------

        public IParseTreeNode ParseCleanToken ( SyntaxToken token, ParserLayer neuerLayer )
        {
            if ( token == null ) return this.SyntaxErrorToken ( null );
            if ( token.Node != null ) return this.GetNodeFromToken ( token );

            this.ActivateLayer(neuerLayer);

            IParseTreeNode result = this.ParseEndExpression ( token );

            if ( result != null ) { this.VorherigesLayer(); return result; }

            result = this.ParsePrioSystem ( token, this.GetGrosstePrio (  ), true );

            if ( result != null ) { this.VorherigesLayer(); return result; }

            result = this.ParseSteuerTokens ( token );

            this.VorherigesLayer();

            if ( result != null ) return result;

            return this.SyntaxErrorToken ( token );
        }

        public IParseTreeNode ParseCleanToken ( SyntaxToken token )
        {
            return this.ParseCleanToken(token, this.CurrentLayer);
        }

        private IParseTreeNode ParseEndExpression(SyntaxToken token)
        {
            foreach ( IParseTreeNode node in this.CurrentParserMembers )
            {
                if (!(node is IEndExpression)) continue;

                IParseTreeNode nodet = node.Parse(this, token);

                if (nodet != null) return nodet;
            }

            return null;
        }

        // -----------------------------------------------

        private IParseTreeNode SyntaxErrorToken ( SyntaxToken token )
        {
            if ( token == null ) token = new SyntaxToken ( SyntaxKind.Unknown, -1, -1, -1, "Unexpectet Error", "Unexpectet Error" );

            //token.Kind = SyntaxKind.Unknown;

            IParseTreeNode error = this.ErrorNode.Parse ( this, token );

            if (error == null) return null;

            this.ParserErrors.Add ( error );

            return error;
        }

        // -----------------------------------------------

        private IParseTreeNode ParseSteuerTokens ( SyntaxToken token )
        {
            foreach ( IParseTreeNode member in this.CurrentParserMembers )
            {
                if ( member is IPriority ) continue;

                IParseTreeNode result = member.Parse ( this, token );

                if ( result != null )
                    return result;
            }

            return null;
        }

        // -----------------------------------------------

        private int GetGrosstePrio()
        {
            if ( this.grosstePrio != -1 ) return this.grosstePrio;

            foreach ( IParseTreeNode member in this.CurrentParserMembers )
            {
                if ( !(member is IPriority t) ) continue;
                if ( t.Prio < this.grosstePrio ) continue;

                this.grosstePrio = t.Prio;
            }

            return this.grosstePrio;
        }

        // -----------------------------------------------

        private IParseTreeNode ParsePrioSystem ( SyntaxToken token, int prio, bool isrekursiv = false )
        {
            if ( prio < 0 ) return null;

            foreach ( IParseTreeNode member in this.CurrentParserMembers )
            {
                if ( !(member is IPriority t) ) continue;
                if ( t.Prio != prio ) continue;

                IParseTreeNode result = member.Parse ( this, token );

                if ( result != null ) return result;
            }

            if (isrekursiv) return this.ParsePrioSystem ( token, prio - 1, isrekursiv );

            return null;
        }

        // -----------------------------------------------

        private IParseTreeNode ParseOneMember ( IParseTreeNode member, SyntaxToken token )
        {
            int pos = this.Position;

            IParseTreeNode result = member.Parse ( this, token );

            if ( result != null ) return result;

            this.Position = pos;

            return null;
        }

        // -----------------------------------------------

        public bool Parse ( ParserLayer start )
        {
            if (start == null) return false;
            if (!this.Fileinfo.Exists) return false;

            this.ActivateLayer(start);

            this.InputStream = File.OpenRead ( this.Fileinfo.FullName );

            if (!this.CheckTokens (  )) return false;

            this.Max = this.CleanTokens.Count;
            List<IParseTreeNode> parentNodes = this.ParseCleanTokens ( 0, this.CleanTokens.Count );
            if ( parentNodes == null ) return false;

            this.ParentContainer = new Container (  );
            this.ParentContainer.Statements = parentNodes;
            this.ParentContainer.Token = new SyntaxToken ( SyntaxKind.BeginContainer, 0, 0, 0, "File", this.Fileinfo.FullName );

            foreach ( IParseTreeNode node in parentNodes )
            {
                node.Token.ParentNode = this.ParentContainer;
            }

            this.PrintPretty ( this.ParentContainer );

            return this.ParserErrors.Count == 0;
        }

        // -----------------------------------------------

        private bool VorherigesLayer()
        {
            this.ParserStack.Pop();

            return true;
        }

        // -----------------------------------------------

        public bool ActivateLayer(ParserLayer start)
        {
            this.ParserStack.Push(start);

            return true;
        }

        // -----------------------------------------------

        public SyntaxToken FindEndToken ( SyntaxToken begin, SyntaxKind endKind, SyntaxKind escapeKind )
        {
            SyntaxToken kind = begin;

            for ( int i = 1; kind.Kind != endKind; i++ )
            {
                kind = this.Peek ( begin, i );

                if ( kind == null ) return null;

                if ( kind.Kind != escapeKind ) continue;

                IParseTreeNode nodeCon = this.ParseCleanToken ( kind );

                if ( nodeCon == null ) return null;

                if ( !(nodeCon is IContainer c) ) return null;

                i = c.Ende.Position - begin.Position;
            }

            return kind;
        }

        public SyntaxToken FindEndTokenWithoutParse ( SyntaxToken begin, SyntaxKind endKind, SyntaxKind escapeKind )
        {
            SyntaxToken kind = begin;

            int counter = 0;

            for ( int i = 1; kind.Kind != endKind || counter > 0; i++ )
            {
                if ( kind.Kind == endKind ) counter--;

                kind = this.Peek ( begin, i );

                if ( kind == null ) return null;

                if ( kind.Kind == escapeKind ) counter++;
            }

            return kind;
        }

        // -----------------------------------------------

        public bool Repleace ( SyntaxToken token, int pos )
        {
            this.CleanTokens[pos] = token;

            return true;
        }

        // -----------------------------------------------

        private bool PrintPretty ( IParseTreeNode node, string lebchilds = "" )
        {
            if (node == null) return true;

            Console.Write ( node.Token.Value );
            Console.WriteLine (  );

            List<IParseTreeNode> childs = node.GetAllChilds;

            if ( childs == null ) return true;

            string neuchild = lebchilds + "│   ";
            int counter = 0;
            string normalChildPrint = "├── ";
            foreach (IParseTreeNode child in childs)
            {
                if (child == null) continue;

                if (counter >= childs.Count - 1)
                {
                    normalChildPrint = "└── ";
                    neuchild = lebchilds + "    ";
                }

                Console.Write ( lebchilds );
                Console.Write ( normalChildPrint );

                this.PrintPretty ( child, neuchild );

                counter++;
            }

            return true;
        }

        // -----------------------------------------------

        public SyntaxToken FindAToken ( SyntaxToken von, SyntaxKind zufinden )
        {
            SyntaxToken kind = von;

            for ( int i = 1; kind != null; i++ )
            {
                if ( kind.Kind == zufinden && kind.Node == null ) return kind;

                kind = this.Peek ( von, i );
            }

            return null;
        }

        // -----------------------------------------------

        public SyntaxToken Peek ( int offset )
        {
            return this.Peek ( this.Current, offset );
        }

        // -----------------------------------------------

        public SyntaxToken Peek ( SyntaxToken token, int offset )
        {
            if (token == null) return null;
            if (this.Max <= offset + token.Position) return null;
            if (0 > offset + token.Position) return null;

            SyntaxToken result = this.CleanTokens[offset + token.Position];

            result.Position = offset + token.Position;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --