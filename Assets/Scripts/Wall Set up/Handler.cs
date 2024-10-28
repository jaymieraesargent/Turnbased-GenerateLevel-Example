using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Handler : MonoBehaviour
{
    [SerializeField] GameObject[] walls;

    private void Start()
    {
        Invoke("RemoveComponents",0.25f);
    }

    private void RemoveComponents()
    {
        foreach (GameObject wall in walls) 
        {
            if (wall.gameObject != null)
            {
                Destroy(wall.GetComponent<BoxCollider>());
                Destroy(wall.GetComponent<Rigidbody>());
                Destroy(wall.GetComponent<DestroyOverlapWall>());
                GameObject wall2 = wall.GetComponentInChildren<MeshRenderer>().gameObject;
                wall2.AddComponent<BoxCollider>();
                ;
            }           
        }
    }
}
