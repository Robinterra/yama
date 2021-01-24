using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileContainer
    {
        public CompileSprungPunkt Begin
        {
            get;
            set;
        }

        public CompileSprungPunkt Ende
        {
            get;
            set;
        }

        public List<DataHold> DataHolds
        {
            get;
            set;
        } = new List<DataHold>();

        public string AddDataCall(string jumpPoint, Compiler compiler)
        {
            DataHold dataHold = new DataHold();

            dataHold.JumpPoint = compiler.Definition.GenerateJumpPointName();
            dataHold.DatenValue = jumpPoint;

            this.DataHolds.Add(dataHold);

            return dataHold.JumpPoint;
        }
    }

}