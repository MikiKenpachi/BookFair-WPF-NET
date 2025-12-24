using System;

namespace SajamKnjigaProjekat.Core.Models
{   
    public class Adresa
    {
        public string Ulica;
        public string Broj;
        public string Grad;
        public string Drzava;


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

