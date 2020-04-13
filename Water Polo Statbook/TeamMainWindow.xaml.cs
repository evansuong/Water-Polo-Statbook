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
        private MySqlQueryBuilder build;

        // mygame object
        private MyGame myGame;

        // player queries 
        private const string SELECT_PLAYERS_QRY = "select * from player where team_id={0}";
        private const string SELECT_PLAYER_ATTR_QRY = "select * from player where id={0}";
        private const string SELECT_PLAYER_STATS_QRY = "select * from player_stats where player_id={0}";
        private const string SEARCH_PLAYER_BY_NAME_QRY = "select * from player where player_name like '{0}%' and team_id={1}";
        private const string SELECT_PLAYER_ATT_STATS_QRY = "select * from player inner join player_stats on player.id = player_stats.player_id where team_id={0}";
        private const string SELECT_MAX_GOL_QRY = "select player.player_name, player_stats.total_gol from player inner join player_stats on player.id = player_stats.player_id where team_id={0} order by total_gol desc limit 1";
        private const string SELECT_MAX_AST_QRY = "select player.player_name, player_stats.total_ast from player inner join player_stats on player.id = player_stats.player_id where team_id={0} order by total_ast desc limit 1";
        private const string SELECT_MAX_BLK_QRY = "select player.player_name, player_stats.total_blk from player inner join player_stats on player.id = player_stats.player_id where team_id={0} order by total_blk desc limit 1";
        private const string SELECT_MAX_STL_QRY = "select player.player_name, player_stats.total_stl from player inner join player_stats on player.id = player_stats.player_id where team_id={0} order by total_stl desc limit 1";

        // game queries
        private const string SELECT_GAME_QRY = "select game.opp_team, game.game_type, game.game_location, game.game_result, date_format(game_date, '%M-%D-%Y') as game_date, game.team_id, game.id, game_stats_opponent.total_gol as opp_total_gol, game_stats.total_gol from game inner join game_stats on game.id = game_stats.game_id inner join game_stats_opponent on game.id = game_stats_opponent.game_id where game.id={0}";
        private const string SELECT_GAMES_QRY = "select game.opp_team, game.game_type, game.game_location, date_format(game.game_date, '%M-%D-%Y') as game_date, game.game_result, game.id, game_stats.total_gol, game_stats_opponent.total_gol as opp_total_gol from game inner join game_stats on game.id = game_stats.game_id inner join game_stats_opponent on game.id = game_stats_opponent.game_id where game.team_id={0}";
        private const string DELETE_GAME_QRY = "delete from game where id={0}";
        private const string SEARCH_GAME_BY_NAME_QRY = "select game.opp_team, game.game_type, game.game_location, date_format(game.game_date, '%M-%D-%Y') as game_date, game.game_result, game.id, game_stats.total_gol, game_stats_opponent.total_gol as opp_total_gol from game inner join game_stats on game.id = game_stats.game_id inner join game_stats_opponent on game.id = game_stats_opponent.game_id where game.opp_team like '{0}%' and game.team_id={1}";

        // team queries
        private const string SELECT_TEAM_STATS_QRY = "select * from team_stats where team_id={0}";
        private const string DELETE_TEAM_QRY = "delete from team where id = {0}";

        // misc queries
        private const string FOREIGN_KEY_DISABLE_QRY = "set FOREIGN_KEY_CHECKS = 0";

        // string formatting
        private const string SCORE_FMT = "{0}-{1}";
        private const string PLAYER_STAT_FMT = "{0}: {1}";

        // error messages
        private const string SELECT_PLAYER_ERR = "Please select a player";
        private const string SELECT_GAME_ERR = "Please select a game";

        // misc messages
        private const string DLT_MSG = "Are you sure you want to delete?";


        public TeamMainWindow(Window callingWindow, MyTeam myTeam, MyPlayer myPlayer, MySqlQueryBuilder build)
        {
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.myPlayer = myPlayer;
            this.build = build;

            myGame = new MyGame();

            InitializeComponent();
            Load_Player_Table_Data();
            Set_Labels();

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
            // prompt user if they want to delete the team
            MessageBoxResult result = MessageBox.Show("", DLT_MSG, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Delete_Team();
            }
            else
            {
                return;
            }
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
            TeamProfileWindow tpw = new TeamProfileWindow(this, myTeam, build);
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
            if (myGame.HasAttributes())
            {
                GameProfileWindow tpw = new GameProfileWindow(this, myTeam, myGame, build);
                tpw.Show();
            }
            else
            {
                MessageBox.Show(SELECT_GAME_ERR);
            }
        }

        /// <summary>
        /// takes user to game editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditGameBTN_Click(object sender, RoutedEventArgs e)
        {
            if(myGame.HasAttributes())
            {
                EditGameWindow egw = new EditGameWindow(this, myGame, build);
                egw.Show();
            }
            else
            {
                MessageBox.Show(SELECT_GAME_ERR);
            }
        }

        /// <summary>
        /// deletes game from stat tables 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteGameBTN_Click(object sender, RoutedEventArgs e)
        {
            if (myGame.HasAttributes())
            {
                // prompt user if they want to leave the window 
                MessageBoxResult result = MessageBox.Show("", DLT_MSG, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Delete_Game();
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show(SELECT_GAME_ERR);
            }
        }

        /// <summary>
        /// takes user to new game window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameBTN_Click(object sender, RoutedEventArgs e)
        {
            AddGameWindow agw = new AddGameWindow(this, myTeam, myGame, build);
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
            // show games that match game name
            Search_Game();
        }

        /// <summary>
        /// fills mygame attribtues with select game info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamesDG_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Set_Game_Attributes();
        }

        /// <summary>
        /// opens player profile for selected player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPlayerBTN_Click(object sender, RoutedEventArgs e)
        {
            if (myPlayer.HasAttributes())
            {
                PlayerProfileWindow ppw = new PlayerProfileWindow(this, myTeam, myPlayer, build);
                ppw.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(SELECT_PLAYER_ERR);
                return;
            }
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
            Set_Team_Stats();
            Set_Labels();
        }

        /// <summary>
        /// set player attributes when a player is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayersDG_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Set_Player_Attributes_Stats();
        }



        private void SearchPlayerTB_KeyUp(object sender, KeyEventArgs e)
        {
            Search_Player();
        }


        /* --------------- PLAYER HELPER METHODS -------------------- */
        /// <summary>
        /// fills myplayer attributes and stats of selected player
        /// </summary>
        private void Set_Player_Attributes_Stats()
        {
            // set team name and id if a team is selected
            if (PlayersDG.SelectedItems.Count == 1)
            {
                var items = PlayersDG.SelectedItems;
                foreach (DataRowView item in items)
                {
                    int id = Int32.Parse(item["id"].ToString());

                    // get player stats from player id
                    string statqry = string.Format(SELECT_PLAYER_STATS_QRY, id);
                    string attrqry = string.Format(SELECT_PLAYER_ATTR_QRY, id);

                    myPlayer.Set_Stats(build.Execute_DataSet_Query(statqry));
                    myPlayer.Set_Attributes(build.Execute_DataSet_Query(attrqry));
                }
            }
        }

        /// <summary>
        /// updates player data table
        /// </summary>
        private void Load_Player_Table_Data()
        {
            // select all players in current team
            string qry = string.Format(SELECT_PLAYER_ATT_STATS_QRY, myTeam.GetId());
            DataTable dt = build.Execute_DataTable_Qry(qry);

            // caculate team highs
            Calculate_Highs();

            // populate datagrid with players datatable 
            PlayersDG.ItemsSource = dt.DefaultView;
        }


        private void Calculate_Highs()
        {
            // build quieries for highest stat counts
            string golqry = string.Format(SELECT_MAX_GOL_QRY, myTeam.GetId());
            string astqry = string.Format(SELECT_MAX_AST_QRY, myTeam.GetId());
            string blkqry = string.Format(SELECT_MAX_BLK_QRY, myTeam.GetId());
            string stlqry = string.Format(SELECT_MAX_STL_QRY, myTeam.GetId());
           
            // parse player names and stat amounts
            string golName = build.Execute_DataTable_Qry(golqry).Rows[0]["player_name"].ToString();
            int golCount = Int32.Parse(build.Execute_DataTable_Qry(golqry).Rows[0]["total_gol"].ToString());
            string astName = build.Execute_DataTable_Qry(astqry).Rows[0]["player_name"].ToString();
            int astCount = Int32.Parse(build.Execute_DataTable_Qry(astqry).Rows[0]["total_ast"].ToString());
            string blkName = build.Execute_DataTable_Qry(blkqry).Rows[0]["player_name"].ToString();
            int blkCount = Int32.Parse(build.Execute_DataTable_Qry(blkqry).Rows[0]["total_blk"].ToString());
            string stlName = build.Execute_DataTable_Qry(stlqry).Rows[0]["player_name"].ToString();
            int stlCount = Int32.Parse(build.Execute_DataTable_Qry(stlqry).Rows[0]["total_stl"].ToString());

            // format label strings 
            if (golCount == 0) golName = "NA";
            else golName = string.Format(PLAYER_STAT_FMT, golName, golCount);

            if (astCount == 0) astName = "NA";
            else astName = string.Format(PLAYER_STAT_FMT, astName, astCount);

            if (blkCount == 0) blkName = "NA";
            else blkName = string.Format(PLAYER_STAT_FMT, blkName, blkCount);

            if (stlCount == 0) stlName = "NA";
            else stlName = string.Format(PLAYER_STAT_FMT, stlName, stlCount);

            // set highs in myteam
            myTeam.Set_Highs(golName, astName, blkName, stlName);
        }


        private void Search_Player()
        {
            // get student id from search box
            string playerName = SearchPlayerTB.Text.ToString();

            // search player info by name
            string qry = string.Format(SEARCH_PLAYER_BY_NAME_QRY, playerName, myTeam.GetId());

            // set player attrivutes and player by searched player
            //myPlayer.Set_Attributes(build.Execute_DataSet_Query(qry));
            PlayersDG.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }



        /* --------------- GAME HELPER METHODS -------------- */
        
        /// <summary>
        /// updates game data table
        /// </summary>
        private void Load_Game_Table_Data()
        {
            // select all players in current team
            string qry = string.Format(SELECT_GAMES_QRY, myTeam.GetId());
            DataTable dt = build.Execute_DataTable_Qry(qry);
            int rows = dt.Rows.Count;

            // add columns not in datatable 
            dt.Columns.Add("game_score");

            // add game score to each game in table
            for (int i = 0; i < rows; i++)
            {
                // format game score
                string goals = dt.Rows[i]["total_gol"].ToString();
                string oppGoals = dt.Rows[i]["opp_total_gol"].ToString();
                string score = string.Format(SCORE_FMT, goals, oppGoals);
                dt.Rows[i]["game_score"] = score;
            }
        
            // populate datagrid with players datatable 
            GamesDG.ItemsSource = dt.DefaultView;
        }


        private void Delete_Game()
        {   
            // disable foregnkey contraints
            string setQry = FOREIGN_KEY_DISABLE_QRY;
            build.Execute_Query(setQry);

            // delete game from database
            string delQry = string.Format(DELETE_GAME_QRY, myGame.GetId());
            build.Execute_Query(delQry);

            Load_Game_Table_Data();
            Set_Team_Stats();
            Set_Player_Attributes_Stats();
            Set_Labels();
            Load_Player_Table_Data();
            myGame.Clear();
        }


        private void Search_Game()
        {
            // get student id from search box
            string gameName = SearchGameTB.Text.ToString();

            // search player info by name
            string qry = string.Format(SEARCH_GAME_BY_NAME_QRY, gameName, myTeam.GetId());
            DataTable dt = build.Execute_DataTable_Qry(qry);
            int rows = dt.Rows.Count;

            // add columns not in datatable 
            dt.Columns.Add("game_score");

            // add game score to each game in table
            for (int i = 0; i < rows; i++)
            {
                // format game score
                string goals = dt.Rows[i]["total_gol"].ToString();
                string oppGoals = dt.Rows[i]["opp_total_gol"].ToString();
                string score = string.Format(SCORE_FMT, goals, oppGoals);
                dt.Rows[i]["game_score"] = score;
            }

            // populate datagrid with players datatable 
            GamesDG.ItemsSource = dt.DefaultView;
        }



        /// <summary>
        /// set mygame attributes to selected game
        /// </summary>
        private void Set_Game_Attributes()
        {
            // set team name and id if a team is selected
            if (GamesDG.SelectedItems.Count == 1)
            {
                var items = GamesDG.SelectedItems;
                foreach (DataRowView item in items)
                {
                    // parse game id from selected game
                    int id = Int32.Parse(item["id"].ToString());

                    // get game attributes from qry by id
                    string qry = string.Format(SELECT_GAME_QRY, id);
                    myGame.Set_Attributes(build.Execute_DataSet_Query(qry));
                }
            }
        }




        /*------------ TEAM HELPER METHODS --------------- */

        /// <summary>
        /// takes user to the team editor window
        /// </summary>
        private void Edit_Team()
        {
            // check if a team is selected
            EditTeamWindow atw = new EditTeamWindow(this, myTeam, myPlayer, build);
            atw.Show();
            this.Hide();
            
        }

        /// <summary>
        /// sets myteam stats to selected team
        /// </summary>
        private void Set_Team_Stats()
        {
            // select team stats and set myteam's stats to selected team
            string qry = string.Format(SELECT_TEAM_STATS_QRY, myTeam.GetId());
            myTeam.Set_Stats(build.Execute_DataSet_Query(qry));
        }




        /// <summary>
        /// takes user to delete team window to ensure user wants to delete the team
        /// </summary>
        private void Delete_Team()
        {
            // disable foreign key checks
            build.Execute_Query(FOREIGN_KEY_DISABLE_QRY);

            // delete team from database
            string qry = string.Format(DELETE_TEAM_QRY, myTeam.GetId());
            build.Execute_Query(qry);
            callingWindow.Show();
            this.Close();
        }






        /* --------------------- MISC METHODS --------------- */


        /// <summary>
        /// sets stat labels of the window 
        /// </summary>
        private void Set_Labels()
        {
            // give value to all properties in model object
            MyModelObject LabelDC = new MyModelObject() {
                Record = myTeam.GetRecord(),
                WinPct = myTeam.GetWinPct().ToString() + "%",
                LeagueRecord = myTeam.GetLeagueRecord(),
                LeagueWinPct = myTeam.GetLeagueWinPct().ToString() + "%",
                TotalGoals = myTeam.GetTotalGoals().ToString(),
                PPG = myTeam.GetPPG().ToString(),
                TotalAssists = myTeam.GetTotalAssists().ToString(),
                APG = myTeam.GetAPG().ToString(),
                TotalBlocks = myTeam.GetTotalBlocks().ToString(),
                BPG = myTeam.GetBPG().ToString(),
                GamesPlayed = myTeam.GetGamesPlayed().ToString(),
                TotalScore = myTeam.GetTotalScore(),
                TopGol = myTeam.GetTopGol(),
                TopAst = myTeam.GetTopAst(),
                TopBlk = myTeam.GetTopBlk(),
                TopStl = myTeam.GetTopStl(),
               
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
            TopGolLBL.DataContext = LabelDC;
            TopAstLBL.DataContext = LabelDC;
            TopBlkLBL.DataContext = LabelDC;
            TopStlLBL.DataContext = LabelDC;
        }
    }
}
