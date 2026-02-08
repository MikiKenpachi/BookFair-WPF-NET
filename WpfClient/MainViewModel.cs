using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WpfClient
{
    public class MainViewModel
    {
        public ObservableCollection<Posetilac> Posetioci { get; set; }

        public MainViewModel()
        {
            // Inicijalizacija posetilaca sa primerima
            Posetioci = new ObservableCollection<Posetilac>
        {
            new Posetilac("Marko", "Marković", new DateTime(1990, 5, 15), new Adresa { Ulica = "Bulevar", Grad = "Beograd", Broj = "11000" }, "0641234567", "marko@mail.com", "123456", 5, StatusPosetioca.R),
            new Posetilac("Ana", "Anić", new DateTime(1985, 3, 22), new Adresa { Ulica = "Kneza Miloša", Grad = "Niš", Broj = "18000" }, "0637654321", "ana@mail.com", "654321", 3, StatusPosetioca.V)
        };
        }
    }
}
