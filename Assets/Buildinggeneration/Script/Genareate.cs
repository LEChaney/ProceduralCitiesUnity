    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genareate : MonoBehaviour
{
    float root2 = 1.42f;

   
    
    public TerrainData terrainData;

    public string property;

    public Vector3[] BoundaryPoints;
    List<Transform> buliding1 =new List<Transform>();
    //public Vector3[] SmallPolygon;
    List <Vector2> SmallPolygon=new List<Vector2>();
    public Transform pointPrefab;
    public Transform[] Building;
    public Transform test;
    List<Vector2> NVector = new List<Vector2>();

    List<Point> Points = new List<Point>();

    List<Point> Polygon = new List<Point>();
    List<Point> Minor = new List<Point>();

    List<Vector2> InsideBuildings;

    [Range(1, 100)]
    public int Num = 1;

    float max_x = float.MinValue;
    float max_z = float.MinValue;
    float min_x = float.MaxValue;
    float min_z = float.MaxValue;

    private Vector3 OnMap(Vector3 pos)
    {
        pos.x = pos.x + terrainData.bounds.extents.x;
        pos.z = pos.z + terrainData.bounds.extents.z;

        return pos;
    }

   

    private Vector2 PointToVector2(Point pos)
    {
        Vector2 a=new Vector2();
        a.x = pos.x;
        a.y = pos.z;
        return a;
    }

    Vector2 ReverseVector2(Vector2 a)
    {
        Vector2 b = new Vector2();
        b.x = -a.x;
        b.y = -a.y;
        return b;
        
    }

    private void Divide(List<Point> p)
    {        
        for (int i=0; i<p.Count; i++)
        {
          int flag = 0;
          for(int j=0;j<p.Count;j++)
            {
                if((p[i].x==p[j].x)&&(p[i].z==p[j].z))
                {
                    flag++;
                }
                
            }
            if (flag > 1)
            {
                p[i].mark = 2;
            } 
        }
        for (int i = 1; i < p.Count-1; i++)
        {
            if ((p[i].mark == 0) && (p[i-1].mark+p[i+1].mark == 4))
            {
                p[i].mark = 2;
            }
        }

       // the first point
       if (p[0].mark == 0)
        {
            if ((p[1].mark + p[p.Count-1].mark) == 4)
            {
                p[0].mark = 2;
            }
            else
                p[0].mark = 1;
        }
        else
         if(p[0].mark != 0)
        {
            if((p[1].mark+p[p.Count-1].mark) == 2)
            {
                p[0].mark = 6;
            }
        }

       // points but the first point and last point
       for(int i=1;i<p.Count-1;i++)
        {
            int sum = p[i - 1].mark + p[i + 1].mark;

            Debug.Log("first" + p[57].mark);
                if(sum==1||sum==6)
            {
                p[i].mark = 1;
                //find the points on the polygon
            }
            else
                if(sum==3&&p[i].mark==0)
            {
                p[i].mark = 1;
                i++;
                p[i].mark = 6;
                if(p[55].mark==6)
                {
                 //   Debug.Log("error1");
                }
                //find the points on the 
            }
            else
                if (sum == 3 && p[i].mark == 2)
            {
                p[i].mark = 6;
                if (p[55].mark == 6)
                {
                  //  Debug.Log("error2");
                }
            }
            else
                if (sum == 2)
            {
                p[i].mark = 6;
                //if (p[55].mark == 6)
                //{
                //    Debug.Log("error3");
                //}
            }
          //  Debug.Log(i+" time " + p[56].mark);
        }

        //
        //the last point
        //
        if (p[p.Count-1].mark == 0)
        {
            if ((p[0].mark + p[p.Count - 2].mark) == 4)
            {
                p[p.Count - 1].mark = 2;
            }
            else
                p[p.Count - 1].mark = 1;
        }
        else
          if (p[p.Count - 1].mark != 0)
        {
            if ((p[0].mark + p[p.Count - 2].mark) == 2)
            {
                p[0].mark = 6;
            }
        }
      //  Debug.Log("last" + p[55]);
    }

    private void ReadPolygon(List<Point> p)
    {
        for(int i=0;i<p.Count;i++)
        {
           if(p[i].mark==2)
            {
                Minor.Add(p[i]);
            }
           else
            {
                Polygon.Add(p[i]);
            }
        }
    }

    private void CleanDuplicate(List<Point> p)
    {
        for(int i=0;i<p.Count-1;i++)
        {
            if (p[i].mark == 8)
                continue;
            for (int j = i+1; j < p.Count; j++)
            {
                if ((p[i].x==p[j].x)&& (p[i].z == p[j].z))
                {
                    p[j].mark = 8;

                }
            }
        }
        for (int i = 0; i < p.Count; i++)
        {
            if (p[i].mark == 8)
            {
                p.RemoveAll(Mark_8);
            }
        }
    }

    private bool Mark_8(Point obj)
    {
        if (obj.mark == 8)
            return true;
        else
            return false;
    }

    private void Import(Vector3[] p)
    {

        if (p != null)
        {
            BoundaryPoints = p;
            Debug.Log("<color=green> Import Data Success </color>");
        }
        else
            Debug.Log("<color=red> Import Data Fail </color>");
    }


    private void Import_2(Vector3[] p)
    {
        if (p != null)
        {
            for (int i = 0; i < p.Length; i++)
            {
                Points.Add(new Point(p[i].x, p[i].z, 0));   
            }          
            Debug.Log("<color=green> Import Data Success </color>");
        }
        else
            Debug.Log("<color=red> Import Data Fail </color>");
    }

    private void shrink(List<Point> p, float distance)
    {
        //First point;

        NVector.Add((PointToVector2(p[0]) - PointToVector2(p[p.Count - 1])).normalized);

        for (int i = 1; i < p.Count ; i++)
        {
            NVector.Add((PointToVector2(p[i]) - PointToVector2(p[i - 1])).normalized);
        }
        Vector2 res=new Vector2();
        float distance1;
        for(int i = 0; i < p.Count-1; i++)
        {
            if (Vector2.Dot(NVector[i], ReverseVector2(NVector[i + 1])) == 1)
            {
                Debug.Log("error1");
                continue;
            }
                //Debug.Log("error1");
               if(Inside_test(p[i], Polygon, i))
                {
                Debug.Log("x " + p[i].x + " z " + p[i].z + " jieguo " + Inside_test(p[i], Polygon, i));
                    distance1 = distance / Mathf.Sin(0.5f*Vector2.Angle(NVector[i], ReverseVector2(NVector[i + 1])));
                    res=(ReverseVector2(NVector[i])+NVector[i+1])*distance1;
                    SmallPolygon.Add(PointToVector2(p[i]) + res);
                }
               else
                {
                Debug.Log("x " + p[i].x + " z " + p[i].z + " jieguo " + Inside_test(p[i], Polygon, i));
                distance1 = distance / Mathf.Sin(0.5f * Vector2.Angle(NVector[i], ReverseVector2(NVector[i + 1])));
                    res = (NVector[i] + ReverseVector2(NVector[i + 1])) * distance1;
                    SmallPolygon.Add(PointToVector2(p[i]) + res);
                }
                    
            
        }

        if (Vector2.Dot(NVector[p.Count-1], ReverseVector2(NVector[0])) != 0)
        {
            if((Inside_test(p[p.Count-1], Polygon, p.Count-1)))
            {
                distance1 = distance / Mathf.Sin(0.5f * Vector2.Angle(NVector[p.Count-1], ReverseVector2(NVector[0])));
                res = (ReverseVector2(NVector[p.Count-1]) + NVector[0]) * distance1;
                SmallPolygon.Add(PointToVector2(p[p.Count-1]) + res);
            }
            else
            {
                distance1 = distance / Mathf.Sin(0.5f * Vector2.Angle(NVector[p.Count-1], ReverseVector2(NVector[0])));
                res = (ReverseVector2(NVector[0]) + NVector[p.Count-1]) * distance1;
                SmallPolygon.Add(PointToVector2(p[p.Count - 1]) + res);
            }
        }

    }



    void BuildingGeneration(float max_x, float max_z,float min_x, float min_z,
        int num, List<Point> p, string pro)
    {
        int app = 0;
      
        for (int index = 0; index < num; )
        {
            app++;
            if (app > 10000)
            {
                Debug.Log("Too much ");
                return;
            }
            float x = UnityEngine.Random.Range(min_x, max_x );
            float z = UnityEngine.Random.Range(min_z, max_z );

            Vector3 res;
            res.x = x;
            res.z = z;
            res.y = terrainData.GetInterpolatedHeight(((res.x+ terrainData.bounds.extents.x) /terrainData.size.x),((res.z + terrainData.bounds.extents.z)/ terrainData.size.z))
                + Building[0].localScale.y / 2 - 0.2f;
            



            Point res1 = new Point(res.x, res.z, 0);

            
            if (Inside_test(res1, p)
                && Distance_test(res1, buliding1)&& Minor_test(res1, Minor) && Slope_test(res1,Building[0]))
            {

              //  Debug.Log("error2");
                Transform Buildings = Instantiate(Building[0]);
                //  Transform Buildings = Instantiate(test);
                    Buildings.SetParent(transform, false);
                    Buildings.localPosition = OnMap(res);
                    buliding1.Add(Buildings);
                    index++;
            }
        }

        

        //return res;
    }

    private bool Slope_test(Point p,Transform t)
    {
        Debug.Log("error111");

        if (terrainData.GetSteepness((p.x + terrainData.bounds.extents.x) / terrainData.size.x, (p.z + terrainData.bounds.extents.z) / terrainData.size.z) > 30f)
        {
            Debug.Log("diyi");
            return false;
        }
    

        Debug.Log("jiaodu  " + terrainData.GetSteepness((p.x+ terrainData.bounds.extents.x) / terrainData.size.x, (p.z+ terrainData.bounds.extents.z) / terrainData.size.z));
        Debug.Log("gaoshix  " + (p.x + terrainData.bounds.extents.x) / terrainData.size.x);
      
        Debug.Log("feliang2" + terrainData.bounds.extents.x);
        


        Debug.Log("gaoshiz  " + p.z + terrainData.bounds.extents.z);
       
        Debug.Log("diLiu");
        return true;
    }

    private bool Distance_test(Point p, List<Transform> b)
    {

       // Debug.Log("building"+ b.Count);

        if (b.Count==0)
        {

         //   Debug.Log("error1");
            return true;
        }
        else
        {
            for (int i = 0; i < b.Count; i++)
            {
                if ((Mathf.Abs(b[i].localPosition.z - (p.z+ terrainData.bounds.extents.z)) < 2f) && (Mathf.Abs(b[i].localPosition.x - (p.x+ terrainData.bounds.extents.x))) < 2f)
                {
                    Debug.Log("The locaposition is :"+ b[i].localPosition.x+" and "+b[i].localPosition.z);
                    return false;
                }
                Debug.Log("The locaposition is :" + b[i].localPosition.x + " and " + b[i].localPosition.z);
                Debug.Log("The position is :" + (p.z + terrainData.bounds.extents.z) + " and " + (p.z + terrainData.bounds.extents.z));
            }
             
        }
        return true;
    }

    private bool Minor_test(Point p, List<Point> b)
    {

        // Debug.Log("building"+ b.Count);

        if (b.Count == 0)
        {

            //   Debug.Log("error1");
            return true;
        }
        else
        {
            for (int i = 0; i < b.Count; i++)
            {
                if ((Mathf.Abs(b[i].z - (p.z + terrainData.bounds.extents.z)) < 1.5f) && (Mathf.Abs(b[i].x - (p.x + terrainData.bounds.extents.x))) < 1.5f)
                {
                    Debug.Log("The locaposition is :" + b[i].x + " and " + b[i].z);
                    return false;
                }
                Debug.Log("The locaposition is :" + b[i].x + " and " + b[i].z);
                Debug.Log("The position is :" + (p.z + terrainData.bounds.extents.z) + " and " + (p.z + terrainData.bounds.extents.z));
            }

        }
        return true;
    }

    private void LoadBoundrary(Vector3[] points)
    {
        if (points.Length == 0)
        {
            Debug.Log("<color=red> Boundary Load Fail </color>");
        }
        else
        {
            for (int i = 0; i < points.Length; i++)
            {
                Transform point = Instantiate(pointPrefab);
                point.SetParent(transform, false);
                point.localPosition = OnMap(points[i]);
            }
            Debug.Log("<color=green> Boundary Load Success </color>");
        }
    }

    private void LoadSmallPolygon(List<Vector2> v)
    {
        if (v.Count == 0)
        {
            Debug.Log("<color=red> Small polygon Load Fail </color>");
        }
        else
        {
            for (int i = 0; i < v.Count; i++)
            {
                Transform point = Instantiate(test);
                point.SetParent(transform, false);
                Vector3 para = new Vector3();
                para.x = v[i].x;
                para.y = 5f;
                para.z = v[i].y;
                point.localPosition = OnMap(para);
                
            }
            Debug.Log("<color=green> Small Polygon Load Success </color>");
        }
    }


    //
    //
    //Inside test for point
    //
    //
    bool Inside_test(Point a, List<Point> p)
    {
        bool res=false;

        int i;
        int j = p.Count - 1;
        int app = 0;
        float res1;
        float res2;
        for (i = 0; i < p.Count;i++)
        {
            app++;
            if(app>10000)
            {
                break;
            }
            if(p[i].z<a.z&&p[j].z>=a.z||p[j].z<a.z&&p[i].z>=a.z)
            {
                res1 = (a.z - p[i].z) / (p[j].z - p[i].z);
                res2 = res1 * p[j].x - p[i].x;
                if(res2<a.x)
                {
                    res = !res;
                }
            }
            j = i;
        }
        return res;
    }

    bool Inside_test(Point a, List<Point> p, int index)
    {
        bool res = false;

        int i;
        int j = p.Count - 1;
        float res1;
        float res2;
        for (i = 0; i < p.Count; i++)
        {
            if(i==index)
            {
                if (i == p.Count - 1)
                    break;
                else
                    i++;
            }
            if (p[i].z < a.z && p[j].z >= a.z || p[j].z < a.z && p[i].z >= a.z)
            {
                res1 = (a.z - p[i].z) / (p[j].z - p[i].z);
                res2 = res1 * p[j].x - p[i].x;
                if (res2 < a.x)
                {
                    res = !res;
                }
            }
            j = i;
        }


        return res;
    }
    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < Num; i++)
        {
            yield return new WaitForSeconds(0.1f);
            BuildingGeneration(max_x, max_z, min_x, min_z, 1, Polygon,property);
        }
    }

    void BuildingPlacement()
    {
        LoadBoundrary(BoundaryPoints);

        Import_2(this.GetComponent<Boundary>().Poss);

        Divide(Points);

        ReadPolygon(Points);

        CleanDuplicate(Minor);

        CleanDuplicate(Polygon);
       
        shrink(Polygon, 1.2f);
        Debug.Log(" //////////////////////loadtest////////////////////////");
        //LoadSmallPolygon(SmallPolygon);

        for (int i = 0; i < SmallPolygon.Count; i++)
        {
            if (SmallPolygon[i].x > max_x)
            {
                max_x = SmallPolygon[i].x;
            }

            if (SmallPolygon[i].y > max_z)
            {
                max_z = SmallPolygon[i].y;
            }

            if (SmallPolygon[i].x < min_x)
            {
                min_x = SmallPolygon[i].x;
            }

            if (SmallPolygon[i].y < min_z)
            {
                min_z = SmallPolygon[i].y;
            }
        }
        max_x = 4;
        min_x = -4;
        max_z = 9;
        min_z = -9;
        ////StartCoroutine(CreateChildren());
        BuildingGeneration(max_x, max_z, min_x, min_z, Num, Polygon, property);
    }

    private void Awake()
    {
        Import(this.GetComponent<Boundary>().Poss);

       
    }

    void Start()
    {
      BuildingPlacement();
    }

   


    // Update is called once per frame
    void Update()
    {
         //BuildingGeneration(max_x, max_z, min_x, min_z, Num, Polygon);
        
    }
    //limitation？？？
    private void FixedUpdate()
    {
        
    }
}
