using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Manager;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerProvider : MonoBehaviour
{
    [SerializeField] private Customer _prefab;

    private CancellationToken _ct;
    
    private void Start()
    {
        _ct = this.GetCancellationTokenOnDestroy();
        
        //InGameまで待つ
        GameManager.gameManager.GameState
            .Where(x => x == GameState.InGame)
            .Subscribe(_ => Init())
            .AddTo(this);
    }

    async void Init()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: _ct);

        var customer = Instantiate(_prefab, this.gameObject.transform);
        customer.Initialize(1, 30, lengthRange:(3, 5));

        //客の入れ替え処理
        customer.OnFinish
            .ToUniTaskAsyncEnumerable()
            .ForEachAsync(_ =>
            {
                Debug.Log("Customer init");
                var count = Random.Range(1, 4);
                var min = Random.Range(1, 3);
                var max = Random.Range(min, 8);
                var time = (int)(30 + (count - 1) * (max + min) * 0.5 * 5);
                customer.Initialize(count, time, (min, max));
            }, _ct);
    }
}
