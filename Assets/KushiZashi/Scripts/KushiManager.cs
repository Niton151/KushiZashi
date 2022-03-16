using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KushiManager : MonoBehaviour
{
    [NonSerialized]public bool isEmpty = true;
    public List<BaseItem> StockItems => _stockItems;

    [SerializeField]private List<BaseItem> _stockItems = new List<BaseItem>();

    private Material _skin;
    
    
    void Start()
    {
        
    }
}
