using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
{
    //用iSavableObjectList来保存所有继承了接口的类
    public List<ISavable> iSavableObjectList;
    
    public GameSave gameSave;


    protected override void Awake()
    {
        base.Awake();
        
        gameSave = new GameSave();

        iSavableObjectList = new List<ISavable>();
        
        for (int i=0;i<SceneManager.sceneCountInBuildSettings;i++)
        {
            gameSave.isFirstTimeSceneLoadedDictionary.Add((SceneName)i, true);
        }
    }


    public void ReStoreCurrentSceneData()
    {
        
    }

    public void StoreCurrentSceneData()
    {
        
    }
    
    #region 从文件中加载和保存

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/Save.data"))
        {
            
            FileStream file = File.Open(Application.persistentDataPath + "/Save.data", FileMode.Open);

            gameSave = (GameSave)bf.Deserialize(file);
            
            // 遍历iSavableObjectList
            for (var i = iSavableObjectList.Count - 1; i >= 0 ; i--)
            {
                iSavableObjectList[i].ILoad(gameSave);
            }
            
            file.Close();
        }
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();
        
        foreach (var iSavableObject in iSavableObjectList)
        {
            iSavableObject.ISave(ref gameSave);
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/Save.data", FileMode.Create);

        bf.Serialize(file, gameSave);

        file.Close();
    }
    
    #endregion
}
