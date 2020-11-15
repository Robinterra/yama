using System.IO;
using System.Linq;
using Yama.Parser;

namespace Yama.Assembler
{
    public class Assembler
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public AssemblerDefinition Definition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Parser.Parser Parser
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool Assemble(FileInfo file)
        {
            Definitionen definition = new Definitionen();

            this.Parser = definition.GetParser(file);

            ParserLayer startlayer = this.Parser.ParserLayers.FirstOrDefault(t=>t.Name == "main");

            if (!this.Parser.Parse(startlayer))
            {
                System.Console.WriteLine("Error");

                foreach (IParseTreeNode node in this.Parser.ParserErrors)
                {
                    this.Parser.PrintSyntaxError(node.Token,"Error");
                }

                return false;
            }

            this.Parser.PrintPretty(this.Parser.ParentContainer);

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}