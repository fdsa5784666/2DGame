using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    private GameObject gameMode;
    
    public void OnClick_Start()
    {
       if(gameMode.activeSelf == false)
        {
            gameMode.SetActive(true); 
        }
       else
        {
            gameMode.SetActive(false);
        }
    }
    
}
