using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        //reference to mygame
        private MyGame myGame;
        //reference to mysql connection
        private MySqlQueryBuilder build;

        // string constatns
        private const string DATE_FMT = "{0}-{1}-{2}";
        private const string INSERT_GAME_QRY = "insert into game values ('{0}', '{1}', '{2}', '{3}', 'T', {4}, NULL)";
        private const string SELECT_GAME_QRY = "select opp_team, game_type, game_location, date_format(game_date, '%M-%D-%Y') as game_date, game_result, team_id, id from game order by id desc limit 1";

        public AddGameWindow(Window callingWindow, MyTeam myTeam, MyGame myGame, MySqlQueryBuilder build)
        {
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.myGame = myGame;
            this.build = build;

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
            GameProfileWindow gpw = new GameProfileWindow(callingWindow, myTeam, myGame, build);
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

            // insert new game into game datatable
            string insqry = string.Format(INSERT_GAME_QRY, oppTeam, gameType, gameLoc, gameDate, myTeam.GetId());
            build.Execute_Query(insqry);

            // select inserted game from datatable
            myGame.init();
            myGame.Set_Attributes(build.Execute_DataSet_Query(SELECT_GAME_QRY));
        }

        private void YearTB_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = "";
        }
    }
}
