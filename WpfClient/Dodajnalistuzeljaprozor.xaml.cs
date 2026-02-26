using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    /// <summary>
    /// Dijalog za odabir knjige koja se dodaje na listu zelja posetioca.
    /// Prikazuje samo knjige koje posjetilac nema ni u kupcima ni u zeljama.
    /// </summary>
    public partial class DodajNaListuZeljaProzor : Window
    {
        public Knjiga OdabranaKnjiga { get; private set; }

        public DodajNaListuZeljaProzor(Posetilac posetilac, List<Knjiga> sveKnjige)
        {
            InitializeComponent();

            // Knjige koje posjetilac vec ima kupljene ili na listi zelja
            var iskljuceneISBN = posetilac.ListaKupovina
                .Select(k => k.ISBN)
                .Union(posetilac.ListaZelja.Select(k => k.ISBN))
                .ToHashSet();

            // Prikazujemo samo one koje moze da doda
            var dostupne = sveKnjige
                .Where(k => !iskljuceneISBN.Contains(k.ISBN))
                .ToList();

            listKnjige.ItemsSource = dostupne;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (listKnjige.SelectedItem is Knjiga odabrana)
            {
                OdabranaKnjiga = odabrana;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Izvlačimo tekstove iz resursa na osnovu trenutnog jezika
                string poruka = Application.Current.FindResource("msgOdaberiKnjigu").ToString();
                string naslov = Application.Current.FindResource("titleObavestenje").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
