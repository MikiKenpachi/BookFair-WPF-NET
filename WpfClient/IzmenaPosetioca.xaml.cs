using Core.DAO;
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
    /// Interaction logic for IzmenaPosetioca.xaml
    /// </summary>
    public partial class IzmenaPosetioca : Window
    {
        // ── Selektovani posetilac ────────────────────────────────────────
        public Posetilac SelektovaniPosetilac { get; private set; }

        // ── DAO sloj ────────────────────────────────────────────────────
        private readonly KupiliDAO _kupiliDao = new KupiliDAO();

        // ── Liste za DataGrid-ove ───────────────────────────────────────
        private ObservableCollection<Kupovina> _kupljene;
        private ObservableCollection<Knjiga> _zelje;

        // ── Sve knjige u sistemu (za dijalog dodavanja zelje) ───────────
        private readonly List<Knjiga> _sveKnjige;

        // ─────────────────────────────────────────────────────────────────
        public IzmenaPosetioca(Posetilac posetilac, List<Knjiga> sveKnjige)
        {
            InitializeComponent();
            SelektovaniPosetilac = posetilac;
            _sveKnjige = sveKnjige ?? new List<Knjiga>();

            PopuniPolja();
            UcitajKupljene();
            UcitajZelje();
        }

        // ─────────────────────────────────────────────────────────────────
        // Tab 1 – Informacije
        // ─────────────────────────────────────────────────────────────────

        private void PopuniPolja()
        {
            txtBrClanske.Text = SelektovaniPosetilac.BrClanskeKarte;
            txtIme.Text = SelektovaniPosetilac.Ime;
            txtPrezime.Text = SelektovaniPosetilac.Prezime;
            dpDatumRodjenja.SelectedDate = SelektovaniPosetilac.DatumRodjenja;
            txtTelefon.Text = SelektovaniPosetilac.Telefon;
            txtEmail.Text = SelektovaniPosetilac.Email;

            cmbStatus.SelectedIndex = SelektovaniPosetilac.Status == StatusPosetioca.R ? 0 : 1;

            if (SelektovaniPosetilac.Adresa != null)
            {
                txtUlica.Text = SelektovaniPosetilac.Adresa.Ulica;
                txtBroj.Text = SelektovaniPosetilac.Adresa.Broj;
                txtGrad.Text = SelektovaniPosetilac.Adresa.Grad;
                txtDrzava.Text = SelektovaniPosetilac.Adresa.Drzava;
            }
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIme.Text) ||
                string.IsNullOrWhiteSpace(txtPrezime.Text))
            {
                MessageBox.Show("Ime i prezime su obavezni.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelektovaniPosetilac.Ime = txtIme.Text.Trim();
            SelektovaniPosetilac.Prezime = txtPrezime.Text.Trim();
            SelektovaniPosetilac.DatumRodjenja =
                dpDatumRodjenja.SelectedDate ?? SelektovaniPosetilac.DatumRodjenja;
            SelektovaniPosetilac.Telefon = txtTelefon.Text.Trim();
            SelektovaniPosetilac.Email = txtEmail.Text.Trim();
            SelektovaniPosetilac.Status =
                cmbStatus.SelectedIndex == 0 ? StatusPosetioca.R : StatusPosetioca.V;

            if (SelektovaniPosetilac.Adresa != null)
            {
                SelektovaniPosetilac.Adresa.Ulica = txtUlica.Text.Trim();
                SelektovaniPosetilac.Adresa.Broj = txtBroj.Text.Trim();
                SelektovaniPosetilac.Adresa.Grad = txtGrad.Text.Trim();
                SelektovaniPosetilac.Adresa.Drzava = txtDrzava.Text.Trim();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // ─────────────────────────────────────────────────────────────────
        // Tab 2 – Kupljene
        // ─────────────────────────────────────────────────────────────────

        private void UcitajKupljene()
        {
            // Ucitavamo sve kupovine za ovog posetioca iz DAO
            var lista = _kupiliDao.GetByPosetilac(SelektovaniPosetilac.BrClanskeKarte);

            // Povezujemo Knjiga objekte sa stvarnim objektima iz _sveKnjige
            foreach (var k in lista)
            {
                if (k.Knjiga != null)
                {
                    var prava = _sveKnjige.FirstOrDefault(kn => kn.ISBN == k.Knjiga.ISBN);
                    if (prava != null) k.Knjiga = prava;
                }
            }

            _kupljene = new ObservableCollection<Kupovina>(lista);
            dgKupljene.ItemsSource = _kupljene;

            OsveziStatistiku();
        }

        private void OsveziStatistiku()
        {
            if (!_kupljene.Any())
            {
                lblProsecnaOcena.Text = "Prosečna ocena: —";
                lblUkupnoPotroseno.Text = "Potrošeno: — RSD";
                return;
            }

            double prosek = _kupljene.Average(k => k.Ocena);
            double ukupno = _kupljene.Sum(k =>
            {
                if (k.Knjiga != null && double.TryParse(k.Knjiga.Cena, out double c))
                    return c;
                return 0;
            });

            lblProsecnaOcena.Text = $"Prosečna ocena: {prosek:F1}";
            lblUkupnoPotroseno.Text = $"Potrošeno: {ukupno} RSD";

            // Azuriramo prosecnu ocenu i na samom posetiocu
            SelektovaniPosetilac.ProsecnaOcena = prosek;
        }

        private void BtnPonistiKupovinu_Click(object sender, RoutedEventArgs e)
        {
            if (dgKupljene.SelectedItem is not Kupovina odabrana)
            {
                MessageBox.Show("Najpre odaberite kupovinu iz tabele.", "Obaveštenje",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var potvrda = MessageBox.Show(
                "Da li ste sigurni da želite da poništite kupovinu?",
                "Poništavanje kupovine",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            // 1. Ukloni iz DAO (fajl)
            _kupiliDao.Remove(
                SelektovaniPosetilac.BrClanskeKarte,
                odabrana.Knjiga.ISBN);

            // 2. Ukloni iz liste posetioca u memoriji
            SelektovaniPosetilac.ListaKupovina.Remove(odabrana.Knjiga);

            // 3. Ukloni iz DataGrid-a
            _kupljene.Remove(odabrana);

            // 4. Prebaci na listu zelja (ako vec nije tamo)
            if (odabrana.Knjiga != null &&
                !SelektovaniPosetilac.ListaZelja.Any(k => k.ISBN == odabrana.Knjiga.ISBN))
            {
                SelektovaniPosetilac.DodajNaListuZelja(odabrana.Knjiga);
                _zelje.Add(odabrana.Knjiga);
            }

            OsveziStatistiku();
        }

        // ─────────────────────────────────────────────────────────────────
        // Tab 3 – Zelje
        // ─────────────────────────────────────────────────────────────────

        private void UcitajZelje()
        {
            _zelje = new ObservableCollection<Knjiga>(
                SelektovaniPosetilac.ListaZelja);
            dgZelje.ItemsSource = _zelje;
        }

        // Dugme "Dodaj" — otvori dijalog za izbor knjige
        private void BtnDodajNaListuZelja_Click(object sender, RoutedEventArgs e)
        {
            var prozor = new DodajNaListuZeljaProzor(SelektovaniPosetilac, _sveKnjige)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.OdabranaKnjiga != null)
            {
                var knjiga = prozor.OdabranaKnjiga;
                SelektovaniPosetilac.DodajNaListuZelja(knjiga);
                _zelje.Add(knjiga);
            }
        }

        // Dugme "Obriši" — ukloni sa liste zelja
        private void BtnUkloniSaListeZelja_Click(object sender, RoutedEventArgs e)
        {
            if (dgZelje.SelectedItem is not Knjiga odabrana)
            {
                MessageBox.Show("Najpre odaberite knjigu iz tabele.", "Obaveštenje",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var potvrda = MessageBox.Show(
                "Da li ste sigurni da želite da uklonite knjigu sa liste želja?",
                "Uklanjanje knjige",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            SelektovaniPosetilac.ListaZelja.Remove(odabrana);
            _zelje.Remove(odabrana);
        }

        // Dugme "Kupovina" — upiši kupovinu i premesti iz zelja u kupljene
        private void BtnUpisKupovine_Click(object sender, RoutedEventArgs e)
        {
            if (dgZelje.SelectedItem is not Knjiga odabrana)
            {
                MessageBox.Show("Najpre odaberite knjigu iz tabele.", "Obaveštenje",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var prozor = new UpisKupovineProzor(SelektovaniPosetilac, odabrana)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.NovaKupovina != null)
            {
                var kupovina = prozor.NovaKupovina;

                // 1. Sacuvaj u DAO (fajl)
                _kupiliDao.Add(kupovina);

                // 2. Dodaj u listu kupovina posetioca (u memoriji)
                SelektovaniPosetilac.DodajKupovinu(odabrana);

                // 3. Dodaj u DataGrid kupljenih
                _kupljene.Add(kupovina);

                // 4. Ukloni sa liste zelja
                SelektovaniPosetilac.ListaZelja.Remove(odabrana);
                _zelje.Remove(odabrana);

                OsveziStatistiku();
            }
        }
    }
}
