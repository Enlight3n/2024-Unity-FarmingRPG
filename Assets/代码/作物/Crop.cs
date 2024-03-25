using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有的作物都附上此脚本组件
/// </summary>
public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0; //标明已在这个作物上收割了几次，比如砍了三次树，但树要五次才能砍倒

    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer;

    [SerializeField] private Transform harvestActionEffectTransform = null;

    [HideInInspector] public Vector2Int cropGridPosition; //记录这个作物是在哪个网格上




    #region pulic

    //处理对作物使用工具的情况，一种是斧头和镐子的砍伐，一种是采集工具的收获
    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown,
        bool isToolUp)
    {
        GridPropertyDetails gridPropertyDetails =
            GridManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
            return;

        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);

        if (seedItemDetails == null)
            return;

        CropDetails cropDetails = GridManager.Instance.GetCropDetails(seedItemDetails.itemCode);

        if (cropDetails == null)
            return;

        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position,
                cropDetails.harvestActionEffect);
        }

        // 如果该工具无法用于收割此作物，则返回 -1，否则返回此工具所需的收割操作次数
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);

        if (requiredHarvestActions == -1)
            return;


        harvestActionCount += 1;

        //如果达到了收割次数，进行收割
        if (harvestActionCount >= requiredHarvestActions)
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
    }

    #endregion




    //处理了农作物的收割逻辑，包括动画播放、声音效果、网格属性的更新以及后续的收割操作。
    //根据是否需要播放动画，收割操作的执行方式有所不同。
    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails,
        GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            //定义了收割后的精灵（harvestedSprite），则将其设置为 cropHarvestedSpriteRenderer 的精灵。    
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            //触发相应的收割动画
            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }

        //播放声音
        if (cropDetails.harvestSound != SoundName.none)
        {
            AudioManager.Instance.PlaySound(cropDetails.harvestSound);
        }

        //更新网格属性
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        //隐藏和禁用碰撞体
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        //使作物在动画播放前不可碰撞
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();

            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        //使用 GridManager 的 SetGridPropertyDetails 方法更新网格的属性
        GridManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,
            gridPropertyDetails);

        //如果需要播放收割动画，则启动一个协程 ProcessHarvestActionsAfterAnimation，在动画播放完成后执行后续的收割操作。
        //如果不需要播放动画，则直接调用 HarvestActions 方法执行收割操作
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }


    //确保在动画播放完成后才执行收割的后续动作
    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails,
        GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        //使用animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested")来检查当前播放的动画是否名为"Harvested"。
        //如果动画名称不是"Harvested"，则暂停协程的执行，直到下一帧
        //协程会不断地检查动画的状态，直到动画播放到名为"Harvested"的帧
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestActions(cropDetails, gridPropertyDetails);
    }


    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        //生成产物
        SpawnHarvestedItems(cropDetails);

        //如果收获后还有剩余，则调用这个方法，比如树砍了之后出现树桩
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }

        Destroy(gameObject);
    }


    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i],
                    cropDetails.cropProducedMaxQuantity[i] + 1);
            }

            //选择生成位置，直接在玩家位置生成/随机位置生成，比如采摘就是在玩家位置，而伐木就是随机位置
            for (int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;

                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f),
                        transform.position.y + Random.Range(-1f, 1f), 0f);
                    
                    SceneItemManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }


    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // 更新网格属性
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceWatered = -1;

        GridManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,
            gridPropertyDetails);


        GridManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }
}