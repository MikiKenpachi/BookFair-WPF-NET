using Core.Storage.Serialization;
using System;

namespace SajamKnjigaProjekat.Core.Models
{
    /// <summary>
    /// Predstavlja vezu između posjetioca i knjige na listi želja.
    /// Čuva se u zelje.txt, analogno kupovina.txt.
    /// </summary>
    public class Zelja : ISerializable
    {
        public Posetilac Posetilac { get; set; }
        public Knjiga Knjiga { get; set; }

        public Zelja() { }

        public Zelja(Posetilac posetilac, Knjiga knjiga)
        {
            Posetilac = posetilac;
            Knjiga = knjiga;
        }

        public string[] ToCSV() => new string[]
        {
            Posetilac.BrClanskeKarte,
            Knjiga.ISBN
        };

        public void FromCSV(string[] values)
        {
            Posetilac = new Posetilac { BrClanskeKarte = values[0] };
            Knjiga = new Knjiga { ISBN = values[1] };
        }
    }
}