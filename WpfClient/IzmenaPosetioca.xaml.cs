using Core.DAO;
using Core.DTO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class IzmenaPosetioca : Window
    {
        public Posetilac SelektovaniPosetilac { get; private set; }
        private readonly KupiliDAO _kupiliDao;
        private ObservableCollection<Kupovina> _kupljene;
        private ObservableCollection<Knjiga> _zelje;
        private readonly List<Knjiga> _sveKnjige;
        private readonly ZeljaDAO _zeljaDao;

        // ── DTO validator — isti koji koristi DodajPosetiocaProzor ──
        private PosetilacDTO _validator = new PosetilacDTO();

        public IzmenaPosetioca(Posetilac posetilac, List<Knjiga> sveKnjige, KupiliDAO kupiliDao, ZeljaDAO zeljaDao)
        {
            InitializeComponent();
            SelektovaniPosetilac = posetilac;
            _sveKnjige = sveKnjige ?? new List<Knjiga>();
            _kupiliDao = kupiliDao;
            _zeljaDao = zeljaDao;

            PopuniPolja();
            UcitajKupljene();
            UcitajZelje();
        }

        private string GetResource(string key) =>
            Application.Current.FindResource(key)?.ToString() ?? key;

        // ================================================================
        // Podaci o posetiocu
        // ================================================================

        private void PopuniPolja()
        {
            txtBrClanske.Text = SelektovaniPosetilac.BrClanskeKarte;
            txtIme.Text = SelektovaniPosetilac.Ime;
            txtPrezime.Text = SelektovaniPosetilac.Prezime;
            dpDatumRodjenja.SelectedDate = SelektovaniPosetilac.DatumRodjenja;
            txtTelefon.Text = SelektovaniPosetilac.Telefon;
            txtEmail.Text = SelektovaniPosetilac.Email;
            txtGodinaClanstva.Text = SelektovaniPosetilac.GodinaClanstva.ToString();
            cmbStatus.SelectedIndex = SelektovaniPosetilac.Status == StatusPosetioca.R ? 0 : 1;

            if (SelektovaniPosetilac.Adresa != null)
            {
                txtUlica.Text = SelektovaniPosetilac.Adresa.Ulica;
                txtBroj.Text = SelektovaniPosetilac.Adresa.Broj;
                txtGrad.Text = SelektovaniPosetilac.Adresa.Grad;
                txtDrzava.Text = SelektovaniPosetilac.Adresa.Drzava;
            }

            // Sinhronizuj validator odmah pri otvaranju pa postavi dugme
            AzurirajValidatorIzForme();
            btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private void AzurirajValidatorIzForme()
        {
            _validator.Ime = txtIme.Text;
            _validator.Prezime = txtPrezime.Text;
            _validator.DatumRodjenja = dpDatumRodjenja.SelectedDate;
            _validator.Telefon = txtTelefon.Text;
            _validator.Email = txtEmail.Text;
            _validator.GodinaClanstva = txtGodinaClanstva.Text;
            _validator.Ulica = txtUlica.Text;
            _validator.Broj = txtBroj.Text;
            _validator.Grad = txtGrad.Text;
            _validator.Drzava = txtDrzava.Text;
        }

        private bool FormaJeValidna()
        {
            return string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Ime)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Prezime)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.DatumRodjenja)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Telefon)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Email)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.GodinaClanstva)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Ulica)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Broj)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Grad)]) &&
                   string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.Drzava)]) &&
                   dpDatumRodjenja.SelectedDate.HasValue;
        }

        // Poziva se na kraju TxtPolje_Changed
        

        private void ObojiNevalidnaPolja()
        {
            ObojiTextBox(txtIme, _validator[nameof(PosetilacDTO.Ime)]);
            ObojiTextBox(txtPrezime, _validator[nameof(PosetilacDTO.Prezime)]);
            ObojiTextBox(txtTelefon, _validator[nameof(PosetilacDTO.Telefon)]);
            ObojiTextBox(txtEmail, _validator[nameof(PosetilacDTO.Email)]);
            ObojiTextBox(txtGodinaClanstva, _validator[nameof(PosetilacDTO.GodinaClanstva)]);
            ObojiTextBox(txtUlica, _validator[nameof(PosetilacDTO.Ulica)]);
            ObojiTextBox(txtBroj, _validator[nameof(PosetilacDTO.Broj)]);
            ObojiTextBox(txtGrad, _validator[nameof(PosetilacDTO.Grad)]);
            ObojiTextBox(txtDrzava, _validator[nameof(PosetilacDTO.Drzava)]);

            // DatePicker
            bool datumNevalidan = !string.IsNullOrEmpty(_validator[nameof(PosetilacDTO.DatumRodjenja)]);
            dpDatumRodjenja.BorderBrush = datumNevalidan
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Gray;
            dpDatumRodjenja.BorderThickness = datumNevalidan
                ? new Thickness(0.5)
                : new Thickness(1);
        }

        private void ObojiTextBox(TextBox tb, string greska)
        {
            bool nevalidno = !string.IsNullOrEmpty(greska);
            tb.BorderBrush = nevalidno
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Gray;
            tb.BorderThickness = new Thickness(nevalidno ? 0.5 : 1);
        }

        private void TxtPolje_Changed(object sender, EventArgs e)
        {
            if (_validator == null) return;
            AzurirajValidatorIzForme();
            ObojiNevalidnaPolja();
            if (btnPotvrdi != null)
                btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            AzurirajValidatorIzForme();

            if (!FormaJeValidna())
            {
                MessageBox.Show(GetResource("msgObaveznaPolja"), GetResource("titleGreska"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelektovaniPosetilac.Ime = _validator.Ime.Trim();
            SelektovaniPosetilac.Prezime = _validator.Prezime.Trim();
            SelektovaniPosetilac.DatumRodjenja = dpDatumRodjenja.SelectedDate ?? SelektovaniPosetilac.DatumRodjenja;
            SelektovaniPosetilac.Telefon = _validator.Telefon.Trim();
            SelektovaniPosetilac.Email = _validator.Email.Trim();
            SelektovaniPosetilac.GodinaClanstva = int.Parse(_validator.GodinaClanstva);
            SelektovaniPosetilac.Status = cmbStatus.SelectedIndex == 0 ? StatusPosetioca.R : StatusPosetioca.V;

            if (SelektovaniPosetilac.Adresa != null)
            {
                SelektovaniPosetilac.Adresa.Ulica = _validator.Ulica.Trim();
                SelektovaniPosetilac.Adresa.Broj = _validator.Broj.Trim();
                SelektovaniPosetilac.Adresa.Grad = _validator.Grad.Trim();
                SelektovaniPosetilac.Adresa.Drzava = _validator.Drzava.Trim();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // ================================================================
        // Tab: Kupljene
        // ================================================================

        private void UcitajKupljene()
        {
            var lista = _kupiliDao.GetByPosetilac(SelektovaniPosetilac.BrClanskeKarte);
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
            string labelaProsek = GetResource("statProsek");
            string labelaPotroseno = GetResource("statPotroseno");

            if (!_kupljene.Any())
            {
                lblProsecnaOcena.Text = $"{labelaProsek} —";
                lblUkupnoPotroseno.Text = $"{labelaPotroseno} — RSD";
                return;
            }

            double prosek = _kupljene.Average(k => k.Ocena);
            double ukupno = _kupljene.Sum(k => {
                if (k.Knjiga != null && double.TryParse(k.Knjiga.Cena, out double c)) return c;
                return 0;
            });

            lblProsecnaOcena.Text = $"{labelaProsek} {prosek:F1}";
            lblUkupnoPotroseno.Text = $"{labelaPotroseno} {ukupno} RSD";

            SelektovaniPosetilac.ProsecnaOcena = prosek;
        }

        private void BtnPonistiKupovinu_Click(object sender, RoutedEventArgs e)
        {
            var odabrane = dgKupljene.SelectedItems.Cast<Kupovina>().ToList();

            if (!odabrane.Any())
            {
                MessageBox.Show(GetResource("msgOdaberiStavku"), GetResource("titleObavestenje"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var potvrda = MessageBox.Show(GetResource("msgPotvrdaPonistiKupovinu"), GetResource("titlePonistavanje"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            foreach (var kupovina in odabrane)
            {
                _kupiliDao.Remove(SelektovaniPosetilac.BrClanskeKarte, kupovina.Knjiga.ISBN);
                SelektovaniPosetilac.ListaKupovina.Remove(kupovina.Knjiga);
                _kupljene.Remove(kupovina);

                if (kupovina.Knjiga != null && !SelektovaniPosetilac.ListaZelja.Any(k => k.ISBN == kupovina.Knjiga.ISBN))
                {
                    SelektovaniPosetilac.DodajNaListuZelja(kupovina.Knjiga);
                    _zelje.Add(kupovina.Knjiga);
                    _zeljaDao.Add(new Zelja(SelektovaniPosetilac, kupovina.Knjiga));
                }
            }

            OsveziStatistiku();
        }

        // ================================================================
        // Tab: Želje
        // ================================================================

        private void UcitajZelje()
        {
            _zelje = new ObservableCollection<Knjiga>(SelektovaniPosetilac.ListaZelja);
            dgZelje.ItemsSource = _zelje;
        }

        private void BtnDodajNaListuZelja_Click(object sender, RoutedEventArgs e)
        {
            var prozor = new DodajNaListuZeljaProzor(SelektovaniPosetilac, _sveKnjige)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.OdabraneKnjige.Any())
            {
                foreach (var knjiga in prozor.OdabraneKnjige)
                {
                    SelektovaniPosetilac.DodajNaListuZelja(knjiga);
                    _zelje.Add(knjiga);
                    _zeljaDao.Add(new Zelja(SelektovaniPosetilac, knjiga));
                }
            }
        }

        private void BtnUkloniSaListeZelja_Click(object sender, RoutedEventArgs e)
        {
            var odabrane = dgZelje.SelectedItems.Cast<Knjiga>().ToList();

            if (!odabrane.Any())
            {
                MessageBox.Show(GetResource("msgOdaberiKnjigu"), GetResource("titleObavestenje"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var potvrda = MessageBox.Show(GetResource("msgPotvrdaUkloniZelju"), GetResource("titleUklanjanje"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            foreach (var knjiga in odabrane)
            {
                SelektovaniPosetilac.ListaZelja.Remove(knjiga);
                _zelje.Remove(knjiga);
                _zeljaDao.Remove(SelektovaniPosetilac.BrClanskeKarte, knjiga.ISBN);
            }
        }

        private void BtnUpisKupovine_Click(object sender, RoutedEventArgs e)
        {
            var odabrane = dgZelje.SelectedItems.Cast<Knjiga>().ToList();

            if (!odabrane.Any())
            {
                MessageBox.Show(GetResource("msgOdaberiKnjigu"), GetResource("titleObavestenje"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var knjiga in odabrane)
            {
                var prozor = new UpisKupovineProzor(SelektovaniPosetilac, knjiga)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                if (prozor.ShowDialog() == true && prozor.NovaKupovina != null)
                {
                    var kupovina = prozor.NovaKupovina;
                    _kupiliDao.Add(kupovina);
                    SelektovaniPosetilac.DodajKupovinu(knjiga);
                    _kupljene.Add(kupovina);
                    _zeljaDao.Remove(SelektovaniPosetilac.BrClanskeKarte, knjiga.ISBN);
                    SelektovaniPosetilac.ListaZelja.Remove(knjiga);
                    _zelje.Remove(knjiga);
                }
            }

            OsveziStatistiku();
        }
    }
}