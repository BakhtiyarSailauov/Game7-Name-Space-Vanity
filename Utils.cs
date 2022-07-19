using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
   static public Material[] GetAllMaterials(GameObject go) 
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        List<Material> mats = new List<Material>();
        foreach (var rend in rends)
        {
            mats.Add(rend.material);
        }
        return (mats.ToArray());
    }
}
