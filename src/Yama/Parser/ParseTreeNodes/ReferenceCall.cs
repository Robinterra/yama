using System;
using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;

namespace Yama.Parser
{
    public class ReferenceCall : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                return result;
            }
        }

        public int Prio
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public ReferenceCall (  )
        {

        }

        public ReferenceCall ( int prio )
        {
            this.Prio = prio;
        }
        #endregion ctor

        #region methods

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( !this.CheckValidTokens ( token ) ) return null;

            ReferenceCall result = new ReferenceCall (  );

            token.Node = result;

            result.Token = token;

            return result;
        }

        private bool CheckValidTokens(IdentifierToken token)
        {
            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.This) return true;
            if (token.Kind == IdentifierKind.Base) return true;

            return false;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (parent is IndexVariabelnReference varref) return this.RefComb(varref);
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            container.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        private bool RefComb(IndexVariabelnReference varref)
        {
            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            string moderesult = "default";
            if (mode == "set") moderesult = mode;
            if (mode == "point") moderesult = mode;
            if (mode == "setpoint") moderesult = mode;
            if (this.Reference.Deklaration is IndexMethodDeklaration dek)
            {
                if (dek.Klasse.IsMethodsReferenceMode && dek.Klasse.Methods.Contains(dek)) return this.RefNameCall(compiler, dek);
                moderesult = "methode";
            }
            if (this.Reference.Deklaration is IndexEnumEntryDeklaration ed) return this.CompileEnumEntry(compiler, ed);
            if (this.Reference.Deklaration is IndexEnumDeklaration) return true;
            if (this.Reference.Deklaration is IndexVektorDeklaration) moderesult = mode;

            if (this.Reference.Deklaration is IndexPropertyGetSetDeklaration)
            {
                moderesult = "point";
                if (mode == "setvektorcall") moderesult = "setpoint";
            }

            compiler.LastVariableCall = this.Token.Text;

            CompileReferenceCall compileReference = new CompileReferenceCall();

            if (this.CheckAndCompileBase(compiler, compileReference, moderesult)) return true;

            return compileReference.Compile(compiler, this, moderesult);
        }

        private bool CheckAndCompileBase(Compiler.Compiler compiler, CompileReferenceCall compileReference, string moderesult)
        {
            if (compiler.LastVariableCall != "base") return false;
            if (!(this.Reference.Deklaration is IndexVariabelnDeklaration t)) return false;
            if (!(t.Type.Deklaration is IndexKlassenDeklaration u)) return false;
            if (!u.IsMethodsReferenceMode) return false;


            return compileReference.CompileDek(compiler, compiler.CurrentThis, moderesult);
        }

        private bool RefNameCall(Compiler.Compiler compiler, IndexMethodDeklaration dek)
        {
            CompileReferenceCall compileReference = new CompileReferenceCall();

            if (compiler.LastVariableCall != "base") compileReference.CompilePoint0(compiler);
            else compileReference.CompileDek(compiler, compiler.CurrentBase, "default");

            compileReference = new CompileReferenceCall();

            return compileReference.Compile(compiler, this, "funcref");
        }

        private bool CompileEnumEntry(Compiler.Compiler compiler, IndexEnumEntryDeklaration dek)
        {
            CompileNumConst constNum = new CompileNumConst();

            constNum.Compile(compiler, new Number { Token = dek.Value });

            return true;
        }

        #endregion methods

    }
}