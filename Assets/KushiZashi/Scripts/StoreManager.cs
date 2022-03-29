using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Manager;
using UniRx;
using UnityEngine;

public class StoreManager : SingletonMonoBehaviour<StoreManager>
{
    public IReadOnlyReactiveProperty<bool> IsOpen => _isOpen;
    
    [SerializeField] private int _openHour;
    [SerializeField] private int _openMinute;
    [SerializeField] private int _closeHour;
    [SerializeField] private int _closeMinute;
    [SerializeField] private float _accelerate;

    private ReactiveProperty<bool> _isOpen = new ReactiveProperty<bool>();
    private CancellationToken _ct;
    
    private SoundManager _se;

    public void FirstInit()
    {
        _ct = this.GetCancellationTokenOnDestroy();
        _se = SoundManager.Instance;

        //開店
        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => TimeManager.Instance.NowTime)
            .Where(x => x.Hour == _openHour && x.Minute == _openMinute && _isOpen.Value == false)
            .ForEachAsync(_ =>
            {
                _se.Audio.PlayOneShot(_se.clock);
                _isOpen.Value = true;
            }, _ct);
        
        //閉店
        UniTaskAsyncEnumerable
            .EveryValueChanged(TimeManager.Instance, _ => TimeManager.Instance.NowTime)
            .Where(x => x.Hour == _closeHour && x.Minute == _closeMinute && _isOpen.Value == true)
            .ForEachAsync(_ =>
            {
                _se.Audio.PlayOneShot(_se.clock);
                _isOpen.Value = false;
            }, _ct);

        
        UniTaskAsyncEnumerable
            .EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Where(_ => !_isOpen.Value)
            .ForEachAsync(_ =>
            {
                TimeManager.Instance.Accelerate(_accelerate, new TimeSpan(_openHour, _openMinute - 30, 0));
            }, _ct);

    }
}
