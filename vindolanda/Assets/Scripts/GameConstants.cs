using UnityEngine;

public class GameConstants
{
    private static GameConstants instance;
    public static GameConstants Instance
    {
        get
        {
            instance ??= new GameConstants();
            return instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        instance = null;
    }

    private GameConstants()
    {
        Player = Object.FindAnyObjectByType<PlayerController>();
        Tour = Object.FindAnyObjectByType<TourController>();
        QuestController = Object.FindAnyObjectByType<Vindolanda.Quest.Controller>();
    }

    public PlayerController Player { get; }
    public TourController Tour { get; }
    public Vindolanda.Quest.Controller QuestController { get; }
}
