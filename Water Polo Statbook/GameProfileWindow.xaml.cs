using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Water_Polo_Statbook
{
    /// <summary>
    /// Interaction logic for GameProfileWindow.xaml
    /// </summary>
    public partial class GameProfileWindow : Window
    {

        // reference to calling window
        private Window callingWindow;
        // id of game
        private int gameId;
        // reference to mysql connection
        private MySqlConnection con;

        public GameProfileWindow(Window callingWindow, int gameId, MySqlConnection con)
        {
            this.callingWindow = callingWindow;
            this.gameId = gameId;
            this.con = con;
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }

        private void AddStatBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveStatBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            callingWindow.Show();
            this.Close();
        }
    }
}
