using System;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Autor
    {




        public string Ime;
        public string Prezime;
        public string Datum_rodjenja;
        public string Adresa;
        public string Telefon;
        public string Email;
        public int Godine_iskustva;
        public string Broj_lk;

        public Autor()
        {

        }

        public Autor(string ime, string prezime, string datum_rodjenja, string adresa, string telefon, string email, string broj_lk, int godine_iskustva)
        {
            Ime = ime;
            Prezime = prezime;
            Datum_rodjenja = datum_rodjenja;
            Adresa = adresa;
            Telefon = telefon;
            Email = email;
            Broj_lk = broj_lk;
            Godine_iskustva = godine_iskustva;
        }
    }

}
