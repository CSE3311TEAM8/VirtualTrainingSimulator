using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{

    public GameObject smoke;
    void Start()
    {

        float x = Random.Range(0, 30);
        Invoke("CreateMyInstance", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance1", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance2", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance3", x);
        Invoke("CreateMyInstance3", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance4", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance5", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance6", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance7", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance8", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance9", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance10", x);
        Invoke("CreateMyInstance10", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance11",x);
        x = Random.Range(0, 30);
        Invoke("CreateMyInstance12", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance13", x);
        x = Random.Range(0, 130);
        Invoke("CreateMyInstance14", x);
        Invoke("CreateMyInstance14", x);

    }

    // Will be called 3 seconds after level start
    void CreateMyInstance()
    {
        Instantiate(smoke, new Vector3(-7.59F, -0.274483F, 4.16F), Quaternion.identity);

    }

    void CreateMyInstance1()
    {
        Instantiate(smoke, new Vector3(-7.59F, -0.274483F, -4.96F), Quaternion.identity);
    }

    void CreateMyInstance2()
    {
        Instantiate(smoke, new Vector3(-0.71F, -0.27F, -4.96F), Quaternion.identity);
    }

    void CreateMyInstance3()
    {
        Instantiate(smoke, new Vector3(-0.71F, -0.27F, 4.05F), Quaternion.identity);
    }

    void CreateMyInstance4()
    {
        Instantiate(smoke, new Vector3(5.23F, -0.27F, 4.05F), Quaternion.identity);
    }

    void CreateMyInstance5()
    {
        Instantiate(smoke, new Vector3(5.23F, -0.27F, -4.79F), Quaternion.identity);
    }

    void CreateMyInstance6()
    {
        Instantiate(smoke, new Vector3(5.23F, -0.27F, -4.79F), Quaternion.Euler(0.0F,-90.0F,90.0F));
    }

    void CreateMyInstance7()
    {
        Instantiate(smoke, new Vector3(5.23F, -0.27F, 4.05F), Quaternion.Euler(0.0F, -90.0F, 90.0F));
    }

    void CreateMyInstance8()
    {
        Instantiate(smoke, new Vector3(-7.59F, -0.274483F, -4.96F), Quaternion.Euler(0.0F, -90.0F, 90.0F));
    }

    void CreateMyInstance9()
    {
        Instantiate(smoke, new Vector3(-0.71F, -0.27F, -4.96F), Quaternion.Euler(0.0F, -90.0F, 90.0F));
    }

    void CreateMyInstance10()
    {
        Instantiate(smoke, new Vector3(20F, 4.38F, -4.79F), Quaternion.Euler(0.0F, -90.0F, 90.0F));
    }

    void CreateMyInstance11()
    {
        Instantiate(smoke, new Vector3(20F, 4.38F, 4.05F), Quaternion.Euler(0.0F, -90.0F, 90.0F));
    }

    void CreateMyInstance12()
    {
        Instantiate(smoke, new Vector3(-7.59F, 4.38F, -4.96F), Quaternion.Euler(0.0F, -90.0F, 90.0F));
    }

    void CreateMyInstance13()
    {
        Instantiate(smoke, new Vector3(-0.71F, 4.38F, -4.96F), Quaternion.Euler(0.0F, -90.0F, 90.0F));
    }
}


