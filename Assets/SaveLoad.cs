using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SaveLoad : MonoBehaviour
{
    
    private bool isHoldingButtonSave;
    private bool isHoldingButtonLoad;
    [SerializeField] private Scrollbar holdScrollbar;
    private float holdingTime;
    
    
    private void Update()
    {
        CheckHoldingStatus();
    }
    
    public void StartHoldSave(BaseEventData baseEventData)
    {
        isHoldingButtonSave = true;
        holdScrollbar.gameObject.SetActive(true);
        
        holdingTime = 0f;
    }

    public void StartHoldLoad(BaseEventData baseEventData)
    {
        isHoldingButtonLoad = true;
        holdScrollbar.gameObject.SetActive(true);
        
        holdingTime = 0f;
    }
    
    public void EndHold(BaseEventData baseEventData)
    {
        isHoldingButtonSave = false;
        isHoldingButtonLoad = false;
        holdScrollbar.gameObject.SetActive(false);
        
        /*
        //松开按钮后，按钮应当变回原色，这里可以我们用原色去替换被选择时的颜色，或者不用代码，直接设置原色和被选择的颜色一致即可
        ColorBlock colors1 = SaveButton.colors;
        colors1.selectedColor = colors1.normalColor;
        SaveButton.colors = colors1;
        
        ColorBlock colors2 = LoadButton.colors;
        colors2.selectedColor = colors2.normalColor;
        LoadButton.colors = colors2;
        */
        
        
        /*//只有长按成功，说明点击成功，此时才关闭菜单
        if (holdingTime > 1f)
        {
            //这个是SaveDataToFile()和LoadDataFromFile()的最后一部分，应当在所有执行完以后再关闭菜单
            UIManager.Instance.EnableRunningMenu();
        }
        
        holdingTime = 0f;*/
        
    }
    

    private void CheckHoldingStatus()
    {
        holdScrollbar.size = holdingTime;
        
        holdingTime += 0.02f;  //这里不能用Time.deltaTime，因为Time.timeScale=0
        
        if (isHoldingButtonSave)
        {
            if (holdingTime > 1f)
            {
                SaveLoadManager.Instance.SaveDataToFile();
                
                isHoldingButtonSave = false;
                isHoldingButtonLoad = false;
                holdScrollbar.gameObject.SetActive(false);
                
                //执行完以后再关闭菜单
                UIManager.Instance.EnableRunningMenu();
            }
        }

        if (isHoldingButtonLoad)
        {
            if (holdingTime > 1f)
            {
                SaveLoadManager.Instance.LoadDataFromFile();
                
                isHoldingButtonSave = false;
                isHoldingButtonLoad = false;
                holdScrollbar.gameObject.SetActive(false);
                
                //执行完以后再关闭菜单
                UIManager.Instance.EnableRunningMenu();
                
            }
        }
    }
}
