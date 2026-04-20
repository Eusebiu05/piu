using RentMaster.Models;

// Configurare fișiere
string fisierMasini = "masini.txt";
string fisierInchirieri = "inchirieri.txt";

// Inițializare fișiere dacă nu există
if (!File.Exists(fisierMasini)) File.Create(fisierMasini).Close();
if (!File.Exists(fisierInchirieri)) File.Create(fisierInchirieri).Close();

while (true)
{
    Console.WriteLine("\n==== RentMaster - Birou Închirieri ====");
    Console.WriteLine("1. Afișează toate mașinile");
    Console.WriteLine("2. Adaugă mașină în flotă");
    Console.WriteLine("3. Închiriază o mașină (Client Nou)");
    Console.WriteLine("4. Ieșire");
    Console.Write("Selectați opțiunea: ");

    var optiune = Console.ReadLine();

    if (optiune == "1")
    {
        Console.WriteLine("\n--- LISTĂ COMPLETĂ MAȘINI ---");
        var linii = File.ReadAllLines(fisierMasini);
        if (linii.Length == 0) Console.WriteLine("Flota este goală.");
        foreach (var linie in linii)
        {
            var d = linie.Split(';');
            string status = d[4] == "True" ? "DISPONIBILĂ" : "ÎNCHIRIATĂ";
            Console.WriteLine($"ID: {d[0]} | {d[1]} {d[2]} | Nr: {d[3]} | Status: {status}");
        }
    }
    else if (optiune == "2")
    {
        Console.WriteLine("\n--- ADĂUGARE MAȘINĂ NOUĂ ---");
        Console.Write("Marcă: "); string marca = Console.ReadLine() ?? "";
        Console.Write("Model: "); string model = Console.ReadLine() ?? "";
        Console.Write("Nr. Înmatriculare: "); string nr = Console.ReadLine() ?? "";

        int id = File.ReadAllLines(fisierMasini).Length + 1;
        string linieNoua = $"{id};{marca};{model};{nr};True";
        
        File.AppendAllLines(fisierMasini, new[] { linieNoua });
        Console.WriteLine("✅ Mașina a fost adăugată cu succes!");
    }
    else if (optiune == "3")
    {
        Console.WriteLine("\n--- PROCES ÎNCHIRIERE ---");
        Console.Write("Introduceți ID-ul mașinii: ");
        string idCautat = Console.ReadLine() ?? "";

        var liniiMasini = File.ReadAllLines(fisierMasini).ToList();
        int indexMasina = -1;

        for (int i = 0; i < liniiMasini.Count; i++)
        {
            var date = liniiMasini[i].Split(';');
            if (date[0] == idCautat && date[4] == "True")
            {
                indexMasina = i;
                break;
            }
        }

        if (indexMasina != -1)
        {
            Console.Write("Nume Client: "); string nume = Console.ReadLine() ?? "";
            Console.Write("Prenume Client: "); string prenume = Console.ReadLine() ?? "";
            Console.Write("CNP: "); string cnp = Console.ReadLine() ?? "";
            Console.Write("Data Start (zz.ll.aaaa): "); string start = Console.ReadLine() ?? "";
            Console.Write("Data Final (zz.ll.aaaa): "); string final = Console.ReadLine() ?? "";

            // Salvăm închirierea
            string infoInchiriere = $"{idCautat};{nume};{prenume};{cnp};{start};{final}";
            File.AppendAllLines(fisierInchirieri, new[] { infoInchiriere });

            // Marcăm mașina ca indisponibilă (False)
            var m = liniiMasini[indexMasina].Split(';');
            liniiMasini[indexMasina] = $"{m[0]};{m[1]};{m[2]};{m[3]};False";
            File.WriteAllLines(fisierMasini, liniiMasini);

            Console.WriteLine("✅ Închiriere salvată! Mașina nu mai este disponibilă.");
        }
        else
        {
            Console.WriteLine("❌ Eroare: Mașina nu a fost găsită sau este deja dată.");
        }
    }
    else if (optiune == "4")
    {
        Console.WriteLine("La revedere!");
        break;
    }
}