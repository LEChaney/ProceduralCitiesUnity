using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

[Serializable]
public class SegmentListj
{
    public List<Segmentj> segmentList;
}

[Serializable]
public class Segmentj
{

    //string json = File.ReadAllText(Application.dataPath + "/Scripts/testdata.json", Encoding.UTF8);
    //JObject obj = JObject.Parse(json);

    public float startx;
    public float starty;
    public float endx;
    public float endy;
    public List<int> segmentArrayj;



    //private Vector2 startj=new Vector2(this.startx, Segmentj.starty);
    //private Vector2 endj = new Vector2(endx,endy);
    //public List<int> getList ()
    //{
    //    return new List<int>(segementArrayj);
    //}

    //public void setstart(Vector2 s)
    //{
    //    startj.x = s.x;
    //    startj.y = s.y;
    //}

    //public void setend(Vector2 s)
    //{
    //    endj.x = s.x;
    //    endj.y = s.y;
    //}

    //public void setList(int s)
    //{
    //    List<int> a = new List<int>(segementArrayj);
    //    a.Add(s);
    //    a.ToArray();
    //}

    //public Vector2 getstart()
    //{
    //    return startj;
    //}

    //public Vector2 getend()
    //{
    //    return endj;
    //}

}
