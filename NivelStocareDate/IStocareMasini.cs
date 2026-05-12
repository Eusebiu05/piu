using LibrarieModele;

namespace NivelStocareDate
{
    public interface IStocareMasini
    {
        void AddMasina(Masina masina);
        List<Masina> GetMasini();
    }
}