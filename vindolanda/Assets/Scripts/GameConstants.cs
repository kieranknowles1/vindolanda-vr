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

    public PlayerController Player;
    public QuestController QuestController;
}
