namespace LearnCsStuf.Basic
{
    public interface ILexerToken
    {
        SyntaxKind Kind
        {
            get;
        }
        bool CheckChar ( char zeichen );

        object GetValue ( string text );
    }
}