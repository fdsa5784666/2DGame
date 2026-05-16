using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance {  get; private set; }

    public Transform VC;
    public Transform playerTransform;
    private void Awake()
    {
        Instance = this;
    }
    public void ResetPlayerTransform()
    {
        playerTransform.position = Vector3.zero;
        playerTransform.rotation = Quaternion.identity;
        VC.position = Vector3.zero;   
    }
}
