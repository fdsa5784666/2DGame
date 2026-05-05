using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    private Transform player;
    private float verticalSpeed;
    private float horizontalSpeed;
    private bool isMove = false;
    private void Start()
    {
        player = transform;
    }
    void Update()
    {
        //verticalSpeed = Input.GetAxis("Vertical");
        //horizontalSpeed = Input.GetAxis("Horizontal");

        verticalSpeed = Input.GetAxisRaw("Vertical");
        horizontalSpeed = Input.GetAxisRaw("Horizontal");

        if (verticalSpeed != 0 || horizontalSpeed != 0)
        {
            isMove = true;
        }
        else
        {
            isMove= false;
        }
    }
    private void LateUpdate()
    {
        if (isMove)
        {

            player.transform.position += new Vector3(horizontalSpeed * Time.deltaTime * GameData.Instance.playerSpeed,
                                                     verticalSpeed * Time.deltaTime * GameData.Instance.playerSpeed, 0);
        }
    }
}
