﻿using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Npgsql;



namespace RetrieveBox {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        string sql = "SELECT name, phone, mail, job, centre FROM phone_number";

        public MainWindow() {
            InitializeComponent();
            StartQuery();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            string userInput = textBox.Text;
            Console.WriteLine(userInput);

            if (String.IsNullOrEmpty(textBox.Text)) {
                ds.Reset();
                sql = "SELECT name, phone, mail, job, centre FROM phone_number";
                StartQuery();
            } else {
                sql = "SELECT name, phone, mail, job, centre FROM phone_number WHERE name ILIKE '%" + userInput + "%' OR phone ILIKE '%" + userInput +
                    "%' OR centre ILIKE '%" + userInput + "%'";
                StartQuery();
            }
        }

        private void StartQuery() {
            try {
                // PostgeSQL-style connection string
                string connstring = String.Format("Server=130.15.0.3;Port=5432;" +
                    "User Id=c#_user;Password=user;Database=phone;");
                // Making connection with Npgsql provider
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since it C# DataSet can handle multiple tables, we will select first
                dt = ds.Tables[0];
                gridView.ItemsSource = dt.DefaultView;

                conn.Close();
            } catch (Exception msg) {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        private void Textbox_Loaded(object sender, RoutedEventArgs e) {
            textBox.Focus();
        }

        private new void MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataRowView selectedRow = gridView.SelectedItem as DataRowView;
            string name = selectedRow["name"].ToString();
            string invertedName = String.Join(" ", name.Split(' ').Reverse().ToArray());
            string email = selectedRow["mail"].ToString();
            string phone = selectedRow["phone"].ToString();
            string job = selectedRow["job"].ToString();
            string centre = selectedRow["centre"].ToString();
            Console.WriteLine("DoubleClick triggered on e-mail :" + email);

            MessageBoxResult result = MessageBox.Show(
                "NOM: " + invertedName + "\n" +
                "FONCTION: " + job + "\n" +
                "CENTRE: " + centre + "\n" +
                "TELEPHONE: " + phone + "\n" +
                "MAIL: " + email + "\n" +
                " \n Écrire un mail à " + invertedName + " ?\n", "Fiche de " + invertedName, MessageBoxButton.YesNo
                );

            switch (result) {
                case MessageBoxResult.Yes:
                    Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
                    Microsoft.Office.Interop.Outlook.MailItem oMsg = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

                    oMsg.Recipients.Add(email);
                    oMsg.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
                    oMsg.HTMLBody = "";
                    oMsg.Display(true);
                    break;
                case MessageBoxResult.No:
                    // Nothing
                    break;
                default:
                    // Nothing
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("     INFO\n" +
                "     ---------\n\n" +
                "La recherche peut s'effectuer via les critères suivants : \n" +
                "   - NOM / PRÉNOM / NUMÉRO DE TÉLEPHONE / CENTRE \n" +
                "Double-cliquez sur un nom pour ouvrir la fiche contact détaillée ou pour envoyer un e-mail à la personne sélectionnée.");
        }
    }
}
