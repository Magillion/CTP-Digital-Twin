using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
/// <summary>
/// StatsScreen handles the presentation of the leaderboard and its entries.
/// It has multple different functions used by different buttons that sort data using Linq 
/// </summary>
public class StatsScreen : MonoBehaviour {

    //UI RELATED
    public GameObject leaderBoardEntryGO;
    public GameObject completeBoardEntryGO;
    public GameObject singleLeaderboardGO;
    public GameObject bothLeaderboardGO;
    public GameObject fittingHeader;
    public GameObject testingHeader;
    public Text leaderboardTitle;
    private GameData loggedInUser;
    private List<GameData> listOfLeaderBoardEntries = new List<GameData>();
    public List<GameObject> listOfEntryObjects = new List<GameObject>();
    //-------------------
    public GameObject mainMenu;
    //----SQL SETUP-----
    private IDbConnection dbcon;

    static string connectionString =
        "Server=127.0.0.1;" +
        "Database=CTPDigitalTwin;" +
        "USER ID=MagillMax;" +
        "Password=minnows37;";
    //-----------------

    bool isFullLeaderBoard = false;




	// Use this for initialization
	void Start () {
        bothLeaderboardGO.SetActive(false);
        testingHeader.SetActive(false);
        gameObject.SetActive(false);
        
	}

    public void SetUpStatScreen(GameData gameData)
    {
        loggedInUser = gameData;
        dbcon = new SqlConnection(connectionString);
        dbcon.Open();
        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "SELECT * FROM ElecCabLeaderBoard ORDER BY UserID";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            GameData newGameDataEntry = new GameData(reader["UserID"].ToString(),
                reader["UserName"].ToString(),
                reader["LeastMistakesFitting"].ToString(),
                reader["FastestTimeFitting"].ToString(),
                reader["FittingHighScore"].ToString(),
                reader["LeastMistakesTesting"].ToString(),
                reader["FastestTimeTesting"].ToString(),
                reader["TestingHighScore"].ToString());
            listOfLeaderBoardEntries.Add(newGameDataEntry);
        }
        for (int i = 0; i < listOfLeaderBoardEntries.Count; i++)
        {
            CreateFittingEntries(listOfLeaderBoardEntries[i]);   
        }
        
    }

    protected void CreateFittingEntries(GameData currentEntry)
    {
        leaderboardTitle.text = "Fitting Scoreboard";
        GameObject newEntry = Instantiate(leaderBoardEntryGO, GameObject.Find("PlayerList").transform);
        newEntry.name = currentEntry.userID;
        newEntry.transform.Find("UserID").GetComponent<Text>().text = currentEntry.userID;
        newEntry.transform.Find("Username").GetComponent<Text>().text = currentEntry.userFirstName;
        newEntry.transform.Find("Fastest time").GetComponent<Text>().text = currentEntry.currFastestTimeFitting;
        newEntry.transform.Find("Least Mistakes").GetComponent<Text>().text = currentEntry.currLeastFittingMistakes;
        newEntry.transform.Find("Highscore").GetComponent<Text>().text = currentEntry.currFittingHighScore;
        listOfEntryObjects.Add(newEntry);
    }
    public void SwitchToTestingView()
    {
        DestroyCurrentLeaderBoardEntries();

        if (isFullLeaderBoard)
        {
            singleLeaderboardGO.SetActive(true);
            bothLeaderboardGO.SetActive(false);
            isFullLeaderBoard = false;
        }
        fittingHeader.SetActive(false);
        testingHeader.SetActive(true);
        leaderboardTitle.text = "Testing Scoreboard";

        for (int i = 0; i < listOfLeaderBoardEntries.Count; i++)
        {
            GameObject newEntry = Instantiate(leaderBoardEntryGO, GameObject.Find("PlayerList").transform);
            newEntry.name = listOfLeaderBoardEntries[i].userID;
            newEntry.transform.Find("UserID").GetComponent<Text>().text = listOfLeaderBoardEntries[i].userID;
            newEntry.transform.Find("Username").GetComponent<Text>().text = listOfLeaderBoardEntries[i].userFirstName;
            newEntry.transform.Find("Fastest time").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currFastestTimeTesting;
            newEntry.transform.Find("Least Mistakes").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currLeastTestingMistakes;
            newEntry.transform.Find("Highscore").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currTestingHighScore;
            listOfEntryObjects.Add(newEntry);
        }

    }

    public void SwitchToFittingView()
    {
        DestroyCurrentLeaderBoardEntries();

        if (isFullLeaderBoard)
        {
            singleLeaderboardGO.SetActive(true);
            bothLeaderboardGO.SetActive(false);
            isFullLeaderBoard = false;
        }

        testingHeader.SetActive(false);
        fittingHeader.SetActive(true);
        for (int i = 0; i < listOfLeaderBoardEntries.Count; i++)
        {
            CreateFittingEntries(listOfLeaderBoardEntries[i]);
        }
    }

    public void CreateCompleteTable()
    {
        leaderboardTitle.text = "Complete Scoreboard";
        for (int i = 0; i < listOfLeaderBoardEntries.Count; i++)
        {
            GameObject newEntry = Instantiate(completeBoardEntryGO, GameObject.Find("BothWorkFlowList").transform);
            newEntry.name = listOfLeaderBoardEntries[i].userID;
            newEntry.transform.Find("UserID").GetComponent<Text>().text = listOfLeaderBoardEntries[i].userID;
            newEntry.transform.Find("Username").GetComponent<Text>().text = listOfLeaderBoardEntries[i].userFirstName;
            newEntry.transform.Find("FittingTime").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currFastestTimeFitting;
            newEntry.transform.Find("FittingMistakes").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currLeastFittingMistakes;
            newEntry.transform.Find("FittingHighScore").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currFittingHighScore;
            newEntry.transform.Find("TestingTime").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currFastestTimeTesting;
            newEntry.transform.Find("TestingMistakes").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currLeastTestingMistakes;
            newEntry.transform.Find("TestingHighScore").GetComponent<Text>().text = listOfLeaderBoardEntries[i].currTestingHighScore;
            listOfEntryObjects.Add(newEntry);
        }
    }

    public void SwitchToBothView()
    {
        DestroyCurrentLeaderBoardEntries();
        singleLeaderboardGO.SetActive(false);
        bothLeaderboardGO.SetActive(true);
        isFullLeaderBoard = true;
        CreateCompleteTable();

    }

    public void DestroyCurrentLeaderBoardEntries()
    {
        foreach (GameObject entries in listOfEntryObjects)
        {
            Destroy(entries);
        }
        listOfEntryObjects.Clear();
    }


    public void SortByUserIDFitting()
    {
        List<GameData> userIdOrder = new List<GameData>();
        userIdOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.userID).ToList();

        DestroyCurrentLeaderBoardEntries();

        if (isFullLeaderBoard)
        {
            for (int i = 0; i < userIdOrder.Count; i++)
            {
                listOfLeaderBoardEntries[i] = userIdOrder[i];
            }
            CreateCompleteTable();
        }
        else
        {
            for (int i = 0; i < userIdOrder.Count; i++)
            {
                CreateFittingEntries(userIdOrder[i]);
            }
        }

    }

    public void SortByUsernameFitting()
    {
        List<GameData> userNameOrder = new List<GameData>();
        userNameOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.userFirstName).ToList();

        DestroyCurrentLeaderBoardEntries();

        if (isFullLeaderBoard)
        {
            for (int i = 0; i < userNameOrder.Count; i++)
            {
                listOfLeaderBoardEntries[i] = userNameOrder[i];
            }
            CreateCompleteTable();
        }
        else
        {
            for (int i = 0; i < userNameOrder.Count; i++)
            {
                CreateFittingEntries(userNameOrder[i]);
            }
        }

    }

    public void SortByUserIDTesting()
    {
        List<GameData> userIdOrder = new List<GameData>();
        userIdOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.userID).ToList();

        DestroyCurrentLeaderBoardEntries();
        for (int i = 0; i < userIdOrder.Count; i++)
        {
            listOfLeaderBoardEntries[i] = userIdOrder[i];
        }
        if (isFullLeaderBoard)
        {
            CreateCompleteTable();
        }
        else
        {
            SwitchToTestingView();
        }

    }

    public void SortByUsernameTesting()
    {
        List<GameData> userNameOrder = new List<GameData>();
        userNameOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.userFirstName).ToList();

        DestroyCurrentLeaderBoardEntries();

        for (int i = 0; i < userNameOrder.Count; i++)
        {
            listOfLeaderBoardEntries[i] = userNameOrder[i];
        }
        if (isFullLeaderBoard)
        {
            CreateCompleteTable();
        }
        else
        {
            SwitchToTestingView();
        }

    }

    public void SortByFastestFit()
    {
        List<GameData> fastestTimeOrder = new List<GameData>();
        fastestTimeOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.currFastestTimeFitting).ToList();

        DestroyCurrentLeaderBoardEntries();

        if (isFullLeaderBoard)
        {
            for (int i = 0; i < fastestTimeOrder.Count; i++)
            {
                listOfLeaderBoardEntries[i] = fastestTimeOrder[i];
            }
            CreateCompleteTable();
        }
        else
        {
            for (int i = 0; i < fastestTimeOrder.Count; i++)
            {
                CreateFittingEntries(fastestTimeOrder[i]);
            }
        }

    }

    public void SortByLeastFittingMistakes()
    {
        List<GameData> leastMistakesOrder = new List<GameData>();
        leastMistakesOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.currLeastFittingMistakes).ToList();

        DestroyCurrentLeaderBoardEntries();
        if (isFullLeaderBoard)
        {
            for (int i = 0; i < leastMistakesOrder.Count; i++)
            {
                listOfLeaderBoardEntries[i] = leastMistakesOrder[i];
            }
            CreateCompleteTable();
        }
        else
        {
            for (int i = 0; i < leastMistakesOrder.Count; i++)
            {
                CreateFittingEntries(leastMistakesOrder[i]);
            }
        }

    }

    public void SortByFittingScore()
    {
        List<GameData> highScoreOrder = new List<GameData>();
        highScoreOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.currFittingHighScore).ToList();

        DestroyCurrentLeaderBoardEntries();

        if (isFullLeaderBoard)
        {
            for (int i = 0; i < highScoreOrder.Count; i++)
            {
                listOfLeaderBoardEntries[i] = highScoreOrder[i];
            }
            CreateCompleteTable();
        }
        else
        {
            for (int i = 0; i < highScoreOrder.Count; i++)
            {
                CreateFittingEntries(highScoreOrder[i]);
            }
        }

    }

    public void SortByFastestTest()
    {
        List<GameData> fastestTimeOrder = new List<GameData>();
        fastestTimeOrder = listOfLeaderBoardEntries.OrderBy(x => x.currFastestTimeTesting).ToList(); //Set to OrderBy to avoid strange ordering issue

        DestroyCurrentLeaderBoardEntries();
        for (int i = 0; i < fastestTimeOrder.Count; i++)
        {
            listOfLeaderBoardEntries[i] = fastestTimeOrder[i];
        }
        if (isFullLeaderBoard)
        {
            CreateCompleteTable();
        }
        else
        {
            SwitchToTestingView();
        }

   
    }

    public void SortByLeastTestMistakes()
    {
        List<GameData> leastMistakesOrder = new List<GameData>();
        leastMistakesOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.currLeastTestingMistakes).ToList();

        DestroyCurrentLeaderBoardEntries();

        for (int i = 0; i < leastMistakesOrder.Count; i++)
        {
            listOfLeaderBoardEntries[i] = leastMistakesOrder[i];
        }
        if (isFullLeaderBoard)
        {
            CreateCompleteTable();
        }
        else
        {
            SwitchToTestingView();
        }

    }

    public void SortByTestingScore()
    {
        List<GameData> testScoreOrder = new List<GameData>();
        testScoreOrder = listOfLeaderBoardEntries.OrderByDescending(x => x.currTestingHighScore).ToList();

        DestroyCurrentLeaderBoardEntries();

        for (int i = 0; i < testScoreOrder.Count; i++)
        {
            listOfLeaderBoardEntries[i] = testScoreOrder[i];
        }

        if (isFullLeaderBoard)
        {
            CreateCompleteTable();
        }
        else
        {
            SwitchToTestingView();
        }

    }
    
    public void ReturnToMainMenu()
    {
        foreach (GameObject entries in listOfEntryObjects)
        {
            Destroy(entries);
        }
        listOfEntryObjects.Clear();
        listOfLeaderBoardEntries.Clear();
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}


