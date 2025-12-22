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
        public double ProsecnaOcenaRec { get; set; }
        public List<Knjiga> ListaKupovina { get; set; }

        public List<Knjiga> ListaZelja {  get; set; }

        public Posetilac()
        {
        }
    }
}
