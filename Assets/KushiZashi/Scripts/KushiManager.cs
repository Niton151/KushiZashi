using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class KushiManager : MonoBehaviour
{
    [NonSerialized] public bool isEmpty = true;
    public bool IsFull => _stockItems.Count == _maxLength;
    public IList<BaseItem> StockItems => _stockItems;

    [SerializeField] private ReactiveCollection<BaseItem> _stockItems = new ReactiveCollection<BaseItem>();
    [SerializeField] private int _maxLength;
    [SerializeField] private int _displayLength;

    private Material _skin;
    private Collider _collider;
    private CancellationToken _ct;

    private async void Start()
    {
        _collider = this.GetComponent<CapsuleCollider>();
        _ct = this.GetCancellationTokenOnDestroy();

        //非推奨
        /*
        this.UpdateAsObservable()
            .Select(_ => IsFull)
            .Distinct()
            .Subscribe(x => _collider.enabled = !x)
            .AddTo(this);
        */
        
        //串が満タンのときコライダーを無効化
        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => IsFull)
            .ForEachAsync(x => _collider.enabled = !x, _ct);

        var kushiCamera = transform.GetComponentInChildren<Camera>().gameObject;
        //串ビューモード
        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => Input.GetKey(KeyCode.E))
            .ForEachAsync(x =>
            {
                kushiCamera.SetActive(x);
            }, _ct);
    }
}
