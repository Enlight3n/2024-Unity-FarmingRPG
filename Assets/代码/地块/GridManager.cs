using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.SceneManagement;


/// <summary>
/// <para>这个类的作用就是管理网格的GridPropertyDetails——将数据容器中的GridProperty细化为GridPropertyDetails</para>
/// <para>场景保存的时候——保存旧场景的网格数据</para>
/// <para>场景加载的时候——加载新场景的网格数据，接着清除再显示新场景的锄地、浇水贴图，作物</para>
/// <para>日期推进的时候——清除再显示当前场景的锄地、浇水贴图，作物</para>
/// <para>我们用gridPropertyDictionary来指明当前场景的网格数据</para>
/// </summary>
public class GridManager : SingletonMonoBehaviour<GridManager>, ISavable
{
    private Transform cropParentTransform;

    private bool isFirstTimeSceneLoaded = true;
    
    public Grid grid; 
    
    private Tilemap groundDecoration1; //锄地的地面
    private Tilemap groundDecoration2; //锄地+浇水的地面
    
    [SerializeField] private Tile[] dugGround = null; //保存锄地贴图
    [SerializeField] private Tile[] wateredGround = null; //保存浇水贴图
    
    
    //获取场景中全部的数据容器
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;

    //保存种植信息的数据容器
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;
    
    //保存玩家当前所在场景的特殊贴图
    private Dictionary<string, GridPropertyDetails> currentGridPropertyDetailsDictionary;
    
    //用于保存全部场景的信息
    private Dictionary<SceneName, Dictionary<string, GridPropertyDetails>> gridPropertyDetailsDictionary;

    
    protected override void Awake()
    {
        base.Awake();

        gridPropertyDetailsDictionary = new Dictionary<SceneName, Dictionary<string, GridPropertyDetails>>();
        
    }

        
    private void Start()
    {
        InitialiseGridProperties();
        
        EventHandler.CallInstantiateCropPrefabsEvent();
    }
    
    
    private void OnEnable()
    {
        IRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded1;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnload;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }
    
    private void OnDisable()
    {
        IDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded1;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnload;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }


    private void BeforeSceneUnload()
    {
        SceneName sceneName = (SceneName)SceneManager.GetActiveScene().buildIndex;

        gridPropertyDetailsDictionary[sceneName] = currentGridPropertyDetailsDictionary;
    }


    private void AfterSceneLoaded1()
    {
        
        if((GameObject.FindGameObjectWithTag(Tags.CropsParentTransform)!=null))
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform = null;
        }
        
        grid = GameObject.FindObjectOfType<Grid>();
        
        
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
        
        AfterSceneLoaded2();
    }


    private void AfterSceneLoaded2()
    {
        SceneName sceneName = (SceneName)SceneManager.GetActiveScene().buildIndex;
        
        gridPropertyDetailsDictionary.TryGetValue(sceneName, out currentGridPropertyDetailsDictionary);

        SaveLoadManager.Instance.gameSave.isFirstTimeSceneLoadedDictionary.TryGetValue(sceneName, out var bIsFirstTimeSceneLoaded);

        if (bIsFirstTimeSceneLoaded)
        {
            EventHandler.CallInstantiateCropPrefabsEvent();
            
            SaveLoadManager.Instance.gameSave.isFirstTimeSceneLoadedDictionary[sceneName] = false;
        }
        
        ClearDisplayGridPropertyDetails();
        
        DisplayGridPropertyDetails();
    }


    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, Week gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        //清除
        ClearDisplayGridPropertyDetails();

        //遍历多个场景的数据容器
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            
            //获取场景的数据
            gridPropertyDetailsDictionary.TryGetValue(so_GridProperties.sceneName, 
                out Dictionary<string, GridPropertyDetails> gridPropertyDetailsMap);
            
            
            if (gridPropertyDetailsMap != null)
            {
                //处理gridPropertyDetails的字典
                for (int i = gridPropertyDetailsMap.Count - 1; i >= 0; i--)
                {

                    var gridPropertyDetails = gridPropertyDetailsMap.ElementAt(i).Value;

                    #region 随着日子推进，更新相应网格的性质
                        
                    //若有作物，长一天
                    if (gridPropertyDetails.growthDays > -1)
                    {
                        gridPropertyDetails.growthDays++;
                    }
                        
                    //若已浇水，则恢复为未浇水状态
                    if (gridPropertyDetails.daysSinceWatered > -1)
                    {
                        gridPropertyDetails.daysSinceWatered = -1;
                    }
                    
                    #endregion 
                }
            }
        }
    
        //显示
        DisplayGridPropertyDetails();
    }
    

    
    
    /// <summary>
    /// 遍历全部场景的数据容器，根据数据容器中记载的gridProperty，更新对应的gridPropertyDetails
    /// 按照场景名保存到GameObjectSave.sceneData中
    /// </summary>
    private void InitialiseGridProperties()
    {
        gridPropertyDetailsDictionary.Clear();
        
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            Dictionary<string, GridPropertyDetails> gridPropertyDetailsMap =
                new Dictionary<string, GridPropertyDetails>();
            
            //根据数据容器中记载的gridProperty，写入到gridPropertyDetails，并将其添加到字典gridPropertyDetailsMap中
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails _gridPropertyDetails;

                //从传入的gridPropertyDictionary中，找到对应xy坐标的特殊贴图的详细信息
                _gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, 
                    gridPropertyDetailsMap);

                if (_gridPropertyDetails == null)
                {
                    _gridPropertyDetails = new GridPropertyDetails();
                }
                
                
                switch(gridProperty.gridProperty)
                {
                    case 地块性质.可放置物品:
                        _gridPropertyDetails.bCanDropItem = true;
                        break;
                    case 地块性质.可耕作:
                        _gridPropertyDetails.bDiggable = true;
                        break;
                }
                
                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, 
                    _gridPropertyDetails, gridPropertyDetailsMap);
                

            }
            
            gridPropertyDetailsDictionary[so_GridProperties.sceneName] = gridPropertyDetailsMap;
            
        }
        
        gridPropertyDetailsDictionary.TryGetValue(SceneName.农场, out currentGridPropertyDetailsDictionary);
    }
    
    
    #region (private)重要方法：清除和显示全部：锄地、浇水贴图，作物
    
    private void ClearDisplayGridPropertyDetails()
    {
        //清除锄地浇水的瓦片贴图
        ClearDisplayGroundDecorations(); 

        //清除所有的作物
        ClearDisplayAllPlantedCrops();
    }

    

    private void DisplayGridPropertyDetails()
    {
        foreach (KeyValuePair<string, GridPropertyDetails> item in currentGridPropertyDetailsDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
            
            DisplayWateredGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);
        }
    }
    
    #endregion



    #region 销毁和重生物体（这些方法都被上面两个重要方法调用）

    #region (private)清除全部的锄地和浇水贴图 + 销毁作物
    
    //清除所有的锄地和浇水贴图
    private void ClearDisplayGroundDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }
    
    //销毁所有的作物
    private void ClearDisplayAllPlantedCrops()
    {
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();

        foreach (Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }
    #endregion
    

    #region (public)生成作物——根据单个网格的gridPropertyDetails
    
    //根据网格的gridPropertyDetails，计算出作物的阶段，生成作物，并给他们添加Crop脚本组件
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1)
        {
            
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if (cropDetails != null)
            {
                int growthStages = cropDetails.growthDays.Length;
                int currentGrowthStage = 0;

                //通过作物的生长周期，每个周期的生长天数，总成熟天数，以及当前生长天数，计算出当前的阶段
                for (int i = growthStages - 1; i >= 0; i--)
                {
                    if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i]) //growthDays[i]是作物i阶段所需的生长天数
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }


                GameObject cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                Vector3 worldPosition =
                    groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY,
                        0));

                //在网格底边中心生成作物
                worldPosition = new Vector3(worldPosition.x + Settings.GridCellSize / 2, worldPosition.y,
                    worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;

                cropInstance.transform.SetParent(cropParentTransform);

                cropInstance.GetComponent<Crop>().cropGridPosition =
                    new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
        }
    }
    
    #endregion

    
    #region 显示锄地和浇水后的单个瓦片贴图
    
    #region (public)显示的主要方法 

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //如果>-1，说明这块地被锄过，则应该显示
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }
    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Watered
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    #endregion

    
    #region (private)显示的主要方法调用到的方法

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //根据周围的瓦片是否被挖掘来寻找合适的瓦片
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        
        //设置合适的瓦片
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), 
            dugTile0);

        //接下来处理周围四个网格的贴图
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0),
                dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0),
                dugTile2);
        }
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0),
                dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0),
                dugTile4);
        }
    }
    
    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {

        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);
        
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), wateredTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), wateredTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), wateredTile4);
        }
    }
    
    //传入xy坐标，根据周围格子的性质，传出当前锄的格子的恰当贴图
    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //获取周围的网格是否被锄过
        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        #region 根据周围格子是否被锄过来设置合适的瓦片贴图

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }

        return null;

        #endregion 
    }
    
    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        //获取相邻的网格是否被浇过水
        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);
        

        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }

        return null;
        
    }
    
    
    //确认是否坐标为xy的格子被锄过
    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    #endregion
    #endregion
    #endregion
    
    
    
    
    #region 设置和获取的网格详细性质(public)

    //把xy坐标保存到gridPropertyDetails中，并为gridPropertyDictionary新设键值对<key, gridPropertyDetails>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, 
        Dictionary<string, GridPropertyDetails> gridPropertyDetailsMap)
    {
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        gridPropertyDetailsMap[key] = gridPropertyDetails;
    }
    
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        currentGridPropertyDetailsDictionary[key] = gridPropertyDetails;
    }
    
    
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        
        return GetGridPropertyDetails(gridX, gridY, currentGridPropertyDetailsDictionary);
        
    }
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY,  Dictionary<string, GridPropertyDetails> 
        gridPropertyDetailsMap)
    {
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        gridPropertyDetailsMap.TryGetValue(key, out gridPropertyDetails);

        return gridPropertyDetails;
        
    }
    
    #endregion
    
    //根据网格的gridPropertyDetails来获取其上作物具有的Crop脚本组件
    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        //获取网格中心的世界坐标
        Vector3 worldPosition =
            grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        
        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            //GetComponentInParent逐层向上查找，从自己这一层开始递归
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition ==
                new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            //GetComponentInChildren逐层向下查找，从自己这一层开始递归
            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition ==
                new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            //所以这两个无论注释哪一个都能正常运行，因为crop和BoxCollider2D是属于同一component
        }

        return crop;
    }
    
    //根据种子ID获取其作物详情
    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
    }
    
    //传入场景名，获取相应的网格范围（起点，高宽）
    public bool GetGridDimensions(SceneName sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        //遍历场景
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            if (so_GridProperties.sceneName == sceneName)
            {
                gridDimensions.x = so_GridProperties.gridWidth;
                gridDimensions.y = so_GridProperties.gridHigh;

                gridOrigin.x = so_GridProperties.originX;
                gridOrigin.y = so_GridProperties.originY;

                return true;
            }
        }

        return false;
    }

    #region 接口

    public void IRegister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Add(this);
    }

    public void IDeregister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Remove(this);
    }

    public void ISave(ref GameSave gameSave)
    {
        BeforeSceneUnload();
        
        gameSave.gridPropertyDetailsDictionary = gridPropertyDetailsDictionary;
    }

    public void ILoad(GameSave gameSave)
    {
        gridPropertyDetailsDictionary = gameSave.gridPropertyDetailsDictionary;
        
        AfterSceneLoaded2();

    }
    
    #endregion
}
