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

    private Customer _customer;
    private CancellationToken _ct;
    
    public void FirstInit()
    {
        _ct = this.GetCancellationTokenOnDestroy();
        _customer = Instantiate(_prefab, this.gameObject.transform);
    }

    public void OpenInit(CancellationToken ctOnClose)
    {
        _customer.Initialize(1, 10, lengthRange:(3, 5), ctOnClose);

        //客の入れ替え処理
        _customer.OnFinish
            .ToUniTaskAsyncEnumerable()
            .ForEachAsync(_ =>
            {
                Debug.Log("Customer init");
                var count = Random.Range(1, 4);
                var min = Random.Range(1, 3);
                var max = Random.Range(min, 8);
                var time = (int)(30 + (count - 1) * (max + min) * 0.5 * 5);
                _customer.Initialize(count, time, (min, max), ctOnClose);
            }, ctOnClose);
    }
}
