using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class UcitaniSimbol
    {
        public string Vrednost { get; set; }
        public string Kod { get; set; }

        public UcitaniSimbol(string vrednost, string kod)
        {
            Vrednost = vrednost;
            Kod = kod;
        }

        public override string ToString()
        {
            return $"Vrednost: {Vrednost} kod: {Kod}";
        }
    }
}
