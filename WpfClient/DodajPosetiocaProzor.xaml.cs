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
            txtUlica.Text = $"{NoviPosetilac.Adresa.Ulica}";
            txtBroj.Text = $"{NoviPosetilac.Adresa.Broj}";
            txtGrad.Text = $"{NoviPosetilac.Adresa.Grad}";
            txtDrzava.Text = $"{NoviPosetilac.Adresa.Drzava}";
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
            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text))
            {
                MessageBox.Show("Molimo unesite ime i prezime.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Ako NoviPosetilac ne postoji (dodavanje novog), kreiraj novu instancu
            // Ako postoji (izmena), samo ćemo mu ažurirati property-je
            if (NoviPosetilac == null)
            {
                NoviPosetilac = new Posetilac();
                // Ako tvoja klasa Posetilac ima i instancu klase Adresa unutar sebe:
                NoviPosetilac.Adresa = new Adresa();
            }

            // 3. Mapiranje podataka iz UI kontrola u objekat
            NoviPosetilac.Ime = txtIme.Text;
            NoviPosetilac.Prezime = txtPrezime.Text;
            NoviPosetilac.DatumRodjenja = dpDatum.SelectedDate ?? DateTime.Now;
            NoviPosetilac.Telefon = txtTelefon.Text;
            NoviPosetilac.Email = txtEmail.Text;

            // Čitanje statusa iz ComboBox-a
            if (cbStatus.SelectedItem is ComboBoxItem selectedItem)
            {
                string sadrzaj = selectedItem.Content.ToString();

                if (sadrzaj == "V.I.P.")
                {
                    NoviPosetilac.Status = StatusPosetioca.V;
                }
                else
                {
                    NoviPosetilac.Status = StatusPosetioca.R;
                }
            }

            // Čitanje razdvojenih polja adrese
            NoviPosetilac.Adresa.Ulica = txtUlica.Text;
            NoviPosetilac.Adresa.Broj = txtBroj.Text;
            NoviPosetilac.Adresa.Grad = txtGrad.Text;
            NoviPosetilac.Adresa.Drzava = txtDrzava.Text;

            // 4. Zatvaranje prozora uz potvrdu
            // Postavljanjem DialogResult na true, signaliziramo pozivaocu (MainWindow) da je akcija uspešna
            this.DialogResult = true;
            this.Close();

        }

        private void txtBrClanske_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
