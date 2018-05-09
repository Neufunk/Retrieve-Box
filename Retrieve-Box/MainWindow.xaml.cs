﻿using System;
using System.Collections.Generic;
using System.Data;
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
using Npgsql;

namespace RetrieveBox {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        string sql = "SELECT name, phone FROM phone_number";

        public MainWindow() {
            InitializeComponent();
            StartQuery();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            string userInput = textBox.Text;
            Console.WriteLine(userInput);
            if (String.IsNullOrEmpty(textBox.Text)) {
                ds.Reset();
                sql = "SELECT name, phone FROM phone_number";
                StartQuery();
            } else {
                sql = "SELECT name, phone FROM phone_number WHERE name ILIKE '%" + userInput + "%'";
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
    }
}
