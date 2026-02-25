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

        // Popunjava se kroz DataBinding.PoveziKnjigeIAutore()
        // NE čuva se u autor.txt — knjiga.txt je jedini izvor istine za ovu vezu
        public List<Knjiga> SpisakKnjiga { get; set; } = new List<Knjiga>();

        public string ImePrezime => $"{Ime} {Prezime}";

        public Autor() { }

        public Autor(string ime, string prezime, DateTime datumRodjenja, Adresa adresa,
                     string telefon, string email, int godineIskustva, string brojLk)
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
            if (!SpisakKnjiga.Contains(knjiga))
                SpisakKnjiga.Add(knjiga);
        }

        // ================================================================
        // Serijalizacija
        //
        // DIZAJN ODLUKA — veza Autor↔Knjiga je VIŠE-VIŠE:
        //   - knjiga.txt čuva IDs autora (polje 7, odvojeni sa ;)
        //   - autor.txt NE čuva knjige — DataBinding ih rekonstruiše
        //
        // Tok pri pokretanju:
        //   1. Učitaj autor.txt  → Autor objekti bez SpisakKnjiga
        //   2. Učitaj knjiga.txt → Knjiga objekti sa stub Autor listom (samo Broj_lk)
        //   3. DataBinding.PoveziKnjigeIAutore() → zameni stubove pravim Autor
        //      objektima I popuni autor.SpisakKnjiga
        //
        // Tok pri snimanju:
        //   - knjigaDao.Save() → snima knjiga.txt sa ispravnim autor IDs
        //   - autorDao.Save()  → snima autor.txt BEZ knjiga (nije potrebno)
        // ================================================================

        public string[] ToCSV()
        {
            // 7 polja: Ime | Prezime | Datum | Telefon | Email | Iskustvo | BrojLK
            // SpisakKnjiga se NE čuva — knjiga.txt je jedini izvor istine
            return new string[]
            {
                Ime,
                Prezime,
                Datum_rodjenja.ToString("yyyy-MM-dd"),
                Telefon,
                Email,
                Godine_iskustva.ToString(),
                Broj_lk
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

            // SpisakKnjiga se prazni — DataBinding ga popunjava iz knjiga.txt
            SpisakKnjiga = new List<Knjiga>();
        }
    }
}