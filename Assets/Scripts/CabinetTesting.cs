using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// CabinetTesting handles the testing workflow
/// </summary>
public class CabinetTesting: MonoBehaviour, IGameLogic<float> {

    public GameObject statusIndicator;
    public int unitsCorrect;
    public GameData gameData;
    public GameObject testingCompleteButton;

    private List<ElectricalCabinetBlock> listOfCabinetBlocks;
    private List<GameObject> listOfStatusBlock; 

    private IDbConnection dbcon;
    string connectionString =
    "Server=127.0.0.1;" +
    "Database=CTPDigitalTwin;" +
    "USER ID=MagillMax;" +
    "Password=minnows37;";

    bool taskCompleted;
    float timeTakenToFix;
    int unitsIncorrect;

    //IGameLogic interface
    public int numOfTasks { get; set; }
    public int numOfLivesUsed { get; set; }
    public float timeTakenToComplete { get; set; }
    public float accuracyOfCompletion { get; set; }
    public float endScore { get; set; }
    public float currentHighScore { get; set; }
    public float currentFastestTime { get; set; }


    private void Update()
    {
        if (listOfStatusBlock.Count == unitsCorrect)
        {
            taskCompleted = true;
            if (taskCompleted)
            {
                timeTakenToComplete = timeTakenToFix;
                accuracyOfCompletion = GetAccuracy(listOfStatusBlock.Count, numOfLivesUsed);
                endScore = GetScore(timeTakenToComplete, accuracyOfCompletion);
                CheckScores();
                testingCompleteButton.SetActive(true);
                
            }
        }
        if (!taskCompleted)
        {
            timeTakenToFix += Time.deltaTime;
        }
    }

    //Called when beginning the testing workflow, get necesary gamedata and initialises blocks
    public void MovingToTesting(int numOfMistakes)
    {
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        testingCompleteButton.SetActive(false);
        unitsCorrect = 0;
        numOfLivesUsed = numOfMistakes;
        taskCompleted = false;
        listOfCabinetBlocks = new List<ElectricalCabinetBlock>();
        listOfStatusBlock = new List<GameObject>();
        dbcon = new SqlConnection(connectionString);
        dbcon.Open();
        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "SELECT * FROM ElecCabStatus";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            ElectricalCabinetBlock elecBlock = new ElectricalCabinetBlock(int.Parse(reader["BlockID"].ToString()), float.Parse(reader["BlockX"].ToString()), float.Parse(reader["BlockY"].ToString()), int.Parse(reader["BlockStatus"].ToString()));
            listOfCabinetBlocks.Add(elecBlock);
        }
        reader.Close();
        reader = null;
        CreateBlocks(); //Position the blocks in the cabinet

    }

    //Creates a representative of the condition of a block based on the fitting data
    private void CreateBlocks()
    {
        for (int i = 0; i < listOfCabinetBlocks.Count; i++)
        {
            GameObject block = Instantiate(statusIndicator, transform);
            block.name = "Block" + listOfCabinetBlocks[i].blockID.ToString();
            block.transform.localPosition = new Vector3(listOfCabinetBlocks[i].blockXPos, listOfCabinetBlocks[i].blockYPos, 0);
     
            if (listOfCabinetBlocks[i].blockStatus == 1)
            {
                Material mat = block.GetComponent<Renderer>().material;
                mat.color = Color.green; //If the block is correct (status 1) set its colour to green as a representative
                mat.SetColor("_EmissionColor", Color.green);
                unitsCorrect++;
            }

            listOfStatusBlock.Add(block);
        }
        if (listOfStatusBlock.Count == unitsCorrect)
        {
            timeTakenToComplete = timeTakenToFix;
            accuracyOfCompletion = GetAccuracy(listOfStatusBlock.Count, numOfLivesUsed);
            endScore = GetScore(30, accuracyOfCompletion);
            CheckScores();
        }
    }
    //TestingCompleted is whenthe workflow is finished, so the experience is added to the user and new level is calculated
    public void TestingCompleted()
    {
        gameData.experienceMultiplier += endScore;
        float finalExpierence = 10 * gameData.experienceMultiplier;
        float currExperience = float.Parse(gameData.userXP);
        finalExpierence += currExperience;
        Debug.Log(finalExpierence);
        gameData.userXP = finalExpierence.ToString(); 
        if (finalExpierence > 1 && finalExpierence < 2)
        {
            gameData.userCurrentLevel = "1";
        }
        else if (finalExpierence > 2 && finalExpierence < 3)
        {
            gameData.userCurrentLevel = "2";
        }
        else if (finalExpierence > 3 && finalExpierence < 4)
        {
            gameData.userCurrentLevel = "3";
        }
        else if (finalExpierence > 4 && finalExpierence < 5)
        {
            gameData.userCurrentLevel = "4";
        }
        else if (finalExpierence > 5 && finalExpierence < 6)
        {
            gameData.userCurrentLevel = "5";
        }
        else if (finalExpierence > 6 && finalExpierence < 7)
        {
            gameData.userCurrentLevel = "6";
        }
        else if (finalExpierence > 7 && finalExpierence < 8)
        {
            gameData.userCurrentLevel = "7";
        }
        else if (finalExpierence > 8 && finalExpierence < 9)
        {
            gameData.userCurrentLevel = "8";
        }
        else if (finalExpierence > 9 && finalExpierence < 10)
        {
            gameData.userCurrentLevel = "9";
        }
        else if (finalExpierence > 10 )
        {
            gameData.userCurrentLevel = "10";
        }
        UpdateUserInfo();
        gameData.UserLoggingOut();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    //Updates the userInformation table with new xp, level and jobs completed
    public void UpdateUserInfo()
    {
        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "UPDATE UserInformation SET UserLevel = '" + gameData.userCurrentLevel + "', UserXP = '" + gameData.userXP + "', CompletedWorkFlow = '" + gameData.userJobsCompleted + "' WHERE UserID = '" + gameData.userID + "'";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
    }
    //IGameLogic interface
    public float GetAccuracy(float tasks, float lives)
    {
        float accuracy = tasks - lives;
        return accuracy;
    }

    public float GetScore(float time, float accuracy)
    {
        float finalScore = time / accuracy;
        return finalScore;
    }

    public float GetHighScore()
    {
        float dbHighScore = float.Parse(gameData.currTestingHighScore);
        return currentHighScore;
    }

    public float GetFastestTime()
    {
        float dbFastestTime = float.Parse(gameData.currFastestTimeTesting);
        return dbFastestTime;
    }

    public void CheckScores()
    {
        currentHighScore = GetHighScore();
        currentFastestTime = GetFastestTime();

        bool numbersHaveChanged = false;
        if (numOfLivesUsed < float.Parse(gameData.currLeastTestingMistakes))
        {
            gameData.currLeastTestingMistakes = numOfLivesUsed.ToString();
            numbersHaveChanged = true;
        }
        if (timeTakenToComplete < currentFastestTime)
        {
            gameData.currFastestTimeTesting = timeTakenToComplete.ToString();
            numbersHaveChanged = true;
        }
        if (endScore > currentHighScore)
        {
            gameData.currTestingHighScore = endScore.ToString();
            numbersHaveChanged = true;
        }
        if (numbersHaveChanged)
        {
            UpdateScoreBoard(numOfLivesUsed, timeTakenToComplete, endScore);
        }
    }

    public void UpdateScoreBoard(float newNumOfLives, float newBestTime, float newHighScore)
    {
        IDbCommand dbcmd = dbcon.CreateCommand();
        string cmdSql = "UPDATE ElecCabLeaderBoard SET LeastMistakesTesting = '" + newNumOfLives + "', FastestTimeTesting = '" + newBestTime + "', TestingHighScore = '" + newHighScore + "' WHERE UserID = '" + gameData.userID + "'";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        gameData.userJobsCompleted = "3";

    }
    //IGameLogic End
}


public class ElectricalCabinetBlock //Class for the block, holds ID, Position and Status
{
    public int blockID;
    public float blockXPos;
    public float blockYPos;
    public int blockStatus;

    public ElectricalCabinetBlock(int id, float posX, float posY, int status)
    {
        blockID = id;
        blockXPos = posX;
        blockYPos = posY;
        blockStatus = status;
    }
}
