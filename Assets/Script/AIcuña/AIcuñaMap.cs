using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AIcuñaMap", menuName = "AIcuña Map", order = 51)]
public class AIcuñaMap : ScriptableObject
{
    public List<ActionNode> actions;
    public List<QuestionNode> questions;
    public List<Connection> connections;
}
