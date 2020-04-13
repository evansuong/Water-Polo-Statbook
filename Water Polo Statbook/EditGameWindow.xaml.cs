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
    /// Interaction logic for EditGameWindow.xaml
    /// </summary>
    public partial class EditGameWindow : Window
    {

        // reference to calling window
        private Window callingWindow;

        // reference to mygame object
        private MyGame myGame;

        // reference to mysqlquerybuilder
        private MySqlQueryBuilder build;

        // message constants
        private const string DLT_MSG = "Are you sure you want to leave before updating";
        private const string DATE_FMT = "{0}-{1}-{2}";


        // mysql queries
        private const string SELECT_GAME_QRY = "select opp_team, game_type, game_location, date_format(game_date, '%M-%D-%Y') as game_date, game_result, team_id, id from game order by id desc limit 1";
        private const string UPDATE_GAME_QRY = "update game set opp_team='{0}', game_type='{1}', game_location='{2}', game_date='{3}' where id={4}";

        public EditGameWindow(Window callingWindow, MyGame myGame, MySqlQueryBuilder build)
        {
            this.callingWindow = callingWindow;
            this.myGame = myGame;
            this.build = build;
            InitializeComponent();

            init();
        }

        private void init()
        {
            OppNameTB.Text = myGame.GetName();
            YearTB.Text = myGame.GetDate();
            DayCB.Text = myGame.GetDate();
            MonthCB.Text = myGame.GetDate();
            GameTypeCB.Text = myGame.GetType().ToString();
            GameLocCB.Text = myGame.GetLoc();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            // prompt user if they want to leave the window 
            MessageBoxResult result = MessageBox.Show("", DLT_MSG, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                callingWindow.Show();
                this.Close();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// clears textbox when user presses to type in it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YearTB_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = "";
        }


        /// <summary>
        /// updates information in database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            Update_Game();

            callingWindow.Show();
            this.Close();
        }

        /// <summary>
        /// updates information in database
        /// </summary>
       private void Update_Game()
        {
            // get game info from window uesr input
            string oppTeam = OppNameTB.Text.ToString();
            string gameType = GameTypeCB.Text.ToString();
            string gameLoc = GameLocCB.Text.ToString();
            string gameDate = string.Format(DATE_FMT, YearTB.Text.ToString(), MonthCB.Text.ToString(), DayCB.Text.ToString());

            // insert new game into game datatable
            string insqry = string.Format(UPDATE_GAME_QRY, oppTeam, gameType, gameLoc, gameDate, myGame.GetId());
            build.Execute_Query(insqry);

            // select inserted game from datatable
            myGame.init();
            myGame.Set_Attributes(build.Execute_DataSet_Query(SELECT_GAME_QRY));
        }
    }
}
