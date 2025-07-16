using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class ScenePlayer : TriggerBase
{
    public List<BehaviorGraph> scenes;
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
        if (scenes.Count == 0 && playedScenes.Count == 0)
        {
            Debug.LogError("No scenes to play, disabling", this);
            enabled = false;
        }
    }

    BehaviorGraph SelectScene()
    {
        // If everything has been played, shuffle scenes back into the deck
        if (scenes.Count == 0)
        {
            (playedScenes, scenes) = (scenes, playedScenes);
            // Don't replay the latest scene unless it is the only one in the deck
            if (scenes.Count > 1)
            {
                var latest = scenes.PopBack();
                // scenes.Count > 1 here, so the shuffle check will fail. No possiblility of stack overflow
                var choice = SelectScene();
                scenes.Add(latest);
                return choice;
            }
        }

        // Select something that hasn't been played before
        int index = Random.Range(0, scenes.Count);
        var scene = scenes[index];
        scenes.RemoveAtSwapBack(index);
        playedScenes.Add(scene);
        return scene;
    }

    IEnumerator StartSceneDelayed()
    {
        float delay = Random.Range(startDelayMin, startDelayMax);
        yield return new WaitForSeconds(delay);
        agent.Graph = SelectScene();
        startupRoutine = null;
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
