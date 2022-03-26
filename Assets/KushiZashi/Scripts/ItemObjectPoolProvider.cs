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
    [SerializeField] private int _initLevel;
    [SerializeField] private string _name;
    [SerializeField] private Vector3 _initialVelocity;
    [SerializeField] private float _cookTime = 3;
    [SerializeField] private Material _cookedMat;
    [SerializeField] private Material _defaultMat;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _initPrice;

    private ItemObjectPool _objectPool;
    private int _upgradeCost;
    private int _price;
    private int _level;

    public int UpgradeCost => _upgradeCost;
    public int UnlockCost => _unlockCost;
    public string Name => _name;
    public Sprite Icon => _icon;
    public int Price => _price;
    public Vector3 InitialVelocity => _initialVelocity;
    public float CookTime => _cookTime;
    public Material CookedMat => _cookedMat;
    public Material DefaultMat => _defaultMat;


    public ItemObjectPool Get()
    {
        //すでに準備済みならそちらを返す
        if (_objectPool != null) return _objectPool;
        
        //ObjectPoolを作成
        _objectPool = new ItemObjectPool(_prefab);

        //アップグレード系の値を初期化
        _price = _initPrice;
        _level = _initLevel;
        _upgradeCost = (int) (_unlockCost * 1.1f);

        return _objectPool;
    }

    public void GradeUp()
    {
        _level++;
        PaymentManager.Fund -= _upgradeCost;
        _price = (int)Mathf.Ceil(_price * 1.05f);
        _upgradeCost = (int) Mathf.Ceil(_upgradeCost * 1.05f);
    }

    private void OnDestroy()
    {
        //すべて破棄
        _objectPool.Dispose();
    }
}