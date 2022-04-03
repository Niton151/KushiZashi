using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TMPro;
using UnityEngine;

public class PaymentManager : SingletonMonoBehaviour<PaymentManager>
{
    [SerializeField] private TMP_Text _fundText;
    [SerializeField] private TMP_Text _upgradeFundText;
    private int _fund = 110;

    public int Fund
    {
        get => _fund;
        set => _fund = value;
    }

    private CancellationToken _ct;

    public void FirstInit()
    {
        _ct = this.GetCancellationTokenOnDestroy();
        _fundText.text = "総資産:0円";
        _upgradeFundText.text = "総資産:0円";

        UniTaskAsyncEnumerable
            .EveryValueChanged(this, _ => _fund)
            .ForEachAsync(_ =>
            {
                _fundText.text = $"総資産:{_fund}円";
                _upgradeFundText.text = $"総資産:{_fund}円";
            }, _ct);
    }
}
