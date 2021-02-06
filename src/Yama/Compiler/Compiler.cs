using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Index;
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

        public ContainerManagment ContainerMgmt
        {
            get;
            set;
        } = new ContainerManagment();

        public List<ICompileRoot> toRemove = new List<ICompileRoot>();

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
        public string LastVariableCall { get; internal set; }
        public IndexVariabelnDeklaration CurrentThis { get; internal set; }
        public IndexVariabelnDeklaration CurrentBase { get; internal set; }
        public List<CompileContainer> Containers { get; private set; } = new List<CompileContainer>();

        public List<SSACompileLine> SSALines
        {
            get;
            set;
        } = new List<SSACompileLine>();

        #endregion get/set

        #region methods

        public CompileAlgo GetAlgo(string algoName, string mode)
        {
            CompileAlgo result = this.Definition.Algos.FirstOrDefault(a=> a.Name == algoName && a.Mode == mode);

            if (result != null) return result;

            this.AddError(string.Format("Der Algorithmus {0} mit dem Modus {1} konnte nicht gefunden werden!", algoName, mode));

            return null;
        }

        public SSACompileLine AddSSALine(SSACompileLine line)
        {
            if (!line.Algo.CanBeDominatet)
            {
                //this.ContainerMgmt.CurrentContainer.Lines.Add(line);
                this.SSALines.Add(line);
                this.ContainerMgmt.CurrentMethod.Lines.Add(line);

                return line;
            }

            foreach (SSACompileLine existLine in this.ContainerMgmt.CurrentContainer.Lines)
            {
                if (!existLine.Algo.Equals(line.Algo)) continue;
                if (existLine.Arguments.Count != line.Arguments.Count) continue;
                if (!this.CheckArgumentsEqual(existLine, line)) continue;

                return existLine;
            }

            this.ContainerMgmt.CurrentMethod.Lines.Add(line);
            this.SSALines.Add(line);

            return line;
        }

        private bool CheckArgumentsEqual(SSACompileLine existLine, SSACompileLine line)
        {
            int count = 0;
            foreach (SSACompileArgument argNew in line.Arguments)
            {
                SSACompileArgument orgArg = existLine.Arguments[count];

                if (argNew.Mode != orgArg.Mode) return false;
                if (argNew.Mode == SSACompileArgumentMode.Reference)
                    if (!argNew.Reference.Equals(orgArg.Reference)) return false;
            }

            return true;
        }

        public bool BeginNewMethode(List<string> registerInUse, CompileContainer compileContainer, ValidUses uses)
        {
            this.Definition.BeginNewMethode(registerInUse);
            this.ContainerMgmt.AddNewMethode(compileContainer);
            this.SetNewContainer(compileContainer);
            compileContainer.RegistersUses = registerInUse;

            foreach (IParent parent in uses.Deklarationen)
            {
                if (!(parent is IndexVariabelnDeklaration dek)) continue;

                SSAVariableMap map = new SSAVariableMap();
                map.Key = dek.Name;
                map.Deklaration = dek;

                this.ContainerMgmt.CurrentMethod.VarMapper.Add(dek.Name, map);
            }

            return true;
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

            this.SetNewContainer(new CompileContainer());
            this.ContainerMgmt.CurrentMethod = this.ContainerMgmt.CurrentContainer;

            this.Header.Compile(this, this.MainFunction);

            foreach (IParseTreeNode node in nodes)
            {
                if (!node.Compile(this)) this.AddError("Beim assembelieren ist in einer Klasse ein Fehler aufgetreten", node);
            }

            if (this.Errors.Count != 0) return false;

            this.DoAllocate();

            if (this.Errors.Count != 0) return false;

            this.RunAssmblerSequence();

            return this.Errors.Count == 0;
        }

        private bool DoAllocate()
        {
            foreach (CompileContainer methode in this.ContainerMgmt.Methods)
            {
                methode.DoAllocate(this);
            }

            return true;
        }

        private bool RunAssmblerSequence()
        {
            if (this.OutputFile != null)
                if (this.OutputFile.Exists) this.OutputFile.Delete();

            //this.AssemblerSequence.AddRange(this.DataSequence);

            try
            {
                if (this.OutputFile != null)
                    this.Writer = new StreamWriter(this.OutputFile.OpenWrite());
            }
            catch (Exception e) { return this.AddError(string.Format("Die Datei in der Assemblercode geschrieben werden sollte konnte nicht angelegt werden. {0}", e.Message)); }

            foreach (SSACompileLine line in this.SSALines)
            {
                if (!line.Owner.InFileCompilen(this)) this.AddError("Beim Schreiben der Assemblersequence ist ein Fehler aufgetreten");
            }

            foreach (ICompileRoot root in this.DataSequence)
            {
                if (!root.InFileCompilen(this)) this.AddError("Beim Schreiben der Assemblersequence ist ein Fehler aufgetreten");
            }

            foreach (ICompileRoot root in this.toRemove )
            {
                this.AssemblerSequence.Remove(root);
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
            this.ContainerMgmt.RootContainer = compileContainer;
            this.PushContainer(compileContainer);

            this.Definition.VariabelCounter = 0;

            return true;
        }

        public bool PushContainer(CompileContainer compileContainer)
        {
            this.ContainerMgmt.ContainerStack.Push(compileContainer);

            this.Containers.Add(compileContainer);

            return true;
        }

        public bool PopContainer()
        {
            CompileContainer container = this.ContainerMgmt.ContainerStack.Pop();

            if (this.ContainerMgmt.CurrentContainer == null) return true;

            this.ContainerMgmt.CurrentContainer.DataHolds.AddRange(container.DataHolds);

            return true;
        }

        #endregion methods
    }
}