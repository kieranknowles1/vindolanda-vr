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

    T FindRequired<T>() where T : Object
    {
        var obj = Object.FindAnyObjectByType<T>();
        if (obj == null)
        {
            Debug.LogError($"Could not find required object of type {typeof(T).FullName}");
        }
        return obj;
    }

    private GameConstants()
    {
        Player = FindRequired<PlayerController>();
        Tour = FindRequired<TourController>();
        QuestController = FindRequired<Vindolanda.Quest.Controller>();
    }

    public PlayerController Player { get; }
    public TourController Tour { get; }
    public Vindolanda.Quest.Controller QuestController { get; }
}
