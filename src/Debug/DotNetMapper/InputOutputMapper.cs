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

        #endregion vars

        #region get/set

        #endregion get/set

        public uint Id
        {
            get;
        } = 6;

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

        public bool Execute (Runtime runtime)
        {
            if (runtime.Register[1] == 1) return this.WriteData(runtime.Register[3] - 4, runtime);
            if (runtime.Register[1] == 2) return this.WriteData(runtime.Register[3], runtime);
            if (runtime.Register[1] == 3) return this.ReadObject(runtime);
            if (runtime.Register[1] == 4) return this.ReadData(runtime);
            if (runtime.Register[1] == 5) return this.Exist(runtime);

            return true;
        }

        #endregion methods

    }
}