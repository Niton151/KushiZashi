using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Pointer : MonoBehaviour
{
    [SerializeField] private Transform _origin;
    [SerializeField] private float _maxDistance;
    [SerializeField] private GameObject _marker;

    private LineRenderer _line;

    private void Start()
    {
        _line = GetComponent<LineRenderer>();
        var hitInfo = new RaycastHit();

        //マーカー生成
        var marker = Instantiate(
            original: _marker,
            position: transform.position,
            rotation: Quaternion.identity,
            parent: transform);
        marker.SetActive(false);
        
        //LineRenderer初期化
        _line.SetPositions(new []{Vector3.zero, Vector3.forward * _maxDistance});
        
        //Ray照射
        this.UpdateAsObservable()
            .Select(_ => Physics.Raycast(_origin.position, _origin.forward, out hitInfo, _maxDistance))
            .Subscribe(x =>
            {
                marker.SetActive(x);
                marker.transform.position = hitInfo.point;
                marker.transform.LookAt(this.transform);
            })
            .AddTo(this);
    }

    private void Update()
    {
        Debug.DrawRay(_origin.position, _origin.forward * _maxDistance, Color.blue);
    }
}
