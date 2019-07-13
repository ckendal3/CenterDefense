using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamWrapper : MonoBehaviour
{
    public Team team = new Team(TeamOrientation.Enemy);
}


public struct Team
{
    public Team(TeamOrientation value)
    {
        orientation = value;
    }

    public TeamOrientation orientation;
}

public enum TeamOrientation
{
    Enemy,
    Ally,
    Neutral
}