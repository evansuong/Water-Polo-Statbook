using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for GameProfileWindow.xaml
    /// </summary>
    public partial class GameProfileWindow : Window
    {

        // reference to calling window
        private Window callingWindow;
        // mygame reference
        private MyGame myGame;
        // myteam reference
        private MyTeam myTeam;
        // reference to mysql connection
        private MySqlConnection con;

        // string to hold radiobutton type
        string btnType;

        // message error constants
        private const string SELECT_QTR_MSG = "please select a quarter";
        private const string SELECT_PLAYER_MSG = "please select a player";
        private const string SELECT_TYPE_MSG = "please select a stat type";


        // query commands
        private const string SELECT_OPP_STATS_QRY = "select * from game_stats_opponent where game_id={0}";
        private const string SELECT_GAMEHIST_QRY = "select * from game_history where event_game_id={0}";
        private const string SELECT_PLAYER_NUMS_QRY = "select player_num from player where team_id={0}";
        private const string SELECT_PLAYER_ID_QRY = "select id from player where player_num={0} and team_id={1}";
        private const string SELECT_PLAYER_GAME_STATS_QRY = "select player_num, player_name, total_gol, total_att, total_ast, total_blk, total_stl, total_exl, total_tov, q1_gol, q2_gol, q3_gol, q4_gol, ot_gol, game_id from player inner join game_stats on player.id = game_stats.player_id where game_stats.game_id={0}";
        private const string INSERT_STAT_QRY = "insert into game_history values ({0}, '{1}', {2}, {3}, {4}, NULL)";

        public GameProfileWindow(Window callingWindow, MyTeam myTeam, MyGame myGame, MySqlConnection con)
        {
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.myGame = myGame;
            this.con = con;
            InitializeComponent();

            AddStatPNL.Visibility = Visibility.Hidden;

            Set_Labels();
            Update_Tables();
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }

        private void AddStatBTN_Click(object sender, RoutedEventArgs e)
        {
            Load_Players();
            QtrCB.Text = "";
            AddStatPNL.Visibility = Visibility.Visible;
            this.UpdateLayout();
        }

        private void RemoveStatBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StatBTN_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            btnType = btn.Content.ToString();

            switch (btnType)
            {
                case "Goal":
                    btnType = "G";
                    break;
                case "Attempt":
                    btnType = "M";
                    break;
                case "Assist":
                    btnType = "A";
                    break;
                case "Block":
                    btnType = "B";
                    break;
                case "Steal":
                    btnType = "S";
                    break;
                case "Exclusion":
                    btnType = "E";
                    break;
                case "Turnover":
                    btnType = "T";
                    break;
                case "Opponent Goal":
                    btnType = "O";
                    break;
            }
        }


        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            // ensure quater is selected
            if (QtrCB.Text == "")
            {
                MessageBox.Show(SELECT_QTR_MSG);
                return;
            }
            // ensure player is selected
            if (PlayerCB.Text == "")
            {
                MessageBox.Show(SELECT_PLAYER_MSG);
                return;
            }
            // ensure stat type is selected
            if (!Convert.ToBoolean(GolBTN.IsChecked) && !Convert.ToBoolean(AttBTN.IsChecked) && !Convert.ToBoolean(AstBTN.IsChecked) && !Convert.ToBoolean(BlkBTN.IsChecked) && !Convert.ToBoolean(StlBTN.IsChecked) && !Convert.ToBoolean(ExlBTN.IsChecked) && !Convert.ToBoolean(TovBTN.IsChecked))
            {
                MessageBox.Show(SELECT_TYPE_MSG);
                return;
            }
            else
            {
                // set deafult qtr values
                int qtr = 1;

                // if qtr = OT replace with 5
                if (QtrCB.Text.ToString() == "OT")
                {
                    qtr = 5;
                }
                else
                {
                    qtr = Int32.Parse(QtrCB.Text.ToString());
                }

                // parse player number from combobox
                int playerNum = Int32.Parse(PlayerCB.Text.ToString());

                // get player id from player number
                string playerIdQry = string.Format(SELECT_PLAYER_ID_QRY, playerNum, myTeam.GetId());
                con.Open();

                MySqlDataAdapter sda = new MySqlDataAdapter(playerIdQry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                int playerId = Int32.Parse(dt.Rows[0]["id"].ToString());

                // place btn type into stat type
                string statType = btnType;

                // insert new game stat into game history and watch the magic unfold
                string insertQry = string.Format(INSERT_STAT_QRY, qtr, statType, playerId, myGame.GetId(), myTeam.GetId());
                MySqlCommand msc = new MySqlCommand(insertQry, con);
                msc.ExecuteNonQuery();
                con.Close();

                // update all tables
                Update_Tables();

                AddStatPNL.Visibility = Visibility.Hidden;
                this.UpdateLayout();
            }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            callingWindow.Show();
            this.Close();
        }

        private void Load_Team_Table()
        {
            // join player and game_stats tables and load into main data table
            string qry = string.Format(SELECT_PLAYER_GAME_STATS_QRY, myGame.GetId());
            con.Open();

            MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();

            HomeDG.ItemsSource = dt.DefaultView;
        }

        private void Load_Opp_Table()
        {
            // get all data grom game_stats_opponent for this game
            string qry = string.Format(SELECT_OPP_STATS_QRY, myGame.GetId());
            con.Open();

            MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();

            OppDG.ItemsSource = dt.DefaultView;
        }

        private void Load_Game_History_Table()
        {
            // get all data grom game_stats_opponent for this game
            string qry = string.Format(SELECT_GAMEHIST_QRY, myGame.GetId());
            con.Open();

            MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();

            GameHistDG.ItemsSource = dt.DefaultView;
        }

        private void Load_Players()
        {
            // clear player cb
            PlayerCB.Items.Clear();

            // find team player names 
            string qry = string.Format(SELECT_PLAYER_NUMS_QRY, myGame.GetTeamId());
            con.Open();

            MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
            DataTable dt = new DataTable();
            con.Close();

            sda.Fill(dt);

            // load all player names into combobox
            foreach (DataRow row in dt.Rows)
            {
                PlayerCB.Items.Add(row["player_num"].ToString());
            }
        }

        private void Set_Labels()
        {
            // give value to all properties in model object
            MyModelObject ControlDC = new MyModelObject()
            {
                GameName = myGame.GetName(),
                GameType = myGame.GetGameType(),
                GameLoc = myGame.GetLoc(),
                GameDate = myGame.GetDate()
            };

            // set all data context bindings for labels in team main window
            NameLBL.DataContext = ControlDC;
            TypeLBL.DataContext = ControlDC;
            LocLBL.DataContext = ControlDC;
            DateLBL.DataContext = ControlDC;
        }

        private void Update_Tables()
        {
            Load_Team_Table();
            Load_Game_History_Table();
            Load_Opp_Table();
            Load_Players();
        }
    }
}