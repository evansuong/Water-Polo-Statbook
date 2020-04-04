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

        // Message Constants
        private const string SELECT_TEAM_MSG = "Please Select a Team";
        private const string FOREIGN_KEY_DISABLE_QRY = "set FOREIGN_KEY_CHECKS = 0";
        private const string SELECT_TEAM_QRY = "select * from team where id = {0}";
        private const string SELECT_TEAM_STATS_QRY = "select * from team_stats where team_id={0}";
        private const string DELETE_TEAM_QRY = "delete from team where id = {0}";
        private const string SEARCH_TEAM_QRY = "select * from team where team_name like '{0}%'";
        private const string GET_TEAMS_QRY = "select * from team";

        // Team Object
        private MyTeam myTeam;

        // Player Object
        private MyPlayer myPlayer;

        public MainWindow()
        {
            InitializeComponent();
            this.con = new MySqlConnection(CONSTRING);

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
            Edit_Team();
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
            Delete_Team();
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Load_Table_Data();
        }


        /* ---------- HELPER METHODS ----------------*/

        /// <summary>
        /// load data into team datagrid
        /// </summary>
        private void Load_Table_Data()
        {

            try
            {
                // select all teams
                string qry = GET_TEAMS_QRY;
                con.Open();

                // build data adapter and use to fill data table
                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                TeamsDT.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }

        private void Edit_Team()
        {
            // check if a team is selected
            if (myTeam.HasInfo())
            {
                EditTeamWindow atw = new EditTeamWindow(this, myTeam, myPlayer, con);
                atw.Show();
                this.Hide();
            }

            else
            {
                MessageBox.Show(SELECT_TEAM_MSG);
            }
        }

        private void View_Team()
        {
            if (myTeam.HasInfo())
            {
                try
                {
                    // select all teams
                    string qry = string.Format(SELECT_TEAM_STATS_QRY, myTeam.GetId());
                    con.Open();

                    // build data adapter and use to fill data table stats
                    MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    con.Close();
                    myTeam.Set_Stats(ds);

                    // open team main window
                    TeamMainWindow tmw = new TeamMainWindow(this, myTeam, myPlayer, con);
                    this.Hide();
                    tmw.Show();
                }

                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            else
            {
                MessageBox.Show(SELECT_TEAM_MSG);
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }

        private void Delete_Team()
        {
            DeleteTeamWindow dtw = new DeleteTeamWindow(this, this, myTeam, con);
            dtw.Show();
            this.Hide();
        }

        private void Add_Team()
        {
            AddTeamWindow atw = new AddTeamWindow(this, myTeam, con);
            atw.Show();
            this.Hide();
        }

        private void Search_Team()
        {

            // get student id from search box
            string teamName = TeamSearchTB.Text.ToString();

            try
            {
                con.Open();

                // build and execute select query according to id
                string qry = string.Format(SEARCH_TEAM_QRY, teamName);
                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                TeamsDT.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// fills myteam attributes of select team
        /// </summary>
        private void Set_Team_Attributes()
        {
            string name;
            int id;
            // set team name and id if a team is selected
            if (TeamsDT.SelectedItems.Count == 1)
            {
                var items = TeamsDT.SelectedItems;
                foreach (DataRowView item in items)
                {
                    id = Int32.Parse(item["id"].ToString());

                    try
                    {
                        // get dataset info from id
                        con.Open();
                        string qry = string.Format(SELECT_TEAM_QRY, id);
                        MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        con.Close();

                        myTeam.SetAttributes(ds);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            if (con.State == ConnectionState.Open)
                con.Close();
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
        private long winPct;
        private long leagueWinPct;
        private long shotPct;

        private int totalGoals;
        private long ppg;

        private int totalAttempts;
        private long mpg;

        private int totalAssists;
        private long apg;

        private int totalBlocks;
        private long bpg;

        private int totalSteals;
        private long spg;

        private int totalExclusions;
        private long epg;

        private int totalTurnovers;
        private long tpg;

        private int opponentGoals;
        private long opg;

        private string totalScore;

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
                this.winPct = wins / gamesPlayed;
                this.ppg = totalGoals / gamesPlayed;
                this.mpg = totalAttempts / gamesPlayed;
                this.apg = totalAssists / gamesPlayed;
                this.bpg = totalBlocks / gamesPlayed;
                this.spg = totalSteals / gamesPlayed;
                this.epg = totalExclusions / gamesPlayed;
                this.tpg = totalTurnovers / gamesPlayed;
                this.opg = opponentGoals / gamesPlayed;
            }

            // get league win pct
            if (leagueGamesPlayed == 0)
            {
                leagueWinPct = 0;
            }
            else
            {
                leagueWinPct = leagueWins / leagueLosses;
            }

            // get shot make pct
            if (totalAttempts == 0)
            {
                shotPct = 0;
            }
            else
            {
                shotPct = totalGoals / totalAttempts;
            }
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

        public long GetWinPct()
        {
            return this.winPct;
        }

        public string GetLeagueRecord()
        {
            return leagueRecord;
        }

        public long GetLeagueWinPct()
        {
            return leagueWinPct;
        }

        public int GetTotalGoals()
        {
            return totalGoals;
        }

        public long GetPPG()
        {
            return ppg;
        }

        public int GetTotalAttempts()
        {
            return totalAttempts;
        }

        public long GetMpg()
        {
            return mpg;
        }

        public long GetShotPct()
        {
            return shotPct;
        }

        public int GetTotalAssists()
        {
            return totalAssists;
        }
        public long GetAPG()
        {
            return apg;
        }
        public int GetTotalBlocks()
        {
            return totalBlocks;
        }
        public long GetBPG()
        {
            return bpg;
        }

        public long GetSPG()
        {
            return spg;
        }

        public long GetEPG()
        {
            return epg;
        }

        public long GetTPG()
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
        private long ppg;
        private int totalAttempts;
        private long mpg;
        private int totalAssists;
        private long apg;
        private int totalBlocks;
        private long bpg;
        private int totalSteals;
        private long spg;
        private int totalExclusions;
        private long epg;
        private int totalTurnovers;
        private long tpg;

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
    public class MyModelObject
    {
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
        public string Name { get; set; }
        public string Num { get; set; }
        public string Pos { get; set; }
        public string Year { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
    }

}
