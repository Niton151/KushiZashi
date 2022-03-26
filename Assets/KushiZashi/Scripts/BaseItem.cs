using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class BaseItem : MonoBehaviour
{
    private readonly Subject<Unit> _finishedSubject = new Subject<Unit>();
    private Rigidbody _rigidbody;
    private KushiManager _kushi;
    private IDisposable _cookDisposable;
    private IDisposable _kushiDisposable;
    private Renderer _renderer;
    private bool _isCooked = false;
    private Transform _root;
    private CancellationToken _ct;

    //オブジェクトを使い終わったことを通知する
    public IObservable<Unit> OnFinishedAsync => _finishedSubject;

    public bool IsCooked
    {
        get => _isCooked;
        set => _isCooked = value;
    }
    
    public Transform Root => _root;
    public ItemObjectPoolProvider Provider;

    private void Start()
    {
        _kushi = GameManager.gameManager.Kushi;
        _root = transform.parent;
        _ct = this.GetCancellationTokenOnDestroy();
    }

    /// <summary>
    /// Instantiate時とObjectPoolからのRent時の初期化処理
    /// </summary>
    /// <param name="initPosition"></param>
    /// <param name="initRotation"></param>
    public void Initialize(Vector3 initPosition, Vector3 initRotation)
    {
        transform.position = initPosition;
        transform.Rotate(initRotation);

        //初速度をかける
        _rigidbody = this.GetComponent<Rigidbody>();
        Observable.NextFrame(FrameCountType.FixedUpdate)
            .TakeUntilDisable(this)
            .Subscribe(_ =>
            {
                _rigidbody.AddForce(Provider.InitialVelocity, ForceMode.VelocityChange);
            });

        //串に衝突するイベント
        _kushiDisposable = this.OnTriggerEnterAsObservable()
            .Where(x => x.gameObject.CompareTag("Kushi"))
            .ThrottleFrame(1)
            .Subscribe(_ => OnHit())
            .AddTo(this);
        
        //串をずらすイベント
        this.OnTriggerStayAsObservable()
            .Where(x => x.gameObject.CompareTag("Kushi"))
            .Where(_ => !_kushi.isEmpty)
            .Subscribe(_ => _kushi.StockItems[0].transform.position += Vector3.down * Time.deltaTime)
            .AddTo(this);

        //焼くイベント
        _renderer = this.GetComponent<Renderer>();
        _cookDisposable = this.OnTriggerEnterAsObservable()
            .Where(x => x.CompareTag("Fire"))
            .SelectMany(_ => Observable.Interval(TimeSpan.FromSeconds(Provider.CookTime)))
            .TakeUntil(this.OnTriggerExitAsObservable())
            .RepeatUntilDestroy(this.gameObject)
            .Subscribe(_ => OnCook())
            .AddTo(this);

        //刺さらずに下に落ちたやつのイベント
        this.UpdateAsObservable()
            .Where(_ => transform.position.y < -20f)
            .Subscribe(_ => Finish())
            .AddTo(this);
    }

    /// <summary>
    /// 串との衝突時の処理
    /// </summary>
    private async void OnHit()
    {
        _kushiDisposable.Dispose();
        //いろいろなところに追加
        this.transform.SetParent(_kushi.transform);
        _kushi.StockItems.Add(this);
        
        //動きを制限
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation
                                |RigidbodyConstraints.FreezePositionX
                                |RigidbodyConstraints.FreezePositionZ;
        
        //位置調整
        var position = _kushi.transform.position;
        transform.position = new Vector3(position.x, transform.position.y,
            position.z);
        
        //空のときは串の下の方で止める
        if (_kushi.isEmpty)
        {
            await UniTask.WaitUntil(() => transform.position.y <= 8, cancellationToken: _ct);
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _kushi.isEmpty = false;
        }
    }

    /// <summary>
    /// 火を通すときの処理
    /// </summary>
    private void OnCook()
    {
        _renderer.material = Provider.CookedMat;
        _isCooked = true;
        
        _cookDisposable.Dispose();
        Debug.Log("cooked");
    }

    /// <summary>
    /// インスタンスを使い終わったときに実行する
    /// </summary>
    public void Finish()
    {
        //Observable, UniTask破棄
        _kushiDisposable.Dispose();
        _cookDisposable.Dispose();
        
        //速度を初期化
        _rigidbody.velocity = Vector3.zero;
        
        //移動制限を初期化
        _rigidbody.constraints = RigidbodyConstraints.None;
        
        //マテリアルを初期化
        _renderer.material = Provider.DefaultMat;
        
        //イベント発行
        _finishedSubject.OnNext(Unit.Default);
    }

    private void OnDestroy()
    {
        _finishedSubject.Dispose();
    }
    
    //objと自分自身が等価のときはtrueを返す
    public override bool Equals(object obj)
    {
        //objがnullか、型が違うときは、等価でない
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        //Numberで比較する
        BaseItem c = (BaseItem)obj;
        return (this.Provider.Name == c.Provider.Name && this._isCooked == c._isCooked);
    }

    //Equalsがtrueを返すときに同じ値を返す
    public override int GetHashCode()
    {
        return this.Provider.Name.GetHashCode() ^ this._isCooked.GetHashCode();
    }
}
