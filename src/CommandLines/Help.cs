using System;
using System.Collections.Generic;

namespace Yama.CommandLines
{
    public class HelpController
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public List<ICommandLine> CommandLines
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int EmptyPlace
        {get;set;}= 20;

        // -----------------------------------------------

        public static string HilfePattern
        {
            get
            {
                return "{0} {1}<!placeholder!>{2}";
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public HelpController(List<ICommandLine> lines)
        {
            this.CommandLines = lines;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool Print (  )
        {
            Console.WriteLine ( "Compiler for Yama, a Object-oriented Language for Microcontroller like ARM Cortex-M and AVR" );
            Console.WriteLine ( "Examples:" );
            Console.WriteLine ( "yama build skip 0x08000000 out ./bin/out.bin define STM32F401 def arm-t32 inc ./src/" );
            Console.WriteLine ( "yama build out ./out.bin def runtime ./test.yama" );
            Console.WriteLine (  );

            foreach ( ICommandLine line in this.CommandLines )
            {
                if ( line == null ) continue;
                if ( !( line is ICommandParent p )) continue;

               this.PrintLine ( line.HelpLine );

               this.PrintChilds ( p.Childs, "    " );
            }

            Console.WriteLine (  );

            return true;
        }

        // -----------------------------------------------

        private bool PrintChilds(List<ICommandLine> childs, string v)
        {
            if (childs == null) return false;

            foreach (ICommandLine line in childs)
            {
                if ( line == null ) continue;

                Console.Write ( v );

               this.PrintLine ( line.HelpLine );

               if ( line is ICommandParent p ) this.PrintChilds ( p.Childs, v + "    " );
            }

            Console.WriteLine (  );

            return true;
        }

        // -----------------------------------------------

        private bool PrintLine ( string line )
        {
            int pos = line.IndexOf ( "<!placeholder!>" );
            if ( pos < 0 ) return false;
            pos = this.EmptyPlace - pos;
            if ( pos < 0 ) pos = 1;

            char[] leerzeichen = new char[pos];

            for ( int i = 0; i < leerzeichen.Length; i++)
            {
                leerzeichen[i] = ' ';
            }

            line = line.Replace ( "<!placeholder!>", new string(leerzeichen) );

            Console.WriteLine ( line );

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --