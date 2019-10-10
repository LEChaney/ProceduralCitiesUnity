using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genareate : MonoBehaviour
{
    
    float root2 = 1.42f;

    public float RoadWigth = 0.5f;
    
    public TerrainData terrainData;

    public TextAsset inputJsonFile;

    public string property;

    //public Vector3[] BoundaryPoints;

    public LandUseData LandData;

    List<Transform> buliding1 =new List<Transform>();
    
    List <Point> SmallPolygon=new List<Point>();

    public Transform pointPrefab;

    public Transform[] Building;

    public Transform test;
   // List<Vector2> NVector = new List<Vector2>();

    List<Point> Points = new List<Point>();

    List<Point> Polygon = new List<Point>();

    List<Point> Minor = new List<Point>();

    //List<Vector2> InsideBuildings;

   
    public float Population;

    public float Density;

    float max_x = float.MinValue;
    float max_z = float.MinValue;
    float min_x = float.MaxValue;
    float min_z = float.MaxValue;

    private Vector3 OnMap(Vector3 pos)
    {
        Vector3 vector3 = new Vector3();

        vector3.x = pos.x + terrainData.bounds.extents.x;
        vector3.z = pos.z + terrainData.bounds.extents.z;

        return vector3;
    }

   

    //private Vector2 PointToVector2(Point pos)
    //{
    //    Vector2 a=new Vector2();
    //    a.x = pos.x;
    //    a.y = pos.z;
    //    return a;
    //}

    //Vector2 ReverseVector2(Vector2 a)
    //{
    //    Vector2 b = new Vector2();
    //    b.x = -a.x;
    //    b.y = -a.y;
    //    return b;
        
    //}

    private void ShowPointPolygon(List<Point> p)
    {
        Vector3 vector3 = new Vector3();

        vector3.y = 72f;

        for (int i = 0; i < p.Count; i++)
        {
            Transform point = Instantiate(test);
            point.SetParent(transform, false);

            vector3.x = p[i].x + terrainData.bounds.extents.x;
            vector3.z = p[i].z + terrainData.bounds.extents.z;
        
            point.localPosition = vector3;
        }
    }

    private void ShowPoint(Point p)
    {
        Vector3 vector3 = new Vector3();

        vector3.y = 72f;

        
            Transform point = Instantiate(test);
            point.SetParent(transform, false);

            vector3.x = p.x + terrainData.bounds.extents.x;
            vector3.z = p.z + terrainData.bounds.extents.z;

            point.localPosition = vector3;
        
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

           // Debug.Log("first" + p[57].mark);
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
            
            
            }
            else
                if (sum == 3 && p[i].mark == 2)
            {
                p[i].mark = 6;
            
            }
            else
                if (sum == 2)
            {
                p[i].mark = 6;  
            }
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

    private void CleanUnnecessaryPoints(List<Point> p)
    {
        float slope1;
        float slope2;

        //first point
        if(p[0].z == p[1].z && p[0].z == p[p.Count - 1].z)
        {
            p[0].mark = 8;
        }
        else
        if(p[0].z == p[1].z || p[0].z == p[p.Count - 1].z)
        {
            // make sure divisor does not equal to 0
        }
        else    
        {
            slope1 = (p[0].x - p[p.Count - 1].x) / (p[0].z - p[p.Count - 1].z);
            slope2 = (p[1].x - p[0].x) / (p[1].z - p[0].z);
                if(slope1==slope2)
            {
                p[0].mark = 8;
            }
        }

        //all points but the first and last
        for (int i = 1; i < p.Count - 1; i++)
        {
            if (p[i].z == p[i-1].z && p[i].z == p[i+1].z)
            {
                p[i].mark = 8;
               // Debug.Log("error 3: " + p[i].x + " " + p[i].z);
            }
            else
            if (p[i].z == p[i-1].z || p[i].z == p[i+1].z)
            {
                // make sure divisor does not equal to 0
                //Debug.Log("error 2: " + p[i].x + " " + p[i].z);
            }
            else
            {
                slope1 = (p[i].x - p[i - 1].x) / (p[i].z - p[i - 1].z);
                slope2 = (p[i+1].x - p[i].x) / (p[i+1].z - p[i].z);

                
                if (slope1 == slope2)
                {
                    //Debug.Log("slope1: " + slope1 + " slope2 :" + slope2);
                  //  Debug.Log("error 1: " + p[i].x + " " + p[i].z);
                    p[i].mark = 8;
                }
            }
        }

        //Last point
        if (p[p.Count-1].z == p[0].z && p[p.Count-2].z == p[p.Count - 1].z)
        {
            p[p.Count - 1].mark = 8;
        }
        else
        if (p[p.Count - 1].z == p[0].z || p[p.Count - 2].z == p[p.Count - 1].z)
        {
            // make sure divisor does not equal to 0
        }
        else
        {
            slope1 = (p[p.Count-1].x - p[p.Count - 2].x) / (p[p.Count-1].z - p[p.Count - 2].z);
            slope2 = (p[0].x - p[p.Count-1].x) / (p[0].z - p[p.Count-1].z);
            if (slope1 == slope2)
            {
                p[p.Count - 1].mark = 8;
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

    private void Import(string path)
    {
            LandData = JsonUtility.FromJson<LandUseData>(path);
           
            Debug.Log("<color=green> Import Data Success </color>");
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

    void Shrink(List<Point> p, string property)
    {
        //First point;

        float Distance;
        float Scale=0;

        switch (property)
        { 
        case "industry":
            {
                Distance = Building[1].localScale.x / 2 * root2+RoadWigth*root2;
               
            }
                break;


            case "commercial":

            {
                Distance = Building[2].localScale.x / 2 * root2 + RoadWigth * root2;
                
            }
                break;

            case "residential":
            {
                Distance = Building[0].localScale.x / 2 * root2 + RoadWigth * root2;
                
            }
                break;

            default:
            {
                Debug.Log("No property, No shrink");
                
            }
                return;

        }

        float sumx=0;
        float sumz=0;

        for (int i=0;i<p.Count;i++)
        {
            sumx += p[i].x;
            sumz += p[i].z;
        }



        Point middlepoint = new Point();
        middlepoint.x = sumx / p.Count;
        middlepoint.z = sumz / p.Count;

        //ShowPoint(middlepoint);

        for (int i=0;i<p.Count;i++)
        {
            float length=Mathf.Sqrt((p[i].x-middlepoint.x)* (p[i].x - middlepoint.x)+(p[i].z - middlepoint.z)* (p[i].z - middlepoint.z));
            if (length <= Distance)
            {
                Debug.Log("not enough");
                return;
            }

            float NewScale = Distance / length;

            if (NewScale>Scale)
            {
                Scale = Distance / length;
            }

            Scale = Distance / length;
            float x = middlepoint.x + (p[i].x - middlepoint.x) * (1-Scale);
            float z = middlepoint.z + (p[i].z - middlepoint.z) * (1-Scale);
            Point point = new Point(x, z, 0);
            SmallPolygon.Add(point);
            
        }
        //NVector.Add((PointToVector2(p[0]) - PointToVector2(p[p.Count - 1])).normalized);

        //for (int i = 1; i < p.Count ; i++)
        //{
        //    NVector.Add((PointToVector2(p[i]) - PointToVector2(p[i - 1])).normalized);
        //}
        //Vector2 res=new Vector2();
        //float distance1;
        //for(int i = 0; i < p.Count-1; i++)
        //{
        //    if (Vector2.Dot(NVector[i], ReverseVector2(NVector[i + 1])) == 1)
        //    {
        //        Debug.Log("error1");
        //        continue;
        //    }
        //        //Debug.Log("error1");
        //       if(Inside_test(p[i], Polygon, i))
        //        {
        //        Debug.Log("x " + p[i].x + " z " + p[i].z + " jieguo " + Inside_test(p[i], Polygon, i));
        //            distance1 = distance / Mathf.Sin(0.5f*Vector2.Angle(NVector[i], ReverseVector2(NVector[i + 1])));
        //            res=(ReverseVector2(NVector[i])+NVector[i+1])*distance1;
        //            SmallPolygon.Add(PointToVector2(p[i]) + res);
        //        }
        //       else
        //        {
        //        Debug.Log("x " + p[i].x + " z " + p[i].z + " jieguo " + Inside_test(p[i], Polygon, i));
        //        distance1 = distance / Mathf.Sin(0.5f * Vector2.Angle(NVector[i], ReverseVector2(NVector[i + 1])));
        //            res = (NVector[i] + ReverseVector2(NVector[i + 1])) * distance1;
        //            SmallPolygon.Add(PointToVector2(p[i]) + res);
        //        }


        //}

        //if (Vector2.Dot(NVector[p.Count-1], ReverseVector2(NVector[0])) != 0)
        //{
        //    if((Inside_test(p[p.Count-1], Polygon, p.Count-1)))
        //    {
        //        distance1 = distance / Mathf.Sin(0.5f * Vector2.Angle(NVector[p.Count-1], ReverseVector2(NVector[0])));
        //        res = (ReverseVector2(NVector[p.Count-1]) + NVector[0]) * distance1;
        //        SmallPolygon.Add(PointToVector2(p[p.Count-1]) + res);
        //    }
        //    else
        //    {
        //        distance1 = distance / Mathf.Sin(0.5f * Vector2.Angle(NVector[p.Count-1], ReverseVector2(NVector[0])));
        //        res = (ReverseVector2(NVector[0]) + NVector[p.Count-1]) * distance1;
        //        SmallPolygon.Add(PointToVector2(p[p.Count - 1]) + res);
        //    }
        //}

    }

    Vector3 GenerateRandomPosition(float max_x, float max_z, float min_x, float min_z)
    {
        float x = UnityEngine.Random.Range(min_x, max_x);
        float z = UnityEngine.Random.Range(min_z, max_z);

        Vector3 res;
        res.x = x;
        res.y = 0f;
        res.z = z;
        return res;
        //res.y = terrainData.GetInterpolatedHeight(((res.x + terrainData.bounds.extents.x) / terrainData.size.x), ((res.z + terrainData.bounds.extents.z) / terrainData.size.z))
        //    + Building[0].localScale.y / 2 - 0.2f;

    }

    void BuildingGeneration(float max_x, float max_z,float min_x, float min_z, List<Point> p, string pro, float density, float population)
    {
        int app = 0;
        bool flag =false;
        int seed;
        int num=0;
        if (population<100)
         num= (int)(population/30)+1;
        else 
            if (population < 1000)
            num = (int)(population /250) + 1;
        else
            if (population < 2000)
            num = (int)(population / 400) + 1;
        else
            num = (int)(population / 700) + 1;
        Vector3 pos = new Vector3();
        Point pos1 = new Point();
        
           // Point res1 = new Point(res.x, res.z, 0);
            
                switch (pro)
                {
                case "industry":
                    {
                  //  Debug.Log("yse industry");
                        for (int index = 0; index < num;)
                      {
                        
                        while (!flag)
                        {
                            app++;
                            if (app > 1000)
                            {
                                Debug.Log("Too much buildings");
                                return;
                            }
                            pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                            pos.y = terrainData.GetInterpolatedHeight(((pos.x + terrainData.bounds.extents.x) / terrainData.size.x), ((pos.z + terrainData.bounds.extents.z) / terrainData.size.z))
                                   + Building[1].localScale.y / 2 - 0.2f;

                            pos1.x = pos.x;
                            pos1.z = pos.z;
                            if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[1]) && Minor_test(pos1, Minor, Building[1]) && Slope_test(pos1))
                            {
                                flag = true;
                                Transform Buildings = Instantiate(Building[1]);
                                Buildings.SetParent(transform, false);
                                Buildings.localPosition = OnMap(pos);
                                buliding1.Add(Buildings);
                                index++;
                            }
                        }
                        
                        flag = false;
                      }
                    
                }
                break;

            case "commercial":
                {
                    for (int index = 0; index < num;)
                    {
                        
                        while (!flag)
                        {
                            app++;
                            if (app > 1000)
                            {
                                Debug.Log("Too much buildings");
                                return;
                            }

                            pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                            pos.y = terrainData.GetInterpolatedHeight(((pos.x + terrainData.bounds.extents.x) / terrainData.size.x), ((pos.z + terrainData.bounds.extents.z) / terrainData.size.z))
                                   + Building[1].localScale.y / 2 - 0.2f;

                            pos1.x = pos.x;
                            pos1.z = pos.z;

                            seed = UnityEngine.Random.Range(0, 99);
                        
                             if (seed < (19 + (density - 0.003) / 0.032) * 500)
                            {
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[2]) && Minor_test(pos1, Minor, Building[2]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[2]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = OnMap(pos);
                                    buliding1.Add(Buildings);
                                    index++;
                                }
                            }
                            else
                            {
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[3]) && Minor_test(pos1, Minor, Building[3]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[3]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = OnMap(pos);
                                    buliding1.Add(Buildings);
                                    index++;
                                }
                            }
                        }
                        
                        flag = false;
                    }
                   
                }
                break;
            case "residential":
                {
                  
                    for (int index = 0; index < num;)
                    {        
                        while (!flag)
                        {
                            app++;
                            if (app > 1000)
                            {
                                Debug.Log("Too much buildings");
                                return;
                            }

                            pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                            pos.y = terrainData.GetInterpolatedHeight(((pos.x + terrainData.bounds.extents.x) / terrainData.size.x), ((pos.z + terrainData.bounds.extents.z) / terrainData.size.z))
                                   + Building[1].localScale.y / 2 - 0.2f;

                            pos1.x = pos.x;
                            pos1.z = pos.z;

                            seed = UnityEngine.Random.Range(0, 99);
                           
                            if (seed < (11+(density-0.003)/0.032*300))
                                {
                              
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[3]) && Minor_test(pos1, Minor, Building[3]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[3]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = OnMap(pos);
                                    buliding1.Add(Buildings);
                                }
                            }
                            else
                            
                             if (seed < (79 - ((density - 0.003) / 0.032) *300))
                            {
                              
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[0]) && Minor_test(pos1, Minor, Building[0]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[0]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = OnMap(pos);
                                    buliding1.Add(Buildings);
                                    index++;
                                }
                            }
                            else
                            {
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[4]) && Minor_test(pos1, Minor, Building[4]) && Slope_test(pos1))
                                {
                                    Debug.Log("seed3 :" + seed);
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[4]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = OnMap(pos);
                                    buliding1.Add(Buildings);
                                    index++;
                                }
                            }
                        }

                        flag = false;
                    }
                    
                }
                break;
            default:
                {
                    Debug.Log("No property");
                  
                }
                return;
        }
    }
    //input pos : the position of generating building
    //
    //output: Null
    //
    //Generate a factory


    private bool Slope_test(Point p)
    {
       // Debug.Log("error111");

        if (terrainData.GetSteepness((p.x + terrainData.bounds.extents.x) / terrainData.size.x, (p.z + terrainData.bounds.extents.z) / terrainData.size.z) > 30f)
        {
            return false;
        }
        return true;
    }

    //input : p:test point, b:list of generated building, building: testing object
    //
    //output: result of test
    //
    //test the deistance between this position and generated buildings. 
    private bool Distance_test(Point p, List<Transform> b, Transform building)
    {

       // Debug.Log("building"+ b.Count);

        if (b.Count==0)
        {

            return true;
        }
        else
        {
            for (int i = 0; i < b.Count; i++)
            {
                if ((Mathf.Abs(b[i].localPosition.z - (p.z+ terrainData.bounds.extents.z)) < (b[i].localScale.z/2*root2+building.localScale.z/2*root2)) && (Mathf.Abs(b[i].localPosition.x - (p.x+ terrainData.bounds.extents.x))) < (b[i].localScale.x/2 * root2 + building.localScale.x/2 * root2))
                {
                    return false;
                }
            }   
        }
        return true;
    }

    //input : p:test point, b:list of generated building, building: testing object
    //
    //output: result of test
    //
    //test the deistance between this position and minor roads. 

    private bool Minor_test(Point p, List<Point> b,Transform building)
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
                if ((Mathf.Abs(b[i].z - (p.z + terrainData.bounds.extents.z)) < (RoadWigth*root2+building.localScale.z*root2)) && ((Mathf.Abs(b[i].x - (p.x + terrainData.bounds.extents.x))) < (RoadWigth * root2 + building.localScale.z * root2)))
                {
                  //  Debug.Log("The locaposition is :" + b[i].x + " and " + b[i].z);
                    return false;
                }
               // Debug.Log("The locaposition is :" + b[i].x + " and " + b[i].z);
               // Debug.Log("The position is :" + (p.z + terrainData.bounds.extents.z) + " and " + (p.z + terrainData.bounds.extents.z));
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
            if(p[i].z<a.z&&p[j].z>=a.z||p[j].z<a.z&&p[i].z>=a.z && (p[i].x <= a.x && p[i].x <= a.x))
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
            if (p[i].z < a.z && p[j].z >= a.z || p[j].z < a.z && p[i].z >= a.z &&(p[i].x<=a.x&& p[i].x <= a.x))
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
    //private IEnumerator CreateChildren()
    //{
    //    for (int i = 0; i < Num; i++)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        BuildingGeneration(max_x, max_z, min_x, min_z, 1, Polygon,property);
    //    }
    //}

    void BuildingPlacement()
    {
        //LoadBoundrary(BoundaryPoints);

        //Import_2(this.GetComponent<Boundary>().Poss);

        Divide(Points);

        ReadPolygon(Points);

        CleanDuplicate(Minor);

        CleanDuplicate(Polygon);

        CleanUnnecessaryPoints(Polygon);
       //F Debug.Log("finish clean");
        //ShowPointPolygon(Polygon);
        
        Shrink(Polygon, property);
        

 //       Debug.Log(" //////////////////////loadtest////////////////////////");
        //LoadSmallPolygon(SmallPolygon);
        for (int i = 0; i < SmallPolygon.Count; i++)
        {
            if (SmallPolygon[i].x > max_x)
            {
                max_x = SmallPolygon[i].x;
            }

            if (SmallPolygon[i].z > max_z)
            {
                max_z = SmallPolygon[i].z;
            }

            if (SmallPolygon[i].x < min_x)
            {
                min_x = SmallPolygon[i].x;
            }

            if (SmallPolygon[i].z < min_z)
            {
                min_z = SmallPolygon[i].z;
            }
        }


        //max_x = 4;
        //min_x = -4;
        //max_z = 9;
        //min_z = -9;
        ////StartCoroutine(CreateChildren());
        BuildingGeneration(max_x, max_z, min_x, min_z, SmallPolygon, property,Density,Population);
       // Debug.Log("finish placement");
        //ShowPointPolygon(SmallPolygon);
    }

   

    private void Awake()
    {
        Import(inputJsonFile.text);  
    }

    void Start()
    {
       // Debug.Log("start" + LandData.Landuse.Count);     
        
        for(int i=0; i<LandData.Landuse.Count;i++)
        {

            property = LandData.Landuse[i].Land_usage;
            //Debug.Log("type :" + property);

            //Debug.Log("index :" + i);
            if (property == "none")
            {
                continue;
            }

            Points= LandData.Landuse[i].Polygon;
           // Debug.Log("read list");
         
            Population = LandData.Landuse[i].Population;
            Density = LandData.Landuse[i].Population_density;
            BuildingPlacement();
            Clean();
        }
       
    }

    private void Clean()
    {
        buliding1.Clear();

        SmallPolygon.Clear();

        Points.Clear();

        Polygon.Clear();

        Minor.Clear(); 
    }




    // Update is called once per frame
    void Update()
    {
       //  Buildingtest(max_x, max_z, min_x, min_z, Num, Polygon);
        
    }

    private void Buildingtest(float max_x, float max_z, float min_x, float min_z, int num, List<Point> p)
    {
    
            float x = UnityEngine.Random.Range(min_x, max_x);
            float z = UnityEngine.Random.Range(min_z, max_z);

            Vector3 res;
            res.x = x;
            res.z = z;
            res.y = 72f;

            Point res1 = new Point(res.x, res.z, 0);

            if (Inside_test(res1, p))
            {
                
                Transform Buildings = Instantiate(test);
                
                Buildings.SetParent(transform, false);
                Buildings.localPosition = OnMap(res);
                buliding1.Add(Buildings);
               
            }
        
    }

    
    private void FixedUpdate()
    {
        
    }
}
