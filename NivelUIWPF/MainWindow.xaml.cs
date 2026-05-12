using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LibrarieModele;
using NivelStocareDate;

namespace NivelUIWPF
{
    public partial class MainWindow : Window
    {
        private readonly IStocareMasini adminMasini;

        public MainWindow()
        {
            InitializeComponent();

            adminMasini = new AdministrareMasiniFisierText("Masini.txt");

            AfiseazaToateMasinile();
        }

        private void BtnAdauga_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMarca.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtNumar.Text) ||
                string.IsNullOrWhiteSpace(txtAn.Text) ||
                string.IsNullOrWhiteSpace(txtPret.Text))
            {
                MessageBox.Show("Completează toate câmpurile.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtAn.Text, out int anFabricatie))
            {
                MessageBox.Show("Anul fabricației trebuie să fie număr întreg.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPret.Text, out decimal pretPeZi))
            {
                MessageBox.Show("Prețul pe zi trebuie să fie număr.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Masina masina = new Masina
            {
                Marca = txtMarca.Text,
                Model = txtModel.Text,
                NumarInmatriculare = txtNumar.Text,
                AnFabricatie = anFabricatie,
                PretPeZi = pretPeZi,
                Disponibila = true
            };

            adminMasini.AddMasina(masina);

            CurataCampuri();
            AfiseazaToateMasinile();

            lblStatus.Content = "Status: mașina a fost adăugată cu succes.";
        }

        private void BtnAfiseazaToate_Click(object sender, RoutedEventArgs e)
        {
            AfiseazaToateMasinile();
        }

        private void BtnAfiseazaDisponibile_Click(object sender, RoutedEventArgs e)
        {
            List<Masina> masiniDisponibile = adminMasini.GetMasini()
                .Where(m => m.Disponibila)
                .ToList();

            AfiseazaLista(masiniDisponibile);

            lblTitluLista.Content = "Mașini disponibile";
            lblStatus.Content = $"Status: {masiniDisponibile.Count} mașini disponibile.";
        }

        private void AfiseazaToateMasinile()
        {
            List<Masina> masini = adminMasini.GetMasini();

            AfiseazaLista(masini);

            lblTitluLista.Content = "Lista tuturor mașinilor";
            lblStatus.Content = $"Status: {masini.Count} mașini salvate.";
        }

        private void AfiseazaLista(List<Masina> masini)
        {
            lstMasini.Items.Clear();

            if (masini.Count == 0)
            {
                lstMasini.Items.Add("Nu există mașini de afișat.");
                return;
            }

            foreach (Masina masina in masini)
            {
                lstMasini.Items.Add(masina.Info());
            }
        }

        private void CurataCampuri()
        {
            txtMarca.Clear();
            txtModel.Clear();
            txtNumar.Clear();
            txtAn.Clear();
            txtPret.Clear();
        }
    }
}