using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


public class ReadJson : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {


        string jsonTest = File.ReadAllText(Application.dataPath + "/Scripts/testdata.json", Encoding.UTF8);
        SegmentListj obj = JsonUtility.FromJson<SegmentListj>(jsonTest);
        //Debug.Log(obj.startx);
        //Debug.Log(obj.endy);
        //foreach (var inter in obj.segementArrayj)
        //{
        //    Debug.Log(inter);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}

