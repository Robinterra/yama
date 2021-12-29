namespace Yama.InformationOutput
{

    public interface IOutputWriter
    {

        bool Write(string? msg, bool newLine = false, ConsoleColor? backcolor = null, ConsoleColor? foreColor = null);

    }

}