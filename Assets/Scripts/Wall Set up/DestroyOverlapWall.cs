using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverlapWall : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Wall")
        {           
            Destroy(this.gameObject);           
        }
    }
}
