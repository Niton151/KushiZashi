using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Menu
{
    public List<BaseItem> MenuList => _menuList;
    public int SumPrice => _sumPrice;

    [SerializeField]private List<BaseItem> _menuList = new List<BaseItem>();
    private int _sumPrice;

    public Menu(int length, List<BaseItem> available)
    {
        for (int i = 0; i < length; i++)
        {
            _menuList.Add(available[Random.Range(0, available.Count)]);
            _menuList[i].IsCooked = true;
        }

        try
        {
            _sumPrice = _menuList.Sum(x => x.Provider.Price);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
        
    }
}
