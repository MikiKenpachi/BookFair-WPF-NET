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
        public ObservableCollection<Izdavac> ListaIzdavaca { get; set; }

        public Izdavac? NoviIzdavac { get; private set; }
        public IzdavaciProzor()
        {
            InitializeComponent();

            // Učitavanje iz fajla preko DAO klase
            var dao = new IzdavacDAO();
            ListaIzdavaca = new ObservableCollection<Izdavac>(dao.GetAll());

            // Povezivanje sa DataGrid-om (u XAML-u ti se zove DataGridIzdavaci)
            DataGridIzdavaci.ItemsSource = ListaIzdavaca;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            // OTVARAMO NOVI PROZOR (DodajIzdavacaProzor), a ne IzdavaciProzor!
            DodajIzdavacaProzor prozor = new DodajIzdavacaProzor();
            prozor.Owner = this;

            if (prozor.ShowDialog() == true)
            {
                // Dodajemo u listu da se vidi u tabeli
                ListaIzdavaca.Add(prozor.Izdavac);

                // Snimamo u fajl
                IzdavacDAO dao = new IzdavacDAO();
                dao.Add(prozor.Izdavac);
            }
        }

        private void BtnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            Izdavac selektovan = (Izdavac)DataGridIzdavaci.SelectedItem;

            if (selektovan != null)
            {
                // Šaljemo selektovanog izdavača u prozor za izmenu
                DodajIzdavacaProzor prozor = new DodajIzdavacaProzor(selektovan);
                prozor.Owner = this;

                if (prozor.ShowDialog() == true)
                {
                    // Osvežavamo prikaz u DataGrid-u (jer su se podaci promenili u memoriji)
                    DataGridIzdavaci.Items.Refresh();

                    // Snimamo izmene u fajl
                    IzdavacDAO dao = new IzdavacDAO();
                    dao.Update(selektovan);
                }
            }
            else
            {
                MessageBox.Show("Molimo selektujte izdavača za izmenu.", "Obaveštenje");
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            Izdavac selektovan = (Izdavac)DataGridIzdavaci.SelectedItem;

            if (selektovan != null)
            {
                var rezultat = MessageBox.Show($"Obrisati izdavača {selektovan.Naziv}?", "Potvrda", MessageBoxButton.YesNo);
                if (rezultat == MessageBoxResult.Yes)
                {
                    ListaIzdavaca.Remove(selektovan);

                    IzdavacDAO dao = new IzdavacDAO();
                    dao.Remove(selektovan); // Proveri da li se metoda zove Delete ili Remove u tvom DAO
                }
            }
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
