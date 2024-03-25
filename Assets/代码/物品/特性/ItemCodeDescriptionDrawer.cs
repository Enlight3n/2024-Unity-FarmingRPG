using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.BeginProperty开始属性的绘制
        EditorGUI.BeginProperty(position, label, property);
        
        //如果属性类型是整数（SerializedPropertyType.Integer），则绘制一个整数字段和一个标签字段。
        //整数字段用于输入整数值，标签字段用于显示与整数值对应的物品描述。
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            //EditorGUI.BeginChangeCheck和EditorGUI.EndChangeCheck用于检测整数字段的值是否发生了变化
            //如果发生了变化，则更新属性的整数值
            EditorGUI.BeginChangeCheck();
            
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, 
                position.width, position.height / 2), label, property.intValue);
            
            
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2,
                    position.width, position.height / 2), "物品名",
                GetItemDescription(property.intValue));
            

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
            
        }
        
        //结束属性的绘制
        EditorGUI.EndProperty();
    }
    
    private string GetItemDescription(int itemCode)
    {
        
        SO_ItemList so_itemList;
        
        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/数据容器/so_ItemList.asset",
            typeof(SO_ItemList)) as SO_ItemList;
        
        List<ItemDetails> itemDetailsList = so_itemList.itemDetails;
        
        //根据物品代码（itemCode）获取对应的物品描述
        ItemDetails itemDetail = itemDetailsList.Find(x => x.itemCode == itemCode);
        
        if (itemDetail != null)
        {
            return itemDetail.itemName;
        }
        else
        {
            return "";
        }
    }
}
