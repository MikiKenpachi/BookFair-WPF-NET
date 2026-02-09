using Core;
using Core.DAO;
using Core.Storage.Serialization;
using SajamKnjigaProjekat.Core.DAO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Posetilac> Posetioci { get; set; }
        public ObservableCollection<Autor> Autori { get; set; }
        public ObservableCollection<Knjiga> Knjige { get; set; }
        public MainWindow()
        {

            InitializeComponent(); // Prvo inicijalizuj komponente
            KnjigaDAO knjigaDao = new KnjigaDAO();
            AutorDAO autorDao = new AutorDAO();
            AdresaDAO adresaDao = new AdresaDAO();
            IzdavacDAO izdavacDao = new IzdavacDAO();
            PosetilacDAO posetilacDao = new PosetilacDAO();

            List<Posetilac> sviPosetioci = posetilacDao.GetAll();
            List<Autor> sviAutori = autorDao.GetAll();
            var listaIzFajla = knjigaDao.GetAll();


            foreach (var p in sviPosetioci)
            {
                // Koristimo BrClanskeKarte kao ID vlasnika (ili BrLicneKarte, proveri šta ti je ID)
                p.Adresa = adresaDao.GetByVlasnikID(p.BrClanskeKarte);
            }

            foreach (var p in sviAutori)
            {
                // Koristimo BrClanskeKarte kao ID vlasnika (ili BrLicneKarte, proveri šta ti je ID)
                p.Adresa = adresaDao.GetByVlasnikID(p.Broj_lk);
            }

            // 4. Sada napuni ObservableCollection uparenim podacima
            Posetioci = new ObservableCollection<Posetilac>(sviPosetioci);
            Autori = new ObservableCollection<Autor>(sviAutori);
            Knjige = new ObservableCollection<Knjiga>(listaIzFajla);


            // 5. Poveži sa DataGrid-om
            DataGridPosetioci.ItemsSource = Posetioci;
            this.DataContext = this;
        }
        


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           this.Width = SystemParameters.PrimaryScreenWidth * 0.75;
           this.Height = SystemParameters.PrimaryScreenHeight * 0.75;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            int index = MainTabControl.SelectedIndex;

            if (index == 0)
            {
                // Otvori dijalog za novog Posetioca
                // var dijalo = new PosetilacDialog();
                // if (dijalog.ShowDialog() == true) { ... }
            }
            else if (index == 1)
            {
                // Otvori dijalog za Autora
            }
            else if (index == 2)
            {
                // Otvori dijalog za Knjigu
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            
            // Ponoviti logiku za ostale tabove...
        }

        private void BtnIzmeni_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void DataGridPosetioci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}