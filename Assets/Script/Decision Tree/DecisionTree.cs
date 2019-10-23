using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    public AIcuñaMap map;

    private List<IDecision> decisions;

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
                    CreateDecision(nodes[i]);
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

    private void CreateDecision(int id)
    {
        if (decisions == null)
        {
            decisions = new List<IDecision>();
        }



        /*
        _buildAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        
        _theresWoodQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _buildAction, _cutAction);

        _rootAI = _hasLifeOptions;
        */
    }
}
