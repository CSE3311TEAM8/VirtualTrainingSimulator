using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    void spin()
    {
        transform.Rotate(0, .2F, 0 * Time.deltaTime);
 }

    void Update()
    {
        spin();
    }
}
