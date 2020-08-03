using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Parser;

namespace Yama.Compiler
{
    public class Compiler
    {

        public CompileHeader Header
        {
            get;
            set;
        } = new CompileHeader();

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

        public List<ICompileRoot> AssemblerSequence
        {
            get;
            set;
        }
        public FunktionsDeklaration MainFunction { get; internal set; }
        public List<CompilerError> Errors { get; set; } = new List<CompilerError>();

        public CompileAlgo GetAlgo(string algoName, string mode)
        {
            CompileAlgo result = this.Definition.Algos.FirstOrDefault(a=> a.Name == algoName && a.Mode == mode);

            if (result != null) return result;

            this.AddError(string.Format("Der Algorithmus {0} mit dem Modus {1} konnte nicht gefunden werden!", algoName, mode));

            return null;
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
            if (this.Definition == null) return this.AddError("Keine definition zur Übersetzung in Assembler gesetzt");

            this.Header.Compile(this, this.MainFunction);

            foreach (IParseTreeNode node in nodes)
            {
                if (!node.Compile(this)) this.AddError("Beim assembelieren ist in einer Klasse ein Fehler aufgetreten", node);
            }

            if (this.Errors.Count != 0) return false;

            this.RunAssmblerSequence();

            return this.Errors.Count == 0;
        }

        private bool RunAssmblerSequence()
        {
            if (this.OutputFile.Exists) this.OutputFile.Delete();

            try
            {
                this.Writer = new StreamWriter(this.OutputFile.OpenWrite());
            }
            catch (Exception e) { return this.AddError(string.Format("Die Datei in der Assemblercode geschrieben werden sollte konnte nicht angelegt werden. {0}", e.Message)); }

            foreach (ICompileRoot root in this.AssemblerSequence)
            {
                if (!root.InFileCompilen(this)) this.AddError("Beim Schreiben der Assemblersequence ist ein Fehler aufgetreten");
            }

            this.Writer.Close();

            return true;
        }

        public bool AddError(string msg, IParseTreeNode node = null)
        {
            CompilerError error = new CompilerError();
            error.Msg = msg;
            error.Use = node;

            this.Errors.Add(error);

            return false;
        }
    }
}