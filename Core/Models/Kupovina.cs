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

        public Kupovina()
        {
            
        }
    }
}
