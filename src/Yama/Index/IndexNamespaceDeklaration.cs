using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexNamespaceDeklaration : IParent
    {

        #region get/set

        public IParseTreeNode Use
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }


        public List<IndexKlassenDeklaration> KlassenDeklarationen
        {
            get;
            set;
        }

        public List<IndexNamespaceReference> References
        {
            get;
            set;
        }

        public List<IndexNamespaceReference> Usings
        {
            get;
            set;
        }

        public List<IndexEnumDeklaration> EnumDeklarationen
        {
            get;
            set;
        }

        private ValidUses thisUses;

        public ValidUses ThisUses
        {
            get
            {
                if (this.thisUses != null) return this.thisUses;

                this.thisUses = new ValidUses(this.ParentUsesSet);

                return this.thisUses;
            }
        }

        public List<FileInfo> Files
        {
            get;
            set;
        }

        public ValidUses ParentUsesSet { get;
        set; }

        #endregion get/set

        #region ctor

        public IndexNamespaceDeklaration (  )
        {
            this.References = new List<IndexNamespaceReference>();
            this.KlassenDeklarationen = new List<IndexKlassenDeklaration>();
            this.Usings = new List<IndexNamespaceReference>();
            this.Files = new List<FileInfo>();
            this.EnumDeklarationen = new List<IndexEnumDeklaration>();
        }

        private bool PreviusMappen()
        {
            //this.PreviusUsingsMappen(this.Usings, this.ThisUses);

            return true;
        }

        public bool Mappen(ValidUses rootValidUses)
        {
            //this.ParentUsesSet = rootValidUses;

            //this.PreviusMappen();

            //this.KlassenMappen(this.KlassenDeklarationen, this.ThisUses);

            return true;
        }

        #endregion ctor
    }
}