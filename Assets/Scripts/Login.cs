using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Login handles the login of users, mostly unity GUI objects, but also checks the user exists
/// </summary>
public class Login : MonoBehaviour
{

    public GameObject mainMenuGameObject;
    public GameData gameData;
    private MainMenu mainMenu;

    private IDbConnection dbcon;

    string connectionString = 
        "Server=127.0.0.1;" +
        "Database=CTPDigitalTwin;" +
        "USER ID=MagillMax;" +
        "Password=minnows37;";


    private string UserID;
    private string password;

    private string usernameString = string.Empty;
    private string passwordString = string.Empty;

    private Rect windowsRect = new Rect(0, 0, 1920, 1080);
    public Text incorrectDetailsText;

    private void OnEnable()
    {
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        incorrectDetailsText.enabled = false;
        mainMenu = mainMenuGameObject.GetComponent<MainMenu>();
        dbcon = new SqlConnection(connectionString);
        dbcon.Open();
    }

    private void OnGUI()
    {
        GUI.Window(0, windowsRect, WindowFunction, "Login");
    }

    void WindowFunction(int windowID)
    {
        GUI.skin.textField.fontSize = 30;
        usernameString = GUI.TextField(new Rect(Screen.width / 3, 2 * Screen.height / 5, Screen.width / 3, Screen.height / 10), usernameString, 10);

        passwordString = GUI.PasswordField(new Rect(Screen.width / 3, 2 * Screen.height / 3, Screen.width / 3, Screen.height / 10), passwordString, "*"[0], 10);

        if (GUI.Button(new Rect(Screen.width / 2, 4 * Screen.height / 5, Screen.width / 8, Screen.height / 8), "Login"))
        {
            SearchTableForUser(usernameString, passwordString);
            usernameString = "";
            passwordString = "";
        }
        if (GUI.Button(new Rect(Screen.width / 3, 4 * Screen.height / 5, Screen.width / 8, Screen.height / 8), "Create User"))
        {
            //Switches to new scene
       
            SceneManager.LoadScene(1,LoadSceneMode.Single);

        }

        GUI.Label(new Rect(Screen.width / 3, 35 * Screen.height / 100, Screen.width / 5, Screen.height / 8), "Username");
        GUI.Label(new Rect(Screen.width / 3, 62 * Screen.height / 100, Screen.width / 8, Screen.height / 8), "Password");
    }


    void SearchTableForUser(string u, string p)
    {
        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "SELECT UserId FROM dbo.LoginDetails WHERE Username = '" + u + "' AND Password = '" + p + "'";
        Debug.Log(cmdSql);
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            if (reader["UserId"] != null)
            {
                UserID = reader["UserID"].ToString();
                gameObject.SetActive(false);
                gameData.GetUserDataFromTable(UserID);
                mainMenuGameObject.SetActive(true);
            }
        }
        reader.Close();
        reader = null;

    }

}

