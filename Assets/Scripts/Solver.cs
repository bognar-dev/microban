using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Solver
{
    class Node
    {
        public Node parent;
        public GameState.Action incomingAction;
        public GameState state; 
    }

    static List<GameState.Action> allActions = new List<GameState.Action>{
        GameState.Action.Up,
        GameState.Action.Down,
        GameState.Action.Left,
        GameState.Action.Right
    };

    public static List<GameState.Action> Solve(GameState state)
    {
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // TODO: implement your solver!

        // TIP: your solver WILL sometimes take a very long time to run, and will lock up the Unity editor.
        // This will be annoying when you have to go into Task Manager and kill the Unity process.
        // To help mitigate this, put the following code somewhere in the main loop of your solver:
        if (stopwatch.Elapsed.Seconds > 10)
        {
            Debug.LogError("Solver timed out");
            return new List<GameState.Action>();
        }
        // This will kill the solver if it takes longer than 10 seconds, and the game will skip the level and move on to the next.

        // Placeholder
        return new List<GameState.Action>();
    }
}
