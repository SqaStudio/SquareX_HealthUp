using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

public class AnimationCode : MonoBehaviour
{

    public Transform Sphere;
    public GameObject[] Body;
    List<string> lines;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 33; i++)
        {
            Body[i] = Instantiate(Sphere).gameObject;
        }
        lines = System.IO.File.ReadLines("Assets/AnimationFile.txt").ToList();
        PythonConnector.instance.RegisterFuncOnCallback(Temp);
    }

    public void Temp(string v)
    {
        string[] points = v.Split(',');
        Debug.Log(points.Length);
        if (points.Length != 99)
            return;
        for (int i =0; i<=32;i++)
        {
            float x = float.Parse(points[0 + (i * 3)]) / 100;
            float y = float.Parse(points[1 + (i * 3)]) / 100;
            float z = float.Parse(points[2 + (i * 3)]) / 300;
            Body[i].transform.localPosition = new Vector3(x, y, z);
        }
    }
}