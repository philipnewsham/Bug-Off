using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : ScriptableObject
{
    public int flyIndex;
    public int[] spiderIndexes;
    public Vector2[] webPoints;
    [SerializeField] public int[] webLinesA;
    [SerializeField] public int[] webLinesB;
    public float cameraSize;
}
