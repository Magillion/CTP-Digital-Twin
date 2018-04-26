using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// CabinetFitting controls the fitting workflow
/// </summary>
public class CabinetFitting : MonoBehaviour, IGameLogic<float> {

    //Connection to SQL database
    private IDbConnection dbcon;
    string connectionString =
        "Server=127.0.0.1;" +
        "Database=CTPDigitalTwin;" +
        "USER ID=MagillMax;" +
        "Password=minnows37;";

    //IGameLogic interface,
    public int numOfTasks { get; set; }
    public int numOfLivesUsed { get; set; }
    public float timeTakenToComplete { get; set; }
    public float accuracyOfCompletion { get; set; }
    public float endScore { get; set; }
    public float currentHighScore { get; set; }
    public float currentFastestTime { get; set; }
    
    
    public GameObject instructionCanvas;
    public GameObject workflowInfoCanvas;
    public GameObject unitObject;
    public GameObject unitParent;
    public GameObject designUnit;
    public Slider completionSlider;
    public GameObject completeButton;
    public GameData gameData;
    public Text timerText;
    GameObject testingCabinet;

    public TextAsset csvFile;

    List<Vector3> unitDesignPosistions = new List<Vector3>();

    bool workFlowActive;
    float completeTime;
    int currDesignPos;
    int numOfMistakes;
    int numOfUnitsPlaced;
    bool workflowComplete = false;

    void OnEnable ()
    {
        GameObject.Find("CompleteTestingButton").SetActive(false);
        workflowInfoCanvas.SetActive(false); //Disable the canvas for the actual workflow, done OnEnable so it can be referenced later

        //Database information 
        dbcon = new SqlConnection(connectionString);
        dbcon.Open();

        ReadCSVFile();
        workflowComplete = false;
        currDesignPos = 0;
        numOfMistakes = 0;
        numOfUnitsPlaced = 0;
        testingCabinet = GameObject.Find("ElectricalCabinetTesting");
        testingCabinet.SetActive(false);

        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        gameData.GetWokFlowLeaderBoardFromTable(gameData.userID);
    }


    private void Update()
    {
        if (workFlowActive)
        {
            completeTime += Time.deltaTime; //Complete time is the time it takes to complete the task
            timerText.text = completeTime.ToString("n2");
        }
    }

    //If the button is pressed to begin fitting, call this function
    public void BeginFitting()
    {
        completeButton.SetActive(false);

        instructionCanvas.SetActive(false);
        workflowInfoCanvas.SetActive(true);
        workFlowActive = true;
        CreateObjects();
    }

    //Create a new unit for placing
    void CreateObjects()
    {
        Instantiate(unitObject, unitParent.transform);
        designUnit.transform.localPosition = unitDesignPosistions[currDesignPos];
        currDesignPos++;
        if (currDesignPos >= unitDesignPosistions.Count)
        {
            workflowComplete = true;
        }
    }

    //Check the units position
    public void CheckUnitPosition(Vector3 unitPos)
    {
        if (unitPos == designUnit.transform.localPosition)
        {
            AddBlockToDatabase(unitPos, 1);
        }
        else
        {
            AddBlockToDatabase(unitPos, 0);
            numOfMistakes++;
        }
    }

    //Add the newly placed units information to the database
    void AddBlockToDatabase(Vector3 currentUnitPos, int unitIsCorrect)
    {
        numOfUnitsPlaced++;
        IDbCommand dbcmd = dbcon.CreateCommand();                                                     
        string cmdSql = "INSERT INTO dbo.ElecCabStatus (BlockID, BlockX, BlockY, BlockStatus) VALUES ('" + numOfUnitsPlaced + "', '" + currentUnitPos.x + "', '" + currentUnitPos.y + "', '" + unitIsCorrect + "')";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        if (!workflowComplete)
        {
            completionSlider.value += 1;
            CreateObjects();
        }
        else
        {
            completionSlider.value = completionSlider.maxValue;
            workFlowActive = false;
            FittingComplete();
        }
    }

    //Called once the fitting is completed, calculations mostly
    void FittingComplete()
    {
        timerText.text = null;
        numOfLivesUsed = numOfMistakes;
        timeTakenToComplete = completeTime;
        accuracyOfCompletion = GetAccuracy(numOfTasks, numOfLivesUsed);
        endScore = GetScore(timeTakenToComplete, accuracyOfCompletion);
        gameData.experienceMultiplier = endScore;
        CheckScores();

        completeButton.SetActive(true);
        GameObject.Find("UserInfoText").GetComponent<Text>().text = "Fitting Complete! Mistakes made: " + numOfMistakes + ". Time taken: " + completeTime.ToString("N2") + ". Click button to move on";
    }

    //Called once the button is clicked to completed the fitting, cleanup mostly
    public void WorkFlowIsComplete()
    {
        GameObject.Find("ElecDesign").SetActive(false);
        gameObject.SetActive(false);
        testingCabinet.SetActive(true);
        testingCabinet.GetComponent<CabinetTesting>().MovingToTesting(numOfMistakes);
        completeButton.SetActive(false);
        completionSlider.gameObject.SetActive(false);
        GameObject.Find("UserInfoText").SetActive(false);
    }

    //Reads the CSV to get positions of blocks
    void ReadCSVFile()
    {
        string[] positions = csvFile.text.Split('\n');//split by new line
        foreach (string position in positions)
        {
            string[] coordinates = position.Split(',');//split by comma
            unitDesignPosistions.Add(new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]), float.Parse(coordinates[2])));
        }
        completionSlider.maxValue = unitDesignPosistions.Count;
        numOfTasks = unitDesignPosistions.Count;
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
        currentHighScore = float.Parse(gameData.currFittingHighScore);
        return currentHighScore;
    }

    public float GetFastestTime()
    {
        currentFastestTime = float.Parse(gameData.currFastestTimeFitting);
        return currentFastestTime;
    }

    public void CheckScores()
    {
        GetHighScore();
        GetFastestTime();
        bool numbersHaveChanged = false;
        if (numOfLivesUsed < float.Parse(gameData.currLeastFittingMistakes))
        {
            gameData.currLeastFittingMistakes = numOfLivesUsed.ToString();
            numbersHaveChanged = true;
        }
        if (timeTakenToComplete < currentFastestTime)
        {
            gameData.currFastestTimeFitting = timeTakenToComplete.ToString();
            numbersHaveChanged = true;
        }
        if (endScore > currentHighScore)
        {
            gameData.currFittingHighScore = endScore.ToString();
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
        string cmdSql = "UPDATE ElecCabLeaderBoard SET LeastMistakesFitting = '" + newNumOfLives + "', FastestTimeFitting = '" + newBestTime + "', FittingHighScore = '" + newHighScore + "' WHERE UserID = '" + gameData.userID + "'";
        dbcmd.CommandText = cmdSql;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        gameData.userJobsCompleted = "2";
    }
    //Interface end
}
