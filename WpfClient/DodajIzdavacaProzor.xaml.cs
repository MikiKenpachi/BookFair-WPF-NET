using Core.DAO;
using Core.DTO;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
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
        public IzdavacDTO izdavacDTO { get; set; }
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

                izdavacDTO = new IzdavacDTO
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
                izdavacDTO = new IzdavacDTO();
                // LOKALIZACIJA NASLOVA
                this.Title = Application.Current.FindResource("titleDodajIzdavaca").ToString();
            }

            this.DataContext = izdavacDTO;
            btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private void TxtPolje_Changed(object sender, EventArgs e)
        {
            if (btnPotvrdi != null)
                btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private bool FormaJeValidna()
        {
            return ValidacijaSifreIzdavaca(txtSifra.Text.Trim()) &&
                   ValidacijaNazivaIzdavaca(txtNaziv.Text.Trim()) &&
                   cbSefovi.SelectedItem != null &&
                   (cbSefovi.SelectedItem as Autor)?.Godine_iskustva >= 5;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Validacija preko DTO-a sa lokalizovanim porukama
            if (!string.IsNullOrEmpty(izdavacDTO["Sifra"]) ||
                !string.IsNullOrEmpty(izdavacDTO["Naziv"]) ||
                !string.IsNullOrEmpty(izdavacDTO["SefIzdavaca"]))
            {
                string poruka = Application.Current.FindResource("msgIspraviGreske").ToString();
                string naslov = Application.Current.FindResource("titleValidacija").ToString();

                if (!ValidacijaSifreIzdavaca(txtSifra.Text.Trim()))
                {
                    MessageBox.Show(Application.Current.FindResource("msgSifraFormat").ToString(),
                        naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ValidacijaNazivaIzdavaca(txtNaziv.Text.Trim()))
                {
                    MessageBox.Show(Application.Current.FindResource("msgNazivNevalidan").ToString(),
                        naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            NoviIzdavac = new Izdavac
            {
                Sifra = izdavacDTO.Sifra,
                Naziv = izdavacDTO.Naziv,
                SefIzdavaca = izdavacDTO.SefIzdavaca,
                ListaAutora = izdavacDTO.ListaAutora,
                ListaKnjiga = izdavacDTO.ListaKnjiga
            };

            this.DialogResult = true;
        }

        private bool ValidacijaSifreIzdavaca(string sifra)
        {
            return Regex.IsMatch(sifra, @"^\d{5}$");
        }

        private bool ValidacijaNazivaIzdavaca(string naziv)
        {
            return !string.IsNullOrWhiteSpace(naziv) && naziv.Length >= 3 && naziv.Length <= 30;
        }


        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
