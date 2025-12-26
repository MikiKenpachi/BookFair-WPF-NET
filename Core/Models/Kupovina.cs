using Core.Storage.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Kupovina : ISerializable
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

        public string[] ToCSV() => new string[] {
            Posetilac.BrClanskeKarte,
            Knjiga.ISBN,
            Datum_kupovine.ToString("o"),
            Ocena.ToString(),
            Komentar
        };

        public void FromCSV(string[] values)
        {

            Posetilac = new Posetilac { BrClanskeKarte = values[0] };
            Knjiga = new Knjiga { ISBN = values[1] };
            Datum_kupovine = DateTime.Parse(values[2]);
            Ocena = int.Parse(values[3]);
            Komentar = values[4];
        }
    }
}
