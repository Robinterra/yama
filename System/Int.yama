namespace "System"
{
    /**
    * Integer current 16 Bit
    */
    public class Int
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region operator

        // -----------------------------------------------

        #region compare

        // -----------------------------------------------

        /**
        * Vergleich 2 Werte miteinander auf kleiner als
        *
        * @param[in] a (Int) Der Linke Child der vom Operator steht
        * @param[in] b (Int) Der Rechte Child der vom Operator steht
        *
        * @return (Bool) Wenn a kleiner ist dann true ansonsten false
        */
        public operator Bool <(Int a, Int b)
        {

        }

        // -----------------------------------------------

        /**
        * Vergleich 2 Werte miteinander auf gleich
        *
        * @param[in] a (Int) Der Linke Child der vom Operator steht
        * @param[in] b (Int) Der Rechte Child der vom Operator steht
        *
        * @return (Bool) Wenn a und b gleich ist dann true ansonsten false
        */
        public operator Bool ==(Int a, Int b)
        {

        }

        // -----------------------------------------------

        #endregion compare

        // -----------------------------------------------

        #region arimetics

        // -----------------------------------------------

        /**
        * Subtrariere
        *
        * @param[in] a (Int) Der Linke Child der vom Operator steht
        * @param[in] b (Int) Der Rechte Child der vom Operator steht
        *
        * @return (Int) Das Ergebenis der Rechnung
        */
        public operator Int -(Int a, Int b)
        {
            Int result;

            #region asm

            // Lade b ins register
            ldd 24,Y+3
            ldd 25,Y+4
            movw r2,r24
            // Lade a ins register
            ldd r24,Y+1
            ldd r25,Y+2

            // subtrahiere a und b
            sub r24,r2
            sbc r25,r3

            //speichere in result
            std Y+5,r24 ; // lege in result ab
            std Y+6,r25 ; // lege in result ab

            #endregion asm

            return result;
        }

        // -----------------------------------------------

        /**
        * Incrementiere um 1
        *
        * @param[in] a (Int) Der Linke oder Rechte Child der vom Operator steht
        *
        * @return (Int) Das Ergebenis der Rechnung
        */
        public operator Int ++(Int a)
        {
            #region asm

            // Lade a ins register
            ldd r24,Y+1
            ldd r25,Y+2

            // increment a
            inc r24
            adc r25, r0

            //speichere in a zurück
            std Y+1,r24
            std Y+2,r25

            #endregion asm

            return a;
        }

        // -----------------------------------------------

        /**
        * Addiere
        *
        * @param[in] a (Int) Der Linke Child der vom Operator steht
        * @param[in] b (Int) Der Rechte Child der vom Operator steht
        *
        * @return (Int) Das Ergebenis der Rechnung
        */
        public operator Int +(Int a, Int b)
        {
            Int result;

            #region asm

            // Lade a ins register
            ldd r24,Y+1
            ldd r25,Y+2
            movw r2,r24

            // Lade b ins register
            ldd r24,Y+3
            ldd r25,Y+4

            // addiere a und b
            add r24,r2
            adc r25,r3

            //speichere in result
            std Y+5,r24
            std Y+6,r25

            #endregion asm

            return result;
        }

        // -----------------------------------------------

        #endregion arimetics

        // -----------------------------------------------

        #endregion operator

        // -----------------------------------------------

    }
}

// --[EOF]--