using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for PoredjenjePosetilacaView.xaml
    /// </summary>
    public partial class PoredjenjePosetilacaView : Window
    {
        private readonly List<Posetilac> _sviPosetioci;
        private readonly List<Knjiga> _sveKnjige;

        public ICollectionView PosetiociView { get; set; }
        public ICollectionView Posetioci2View { get; set; }

        public PoredjenjePosetilacaView(List<Posetilac> posetioci, List<Knjiga> knjige)
        {
            InitializeComponent();

            _sviPosetioci = posetioci;
            _sveKnjige = knjige;

            cmbKnjiga1.ItemsSource = _sveKnjige;
            cmbKnjiga1.DisplayMemberPath = "Naziv";
            cmbKnjiga2.ItemsSource = _sveKnjige;
            cmbKnjiga2.DisplayMemberPath = "Naziv";

            cmbKnjiga1.SelectionChanged += CmbKnjiga1_SelectionChanged;
            cmbKnjiga2.SelectionChanged += CmbKnjiga2_SelectionChanged;
        }

        private void Prikazi_Click(object sender, RoutedEventArgs e)
        {
            var k1 = cmbKnjiga1.SelectedItem as Knjiga;
            var k2 = cmbKnjiga2.SelectedItem as Knjiga;

            if (k1 == null || k2 == null)
            {
                string poruka = Application.Current.FindResource("msgOdaberiObeKnjige")?.ToString() ?? "Molimo odaberite obe knjige.";
                string naslov = Application.Current.FindResource("titleObavestenje")?.ToString() ?? "Obaveštenje";

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tabela 1: Posetioci koji imaju OBE knjige na listi želja
            var zeljeObe = _sviPosetioci.Where(p =>
                p.ListaZelja != null &&
                p.ListaZelja.Any(z => z.ISBN == k1.ISBN) &&
                p.ListaZelja.Any(z => z.ISBN == k2.ISBN)
            ).ToList();

            PosetiociView = CollectionViewSource.GetDefaultView(zeljeObe);
            dgZeljeObe.ItemsSource = PosetiociView;

            // Tabela 2: Posetioci koji su kupili PRVU knjigu (k1), a NISU kupili drugu (k2)
            var kupiliPrvuNisuDrugu = _sviPosetioci.Where(p =>
                p.ListaKupovina != null &&
                p.ListaKupovina.Any(k => k.ISBN == k1.ISBN) &&
                !p.ListaKupovina.Any(k => k.ISBN == k2.ISBN)
            ).ToList();

            Posetioci2View = CollectionViewSource.GetDefaultView(kupiliPrvuNisuDrugu);
            dgKupljenaPrva.ItemsSource = Posetioci2View;
        }

        private void Zatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SearchLevo_Click(object sender, RoutedEventArgs e)
        {
            string filter = SearchLevoTextBox.Text.ToLower();
            FiltrirajPosetioce(filter);
        }

        private void SearchDesno_Click(object sender, RoutedEventArgs e)
        {
            string filter = SearchDesnoTextBox.Text.ToLower();
            FiltrirajPosetioce2(filter);
        }

        private void FiltrirajPosetioce(string upit)
        {
            if (PosetiociView == null) return;

            if (string.IsNullOrWhiteSpace(upit))
            {
                PosetiociView.Filter = null;
            }
            else
            {
                string[] delovi = upit.ToLower().Split(',');
                for (int i = 0; i < delovi.Length; i++) delovi[i] = delovi[i].Trim();

                PosetiociView.Filter = obj =>
                {
                    var p = obj as Posetilac;
                    if (p == null) return false;

                    string prezime = p.Prezime?.ToLower() ?? "";
                    string ime = p.Ime?.ToLower() ?? "";
                    string karta = p.BrClanskeKarte?.ToLower() ?? "";

                    if (delovi.Length == 1)
                        return prezime.Contains(delovi[0]);
                    else if (delovi.Length == 2)
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);
                    else if (delovi.Length >= 3)
                        return karta.Contains(delovi[0]) && ime.Contains(delovi[1]) && prezime.Contains(delovi[2]);

                    return false;
                };
            }

            PosetiociView.Refresh();
        }

        private void FiltrirajPosetioce2(string upit)
        {
            if (Posetioci2View == null) return;

            if (string.IsNullOrWhiteSpace(upit))
            {
                Posetioci2View.Filter = null;
            }
            else
            {
                string[] delovi = upit.ToLower().Split(',');
                for (int i = 0; i < delovi.Length; i++) delovi[i] = delovi[i].Trim();

                Posetioci2View.Filter = obj =>
                {
                    var p = obj as Posetilac;
                    if (p == null) return false;

                    string prezime = p.Prezime?.ToLower() ?? "";
                    string ime = p.Ime?.ToLower() ?? "";
                    string karta = p.BrClanskeKarte?.ToLower() ?? "";

                    if (delovi.Length == 1)
                        return prezime.Contains(delovi[0]);
                    else if (delovi.Length == 2)
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);
                    else if (delovi.Length >= 3)
                        return karta.Contains(delovi[0]) && ime.Contains(delovi[1]) && prezime.Contains(delovi[2]);

                    return false;
                };
            }

            Posetioci2View.Refresh();
        }

        private void CmbKnjiga1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var k1 = cmbKnjiga1.SelectedItem as Knjiga;

            if (k1 != null && cmbKnjiga2.SelectedItem is Knjiga k2 && k1.ISBN == k2.ISBN)
                cmbKnjiga2.SelectedItem = null;

            cmbKnjiga2.ItemsSource = k1 == null
                ? _sveKnjige
                : _sveKnjige.Where(k => k.ISBN != k1.ISBN).ToList();
        }

        private void CmbKnjiga2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var k2 = cmbKnjiga2.SelectedItem as Knjiga;

            if (k2 != null && cmbKnjiga1.SelectedItem is Knjiga k1 && k2.ISBN == k1.ISBN)
                cmbKnjiga1.SelectedItem = null;

            cmbKnjiga1.ItemsSource = k2 == null
                ? _sveKnjige
                : _sveKnjige.Where(k => k.ISBN != k2.ISBN).ToList();
        }
    }
}