using System;
using UnityEngine;

public class DecisionTree<Client, Result>
{
    public Node root;
    public bool showDebugInfo;
    public Result Evaluate(Client client)
    {
        if (showDebugInfo)
            Debug.Log("START DECISION");
        Result res = root.Evaluate(client, showDebugInfo);
        if (showDebugInfo)
            Debug.Log("END DECISION");

        return res;
    }

    public class Node
    {
        public Node positive;
        public Node negative;

        public Result decision;
        public Func<Client, bool> Condition;

        public Result Evaluate(Client client, bool showDebugInfo)
        {
            if (Condition == null)
            {
                if (decision == null)
                {
                    Debug.LogWarning("DecisionTree LeafNode doesn't have a descision.");
                    return default(Result);
                }
                else
                {
                    return decision;
                }
            }
            bool res = Condition(client);

            if (res)
            {
                if (positive == null)
                {
                    Debug.LogWarning("DecisionTree incomplete.");
                    return default(Result);
                }

                if (showDebugInfo)
                    Debug.Log("Going Positive");
                return positive.Evaluate(client, showDebugInfo);
            }
            else
            {
                if (negative == null)
                {
                    Debug.LogWarning("DecisionTree incomplete.");
                    return default(Result);
                }
                if (showDebugInfo)
                    Debug.Log("Going Negative");
                return negative.Evaluate(client, showDebugInfo);
            }
        }
    }
}