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

    private void Start()
    {
        _startButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManager.LoadScene(Const.MainScene);
                GameManager.gameManager.GameState.Value = GameState.Initializing;
            }).AddTo(this);
    }
}
