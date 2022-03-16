using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Manager;

public class KushiMove : MonoBehaviour
{
    private GameManager _gameManager;
    
    void Awake()
    {
        _gameManager = GameObject.Find(Const.GameManager).GetComponent<GameManager>();
        Initialize();
    }
    
    void Initialize()
    {
        //カーソルの移動を画面内に制限
        Cursor.lockState = CursorLockMode.Confined;
        
        //串の座標をマウスの位置と同期
        this.UpdateAsObservable()
            .Select(_ => Input.mousePosition)
            .Subscribe(v =>
            {
                v.z = 11f;  //z座標は適当にセット
                var position = Camera.main.ScreenToWorldPoint(v);
                gameObject.transform.position = position;
            });
    }
    void Update()
    {
    }
}
