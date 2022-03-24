using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Manager;
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
    private GameObject _kushiCamera;

    public void FirstInit()
    {
        _collider = this.GetComponent<CapsuleCollider>();
        _kushiCamera = transform.GetComponentInChildren<Camera>().gameObject;
        _ct = this.GetCancellationTokenOnDestroy();   
        
        //串が満タンのときコライダーを無効化
        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => IsFull)
            .ForEachAsync(x => _collider.enabled = !x, _ct);
        
        this.gameObject.SetActive(false);
    }

    public async void OpenInit(CancellationToken ctOnClose)
    {
        gameObject.SetActive(true);
        //串ビューモード
        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => Input.GetKey(KeyCode.E))
            .ForEachAsync(x =>
            {
                _kushiCamera.SetActive(x);
            }, ctOnClose);

        await GameManager.gameManager.GameState
            .Where(x => x != GameState.Open)
            .ToUniTask(true, _ct);
        gameObject.SetActive(false);
    }
}
