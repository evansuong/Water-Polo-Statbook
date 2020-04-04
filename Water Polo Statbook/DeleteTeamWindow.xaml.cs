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
    /// Interaction logic for DeleteTeamWindow.xaml
    /// </summary>
    public partial class DeleteTeamWindow : Window
    {

        // reference to calling window
        private Window callingWindow;
        // reference to main window
        private Window mainWindow;

        // reference to myTeam
        private MyTeam myTeam;

        // reference to mysqlconnection
        MySqlConnection con;

        // qry constant
        private const string DELETE_TEAM_QRY = "delete from team where id={0}";
        private const string DISABLE_FOREIGN_KEy_QRY = "set FOREIGN_KEY_CHECKS = 0";


        public DeleteTeamWindow(Window callingWindow, Window mainWindow, MyTeam myTeam, MySqlConnection con)
        {
            this.callingWindow = callingWindow;
            this.mainWindow = mainWindow;
            this.myTeam = myTeam;
            this.con = con;
            InitializeComponent();
        }

        /// <summary>
        /// deletes team from team table and takes user back to home window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YesBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // delete team from team table
                con.Open();

                string qry1 = DISABLE_FOREIGN_KEy_QRY;
                MySqlCommand msc = new MySqlCommand(qry1, con);
                msc.ExecuteNonQuery();

                string qry2 = string.Format(DELETE_TEAM_QRY, myTeam.GetId());
                msc.CommandText = qry2;
                msc.ExecuteNonQuery();
                con.Close();


                // take user back to main window
                mainWindow.Show();
                callingWindow.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (con.State == System.Data.ConnectionState.Open)
                con.Close();
        }

        /// <summary>
        /// takes user back to calling window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoBTN_Click(object sender, RoutedEventArgs e)
        {
            // take user back to calling window
            callingWindow.Show();
            this.Close();
        }
    }
}
