using System;
using System.Collections.Generic;
using System.Linq;
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

        // string formatting
        private const string DATE_FMT = "{0}-{1}-{2}";

        // mysql queries
        private const string SELECT_GAME_QRY = "select opp_team, game_type, game_location, date_format(game_date, '%M-%D-%Y') as game_date, game_result, team_id, id, total_gol, opp_total_gol from game order by id desc limit 1";
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
            Dictionary<string, string> months = new Dictionary<string, string>();

            months.Add("January", "01");
            months.Add("February", "02");
            months.Add("March", "03");
            months.Add("April", "04");
            months.Add("May", "05");
            months.Add("June", "06");
            months.Add("July", "07");
            months.Add("August", "08");
            months.Add("September", "09");
            months.Add("October", "10");
            months.Add("November", "11");
            months.Add("December", "12");

            OppNameTB.Text = myGame.GetName();

            // reformat date 
            string date = myGame.GetDate();
            //parse year
            string year = date.Substring(date.IndexOf("-") + 5, 4);
            YearTB.Text = year;
            // parse day
            string day = string.Empty;
            foreach (char c in date.Substring(date.IndexOf("-") + 1, 3))
            {
                if (char.IsDigit(c))
                    day += c;
            }
            if (day.Length < 2)
                day = "0" + day;
            DayCB.Text = day;

            // parse month
            string month = months[date.Substring(0, date.IndexOf("-"))];
            MonthCB.Text = month;

            // set game type
            GameTypeCB.Text = myGame.GetGameType().ToString();

            // reformat and set game location 
            string loc = myGame.GetLoc().Substring(1, 1);
            MessageBox.Show(loc);
            GameLocCB.Text = loc;

        }

        /// <summary>
        /// sends user back to team main window after prompt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            tb.Foreground = Brushes.Black;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
