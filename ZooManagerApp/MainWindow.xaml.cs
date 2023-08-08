using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZooManagerApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            
            InitializeComponent();
            ///Connection to Datenbank
            ///Add new reference (Verweis)
            /// Add using System.Configuration
            /// And in the End tipp this line=>>    string connectionString = ConfigurationManager.ConnectionStrings["ZooManagerApp.Properties.Settings.AmirSQLDBConnectionString"].ConnectionString;
            string connectionString = ConfigurationManager.ConnectionStrings["ZooManagerApp.Properties.Settings.AmirSQLDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            showZoos();
            showAnimals();
        }

        public void showZoos()
        {
            
            try
            {
                string query = "select * from Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    // Welche Informations der Tabelle in unserem DataTable soll in unsere Listbox angezeigt werden  
                    ListZoos.DisplayMemberPath = "Location";
                    // Welche wert soll gegeben werden wenn eines unsere Items von der Listbox ausgewält wird 
                    ListZoos.SelectedValuePath = "Id";

                    ListZoos.ItemsSource = zooTable.DefaultView;

                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }

        }

        public void showAnimals()
        {
            
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    // Welche Informations der Tabelle in unserem DataTable soll in unsere Listbox angezeigt werden  
                    ListAnimals.DisplayMemberPath = "Name";
                    // Welche wert soll gegeben werden wenn eines unsere Items von der Listbox ausgewält wird 
                    ListAnimals.SelectedValuePath = "Id";

                    ListAnimals.ItemsSource = animalTable.DefaultView;

                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }

        }

        public void showAssociatedAnimals()
        {
            /// if a Zoo is not selected dont show it !!!  
            if (ListZoos.SelectedValue == null)
            {
                return;
            }
            /// But if Zoo is selected show it !!!!
            try
            {
                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);

                    DataTable animalTabel = new DataTable();
                    sqlDataAdapter.Fill(animalTabel);

                    // Welche Informations der Tabelle in unserem DataTable soll in unsere Listbox angezeigt werden  
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    // Welche wert soll gegeben werden wenn eines unsere Items von der Listbox ausgewält wird 
                    listAssociatedAnimals.SelectedValuePath = "Id";

                    listAssociatedAnimals.ItemsSource = animalTabel.DefaultView;

                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }

        private void ListZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListZoos.SelectedValue == null)

            {

                return;

            }
            showAssociatedAnimals();
            ShowSelectedZooInTextBox();
        }

        private void ListAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListAnimals.SelectedValue == null)

            {

                return;

            }
            ShowSelectedAnimalInTextBox();
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);
                ///ExecuteScalar will do your Order or what you want to do  
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showZoos(); 
            }
        }

        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                ///ExecuteScalar will do your Order or what you want to do  
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showZoos();
            }  
            
        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            if(ListZoos.SelectedValue == null || ListAnimals.SelectedValue == null)
            { 
                return;
            } 
            try
            {
                string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", ListAnimals.SelectedValue);
                ///ExecuteScalar will do your Order or what you want to do  
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally 
            {
                sqlConnection.Close();
                showAssociatedAnimals();
            }

        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                ///ExecuteScalar will do your Order or what you want to do  
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showAnimals();
            }

        }

        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", ListAnimals.SelectedValue);
                ///ExecuteScalar will do your Order or what you want to do  
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showAnimals();
                showAssociatedAnimals();
                
            }
        }

        private void ShowSelectedZooInTextBox()
        {
            
            try
            {
                string query = "select location from Zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);
                    DataTable zooDataTable = new DataTable();
                    sqlDataAdapter.Fill(zooDataTable);
                    
                    myTextBox.Text = zooDataTable.Rows[0]["Location"].ToString();

                }
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowSelectedAnimalInTextBox()
        {
            try
            {
                string query = "select name from Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@AnimalId", ListAnimals.SelectedValue);
                    DataTable animalDataTable = new DataTable();
                    sqlDataAdapter.Fill(animalDataTable);

                    myTextBox.Text = animalDataTable.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            if(ListZoos.SelectedValue == null)
            {
                return;
            }  
            try
            {
                string query = "update Zoo Set Location = @Location where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception )
            {

                MessageBox.Show("dsfdsfdsf");
            }
            finally
            {
                sqlConnection.Close();
                showZoos();   
            }
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (ListAnimals.SelectedValue == null)
            {
                return;
            }
            try
            {
                string query = "update Animal Set name = @Name where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", ListAnimals.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception )
            {

                MessageBox.Show("sdfsd");
            }
            finally
            {
                sqlConnection.Close();
                showAnimals();
                showAssociatedAnimals();
            }
        }

        private void RemoveAnimalFromZoo(object sender, RoutedEventArgs e)
        {
            if (listAssociatedAnimals.SelectedValue == null)
                return;
            try
            {
                string query = "DELETE FROM ZooAnimal WHERE AnimalId = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAssociatedAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                showAssociatedAnimals();
            }

        }
    }
}
