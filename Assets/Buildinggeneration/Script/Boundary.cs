using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    
   // public Transform pointPrefab;
    Vector3 pos;
    public Vector3[] Poss = new Vector3[64];
    void Awake()
    {
        //left
        for (int i = 0; i < 21; i++)
        {
          
            pos.x = -5f;
            pos.y = 72f;
            pos.z = -10f + i;

            Poss[i] = pos;  
        }
        //top
        for (int i = 0; i < 9; i++)
        {
            pos.x = -4f + i;
            pos.y = 72f;
            pos.z = 10f;

            Poss[21 + i] = pos;
        }
        //right
        for (int i = 0; i < 21; i++)
        {
            pos.y = 72f;
            pos.z =10f - i;
            pos.x = Mathf.Sin(pos.z)+5;
           // Debug.Log("NO "+pos.z+" " +Mathf.Sin(pos.z));

            Poss[29 + i] = pos;
        }

        //bottom
        for (int i = 0; i < 5; i++)
        {
            pos.x = 4f - i;
            pos.y = 72f;
            pos.z = -10f;

            Poss[49 + i] = pos;
        }

        Poss[54].x = 0f;
        Poss[54].y = 72f;
        Poss[54].z = -9f;

        Poss[55].x = 0f;
        Poss[55].y = 72f;
        Poss[55].z = -8f;

        Poss[56].x = 0f;
        Poss[56].y = 72f;
        Poss[56].z = -7f;

        Poss[57].x = 0f;
        Poss[57].y = 72f;
        Poss[57].z = -8f;

        Poss[58].x = 0f;
        Poss[58].y = 72f;
        Poss[58].z = -9f;


        Poss[59].x = 0f;
        Poss[59].y = 72f;
        Poss[59].z = -10f;

        for (int i = 0; i < 5; i++)
        {
            pos.x = 0f - i;
            pos.y = 72f;
            pos.z = -10f;

            Poss[59 + i] = pos;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
   // Update is called once per frame
    void Update()
    {
        
    }
}
