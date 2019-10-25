using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    public AIcuñaMap map;

    private List<IDecision> _decisions;

    private IDecision _rootAI;

    private void Start()
    {
        GenerateMyAI();
    }

    private void Update()
    {
        if (_rootAI != null) _rootAI.Execute();
    }

    private void GenerateMyAI()
    {
        List<int> nodes = new List<int>();
        List<int> done = new List<int>();
        List<int> unready = new List<int>();

        for (int i = 0; i < map.nodes.Count; i++)
        {
            nodes.Add(i);
        }

        while (nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (IsReadyToCreate(nodes[i], done))
                {
                    CreateDecision(nodes[i], done);
                    done.Add(i);
                }
                else
                {
                    unready.Add(nodes[i]);
                }

                nodes.RemoveAt(i);
            }

            nodes = new List<int>(unready);
            unready.Clear();
        }

        SetRoot(done);
    }

    private bool IsReadyToCreate(int id, List<int> ready)
    {
        for (int i = 0; i < map.connections.Count; i++)
        {
            if (map.connections[i].outPoint.node == map.nodes[id])
            {
                if (!ready.Contains(map.nodes.IndexOf(map.connections[i].inPoint.node)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void CreateDecision(int id, List<int> ready)
    {
        if (_decisions == null)
        {
            _decisions = new List<IDecision>();
        }

        if (map.nodes[id].GetType() == typeof(ActionNode))
        {
            ActionNode a = (ActionNode)map.nodes[id];

            _decisions.Add(new ActionDT(a.action));
        }

        if (map.nodes[id].GetType() == typeof(QuestionNode))
        {
            QuestionNode q = (QuestionNode)map.nodes[id];
            IDecision t = null;
            IDecision f = null;

            for (int i = 0; i < map.connections.Count; i++)
            {
                if (map.connections[i].outPoint == q.truePoint)
                {
                    t = _decisions[ready.IndexOf(map.nodes.IndexOf(map.connections[i].inPoint.node))];
                }
                if (map.connections[i].outPoint == q.falsePoint)
                {
                    f = _decisions[ready.IndexOf(map.nodes.IndexOf(map.connections[i].inPoint.node))];
                }
            }

            _decisions.Add(new QuestionDT(q.question, t, f));
        }
    }

    private void SetRoot(List<int> ready)
    {
        for (int i = 0; i < map.nodes.Count; i++)
        {
            bool isRoot = true;

            for (int j = 0; j < map.connections.Count; j++)
            {
                if (map.connections[j].inPoint.node == map.nodes[i])
                {
                    isRoot = false;
                    break;
                }
            }

            if(isRoot)
            {
                _rootAI = _decisions[ready.IndexOf(i)];
                break;
            }
        }
    }
}
