using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genareate : MonoBehaviour
{
    
    float root2 = 1.42f;

    public float RoadWidth = 7f;
    
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
        int num=50;
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
                            if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[1]) && Road_test(pos1, Points, Building[1]) && Slope_test(pos1))
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
                        if (density < 0.002)
                        {
                            while (!flag)
                            {
                                app++;
                                if (app > 1000)
                                {
                                    Debug.Log("Too much buildings");
                                    return;
                                }

                                seed = UnityEngine.Random.Range(0, 99);

                                if (seed < 30)
                                {

                                    pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                    pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                           + Building[2].localScale.y / 2 - 0.2f;

                                    pos1.x = pos.x;
                                    pos1.z = pos.z;
                                    if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[2]) && Road_test(pos1, Points, Building[2]) && Slope_test(pos1))
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
                                    pos1.x = pos.x;
                                    pos1.z = pos.z;
                                    if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[3]) && Road_test(pos1, Points, Building[3]) && Slope_test(pos1))
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
                        else
                        {

                            while (!flag)
                            {
                                app++;
                                if (app > 1000)
                                {
                                    Debug.Log("Too much buildings");
                                    return;
                                }

                                seed = UnityEngine.Random.Range(0, 99);

                                if (seed < 30)
                                {

                                    pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                    pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                           + Building[2].localScale.y / 2 - 0.2f;

                                    pos1.x = pos.x;
                                    pos1.z = pos.z;
                                    if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[2]) && Road_test(pos1, Points, Building[2]) && Slope_test(pos1))
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
                                    pos1.x = pos.x;
                                    pos1.z = pos.z;
                                    if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[3]) && Road_test(pos1, Points, Building[3]) && Slope_test(pos1))
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
                   
                }
                break;
            case "residential":
                {
                    if (density > 0.002)
                        num = num / 30 + 1;
                    else
                        num = num / 5 + 1;

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

             

                       
                           
                            if (density <= 0.002)
                            {

                                pos = GenerateRandomPosition(max_x, max_z, min_x, min_z);
                                pos.y = terrainData.GetInterpolatedHeight((pos.x / terrainData.size.x), ((pos.z / terrainData.size.z)))
                                       + Building[4].localScale.y / 2 - 0.2f;
                                pos1.x = pos.x;
                                pos1.z = pos.z;

                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[4]) && Road_test(pos1, Points, Building[4]) && Slope_test(pos1))
                                {
                                    // Debug.Log("seed3 :" + seed);
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[4]);
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
                                       + Building[0].localScale.y / 2 - 0.2f;

                                pos1.x = pos.x;
                                pos1.z = pos.z;

                                if (Inside_test(pos1, p) && Distance_test(pos1, buliding1, Building[0]) && Road_test(pos1, Points, Building[0]) && Slope_test(pos1))
                                {
                                    flag = true;
                                    Transform Buildings = Instantiate(Building[0]);
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

    private bool Road_test(Point p, List<Point> road, Transform transform)
    {
        float distance = 3+root2*RoadWidth + Mathf.Sqrt(transform.localScale.x / 2 * transform.localScale.x / 2 + transform.localScale.y / 2 * transform.localScale.y / 2);
        //Debug.Log("x "+transform.localScale.x / 2);
        //Debug.Log("distance "+distance);
       for(int i=0;i<road.Count;i++)
        {
          if(PointToLine(road[i],road[(i + 1) % road.Count],p)<=distance)
            {
                return false;
            }
           
        }
        //for (int i = 0; i < road.Count; i++)
        //{

        //    Debug.Log("标准距离 " + distance);
        //    Debug.Log("距离"+PointToLine(road[i], road[(i + 1) % road.Count], p));
        //    Debug.Log("start x " + road[i].x + "start z" + road[i].z);
        //    Debug.Log("end x " + road[(i + 1) % road.Count].x + "end z" + road[(i + 1) % road.Count].z);
        //    Debug.Log("x " + p.x + " z" + p.z);
        //    float a = LineLength(road[i], road[(i + 1) % road.Count]);
        //    float b = LineLength(road[i], p);
        //    float c = LineLength(road[(i + 1) % road.Count], p);

        //    float pc = (a + b + c) / 2;
        //    Debug.Log("半周长"+pc);
        //    float s = Mathf.Sqrt(pc * (pc - a) * (pc - b) * (pc - c));
        //    Debug.Log("面积"+s);
        //}
       
        return true;
    }

    //input pos : the position of generating building
    //
    //output: Null
    //
    //Generate a factory


    private bool Slope_test(Point p)
    {
        if (terrainData.GetSteepness((p.x) / terrainData.size.x, (p.z) / terrainData.size.z) > 390f)
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
                if ((Mathf.Abs(b[i].localPosition.z - p.z) < (b[i].localScale.z/2+building.localScale.z/2)+10) && (Mathf.Abs(b[i].localPosition.x - p.x)) < (b[i].localScale.x/2 + building.localScale.x/2)+10)
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

bool Inside_test(Point pt, List<Point> plist)
    {
        int nCross = 0;   

        for (int i = 0; i < plist.Count; i++)
        {   

            Point p1;
            Point p2;

            p1 = plist[i];
            p2 = plist[(i + 1) % plist.Count];  
                                                 

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

       // ShowBoundary(Polygon,test2);
     
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
    private float LineLength(Point a, Point b)
    {
        float Length = 0;
        Length = Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));

        return Length;

    }

    private float PointToLine(Point start, Point end, Point test)
    {
        float length = 0;
        float a, b, c;
        a = LineLength(start, end);   
        b = LineLength(start, test);   
        c = LineLength(end, test);   
        if (c <= 0.000001 || b <= 0.000001)
        {
            length = 0;
            return length;
        }
        if (c * c >= a * a + b * b)
        {
            length = b;
            return length;
        }
        if (b * b >= a * a + c * c)
        {
            length = c;
            return length;
        }
        float p = (a + b + c) / 2;
        //Debug.Log(p);
        float s = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
        //Debug.Log(s);
        length = 2 * s / a;
        //Debug.Log(length);
        return length;
    }



    private void Awake()
    {
        Import(inputJsonFile.text);  
    }

    void Start()
    {
        // Debug.Log("start" + LandData.Landuse.Count);     

        for(int i=0; i<LandData.Landuse.Count;i++)

        //for (int i = 0; i < 1; i++)
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
