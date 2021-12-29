namespace Yama.InformationOutput
{

    public class ConsoleWriter : IOutputWriter
    {

        public bool Write(string msg, bool newLine = false, ConsoleColor? backColor = null, ConsoleColor? foreColor = null)
        {
            ConsoleColor originalBackColor = Console.BackgroundColor;
            if (backColor is not null) Console.BackgroundColor = (ConsoleColor)backColor;

            ConsoleColor originalForeColor = Console.ForegroundColor;
            if (foreColor is not null) Console.ForegroundColor = (ConsoleColor)foreColor;

            Console.Write(msg);
            if (newLine) Console.WriteLine();

            Console.BackgroundColor = originalBackColor;
            Console.ForegroundColor = originalForeColor;

            return false;
        }

    }

}