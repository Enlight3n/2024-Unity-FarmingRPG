using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "so_GridProperties",menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName sceneName;
    public int gridHigh;
    public int gridWidth;
    public int originX;
    public int originY;
    [SerializeField] public List<GridProperty> gridPropertyList;
}