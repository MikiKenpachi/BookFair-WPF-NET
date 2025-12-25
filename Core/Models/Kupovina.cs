using System;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Kupovina
    {
        public Posetilac Posetilac { get; set; }
        public Knjiga Knjiga { get; set; }
        public DateTime Datum_kupovine { get; set; }
        public int Ocena { get; set; }
        public string Komentar { get; set; }

        public Kupovina() { }

        public Kupovina(Posetilac posetilac, Knjiga knjiga, DateTime datum_kupovine, int ocena, string komentar)
        {
            Posetilac = posetilac;
            Knjiga = knjiga;
            Datum_kupovine = datum_kupovine;
            Ocena = ocena;
            Komentar = komentar;
        }
    }
}
