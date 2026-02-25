using Core.DAO;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for IzdavaciProzor.xaml
    /// </summary>
    /// 

    public partial class IzdavaciProzor : Window
    {
        // Koristimo referencu na listu iz MainWindow
        public ObservableCollection<Izdavac> ListaIzdavaca { get; set; }

        public IzdavaciProzor(ObservableCollection<Izdavac> listaIzGlavnog)
        {
            InitializeComponent();

            // Povezujemo se na istu listu koju koristi MainWindow
            ListaIzdavaca = listaIzGlavnog;
            DataGridIzdavaci.ItemsSource = ListaIzdavaca;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            DodajIzdavacaProzor prozor = new DodajIzdavacaProzor();
            prozor.Owner = this;

            if (prozor.ShowDialog() == true)
            {
                // SAMO dodajemo u listu. NE zovemo dao.Add()!
                ListaIzdavaca.Add(prozor.Izdavac);
            }
        }

        private void BtnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            Izdavac selektovan = (Izdavac)DataGridIzdavaci.SelectedItem;
            if (selektovan != null)
            {
                DodajIzdavacaProzor prozor = new DodajIzdavacaProzor(selektovan);
                prozor.Owner = this;

                if (prozor.ShowDialog() == true)
                {
                    DataGridIzdavaci.Items.Refresh();
                    // Opet, NE zovemo dao.Update()!
                }
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            Izdavac selektovan = (Izdavac)DataGridIzdavaci.SelectedItem;
            if (selektovan != null)
            {
                if (MessageBox.Show($"Obrisati?", "Potvrda", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // SAMO uklanjamo iz liste
                    ListaIzdavaca.Remove(selektovan);
                }
            }
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
