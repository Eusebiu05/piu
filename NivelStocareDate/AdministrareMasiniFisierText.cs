using LibrarieModele;

namespace NivelStocareDate
{
    public class AdministrareMasiniFisierText : IStocareMasini
    {
        private const int ID_PRIMA_MASINA = 1;
        private const int INCREMENT = 1;

        private string numeFisier;

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
                string linieFisier;

                while ((linieFisier = sr.ReadLine()) != null)
                {
                    masini.Add(new Masina(linieFisier));
                }
            }

            return masini;
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