using System.Collections.Generic;

namespace LearnCsStuf.Automaten
{
    public class AutomatenTest
    {
        public bool Run(string input)
        {
            //Alphabet
            MusterToken t0 = "H";
            MusterToken t1 = "e";
            MusterToken t2 = "i";
            List<Token> Alphabet = new List<Token> { t0, t1, t2 };

            // Zustände
            MusterZustand q0 = new MusterZustand();
            MusterZustand q1 = new MusterZustand();
            MusterZustand q2 = new MusterZustand();
            List<Zustand> Q = new List<Zustand> { q0, q1, q2 };
            List<Zustand> F = new List<Zustand> { q2 };

            // Relationen
            MusterRelation r0 = new MusterRelation(q0, q1, t0);
            MusterRelation r1 = new MusterRelation(q1, q1, t1);
            MusterRelation r2 = new MusterRelation(q1, q2, t2);
            List<Relation> relationen = new List<Relation> { r0, r1, r2 };

            Automat automat = new Automat( Q, q0, Alphabet, F, relationen );

            char[] chars = input.ToCharArray();

            foreach (char elem in chars)
            {
                Status status = automat.Next ( (MusterToken)elem.ToString() );
                System.Console.WriteLine("{0}={1}",elem, status);
            }

            return true;
        }
    }
}