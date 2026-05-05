using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EGameState GameState;




}
public enum EGameState
{
    Playing,
    Paused,
    GameOver,
    Victory
}

public enum GameMode
{
    Standard,
    Endless
}