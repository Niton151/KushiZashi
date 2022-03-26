using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject _template;
    [SerializeField] private Transform _parent;
    [SerializeField] private Button _newItemButton;
    [SerializeField] private TMP_Text _unlockCostText;
    [SerializeField] private GameObject _upgradeUI;

    private List<Button> _upgradeButtons = new List<Button>();
    private ItemGenerator _itemGenerator;
    private ItemManager _itemManager;
    private int _nextIndex = 0;

    public GameObject UpgradeUI => _upgradeUI;

    public void FirstInit()
    {
        _itemGenerator = GameManager.gameManager.ItemGenerator;
        _itemManager = GameManager.gameManager.ItemManager;

        Unlock(_itemManager.Items[_nextIndex]);

        //ボタンクリック時の処理(フィルタリングも)
        _newItemButton
            .OnClickAsObservable()
            .Select(_ => _itemManager.Items[_nextIndex])
            .Where(x => x.UnlockCost <= PaymentManager.Fund)
            .Subscribe(Unlock)
            .AddTo(this);
    }

    /// <summary>
    /// 食材アンロック時の処理
    /// </summary>
    /// <param name="itemProvider"></param>
    private void Unlock(ItemObjectPoolProvider itemProvider)
    {
        _itemGenerator.AddProvider(itemProvider);

        //UI生成
        var element = Instantiate(_template, _parent);
        var uiMediator = element.GetComponent<UpgradeUIMediator>();

        uiMediator.upgradeButton
            .OnClickAsObservable()
            .Where(_ => itemProvider.UpgradeCost <= PaymentManager.Fund)
            .Subscribe(_ =>
            {
                itemProvider.GradeUp();
                uiMediator.upgradeCost.text = itemProvider.UpgradeCost.ToString();
                uiMediator.price.text = itemProvider.Price.ToString();
            })
            .AddTo(this);

        PaymentManager.Fund -= itemProvider.UnlockCost;

        //UI初期化
        uiMediator.name.text = itemProvider.Name;
        uiMediator.image.sprite = itemProvider.Icon;
        uiMediator.price.text = itemProvider.Price.ToString();
        uiMediator.upgradeCost.text = itemProvider.UpgradeCost.ToString();

        //アンロックのテキストを書き換え
        try
        {
            _unlockCostText.text = _itemManager.Items[++_nextIndex].UnlockCost.ToString();
        }
        catch (ArgumentOutOfRangeException)
        {
            _newItemButton.gameObject.SetActive(false);
        }

        //追加ボタンを一番下に
        _newItemButton.transform.SetAsLastSibling();
    }
}