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

    public LandUseData LandData;

    List<Transform> buliding1 =new List<Transform>();
    
    List <Point> SmallPolygon=new List<Point>();

    List<Point> NewSmallPolygon = new List<Point>();

    public Transform pointPrefab;

    public Transform[] Building;

    public Transform test1;

    public Transform test2;

    List<Point> Points = new List<Point>();

    List<Point> Polygon = new List<Point>();

    List<Point> Minor = new List<Point>();

    Vector3 ZeroPoint = new Vector3(0,0,0);
   
    public float Population;

    public float Density;

    float max_x = float.MinValue;
    float max_z = float.MinValue;
    float min_x = float.MaxValue;
    float min_z = float.MaxValue;

    public float inputWidth = 872;
    public float inputHeight = 843;

    public void Import(string path)
    {
        //data = JsonUtility.FromJson<RoadNetworkData>(json);

        LandData = JsonUtility.FromJson<LandUseData>(path);


        for (int i=0;i < LandData.Landuse.Count; i++)
        {
            for(int j = 0; j < LandData.Landuse[i].Polygon.Count; j++)
            {
                LandData.Landuse[i].Polygon[j].x= LandData.Landuse[i].Polygon[j].x / inputWidth * terrainData.size.x;
                LandData.Landuse[i].Polygon[j].z= terrainData.size.z - LandData.Landuse[i].Polygon[j].z / inputHeight * terrainData.size.z;
            }
        }  
    }

    //private Vector3 ConvertCo(Vector3 v)
    //{
    //    Vector3 vector3 = new Vector3();
    //    vector3.x = v.x / 843 * terrainData.size.x;
    //    vector3.y = v.y;
    //    vector3.z = terrainData.size.z-v.z / 843 * terrainData.size.z;
        

    //    return vector3;
    //}

    //private Point ConvertCo(Point p)
    //{
    //    Point point = new Point();

    //    point.x = p.x / 843  * terrainData.size.x;

    //    point.z = terrainData.size.z - p.z / 843* terrainData.size.z;

    //    return point;
    //}

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

            }
            else
            if (p[i].z == p[i-1].z || p[i].z == p[i+1].z)
            {

            }
            else
            {
                slope1 = (p[i].x - p[i - 1].x) / (p[i].z - p[i - 1].z);
                slope2 = (p[i+1].x - p[i].x) / (p[i+1].z - p[i].z);

                
                if (slope1 == slope2)
                {
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



    //void Shrink(List<Point> p, string property)
    //{
    //    //First point;

    //    float Distance;
    //    float Scale=0;

    //    switch (property)
    //    { 
    //    case "industry":
    //        {
    //            Distance = Building[1].localScale.x / 2 * root2 + RoadWigth*root2;
               
    //        }
    //            break;


    //        case "commercial":

    //        {
    //            Distance = Building[2].localScale.x / 2 * root2 + RoadWigth * root2;
                
    //        }
    //            break;

    //        case "residential":
    //        {
    //            Distance = Building[0].localScale.x / 2 * root2 + RoadWigth * root2;
                
    //        }
    //            break;

    //        default:
    //        {
    //            Debug.Log("No property, No shrink");
                
    //        }
    //            return;

    //    }

    //    float sumx=0;
    //    float sumz=0;

    //    for (int i=0;i<p.Count;i++)
    //    {
    //        sumx += p[i].x;
    //        sumz += p[i].z;
    //    }



    //    Point middlepoint = new Point();
    //    middlepoint.x = sumx / p.Count;
    //    middlepoint.z = sumz / p.Count;

    //    //ShowPoint(middlepoint);

    //    for (int i=0;i<p.Count;i++)
    //    {
    //        float length=Mathf.Sqrt((p[i].x-middlepoint.x)* (p[i].x - middlepoint.x)+(p[i].z - middlepoint.z)* (p[i].z - middlepoint.z));
    //        if (length <= Distance)
    //        {
    //            Debug.Log("not enough");
    //            return;
    //        }

    //        float NewScale = Distance / length;

    //        if (NewScale>Scale)
    //        {
    //            Scale = Distance / length;
    //        }

    //        Scale = Distance / length;
    //        float x = middlepoint.x + (p[i].x - middlepoint.x) * (1-Scale);
    //        float z = middlepoint.z + (p[i].z - middlepoint.z) * (1-Scale);
    //        Point point = new Point(x, z, 0);
    //        SmallPolygon.Add(point);
            
    //    }
    //}

    Vector3 GenerateRandomPosition(float max_x, float max_z, float min_x, float min_z)
    {
        float x = UnityEngine.Random.Range(min_x, max_x);
        float z = UnityEngine.Random.Range(min_z, max_z);

        Vector3 res;
        res.x = x;
        res.y = 0f;
        res.z = z;
        return res;
    }

    void BuildingGeneration(float max_x, float max_z,float min_x, float min_z, List<Point> p, string pro, float density, float population)
    {
        int app = 0;
        bool flag =false;
        int seed;
        int num=3;
        //if (population<100)
        // num= (int)(population/5)+1;
        //else 
        //    if (population < 1000)
        //    num = (int)(population /25) + 1;
        //else
        //    if (population < 2000)
        //    num = (int)(population / 40) + 1;
        //else
        //    num = (int)(population / 70) + 1;
        Vector3 pos = new Vector3();
        Point pos1 = new Point();        
            
                switch (pro)
                {
                case "industry":
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
                            pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                   + Building[1].localScale.y / 2 - 0.2f;

                            pos1.x = pos.x;
                            pos1.z = pos.z;
                            if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[1]) && Minor_test(pos1, Minor, Building[1]) && Slope_test(pos1))
                            {
                                flag = true;
                                Transform Buildings = Instantiate(Building[1]);
                                Buildings.SetParent(transform, false);
                                Buildings.localPosition = pos;
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

                           
  

                            pos1.x = pos.x;
                            pos1.z = pos.z;

                            seed = UnityEngine.Random.Range(0, 99);
                        
                             if (seed < 19)
                            {
                                
                                pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                       + Building[2].localScale.y / 2 - 0.2f;
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[2]) && Minor_test(pos1, Minor, Building[2]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[2]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = pos;
                                    buliding1.Add(Buildings);
                                    index++;
                                }
                            }
                            else
                            {
                                
                                pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                       + Building[3].localScale.y / 2 - 0.2f;
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[3]) && Minor_test(pos1, Minor, Building[3]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[3]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = pos;
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

                           
                            pos1.x = pos.x;
                            pos1.z = pos.z;

                            seed = UnityEngine.Random.Range(0, 99);
                           
                            if (seed < (11+(density-0.003)/0.032*300))
                                {
                                 
                                pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                       + Building[3].localScale.y / 2 - 0.2f;
                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[3]) && Minor_test(pos1, Minor, Building[3]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[3]);
                                    //Buildings.localScale *= 100;
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = pos;
                                    buliding1.Add(Buildings);
                                }
                            }
                            else
                            
                             if (seed < (79 - ((density - 0.003) / 0.032) *300))
                            {

                                 
                                pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                       + Building[0].localScale.y / 2 - 0.2f;

                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[0]) && Minor_test(pos1, Minor, Building[0]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[0]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = pos;
                                    buliding1.Add(Buildings);
                                    index++;
                                }
                            }
                            else
                            {
                                
                                pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                       + Building[4].localScale.y / 2 - 0.2f;

                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[4]) && Minor_test(pos1, Minor, Building[4]) && Slope_test(pos1))
                                {
                                    Debug.Log("seed3 :" + seed);
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[4]);
                                    Buildings.SetParent(transform, false);
                                    Buildings.localPosition = pos;
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
        if (terrainData.GetSteepness((p.x) / terrainData.size.x, (p.z) / terrainData.size.z) > 30f)
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
        if (b.Count==0)
        {

            return true;
        }
        else
        {
            for (int i = 0; i < b.Count; i++)
            {
                if ((Mathf.Abs(b[i].localPosition.z - p.z) < (b[i].localScale.z+building.localScale.z)) || (Mathf.Abs(b[i].localPosition.x - p.x)) < (b[i].localScale.x/2 + building.localScale.x/2))
                {
                    return false;
                }
                //else
                //{
                //    //Debug.Log("distance Z " + (Mathf.Abs(b[i].localPosition.z - p.z) + "distance X " + Mathf.Abs(b[i].localPosition.x - p.x)));
                //    //Debug.Log("distance Z " + (Mathf.Abs(b[i].localPosition.z - p.z) + "distance Z " + Mathf.Abs(b[i].localPosition.x - p.x)));
                //    //Debug.Log("distance Z " + (Mathf.Abs(b[i].localPosition.z - p.z) + "distance Z " + Mathf.Abs(b[i].localPosition.x - p.x)));
                //    //Debug.Log("distance Z " + (Mathf.Abs(b[i].localPosition.z - p.z) + "distance Z " + Mathf.Abs(b[i].localPosition.x - p.x)));
                //}
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
        if (b.Count == 0)
        {
            return true;
        }
        else
        {
            for (int i = 0; i < b.Count; i++)
            {
                if ((Mathf.Abs(b[i].z - (p.z)) < (RoadWigth*root2+building.localScale.z*root2)) && ((Mathf.Abs(b[i].x - (p.x + terrainData.bounds.extents.x))) < (RoadWigth * root2 + building.localScale.z * root2)))
                {
                    return false;
                }
            }

        }
        return true;
    }




    //
    //
    //Inside test for point
    //
    //

bool pointInRegion(Point pt, List<Point> plist)
    {
        int nCross = 0;    // 定义变量，统计目标点向右画射线与多边形相交次数

        for (int i = 0; i < plist.Count; i++)
        {   //遍历多边形每一个节点

            Point p1;
            Point p2;

            p1 = plist[i];
            p2 = plist[(i + 1) % plist.Count];  // p1是这个节点，p2是下一个节点，两点连线是多边形的一条边
                                                 // 以下算法是用是先以y轴坐标来判断的

            if (p1.z == p2.z)
                continue;   //如果这条边是水平的，跳过

            if (pt.z < Mathf.Min(p1.z, p2.z)) //如果目标点低于这个线段，跳过
                continue;

            if (pt.z >= Mathf.Max(p1.z, p2.z)) //如果目标点高于这个线段，跳过
                continue;
            //那么下面的情况就是：如果过p1画水平线，过p2画水平线，目标点在这两条线中间
            float x = (pt.z - p1.z) * (p2.x - p1.x) / (p2.z - p1.z) + p1.x;
            // 这段的几何意义是 过目标点，画一条水平线，x是这条线与多边形当前边的交点x坐标
            if (x > pt.x)
                nCross++; //如果交点在右边，统计加一。这等于从目标点向右发一条射线（ray），与多边形各边的相交（crossing）次数
        }

        if (nCross % 2 == 1)
        {

            return true; //如果是奇数，说明在多边形里
        }
        else
        {

            return false; //否则在多边形外 或 边上
        }

    }


    private bool Inside_test(Point checkPoint, List<Point> polygonPoints)
    {
        int counter = 0;
        int i;
        float xinters;
        Point p1, p2;
        int pointCount = polygonPoints.Count;
        p1 = polygonPoints[0];
        for (i = 1; i <= pointCount; i++)
        {
            p2 = polygonPoints[i % pointCount];
            if (checkPoint.z > Math.Min(p1.z, p2.z)//校验点的Y大于线段端点的最小Y  
                && checkPoint.z <= Math.Max(p1.z, p2.z))//校验点的Y小于线段端点的最大Y  
            {
                if (checkPoint.x <= Math.Max(p1.x, p2.x))//校验点的X小于等线段端点的最大X(使用校验点的左射线判断).  
                {
                    if (p1.z != p2.z)//线段不平行于X轴  
                    {
                        xinters = (checkPoint.z - p1.z) * (p2.x - p1.x) / (p2.z - p1.z) + p1.x;
                        if (p1.x == p2.x || checkPoint.x <= xinters)
                        {
                            counter++;
                        }
                    }
                }

            }
            p1 = p2;
        }

        if (counter % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //bool Inside_test(Point a, List<Point> p)
    //{
    //    //bool res=false;

    //    //int i;
    //    //int j = p.Count - 1;
    //    //float res1;
    //    //float res2;

    //    //int right=0;
    //    //int left=0;
    //    //for (i = 0; i < p.Count;i++)
    //    //{         
    //    //    if(p[i].z<a.z&& p[j].z>=a.z|| p[j].z<a.z&& p[i].z>=a.z )
    //    //    {
    //    //        res1 = (p[i].x+(a.z - p[i].z)) / (p[j].z - p[i].z) * (p[j].x - p[i].x);
                
    //    //        if(res1<a.x)
    //    //        {
    //    //            res = !res;
    //    //        }
    //    //    }
    //    //    j = i;
    //    //}
    //    //return res;
    //}

    ////private IEnumerator CreateChildren()
    ////{
    ////    for (int i = 0; i < Num; i++)
    ////    {
    ////        yield return new WaitForSeconds(0.1f);
    ////        BuildingGeneration(max_x, max_z, min_x, min_z, 1, Polygon,property);
    ////    }
    //}

    void ShowBoundary(List<Point> P, Transform t)
    {

        for (int i = 0; i < P.Count; i++)
        {
            Transform Buildings = Instantiate(t);
            Buildings.SetParent(transform, false);
            Vector3 vector3 = new Vector3();
            vector3.x = P[i].x;
            vector3.z = P[i].z;
            vector3.y = terrainData.GetInterpolatedHeight((P[i].x / terrainData.size.x), ((P[i].z / terrainData.size.z)))
                                           + t.localScale.y / 2 - 0.2f;
            Buildings.localPosition = vector3;
        }
    }

    void BuildingPlacement()
    {
        Divide(Points);

        ReadPolygon(Points);

        CleanDuplicate(Minor);

        CleanDuplicate(Polygon);

     

        //CleanUnnecessaryPoints(Polygon);

        //ShowBoundary(Polygon,test2);
     
        //Shrink(Polygon, property);

        //ShowBoundary(SmallPolygon,test2);

        for (int i = 0; i < Polygon.Count; i++)
        {
            if (Polygon[i].x > max_x)
            {
                max_x = Polygon[i].x;
            }

            if (Polygon[i].z > max_z)
            {
                max_z = Polygon[i].z;
            }

            if (Polygon[i].x < min_x)
            {
                min_x = Polygon[i].x;
            }

            if (Polygon[i].z < min_z)
            {
                min_z = Polygon[i].z;
            }

        }

        //SmallPolygon.Add(new Point(min_x, min_z, 0));
        //SmallPolygon.Add(new Point(min_x, max_z, 0));
        //SmallPolygon.Add(new Point(max_x, min_z, 0));
        //SmallPolygon.Add(new Point(max_x, max_z, 0));

        //ShowBoundary(SmallPolygon, test1);

        BuildingGeneration(max_x, max_z, min_x, min_z, Polygon, property, Density, Population);
    }



    private void Awake()
    {
        Import(inputJsonFile.text);  
    }

    void Start()
    {
        // Debug.Log("start" + LandData.Landuse.Count);     

         for(int i=0; i<LandData.Landuse.Count;i++)

        //for (int i = 0; i < 3; i++)
        {

            property = LandData.Landuse[i].Land_usage;

            if (property == "none")
            {
                continue;
            }

            Points= LandData.Landuse[i].Polygon;
         
            Population = LandData.Landuse[i].Population;
            Density = LandData.Landuse[i].Population_density;
            BuildingPlacement();
            //ShowBoundary(Points);
            Clean();
            
        }
        Debug.Log("finish");
       
    }

    private void Clean()
    {
        buliding1.Clear();

        SmallPolygon.Clear();

        Points.Clear();

        Polygon.Clear();

        Minor.Clear();

        max_x = float.MinValue;
        max_z = float.MinValue;
        min_x = float.MaxValue;
        min_z = float.MaxValue;
    }




    // Update is called once per frame
    void Update()
    {
       //  Buildingtest(max_x, max_z, min_x, min_z, Num, Polygon);
        
    }

     
    private void FixedUpdate()
    {
        
    }
}
