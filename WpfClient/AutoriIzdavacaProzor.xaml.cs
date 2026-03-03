using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for AutoriIzdavacaProzor.xaml
    /// </summary>
    public partial class AutoriIzdavacaProzor : Window
    {
        public List<Autor> SviAutori { get; set; }

        public ICollectionView AutoriView { get; set; }

        public AutoriIzdavacaProzor(List<Autor> autori, string nazivIzdavaca)
        {
            InitializeComponent();
        
           
            

            AutoriView = CollectionViewSource.GetDefaultView(autori);

            dgAutori.ItemsSource = AutoriView;
            txtNaslov.Text += " : " +  nazivIzdavaca;
        }

        private void BtnPretrazi_Click(object sender, RoutedEventArgs e)
        {
            string filter = txtPretraga.Text.ToLower();

            
                FiltrirajAutore(filter);
           
        }

        private void FiltrirajAutore(string upit)
        {
            if (AutoriView == null) return;

            if (string.IsNullOrWhiteSpace(upit))
            {
                AutoriView.Filter = null;
            }
            else
            {
                string[] delovi = upit.ToLower().Split(',');
                for (int i = 0; i < delovi.Length; i++) delovi[i] = delovi[i].Trim();

                AutoriView.Filter = obj =>
                {
                    var a = obj as Autor;
                    if (a == null) return false;

                    string prezime = a.Prezime?.ToLower() ?? "";
                    string ime = a.Ime?.ToLower() ?? "";

                    if (delovi.Length == 1)
                        return prezime.Contains(delovi[0]);
                    else if (delovi.Length >= 2)
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);

                    return false;
                };
            }

            AutoriView.Refresh();
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

