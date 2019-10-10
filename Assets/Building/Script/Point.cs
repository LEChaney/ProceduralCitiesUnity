using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Point
{
    // Start is called before the first frame update
    [SerializeField]
    public float x;

    [SerializeField]
    public float z;

    [SerializeField]
    public int mark;

    //0 : default
    //1 : the points on the polygone
    //2 : the points ont the minor road 
    //6 : the intersection between minor and major road
    //8 : duplicate elements

    public Point(float a, float b, int c)
    {
        x = a;
        z = b;
        mark = c;
    }
    public Point()
    {
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
