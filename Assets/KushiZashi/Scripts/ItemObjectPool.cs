using System.Collections;
using System.Collections.Generic;
using UniRx.Toolkit;
using UnityEngine;

public class ItemObjectPool : ObjectPool<BaseItem>
{
    //ItemのPrefab
    private readonly BaseItem _prefab;
    
    //ヒエラルキウィンドウ上で親となるTransform
    private readonly Transform _root;

    public ItemObjectPool(BaseItem prefab)
    {
        _prefab = prefab;
        
        //親になるObject
        _root = new GameObject().transform;
        _root.name = $"{prefab.name}s";
        _root.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    protected override BaseItem CreateInstance()
    {
        //インスタンスが新しく必要になったらInstantiate
        var newItem = GameObject.Instantiate(_prefab);
        
        //親となるTransformを変更する
        newItem.transform.SetParent(_root);

        return newItem;
    }
}
