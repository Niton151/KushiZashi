using System.Collections;
using System.Collections.Generic;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _menuUI;
    void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Escape))
            .Subscribe(_ => _menuUI.SetActive(true))
            .AddTo(this);
    }
}
