using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneItemManager : SingletonMonoBehaviour<SceneItemManager>, ISavable
{
    
    private Transform parentItem; //获取场景中所有可拾取物体的父物体的Item
    
    [SerializeField] private GameObject itemPrefab = null; //预制体中的item，用来作为生成物体的模板

    //保存全部场景的物品信息
    private Dictionary<SceneName, List<SceneItem>> sceneItemDictionary;
    
    
    #region 场景物体生成和销毁的相关函数
    

    //摧毁场景中所有带Item脚本的物体,这里应该用在恢复场景的时候，先全部删掉，看有什么再逐一补上
    private void DestroySceneItems()
    {
        //寻找场景中，所有具有Item脚本的物体
        Item[] itemInScene = GameObject.FindObjectsOfType<Item>();

        //倒序遍历，逐一销毁
        for (int i = itemInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemInScene[i].gameObject);
        }
    }

    //根据传入的物品代码和位置，生成单个物品，用于收获
    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);

        Item item = itemGameObject.GetComponent<Item>();

        item.Init(itemCode);
    }

    //根据SceneItem的数据列表，逐一生成物体，用于场景加载后
    private void InstantiateSceneItems(List<SceneItem> _sceneItemList)
    {
        GameObject itemGameObject;

        foreach (SceneItem sceneItem in _sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab,
                new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z),
                Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();

            item.ItemCode = sceneItem.itemCode;
            
            itemGameObject.name = sceneItem.itemName;
            
            item.Init(item.ItemCode);
        }
    }
    
    #endregion
    
    
    
    //场景加载以后
    private void AfterSceneLoaded()
    {
        //获取Item物体的transform
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemParentTransform).transform;

        //获取保存的sceneItems
        sceneItemDictionary.TryGetValue((SceneName)SceneManager.GetActiveScene().buildIndex, out var sceneItems);
        
        
        //根据SceneItem的数据列表，逐一生成物体，如果不空，说明还没保存过，不用管
        if (sceneItems != null)
        {
            DestroySceneItems();
            
            InstantiateSceneItems(sceneItems);
        }
    }


    protected override void Awake()
    {
        base.Awake();
        sceneItemDictionary = new Dictionary<SceneName, List<SceneItem>>();
    }


    private void OnEnable()
    {
        IRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnload;
    }

    //场景卸载以前，保存场景中的物品到sceneItemDictionary
    private void BeforeSceneUnload()
    {
        var gameObjects = parentItem.GetComponentsInChildren<Item>();
        
        List<SceneItem> sceneItems = new List<SceneItem>();
        
        foreach (var item in gameObjects)
        {
            SceneItem sceneItem = new SceneItem();
            
            sceneItem.itemCode = item.ItemCode;
            
            var position = item.gameObject.transform.position;
            sceneItem.position = new Vector3Serializable(position.x, position.y, position.z);
            
            sceneItems.Add(sceneItem);
        }
        
        sceneItemDictionary[(SceneName)SceneManager.GetActiveScene().buildIndex] = sceneItems;
    }


    private void OnDisable()
    {
        IDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnload;
    }

    #region 接口函数
    
    //启用脚本时，把脚本添加到SaveLoadManager中的iSaveableObjectList
    public void IDeregister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Remove(this);
    }
    
    //禁用脚本时，把脚本从SaveLoadManager中的iSaveableObjectList移除
    public void IRegister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Add(this);
    }
    
    
    public void ISave(ref GameSave gameSave)
    {
        BeforeSceneUnload();
        
        gameSave.sceneItemDictionary = sceneItemDictionary;
    }

    public void ILoad(GameSave gameSave)
    {
        sceneItemDictionary = gameSave.sceneItemDictionary;

        if (parentItem == null)
        {
            //获取Item物体的transform
            parentItem = GameObject.FindGameObjectWithTag(Tags.ItemParentTransform).transform;
        }
        
        //根据SceneItem的数据列表，逐一生成物体，如果不空，说明还没保存过，不用管
        if (sceneItemDictionary.TryGetValue((SceneName)SceneManager.GetActiveScene().buildIndex, out var sceneItemList))
        {
            
            DestroySceneItems();
            
            InstantiateSceneItems(sceneItemList);
        }
    }
    #endregion
}
