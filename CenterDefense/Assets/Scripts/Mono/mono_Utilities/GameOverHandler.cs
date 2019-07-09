using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Core.States;

public class GameOverHandler : MonoBehaviour
{
    private void OnDestroy()
    {
        GameEvents.onGameStatusChange(GameStateStatusV2.GameOver);
    }
}
