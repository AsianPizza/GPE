using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAnimScript : MonoBehaviour {

    GameObject[] bee;
    float dist = 0;
    Vector3 middle = new Vector3(5f, 2f, 5f);
    Vector3 oink;
    Rigidbody rb;

    private void Start()
    {
        //grab array
        bee = BB.bees.bee;

        // 1st, go to ONE point
        rb = GetComponent<Rigidbody>();
        oink = Vector3.ClampMagnitude(middle - transform.position, 1);
    }

    void FixedUpdate() {

        rb.AddForce(oink * 2f, ForceMode.Acceleration);

        for (int i = 0; i < BB.bees.nBees; i++) {
            //do not check self!
            if (bee[i] != this.gameObject) {
                //distance check:
                dist = Vector3.Distance(transform.position, bee[i].transform.position);
                if (dist < BB.bees.spacing) {
                    Vector3 targetDirection = Vector3.ClampMagnitude(transform.position - bee[i].transform.position, 1);
                    rb.AddForce(targetDirection * 3f, ForceMode.Acceleration);
                }
            }
        }
        //finally, avoid Queen Bee
        dist = Vector3.Distance(transform.position, BB.bees.queen.transform.position);
        if (dist < BB.bees.spacing*2.5f) {
            Vector3 targetDirection = Vector3.ClampMagnitude(transform.position - BB.bees.queen.transform.position, 1);
            rb.AddForce(targetDirection * 3f, ForceMode.Acceleration);
        }
    }

            
}
