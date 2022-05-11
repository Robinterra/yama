using System;
using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;

namespace Yama.Parser
{
    public class ReferenceCall : IParseTreeNode, IIndexNode, ICompileNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference? Reference
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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ReferenceCall (  )
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        public ReferenceCall ( int prio ) : this()
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( !this.CheckValidTokens ( request.Token ) ) return null;

            ReferenceCall result = new ReferenceCall (  );

            result.Token = request.Token;
            result.AllTokens.Add(request.Token);

            return result;
        }

        private bool CheckValidTokens(IdentifierToken token)
        {
            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.This) return true;
            if (token.Kind == IdentifierKind.Base) return true;

            return false;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is IndexVariabelnReference varref) return this.RefComb(varref);
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference(this, this.Token.Text);
            container.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        private bool RefComb(IndexVariabelnReference varref)
        {
            IndexVariabelnReference reference = new IndexVariabelnReference(this, this.Token.Text);
            reference.RefCombination = varref;
            varref.IsPointIdentifier = true;
            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Reference is null) return false;

            string moderesult = "default";
            if (request.Mode == "set") moderesult = request.Mode;
            if (request.Mode == "point") moderesult = request.Mode;
            if (request.Mode == "setpoint") moderesult = request.Mode;
            if (this.Reference.Deklaration is IMethode dek)
            {
                if (dek.Klasse is null) return false;

                if (dek.Klasse.IsMethodsReferenceMode && dek.Klasse.Methods.Contains(dek)) return this.RefNameCall(request.Compiler, dek, request.Mode);
                moderesult = "methode";
            }
            if (this.Reference.Deklaration is IndexEnumEntryDeklaration ed) return this.CompileEnumEntry(request.Compiler, ed);
            if (this.Reference.Deklaration is IndexEnumDeklaration) return true;
            if (this.Reference.Deklaration is IndexVektorDeklaration) moderesult = request.Mode;

            if (this.Reference.Deklaration is IndexPropertyGetSetDeklaration)
            {
                moderesult = "point";
                if (request.Mode == "setvektorcall") moderesult = "setpoint";
            }

            if (this.Reference.Deklaration is IndexPropertyDeklaration k)
            {
                if (k.Zusatz == MethodeType.Static)
                {
                    CompileReferenceCall refCall = new CompileReferenceCall();

                    refCall.Compile(request.Compiler, this, "methode");
                }
            }

            request.Compiler.LastVariableCall = this.Token.Text;

            CompileReferenceCall compileReference = new CompileReferenceCall();

            if (this.CheckAndCompileBase(request.Compiler, compileReference, moderesult)) return true;

            if ( request.Mode == "nullChecking" ) compileReference.IsNullCheck = true;
            return compileReference.Compile(request.Compiler, this, moderesult);
        }

        private bool CheckAndCompileBase(Compiler.Compiler compiler, CompileReferenceCall compileReference, string moderesult)
        {
            if (this.Reference is null) return false;

            if (compiler.LastVariableCall != "base") return false;
            if (this.Reference.Deklaration is not IndexVariabelnDeklaration t) return false;
            if (t.Type.Deklaration is not IndexKlassenDeklaration u) return false;
            if (!u.IsMethodsReferenceMode) return false;
            if (compiler.CurrentThis is null) return false;

            return compileReference.CompileDek(compiler, compiler.CurrentThis, moderesult);
        }

        private bool RefNameCall(Compiler.Compiler compiler, IMethode dek, string mode)
        {
            CompileReferenceCall compileReference = new CompileReferenceCall();

            if (compiler.LastVariableCall != "base") compileReference.CompilePoint0(compiler);
            else
            {
                if (compiler.CurrentBase is null) return false;
                compileReference.CompileDek(compiler, compiler.CurrentBase, "default");
            }

            compileReference = new CompileReferenceCall();

            string resultMode = "funcref";
            if (mode == "setvektorcall") resultMode = "setref";
            if (mode == "set") resultMode = "setref";
            if (mode == "setpoint") resultMode = "setref";

            return compileReference.Compile(compiler, this, resultMode);
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