using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class KushiManager : MonoBehaviour
{
    [NonSerialized] public bool isEmpty = true;
    public bool IsFull => _stockItems.Count == _maxLength;
    public List<BaseItem> StockItems => _stockItems;

    [SerializeField] private List<BaseItem> _stockItems = new List<BaseItem>();
    [SerializeField] private int _maxLength;

    private Material _skin;
    private Collider _collider;

    private async void Start()
    {
        _collider = this.GetComponent<CapsuleCollider>();

        this.UpdateAsObservable()
            .Select(_ => IsFull)
            .Distinct()
            .Subscribe(x => _collider.enabled = !x);
    }
}
