using Core.DTO;
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

        private PosetilacDTO posetilacDTO;
        public DodajPosetiocaProzor(Posetilac p = null)
        {
            InitializeComponent();

            if (p != null)
            {
                NoviPosetilac = p;

                posetilacDTO = new PosetilacDTO
                {
                    Ime = p.Ime,
                    Prezime = p.Prezime,
                    DatumRodjenja = p.DatumRodjenja,
                    Telefon = p.Telefon,
                    Email = p.Email,
                    GodinaClanstva = p.GodinaClanstva.ToString(),
                    Ulica = p.Adresa?.Ulica,
                    Broj = p.Adresa?.Broj,
                    Grad = p.Adresa?.Grad,
                    Drzava = p.Adresa?.Drzava,
                    Status = p.Status
                };

                //PopuniPolja();
                // Lokalizovan naslov za izmenu
                this.Title = Application.Current.FindResource("titleIzmeniPosetioca").ToString();
            }
            else
            {
                // Lokalizovan naslov za dodavanje
                posetilacDTO = new PosetilacDTO(); // Prazan DTO za unos novog posetioca
                this.Title = Application.Current.FindResource("titleDodajPosetioca").ToString();
            }

            this.DataContext = posetilacDTO;
            btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private void PopuniPolja()
        {
            txtIme.Text = NoviPosetilac.Ime;
            txtPrezime.Text = NoviPosetilac.Prezime;
            dpDatum.SelectedDate = NoviPosetilac.DatumRodjenja; // Proveri naziv property-ja
            txtTelefon.Text = NoviPosetilac.Telefon;
            txtEmail.Text = NoviPosetilac.Email;
            txtGodinaClanstva.Text = NoviPosetilac.GodinaClanstva.ToString();
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
            if (NoviPosetilac == null)
            {
                NoviPosetilac = new Posetilac();
                NoviPosetilac.Adresa = new Adresa();
            }

            NoviPosetilac.Ime = posetilacDTO.Ime;
            NoviPosetilac.Prezime = posetilacDTO.Prezime;
            NoviPosetilac.DatumRodjenja = dpDatum.SelectedDate ?? DateTime.Now;
            NoviPosetilac.Telefon = posetilacDTO.Telefon;
            NoviPosetilac.Email = posetilacDTO.Email;
            NoviPosetilac.GodinaClanstva = int.Parse(posetilacDTO.GodinaClanstva);
            NoviPosetilac.Adresa.Ulica = posetilacDTO.Ulica;
            NoviPosetilac.Adresa.Broj = posetilacDTO.Broj;
            NoviPosetilac.Adresa.Grad = posetilacDTO.Grad;
            NoviPosetilac.Adresa.Drzava = posetilacDTO.Drzava;

            if (cbStatus.SelectedItem is ComboBoxItem item && item.Content.ToString() == "V.I.P.")
                NoviPosetilac.Status = StatusPosetioca.V;
            else
                NoviPosetilac.Status = StatusPosetioca.R;

            this.DialogResult = true;
            this.Close();
        }

        private void TxtPolje_Changed(object sender, EventArgs e)
        {

            posetilacDTO.DatumRodjenja = dpDatum.SelectedDate;

            if (btnPotvrdi != null)
                btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private bool FormaJeValidna()
        {
            return string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Ime)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Prezime)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Telefon)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Email)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.GodinaClanstva)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Ulica)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Broj)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Grad)]) &&
                   string.IsNullOrEmpty(posetilacDTO[nameof(posetilacDTO.Drzava)]) &&
                   dpDatum.SelectedDate.HasValue;
        }

    }
}
