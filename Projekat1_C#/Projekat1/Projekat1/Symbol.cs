using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class Symbol
    {
        public char Value { get; private set; }//vrednost 
        public double Probability { get; private set; }//verovatnoca
        public string Code { get; set; }//kod

        public Symbol(char value, double probability)
        {
            this.Value = value;
            this.Probability = probability;
            this.Code = "";
        }

        public char GetValue()
        {
            return Value;
        }

        public string GetCode()
        {
            return Code;
        }

        public double GetProbability()
        {
            return Probability;
        }
    }

}
