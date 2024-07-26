using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class LZ77Tuple
    {
        public int Bit { get; set; }//0 ili 1 (oznacava da li postoji ponavljanje)
        public char? Karakter { get; set; }//nullable char za slučaj kada je Karakter null
        public int Move { get; set; }//pre koliko simbola je pocelo ponavljanje
        public int Length { get; set; }//duzina ponavljanja

        public LZ77Tuple(int bit, char karakter)
        {
            Bit = bit;
            Karakter = karakter;
        }

        public LZ77Tuple(int bit, int move, int length)
        {
            Bit = bit;
            Move = move;
            Length = length;
        }

        public override string ToString()
        {
            if (Bit == 1)
            {
                return $"(1, {Move}, {Length}) ";
            }
            else
            {
                return $"(0, {Karakter}) ";
            }
        }
    }

}
