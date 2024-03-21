using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_ItemList",menuName = "Scriptable Objects/Item/Item List")]

//ScriptableObject的一个主要用途是通过避免值的复制来减少你的项目的内存占用。

public class SO_ItemList : ScriptableObject
{
    public List<ItemDetails> itemDetails;
}