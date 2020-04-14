using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Water_Polo_Statbook.MainWindow;

namespace Water_Polo_Statbook
{
    /// <summary>
    /// Interaction logic for AddTeamWindow.xaml
    /// </summary>
    public partial class AddTeamWindow : Window
    { 

        // Main window reference
        private MainWindow mainWindow;

        // MySql connection reference 
        private MySqlQueryBuilder build;

        // myteam reference 
        private MyTeam myTeam;

        // Message constants
        private const string YEAR_FORMAT_MSG = "Ensure year is formatted: yyyy";
        private const string SELECT_RECENT_TEAM_QRY = "select * from team order by id desc limit 1";
        private const string INSERT_TEAM_QRY = "insert into team values ('{0}','{1}', NULL)";
        private const string FILL_TEXT_MSG = "Ensure all textboxes are filled";

        public AddTeamWindow(MainWindow mainWindow, MyTeam myTeam, MySqlQueryBuilder build)
        {
            this.mainWindow = mainWindow;
            this.myTeam = myTeam;
            this.build = build;
            InitializeComponent();
        }

        /// <summary>
        /// takes user back to home window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// creates team 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateBTN_Click(object sender, RoutedEventArgs e)
        {
            Create_Team();
        }

        /// <summary>
        /// cleara textboxes when user clicks on it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TeamYearTB1_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = "";
            tb.Foreground = Brushes.Black;
        }

        private void TeamYearTB2_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = "";
            tb.Foreground = Brushes.Black;
        }

        /* ------------- HELPER METHODS ------------ */
        /// <summary>
        /// fills myteam with attributes and stats and inserts into datatable
        /// navigates user to team editor of new team
        /// </summary>
        private void Create_Team()
        {
            if (Is_TB_Empty())
            {
                MessageBox.Show(FILL_TEXT_MSG);
                return;
            }

            // get teamname
            string teamName = TeamNameTB.Text.ToString();

            // ensure team year is formatted correctly then parse
            if (TeamYearTB1.Text.Length == 4 && TeamYearTB2.Text.Length == 4)
            {
                // format team year string
                string teamYear = TeamYearTB1.Text.ToString() + "-" + TeamYearTB2.Text.ToString();

                // add team to database
                AddTeam(teamName, teamYear);

                // get team info from data table
                string qry = string.Format(SELECT_RECENT_TEAM_QRY);

                // set team attributes from dataset
                myTeam.SetAttributes(build.Execute_DataSet_Query(qry));

                // navigate user to team editor of new team
                EditTeamWindow etw = new EditTeamWindow(mainWindow, myTeam, mainWindow.GetMyPlayer(), build);
                etw.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(YEAR_FORMAT_MSG);
            }
        }

        /// <summary>
        /// Inserts team into the team database
        /// </summary>
        private void AddTeam(string teamName, string teamYear)
        {
            // add team to database
            string qry = string.Format(INSERT_TEAM_QRY, teamName, teamYear);
            build.Execute_Query(qry);
        }

        /// <summary>
        /// checks if any textboxes are left empty
        /// </summary>
        /// <returns></returns>
        private bool Is_TB_Empty()
        {
            if (TeamNameTB.Text.ToString() == "" ||
                TeamYearTB1.Text.ToString() == "" ||
                TeamYearTB2.Text.ToString() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
