using System;
using System.Collections.Generic;
namespace SajamKnjigaProjekat.Core.Models
{
    public class Autor
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime Datum_rodjenja { get; set; }
        public string Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public int Godine_iskustva { get; set; }
        public string Broj_lk { get; set; }
        public List<Knjiga> SpisakKnjiga { get; set; } = new List<Knjiga>();

        public Autor(){ }

        public Autor(string ime, string prezime, DateTime datum_rodjenja, string adresa, string telefon, string email, int godine_iskustva, string broj_lk, List<Knjiga> spisakKnjiga)
        {
            Ime = ime;
            Prezime = prezime;
            Datum_rodjenja = datum_rodjenja;
            Adresa = adresa;
            Telefon = telefon;
            Email = email;
            Godine_iskustva = godine_iskustva;
            Broj_lk = broj_lk;
            SpisakKnjiga = spisakKnjiga;
        }

        public void DodajSpisakKnjiga(Knjiga knjiga)
        {
            if (!SpisakKnjiga.Contains(knjiga))
                SpisakKnjiga.Add(knjiga);
        }


    }

}
