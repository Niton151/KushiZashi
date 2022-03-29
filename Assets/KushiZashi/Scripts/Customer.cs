using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Customer : MonoBehaviour
{
    private int _orderCount;
    private float _timeLimit;
    private (int min, int max) _lengthRange;
    [SerializeField] private List<Menu> _menus = new List<Menu>();

    private float timer = 0;
    private CancellationTokenSource _cts;
    private IDisposable _disposable;

    private List<Button> _panels;
    private List<List<Image>> _images = new List<List<Image>>();
    private Slider _timeSlider;

    [SerializeField] private Sprite _defaultIcon;

    private KushiManager _kushi;
    private SoundManager _se;

    public Subject<Unit> OnFinish => _onFinishSubject;
    private Subject<Unit> _onFinishSubject = new Subject<Unit>();
    
    //チュートリアル用
    public int FirstMenuCount => _menus[0].MenuList.Count;
    

    private void Awake()
    {
        _kushi = GameManager.gameManager.Kushi;
        _se = SoundManager.Instance;

        //注文内容を表示するためののImageを取得
        var parent = GameObject.Find("OrderBack");
        _panels = parent.GetComponentsInChildren<Button>().ToList();
        foreach (var t in _panels)
        {
            _images.Add(t.GetComponentsInChildren<Image>().Skip(1).ToList());
        }

        //タイマーのSlider初期化
        _timeSlider = parent.GetComponentInChildren<Slider>();
    }

    /// <summary>
    /// Instantiate時と新しい客に入れ替わるときの処理
    /// </summary>
    /// <param name="orderCount">注文数</param>
    /// <param name="timeLimit">制限時間</param>
    /// <param name="lengthRange">要求食材の数(範囲)</param>
    public async void Initialize(int orderCount, float timeLimit, (int min, int max) lengthRange, CancellationToken ctOnClose)
    {
        _orderCount = orderCount;
        _timeLimit = timeLimit;
        _lengthRange = lengthRange;
        _timeSlider.maxValue = _timeLimit;

        _cts = new CancellationTokenSource();
        var (cancelUniTask, _) = ctOnClose.ToUniTask();

        //Image初期化
        _images
            .SelectMany(panel => panel.Select(x => x))
            .ToList()
            .ForEach(x => x.sprite = _defaultIcon);

        //注文数文
        for (int i = 0; i < _orderCount; i++)
        {
            //注文生成
            var randLen = Random.Range(_lengthRange.min, _lengthRange.max);
            var available = GameManager.gameManager.ItemGenerator.ItemObjectPoolProviders
                .Select(x => x.Item)
                .ToList();
            _menus.Add(new Menu(randLen, available));
        }

        OrderDisplay();

        //時間制限
        _disposable = this.UpdateAsObservable()
            .Where(_ => timer <= _timeLimit)
            .Subscribe(_ =>
            {
                timer += Time.deltaTime;
                _timeSlider.value = _timeLimit - timer;
            });

        //完成するか制限時間になるかを待つ
        try
        {
            await UniTask.WhenAny(
                UniTask.WhenAll(
                    UniTask.Create(async () => await Compare(0)),
                    UniTask.Create(async () => await Compare(1)),
                    UniTask.Create(async () => await Compare(2)),
                    UniTask.Create(async () => await Compare(3))
                ),
                UniTask.WaitUntil(() => timer >= _timeLimit, cancellationToken: _cts.Token),
                cancelUniTask
            );
        }
        catch
        {
            
        }
        Exit(ctOnClose.IsCancellationRequested);
    }

    /// <summary>
    /// 串と注文を比較
    /// </summary>
    /// <param name="i">panelIndex</param>
    /// <returns></returns>
    private async UniTask Compare(int i)
    {
        while (true)
        {
            if (i < _orderCount)
            {
                await _panels[i].OnClickAsync(_cts.Token);
                if (_menus[i].MenuList.SequenceEqual(_kushi.StockItems))
                {
                    //icon削除
                    _images[i].ForEach(x => x.sprite = _defaultIcon);
                    //串をclear
                    _kushi.AllClear();
                    _se.Audio.PlayOneShot(_se.match);
                    break;
                }
                _se.Audio.PlayOneShot(_se.wrong);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 注文をImage表示
    /// </summary>
    private void OrderDisplay()
    {
        for (int i = 0; i < _orderCount; i++)
        {
            var count = _menus[i].MenuList.Count;
            for (int j = 0; j < count; j++)
            {
                var item = _menus[i].MenuList[j];
                _images[i][count - j - 1].sprite = item.Provider.Icon;
            }
        }
    }

    /// <summary>
    /// 客入れ替え時の処理
    /// </summary>
    private void Exit(bool isCancelled)
    {
        //成功時
        if (timer < _timeLimit && !isCancelled)
        {
            _kushi.AllClear();
            PaymentManager.Instance.Fund += _menus.Select(x => x.SumPrice).Sum();
            _se.Audio.PlayOneShot(_se.money);
        }
        else if(timer >= _timeLimit)
        {
            _se.Audio.PlayOneShot(_se.timeup);
        }

        _images?.ForEach(x => x.ForEach(y => y.sprite = _defaultIcon));
        
        _disposable?.Dispose();
        _cts?.Cancel();
        _cts?.Dispose();
        timer = 0;
        _menus.Clear();
        Debug.Log("exit");
        _onFinishSubject.OnNext(Unit.Default);
    }

    private void OnDestroy()
    {
        _disposable?.Dispose();
        _cts?.Cancel();
        _cts?.Dispose();
    }
}