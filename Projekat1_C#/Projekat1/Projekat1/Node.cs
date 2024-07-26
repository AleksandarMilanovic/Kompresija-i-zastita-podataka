using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class Node
    {
        public double Probability { get; set; }//verovatnoca
        public string Symbol { get; set; }//simbol
        public Node Left { get; set; }//levi potomak
        public Node Right { get; set; }//desni potomak
        public string Code { get; set; }//kod

        public Node(double probability, string symbol, Node left, Node right)
        {
            Probability = probability;
            Symbol = symbol;
            Left = left;
            Right = right;
            Code = string.Empty;
        }

        public override string ToString()
        {
            return $"Simbol: {Symbol} prob: {Probability}\n";
        }
    }

}
