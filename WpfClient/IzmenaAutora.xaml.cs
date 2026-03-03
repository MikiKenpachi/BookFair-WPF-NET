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

        public Autor SelektovaniAutor { get; private set; }
        private ObservableCollection<Knjiga> _knjige;
        private readonly List<Knjiga> _sveKnjige;
        private readonly AutorDAO _autorDao;
        private readonly KnjigaDAO _knjigaDao;

        private AutorDTO _validator = new AutorDTO();

        public IzmenaAutora(Autor autor, List<Knjiga> sveKnjige, AutorDAO autorDao, KnjigaDAO knjigaDao)
        {
            InitializeComponent();

            SelektovaniAutor = autor;
            _sveKnjige = sveKnjige ?? new List<Knjiga>();
            _autorDao = autorDao;
            _knjigaDao = knjigaDao;

            PopuniValidator();
            this.DataContext = _validator;

            UcitajKnjige();
            OsveziDugmePotvrdi();
        }

        private void PopuniValidator()
        {
            if (SelektovaniAutor == null) return;

            _validator.BrojLk = SelektovaniAutor.Broj_lk;
            _validator.Ime = SelektovaniAutor.Ime;
            _validator.Prezime = SelektovaniAutor.Prezime;
            _validator.Email = SelektovaniAutor.Email;
            _validator.Telefon = SelektovaniAutor.Telefon;
            _validator.GodineIskustva = SelektovaniAutor.Godine_iskustva.ToString();
            _validator.DatumRodjenja = SelektovaniAutor.Datum_rodjenja;

            if (SelektovaniAutor.Adresa != null)
            {
                _validator.Ulica = SelektovaniAutor.Adresa.Ulica;
                _validator.Broj = SelektovaniAutor.Adresa.Broj;
                _validator.Grad = SelektovaniAutor.Adresa.Grad;
                _validator.Drzava = SelektovaniAutor.Adresa.Drzava;
            }
        }

        private void Polje_Changed(object sender, EventArgs e)
        {
            OsveziDugmePotvrdi();
        }

        private void OsveziDugmePotvrdi()
        {
            if (btnPotvrdi != null)
                btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private bool FormaJeValidna()
        {
            string[] polja = {
                nameof(_validator.Ime), nameof(_validator.Prezime), nameof(_validator.Email),
                nameof(_validator.BrojLk), nameof(_validator.GodineIskustva),
                nameof(_validator.Grad), nameof(_validator.DatumRodjenja)
            };

            foreach (var p in polja)
            {
                if (!string.IsNullOrEmpty(_validator[p])) return false;
            }

            return !string.IsNullOrWhiteSpace(_validator.Ime) &&
                   !string.IsNullOrWhiteSpace(_validator.Prezime) &&
                   _validator.DatumRodjenja.HasValue;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (!FormaJeValidna())
            {
                string poruka = Application.Current.FindResource("msgIspraviGreske").ToString();
                MessageBox.Show(poruka, "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelektovaniAutor.Ime = _validator.Ime.Trim();
            SelektovaniAutor.Prezime = _validator.Prezime.Trim();
            SelektovaniAutor.Email = _validator.Email.Trim();
            SelektovaniAutor.Telefon = _validator.Telefon?.Trim();
            SelektovaniAutor.Datum_rodjenja = _validator.DatumRodjenja.Value;
            SelektovaniAutor.Godine_iskustva = int.Parse(_validator.GodineIskustva);
            SelektovaniAutor.Broj_lk = _validator.BrojLk.Trim();

            if (SelektovaniAutor.Adresa == null) SelektovaniAutor.Adresa = new Adresa();
            SelektovaniAutor.Adresa.Ulica = _validator.Ulica?.Trim();
            SelektovaniAutor.Adresa.Broj = _validator.Broj?.Trim();
            SelektovaniAutor.Adresa.Grad = _validator.Grad?.Trim();
            SelektovaniAutor.Adresa.Drzava = _validator.Drzava?.Trim();

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void UcitajKnjige()
        {
            _knjige = new ObservableCollection<Knjiga>(SelektovaniAutor.SpisakKnjiga ?? new List<Knjiga>());
            dgKnjige.ItemsSource = _knjige;
        }

        private void BtnDodajKnjigu_Click(object sender, RoutedEventArgs e)
        {
            var autoroveISBN = _knjige.Select(k => k.ISBN).ToHashSet();
            var dostupne = _sveKnjige.Where(k => !autoroveISBN.Contains(k.ISBN)).ToList();

            if (!dostupne.Any())
            {
                MessageBox.Show(Application.Current.FindResource("msgNemaKnjigaZaDodavanje").ToString(),
                    Application.Current.FindResource("titleInfo").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var prozor = new OdaberiKnjiguProzor(dostupne) { Owner = this };

            if (prozor.ShowDialog() == true && prozor.OdabranaKnjiga != null)
            {
                Knjiga odabrana = prozor.OdabranaKnjiga;
                _knjige.Add(odabrana);
                if (SelektovaniAutor.SpisakKnjiga == null) SelektovaniAutor.SpisakKnjiga = new List<Knjiga>();
                if (!SelektovaniAutor.SpisakKnjiga.Contains(odabrana)) SelektovaniAutor.SpisakKnjiga.Add(odabrana);
                if (odabrana.ListaAutora == null) odabrana.ListaAutora = new List<Autor>();
                if (!odabrana.ListaAutora.Contains(SelektovaniAutor)) odabrana.ListaAutora.Add(SelektovaniAutor);

                _autorDao.Update(SelektovaniAutor);
                _knjigaDao.Save();
            }
        }

        private void BtnUkloniKnjigu_Click(object sender, RoutedEventArgs e)
        {
            var selektovane = dgKnjige.SelectedItems.Cast<Knjiga>().ToList();
            if (!selektovane.Any())
            {
                MessageBox.Show(Application.Current.FindResource("msgOdaberiKnjiguIzTabele").ToString(),
                    Application.Current.FindResource("titleObavestenje").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string start = Application.Current.FindResource("msgPotvrdaUklanjanjaStart").ToString();
            string popisKnjiga = string.Join("\n", selektovane.Select(k => $"• {k.Naziv}"));
            if (MessageBox.Show($"{start}\n{popisKnjiga}", Application.Current.FindResource("titleUklanjanjeKnjige").ToString(),
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var knjiga in selektovane)
                {
                    _knjige.Remove(knjiga);
                    SelektovaniAutor.SpisakKnjiga?.Remove(knjiga);
                    knjiga.ListaAutora?.Remove(SelektovaniAutor);
                }
                _autorDao.Update(SelektovaniAutor);
                _knjigaDao.Save();
            }

        }
    }
}
