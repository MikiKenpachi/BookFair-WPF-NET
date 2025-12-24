using System;
using System.Collections.Generic;

namespace SajamKnjigaProjekat.Core.Models
{
    public enum StatusPosetioca { R, V }

    public class Posetilac
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string BrClanskeKarte { get; set; }
        public int GodinaClanstva { get; set; }
        public StatusPosetioca Status { get; set; }
        public double ProsecnaOcena { get; set; }
        public List<Knjiga> ListaKupovina { get; set; } = new List<Knjiga>();
        public List<Knjiga> ListaZelja { get; set; } = new List<Knjiga>();

        public Posetilac() { }

        public Posetilac(string ime, string prezime, DateTime datumRodjenja, string adresa, string telefon, string email, string brClanskeKarte, int godinaClanstva, StatusPosetioca status)
        {
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
            Adresa = adresa;
            Telefon = telefon;
            Email = email;
            BrClanskeKarte = brClanskeKarte;
            GodinaClanstva = godinaClanstva;
            Status = status;
        }

        public void DodajKupovinu(Knjiga knjiga)
        {
            ListaKupovina.Add(knjiga);
        }

        public void DodajNaListuZelja(Knjiga knjiga)
        {
            if (!ListaZelja.Contains(knjiga))
            {
                ListaZelja.Add(knjiga);
            }
        }
    }
}
