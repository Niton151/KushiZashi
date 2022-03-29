using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using Manager;
using UniRx;
using Random = UnityEngine.Random;

public class ItemGenerator : MonoBehaviour
{
    public List<ItemObjectPoolProvider> ItemObjectPoolProviders => _itemObjectPoolProviders;
    
    [SerializeField] private List<ItemObjectPoolProvider> _itemObjectPoolProviders = new List<ItemObjectPoolProvider>();

    [SerializeField] private float _radius = 3f;

    [SerializeField] private float _minSpan;

    [SerializeField] private float _maxSpan;

    private CancellationToken _ct;

    private List<ItemObjectPool> _itemObjectPools = new List<ItemObjectPool>();
    
    public void FirstInit()
    {
        _ct = this.GetCancellationTokenOnDestroy();
    }

    public void OpenInit(CancellationToken ctOnClose)
    {
        var span = Random.Range(_minSpan, _maxSpan);
        
        //ランダムな間隔でアイテム生成
        UniTaskAsyncEnumerable
            .Interval(TimeSpan.FromSeconds(span))
            .ForEachAsync(_ =>
            {
                Generate();
                span = Random.Range(_minSpan, _maxSpan);
            }, cancellationToken:ctOnClose);
    }

    /// <summary>
    /// アイテム生成処理
    /// </summary>
    private void Generate()
    {
        //ランダムな位置取得
        var tempVec2 = Random.insideUnitCircle * _radius;
        var initPos = new Vector3(tempVec2.x, 0, tempVec2.y) + transform.position;
        //ランダムな回転量取得
        var rotValue = Random.Range(0, 360);
        var initRot = new Vector3(rotValue, rotValue, rotValue);
        

        //ランダムなアイテムを生成
        var randomIndex = Random.Range(0, _itemObjectPoolProviders.Count);
        var item = _itemObjectPools[randomIndex].Rent();
        
        item.Initialize(initPos, initRot);

        item.OnFinishedAsync
            .Take(1)
            .Subscribe(_ =>
            {
                _itemObjectPools[randomIndex].Return(item);
            })
            .AddTo(this);
    }

    public void AddProvider(ItemObjectPoolProvider provider)
    {
        _itemObjectPoolProviders.Add(provider);
        _itemObjectPools.Add(provider.Get());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
