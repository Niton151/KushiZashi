using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameObject.Find(Const.GameManager).GetComponent<GameManager>();
    }

    private void Start()
    {
        _startButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                _gameManager.GameState.Value = GameState.Initializing;
                SceneManager.LoadScene(Const.MainScene);
            }).AddTo(this);
    }
}
