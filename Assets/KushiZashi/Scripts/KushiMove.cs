using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Manager;

public class KushiMove : MonoBehaviour
{
    private Camera _camera;

    void Awake()
    {
        _camera = Camera.main;
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
                var position = _camera.ScreenToWorldPoint(v);
                gameObject.transform.position = position;
            })
            .AddTo(this);
    }
}
