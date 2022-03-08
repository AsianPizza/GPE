using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool pickedUp;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "plr1" || other.gameObject.name == "plr2")
        {
            other.gameObject.GetComponent<PlayerScript>().poweredUp = true;
            pickedUp = true;
        }
    }
}
