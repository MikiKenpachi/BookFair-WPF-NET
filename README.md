# 📚 Sajam Knjiga - Informacioni sistem uprave sajma knjiga

Predmetni projekat iz predmeta **Osnovi informacionih sistema i softverskog inženjerstva (OISISI)** **Fakultet tehničkih nauka, Univerzitet u Novom Sadu**

## 📖 Opis projekta

**SajamKnjiga** je desktop aplikacija namenjena referentu sajma za efikasno upravljanje podacima tokom sajamske manifestacije. Projekat je fokusiran na implementaciju sistema za evidenciju učesnika, kataloga knjiga i realizovanih kupovina kroz intuitivan grafički interfejs.

Aplikacija omogućava rad sa sledećim entitetima:
* **Posetioci** (evidencija i podaci o posetiocima sajma)
* **Autori i Knjige** (katalog napisanih dela i biografije autora)
* **Izdavači** (upravljanje podacima o izdavačkim kućama)
* **Kupovine** (evidencija realizovanih kupovina)

## 🏗️ Arhitektura softvera

Projekat je realizovan uz strogo poštovanje inženjerskih principa i obrazaca:
* **MVC (Model-View-Controller):** Jasno razdvajanje poslovne logike, podataka i korisničkog prikaza.
* **DAO (Data Access Object):** Standardizovan sloj za pristup podacima i njihovu perzistenciju.
* **Serijalizacija:** Čuvanje i učitavanje stanja sistema putem datoteka.

## 🛠️ Tehnologije

| Tehnologija | Opis |
| :--- | :--- |
| **C# / .NET** | Osnovni programski jezik i platforma. |
| **WPF (XAML)** | Izrada korisničkog interfejsa. |
| **MVC** | Arhitektonski šablon za organizaciju koda. |
| **Git** | Sistem za kontrolu verzija. |

## ✨ Ključne funkcionalnosti

* **CRUD operacije:** Dodavanje, izmena, brisanje i pregled svih entiteta sistema.
* **Lokalizacija:** Podrška za srpski i engleski jezik unutar cele aplikacije.
* **Paginacija:** Efikasan prikaz podataka (do 16 entiteta po stranici).
* **Pretraga i filtriranje:** Case-insensitive pretraga po specifičnim kriterijumima za posetioce, autore i knjige.
* **Validacija:** Provera ispravnosti unosa pre potvrde akcija.

## 👥 Članovi tima

| Ime i prezime | Broj indeksa | Smer |
| :--- | :--- | :--- |
| **Miloš Trišić** | RA 39/2023 | Računarstvo i automatika |
| **Boris Stepanović** | RA 97/2023 | Računarstvo i automatika |

## 📄 Licenca

Ovaj projekat je izrađen isključivo u obrazovne svrhe u okviru studija na Fakultetu tehničkih nauka u Novom Sadu.
