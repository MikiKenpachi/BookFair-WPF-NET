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
                this.Title = "Izmeni autora"; // Promenimo naslov prozora
            }
            else
            {
                this.Title = "Dodaj novog autora";
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

            // Adresa (pretpostavka da je NoviAutor.Adresa inicijalizovana)
            if (NoviAutor.Adresa != null)
            {
                txtUlica.Text = NoviAutor.Adresa.Ulica;
                txtBroj.Text = NoviAutor.Adresa.Broj;
                txtGrad.Text = NoviAutor.Adresa.Grad;
                txtDrzava.Text = NoviAutor.Adresa.Drzava;
            }

            // Specifična polja za Autora
            // ToString() je neophodan jer su godine iskustva tipa int
            txtIskustvo.Text = NoviAutor.Godine_iskustva.ToString();
            txtBrojLk.Text = NoviAutor.Broj_lk;
        }
        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text))
            {
                MessageBox.Show("Molimo unesite ime i prezime.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Ako NoviPosetilac ne postoji (dodavanje novog), kreiraj novu instancu
            // Ako postoji (izmena), samo ćemo mu ažurirati property-je
            if (NoviAutor == null)
            {
                NoviAutor = new Autor();
                // Ako tvoja klasa Posetilac ima i instancu klase Adresa unutar sebe:
                NoviAutor.Adresa = new Adresa();
            }

            // 3. Mapiranje podataka iz UI kontrola u objekat
            string ime = txtIme.Text;
            string prezime = txtPrezime.Text;
            DateTime datumRodjenja = dpDatum.SelectedDate ?? DateTime.Now;
            string telefon = txtTelefon.Text;
            string email = txtEmail.Text;
            string brojLk = txtBrojLk.Text;

            // 2. Parsiranje godina iskustva (iz stringa u int)
            if (!int.TryParse(txtIskustvo.Text, out int godineIskustva))
            {
                MessageBox.Show("Molimo unesite ispravan broj za godine iskustva.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Kreiranje objekta Adresa
            // Pretpostavljamo da Adresa ima default konstruktor ili setere
            Adresa adr = new Adresa
            {
                Ulica = txtUlica.Text,
                Broj = txtBroj.Text,
                Grad = txtGrad.Text,
                Drzava = txtDrzava.Text
            };

            // 4. Pozivanje tvog konstruktora za Autora
            // Redosled: (ime, prezime, datumRodjenja, adresa, telefon, email, godineIskustva, brojLk)
            NoviAutor = new Autor(
                ime,
                prezime,
                datumRodjenja,
                adr,
                telefon,
                email,
                godineIskustva,
                brojLk
            );

            // 4. Zatvaranje prozora uz potvrdu
            // Postavljanjem DialogResult na true, signaliziramo pozivaocu (MainWindow) da je akcija uspešna
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
