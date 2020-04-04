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
    /// Interaction logic for TeamMainWindow.xaml
    /// </summary>
    public partial class TeamMainWindow : Window
    {

        // myteam object reference
        private MyTeam myTeam;
        // myplayer object reference
        private MyPlayer myPlayer;

        // reference to calling window
        private Window callingWindow;

        // mysql connection reference
        private MySqlConnection con;

        // constant strings
        private const string SCORE_FMT = "{0}-{1}";
        private const string SELECT_PLAYER_MSG = "Please select a player";
        private const string SELECT_GAME_MSG = "Please select a player";
        private const string SELECT_TEAM_STATS_QRY = "select * from team_stats where team_id={0}";
        private const string SELECT_PLAYERS_QRY = "select * from player where team_id={0}";
        private const string SELECT_PLAYER_ATTR_QRY = "select * from player where id={0}";
        private const string SELECT_PLAYER_STATS_QRY = "select * from player_stats where player_id={0}";
        private const string SELECT_PLAYER_ATT_STATS_QRY = "select * from player inner join player_stats on player.id = player_stats.player_id where team_id={0}"; 
        private const string SELECT_GAMES_QRY = "select game.opp_team, game.game_type, game.game_location, game.game_date, game.game_result, game.id, game_stats.total_gol, game_stats_opponent.total_gol as opp_total_gol from game inner join game_stats on game.id = game_stats.game_id inner join game_stats_opponent on game.id = game_stats_opponent.game_id where game.team_id={0}";


        public TeamMainWindow(Window callingWindow, MyTeam myTeam, MyPlayer myPlayer, MySqlConnection con)
        {
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.myPlayer = myPlayer;
            this.con = con;


            InitializeComponent();
            Set_Labels();
            Load_Player_Table_Data();
        }

        /// <summary>
        /// takes user to team editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTeamBTN_Click(object sender, RoutedEventArgs e)
        {
            Edit_Team();
        }

        /// <summary>
        /// deletes team from datatables and returns user to home screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTeamBTN_Click(object sender, RoutedEventArgs e)
        {
            Delete_Team();
        }

        /// <summary>
        /// returns user to home screen 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            callingWindow.Show();
            this.Close();
        }

        /// <summary>
        /// opens full stat table for current team
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullStatsBTN_Click(object sender, RoutedEventArgs e)
        {
            // allow stat bar to close when clocked away from
            TeamProfileWindow tpw = new TeamProfileWindow(this, myTeam, con);
            tpw.Show();
            tpw.Deactivated += (sender, args) => { tpw.Close(); };
        }

        /// <summary>
        /// takes user to statsheet of selected game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewGameBTN_Click(object sender, RoutedEventArgs e)
        {
            GameProfileWindow tpw = new GameProfileWindow(this, 1, con);
            tpw.Show();
        }

        /// <summary>
        /// takes user to game editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditGameBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// deletes game from stat tables 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteGameBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// takes user to new game window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameBTN_Click(object sender, RoutedEventArgs e)
        {
            AddGameWindow agw = new AddGameWindow(this, myTeam, con);
            agw.Show();
            this.Hide();
        }

        /// <summary>
        /// searches for game in game datagrid by opponenet team name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchGameTB_KeyUp(object sender, KeyEventArgs e)
        {

        }

        /// <summary>
        /// opens player profile for selected player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPlayerBTN_Click(object sender, RoutedEventArgs e)
        {
            PlayerProfileWindow ppw = new PlayerProfileWindow(this, myTeam, myPlayer, con);
            ppw.Show();
            this.Hide();
        }

        /// <summary>
        /// takes user to team editor window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditRosterBTN_Click(object sender, RoutedEventArgs e)
        {
            Edit_Team();
        }

        /// <summary>
        /// upon window loading, update all labels and data tables 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            Load_Player_Table_Data();
            Load_Game_Table_Data();
            Update_Team();
            Set_Labels();
        }

        /// <summary>
        /// set player attributes when a player is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayersDT_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Set_Player_Info();
        }


        /* --------------- HELPER METHODS -------------------- */
        /// <summary>
        /// fills myplayer attributes and stats of selected player
        /// </summary>
        private void Set_Player_Info()
        {
            // set team name and id if a team is selected
            if (PlayersDT.SelectedItems.Count == 1)
            {
                var items = PlayersDT.SelectedItems;
                foreach (DataRowView item in items)
                {
                    int id = Int32.Parse(item["id"].ToString());

                    try
                    {
                        // get player stats from player id
                        string statQry = string.Format(SELECT_PLAYER_STATS_QRY, id);
                        string attrQry = string.Format(SELECT_PLAYER_ATTR_QRY, id);
                        con.Open();

                        // populate attribute dataset then set player attributes
                        MySqlDataAdapter statSda = new MySqlDataAdapter(statQry, con);
                        DataSet statDs = new DataSet();
                        statSda.Fill(statDs);

                        // populate stats dataset then set player stats
                        MySqlDataAdapter attrSda = new MySqlDataAdapter(attrQry, con);
                        DataSet attrDs = new DataSet();
                        attrSda.Fill(attrDs);

                        con.Close();

                        myPlayer.Set_Stats(statDs);
                        myPlayer.Set_Attributes(attrDs);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }

        /// <summary>
        /// updates player data table
        /// </summary>
        private void Load_Player_Table_Data()
        {
            try
            {
                // select all players in current team
                string qry = string.Format(SELECT_PLAYER_ATT_STATS_QRY, myTeam.GetId());
                con.Open();

                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();

                // populate datagrid with players datatable 
                PlayersDT.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }

        /// <summary>
        /// updates game data table
        /// </summary>
        private void Load_Game_Table_Data()
        {
            try
            {
                // select all players in current team
                string qry = string.Format(SELECT_GAMES_QRY, myTeam.GetId());
                con.Open();

                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                int rows = sda.Fill(dt);
                con.Close();

                // add columns not in datatable 
                dt.Columns.Add("game_score");
                string score = "";

                if (rows == 0)
                {
                    score = "0-0";
                }
                else
                {
                    // format game score
                    int goals = Int32.Parse(dt.Rows[0]["total_gol"].ToString());
                    int oppGoals = Int32.Parse(dt.Rows[0]["opp_total_gol"].ToString());


                    score = string.Format(SCORE_FMT, goals, oppGoals);
                    dt.Rows[0]["game_score"] = score;
                }

                // populate datagrid with players datatable 
                GamesDT.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }

        /// <summary>
        /// takes user to the team editor window
        /// </summary>
        private void Edit_Team()
        {
            // check if a team is selected
            EditTeamWindow atw = new EditTeamWindow(this, myTeam, myPlayer, con);
            atw.Show();
            this.Hide();
            
        }

        /// <summary>
        /// updates team stats 
        /// </summary>
        private void Update_Team()
        {
            // update team record
            Check_Record();

            // select all teams
            string qry = string.Format(SELECT_TEAM_STATS_QRY, myTeam.GetId());
            con.Open();

            // build data adapter and use to fill data table stats
            MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();
            myTeam.Set_Stats(ds);
        }

        /// <summary>
        /// takes user to delete team window to ensure user wants to delete the team
        /// </summary>
        private void Delete_Team()
        {
            DeleteTeamWindow dtw = new DeleteTeamWindow(this, callingWindow, myTeam, con);
            dtw.Show();
        }


        /// <summary>
        /// sets stat labels of the window 
        /// </summary>
        private void Set_Labels()
        {
            // give value to all properties in model object
            MyModelObject LabelDC = new MyModelObject() {
                Record = myTeam.GetRecord(),
                WinPct = myTeam.GetWinPct().ToString(),
                LeagueRecord = myTeam.GetRecord(),
                LeagueWinPct = myTeam.GetLeagueWinPct().ToString(),
                TotalGoals = myTeam.GetTotalGoals().ToString(),
                PPG = myTeam.GetPPG().ToString(),
                TotalAssists = myTeam.GetTotalAssists().ToString(),
                APG = myTeam.GetAPG().ToString(),
                TotalBlocks = myTeam.GetTotalBlocks().ToString(),
                BPG = myTeam.GetBPG().ToString(),
                GamesPlayed = myTeam.GetGamesPlayed().ToString(),
                TotalScore = myTeam.GetTotalScore()
            };

            // set all data context bindings for labels in team main window
            RecordLBL.DataContext = LabelDC;
            WinPctLBL.DataContext = LabelDC;
            LeagueRecordLBL.DataContext = LabelDC;
            LeagueWinPctLBL.DataContext = LabelDC;
            TotalGoalsLBL.DataContext = LabelDC;
            PPGLBL.DataContext = LabelDC;
            TotalAssistsLBL.DataContext = LabelDC;
            APGLBL.DataContext = LabelDC;
            TotalBlocksLBL.DataContext = LabelDC;
            BPGLBL.DataContext = LabelDC;
            GamesPlayedLBL.DataContext = LabelDC;
            TotalScoreLBL.DataContext = LabelDC;
        }

        private void Check_Record()
        {

        }
    }
}
