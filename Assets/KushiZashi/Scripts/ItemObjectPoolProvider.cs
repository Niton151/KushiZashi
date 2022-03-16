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
    [SerializeField] private BaseItem _prefab;

    private ItemObjectPool _objectPool;

    public ItemObjectPool Get()
    {
        //すでに準備済みならそちらを返す
        if (_objectPool != null) return _objectPool;
        
        //ObjectPoolを作成
        _objectPool = new ItemObjectPool(_prefab);

        return _objectPool;
    }

    private void OnDestroy()
    {
        //すべて破棄
        _objectPool.Dispose();
    }
}
