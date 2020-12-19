using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Parser;

namespace Yama.Compiler
{
    public class Compiler
    {

        #region get/set

        public CompileHeader Header
        {
            get;
            set;
        } = new CompileHeader();

        public ContainerManagment CurrentContainer
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

        public List<ICompileRoot> AssemblerSequence
        {
            get;
            set;
        } = new List<ICompileRoot>();
        public MethodeDeclarationNode MainFunction { get; internal set; }
        public List<CompilerError> Errors { get; set; } = new List<CompilerError>();

        public List<string> Defines { get; set; }
        public Optimize OptimizeLevel
        {
            get;
            set;
        }
        public List<ICompileRoot> DataSequence { get; internal set; } = new List<ICompileRoot>();

        #endregion get/set

        #region methods

        public CompileAlgo GetAlgo(string algoName, string mode)
        {
            CompileAlgo result = this.Definition.Algos.FirstOrDefault(a=> a.Name == algoName && a.Mode == mode);

            if (result != null) return result;

            this.AddError(string.Format("Der Algorithmus {0} mit dem Modus {1} konnte nicht gefunden werden!", algoName, mode));

            return null;
        }

        public bool AddLine(RequestAddLine request)
        {
            if (request.Dictionaries != null)
            {
                foreach(KeyValuePair<string, string> pair in request.Dictionaries)
                {
                    request.AssemblyCode = request.AssemblyCode.Replace(pair.Key, pair.Value);
                }
            }

            if (request.PostReplaces != null)
            {
                foreach(KeyValuePair<string, string> pair in request.PostReplaces)
                {
                    request.AssemblyCode = request.AssemblyCode.Replace(pair.Key, pair.Value);
                }
            }

            if (this.Writer != null)
                this.Writer.WriteLine(request.AssemblyCode);

            request.Root.AssemblyCommands.Add(request.AssemblyCode);

            return true;
        }

        public bool Compilen(List<IParseTreeNode> nodes)
        {
            if (this.Definition == null) return this.AddError("Keine definition zur Übersetzung in Assembler gesetzt");
            this.Definition.Compiler = this;

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
            if (this.OutputFile != null)
                if (this.OutputFile.Exists) this.OutputFile.Delete();

            this.AssemblerSequence.AddRange(this.DataSequence);

            try
            {
                if (this.OutputFile != null)
                    this.Writer = new StreamWriter(this.OutputFile.OpenWrite());
            }
            catch (Exception e) { return this.AddError(string.Format("Die Datei in der Assemblercode geschrieben werden sollte konnte nicht angelegt werden. {0}", e.Message)); }

            foreach (ICompileRoot root in this.AssemblerSequence)
            {
                if (!root.InFileCompilen(this)) this.AddError("Beim Schreiben der Assemblersequence ist ein Fehler aufgetreten");
            }

            if (this.OutputFile != null)
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

        public bool SetNewContainer(CompileContainer compileContainer)
        {
            this.CurrentContainer = new ContainerManagment();

            this.CurrentContainer.RootContainer = compileContainer;
            this.CurrentContainer.ContainerStack.Push(compileContainer);

            this.Definition.VariabelCounter = 0;

            return true;
        }

        public bool PushContainer(CompileContainer compileContainer)
        {
            this.CurrentContainer.ContainerStack.Push(compileContainer);

            return true;
        }

        public bool PopContainer()
        {
            this.CurrentContainer.ContainerStack.Pop();

            return true;
        }

        #endregion methods
    }
}