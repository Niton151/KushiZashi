using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private List<ItemObjectPoolProvider> _items = new List<ItemObjectPoolProvider>();

    public List<ItemObjectPoolProvider> Items => _items;
}
