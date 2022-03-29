using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    [SerializeField] private List<ItemObjectPoolProvider> _items = new List<ItemObjectPoolProvider>();

    public List<ItemObjectPoolProvider> Items => _items;
}
