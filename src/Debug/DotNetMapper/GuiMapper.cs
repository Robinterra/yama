using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using Yama.Compiler;
using Yama.Parser;
using System.Linq;

namespace Yama.Debug
{
    public class GuiMapper : IMapper
    {

        #region vars

        #endregion vars

        #region get/set

        #endregion get/set

        public uint Id
        {
            get;
        } = 7;

        public uint Counter
        {
            get;
            set;
        }

        public List<ObjectIndex> Index
        {
            get;
            set;
        } = new List<ObjectIndex>();

        #region methods

        public bool Execute (Runtime runtime)
        {
            if (runtime.Register[1] == 1) return this.NewForms(runtime);
            if (runtime.Register[1] == 2) return this.ShowForm(runtime);
            if (runtime.Register[1] == 3) return  this.Destroy(runtime);
            if (runtime.Register[1] == 4) return  this.StartMainForm(runtime);
            //if (runtime.Register[1] == 2) return this.WriteData(runtime.Register[3], runtime);
            //if (runtime.Register[1] == 3) return this.ReadObject(runtime);
            //if (runtime.Register[1] == 4) return this.ReadData(runtime);
            //if (runtime.Register[1] == 5) return this.Exist(runtime);

            return true;
        }

        private bool StartMainForm(Runtime runtime)
        {
            ObjectIndex index = this.GetObject(runtime.Register[2]);
            if (index == null) return false;

            System.Threading.Thread t1 = new System.Threading.Thread(this.RunFormNewThread);
            t1.Start(index.Object);

            return true;
        }

        private void RunFormNewThread(object obj)
        {
            if (!(obj is Form t)) return;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(t);

            return;
        }

        private bool Destroy(Runtime runtime)
        {
            ObjectIndex index = this.GetObject(runtime.Register[2]);

            if (index == null) return true;

            if (index.Object is IDisposable t) t.Dispose();

            this.Index.Remove(index);

            return true;
        }

        private bool ShowForm(Runtime runtime)
        {
            ObjectIndex index = this.GetObject(runtime.Register[2]);
            if (index == null) return false;

            if (!(index.Object is Control t)) return false;

            t.Show();

            return true;
        }

        public ObjectIndex GetObject(uint adresse)
        {
            return this.Index.FirstOrDefault(t=>t.Adresse == adresse);
        }

        public uint AddObject(object obj)
        {
            ObjectIndex index = new ObjectIndex();
            this.Counter += 1;
            index.Adresse = this.Counter;
            index.Object = obj;

            this.Index.Add(index);

            return index.Adresse;
        }

        private bool NewForms(Runtime runtime)
        {
            runtime.Register[12] = this.AddObject(new Form());

            return true;
        }

        #endregion methods

    }
}