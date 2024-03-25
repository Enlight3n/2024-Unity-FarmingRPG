using System.Collections;
using System.Collections.Generic;using System.Security.Claims;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 用来保存游戏中的全部数据，一个GameSave就是一份存档
/// </summary>
/// 场景中物品-
/// 地块性质-
/// 作物情况-
/// 玩家位置，朝向 --
/// 游戏时间-
/// 玩家所在场景 --
/// 玩家物品栏 --
///
[System.Serializable]
public class GameSave
{
    //指明场景是否第一次被加载，如果是第一次被加载，需要初始化
    public Dictionary<SceneName, bool> isFirstTimeSceneLoadedDictionary;
    
    //保存全部场景的地块信息
    public Dictionary<SceneName, Dictionary<string, GridPropertyDetails>> gridPropertyDetailsDictionary;
    
    //保存全部场景的物品信息
    public Dictionary<SceneName, List<SceneItem>> sceneItemDictionary;
    
    //保存时间
    public TimeSave timeSave;
    
    //保存玩家位置
    public Vector3Serializable playerPosition;
    
    //保存玩家物品栏
    public List<InventoryItem> playerInventory;
    
    public GameSave()
    {
        isFirstTimeSceneLoadedDictionary = new Dictionary<SceneName, bool>();
        
        gridPropertyDetailsDictionary = new Dictionary<SceneName, Dictionary<string, GridPropertyDetails>>();

        sceneItemDictionary = new Dictionary<SceneName, List<SceneItem>>();
        
        timeSave = new TimeSave();

        playerPosition = new Vector3Serializable();

        playerInventory = new List<InventoryItem>();
    }
}
[System.Serializable]
public class TimeSave
{
    public int gameYear = 1;
    public Season gameSeason = Season.春;
    public int gameDay = 1;
    public int gameHour = 6;
    public int gameMinute = 30;
    public int gameSecond = 0;
    public Week gameDayOfWeek = Week.周一;
}
