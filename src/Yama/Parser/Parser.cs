using System;
using System.IO;
using System.Collections.Generic;
using Yama.Lexer;
using System.Linq;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class Parser
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        private int grosstePrio = -1;

        public List<IParseTreeNode> possibleParents;

        // -----------------------------------------------

        #endregion vars

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public int Position
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int Start
        {
            get;
            set;
        }

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

        private List<IdentifierToken> CleanTokens
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IdentifierToken> SyntaxErrors
        {
            get;
            set;
        }

        // -----------------------------------------------

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

        public IdentifierToken Current
        {
            get
            {
                if (this.CleanTokens.Count <= this.Position) return null;

                IdentifierToken result = this.CleanTokens[this.Position];
                result.Position = this.Position;

                return result;
            }
        }

        // -----------------------------------------------

        public List<IParseTreeNode> MethodTag
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        private Parser ( List<ParserLayer> layers, Lexer.Lexer lexer )
        {
            this.SyntaxErrors = new List<IdentifierToken> (  );
            this.MethodTag = new List<IParseTreeNode> (  );
            this.ParserLayers = layers;
            this.Tokenizer = lexer;
            lexer.Reset();
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

        public bool NewParse()
        {
            this.SyntaxErrors.Clear();
            this.Tokenizer.Reset();
            if (this.CleanTokens != null) this.CleanTokens.Clear();

            return true;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> ParseCleanTokens(IdentifierToken left, int start, int position)
        {
            if (left.Node == null) return this.ParseCleanTokens(start, position);

            List<IParseTreeNode> result = new List<IParseTreeNode>();

            IdentifierToken token = this.GetParent(left);

            if (token != null) result.Add(token.Node);
            else result.Add(left.Node);

            return result;
        }

        // -----------------------------------------------

        public T GetRule<T>() where T : IParseTreeNode
        {
            foreach (ParserLayer layer in this.ParserLayers)
            {
                IParseTreeNode rule = layer.ParserMembers.Find(t => t is T);

                if (rule != null) return (T)rule;
            }

            return default;
        }

        // -----------------------------------------------

        public bool PrintSyntaxError(IdentifierToken token, string msg, string nexterrormsg = "Syntax error")
        {
            this.SyntaxErrors.Add ( token );

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            string filename = "Stream";
            if (this.Fileinfo != null) filename = this.Fileinfo.FullName;

            Console.Error.WriteLine ( "{4}({0},{1}): {5} - {3} \"{2}\"", token.Line, token.Column, token.Text, msg, filename, nexterrormsg );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        public IdentifierToken GetParent ( IdentifierToken token )
        {
            if ( token.ParentNode == null ) return token;

            return this.GetParent ( token.ParentNode.Token );
        }

        // -----------------------------------------------

        private bool CheckTokens()
        {
            this.Tokenizer.Daten = this.InputStream;
            this.CleanTokens = new List<IdentifierToken>();

            foreach (IdentifierToken token in this.Tokenizer)
            {
                if (token.Kind == IdentifierKind.Whitespaces) continue;
                if (token.Kind == IdentifierKind.Comment) continue;
                if (token.Kind == IdentifierKind.Unknown && this.PrintSyntaxError ( token, "unkown char" )) continue;

                token.FileInfo = this.Fileinfo;

                this.CleanTokens.Add ( token );
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

            ParserPositionStack pos = new ParserPositionStack(this);
            this.Position = von;
            this.Start = von;
            this.Max = bis;

            this.possibleParents = new List<IParseTreeNode>();

            bool isok = true;
            bool vorgangOhneNeueNodes = true;
            while ( isok )
            {
                IParseTreeNode node = this.ParsePrimary ( this.Max );

                if ( node != null && !(node is ParserError) ) vorgangOhneNeueNodes = false;

                isok = node != null;

                this.possibleParents.Add ( node );

                this.NextToken (  );

                if ( isok ) continue;
                if ( vorgangOhneNeueNodes ) continue;

                this.Position = von;
                isok = true;
                vorgangOhneNeueNodes = true;
            }

            List<IParseTreeNode> nodeParents = this.GetCleanNodeParents();

            this.FindTokensThatHasNoMatchNode();

            pos.SetToParser(this);

            return nodeParents;
        }

        // -----------------------------------------------

        private bool FindTokensThatHasNoMatchNode()
        {
            for (int i = this.Start; i < this.Max; i++)
            {
                if ( this.Peek(i) == null ) continue;
                if ( this.Peek(i).Node != null ) continue;

                this.SyntaxErrorToken ( this.Peek(i) );
            }

            return true;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> PopMethodTag()
        {
            List<IParseTreeNode> result = this.MethodTag;

            this.MethodTag = new List<IParseTreeNode>();

            return result;
        }

        // -----------------------------------------------

        private List<IParseTreeNode> GetCleanNodeParents()
        {
            if (this.Max - this.Start < 0) return new List<IParseTreeNode>();

            List<IdentifierToken> unclean = this.CleanTokens.GetRange(this.Start, this.Max - this.Start);
            List<IParseTreeNode> result = new List<IParseTreeNode>();
            List<IParseTreeNode> clean = new List<IParseTreeNode>();

            foreach ( IdentifierToken token in unclean )
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

        // -----------------------------------------------

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
                if ( this.Current.Node == null ) return this.SyntaxErrorToken ( this.Current );

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

        private IParseTreeNode GetNodeFromToken ( IdentifierToken token )
        {
            if ( token.ParentNode != null ) return this.GetNodeFromToken ( token.ParentNode.Token );

            return token.Node;
        }

        // -----------------------------------------------

        public IParseTreeNode ParseCleanToken ( IdentifierToken token, ParserLayer neuerLayer )
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

        // -----------------------------------------------

        public IParseTreeNode ParseCleanToken ( IdentifierToken token )
        {
            return this.ParseCleanToken(token, this.CurrentLayer);
        }

        // -----------------------------------------------

        private IParseTreeNode ParseEndExpression(IdentifierToken token)
        {
            Request.RequestParserTreeParser request = new Request.RequestParserTreeParser(this, token);

            foreach ( IParseTreeNode node in this.CurrentParserMembers )
            {
                if (!(node is IEndExpression)) continue;

                IParseTreeNode nodet = node.Parse ( request );

                if (nodet == null) continue;

                this.CleanPareNode ( nodet );

                return nodet;
            }

            return null;
        }

        // -----------------------------------------------

        private IParseTreeNode SyntaxErrorToken ( IdentifierToken token )
        {
            if ( token == null ) token = new IdentifierToken ( IdentifierKind.Unknown, -1, -1, -1, "Unexpectet Error", "Unexpectet Error" );

            IParseTreeNode error = this.ErrorNode.Parse ( new Request.RequestParserTreeParser ( this, token ) );

            if (error == null) return null;

            this.ParserErrors.Add ( error );

            return error;
        }

        // -----------------------------------------------

        private IParseTreeNode ParseSteuerTokens ( IdentifierToken token )
        {
            Request.RequestParserTreeParser request = new Request.RequestParserTreeParser ( this, token );

            foreach ( IParseTreeNode member in this.CurrentParserMembers )
            {
                if ( member is IPriority ) continue;

                IParseTreeNode result = member.Parse ( request );

                if ( result == null ) continue;

                this.CleanPareNode ( result );

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

        private IParseTreeNode ParsePrioSystem ( IdentifierToken token, int prio, bool isrekursiv = false )
        {
            if ( prio < 0 ) return null;

            Request.RequestParserTreeParser request = new Request.RequestParserTreeParser ( this, token );

            foreach ( IParseTreeNode member in this.CurrentParserMembers )
            {
                if ( !(member is IPriority t) ) continue;
                if ( t.Prio != prio ) continue;

                IParseTreeNode result = member.Parse ( request );

                if ( result == null ) continue;

                this.CleanPareNode ( result );

                return result;
            }

            if (isrekursiv) return this.ParsePrioSystem ( token, prio - 1, isrekursiv );

            return null;
        }

        // -----------------------------------------------

        public IParseTreeNode TryToParse ( IParseTreeNode rule, IdentifierToken token )
        {
            IParseTreeNode result = rule.Parse ( new RequestParserTreeParser (this, token) );
            if ( result == null ) return null;

            this.CleanPareNode ( result );

            return result;
        }
        
        // -----------------------------------------------

        private bool CleanPareNode ( IParseTreeNode result )
        {
            if ( result.AllTokens == null ) return false;

            foreach ( IdentifierToken token in result.AllTokens )
            {
                token.Node = result;
            }

            foreach ( IParseTreeNode child in result.GetAllChilds )
            {
                child.Token.ParentNode = result;
            }

            return true;
        }

        // -----------------------------------------------

        private IParseTreeNode ParseOneMember ( IParseTreeNode member, IdentifierToken token )
        {
            int pos = this.Position;

            IParseTreeNode result = member.Parse ( new Request.RequestParserTreeParser ( this, token ) );

            if ( result != null ) return result;

            this.Position = pos;

            return null;
        }

        // -----------------------------------------------

        public bool Parse ( ParserLayer start )
        {
            if (start == null) return false;
            if (!this.Fileinfo.Exists) return this.FileNotFoundError();

            this.ActivateLayer(start);

            this.InputStream = File.OpenRead ( this.Fileinfo.FullName );

            if (!this.CheckTokens (  )) return false;

            this.Max = this.CleanTokens.Count;
            List<IParseTreeNode> parentNodes = this.ParseCleanTokens ( 0, this.CleanTokens.Count );
            if ( parentNodes == null ) return this.EmptyFileError();

            this.ParentContainer = new Container (  );
            this.ParentContainer.Statements = parentNodes;
            this.ParentContainer.Token = new IdentifierToken ( IdentifierKind.BeginContainer, 0, 0, 0, "File", this.Fileinfo.FullName );

            foreach ( IParseTreeNode node in parentNodes )
            {
                node.Token.ParentNode = this.ParentContainer;
            }

            return this.ParserErrors.Count == 0;
        }

        // -----------------------------------------------

        public bool Parse ( ParserLayer start, MemoryStream stream )
        {
            if (start == null) return false;

            this.ActivateLayer(start);

            this.InputStream = stream;

            if (!this.CheckTokens (  )) return false;

            this.Max = this.CleanTokens.Count;
            List<IParseTreeNode> parentNodes = this.ParseCleanTokens ( 0, this.CleanTokens.Count );

            if ( parentNodes == null )
                parentNodes = new List<IParseTreeNode>();

            this.ParentContainer = new Container (  );
            this.ParentContainer.Statements = parentNodes;
            this.ParentContainer.Token = new IdentifierToken ( IdentifierKind.BeginContainer, 0, 0, 0, "Stream", "stream" );

            foreach ( IParseTreeNode node in parentNodes )
            {
                node.Token.ParentNode = this.ParentContainer;
            }

            return this.ParserErrors.Count == 0;
        }

        // -----------------------------------------------

        private bool EmptyFileError()
        {
            IdentifierToken token = new IdentifierToken ( IdentifierKind.Unknown, -1, -1, -1, this.Fileinfo.Name, "Empty File" );

            this.PrintSyntaxError(token, "Empty File");

            return false;
        }

        // -----------------------------------------------

        private bool FileNotFoundError()
        {
            IdentifierToken token = new IdentifierToken ( IdentifierKind.Unknown, -1, -1, -1, this.Fileinfo.Name, "File not Found" );

            this.PrintSyntaxError(token, "File not Found");

            return false;
        }

        // -----------------------------------------------

        public bool VorherigesLayer()
        {
            this.ParserStack.Pop();

            return true;
        }

        // -----------------------------------------------

        public bool ActivateLayer(ParserLayer start)
        {
            if (start == null) return false;

            this.ParserStack.Push(start);

            return true;
        }

        // -----------------------------------------------

        public IdentifierToken FindEndToken ( IdentifierToken begin, IdentifierKind endKind, IdentifierKind escapeKind )
        {
            IdentifierToken kind = begin;

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

        // -----------------------------------------------

        public IdentifierToken FindEndTokenWithoutParse ( IdentifierToken begin, IdentifierKind endKind, IdentifierKind escapeKind )
        {
            IdentifierToken kind = begin;

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

        public bool Repleace ( IdentifierToken token, int pos )
        {
            this.CleanTokens[pos] = token;

            return true;
        }

        // -----------------------------------------------

        public bool PrintPretty ( IParseTreeNode node, string lebchilds = "" )
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

        public IdentifierToken FindAToken ( IdentifierToken von, IdentifierKind zufinden )
        {
            IdentifierToken kind = von;

            for ( int i = 1; kind != null; i++ )
            {
                if ( kind.Kind == zufinden && kind.Node == null ) return kind;

                kind = this.Peek ( von, i );
            }

            return null;
        }

        // -----------------------------------------------

        public IdentifierToken Peek ( int offset )
        {
            return this.Peek ( this.Current, offset );
        }

        // -----------------------------------------------

        public IdentifierToken Peek ( IdentifierToken token, int offset )
        {
            if (token == null) return null;
            if (this.Max <= offset + token.Position) return null;
            if (0 > offset + token.Position) return null;

            IdentifierToken result = this.CleanTokens[offset + token.Position];

            result.Position = offset + token.Position;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --