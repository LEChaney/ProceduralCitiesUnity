using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonReader : MonoBehaviour
{
    float max_p=float.MinValue;
    float min_p=float.MaxValue;
    float max_d=float.MinValue;
    float min_d=float.MaxValue;

    public int a1 = 0;
    public int a2 = 0;
    public int a3 = 0;
    public int a4 = 0;
    public int a5 = 0;
    public int a6 = 0;
    public int a7 = 0;

    public int b1 = 0;
    public int b2 = 0;
    public int b3 = 0;
    public int b4 = 0;
    public int b5 = 0;
    public int b8 = 0;
    public int b6 = 0;
    public int b7 = 0;
    public int b9 = 0;

    //string Path = "Assets / Building / Data / roadnetwork.json";

    public LandUseData data;

    public TextAsset inputJsonFile;


    public void ReadJson(string path)
    {   
        data = JsonUtility.FromJson<LandUseData>(path);
        Debug.Log("Json" + data.Landuse.Count);
    }
    
    private void Awake()
    {
        ReadJson(inputJsonFile.text);
        //Debug.Log("list :" + data.Landuse[0].Polygon[0]);


    }
    void Start()
    {
        for (int i = 0; i < data.Landuse.Count; i++)
        {
            if (data.Landuse[i].Population > max_p)
                max_p = data.Landuse[i].Population;
            if (data.Landuse[i].Population < min_p)
                min_p = data.Landuse[i].Population;
            if (data.Landuse[i].Population_density > max_d)
                max_d = data.Landuse[i].Population_density;
            if (data.Landuse[i].Population_density < min_d)
                min_d = data.Landuse[i].Population_density;
        }
        Debug.Log("MAXP: " + max_p);
        Debug.Log("MINP: " + min_p);
        Debug.Log("MAXD: " + max_d);
        Debug.Log("MIND: " + min_d);



        for (int i = 0; i < data.Landuse.Count; i++)
        {
            if (data.Landuse[i].Population_density < 0.01 && data.Landuse[i].Land_usage == "commercial")
                a1++;
            else
                if (data.Landuse[i].Population_density < 0.02 && data.Landuse[i].Land_usage == "commercial")
                a2++;
            else
                if (data.Landuse[i].Population_density < 0.03 && data.Landuse[i].Land_usage == "commercial")
                a3++;

            if (data.Landuse[i].Population_density < 0.002 && data.Landuse[i].Land_usage == "commercial")
                a4++;
            else
            if (data.Landuse[i].Population_density < 0.004 && data.Landuse[i].Land_usage == "commercial")
                a5++;
            else
            if (data.Landuse[i].Population_density < 0.006 && data.Landuse[i].Land_usage == "commercial")
                a6++;
            else
            if (data.Landuse[i].Population_density < 0.008 && data.Landuse[i].Land_usage == "commercial")
                a7++;
        }
        for (int i = 0; i < data.Landuse.Count; i++)
        {
            if (data.Landuse[i].Population < 1000 && data.Landuse[i].Land_usage == "residential")
                b1++;
            else
                if (data.Landuse[i].Population < 2000 && data.Landuse[i].Land_usage == "residential")
                b2++;
            else
                if (data.Landuse[i].Population < 3000 && data.Landuse[i].Land_usage == "residential")
                b3++;
            else
            if (data.Landuse[i].Population < 4300 && data.Landuse[i].Land_usage == "residential")
                b4++;

            if (data.Landuse[i].Population < 100 && data.Landuse[i].Land_usage == "residential")
                b5++;
            else
            if (data.Landuse[i].Population < 200 && data.Landuse[i].Land_usage == "residential")
                b6++;
            else
            if (data.Landuse[i].Population < 300 && data.Landuse[i].Land_usage == "residential")
                b7++;
            else
            if (data.Landuse[i].Population < 600 && data.Landuse[i].Land_usage == "residential")
                b8++;
            else
            if (data.Landuse[i].Population < 900 && data.Landuse[i].Land_usage == "residential")
                b9++;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
