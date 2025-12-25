using System;
using System.Collections.Generic;
using Core.Storage.Serialization;

namespace SajamKnjigaProjekat.Core.Models
{
    public enum StatusPosetioca { R, V }

    public class Posetilac : ISerializable
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public Adresa Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string BrClanskeKarte { get; set; }
        public int GodinaClanstva { get; set; }
        public StatusPosetioca Status { get; set; }
        public double ProsecnaOcena { get; set; }
        public List<Knjiga> ListaKupovina { get; set; } = new List<Knjiga>();
        public List<Knjiga> ListaZelja { get; set; } = new List<Knjiga>();

        public Posetilac() { }

        public Posetilac(string ime, string prezime, DateTime datumRodjenja, Adresa adresa, string telefon, string email, string brClanskeKarte, int godinaClanstva, StatusPosetioca status)
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


        public string[] ToCSV()
        {
            return new string[]
            {
                Ime,
                Prezime,
                DatumRodjenja.ToString("o"),
                Adresa.Ulica,
                Adresa.Grad,
                Adresa.Broj,
                Adresa.Drzava, 
                Telefon,
                Email,
                BrClanskeKarte,
                GodinaClanstva.ToString(),
                Status.ToString(),
                ProsecnaOcena.ToString()
                // You may want to serialize ListaKupovina and ListaZelja as needed
            };
        }

        public void FromCSV(string[] values)
        {
            if (values.Length < 13)
                throw new ArgumentException("Invalid CSV data for Posetilac.");

            Ime = values[0];
            Prezime = values[1];
            DatumRodjenja = DateTime.Parse(values[2]);
            Adresa = new Adresa(values[3], values[4], values[5], values[6]);
            Telefon = values[7];
            Email = values[8];
            BrClanskeKarte = values[9];
            GodinaClanstva = int.Parse(values[10]);
            Status = (StatusPosetioca)Enum.Parse(typeof(StatusPosetioca), values[11]);
            ProsecnaOcena = double.Parse(values[12]);
            // You may want to deserialize ListaKupovina and ListaZelja as needed
        }
    }
}
