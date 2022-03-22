using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UniRx;
using UnityEngine;

namespace Manager
{
    public enum GameState
    {
        Title,
        Initializing,
        InGame,
        Menu,
        Cleaning,
        Result
    }

    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public static GameManager gameManager => Instance;
        public ReactiveProperty<GameState> GameState = new ReactiveProperty<GameState>(Manager.GameState.Title);
        public ItemGenerator ItemGenerator { get; private set; }
        public KushiManager Kushi { get; private set; }
        public PaymentManager PaymentManager { get; private set; }

        private CancellationToken _ct;

        private void Awake()
        {
            _ct = this.GetCancellationTokenOnDestroy();
            DontDestroyOnLoad(this.gameObject);
        }

        async void Start()
        {
            //GameState管理
            await GameState.ToUniTaskAsyncEnumerable().ForEachAwaitAsync(async _ =>
            {
                switch (GameState.Value)
                {
                    case Manager.GameState.Title :
                        Debug.Log("Title");
                        break;
                    case Manager.GameState.Initializing :
                        Debug.Log("Initializing");
                        await MainInitialize();
                        GameState.Value = Manager.GameState.InGame;
                        break;
                    case Manager.GameState.InGame :
                        Debug.Log("InGame");
                        break;
                    case Manager.GameState.Menu :
                        Debug.Log("Menu");
                        break;
                    case Manager.GameState.Cleaning :
                        Debug.Log("End");
                        break;
                    case Manager.GameState.Result :
                        Debug.Log("Result");
                        break;
                }
            }, cancellationToken:_ct);
        }

        /// <summary>
        /// MainSceneの初期化処理
        /// </summary>
        /// <returns></returns>
        private async UniTask MainInitialize()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _ct);
            //ここに初期化処理を書く
            ItemGenerator = GameObject.Find(Const.ItemGenerator).GetComponent<ItemGenerator>();
            Kushi = GameObject.Find(Const.Kushi).GetComponent<KushiManager>();
            PaymentManager = GameObject.Find(Const.PaymentManager).GetComponent<PaymentManager>();
        }
    }
}
