using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable 
{
    #region 注册到SaveLoadManager中
    
    void SavableRegister();

    void SavableDeregister();

    #endregion
    
    
    #region 存储和恢复

    void SavableStoreScene(string sceneName);

    void SavableRestoreScene(string sceneName);
    
    #endregion
}
