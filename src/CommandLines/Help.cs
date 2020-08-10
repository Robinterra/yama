using System;
using System.Collections.Generic;

namespace LearnCsStuf.CommandLines
{
    public class Help
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

        #region methods

        // -----------------------------------------------

        public bool Print (  )
        {
            Console.WriteLine ( "Compiler for Yama, a Object-oriented Language for Microcontroller like ARM Cortex-M and AVR" );
            Console.WriteLine ( "yama define atmega328p out out.S def avr inc bin/Debug/netcoreapp3.1/atmega328p bin/Debug/netcoreapp3.1/iftest.y" );
            Console.WriteLine (  );

            foreach ( ICommandLine line in this.CommandLines )
            {
                if ( line == null ) continue;

               this.PrintLine ( line.HelpLine );
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