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

        private bool WriteData(uint adresse, Runtime runtime)
        {
            string path = runtime.GetStringFromRegister(2);
            uint length = BitConverter.ToUInt32(runtime.Memory, (int)adresse);
            byte[] data = new byte[length];

            Array.Copy(runtime.Memory, adresse + 4, data, 0, data.Length);
            File.WriteAllBytes(path, data);

            return true;
        }

        private bool ReadData(Runtime runtime)
        {
            string path = runtime.GetStringFromRegister(2);

            byte[] data = File.ReadAllBytes(path);

            byte[] result = new byte[data.Length + 4];

            Array.Copy(data, 0, result, 4, data.Length);
            Array.Copy(BitConverter.GetBytes(data.Length), 0, result, 0, 4);

            runtime.AddObjectToMemory(result, out uint adresse);

            runtime.Register[12] = adresse;

            return true;
        }

        private bool ReadObject(Runtime runtime)
        {
            string path = runtime.GetStringFromRegister(2);

            byte[] data = File.ReadAllBytes(path);

            runtime.AddObjectToMemory(data, out uint adresse);

            runtime.Register[12] = adresse;

            return true;
        }
        private bool Exist(Runtime runtime)
        {
            string path = runtime.GetStringFromRegister(2);

            uint result = 0xff;
            if (!File.Exists(path)) result = 0;

            runtime.Register[12] = result;

            return true;
        }

        public bool OpenReadStream(Runtime runtime)
        {
            string path = runtime.GetStringFromRegister(2);

            FileStream stream = null;
            try
            {
                stream = File.OpenRead(path);
            } catch { Console.WriteLine("Can not open Read stream"); runtime.Register[12] = 0; return true; }

            this.filemapId = this.filemapId + 1;

            this.FileStreamMapper.Add(this.filemapId, stream);

            runtime.Register[12] = (uint)this.filemapId;

            return true;
        }

        public bool ReadStream(Runtime runtime)
        {
            int id = (int)runtime.Register[2];
            int size = (int)runtime.Register[3];
            if (id == 0 || !this.FileStreamMapper.ContainsKey(id))
            {
                Console.WriteLine("Can not Read from stream");
                runtime.Register[12] = 0;
                return true;
            }
            byte[] daten = new byte[size];

            FileStream stream = this.FileStreamMapper[id];
            int result = stream.Read(daten);

            if (result == 0)
            {
                runtime.Register[12] = 0;

                return true;
            }

            byte[] target = new byte[result + 4];
            Array.Copy(daten, 0, target, 4, result);
            Array.Copy(BitConverter.GetBytes(result), 0, target, 0, 4);

            if (!runtime.AddObjectToMemory(target, out uint adresse))
            {
                runtime.Register[12] = 0;

                return true;
            }

            runtime.Register[12] = adresse;

            return true;
        }

        public bool CloseReadStream(Runtime runtime)
        {
            int id = (int)runtime.Register[2];
            if (id == 0 || !this.FileStreamMapper.ContainsKey(id))
            {
                Console.WriteLine("Can not Read from stream");
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
            if (runtime.Register[1] == 1) return this.WriteData(runtime.Register[3] - 4, runtime);
            if (runtime.Register[1] == 2) return this.WriteData(runtime.Register[3], runtime);
            if (runtime.Register[1] == 3) return this.ReadObject(runtime);
            if (runtime.Register[1] == 4) return this.ReadData(runtime);
            if (runtime.Register[1] == 5) return this.Exist(runtime);
            if (runtime.Register[1] == 6) return this.OpenReadStream(runtime);
            if (runtime.Register[1] == 7) return this.ReadStream(runtime);
            if (runtime.Register[1] == 8) return this.CloseReadStream(runtime);

            return true;
        }

        #endregion methods

    }
}