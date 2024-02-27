using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/MapSellectionData", fileName = "MapSellectionData")]
public class MapSellectionData : ScriptableObject
{
    public List<MapInfo> Maps;
}
[Serializable]
public struct MapInfo
{
    public Color MapThumbnail;
    public string MapName;
    public string SceneName;
}
