using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Water_Polo_Statbook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // SQL objects
        private const string CONSTRING = @"server=localhost;user id=root;database=waterpolo_team_db;PASSWORD=Tsmrth001!";
        private MySqlConnection con;

        // MySqlQueryBuilde Object
        private MySqlQueryBuilder build;

        // Message Constants
        private const string SELECT_TEAM_MSG = "Please Select a Team";

        // qry constants
        private const string FOREIGN_KEY_DISABLE_QRY = "set FOREIGN_KEY_CHECKS = 0";
        private const string SELECT_TEAM_QRY = "select * from team where id = {0}";
        private const string SELECT_TEAM_STATS_QRY = "select * from team_stats where team_id={0}";
        private const string DELETE_TEAM_QRY = "delete from team where id = {0}";
        private const string SEARCH_TEAM_QRY = "select * from team where team_name like '{0}%'";
        private const string GET_TEAMS_QRY = "select * from team";

        // misc messages
        private const string DLT_MSG = "Are you sure you want to delete?";

        // Team Object
        private MyTeam myTeam;

        // Player Object
        private MyPlayer myPlayer;

  

        public MainWindow()
        {
            InitializeComponent();
            this.con = new MySqlConnection(CONSTRING);
            this.build = new MySqlQueryBuilder(con);

            this.myPlayer = new MyPlayer();
            this.myTeam = new MyTeam();

            Load_Table_Data();
        }

        /// <summary>
        /// takes user to team editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTeamBTN_Click(object sender, RoutedEventArgs e)
        {
            // check if a team is selected
            if (myTeam.HasInfo())
            {
                EditTeamWindow atw = new EditTeamWindow(this, myTeam, myPlayer, build);
                atw.Show();
                this.Hide();
            }

            else
            {
                MessageBox.Show(SELECT_TEAM_MSG);
            }
        }

        /// <summary>
        /// takes user to team main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewTeamBTN_Click(object sender, RoutedEventArgs e)
        {
            View_Team();
        }

        /// <summary>
        /// takes user to delete team window
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
       /// takes user to add team window
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void AddTeamBTN_Click(object sender, RoutedEventArgs e)
        {
            Add_Team();
        }

        /// <summary>
        /// searches for team in data table by name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TeamSearchTB_KeyUp(object sender, KeyEventArgs e)
        {
            Search_Team();
        }

        /// <summary>
        /// sets 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TeamsDT_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Set_Team_Attributes();
        }



        /// <summary>
        /// loads table data every time this window is laoded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Load_Table_Data();
        }


        /* ---------- HELPER METHODS ----------------*/

        /* ---------- TEAM HELPER METHODS ------------- */

        /// <summary>
        /// takes user to main window of selected team
        /// </summary>
        private void View_Team()
        {
            if (myTeam.HasInfo())
            {
                // select all teams
                string qry = string.Format(SELECT_TEAM_STATS_QRY, myTeam.GetId());
                myTeam.Set_Stats(build.Execute_DataSet_Query(qry));

                // open team main window
                TeamMainWindow tmw = new TeamMainWindow(this, myTeam, myPlayer, build);
                this.Hide();
                tmw.Show();
            }
            else
            {
                MessageBox.Show(SELECT_TEAM_MSG);
            }
        }

        /// <summary>
        /// takes user to delete window of selected team
        /// </summary>
        private void Delete_Team()
        {
            // disable foreign key checks
            build.Execute_Query(FOREIGN_KEY_DISABLE_QRY);

            // delete team from database
            string qry = string.Format(DELETE_TEAM_QRY, myTeam.GetId());
            build.Execute_Query(qry);
            Load_Table_Data();
        }


        /// <summary>
        /// takes user to add team window
        /// </summary>
        private void Add_Team()
        {
            AddTeamWindow atw = new AddTeamWindow(this, myTeam, build);
            atw.Show();
            this.Hide();
        }


        /// <summary>
        /// searches for team by name in database
        /// </summary>
        private void Search_Team()
        {
            // get student id from search box
            string teamName = TeamSearchTB.Text.ToString();

            // build and execute select query according to id
            string qry = string.Format(SEARCH_TEAM_QRY, teamName);
            TeamsDT.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }

        /// <summary>
        /// fills myteam attributes of selected team
        /// </summary>
        private void Set_Team_Attributes()
        {
            int id;
            // set team name and id if a team is selected
            if (TeamsDT.SelectedItems.Count == 1)
            {
                var items = TeamsDT.SelectedItems;
                foreach (DataRowView item in items)
                {
                    id = Int32.Parse(item["id"].ToString());
                    string qry = string.Format(SELECT_TEAM_QRY, id);
                    myTeam.SetAttributes(build.Execute_DataSet_Query(qry));
                }
            }
        }



        /* --------------- MISC HELPER METHODS --------------- */
        /// <summary>
        /// load all teams into team datagrid
        /// </summary>
        private void Load_Table_Data()
        {
            // select all teams
            string qry = GET_TEAMS_QRY;
            TeamsDT.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }



        /* --------- GETTERS AND SETTERS -------------- */
        public MyPlayer GetMyPlayer()
        {
            return myPlayer;
        }
    }

    /// <summary>
    /// contains all attributes and stats of a selected team
    /// </summary>
    public class MyTeam 
    {

        // team attributes
        private string name;
        private int id;

        // team stats
        private int gamesPlayed, leagueGamesPlayed;
        private int wins, losses;
        private int leagueWins, leagueLosses;
        private string record;
        private string leagueRecord;
        private float winPct;
        private float leagueWinPct;
        private float shotPct;

        private int totalGoals;
        private float ppg;

        private int totalAttempts;
        private float mpg;

        private int totalAssists;
        private float apg;

        private int totalBlocks;
        private float bpg;

        private int totalSteals;
        private float spg;

        private int totalExclusions;
        private float epg;

        private int totalTurnovers;
        private float tpg;

        private int opponentGoals;
        private float opg;

        private string totalScore;

        // playerhighs
        private string topGol, topAst, topStl, topBlk;

        // string formats
        private const string RECORD_FMT = "{0}-{1}";

        // check if team has info
        private bool hasInfo;

        public MyTeam ()
        {
            hasInfo = false;
        }

        public void SetAttributes (DataSet ds)
        {
            this.name = ds.Tables[0].Rows[0]["team_name"].ToString();
            this.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);
            this.hasInfo = true;
        }

        public void Set_Stats(DataSet ds) 
        {
            this.gamesPlayed = Convert.ToInt32(ds.Tables[0].Rows[0]["games_played"]);
            this.leagueGamesPlayed = Convert.ToInt32(ds.Tables[0].Rows[0]["league_games_played"]);
            this.wins = Convert.ToInt32(ds.Tables[0].Rows[0]["wins"]);
            this.losses = Convert.ToInt32(ds.Tables[0].Rows[0]["losses"]);
            this.record = string.Format(RECORD_FMT, wins.ToString(), losses.ToString());
           
            this.leagueWins = Convert.ToInt32(ds.Tables[0].Rows[0]["league_wins"]); ;
            this.leagueLosses = Convert.ToInt32(ds.Tables[0].Rows[0]["league_losses"]); ;
            this.leagueRecord = string.Format(RECORD_FMT, leagueWins, leagueLosses);
            this.totalGoals = Convert.ToInt32(ds.Tables[0].Rows[0]["total_gol"]);
            this.totalAttempts = Convert.ToInt32(ds.Tables[0].Rows[0]["total_att"]);
            this.totalAssists = Convert.ToInt32(ds.Tables[0].Rows[0]["total_ast"]); ;
            this.totalBlocks = Convert.ToInt32(ds.Tables[0].Rows[0]["total_blk"]); ;
            this.totalSteals = Convert.ToInt32(ds.Tables[0].Rows[0]["total_stl"]); ;
            this.totalExclusions = Convert.ToInt32(ds.Tables[0].Rows[0]["total_exl"]); ;
            this.totalTurnovers = Convert.ToInt32(ds.Tables[0].Rows[0]["total_tov"]); ;
            this.opponentGoals = Convert.ToInt32(ds.Tables[0].Rows[0]["total_opponent_gol"]); ;
            this.totalScore = string.Format(RECORD_FMT, totalGoals, opponentGoals);

            if (gamesPlayed == 0)
            {
                this.winPct = 0;
                this.shotPct = 0;
                this.ppg = 0;
                this.mpg = 0;
                this.apg = 0;
                this.bpg = 0;
                this.spg = 0;
                this.epg = 0;
                this.tpg = 0;
                this.opg = 0;
                this.leagueWinPct = 0;
            }
            else
            {
                this.winPct = Calculate_Avg(wins, wins + losses) * 100; 
                this.ppg = Calculate_Avg(totalGoals, gamesPlayed);
                this.mpg = Calculate_Avg(totalAttempts, gamesPlayed);
                this.apg = Calculate_Avg(totalAssists, gamesPlayed);
                this.bpg = Calculate_Avg(totalBlocks, gamesPlayed);
                this.spg = Calculate_Avg(totalSteals, gamesPlayed);
                this.epg = Calculate_Avg(totalExclusions, gamesPlayed);
                this.tpg = Calculate_Avg(totalTurnovers, gamesPlayed);
                this.opg = Calculate_Avg(opponentGoals, gamesPlayed);
            }

            // get league win pct
            if (leagueGamesPlayed == 0)
            {
                leagueWinPct = 0;
            }
            else if (leagueLosses == 0 && leagueWins > 0)
            {
                leagueWinPct = 100;
            }
            else
            {
                leagueWinPct = Calculate_Avg(leagueWins, leagueLosses + leagueWins) * 100;
            }

            // get shot make pct
            if (totalAttempts == 0)
            {
                shotPct = 0;
            }
            else
            {
                shotPct = Calculate_Avg(totalGoals, totalAttempts) * 100;
            }
        }

        public void Set_Highs(string topGol, string topAst, string topBlk, string topStl)
        {
            this.topGol = topGol;
            this.topAst = topAst;
            this.topBlk = topBlk;
            this.topStl = topStl;
        }

        private float Calculate_Avg(int stat, int div)
        {
            if (div == 0)
                return 0;
            else
                return float.Parse(string.Format("{0:0.00}", stat / (float) div));
        }

        public string GetName()
        {
            return this.name;
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetRecord()
        {
            return this.record;
        }

        public float GetWinPct()
        {
            return this.winPct;
        }

        public string GetLeagueRecord()
        {
            return leagueRecord;
        }

        public float GetLeagueWinPct()
        {
            return leagueWinPct;
        }

        public int GetTotalGoals()
        {
            return totalGoals;
        }

        public float GetPPG()
        {
            return ppg;
        }

        public int GetTotalAttempts()
        {
            return totalAttempts;
        }

        public float GetMpg()
        {
            return mpg;
        }

        public float GetShotPct()
        {
            return shotPct;
        }

        public int GetTotalAssists()
        {
            return totalAssists;
        }
        public float GetAPG()
        {
            return apg;
        }
        public float GetTotalBlocks()
        {
            return totalBlocks;
        }
        public float GetBPG()
        {
            return bpg;
        }

        public float GetSPG()
        {
            return spg;
        }

        public float GetEPG()
        {
            return epg;
        }

        public float GetTPG()
        {
            return tpg;
        }

        public int GetGamesPlayed()
        {
            return gamesPlayed;
        }

        public string GetTotalScore()
        {
            return totalScore;
        }

        public bool HasInfo()
        {
            return this.hasInfo;
        }

        public string GetTopGol()
        {
            return topGol;
        }
        public string GetTopAst()
        {
            return topAst;
        }
        public string GetTopBlk()
        {
            return topBlk;
        }
        public string GetTopStl()
        {
            return topStl;
        }
    }

    /// <summary>
    /// contains all stats and attributes of a selected player
    /// </summary>
    public class MyPlayer
    {

        // player attributes
        private int num;
        private string name;
        private string pos;
        private int year;
        private int height, weight;
        private int id, teamId;

        // check if player has attributes
        private bool hasAttributes;

        // player stats
        private int totalGoals;
        private int totalAttempts;
        private int totalAssists;
        private int totalBlocks;
        private int totalSteals;
        private int totalExclusions;
        private int totalTurnovers;

        public MyPlayer ()
        {

        }

        /// <summary>
        /// sets attributes of all players
        /// </summary>
        /// <param name="ds">dataset containing all player attributes</param>
        public void Set_Attributes(DataSet ds)
        {
            this.num = Convert.ToInt32(ds.Tables[0].Rows[0]["player_num"]);
            this.name = ds.Tables[0].Rows[0]["player_name"].ToString();
            this.pos = ds.Tables[0].Rows[0]["player_pos"].ToString(); 
            this.year = Convert.ToInt32(ds.Tables[0].Rows[0]["player_year"]); 
            this.height = Convert.ToInt32(ds.Tables[0].Rows[0]["player_height"]);  
            this.weight = Convert.ToInt32(ds.Tables[0].Rows[0]["player_weight"]); 
            this.teamId = Convert.ToInt32(ds.Tables[0].Rows[0]["team_id"]); 
            this.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]); 

            this.hasAttributes = true;
        }

        /// <summary>
        /// sets all stat properties of player
        /// </summary>
        /// <param name="ds">dataset containing all player stat values</param>
        public void Set_Stats(DataSet ds)
        {
            this.totalGoals = Convert.ToInt32(ds.Tables[0].Rows[0]["total_gol"]);
            this.totalAttempts = Convert.ToInt32(ds.Tables[0].Rows[0]["total_att"]);
            this.totalAssists = Convert.ToInt32(ds.Tables[0].Rows[0]["total_ast"]);
            this.totalBlocks = Convert.ToInt32(ds.Tables[0].Rows[0]["total_blk"]);
            this.totalSteals = Convert.ToInt32(ds.Tables[0].Rows[0]["total_stl"]);
            this.totalExclusions= Convert.ToInt32(ds.Tables[0].Rows[0]["total_exl"]);
            this.totalTurnovers = Convert.ToInt32(ds.Tables[0].Rows[0]["total_tov"]);
            this.id = Convert.ToInt32(ds.Tables[0].Rows[0]["player_id"]);
        }

        public bool HasAttributes()
        {
            return hasAttributes;
        }

        public int GetNum()
        {
            return num;
        }

        public string GetName()
        {
            return name;
        }

        public string GetPos()
        {
            return pos;
        }

        public int GetYear()
        {
            return year;
        }

        public int GetHeight()
        {
            return height;
        }

        public int GetWeight()
        {
            return weight;
        }

        public int GetTeamId()
        {
            return teamId;
        }

        public int GetId()
        {
            return id;
        }

        public int GetTotalGoals()
        {
            return totalGoals;
        }
        public int GetTotalAttempts()
        {
            return totalAttempts;
        }
        public int GetTotalAssists()
        {
            return totalAssists;
        }
        public int GetTotalBlocks()
        {
            return totalBlocks;
        }
        public int GetTotalSteals()
        {
            return totalSteals;
        }
        public int GetTotalExclusions()
        {
            return totalExclusions;
        }
        public int GetTotalTurnovers()
        {
            return totalTurnovers;
        }

        public void SetId(int id)
        {
            this.id = id;
        }
    }


    public class MyGame
    {
        private string name;
        private string type;
        private string loc;
        private string date;
        private string result;
        private int teamId;
        private int id;
        private bool hasAttributes = false;

        private int homeGoals;
        private int oppGoals;

        public MyGame()
        {
            init();
        }

        public void init ()
        {
            homeGoals = 0;
            oppGoals = 0;
        }

        public void Set_Stats(DataSet ds)
        {
            this.homeGoals = Convert.ToInt32(ds.Tables[0].Rows[0]["total_gol"].ToString());
            this.oppGoals = Convert.ToInt32(ds.Tables[0].Rows[0]["opp_total_gol"].ToString());
        }

        public void Set_Attributes(DataSet ds)
        {
            // format name
            this.name = ds.Tables[0].Rows[0]["opp_team"].ToString();
            this.name = "Vs. " + name;

            // format type
            this.type = ds.Tables[0].Rows[0]["game_type"].ToString();
            if (type == "Tourn.")
            {
                type = "Tournament";
            }
            else if (type == "Non Lg")
            {
                type = "Non League";
            }

            // format location 
            this.loc = ds.Tables[0].Rows[0]["game_location"].ToString();
            this.loc = "@" + loc;

            // set all other attributes
            this.date = ds.Tables[0].Rows[0]["game_date"].ToString();
            this.result = ds.Tables[0].Rows[0]["game_result"].ToString();
            this.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
            this.teamId = Convert.ToInt32(ds.Tables[0].Rows[0]["team_id"].ToString());
            this.homeGoals = Convert.ToInt32(ds.Tables[0].Rows[0]["total_gol"].ToString());
            this.oppGoals = Convert.ToInt32(ds.Tables[0].Rows[0]["opp_total_gol"].ToString());

            this.hasAttributes = true;
        }

        public void Clear()
        {
            hasAttributes = false;
        }

        public string GetName()
        {
            return name;
        }
        public string GetGameType()
        {
            return this.type;
        }
        public string GetLoc()
        {
            return loc;
        }
        public string GetDate()
        {
            return date;
        }
        public string GetResult()
        {
            return result;
        }
        public int GetTeamId()
        {
            return teamId;
        }
        public int GetId()
        {
            return id;
        }
        public bool HasAttributes()
        {
            return hasAttributes;
        }

        public int GetHomeGoals()
        {
            return homeGoals;
        }

        public int GetOppGoals()
        {
            return oppGoals;
        }
    }


    public class MyModelObject
    {
        // team labels
        public string Record { get; set; }
        public string WinPct { get; set; }
        public string LeagueRecord { get; set; }
        public string LeagueWinPct { get; set; }
        public string TotalGoals { get; set; }
        public string PPG { get; set; }
        public string TotalAttempts { get; set; }
        public string MPG { get; set; }
        public string ShotPct { get; set; }
        public string TotalAssists { get; set; }
        public string APG { get; set; }
        public string TotalBlocks { get; set; }
        public string BPG { get; set; }
        public string GamesPlayed { get; set; }
        public string TotalScore { get; set; }

        // player labels
        public string Name { get; set; }
        public string Num { get; set; }
        public string Pos { get; set; }
        public string Year { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }

        // game labels
        public string GameName { get; set; }
        public string GameType { get; set; }
        public string GameLoc { get; set; }
        public string GameDate { get; set; }
        public string HomeGoals { get; set; }
        public string OppGoals { get; set; }
        public string TopGol { get; set; }
        public string TopAst { get; set; }
        public string TopBlk { get; set; }
        public string TopStl { get; set; }
    }

}
