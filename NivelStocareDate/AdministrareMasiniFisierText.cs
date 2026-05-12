using LibrarieModele;

namespace NivelStocareDate
{
    public class AdministrareMasiniFisierText : IStocareMasini
    {
        private const int ID_PRIMA_MASINA = 1;
        private const int INCREMENT = 1;

        private readonly string numeFisier;

        public AdministrareMasiniFisierText(string numeFisier)
        {
            this.numeFisier = numeFisier;

            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AddMasina(Masina masina)
        {
            masina.IdMasina = GetNextIdMasina();

            using (StreamWriter sw = new StreamWriter(numeFisier, true))
            {
                sw.WriteLine(masina.ConversieLaSirPentruFisier());
            }
        }

        public List<Masina> GetMasini()
        {
            List<Masina> masini = new List<Masina>();

            using (StreamReader sr = new StreamReader(numeFisier))
            {
                string? linieFisier;

                while ((linieFisier = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(linieFisier))
                    {
                        masini.Add(new Masina(linieFisier));
                    }
                }
            }

            return masini;
        }

        public bool ModificaMasina(Masina masinaActualizata)
        {
            List<Masina> masini = GetMasini();

            int index = masini.FindIndex(m => m.IdMasina == masinaActualizata.IdMasina);

            if (index == -1)
            {
                return false;
            }

            masinaActualizata.DataActualizare = DateTime.Today;
            masini[index] = masinaActualizata;

            using (StreamWriter sw = new StreamWriter(numeFisier, false))
            {
                foreach (Masina masina in masini)
                {
                    sw.WriteLine(masina.ConversieLaSirPentruFisier());
                }
            }

            return true;
        }

        private int GetNextIdMasina()
        {
            List<Masina> masini = GetMasini();

            if (masini.Count == 0)
            {
                return ID_PRIMA_MASINA;
            }

            return masini.Max(m => m.IdMasina) + INCREMENT;
        }
    }
}