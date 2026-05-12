using LibrarieModele;
using NivelStocareDate;

IStocareMasini adminMasini = new AdministrareMasiniFisierText("Masini.txt");

bool ruleaza = true;

while (ruleaza)
{
    Console.WriteLine("=== Aplicatie Inchirieri Masini ===");
    Console.WriteLine("1. Adauga masina");
    Console.WriteLine("2. Afiseaza toate masinile");
    Console.WriteLine("3. Afiseaza masinile disponibile");
    Console.WriteLine("0. Iesire");
    Console.Write("Alege optiunea: ");

    string optiune = Console.ReadLine();

    switch (optiune)
    {
        case "1":
            AdaugaMasina(adminMasini);
            break;

        case "2":
            AfiseazaMasini(adminMasini.GetMasini());
            break;

        case "3":
            AfiseazaMasiniDisponibile(adminMasini.GetMasini());
            break;

        case "0":
            ruleaza = false;
            break;

        default:
            Console.WriteLine("Optiune invalida.");
            break;
    }

    Console.WriteLine();
}

static void AdaugaMasina(IStocareMasini adminMasini)
{
    Masina masina = new Masina();

    Console.Write("Marca: ");
    masina.Marca = Console.ReadLine();

    Console.Write("Model: ");
    masina.Model = Console.ReadLine();

    Console.Write("Numar inmatriculare: ");
    masina.NumarInmatriculare = Console.ReadLine();

    Console.Write("An fabricatie: ");
    masina.AnFabricatie = Convert.ToInt32(Console.ReadLine());

    Console.Write("Pret pe zi: ");
    masina.PretPeZi = Convert.ToDecimal(Console.ReadLine());

    masina.Disponibila = true;

    adminMasini.AddMasina(masina);

    Console.WriteLine("Masina a fost adaugata cu succes.");
}

static void AfiseazaMasini(List<Masina> masini)
{
    if (masini.Count == 0)
    {
        Console.WriteLine("Nu exista masini salvate.");
        return;
    }

    foreach (Masina masina in masini)
    {
        Console.WriteLine(masina.Info());
    }
}

static void AfiseazaMasiniDisponibile(List<Masina> masini)
{
    List<Masina> disponibile = masini.Where(m => m.Disponibila == true).ToList();

    if (disponibile.Count == 0)
    {
        Console.WriteLine("Nu exista masini disponibile.");
        return;
    }

    foreach (Masina masina in disponibile)
    {
        Console.WriteLine(masina.Info());
    }
}