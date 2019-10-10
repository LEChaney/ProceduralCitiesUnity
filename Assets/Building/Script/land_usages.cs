using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class land_usages 
{
    [SerializeField]
    private List<Point> polygon;
    public List<Point> Polygon => polygon;
    //private List<Dictionary<float, float>> polygon;
    //public List<Dictionary<float,float>> Polygon => polygon;
    [SerializeField]
    private string land_usage;
    public string Land_usage => land_usage;

    [SerializeField]
    private float population_density;
    public float Population_density => population_density;


    [SerializeField]
    private float population;
    public float Population => population;


    //public land_usages(List<Dictionary<float, float>> polygon, string land_usage, float population_density, float population)
    //{
    //    this.polygon = polygon;
    //    this.land_usage = land_usage;
    //    this.population_density = population_density;
    //    this.population = population;
    //}
    public land_usages(List<Point> polygon, string land_usage, float population_density, float population)
    {
        this.polygon = polygon;
        this.land_usage = land_usage;
        this.population_density = population_density;
        this.population = population;
    }
}
