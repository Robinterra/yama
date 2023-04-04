using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class InputOutputMapper : IMapper
    {

        #region vars

        private int filemapId;

        #endregion vars

        #region get/set

        #endregion get/set

        public uint Id
        {
            get;
        } = 6;

        public Dictionary<int, FileStream> FileStreamMapper
        {
            get;
            set;
        } = new Dictionary<int, FileStream>();

        #region methods

        private bool Exist(Runtime runtime)
        {
            string path = runtime.GetStringFromRegister(2);

            uint result = 0xff;
            if (!File.Exists(path)) result = 0;

            runtime.Register[12] = result;

            return true;
        }

        public bool OpenStream(Runtime runtime)
        {
            string path = runtime.GetStringFromRegister(2);
            uint flags = runtime.Register[0];
            uint mode = runtime.Register[3];

            FileMode fileMode = this.TranslateLinuxFileFlagsToDotNetFileMode(flags, path);

            FileStream? stream = null;
            try
            {
                stream = flags == 0 ? File.OpenRead(path) : File.Open(path, fileMode);
            } catch { Console.WriteLine("Can not open stream"); runtime.Register[12] = 0; return true; }

            this.filemapId = this.filemapId + 1;

            this.FileStreamMapper.Add(this.filemapId, stream);

            runtime.Register[12] = (uint)this.filemapId;

            return true;
        }

        private FileMode TranslateLinuxFileFlagsToDotNetFileMode(uint flags, string filename)
        {
            FileMode fileMode = FileMode.Open;
            if ((flags & 0x400) == 0x400) fileMode = FileMode.Append;
            if ((flags & 0x40) == 0x40) fileMode = FileMode.OpenOrCreate;
            if ((flags & 0x200) == 0x200)
            {
                if (fileMode == FileMode.OpenOrCreate && !File.Exists(filename)) return fileMode;

                fileMode = FileMode.Truncate;
            }

            return fileMode;
        }

        private bool WriteStream(Runtime runtime)
        {
            int id = (int)runtime.Register[0];
            uint adresse = runtime.Register[3];
            if (id == 0 || !this.FileStreamMapper.ContainsKey(id))
            {
                Console.WriteLine("Can not Write from stream");
                runtime.Register[12] = 0;
                return true;
            }

            uint length = runtime.Register[2];
            byte[] data = new byte[length];

            Array.Copy(runtime.Memory, adresse, data, 0, data.Length);

            FileStream stream = this.FileStreamMapper[id];
            stream.Write(data);

            runtime.Register[12] = 1;

            return true;
        }

        public bool ReadStream(Runtime runtime)
        {
            int id = (int)runtime.Register[0];
            int size = (int)runtime.Register[2];
            int adresse = (int)runtime.Register[3];
            if (id == 0 || !this.FileStreamMapper.ContainsKey(id) || adresse == 0)
            {
                Console.WriteLine("Can not Read from stream");
                runtime.Register[12] = 0;
                return true;
            }

            FileStream stream = this.FileStreamMapper[id];
            int result = stream.Read(runtime.Memory, adresse, size);

            if (result < 0)
            {
                runtime.Register[12] = 0;

                return true;
            }

            /*byte[] target = new byte[result + 4];
            Array.Copy(daten, 0, target, 4, result);
            Array.Copy(BitConverter.GetBytes(result), 0, target, 0, 4);

            if (!runtime.AddObjectToMemory(target, out uint adresse))
            {
                runtime.Register[12] = 0;

                return true;
            }*/

            runtime.Register[12] = (uint)result;

            return true;
        }

        public bool CloseStream(Runtime runtime)
        {
            int id = (int)runtime.Register[2];
            if (id == 0 || !this.FileStreamMapper.ContainsKey(id))
            {
                Console.WriteLine("Can not Close stream");
                runtime.Register[12] = 0;
                return true;
            }

            FileStream stream = this.FileStreamMapper[id];
            stream.Close();
            stream.Dispose();

            this.FileStreamMapper.Remove(id);

            runtime.Register[12] = 0xff;

            return true;
        }

        public bool Execute (Runtime runtime)
        {
            //if (runtime.Register[1] == 1) return this.WriteData(runtime.Register[3] - 4, runtime);
            //if (runtime.Register[1] == 2) return this.WriteData(runtime.Register[3], runtime);
            //if (runtime.Register[1] == 3) return this.ReadObject(runtime);
            //if (runtime.Register[1] == 4) return this.ReadData(runtime);
            if (runtime.Register[1] == 5) return this.Exist(runtime);
            if (runtime.Register[1] == 6) return this.OpenStream(runtime);
            if (runtime.Register[1] == 7) return this.ReadStream(runtime);
            if (runtime.Register[1] == 8) return this.CloseStream(runtime);
            //if (runtime.Register[1] == 9) return this.OpenWriteStream(runtime);
            if (runtime.Register[1] == 10) return this.WriteStream(runtime);
            if (runtime.Register[1] == 11) return this.SeekStream(runtime);

            return true;
        }

        private bool SeekStream(Runtime runtime)
        {
            int id = (int)runtime.Register[0];
            if (id == 0 || !this.FileStreamMapper.ContainsKey(id))
            {
                Console.WriteLine("Can not Close stream");
                runtime.Register[12] = 0;
                return true;
            }

            int seek = (int)runtime.Register[2];
            int seekmode = (int)runtime.Register[3];

            FileStream stream = this.FileStreamMapper[id];
            stream.Seek(seek, (SeekOrigin)seekmode);

            runtime.Register[12] = 0xff;

            return true;
        }

        #endregion methods

    }
}