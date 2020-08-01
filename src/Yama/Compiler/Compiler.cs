using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Parser;

namespace Yama.Compiler
{
    public class Compiler
    {

        public List<CompileAlgo> Algos
        {
            get;
            set;
        }

        public FileInfo OutputFile
        {
            get;
            set;
        }

        public StreamWriter Writer
        {
            get;
            set;
        }

        public IProcessorDefinition Definition
        {
            get;
            set;
        }

        public CompileAlgo GetAlgo(string algoName, string mode)
        {
            CompileAlgo result = this.Algos.FirstOrDefault(a=> a.Name == algoName && a.Mode == mode);

            return result;
        }

        public bool AddLine(string assemblyCode, Dictionary<string, string> dictionaries)
        {
            if (dictionaries != null)
            {
                foreach(KeyValuePair<string, string> pair in dictionaries)
                {
                    assemblyCode = assemblyCode.Replace(pair.Key, pair.Value);
                }
            }

            this.Writer.WriteLine(assemblyCode);

            return true;
        }

        public bool Compilen(List<IParseTreeNode> nodes)
        {
            this.Writer = new StreamWriter(this.OutputFile.OpenWrite());

            foreach (IParseTreeNode node in nodes)
            {
                node.Compile(this);
            }

            this.Writer.Close();

            return true;
        }
    }
}