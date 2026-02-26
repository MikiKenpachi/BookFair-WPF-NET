using Core.DAO;
using Core.DTO;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for DodajIzdavacaProzor.xaml
    /// </summary>

    public partial class DodajIzdavacaProzor : Window
    {
        public IzdavacDTO MojDTo { get; set; }
        public List<Autor> SviAutori { get; set; }

        // 1. DODAJ OVO: Ovo svojstvo omogućava glavnom prozoru da "izvuče" rezultat
        public Izdavac NoviIzdavac { get; private set; }

        public DodajIzdavacaProzor(List<Autor> sviAutori, Izdavac postojeciIzdavac = null)
        {
            InitializeComponent();

            if (postojeciIzdavac != null)
            {
                var autoriKojiRadeZaIzdavaca = postojeciIzdavac.ListaKnjiga
                    .SelectMany(k => k.ListaAutora)
                    .GroupBy(a => a.Broj_lk)
                    .Select(g => g.First())
                    .ToList();

                cbSefovi.ItemsSource = autoriKojiRadeZaIzdavaca;

                var selektovaniSef = autoriKojiRadeZaIzdavaca
                    .FirstOrDefault(a => a.Broj_lk == postojeciIzdavac.SefIzdavaca?.Broj_lk);

                MojDTo = new IzdavacDTO
                {
                    Sifra = postojeciIzdavac.Sifra,
                    Naziv = postojeciIzdavac.Naziv,
                    SefIzdavaca = selektovaniSef,
                    ListaAutora = postojeciIzdavac.ListaAutora,
                    ListaKnjiga = postojeciIzdavac.ListaKnjiga
                };

                // LOKALIZACIJA NASLOVA
                this.Title = Application.Current.FindResource("titleIzmenaIzdavaca").ToString();
                txtSifra.IsEnabled = false;
            }
            else
            {
                cbSefovi.ItemsSource = sviAutori;
                MojDTo = new IzdavacDTO();
                // LOKALIZACIJA NASLOVA
                this.Title = Application.Current.FindResource("titleDodajIzdavaca").ToString();
            }

            this.DataContext = MojDTo;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Validacija preko DTO-a sa lokalizovanim porukama
            if (!string.IsNullOrEmpty(MojDTo["Sifra"]) ||
                !string.IsNullOrEmpty(MojDTo["Naziv"]) ||
                !string.IsNullOrEmpty(MojDTo["SefIzdavaca"]))
            {
                string poruka = Application.Current.FindResource("msgIspraviGreske").ToString();
                string naslov = Application.Current.FindResource("titleValidacija").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NoviIzdavac = new Izdavac
            {
                Sifra = MojDTo.Sifra,
                Naziv = MojDTo.Naziv,
                SefIzdavaca = MojDTo.SefIzdavaca,
                ListaAutora = MojDTo.ListaAutora,
                ListaKnjiga = MojDTo.ListaKnjiga
            };

            this.DialogResult = true;
        }



        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
