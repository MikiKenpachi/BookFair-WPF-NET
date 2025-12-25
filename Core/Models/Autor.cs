using System;
using System.Collections.Generic;
using Core.Storage.Serialization;
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
            Adresa.Ulica,
            Adresa.Grad,
            Adresa.Broj,
            Adresa.Drzava,
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
            if (values == null || values.Length < 12)
                throw new ArgumentException("Invalid CSV data for Autor.");

            Ime = values[0];
            Prezime = values[1];
            Datum_rodjenja = DateTime.Parse(values[2]);
            Adresa =new Adresa(values[3], values[4], values[5], values[6]);
            Telefon = values[7];
            Email = values[8];
            Godine_iskustva = int.Parse(values[9]);
            Broj_lk = values[10];

            SpisakKnjiga = new List<Knjiga>();
            if (!string.IsNullOrEmpty(values[11]))
            {
                var isbnList = values[11].Split(';');
                foreach (var isbn in isbnList)
                {
                    
                }
            }
        }
    }
}
