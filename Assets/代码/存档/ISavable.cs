using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable 
{
    #region 注册到SaveLoadManager中
    
    void IRegister();

    void IDeregister();

    #endregion
    

    #region 存档读档

    void ISave(ref GameSave gameSave);

    void ILoad(GameSave gameSave);

    #endregion
}
