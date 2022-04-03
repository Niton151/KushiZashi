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

public class UpgradeManager : SingletonMonoBehaviour<UpgradeManager>
{
    [SerializeField] private GameObject _template;
    [SerializeField] private Transform _parent;
    [SerializeField] private Button _newItemButton;
    [SerializeField] private TMP_Text _unlockCostText;
    [SerializeField] private GameObject _upgradeUI;

    private List<Button> _upgradeButtons = new List<Button>();
    private ItemGenerator _itemGenerator;
    private ItemManager _itemManager;
    private SoundManager _se;
    private int _nextIndex = 0;

    public GameObject UpgradeUI => _upgradeUI;

    public void FirstInit()
    {
        _itemGenerator = GameManager.gameManager.ItemGenerator;
        _itemManager = ItemManager.Instance;
        _se = SoundManager.Instance;
        //_upgradeUI.SetActive(false);

        Unlock(_itemManager.Items[_nextIndex]);

        //ボタンクリック時の処理(フィルタリングも)
        _newItemButton
            .OnClickAsObservable()
            .Select(_ => _itemManager.Items[_nextIndex])
            .Where(x => x.UnlockCost <= PaymentManager.Instance.Fund)
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
        _se.Audio.PlayOneShot(_se.unlock);

        //UI生成
        var element = Instantiate(_template, _parent);
        var uiMediator = element.GetComponent<UpgradeUIMediator>();

        uiMediator.upgradeButton
            .OnClickAsObservable()
            .Where(_ => itemProvider.UpgradeCost <= PaymentManager.Instance.Fund)
            .Subscribe(_ =>
            {
                itemProvider.GradeUp();
                uiMediator.price.text = itemProvider.Price + "円";
                uiMediator.upgradeCost.text = itemProvider.UpgradeCost + "円";
                uiMediator.level.text = itemProvider.Level.ToString();
                uiMediator.plus.text = itemProvider.Plus + "円"; 
                
                _se.Audio.PlayOneShot(_se.upgrade);
            })
            .AddTo(this);

        PaymentManager.Instance.Fund -= itemProvider.UnlockCost;

        //UI初期化
        uiMediator.name.text = itemProvider.Name;
        uiMediator.image.sprite = itemProvider.Icon;
        uiMediator.price.text = itemProvider.Price + "円";
        uiMediator.upgradeCost.text = itemProvider.UpgradeCost + "円";
        uiMediator.description.text = itemProvider.Description;
        uiMediator.level.text = itemProvider.Level.ToString();
        uiMediator.plus.text = itemProvider.Plus + "円"; 

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