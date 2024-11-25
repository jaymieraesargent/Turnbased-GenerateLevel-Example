using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnCollider : MonoBehaviour
{
    public Collider myCollider;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ColliderOn",0.25f);
    }

    // Update is called once per frame
    void ColliderOn()
    {
        myCollider.enabled = true;
    }
}
