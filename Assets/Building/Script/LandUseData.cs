using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LandUseData 
{

    [SerializeField]
    private List<land_usages> land_usages = new List<land_usages>();
    public List<land_usages> Landuse => land_usages;
    public void Clear()
    {
        Landuse.Clear();
    }
}
