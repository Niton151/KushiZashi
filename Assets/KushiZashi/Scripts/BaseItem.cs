using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private Vector3 _initialVelocity;

    private readonly Subject<Unit> _finishedSubject = new Subject<Unit>();
    private Rigidbody _rigidbody;
    private KushiManager _kushi;
    
    //オブジェクトを使い終わったことを通知する
    public IObservable<Unit> OnFinishedAsync => _finishedSubject;

    private async void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
        
        this.OnTriggerEnterAsObservable()
            .Where(x => x.gameObject.TryGetComponent<KushiManager>(out _kushi))
            .Subscribe(_ => OnHit())
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => transform.position.y < 0f)
            .Subscribe(_ => Finish())
            .AddTo(this);
    }

    public void Initialize(Vector3 initPosition, Vector3 initRotation)
    {
        transform.position = initPosition;
        transform.Rotate(initRotation);

        Observable.NextFrame(FrameCountType.FixedUpdate)
            .TakeUntilDisable(this)
            .Subscribe(_ =>
            {
                _rigidbody.AddForce(_initialVelocity, ForceMode.VelocityChange);
            });
    }

    private async void OnHit()
    {
        this.transform.SetParent(_kushi.transform);
        _kushi.StockItems.Add(this);
        
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation
                                |RigidbodyConstraints.FreezePositionX
                                |RigidbodyConstraints.FreezePositionZ;
        
        transform.position = new Vector3(_kushi.transform.position.x, transform.position.y,
            _kushi.transform.position.z);
        
        if (_kushi.isEmpty)
        {
            await UniTask.WaitUntil(() => transform.position.y <= 5);
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _kushi.isEmpty = false;
        }
    }

    /// <summary>
    /// インスタンスを使い終わったときに実行する
    /// </summary>
    private void Finish()
    {
        //速度を初期化
        _rigidbody.velocity = Vector3.zero;
        
        //イベント発行
        _finishedSubject.OnNext(Unit.Default);
    }

    private void OnDestroy()
    {
        _finishedSubject.Dispose();
    }
}
