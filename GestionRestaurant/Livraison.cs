using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace GestionRestaurant
{
    public partial class Livraison : UserControl
    {
        private const string ConnectionString = "Server=localhost;Database=restaurants;Uid=zandry;Pwd=admin;";

        public Livraison()
        {
            InitializeComponent();
            LoadCommandeData();
        }

        public void LoadCommandeData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    string query = "SELECT commande.id_commande, commande.date_commande as 'Date', plat.Nom AS plat_nom, tables.nom AS table_nom, commande.quantite, commande.clt " +
                                   "FROM commande " +
                                   "INNER JOIN plat ON commande.id_plat = plat.ID_Plat " +
                                   "INNER JOIN tables ON commande.id_table = tables.id_table";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des données de commande : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gunaCircleButton1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedRows[0].Index;

                int selectedCommandeId = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["id_commande"].Value);
                string selectedPlatNom = dataGridView1.Rows[rowIndex].Cells["plat_nom"].Value.ToString();
                int quantite = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["quantite"].Value);

                DiminuerQuantitePlat(selectedPlatNom, quantite);
                EnregistrerFacture(selectedPlatNom, quantite);
                SupprimerCommandeLivree(selectedCommandeId);

                AfficherFactureDansLabels(selectedPlatNom, quantite);

                MessageBox.Show("Commande a été livrée, facturée et supprimée");
                LoadCommandeData();
            }
        }

        private void DiminuerQuantitePlat(string platNom, int quantite)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    string query = "UPDATE plat SET nombre = nombre - @quantite WHERE Nom = @platNom";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@quantite", quantite);
                    command.Parameters.AddWithValue("@platNom", platNom);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour de la quantité du plat : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnregistrerFacture(string platNom, int quantite)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    string query = "INSERT INTO factures (Nom, Plat, Nombre, Somme, Date) " +
                                   "VALUES (@nom, @plat, @nombre, @somme, @date)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nom", dataGridView1.SelectedRows[0].Cells["clt"].Value.ToString());
                    command.Parameters.AddWithValue("@plat", platNom);
                    command.Parameters.AddWithValue("@nombre", quantite);
                    command.Parameters.AddWithValue("@somme", quantite * GetPlatPrix(platNom));
                    command.Parameters.AddWithValue("@date", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement de la facture : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal GetPlatPrix(string platNom)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = "SELECT prix FROM plat WHERE Nom = @platNom";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@platNom", platNom);

                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToDecimal(result);
                }
                return 0;
            }
        }

        private void AfficherFactureDansLabels(string platNom, int quantite)
        {
            lblnom.Text = dataGridView1.SelectedRows[0].Cells["clt"].Value.ToString();
            lblplat.Text = platNom;
            lblnombre.Text = quantite.ToString();
            lblsomme.Text = (quantite * GetPlatPrix(platNom)).ToString();
            lbldate.Text = DateTime.Now.ToString();
        }

        private void SupprimerCommandeLivree(int idCommande)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    string query = "DELETE FROM commande WHERE id_commande = @idCommande";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idCommande", idCommande);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la suppression de la commande livrée : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gunaCircleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                string contenu = $"Nom : {lblnom.Text}\r\n" +
                                 $"Plat : {lblplat.Text}\r\n" +
                                 $"Nombre : {lblnombre.Text}\r\n" +
                                 $"Somme : {lblsomme.Text}\r\n" +
                                 $"Date : {lbldate.Text}\r\n";

                string fichierTemporaire = System.IO.Path.GetTempFileName() + ".txt";

                System.IO.File.WriteAllText(fichierTemporaire, contenu);
                System.Diagnostics.Process.Start("notepad.exe", fichierTemporaire);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ouverture du fichier : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Livraison_Load(object sender, EventArgs e)
        {
            LoadCommandeData();
        }
    }
}
