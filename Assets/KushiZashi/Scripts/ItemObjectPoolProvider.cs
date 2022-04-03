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
    [SerializeField] private string _description;

    private ItemObjectPool _objectPool;
    private int _upgradeCost;
    private int _price;
    private int _level;
    private int _plus;

    public int UpgradeCost => _upgradeCost;
    public int UnlockCost => _unlockCost;
    public string Name => _name;
    public Sprite Icon => _icon;
    public int Price => _price;
    public Vector3 InitialVelocity => _initialVelocity;
    public float CookTime => _cookTime;
    public Material CookedMat => _cookedMat;
    public Material DefaultMat => _defaultMat;
    public string Description => _description;
    public int Level => _level;
    public int Plus => _plus;


    public ItemObjectPool Get()
    {
        //すでに準備済みならそちらを返す
        if (_objectPool != null) return _objectPool;
        
        //ObjectPoolを作成
        _objectPool = new ItemObjectPool(_prefab);

        //アップグレード系の値を初期化
        _price = _initPrice;
        _plus = (int)Mathf.Ceil(_price * (_initPrice * 0.01f));
        _level = _initLevel;
        _upgradeCost = (int) Mathf.Ceil(_unlockCost * (_initPrice * 0.01f + 1f));

        return _objectPool;
    }

    public void GradeUp()
    {
        _level++;
        PaymentManager.Instance.Fund -= _upgradeCost;
        _plus = (int)Mathf.Ceil(_price * (_unlockCost * 0.002f) + _level * 0.08f);
        if (_level % 10 == 0)
        {
            _price = (int)(_price * 1.2f);
        }
        _price += _plus;
        _upgradeCost = (int) Mathf.Ceil(_upgradeCost * ((_unlockCost + _level) * 0.002f + 1f));
    }

    private void OnDestroy()
    {
        //すべて破棄
        _objectPool.Dispose();
    }
}