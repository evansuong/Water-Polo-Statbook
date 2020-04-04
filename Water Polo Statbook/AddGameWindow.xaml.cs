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
    /// Interaction logic for AddGameWindow.xaml
    /// </summary>
    public partial class AddGameWindow : Window
    {

        //reference to calling window
        private Window callingWindow;
        //reference to myteam
        private MyTeam myTeam;
        //reference to mysql connection
        private MySqlConnection con;

        // string constatns
        private const string DATE_FMT = "{0}-{1}-{2}";
        private const string INSERT_GAME_QRY = "insert into game values ('{0}', '{1}', '{2}', '{3}', 'T', {4}, NULL)";

        public AddGameWindow(Window callingWindow, MyTeam myTeam, MySqlConnection con)
        {
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.con = con;

            InitializeComponent();
        }

        /// <summary>
        /// creates game and takes user to game statsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateBTN_Click(object sender, RoutedEventArgs e)
        {
            Create_Game();
            GameProfileWindow gpw = new GameProfileWindow(callingWindow, 0, con);
            gpw.Show();
            this.Close();
        }

        /// <summary>
        /// returns user to team main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            callingWindow.Show();
            this.Close();
        }

        /// <summary>
        /// inserts new game into game datatable 
        /// </summary>
        private void Create_Game()
        {
            // get game info from window uesr input
            string oppTeam = OppNameTB.Text.ToString();
            string gameType = GameTypeCB.Text.ToString();
            string gameLoc = GameLocCB.Text.ToString();
            string gameDate = string.Format(DATE_FMT, YearTB.Text.ToString(), MonthCB.Text.ToString(), DayCB.Text.ToString());

            try
            {
                // insert new game into game datatable
                string qry = string.Format(INSERT_GAME_QRY, oppTeam, gameType, gameLoc, gameDate, myTeam.GetId());
                con.Open();

                MySqlCommand msc = new MySqlCommand(qry, con);
                msc.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (con.State == System.Data.ConnectionState.Open)
                con.Close();
        }
    }
}
