using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Data;
using System.Data.SqlClient;


/// <summary>
/// MainMenu handles the functionalityof the buttons on the main menu
/// </summary>
public class MainMenu : MonoBehaviour {

    //----SQL SETUP-----
    private IDbConnection dbcon;

    string connectionString =
        "Server=127.0.0.1;" +
        "Database=CTPDigitalTwin;" +
        "USER ID=MagillMax;" +
        "Password=minnows37;";
    //-----------------

    //Database related
    public string usernameID;
    public string username;
    public GameData gameData;
    //----------------------

    public GameObject LoginScreen;
    public GameObject statScreenGameObject;
    private StatsScreen statsScreen;
    //---UI related---
    private Text userNameText;
    private Button viewStatsButton;
    private Button contCurrWorkflowButton;
    private Button logOutButton;
    //-------------



    private void Start()
    {
        userNameText = GameObject.Find("UserNameText").GetComponent<Text>();
        viewStatsButton = GameObject.Find("ViewStatsButton").GetComponent<Button>();
        contCurrWorkflowButton = GameObject.Find("CurrentWorkFlowButton").GetComponent<Button>();
        logOutButton = GameObject.Find("LogOutButton").GetComponent<Button>();
        statsScreen = statScreenGameObject.GetComponent<StatsScreen>();
        gameObject.SetActive(false);
    }

    void OnEnable ()
    {
        UpdateMainMenu();
    }
	
    public void UpdateMainMenu()
    {
        userNameText.text = "Welcome " + gameData.userFirstName + "\nLevel: " + gameData.userCurrentLevel;
        viewStatsButton.GetComponentInChildren<Text>().text = "View Leaderboard";
        contCurrWorkflowButton.GetComponentInChildren<Text>().text = "Enter " + gameData.userCurrentJob + " Digital twin";
        logOutButton.GetComponentInChildren<Text>().text = "Not " + gameData.userFirstName + "? Log out";
    }

    public void SwitchToWorkFlow()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void ShowUserStats()
    {
        gameObject.SetActive(false);
        statScreenGameObject.SetActive(true);
        statsScreen.SetUpStatScreen(gameData);
    }

    public void UserLogOut()
    {
        gameData.UserLoggingOut();
        gameObject.SetActive(false);
        LoginScreen.SetActive(true);
    }

}


