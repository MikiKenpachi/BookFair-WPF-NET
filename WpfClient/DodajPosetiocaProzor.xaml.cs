using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for DodajPosetiocaProzor.xaml
    /// </summary>
    public partial class DodajPosetiocaProzor : Window
    {
        public Posetilac? NoviPosetilac { get; private set; }
        public DodajPosetiocaProzor(Posetilac p = null) // Konstruktor prima opcionog posetioca
        {
            InitializeComponent();

            if (p != null)
            {
                NoviPosetilac = p;
                PopuniPolja();
                this.Title = "Izmeni posetioca"; // Promenimo naslov prozora
            }
            else
            {
                this.Title = "Dodaj novog posetioca";
            }
        }

        private void PopuniPolja()
        {
            txtIme.Text = NoviPosetilac.Ime;
            txtPrezime.Text = NoviPosetilac.Prezime;
            dpDatum.SelectedDate = NoviPosetilac.DatumRodjenja; // Proveri naziv property-ja
            txtTelefon.Text = NoviPosetilac.Telefon;
            txtEmail.Text = NoviPosetilac.Email;
            txtAdresa.Text = $"{NoviPosetilac.Adresa.Ulica} {NoviPosetilac.Adresa.Broj}";
            // Postavi i ComboBox status ako je potrebno
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            // Postavlja DialogResult na false (da MainWindow zna da si odustao)
            // i automatski zatvara prozor
            this.DialogResult = false;
            this.Close();
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (NoviPosetilac.Adresa == null)
                NoviPosetilac.Adresa = new Adresa();

            // Veoma jednostavan način da podeliš unos (npr. "Knez Mihailova 10")
            string[] delovi = txtAdresa.Text.Split(' ');
            if (delovi.Length >= 2)
            {
                NoviPosetilac.Adresa.Broj = delovi[delovi.Length - 1]; // Zadnja reč je broj
                NoviPosetilac.Adresa.Ulica = string.Join(" ", delovi.Take(delovi.Length - 1));
            }
            else
            {
                NoviPosetilac.Adresa.Ulica = txtAdresa.Text;
                NoviPosetilac.Adresa.Broj = "bb"; // ili ostavi prazno
            }

            this.DialogResult = true;
        }

        private void txtBrClanske_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
