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
        private MySqlQueryBuilder build;

        // string to hold radiobutton type
        char statType;

        // message error constants
        private const string SELECT_QTR_MSG = "please select a quarter";
        private const string SELECT_PLAYER_MSG = "please select a player";
        private const string SELECT_TYPE_MSG = "please select a stat type";


        // query commands
        private const string SELECT_GOALS_QRY = "select sum(game_stats.total_gol) as total_gol, game_stats_opponent.total_gol as opp_total_gol from game_stats inner join game_stats_opponent on game_stats.game_id = game_stats_opponent.game_id where game_stats.game_id={0}";
        private const string SELECT_OPP_STATS_QRY = "select * from game_stats_opponent where game_id={0}";
        private const string SELECT_GAMEHIST_QRY = "select * from game_history where event_game_id={0}";
        private const string SELECT_PLAYER_NUMS_QRY = "select player_num from player where team_id={0}";
        private const string SELECT_PLAYER_ID_QRY = "select id from player where player_num={0} and team_id={1}";
        private const string SELECT_PLAYER_GAME_STATS_QRY = "select player_num, player_name, total_gol, total_att, total_ast, total_blk, total_stl, total_exl, total_tov, q1_gol, q2_gol, q3_gol, q4_gol, ot_gol, game_id from player inner join game_stats on player.id = game_stats.player_id where game_stats.game_id={0} order by player_num";
        private const string INSERT_STAT_QRY = "insert into game_history values ({0}, '{1}', {2}, {3}, {4}, NULL)";
        private const string UPDATE_GAME_QRY = "update game set game_result='{0}', total_gol={1}, opp_total_gol={2} where id={3}";
        private const string DELETE_STAT_QRY = "delete from game where id={0}";

        public GameProfileWindow(Window callingWindow, MyTeam myTeam, MyGame myGame, MySqlQueryBuilder build)
        {
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.myGame = myGame;
            this.build = build;
            InitializeComponent();
            init();
        }

        private void init()
        {
            AddStatPNL.Visibility = Visibility.Hidden;

            Set_Game_Stats();
            Load_Opp_Table();
            Update_Tables();
            Set_Labels();
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
            Delete_Stat();
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
                Add_Stat();
            }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Update_Team_Stats();
            Set_Game_Stats();
            callingWindow.Show();
            this.Close();
        }


        /* ----------------- HELPER METHODS ------------------- */


        /// <summary>
        /// adds user specified stat to game and to all team and player stat totals
        /// </summary>
        private void Add_Stat()
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

            // deafult player id
            int playerId;

            // parse player number from combobox
            int playerNum;

            // if player is opponent, dont parse player value
            if (PlayerCB.Text.ToString() == "Opponent")
            {
                statType = 'O';
                playerNum = Int32.Parse(PlayerCB.Items[0].ToString());
            }

            // if home team stat, parse player id from player number 
            else
            {
                statType = Set_Stat_Type();
                playerNum = Int32.Parse(PlayerCB.Text.ToString());
            }

            // get player id from player number
            string selqry = string.Format(SELECT_PLAYER_ID_QRY, playerNum, myTeam.GetId());

            DataTable dt = build.Execute_DataTable_Qry(selqry);
            playerId = Int32.Parse(dt.Rows[0]["id"].ToString());

            // insert new game stat into game history and watch the magic happen
            string insqry = string.Format(INSERT_STAT_QRY, qtr, this.statType, playerId, myGame.GetId(), myTeam.GetId());
            build.Execute_Query(insqry);

            // update all tables
            Set_Game_Stats();
            Update_Tables();

            AddStatPNL.Visibility = Visibility.Hidden;
            this.UpdateLayout();
        }


        /// <summary>
        /// deletes selected stat
        /// </summary>
        private void Delete_Stat()
        {
            // get stat id from game history dg
            if (GameHistDG.SelectedItems.Count == 1)
            {
                var items = GameHistDG.SelectedItems;
                foreach (DataRowView item in items)
                {
                    int id = Int32.Parse(item["id"].ToString());

                    // delete stat that matches stat id
                    string qry = string.Format(DELETE_STAT_QRY, id);
                    build.Execute_Query(qry);
                }
                // refresh page
                Load_Opp_Table();
                Update_Tables();
            }
        }

        /// <summary>
        /// sets home and away goals of game upon returning to team main window
        /// </summary>
        private void Set_Game_Stats()
        {// TODO update game goals when user leaves the screen
            // TODO update score in this window when a goal is scoredn
            // set home and opponent goals of mygame
            string selqry = string.Format(SELECT_GOALS_QRY, myGame.GetId());

            // fill game stats
            myGame.Set_Stats(build.Execute_DataSet_Query(selqry));
        }

        /// <summary>
        /// formats stat type to be passed into datatable
        /// </summary>
        /// <param name="statType"></param>
        /// <returns></returns>
        private char Set_Stat_Type()
        {
            if ((bool)GolBTN.IsChecked)
                return 'G'; 
            else if ((bool)AttBTN.IsChecked)
                return 'M'; 
            else if ((bool)AstBTN.IsChecked)
                return 'A'; 
            else if ((bool)BlkBTN.IsChecked)
                return 'B'; 
            else if ((bool)StlBTN.IsChecked)
                return 'S';
            else if ((bool)ExlBTN.IsChecked)
                return 'E';
            else
                return 'T';
        }

        /// <summary>
        /// udpates game result and score in game table
        /// </summary>
        private void Update_Team_Stats()
        {
            // check whether game was a win, loss, or tie
            int homeGoals = myGame.GetHomeGoals();
            int oppGoals = myGame.GetOppGoals();
            char result = 'T';

            if (homeGoals > oppGoals)
            {
                result = 'W';
            }
            else if (homeGoals < oppGoals)
            {
                result = 'L';
            }

            // update game result in database
            string qry = string.Format(UPDATE_GAME_QRY, result, homeGoals, oppGoals, myGame.GetId());
            build.Execute_Query(qry);
        }

        private void Load_Team_Table()
        {
            // join player and game_stats tables and load into main data table
            string qry = string.Format(SELECT_PLAYER_GAME_STATS_QRY, myGame.GetId());
            HomeDG.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }

        private void Load_Opp_Table()
        {
            // get all data grom game_stats_opponent for this game
            string qry = string.Format(SELECT_OPP_STATS_QRY, myGame.GetId());
            OppDG.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }

        private void Load_Game_History_Table()
        {
            // get all data grom game_stats_opponent for this game
            string qry = string.Format(SELECT_GAMEHIST_QRY, myGame.GetId());
            GameHistDG.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }

        private void Load_Players()
        {
            // clear player cb
            PlayerCB.Items.Clear();

            // find team player names 
            string qry = string.Format(SELECT_PLAYER_NUMS_QRY, myGame.GetTeamId());
            DataTable dt = build.Execute_DataTable_Qry(qry);

            // load all player names into combobox
            foreach (DataRow row in dt.Rows)
            {
                PlayerCB.Items.Add(row["player_num"].ToString());
            }

            // add opponent into combobox 
            PlayerCB.Items.Add("Opponent");
        }

        /// <summary>
        /// set all label data context
        /// </summary>
        private void Set_Labels()
        {
            // give value to all properties in model object
            MyModelObject ControlDC = new MyModelObject()
            {
                GameName = myGame.GetName(),
                GameType = myGame.GetGameType(),
                GameLoc = myGame.GetLoc(),
                GameDate = myGame.GetDate(),
                HomeGoals = myGame.GetHomeGoals().ToString(),
                OppGoals = myGame.GetOppGoals().ToString()
            };

            // set all data context bindings for labels in team main window
            NameLBL.DataContext = ControlDC;
            TypeLBL.DataContext = ControlDC;
            LocLBL.DataContext = ControlDC;
            DateLBL.DataContext = ControlDC;
            HomeGoalsLBL.DataContext = ControlDC;
            OppGoalsLBL.DataContext = ControlDC;
        }

        /// <summary>
        /// update content of all tables in window
        /// </summary>
        private void Update_Tables()
        {
            if (statType == 'O')
            {
                Load_Opp_Table();
            }
            else
            {
                Load_Team_Table();
            }
            Set_Labels();
            Load_Game_History_Table();
        }
    }
}