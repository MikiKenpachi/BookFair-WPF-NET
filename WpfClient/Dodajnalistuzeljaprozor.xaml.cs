using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class DodajNaListuZeljaProzor : Window
    {
        public List<Knjiga> OdabraneKnjige { get; private set; } = new List<Knjiga>();

        public DodajNaListuZeljaProzor(Posetilac posetilac, List<Knjiga> sveKnjige)
        {
            InitializeComponent();

            var iskljuceneISBN = posetilac.ListaKupovina
                .Select(k => k.ISBN)
                .Union(posetilac.ListaZelja.Select(k => k.ISBN))
                .ToHashSet();

            var dostupne = sveKnjige
                .Where(k => !iskljuceneISBN.Contains(k.ISBN))
                .ToList();

            listKnjige.ItemsSource = dostupne;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            var odabrane = listKnjige.SelectedItems.Cast<Knjiga>().ToList();

            if (!odabrane.Any())
            {
                string poruka = Application.Current.FindResource("msgOdaberiKnjigu").ToString();
                string naslov = Application.Current.FindResource("titleObavestenje").ToString();
                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            OdabraneKnjige = odabrane;
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