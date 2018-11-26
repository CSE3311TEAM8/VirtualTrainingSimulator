using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildSpawn : MonoBehaviour {

    public GameObject child;
    public float[][] array3d = new float[5][];
    public float x = -7.638F;
    public float y = 3.75500F;
    public float z = -2.99F;

    float tempTime;

    // Use this for initialization
    void Start()
    {
        array3d[0] = new float[] { -10.142F, 3.169519F, -4.551F };
        array3d[1] = new float[] { -3.156F, 3.75500F, -6.888F };
        array3d[2] = new float[] { 8.837F, 3.169517F, -0.5613F };
        array3d[3] = new float[] { 10.0362F, -0.03448236F, -3.023F };
        array3d[4] = new float[] { -2.917F, 0.552F, 2.955F };

        //int position = Random.Range(0, 4);
        int position = 0;
            x = array3d[position][0];
            y = array3d[position][1];
            z = array3d[position][2];
        Invoke("CreateMyInstance", 0.0F);
  


    }
    /*
    private void Update()
    {
        tempTime += Time.deltaTime;
        if (tempTime > 10.0F)
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
    */
    void CreateMyInstance()
    {
        Instantiate(child, new Vector3(x,y,z), Quaternion.identity);
        //child.transform.position = new Vector3(0, 0, 0);
    }
    
}


