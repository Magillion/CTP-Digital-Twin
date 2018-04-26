/// <summary>
/// The IGameLogic interface is the interface implemented by all digital twins allowing them to becomming games.
/// The GameData class acts as a middle man
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGameLogic<T>
{
    int numOfTasks { get; set; }
    int numOfLivesUsed { get; set; }
    float timeTakenToComplete { get; set; }
    float accuracyOfCompletion { get; set; }
    float endScore { get; set; }
    float currentHighScore { get; set; }
    float currentFastestTime { get; set; }

    float GetAccuracy(T tasks, T lives);
    float GetScore(T time, T accuracy);
    float GetHighScore();
    float GetFastestTime();
    void CheckScores();
    void UpdateScoreBoard(T newNumOfLives, T newBestTime, T newHighScore);
}
