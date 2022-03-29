using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Manager;
using TMPro;
using UnityEngine;

public class Instructor : SingletonMonoBehaviour<Instructor>
{
    [SerializeField] private TMP_Text _text;

    [SerializeField] private List<GameObject> _slides = new List<GameObject>();

    private KushiManager _kushiManager;
    private CancellationToken _ct;

    public async void Begin()
    {
        _kushiManager = GameManager.Instance.Kushi;
        _ct = this.GetCancellationTokenOnDestroy();

        _text.text = "串を動かして落ちてくる食材を刺してみよう!";
        _slides[0].SetActive(true);
        await UniTask.WaitUntil(() => _kushiManager.StockItems.Count == 1, cancellationToken: _ct);

        _text.text = "注文と同じように食材を刺していこう!\n(間違えたらCキーでやり直せるよ)";
        await UniTask.WaitUntil(() =>
                CustomerProvider.Instance.Customer.FirstMenuCount == _kushiManager.StockItems.Count,
            cancellationToken: _ct);

        _text.text = "左の網で食材を焼こう!";
        _slides[1].SetActive(true);
        await UniTask.WaitUntil(() => _kushiManager.StockItems.Select(x => x.IsCooked).All(x => x),
            cancellationToken: _ct);

        _text.text = "お客さんに提供しよう!";
        _slides[2].SetActive(true);
        await CustomerProvider.Instance.Customer.OnFinish.ToUniTask(true, _ct);
        _text.text = "閉店時間(23時)までがんばろう！";

        await UniTask.WaitUntil(() => GameManager.Instance.GameState.Value == GameState.Close, cancellationToken: _ct);
        _slides[3].SetActive(true);

        await UniTask.WaitUntil(() => _slides[3].activeSelf == false, cancellationToken: _ct);
        _slides[4].SetActive(true);
        _text.text = "総資金を10万円にする";

        await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _ct);
        _slides[4].SetActive(false);

        await UniTask.WaitUntil(() => PaymentManager.Instance.Fund >= 100000, cancellationToken: _ct);
        _slides[5].SetActive(true);
        _text.text = String.Empty;
    }
}