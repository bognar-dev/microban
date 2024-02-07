using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Solver
{
    class Node
    {
        public Node(Node parent, GameState.Action incomingAction, GameState state)
        {
            this.parent = parent;
            this.incomingAction = incomingAction;
            this.state = state;
        }
        public Node(GameState state)
        {
            this.state = state;
        }
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

        HashSet<GameState> seenNodes = new HashSet<GameState>();
        Node root = new Node(state);
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(root);
        while (q.Count != 0)
        {
            if (stopwatch.Elapsed.Seconds > 10)
            {
                Debug.LogError("Solver timed out");
                return new List<GameState.Action>();
            }
            Node currentNode = q.Dequeue();
            List<GameState.Action> availableActions = new List<GameState.Action>();
            foreach (GameState.Action a in allActions)
            {
                if (currentNode.state.IsActionAvailable(a))
                {

                    availableActions.Add(a);
                }
            }
            foreach (GameState.Action a in availableActions)
            {
                GameState nextState = currentNode.state.Copy();
                nextState.DoAction(a);
                Node child = new Node(currentNode, a, nextState);
                if (nextState.IsSolved)
                {
                    return ReconstructPlan(child);
                }
                if (!seenNodes.Contains(nextState))
                {
                    seenNodes.Add(nextState);
                    q.Enqueue(child);
                }
            }

        }

        return new List<GameState.Action>();
    }

    private static List<GameState.Action> ReconstructPlan(Node node)
    {
        List<GameState.Action> plan = new List<GameState.Action>();
        while (node != null)
        {
            plan.Insert(0, node.incomingAction);
            node = node.parent;
        }


        return plan;
    }
}
