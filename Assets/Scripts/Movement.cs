/*
 * Author: Gonzalo Munoz & John Tieken 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public GameObject child;
    public float[][] array3d = new float[5][];

    float tempTime;
    public float x = -7.638F;
    public float y = 3.75500F;
    public float z = -2.99F;
    // Use this for initialization
    void Start () {
        array3d[0] = new float[] { -9.607F, 3.75500F, -2.99F };
        array3d[1] = new float[] { -3.156F, 3.75500F, -6.888F };
        array3d[2] = new float[] { 8.364F, 3.75500F, -0.5613F };
        array3d[3] = new float[] { 9.572F, 0.552F, -3.023F };
        array3d[4] = new float[] { -2.917F, 0.552F, 2.955F };



    }

    // Update is called once per frame
    void Update () {

        

        tempTime += Time.deltaTime;
        if (tempTime > 40.0F)
        {
            tempTime = 0;

            int position = Random.Range(0, 4);
            x = array3d[position][0];
            y = array3d[position][1];
            z = array3d[position][2];
            child.transform.position = new Vector3(x, y, z);
            Debug.Log(x + " " + y + " " + z);
        }
    }
}
