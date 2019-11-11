using System;
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
        List<Node> allNodes = new List<Node>();
        List<int> nodesIndex = new List<int>();
        List<int> doneIndex = new List<int>();
        List<int> unreadyIndex = new List<int>();

        foreach (var item in map.actions)
        {
            item.action = (Action)Delegate.CreateDelegate(typeof(Action), GameObject.Find(item.goName).GetComponent(item.scriptName) as MonoBehaviour, item.methodName);
            allNodes.Add(item);
        }

        foreach(var item in map.questions)
        {
            item.question = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), GameObject.Find(item.goName).GetComponent(item.scriptName) as MonoBehaviour, item.methodName);
            allNodes.Add(item);
        }

        for (int i = 0; i < allNodes.Count; i++)
        {
            nodesIndex.Add(i);
        }

        while (nodesIndex.Count > 0)
        {
            for (int i = 0; i < nodesIndex.Count; i++)
            {
                if (IsReadyToCreate(nodesIndex[i], doneIndex, allNodes))
                {
                    CreateDecision(nodesIndex[i], doneIndex, allNodes);
                    doneIndex.Add(nodesIndex[i]);
                }
                else
                {
                    unreadyIndex.Add(nodesIndex[i]);
                }

                nodesIndex.RemoveAt(i);
            }

            nodesIndex = new List<int>(unreadyIndex);
            unreadyIndex.Clear();
        }
        SetRoot(doneIndex, allNodes);
    }

    private bool IsReadyToCreate(int id, List<int> ready, List<Node> data)
    {
        for (int i = 0; i < map.connections.Count; i++)
        {
            if (map.connections[i].outPoint.nodeID == data[id].id)
            {
                for (int j = 0; j < ready.Count; j++)
                {
                    if (map.connections[i].inPoint.nodeID == data[ready[j]].id)
                        return true;
                }

                return false;
            }
        }

        return true;
    }

    private void CreateDecision(int id, List<int> ready, List<Node> data)
    {
        if (_decisions == null)
        {
            _decisions = new List<IDecision>();
        }

        if (data[id].GetType() == typeof(ActionNode))
        {
            ActionNode a = (ActionNode)data[id];

            _decisions.Add(new ActionDT(a.action));
        }

        if (data[id].GetType() == typeof(QuestionNode))
        {
            QuestionNode q = (QuestionNode)data[id];
            IDecision t = null;
            IDecision f = null;

            for (int i = 0; i < map.connections.Count; i++)
            {
                if (map.connections[i].outPoint.id == q.truePoint.id)
                {
                    for (int j = 0; j < ready.Count; j++)
                    {
                        if (map.connections[i].inPoint.nodeID == data[ready[j]].id)
                            t = _decisions[j];
                    }
                }
                if (map.connections[i].outPoint.id == q.falsePoint.id)
                {
                    for (int j = 0; j < ready.Count; j++)
                    {
                        if (map.connections[i].inPoint.nodeID == data[ready[j]].id)
                            f = _decisions[j];
                    }
                }
            }

            _decisions.Add(new QuestionDT(q.question, t, f));
        }
    }

    private void SetRoot(List<int> ready, List<Node> data)
    {

        for (int i = 0; i < data.Count; i++)
        {

            bool isRoot = true;

            for (int j = 0; j < map.connections.Count; j++)
            {
                if (map.connections[j].inPoint.nodeID == data[i].id)
                {
                    isRoot = false;
                    break;
                }
            }

            if(isRoot)
            {
                Debug.Log("i " + i + " _decisions " + _decisions.Count + " ready.IndexOf(i) " + ready.IndexOf(i));
                _rootAI = _decisions[ready.IndexOf(i)];
                break;
            }
        }
    }
}
