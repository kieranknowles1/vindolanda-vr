using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class ScenePlayer : TriggerBase
{
    [System.Serializable]
    public struct SceneEntry
    {
        public int weight;
        public BehaviorGraph graph;
    }

    public List<SceneEntry> allScenes;
    List<BehaviorGraph> availableScenes = new();
    List<BehaviorGraph> playedScenes = new();

    [Min(0)]
    public float startDelayMin;
    [Min(0)]
    public float startDelayMax;

    BehaviorGraphAgent agent;

    private void Start()
    {
        agent = GetComponent<BehaviorGraphAgent>();

        // Safety check: We can't do anything if there are no scenes
        if (allScenes.Count == 0)
        {
            Debug.LogError("No scenes to play, disabling", this);
            enabled = false;
        }

        // Build the deck of available scenes
        foreach (var entry in allScenes)
        {
            for (int i = 0; i < entry.weight; i++)
            {
                availableScenes.Add(entry.graph);
            }
        }
    }

    BehaviorGraph SelectScene()
    {
        // If everything has been played, shuffle scenes back into the deck
        if (availableScenes.Count == 0)
        {
            (playedScenes, availableScenes) = (availableScenes, playedScenes);
            // Don't replay the latest scene unless it is the only one in the deck
            if (availableScenes.Count > 1)
            {
                var latest = availableScenes.PopBack();
                // scenes.Count > 1 here, so the shuffle check will fail. No possiblility of stack overflow
                var choice = SelectScene();
                availableScenes.Add(latest);
                return choice;
            }
        }

        // Select something that hasn't been played before
        int index = Random.Range(0, availableScenes.Count);
        var scene = availableScenes[index];
        availableScenes.RemoveAtSwapBack(index);
        playedScenes.Add(scene);
        return scene;
    }

    IEnumerator StartSceneDelayed()
    {
        while (PlayerPresent)
        {
            float delay = Random.Range(startDelayMin, startDelayMax);
            yield return new WaitForSeconds(delay);
            agent.Graph = SelectScene();
            do {
                yield return new WaitForSeconds(1.0f);
            } while (agent.Graph.IsRunning);
            agent.Graph = null;
        }
    }
    Coroutine startupRoutine;

    protected override void Execute(PlayerController player)
    {
        startupRoutine = StartCoroutine(StartSceneDelayed());
    }

    protected override void ExecuteExit(PlayerController player)
    {
        if (startupRoutine != null)
        {
            StopCoroutine(startupRoutine);
            startupRoutine = null;
        }
    }
}
