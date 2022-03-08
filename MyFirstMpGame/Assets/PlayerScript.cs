using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float speed = 0.1f;
    float powerUpSpeed = 0.3f;
    public bool poweredUp;
    private float powerUpTimer = 3;
    public float activeSpeed;

    // Update is called once per frame
    void Update()
    {
        if (poweredUp)
        {
            activeSpeed = powerUpSpeed;
        }
        else
            activeSpeed = speed;

        if (poweredUp && powerUpTimer > 0)
        {
            powerUpTimer -= Time.deltaTime;
        }else if (powerUpTimer <= 0)
        {
            poweredUp = false;
            powerUpTimer = 3;
        }
    }
}
