using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TMPro;
using UnityEngine;

public class PaymentManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _fundText;
    private static int _fund = 10000;

    public static int Fund
    {
        get => _fund;
        set => _fund = value;
    }

    private CancellationToken _ct;

    public void FirstInit()
    {
        _ct = this.GetCancellationTokenOnDestroy();
        _fundText.text = "総資産:0¥";

        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => _fund)
            .ForEachAsync(_ => _fundText.text = $"総資産:{_fund}¥", _ct);
    }
}
