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
    [SerializeField] private GameObject _titleGamemanager;

    public static bool isLoadedOnce = false;
    public static GameObject gameManager;

    private void Start()
    {
        if (isLoadedOnce == false)
        {
            gameManager = Instantiate(_titleGamemanager);
            isLoadedOnce = true;
        }

        gameManager.GetComponent<GameManager>().GameState.Value = GameState.Title;
        _startButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManager.LoadScene(Const.MainScene);
                GameManager.gameManager.GameState.Value = GameState.Initializing;
            }).AddTo(this);
    }
}
