using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UniRx;
using UnityEngine;
using Progress = UnityEditor.Progress;

namespace Manager
{
    public enum GameState
    {
        Title,
        Initializing,
        InGame,
        Open,
        Close,
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
        public TimeManager TimeManager { get; private set; }
        public StoreManager StoreManager { get; private set; }
        public CustomerProvider CustomerProvider { get; private set; }

        private CancellationTokenSource _ctsOnClose;

        private CancellationToken _ct;

        private void Awake()
        {
            _ct = this.GetCancellationTokenOnDestroy();
            DontDestroyOnLoad(this.gameObject);
        }

        async void Start()
        {
            //GameState管理
            await GameState.ToUniTaskAsyncEnumerable().Skip(1).ForEachAwaitAsync(async _ =>
            {
                switch (GameState.Value)
                {
                    case Manager.GameState.Title:
                        Debug.Log("Title");
                        break;
                    case Manager.GameState.Initializing:
                        Debug.Log("Initializing");
                        await MainInitialize();
                        GameState.Value = Manager.GameState.InGame;
                        break;
                    case Manager.GameState.InGame:
                        Debug.Log("InGame");
                        await StoreManager.IsOpen.Where(x => x).ToUniTask(true, _ct);
                        GameState.Value = Manager.GameState.Open;
                        break;
                    case Manager.GameState.Open:
                        Debug.Log("Open");
                        await OpenInitialize();
                        await StoreManager.IsOpen.Where(x => !x).ToUniTask(true, _ct);
                        GameState.Value = Manager.GameState.Close;
                        break;
                    case Manager.GameState.Close:
                        Debug.Log("Close");
                        _ctsOnClose.Cancel();
                        await StoreManager.IsOpen.Where(x => x).ToUniTask(true, _ct);
                        GameState.Value = Manager.GameState.Open;
                        break;
                    case Manager.GameState.Menu:
                        Debug.Log("Menu");
                        break;
                    case Manager.GameState.Cleaning:
                        Debug.Log("End");
                        break;
                    case Manager.GameState.Result:
                        Debug.Log("Result");
                        break;
                }
            }, cancellationToken: _ct);
        }

        /// <summary>
        /// MainSceneの初期化処理
        /// </summary>
        /// <returns></returns>
        private async UniTask MainInitialize()
        {
            //ここに初期化処理を書く
            ItemGenerator = GameObject.Find(Const.ItemGenerator).GetComponent<ItemGenerator>();
            ItemGenerator.FirstInit();

            Kushi = GameObject.Find(Const.Kushi).GetComponent<KushiManager>();
            Kushi.FirstInit();

            PaymentManager = GameObject.Find(Const.PaymentManager).GetComponent<PaymentManager>();
            PaymentManager.FirstInit();

            TimeManager = GameObject.Find(Const.TimeManager).GetComponent<TimeManager>();
            TimeManager.FirstInit();

            StoreManager = GameObject.Find(Const.StoreManager).GetComponent<StoreManager>();
            StoreManager.FirstInit();

            CustomerProvider = GameObject.Find(Const.CustomerProvider).GetComponent<CustomerProvider>();
            CustomerProvider.FirstInit();
        }

        private async UniTask OpenInitialize()
        {
            _ctsOnClose = new CancellationTokenSource();

            ItemGenerator.OpenInit(_ctsOnClose.Token);
            Kushi.OpenInit(_ctsOnClose.Token);
            CustomerProvider.OpenInit(_ctsOnClose.Token);
        }
    }
}