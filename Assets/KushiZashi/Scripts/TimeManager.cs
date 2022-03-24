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

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float _min1sec;
    [SerializeField] private TMP_Text _nowTimeText;
    [SerializeField] private TMP_Text _nowDayText;
    [SerializeField] private Light _sun;
    
    private DateTime _now = new DateTime(1, 1, 1, 9, 0, 0);
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
                _sun.transform.Rotate(_degree1sec * Time.deltaTime, 0, 0);
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
    }
}
