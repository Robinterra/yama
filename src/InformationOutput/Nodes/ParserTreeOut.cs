using Yama.Lexer;
using Yama.Parser;

namespace Yama.InformationOutput.Nodes
{

    public class ParserTreeOut : IOutputNode
    {

        #region get/set

        public IParseTreeNode Root
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ParserTreeOut(IParseTreeNode root)
        {
            this.Root = root;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            return this.PrintPretty(Root, o);
        }

        // -----------------------------------------------

        private bool PrintPretty (IParseTreeNode node, RequestOutput o, string lebchilds = "")
        {
            o.Info.Write(node.Token.Value is null ? string.Empty : node.Token.Value.ToString());
            o.Info.Write("\n");

            List<IParseTreeNode> childs = node.GetAllChilds;

            string neuchild = lebchilds + "│   ";
            int counter = 0;
            string normalChildPrint = "├── ";
            foreach (IParseTreeNode child in childs)
            {
                if (counter >= childs.Count - 1)
                {
                    normalChildPrint = "└── ";
                    neuchild = lebchilds + "    ";
                }

                o.Info.Write(lebchilds);
                o.Info.Write(normalChildPrint);

                this.PrintPretty ( child, o, neuchild );

                counter++;
            }

            return true;
        }

        // -----------------------------------------------

        #endregion methods

    }

}