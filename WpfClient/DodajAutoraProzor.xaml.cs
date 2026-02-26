using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
    /// Interaction logic for DodajAutoraProzor.xaml
    /// </summary>
    public partial class DodajAutoraProzor : Window
    {

        public Autor? NoviAutor { get; private set; }
        public DodajAutoraProzor(Autor a = null)
        {
            InitializeComponent();

            if (a != null)
            {
                NoviAutor = a;
                PopuniPolja();
                // Dinamički naslov iz resursa za izmenu
                this.Title = Application.Current.FindResource("titleIzmeniAutora").ToString();
            }
            else
            {
                // Dinamički naslov iz resursa za dodavanje
                this.Title = Application.Current.FindResource("titleDodajNovogAutora").ToString();
            }
        }
        private void PopuniPolja()
        {
            if (NoviAutor == null) return;

            // Osnovni podaci
            txtIme.Text = NoviAutor.Ime;
            txtPrezime.Text = NoviAutor.Prezime;

            // Obrati pažnju: u konstruktoru si naveo Datum_rodjenja sa donjom crtom
            dpDatum.SelectedDate = NoviAutor.Datum_rodjenja;

            txtTelefon.Text = NoviAutor.Telefon;
            txtEmail.Text = NoviAutor.Email;

            txtUlica.Text = $"{NoviAutor.Adresa.Ulica}";
            txtBroj.Text = $"{NoviAutor.Adresa.Broj}";
            txtGrad.Text = $"{NoviAutor.Adresa.Grad}";
            txtDrzava.Text = $"{NoviAutor.Adresa.Drzava}";

            // Adresa (pretpostavka da je NoviAutor.Adresa inicijalizovana)
           

            // Specifična polja za Autora
            // ToString() je neophodan jer su godine iskustva tipa int
            txtIskustvo.Text = NoviAutor.Godine_iskustva.ToString();
            txtBrojLk.Text = NoviAutor.Broj_lk;
        }
        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Naslov za MessageBox (koristimo onaj "errorTitle" ili "titleNotice" što smo već napravili)
            string naslovGreska = Application.Current.FindResource("errorTitle").ToString();

            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text))
            {
                string poruka = Application.Current.FindResource("msgUnesiteImePrezime").ToString();
                MessageBox.Show(poruka, naslovGreska, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NoviAutor == null)
            {
                NoviAutor = new Autor();
                NoviAutor.Adresa = new Adresa();
            }

            // Mapiranje osnovnih podataka
            NoviAutor.Ime = txtIme.Text;
            NoviAutor.Prezime = txtPrezime.Text;
            NoviAutor.Datum_rodjenja = dpDatum.SelectedDate ?? DateTime.Now;
            NoviAutor.Telefon = txtTelefon.Text;
            NoviAutor.Email = txtEmail.Text;
            NoviAutor.Broj_lk = txtBrojLk.Text;

            // Provera i parsiranje iskustva
            if (!int.TryParse(txtIskustvo.Text, out int godineIskustva))
            {
                string porukaIskustvo = Application.Current.FindResource("msgIspravnoIskustvo").ToString();
                MessageBox.Show(porukaIskustvo, naslovGreska, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NoviAutor.Godine_iskustva = godineIskustva;

            // Adresa
            NoviAutor.Adresa.Ulica = txtUlica.Text;
            NoviAutor.Adresa.Broj = txtBroj.Text;
            NoviAutor.Adresa.Grad = txtGrad.Text;
            NoviAutor.Adresa.Drzava = txtDrzava.Text;

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
