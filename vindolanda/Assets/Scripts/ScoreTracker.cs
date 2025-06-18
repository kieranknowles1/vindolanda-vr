using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public int Score { get; private set; }
    public int TotalHits { get; private set; }

    public void Reset()
    {
        Score = 0;
        TotalHits = 0;
    }

    public void AddScore(int score)
    {
        Score += score;
        TotalHits++;
    }
}
