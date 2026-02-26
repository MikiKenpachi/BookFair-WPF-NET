using Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; // Ne zaboravi Linq za Any, Sum, Average
using System.Windows;

namespace WpfClient
{
    public partial class IzmenaPosetioca : Window
    {
        public Posetilac SelektovaniPosetilac { get; private set; }
        private readonly KupiliDAO _kupiliDao;
        private ObservableCollection<Kupovina> _kupljene;
        private ObservableCollection<Knjiga> _zelje;
        private readonly List<Knjiga> _sveKnjige;

        public IzmenaPosetioca(Posetilac posetilac, List<Knjiga> sveKnjige, KupiliDAO kupiliDao)
        {
            InitializeComponent();
            SelektovaniPosetilac = posetilac;
            _sveKnjige = sveKnjige ?? new List<Knjiga>();
            _kupiliDao = kupiliDao;

            PopuniPolja();
            UcitajKupljene();
            UcitajZelje();
        }

        // --- POMOĆNA METODA ZA RAD SA RESURSIMA ---
        private string GetResource(string key) => Application.Current.FindResource(key)?.ToString() ?? key;

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
            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text))
            {
                // Lokalizovana poruka o grešci
                MessageBox.Show(GetResource("msgObaveznaPolja"), GetResource("titleGreska"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelektovaniPosetilac.Ime = txtIme.Text.Trim();
            SelektovaniPosetilac.Prezime = txtPrezime.Text.Trim();
            SelektovaniPosetilac.DatumRodjenja = dpDatumRodjenja.SelectedDate ?? SelektovaniPosetilac.DatumRodjenja;
            SelektovaniPosetilac.Telefon = txtTelefon.Text.Trim();
            SelektovaniPosetilac.Email = txtEmail.Text.Trim();
            SelektovaniPosetilac.Status = cmbStatus.SelectedIndex == 0 ? StatusPosetioca.R : StatusPosetioca.V;

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

        // --- TAB: KUPLJENE ---

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
            string labelaProsek = GetResource("statProsek"); // "Prosečna ocena: "
            string labelaPotroseno = GetResource("statPotroseno"); // "Potrošeno: "

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
            if (dgKupljene.SelectedItem is not Kupovina odabrana)
            {
                MessageBox.Show(GetResource("msgOdaberiStavku"), GetResource("titleObavestenje"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var potvrda = MessageBox.Show(GetResource("msgPotvrdaPonistiKupovinu"), GetResource("titlePonistavanje"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            _kupiliDao.Remove(SelektovaniPosetilac.BrClanskeKarte, odabrana.Knjiga.ISBN);
            SelektovaniPosetilac.ListaKupovina.Remove(odabrana.Knjiga);
            _kupljene.Remove(odabrana);

            if (odabrana.Knjiga != null && !SelektovaniPosetilac.ListaZelja.Any(k => k.ISBN == odabrana.Knjiga.ISBN))
            {
                SelektovaniPosetilac.DodajNaListuZelja(odabrana.Knjiga);
                _zelje.Add(odabrana.Knjiga);
            }

            OsveziStatistiku();
        }

        // --- TAB: ŽELJE ---

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

            if (prozor.ShowDialog() == true && prozor.OdabranaKnjiga != null)
            {
                var knjiga = prozor.OdabranaKnjiga;
                SelektovaniPosetilac.DodajNaListuZelja(knjiga);
                _zelje.Add(knjiga);
            }
        }

        private void BtnUkloniSaListeZelja_Click(object sender, RoutedEventArgs e)
        {
            if (dgZelje.SelectedItem is not Knjiga odabrana)
            {
                MessageBox.Show(GetResource("msgOdaberiKnjigu"), GetResource("titleObavestenje"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var potvrda = MessageBox.Show(GetResource("msgPotvrdaUkloniZelju"), GetResource("titleUklanjanje"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            SelektovaniPosetilac.ListaZelja.Remove(odabrana);
            _zelje.Remove(odabrana);
        }

        private void BtnUpisKupovine_Click(object sender, RoutedEventArgs e)
        {
            if (dgZelje.SelectedItem is not Knjiga odabrana)
            {
                MessageBox.Show(GetResource("msgOdaberiKnjigu"), GetResource("titleObavestenje"),
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
                _kupiliDao.Add(kupovina);
                SelektovaniPosetilac.DodajKupovinu(odabrana);
                _kupljene.Add(kupovina);
                SelektovaniPosetilac.ListaZelja.Remove(odabrana);
                _zelje.Remove(odabrana);

                OsveziStatistiku();
            }
        }
    }
}
