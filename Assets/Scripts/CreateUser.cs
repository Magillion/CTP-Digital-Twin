using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// CreateUser handles the creation of ne users and their data in the tables
/// </summary>
public class CreateUser : MonoBehaviour {


    private IDbConnection dbcon;

    string connectionString =
        "Server=127.0.0.1;" +
        "Database=CTPDigitalTwin;" +
        "USER ID=MagillMax;" +
        "Password=minnows37;";

    private string username;
    private string password;

    private string usernameString = string.Empty;
    private string passwordString = string.Empty;
    private string userID = string.Empty;

    private Rect windowsRect = new Rect(0, 0, 1920, 1080);
    public Text UserCreatedText;

    private void OnEnable()
    {
        UserCreatedText.enabled = false;
        dbcon = new SqlConnection(connectionString);
        dbcon.Open();
      
    }
    private void OnGUI()
    {
        GUI.Window(0, windowsRect, WindowFunction, "Login");
    }
    //Uses th unity GUI system 
    void WindowFunction(int windowID)
    {
        usernameString = GUI.TextField(new Rect(Screen.width / 3, 2 * Screen.height / 8, Screen.width / 3, Screen.height / 10), usernameString, 10);

        userID = GUI.TextField(new Rect(Screen.width / 3, 2 * Screen.height / 4.5f, Screen.width / 3, Screen.height / 10), userID, 10);

        passwordString = GUI.TextField(new Rect(Screen.width / 3, 2 * Screen.height / 3, Screen.width / 3, Screen.height / 10), passwordString, 10);

        if (GUI.Button(new Rect(Screen.width / 2, 4 * Screen.height / 5, Screen.width / 8, Screen.height / 8), "Create User"))
        {
            AddUserToDatabase(usernameString, passwordString, userID);
        }


        GUI.Label(new Rect(Screen.width / 3, 23 * Screen.height / 100, Screen.width / 5, Screen.height / 8), "Username");
        GUI.Label(new Rect(Screen.width / 3, 43 * Screen.height / 100, Screen.width / 5, Screen.height / 8), "UserID");
        GUI.Label(new Rect(Screen.width / 3, 65 * Screen.height / 100, Screen.width / 8, Screen.height / 8), "Password");
    }

    //Following three functions add the user to the required tables
    private void AddUserToDatabase(string u, string p, string n)
    {
        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "INSERT INTO dbo.LoginDetails (Username, Password, UserId) VALUES ('"+ u + "', '" + p + "', '" + n + "')";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;

        CreateUserInformationEntry(u, n);

    }

    private void CreateUserInformationEntry(string newUserName, string newUserID)
    {
        string newUserLvl = "1";
        string newUserJob = "ElectricalCabinet";
        string newUserXP = "0";
        string newUserCompletedWorkFlows = "0";
        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "INSERT INTO dbo.UserInformation (UserID, UserName, UserLevel, UserJob, UserXP, CompletedWorkFlow) VALUES ('" + newUserID + "', '" + newUserName + "', '" + newUserLvl + "', '" + newUserJob + "', '" + newUserXP + "', '" + newUserCompletedWorkFlows + "')";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        CreateUserLeaderboardEntry(newUserName, newUserID);
   
    }

    private void CreateUserLeaderboardEntry(string newUserName, string newUserID)
    {
        string newLeastFittingMistakes = "2000";
        string newFastestFit = "2000";
        string newFittingScore = "0";
        string newLeastTestMistakes = "2000";
        string newFastestTest = "2000";
        string newTestingScore = "0";

        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "INSERT INTO dbo.ElecCabLeaderBoard (UserID, UserName, LeastMistakesFitting, FastestTimeFitting, FittingHighScore, LeastMistakesTesting, FastestTimeTesting, TestingHighScore) VALUES ('" + newUserID + "', '" + newUserName + "', '" + newLeastFittingMistakes + "', '" + newFastestFit + "', '" + newFittingScore + "', '" + newLeastTestMistakes + "', '" + newFastestTest + "', '" + newTestingScore + "')";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        StartCoroutine(ReturnToLogin());
    }
    IEnumerator ReturnToLogin()
    {
        UserCreatedText.enabled = true;
        UserCreatedText.text = "User " + usernameString + " created";
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
