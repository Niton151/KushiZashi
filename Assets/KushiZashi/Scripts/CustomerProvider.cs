using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Manager;
using UniRx;
using UnityEngine;

public class CustomerProvider : MonoBehaviour
{
    [SerializeField] private Customer _prefab;
    
    private void Start()
    {
        //InGameまで待つ
        GameManager.gameManager.GameState
            .Where(x => x == GameState.InGame)
            .Subscribe(_ => Init());
    }

    async void Init()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3f));

        var customer = Instantiate(_prefab, this.gameObject.transform);
        customer.Initialize(1, 30, (3, 5));
    }
}
