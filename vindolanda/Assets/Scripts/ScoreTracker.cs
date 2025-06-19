using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class ScoreTracker : MonoBehaviour
{
    public int Score { get; private set; }
    public int TotalHits { get; private set; }

    public DefaultEvent OnTargetScoreReached;
    public DefaultEvent OnMaxHitsReached;
    public int targetScore;
    public int maxHits;

    public VariablesGroupAsset vars;
    IntVariable scoreVar;

    private void Start()
    {
        scoreVar = vars["score"] as IntVariable;
    }

    public void Reset()
    {
        Score = 0;
        TotalHits = 0;
    }

    public void AddScore(int score)
    {
        Score += score;
        TotalHits++;

        scoreVar.Value = Score;

        if (score >= targetScore) OnTargetScoreReached?.SendEventMessage();
        if (TotalHits >= maxHits) OnMaxHitsReached?.SendEventMessage();
    }
}
