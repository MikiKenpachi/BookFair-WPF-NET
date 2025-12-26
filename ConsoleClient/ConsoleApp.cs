using Core.DAO;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    internal class ConsoleApp
    {
        //objekti u DAO sloju
        PosetilacDAO posetilacDao = new PosetilacDAO();
        KnjigaDAO knjigaDao = new KnjigaDAO();
        AutorDAO autorDao = new AutorDAO();
        AdresaDAO adresaDao= new AdresaDAO();
        IzdavacDAO izdavacDao = new IzdavacDAO();
        KupiliDAO kupiliDao = new KupiliDAO(); //neiskorisceno

        public void Run()
        {
            int opcija;
            do
            {
                MenuBar.PrikaziMeni();
                opcija = MenuBar.UcitajOpciju();
                ObradiOpciju(opcija);

            }while(opcija != 0);
        }

        public void ObradiOpciju(int opcija)
        {
            switch (opcija)
            {
                case 0:
                    Console.WriteLine("Kraj programa...");
                    break;
                case 1:
                    DodajPosetioca();
                    break;
                case 2:
                    PrikazPosetilaca();
                    break;
                case 3:
                    IzmenaPosetioca();  
                    break;
                case 4:
                    BrisanjePosetioca();
                    break;
                case 5:
                    DodajKnjigu();
                    break;
                case 6:
                    IzmenaKnjige();
                    break;
                case 7:
                    DodajAutora();
                    break;
                case 8:
                    PrikazAutora();
                    break;
                case 9:
                    IzmenaAutora();
                    break;
                case 10:
                    BrisanjeAutora();
                    break;
                case 11:
                    PrikazKnjige();
                    break;
                case 12:
                    BrisanjeKnjige();
                    break;
                case 13:
                    PrikaziAdrese();
                    break;
                //TODO
                default:
                    StatusBar.PrikaziPoruku("Nepoznata opcija");
                    break;

            }
        }

        public void PrikaziAdrese()
        {
            var sve = adresaDao.GetAll();
            if(sve.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih adresa.");
                return;
            }
            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("        ADRESE : ");
            Console.WriteLine(new string('=', 30));
            foreach(var a in sve)
            {
                Console.WriteLine($"Ulica : {a.Ulica} {a.Broj} , Grad : {a.Grad} , Drzava : {a.Drzava}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
            Console.ReadLine();
        }
        public void DodajPosetioca()
        {
            Adresa adr=new Adresa();

            Console.WriteLine("=== Dodavanje posetioca ===");
            Console.Write("Ime : ");
            string ime = Console.ReadLine();
            Console.Write("Prezime : ");
            string prezime = Console.ReadLine();
            Console.Write("Datum rodjenja yyyy/mm/dd : ");
            DateTime datum = DateTime.Parse(Console.ReadLine());
            Console.Write("Adresa : ");
            adr = DodajAdresu();
            Console.Write("Telefon : ");
            string telefon = Console.ReadLine();
            Console.Write("Email : ");
            string email = Console.ReadLine();
            string brkarte = (posetilacDao.GetAll().Count + 1).ToString();
            Console.Write("Godina clanstva : ");
            int godina = int.Parse(Console.ReadLine());
            Console.Write("Status (R - redovan, V - vip) : ");
            StatusPosetioca status = Console.ReadLine().ToUpper() == "V" ? StatusPosetioca.V : StatusPosetioca.R;

           
            Posetilac novi = new Posetilac(ime,prezime,datum,adr,telefon,email,brkarte,godina,status);

            posetilacDao.Add(novi);

            StatusBar.PrikaziPoruku("Posetilac uspesno dodat!");
        }

        public void PrikazPosetilaca()
        {
            var svi = posetilacDao.GetAll(); 
            if (svi.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih posetilaca.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("      LISTA POSETILACA");
            Console.WriteLine(new string('=', 30));

            foreach (var p in svi)
            {
                
                string statusOpis = p.Status == StatusPosetioca.V ? "VIP Posetilac" : "Redovni posetilac";

                string adresaInfo = p.Adresa != null
                    ? $"{p.Adresa.Ulica} {p.Adresa.Broj}, {p.Adresa.Grad}"
                    : "Nema adrese";

                Console.WriteLine($"Članska karta:  {p.BrClanskeKarte}");
                Console.WriteLine($"Ime i prezime:  {p.Ime} {p.Prezime}");
                Console.WriteLine($"Status:         {statusOpis}");
                Console.WriteLine($"Član od:        {p.GodinaClanstva}. godine");
                Console.WriteLine($"Prosečna ocena: {p.ProsecnaOcena:F2}");
                Console.WriteLine($"Telefon:        {p.Telefon}");
                Console.WriteLine($"Email:          {p.Email}");

                Console.WriteLine($"Adresa:         {adresaInfo}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
            Console.ReadLine();
        }

        public void IzmenaPosetioca()
        {
            Console.Write("Unesite broj clanske karte posetioca za izmenu: ");
            string brKarte = Console.ReadLine();

            var posetilac = posetilacDao.GetByClanskaKarta(brKarte);
            if (posetilac == null)
            {
                StatusBar.PrikaziPoruku("Posetilac ne postoji!");
                return;
            }

            Console.Write("Novo ime / ENTER (bez izmena): ");
            string ime = Console.ReadLine();
            if (!string.IsNullOrEmpty(ime)) posetilac.Ime = ime;

            Console.Write("Novo prezime / ENTER (bez izmena): ");
            string prezime = Console.ReadLine();
            if (!string.IsNullOrEmpty(prezime)) posetilac.Prezime = prezime;

            

            Console.Write("Telefon / ENTER (bez izmena): ");
            string telefon = Console.ReadLine();
            if (!string.IsNullOrEmpty(telefon)) posetilac.Telefon = telefon;

            Console.Write("Email / ENTER (bez izmena): ");
            string email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email)) posetilac.Email = email;

            Console.Write("Status R / V / ENTER (bez izmena): ");
            string status = Console.ReadLine().ToUpper();
            if (status == "R") posetilac.Status = StatusPosetioca.R;
            else if (status == "V") posetilac.Status = StatusPosetioca.V;

            StatusBar.PrikaziPoruku("Posetilac uspesno izmenjen!");
            posetilacDao.Save();
        }

        public void BrisanjePosetioca()
        {
            Console.Write("Unesite broj clanske karte posetioca za brisanje: ");
            string brKarte = Console.ReadLine();

            var posetilac = posetilacDao.GetByClanskaKarta(brKarte);
            if (posetilac == null)
            {
                StatusBar.PrikaziPoruku("Posetilac ne postoji!");
                return;
            }

            posetilacDao.Remove(posetilac);
            StatusBar.PrikaziPoruku("Posetilac uspesno obrisan!");
        }

        public void DodajIzdavaca()
        {
            Console.Write("Unesite šifru izdavača: ");
            string sifra = Console.ReadLine();
            Console.Write("Unesite naziv izdavača: ");
            string naziv = Console.ReadLine();
            Console.Write("Unesite Broj LK autora (Šefa): ");
            string lkSefa = Console.ReadLine();

            var autor = autorDao.GetByLicnaKarta(lkSefa);

            if (autor == null)
            {
                StatusBar.PrikaziPoruku("Greška: Autor sa tim LK ne postoji!");
                return;
            }

            if (autor.Godine_iskustva < 5)
            {
                StatusBar.PrikaziPoruku("Greška: Šef mora imati min. 5 godina iskustva!");
                return;
            }

            Izdavac noviIzdavac = new Izdavac(sifra, naziv, autor, new List<Autor>(), new List<Knjiga>());
            izdavacDao.Add(noviIzdavac);
            StatusBar.PrikaziPoruku("Izdavač uspešno dodat!");
        }
        public void DodajKnjigu()
        {
            Console.WriteLine("=== Dodavanje knjige ===");

            Console.Write("ISBN: ");
            string isbn = Console.ReadLine();

            Console.Write("Naziv: ");
            string naziv = Console.ReadLine();

            Console.Write("Žanr: ");
            string zanr = Console.ReadLine();

            Console.Write("Godina izdanja: ");
            string godina = Console.ReadLine();

            Console.Write("Cena: ");
            string cena = Console.ReadLine();

            Console.Write("Broj strana: ");
            string brojStrana = Console.ReadLine();

            // string id = (knjigaDao.GetAll().Count + 1).ToString();

            List<Autor> autori = new List<Autor>();
            Izdavac izdavac = new Izdavac();

            Knjiga nova = new Knjiga();
            nova.ISBN = isbn;
            nova.Naziv = naziv;
            
            nova.Zanr = (Knjiga.Zanrovi)Enum.Parse(typeof(Knjiga.Zanrovi), zanr.Replace(' ', '_'), true);
            nova.Godina_izdanja = godina;
            nova.Cena = cena;
            nova.Broj_strana = brojStrana;
            nova.ListaAutora = autori;
            nova.Izdavac = izdavac;

            knjigaDao.Add(nova);

            StatusBar.PrikaziPoruku("Knjiga uspešno dodata!");
        }

        public void IzmenaKnjige()
        {
            Console.Write("Unesite ISBN knjige za izmenu: ");
            string isbn = Console.ReadLine();

            Knjiga knjiga = knjigaDao.GetByISBN(isbn);
            if (knjiga == null)
            {
                StatusBar.PrikaziPoruku("Knjiga ne postoji!");
                return;
            }

            Console.Write("=== Izmena knjige === ");
            Console.WriteLine("(ENTER - bez promene)");

            Console.Write($"Naziv ({knjiga.Naziv}): ");
            string naziv = Console.ReadLine();
            if (!string.IsNullOrEmpty(naziv))
                knjiga.Naziv = naziv;

            Console.Write($"Žanr ({knjiga.Zanr}): ");
            string zanr = Console.ReadLine();
            if (!string.IsNullOrEmpty(zanr))
                knjiga.Zanr = (Knjiga.Zanrovi)Enum.Parse(typeof(Knjiga.Zanrovi), zanr.Replace(' ', '_'), true);

            Console.Write($"Godina izdanja ({knjiga.Godina_izdanja}): ");
            string godina = Console.ReadLine();
            if (!string.IsNullOrEmpty(godina))
                knjiga.Godina_izdanja = godina;

            Console.Write($"Cena ({knjiga.Cena}): ");
            string cena = Console.ReadLine();
            if (!string.IsNullOrEmpty(cena))
                knjiga.Cena = cena;

            Console.Write($"Broj strana ({knjiga.Broj_strana}): ");
            string brStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(brStr))
                knjiga.Broj_strana = brStr;

            Console.Write($"Izdavač ({knjiga.Izdavac}): ");
            string izdavac = Console.ReadLine();
            if (!string.IsNullOrEmpty(izdavac))
                knjiga.Izdavac.Naziv = izdavac;

            StatusBar.PrikaziPoruku("Knjiga uspešno izmenjena!");
            knjigaDao.Save();

        }

        public Adresa DodajAdresu()
        {
            
            Console.Write("Ime ulice : ");
            string ulica = Console.ReadLine();
            Console.Write("Grad : ");
            string grad = Console.ReadLine();
            Console.Write("Broj : ");
            string broj = Console.ReadLine();
            Console.Write("Drzava : ");
            string drzava = Console.ReadLine();


             Adresa adresa=new Adresa(ulica,grad,broj,drzava);

            adresaDao.Add(adresa);
            

            return adresa;
        }
        public void DodajAutora()
        {
            Adresa adr = new Adresa();

            Console.WriteLine("=== Dodavanje autora ===");
            Console.Write("Ime : ");
            string ime = Console.ReadLine();
            Console.Write("Prezime : ");
            string prezime = Console.ReadLine();
            Console.Write("Datum rodjenja yyyy/mm/dd : ");
            
            DateTime datum = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Dodaj adresu");

            adr=DodajAdresu();

            Console.Write("Telefon : ");
            string telefon = Console.ReadLine();
            Console.Write("Email : ");
            string email = Console.ReadLine();       
            Console.Write("Broj lične karte : ");
            string lk = Console.ReadLine();
            Console.Write("Godine iskustva : ");
            int godine = int.Parse(Console.ReadLine());


            

            Autor novi = new Autor(ime, prezime, datum, adr, telefon, email, godine, lk);

            autorDao.Add(novi);

            StatusBar.PrikaziPoruku("Autor uspesno dodat!");
        }


        public void PrikazAutora()
        {
            var svi = autorDao.GetAll();
            if (svi.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih autora.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("       LISTA AUTORA");
            Console.WriteLine(new string('=', 30));

            foreach (var p in svi)
            {

                string adresaInfo = p.Adresa != null
                    ? $"{p.Adresa.Ulica} {p.Adresa.Broj}, {p.Adresa.Grad}"
                    : "Nema adrese";

                string knjigeID = (p.SpisakKnjiga != null && p.SpisakKnjiga.Any())
                    ? string.Join(", ", p.SpisakKnjiga.Select(k => k.ISBN))
                    : "Nema zapisanih knjiga";

                
                Console.WriteLine($"LK:       {p.Broj_lk}");
                Console.WriteLine($"Autor:    {p.Ime} {p.Prezime}");
                Console.WriteLine($"Rođen:    {p.Datum_rodjenja.ToShortDateString()}");
                Console.WriteLine($"Iskustvo: {p.Godine_iskustva} god.");
                Console.WriteLine($"Telefon:        {p.Telefon}");
                Console.WriteLine($"Email:          {p.Email}");
                Console.WriteLine($"Adresa:   {adresaInfo}");
                Console.WriteLine($"Knjige (ISBN): {knjigeID}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
            Console.ReadLine();
        }


        public void IzmenaAutora()
        {
            Console.Write("Unesite ličnu kartu autora za izmenu: ");
            string lck = Console.ReadLine();

            Autor autor = autorDao.GetByLicnaKarta(lck);
            if (autor == null)
            {
                StatusBar.PrikaziPoruku("Autor ne postoji!");
                return;
            }

            Console.Write("=== Izmena autora === ");
            Console.WriteLine("(ENTER - bez promene)");

            Console.Write($"Ime ({autor.Ime}): ");
            string naziv = Console.ReadLine();
            if (!string.IsNullOrEmpty(naziv))
                autor.Ime = naziv;

            Console.Write($"Prezime ({autor.Prezime}): ");
            string pr = Console.ReadLine();
            if (!string.IsNullOrEmpty(pr))
                autor.Prezime = pr;

            Console.Write($"Datum rodjenja ({autor.Datum_rodjenja}): ");
            string datum = Console.ReadLine();
            if (!string.IsNullOrEmpty(datum))
                autor.Datum_rodjenja = DateTime.Parse(datum);

           //Izmena adrese ???

            Console.Write($"Telefon ({autor.Telefon}): ");
            string tel = Console.ReadLine();
            if (!string.IsNullOrEmpty(tel))
                autor.Telefon = tel;

            Console.Write($"Godine iskustva ({autor.Godine_iskustva}): ");
            string godine = Console.ReadLine();
            if (!string.IsNullOrEmpty(godine))
                autor.Godine_iskustva = int.Parse(godine);


            StatusBar.PrikaziPoruku("Autor uspešno izmenjen!");
            autorDao.Save();

        }

        public void BrisanjeAutora()
        {
            Console.Write("Unesite broj licne karte autora za brisanje: ");
            string brKarte = Console.ReadLine();

            var autor = autorDao.GetByLicnaKarta(brKarte);
            if (autor == null)
            {
                StatusBar.PrikaziPoruku("Autor ne postoji!");
                return;
            }

            autorDao.Remove(autor);
            StatusBar.PrikaziPoruku("Autor uspesno obrisan!");
        }

        public void BrisanjeKnjige()
        {
            Console.Write("Unesite ISBN knjige za brisanje: ");
            string brKarte = Console.ReadLine();

            var knjiga = knjigaDao.GetByISBN(brKarte);
            if (knjiga == null)
            {
                StatusBar.PrikaziPoruku("Knjiga ne postoji!");
                return;
            }

            knjigaDao.Remove(knjiga);
            StatusBar.PrikaziPoruku("Knjiga uspesno obrisana!");
        }
        public void PrikazKnjige()
        {
            var svi = knjigaDao.GetAll();
            if (svi.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih knjiga.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("       LISTA KNJIGA");
            Console.WriteLine(new string('=', 30));

            foreach (var p in svi)
            {

                string autoriID = (p.ListaAutora != null && p.ListaAutora.Any())
                    ? string.Join(", ", p.ListaAutora.Select(a => a.Broj_lk))
                    : "Nema autora";

                string izdavacSifra = p.Izdavac != null ? p.Izdavac.Sifra : "Nema izdavača";

                Console.WriteLine($"ID: {p.ISBN}");
                Console.WriteLine($"Naslov:  {p.Naziv}");
                Console.WriteLine($"Detalji: {p.Zanr} | {p.Godina_izdanja}. god | {p.Broj_strana} str.");
                Console.WriteLine($"Cena:    {p.Cena} RSD");
                Console.WriteLine($"Izdavač: {izdavacSifra}");
                Console.WriteLine($"Autori (LK): {autoriID}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
            Console.ReadLine();
        }

    }
}
