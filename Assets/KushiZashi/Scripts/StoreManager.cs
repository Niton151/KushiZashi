using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Manager;
using UniRx;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public IReadOnlyReactiveProperty<bool> IsOpen => _isOpen;
    
    [SerializeField] private int _openHour;
    [SerializeField] private int _openMinute;
    [SerializeField] private int _closeHour;
    [SerializeField] private int _closeMinute;

    private ReactiveProperty<bool> _isOpen = new ReactiveProperty<bool>();
    private CancellationToken _ct;

    private TimeManager _timeManager;

    public void FirstInit()
    {
        _ct = this.GetCancellationTokenOnDestroy();
        _timeManager = GameManager.gameManager.TimeManager;

        //開店
        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => _timeManager.NowTime)
            .Where(x => x.Hour == _openHour && x.Minute == _openMinute)
            .ForEachAsync(_ =>
            {
                _isOpen.Value = true;
            }, _ct);
        
        //閉店
        UniTaskAsyncEnumerable
            .EveryValueChanged(_timeManager, _ => _timeManager.NowTime)
            .Where(x => x.Hour == _closeHour && x.Minute == _closeMinute)
            .ForEachAsync(_ =>
            {
                _isOpen.Value = false;
            }, _ct);
            
    }
}
