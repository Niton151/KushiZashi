using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ItemObjectPoolを提供する
/// </summary>
public class ItemObjectPoolProvider : MonoBehaviour
{
    public BaseItem Item => _prefab;
    
    [SerializeField] private BaseItem _prefab;
    [SerializeField] private int _unlockCost;
    [SerializeField] private int _level = 1;

    private ItemObjectPool _objectPool;
    private int _upgradeCost;

    public int UpgradeCost => _upgradeCost;
    public int UnlockCost => _unlockCost;

    public ItemObjectPool Get()
    {
        //すでに準備済みならそちらを返す
        if (_objectPool != null) return _objectPool;
        
        //ObjectPoolを作成
        _objectPool = new ItemObjectPool(_prefab);
        
        //アップグレード系の値を初期化
        _upgradeCost = (int) (_unlockCost * 1.1f);

        return _objectPool;
    }
    
    public void GradeUp()
    {
        _level++;
        Item.Price = (int) (Item.Price * 1.1f);
        _upgradeCost = (int) (_upgradeCost * 1.1f);
    }

    private void OnDestroy()
    {
        //すべて破棄
        _objectPool.Dispose();
    }
}
