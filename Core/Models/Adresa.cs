using System;

namespace SajamKnjigaProjekat.Core.Models
{   
    public class Adresa
    {
        public string Ulica { get; set; }
        public string Broj { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }


        public Adresa(){ }

        public Adresa(string ulica, string broj, string grad, string drzava)
        {
            Ulica = ulica;
            Broj = broj;
            Grad = grad;
            Drzava = drzava;
        }
    }
}

