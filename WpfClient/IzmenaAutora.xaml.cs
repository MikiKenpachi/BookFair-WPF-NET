using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for IzmenaAutora.xaml
    /// Sadrži dva taba:
    ///   - Informacije : izmena osnovnih podataka autora
    ///   - Knjige      : prikaz, dodavanje i uklanjanje knjiga koje je autor napisao
    /// </summary>
    public partial class IzmenaAutora : Window
    {
        // ── Selektovani autor (menjamo direktno u memoriji) ─────────────
        public Autor SelektovaniAutor { get; private set; }

        // ── Lista knjiga koje autor trenutno ima (za DataGrid) ───────────
        private ObservableCollection<Knjiga> _knjige;

        // ── Sve knjige u sistemu (potrebne za dijalog dodavanja) ─────────
        private readonly List<Knjiga> _sveKnjige;

        // ── DAO (za snimanje promena) ─────────────────────────────────────
        private readonly AutorDAO _autorDao = new AutorDAO();
        private readonly KnjigaDAO _knjigaDao = new KnjigaDAO();

        // ================================================================
        // Konstruktor
        // ================================================================
        /// <param name="autor">Autor koji se menja</param>
        /// <param name="sveKnjige">Sve knjige u sistemu — potrebno za tab Knjige</param>
        public IzmenaAutora(Autor autor, List<Knjiga> sveKnjige, AutorDAO autorDao, KnjigaDAO knjigaDao)
        {
            InitializeComponent();

            SelektovaniAutor = autor;
            _sveKnjige = sveKnjige ?? new List<Knjiga>();
            //_autorDao = autorDao;
            //_knjigaDao = knjigaDao;

            PopuniPolja();
            UcitajKnjige();
        }

        // ================================================================
        // Tab 1 — Informacije
        // ================================================================

        private void PopuniPolja()
        {
            if (SelektovaniAutor == null) return;

            txtBrojLK.Text = SelektovaniAutor.Broj_lk;
            txtIme.Text = SelektovaniAutor.Ime;
            txtPrezime.Text = SelektovaniAutor.Prezime;
            txtEmail.Text = SelektovaniAutor.Email;
            txtTelefon.Text = SelektovaniAutor.Telefon;
            txtIskustvo.Text = SelektovaniAutor.Godine_iskustva.ToString();
            dpDatumRodjenja.SelectedDate = SelektovaniAutor.Datum_rodjenja;

            if (SelektovaniAutor.Adresa != null)
            {
                txtUlica.Text = SelektovaniAutor.Adresa.Ulica;
                txtBroj.Text = SelektovaniAutor.Adresa.Broj;
                txtGrad.Text = SelektovaniAutor.Adresa.Grad;
                txtDrzava.Text = SelektovaniAutor.Adresa.Drzava;
            }
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIme.Text) ||
          string.IsNullOrWhiteSpace(txtPrezime.Text) ||
          string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string poruka = Application.Current.FindResource("msgPopuniteSvaPolja").ToString();
                string naslov = Application.Current.FindResource("errorTitle").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelektovaniAutor.Ime = txtIme.Text.Trim();
            SelektovaniAutor.Prezime = txtPrezime.Text.Trim();
            SelektovaniAutor.Email = txtEmail.Text.Trim();
            SelektovaniAutor.Telefon = txtTelefon.Text.Trim();
            SelektovaniAutor.Datum_rodjenja =
                dpDatumRodjenja.SelectedDate ?? SelektovaniAutor.Datum_rodjenja;

            if (int.TryParse(txtIskustvo.Text, out int iskustvo))
                SelektovaniAutor.Godine_iskustva = iskustvo;

            if (SelektovaniAutor.Adresa != null)
            {
                SelektovaniAutor.Adresa.Ulica = txtUlica.Text.Trim();
                SelektovaniAutor.Adresa.Broj = txtBroj.Text.Trim();
                SelektovaniAutor.Adresa.Grad = txtGrad.Text.Trim();
                SelektovaniAutor.Adresa.Drzava = txtDrzava.Text.Trim();
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
        // Tab 2 — Knjige
        // ================================================================

        /// <summary>
        /// Puni DataGrid knjigama koje autor već ima u svom SpisakKnjiga.
        /// SpisakKnjiga je popunjen u DataBinding.PoveziSve() pri startu aplikacije.
        /// </summary>
        private void UcitajKnjige()
        {
            _knjige = new ObservableCollection<Knjiga>(
                SelektovaniAutor.SpisakKnjiga ?? new List<Knjiga>());

            dgKnjige.ItemsSource = _knjige;
        }

        // ── Dugme "Dodaj knjigu" ─────────────────────────────────────────
        private void BtnDodajKnjigu_Click(object sender, RoutedEventArgs e)
        {
            // Skup ISBN-ova knjiga koje autor već ima — da ih ne nudimo ponovo
            var autoroveISBN = _knjige.Select(k => k.ISBN).ToHashSet();

            var dostupne = _sveKnjige
                .Where(k => !autoroveISBN.Contains(k.ISBN))
                .ToList();
            if (!dostupne.Any())
            {
                string poruka = Application.Current.FindResource("msgNemaKnjigaZaDodavanje").ToString();
                string naslov = Application.Current.FindResource("titleInfo").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var prozor = new OdaberiKnjiguProzor(dostupne)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.OdabranaKnjiga != null)
            {
                Knjiga odabrana = prozor.OdabranaKnjiga;

                // 1. Dodaj u DataGrid (ObservableCollection obaveštava UI automatski)
                _knjige.Add(odabrana);

                // 2. Dodaj u memorijsku listu autora
                if (SelektovaniAutor.SpisakKnjiga == null)
                    SelektovaniAutor.SpisakKnjiga = new List<Knjiga>();

                if (!SelektovaniAutor.SpisakKnjiga.Contains(odabrana))
                    SelektovaniAutor.SpisakKnjiga.Add(odabrana);

                // 3. Bidirekciona veza — dodaj autora u ListaAutora knjige
                if (odabrana.ListaAutora == null)
                    odabrana.ListaAutora = new List<Autor>();

                if (!odabrana.ListaAutora.Contains(SelektovaniAutor))
                    odabrana.ListaAutora.Add(SelektovaniAutor);

                // 4. Snimi u fajlove
                _autorDao.Update(SelektovaniAutor);
                _knjigaDao.Save();
            }
        }

        // ── Dugme "Ukloni knjigu" ────────────────────────────────────────
        private void BtnUkloniKnjigu_Click(object sender, RoutedEventArgs e)
        {
            var selektovane = dgKnjige.SelectedItems
                .Cast<Knjiga>()
                .ToList();

            if (!selektovane.Any())
            {
                MessageBox.Show(
                    Application.Current.FindResource("msgOdaberiKnjiguIzTabele").ToString(),
                    Application.Current.FindResource("titleObavestenje").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Poruka potvrde
            string start = Application.Current.FindResource("msgPotvrdaUklanjanjaStart").ToString();
            
            string naslov = Application.Current.FindResource("titleUklanjanjeKnjige").ToString();

            string popisKnjiga = string.Join("\n", selektovane.Select(k => $"• {k.Naziv}"));
            string punaPoruka = $"{start}\n{popisKnjiga}\n";

            var potvrda = MessageBox.Show(punaPoruka, naslov, MessageBoxButton.YesNo, MessageBoxImage.None);
            if (potvrda != MessageBoxResult.Yes) return;

            foreach (var knjiga in selektovane)
            {
                // 1. Ukloni iz DataGrid-a (ObservableCollection)
                _knjige.Remove(knjiga);
                // 2. Ukloni iz memorijskog spiska autora
                SelektovaniAutor.SpisakKnjiga?.Remove(knjiga);
                // 3. Bidirekciona veza
                knjiga.ListaAutora?.Remove(SelektovaniAutor);
            }

            // 4. Snimi promene — jednom nakon petlje
            _autorDao.Update(SelektovaniAutor);
            _knjigaDao.Save();
        }
    }
}
