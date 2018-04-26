using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// GameData is the class used to persist the current users data throughout the project. Mostly for holding data
/// </summary>
public class GameData : MonoBehaviour
{
    private IDbConnection dbcon;
    static string connectionString =
        "Server=127.0.0.1;" +
        "Database=CTPDigitalTwin;" +
        "USER ID=MagillMax;" +
        "Password=minnows37;";

    //userdata
    public string userID;
    public string userFirstName;
    public string userCurrentLevel;
    public string userCurrentJob;
    public string userXP;
    public string userJobsCompleted;

    //workflow
    public string currLeastFittingMistakes;
    public string currFastestTimeFitting;
    public string currFittingHighScore;
    public string currLeastTestingMistakes;
    public string currFastestTimeTesting;
    public string currTestingHighScore;

    public float experienceMultiplier;

    public GameData(string newUserID, string newUserFirstName, string userLeastFittingMistakes, string userFastestFittingTime, string userFittingHighScore, string userLeastTestingMistakes, string userFastestTestingTime, string userTestingHighScore)
    {
        userID = newUserID;
        userFirstName = newUserFirstName;
        currLeastFittingMistakes = userLeastFittingMistakes;
        currFastestTimeFitting = userFastestFittingTime;
        currFittingHighScore = userFittingHighScore;
        currLeastTestingMistakes = userLeastTestingMistakes;
        currFastestTimeTesting = userFastestTestingTime;
        currTestingHighScore = userTestingHighScore;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    public void GetUserDataFromTable(string newUserID)
    {
        dbcon = new SqlConnection(connectionString);
        dbcon.Open();
        userID = newUserID;
        IDbCommand dbcmd = dbcon.CreateCommand();
        string sqlCommand = "SELECT * FROM UserInformation LEFT JOIN LoginDetails ON UserInformation.UserID = LoginDetails.UserID WHERE LoginDetails.UserID = '" + newUserID + "'";
        dbcmd.CommandText = sqlCommand;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            userFirstName = reader["UserName"].ToString();
            userCurrentLevel = reader["UserLevel"].ToString();
            userCurrentJob = reader["UserJob"].ToString();
            userXP = reader["UserXP"].ToString();
            userJobsCompleted = reader["CompletedWorkFlow"].ToString();
        }
        reader.Close();
        reader = null;
        dbcon.Close();
        dbcon = null;
    }

    public void GetWokFlowLeaderBoardFromTable(string userID)
    {
        dbcon = new SqlConnection(connectionString);
        dbcon.Open();
        IDbCommand dbcmd = dbcon.CreateCommand();
        string sqlCommand = "SELECT LeastMistakesFitting, FastestTimeFitting, FittingHighScore, LeastMistakesTesting, FastestTimeTesting, TestingHighScore FROM dbo.ElecCabLeaderBoard WHERE UserID = '" + userID + "'";
        dbcmd.CommandText = sqlCommand;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            currLeastFittingMistakes = reader["LeastMistakesFitting"].ToString();
            currFastestTimeFitting = reader["FastestTimeFitting"].ToString();
            currFittingHighScore = reader["FittingHighScore"].ToString();
            currLeastTestingMistakes = reader["LeastMistakesTesting"].ToString();
            currFastestTimeTesting = reader["FastestTimeTesting"].ToString();
            currTestingHighScore = reader["TestingHighScore"].ToString();
        }
        reader.Close();
        reader = null;
        dbcon.Close();
        dbcon = null;
    }

    public void UserLoggingOut()
    {
        userFirstName = null;
        userCurrentLevel = null;
        userCurrentJob = null;
        userXP = null;
        userJobsCompleted = null;
        currLeastFittingMistakes = null;
        currFastestTimeFitting = null;
        currFittingHighScore = null;
        currLeastTestingMistakes = null;
        currFastestTimeTesting = null;
        currTestingHighScore = null;
        experienceMultiplier = 0;
}

}
