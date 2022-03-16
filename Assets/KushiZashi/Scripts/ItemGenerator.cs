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
    [SerializeField] private List<ItemObjectPoolProvider> _itemObjectPoolProviders = new List<ItemObjectPoolProvider>();

    [SerializeField] private float _radius = 3f;

    [SerializeField] private float _minSpan;

    [SerializeField] private float _maxSpan;

    private GameManager _gameManager;

    private CancellationToken _ct;

    private List<ItemObjectPool> _itemObjectPools = new List<ItemObjectPool>();
    
    void Awake()
    {
        _gameManager = GameObject.Find(Const.GameManager).GetComponent<GameManager>();
        _ct = this.GetCancellationTokenOnDestroy();

        for (int i = 0; i < _itemObjectPoolProviders.Count; i++)
        {
            _itemObjectPools.Add(_itemObjectPoolProviders[i].Get());
        }
    }

    async void Start()
    {
        var span = Random.Range(_minSpan, _maxSpan);
        await UniTaskAsyncEnumerable
            .Interval(TimeSpan.FromSeconds(span))
            .ForEachAsync(_ =>
            {
                Generate();
                span = Random.Range(_minSpan, _maxSpan);
            }, cancellationToken:_ct);
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
            });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
