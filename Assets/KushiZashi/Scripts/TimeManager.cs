using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    [SerializeField] private float _min1sec;
    [SerializeField] private TMP_Text _nowTimeText;
    [SerializeField] private TMP_Text _nowDayText;
    [SerializeField] private Light _sun;
    
    [SerializeField] private Material _asaToHiru;
    [SerializeField] private Material _hiruToYuu;
    [SerializeField] private Material _yuuToYoru;
    [SerializeField] private Material _yoruToAsa;

    [SerializeField] private int _asa;
    [SerializeField] private int _hiru;
    [SerializeField] private int _yuu;
    [SerializeField] private int _yoru;
    
    private DateTime _now = new DateTime(1, 1, 1, 9, 50, 0);
    private CancellationToken _ct;

    private float _degree1sec;
    
    public DateTime NowTime => _now;

    public void FirstInit()
    {
        _ct = this.GetCancellationTokenOnDestroy();
        //計算した結果
        _degree1sec = 0.25f * _min1sec;

        //時間を動かす
        UniTaskAsyncEnumerable
            .EveryUpdate()
            .ForEachAsync(_ =>
            {
                //_sun.transform.Rotate(_degree1sec * Time.deltaTime, 0, 0);
                _now = _now.AddMinutes(_min1sec * Time.deltaTime);
                _nowTimeText.text = _now.ToString("t");
            }, _ct);

        //日にち表示
        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => _now.Day)
            .ForEachAsync(_ =>
            {
                _nowDayText.text = (_now.Day - DateTime.MinValue.Day + 1).ToString() + "日目";
            }, _ct);
        
        //skyboxを操作(朝スタートのみ)
        UniTaskAsyncEnumerable
            .EveryUpdate()
            .ForEachAwaitAsync(async _ =>
            {
                await UniTask.WaitUntil(() => _now.Hour == _asa, cancellationToken: _ct);
                RenderSettings.skybox = _asaToHiru;
                await Blend(_asa, _hiru, _asaToHiru);

                await UniTask.WaitUntil(() => _now.Hour == _hiru, cancellationToken: _ct);
                RenderSettings.skybox = _hiruToYuu;
                await Blend(_hiru, _yuu, _hiruToYuu);
                
                await UniTask.WaitUntil(() => _now.Hour == _yuu, cancellationToken: _ct);
                RenderSettings.skybox = _yuuToYoru;
                await Blend(_yuu, _yoru, _yuuToYoru);
                
                await UniTask.WaitUntil(() => _now.Hour == _yoru, cancellationToken: _ct);
                RenderSettings.skybox = _yoruToAsa;
                await Blend(_yoru, _asa + 24, _yoruToAsa);
            }, _ct);
    }

    public async void Accelerate(float min1sec, DateTime until)
    {
        var temp = _min1sec;
        _min1sec = min1sec;

        SoundManager.Instance.Audio.PlayOneShot(SoundManager.Instance.timeQuick);
        await UniTask.WaitUntil(() => _now >= until, cancellationToken:_ct);

        _min1sec = temp;
        _degree1sec = 0.25f * _min1sec;
    }

    /// <summary>
    /// skyboxを徐々にblendしていく
    /// </summary>
    /// <param name="from"></param>
    /// <param name="until"></param>
    /// <param name="skybox"></param>
    private async UniTask Blend(int from, int until, Material skybox)
    {
        double blend = 0;
        await UniTaskAsyncEnumerable
            .EveryUpdate()
            .TakeWhile(_ => blend < 1)
            .ForEachAsync(_ =>
            {
                var rate = (double)((Time.deltaTime * _min1sec) / (60 * (until - from)));
                blend += rate;
                skybox.SetFloat("_Blend", (float)blend);
            }, _ct);
    }
}
