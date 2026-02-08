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
            //izracunati prosecnu ocenu 
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
                Telefon,
                Email,
                BrClanskeKarte,
                GodinaClanstva.ToString(),
                Status.ToString(),
                ProsecnaOcena.ToString()
               
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 9)
                throw new ArgumentException("Invalid CSV data for Posetilac.");

            Ime = values[0];
            Prezime = values[1];
            DatumRodjenja = DateTime.Parse(values[2]);
            Telefon = values[3];
            Email = values[4];
            BrClanskeKarte = values[5];
            GodinaClanstva = int.Parse(values[6]);
            Status = (StatusPosetioca)Enum.Parse(typeof(StatusPosetioca), values[7]);
            ProsecnaOcena = double.Parse(values[8]);
            
        }
    }
}
