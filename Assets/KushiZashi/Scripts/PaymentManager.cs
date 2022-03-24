using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaymentManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _fundText;
    private int _fund;

    public void FirstInit()
    {
        _fundText.text = "総資産:0¥";
    }

    public void AddMoney(int payment)
    {
        _fund += payment;
        _fundText.text = $"総資産:{_fund}¥";
    }
}
