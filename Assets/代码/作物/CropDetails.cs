using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription] public int 种子id; //种子物体的ID
    
    [Header("各个生长阶段参数")]
    public int[] growthDays; //从头开始，生长到第i个阶段总共所需的生长天数
    public GameObject[] growthPrefab;// 每个生长阶段的预制体
    public Sprite[] growthSprite; // 每个生长阶段的精灵
    
    public Sprite harvestedSprite; // 收获时的精灵
    
    [ItemCodeDescription] 
    public int harvestedTransformItemCode; //收获后转化为的物体ID，不一定会有。如树被砍伐了转化为树桩
    
    //在收获动画以前，是否隐藏收获动画调用的精灵
    public bool hideCropBeforeHarvestedAnimation;  
    
    //在收获动画以前，指明作物碰撞体是否被禁用
    public bool disableCropCollidersBeforeHarvestedAnimation; 
    
    //是否在最后阶段存在收获动画
    public bool isHarvestedAnimation; 
    
    //是否在玩家的位置生成作物
    public bool spawnCropProducedAtPlayerPosition;
   
    public bool isHarvestActionEffect = false; // // 是否在收割动作时具有粒子效果
    public HarvestActionEffect harvestActionEffect; // 指明哪种粒子效果
    
    [ItemCodeDescription] public int[] harvestToolItemCode; //指明用来收获的工具的类型，如果是0，则表明不用工具
    
    public int[] requiredHarvestActions; ////每种工具对应的收获动作的数量，如树需要斧子砍五次才倒
    
    [ItemCodeDescription] public int[] cropProducedItemCode; //收获的物体的ID
    
    //// 收获的最小和最大作物数量，如果不一致，返回其间的随机数，这里的数组和收获的物体对应
    public int[] cropProducedMinQuantity; 
    public int[] cropProducedMaxQuantity; 
    
    
    public SoundName harvestSound;


    /// <summary>
    /// 若工具ID可以用来收获这个作物则返回true
    /// </summary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
    
    /// <summary>
    /// 如果该工具无法用于收割此作物，则返回 -1，否则返回此工具所需的收割操作次数
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}

