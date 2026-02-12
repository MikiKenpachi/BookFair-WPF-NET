using Core.Storage.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SajamKnjigaProjekat.Core.Models
{
    public class Autor : ISerializable
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime Datum_rodjenja { get; set; }
        public Adresa Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public int Godine_iskustva { get; set; }
        public string Broj_lk { get; set; }
        public List<Knjiga> SpisakKnjiga { get; set; } = new List<Knjiga>();

        public List<string> SpisakKnjigaISBN { get; set; } = new List<string>();

        public string ImePrezime => $"{Ime} {Prezime}";

        public Autor() { }

        public Autor(string ime, string prezime, DateTime datumRodjenja, Adresa adresa, string telefon, string email, int godineIskustva, string brojLk)
        {
            Ime = ime;
            Prezime = prezime;
            Datum_rodjenja = datumRodjenja;
            Adresa = adresa;
            Telefon = telefon;
            Email = email;
            Godine_iskustva = godineIskustva;
            Broj_lk = brojLk;
            
        }   

        public void DodajSpisakKnjiga(Knjiga knjiga)
        {
            if (SpisakKnjiga == null)
                SpisakKnjiga = new List<Knjiga>();
            SpisakKnjiga.Add(knjiga);
        }

        public string[] ToCSV()
        {
            return new string[]
            {
            Ime,
            Prezime,
            Datum_rodjenja.ToString("yyyy-MM-dd"),
            Telefon,
            Email,
            Godine_iskustva.ToString(),
            Broj_lk,
            SpisakKnjiga != null && SpisakKnjiga.Count > 0
                ? string.Join(";", SpisakKnjiga.ConvertAll(k => k.ISBN))
                : ""
                };
        }

        public void FromCSV(string[] values)
        {
            Ime = values[0];
            Prezime = values[1];
            Datum_rodjenja = DateTime.Parse(values[2]);
            Telefon = values[3];
            Email = values[4];
            Godine_iskustva = int.Parse(values[5]);
            Broj_lk = values[6];

            // Kreiramo praznu listu knjiga, popuniće se u DataBinding
            SpisakKnjiga = new List<Knjiga>();
        }

    }
}
