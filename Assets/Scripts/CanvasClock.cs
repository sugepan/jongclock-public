using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class CanvasClock : MonoBehaviour
{
    public GameObject canvasClock;
    public GameObject canvasConfig;
    public AudioClip dahaiSE;   // 打牌の効果音
    public AudioClip nakiSE;    // 鳴きの効果音
    public AudioClip agariSE;   // 和了の効果音
    public AudioClip timeUpSE;  // 時間切れの効果音
    public AudioClip startSE;   // 対局開始の効果音
    public AudioClip[] cdGirlSE = new AudioClip[11];  // 元気な女の子のカウントダウン

    public enum MODE   // モード定義
    {
        READY,                  // 開始前
        PLAY,                   // プレイ中
        AGARI,                  // 和了
        RYUKYOKU,               // 流局
        TIMEUP,                 // 時間切れ
        PAUSE,                  // 一時停止
    };
    public MODE mode = MODE.READY;

    enum STATUS     // 状態定義
    {
        START,
        DAHAI,
        CHII,
        PONKAN,
    };

    // 考慮時間
    public float considerTime;

    // フィッシャールール
    public bool isFischer;
    public float fischerSec;  // 1手で増える秒数

    // 最大打牌回数
    const int MAX_DAHAI_CNT = 70;
    int dahaiCnt = 0;

    private AudioSource audioSource;    // オーディオソース

    private int[] cdFlg = new int[] { 10, 10, 10, 10 };  // カウントダウンフラグ

    public static int playersNum;     // プレイヤー人数

    public int turnPlayer;     // 誰の番か

    public float[] elapsedTime = new float[4]; // 残り時間
    List<float>[] elapsedTimeHistory = new List<float>[4];      // 残り時間の履歴
    public System.Diagnostics.Stopwatch[] stopwatch = new System.Diagnostics.Stopwatch[4];  // ストップウォッチ
    bool[] boolBtnNaki = new bool[] {true, true, true, true};
    bool[] boolBtnAgari = new bool[] { true, true, true, true};

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();

        // プレイヤー人数
        playersNum = PlayerPrefs.GetInt(CanvasConfig.KEY_PLAYERS_NUM, 4);

        for (int i = 0; i < 4; i++)
        {
            // ストップウォッチ 初期化
            stopwatch[i] = new System.Diagnostics.Stopwatch();

            // 残り時間 初期化
            elapsedTime[i] = PlayerPrefs.GetInt(CanvasConfig.KEY_ELAPSED_MIN_ + i, CanvasConfig.DEF_ELAPSED_MIN) * 60f;
            // 考慮時間 初期化
            considerTime = PlayerPrefs.GetInt(CanvasConfig.KEY_CONSIDER_SEC, CanvasConfig.DEF_CONSIDER_SEC);
            // フィッシャールール
            isFischer = PlayerPrefs.GetInt(CanvasConfig.KEY_FISCHER_BOOL, CanvasConfig.DEF_FISCHER_BOOL ? (int)CanvasConfig.Bool.TRUE : (int)CanvasConfig.Bool.FALSE) == (int)CanvasConfig.Bool.TRUE ? true : false;
            fischerSec = PlayerPrefs.GetInt(CanvasConfig.KEY_FISCHER_SEC, CanvasConfig.DEF_FISCHER_SEC);  // 1手で増える秒数
        }

        // 初期化
        Init();

    }

    // Update is called once per frame
    void Update()
    {

        // プレイ中
        if (mode == MODE.PLAY)
        {

            //timeSpan = TimeSpan.FromSeconds(elapsedTime[turnPlayer] - (float)stopwatch[turnPlayer].Elapsed.TotalSeconds);
            if (GetElapsedTimeFloat(turnPlayer) <= 0f)
            {
                // 時間切れ
                mode = MODE.TIMEUP;

                PlayOneShot(timeUpSE, nameof(timeUpSE));  // 時間切れの効果音

                canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").Find("ImageTimeUp").gameObject.SetActive(true);  // TIMEUPマーク

                stopwatch[turnPlayer].Stop();   // ストップウォッチ ストップ

                // 手番プレイヤーのボタンを押せなくする
                canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").GetComponent<Button>().interactable = false;  // 打牌ボタン
                canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonNaki").GetComponent<Button>().interactable = false;  // 鳴きボタン
                canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false; // 和了ボタン

                // 全員の鳴きボタンを押せなくする
                for (int i = 0; i < 4; i++)
                {
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;  // 鳴きボタン
                }

                elapsedTime[turnPlayer] = 0f;
                stopwatch[turnPlayer].Reset();   // ストップウォッチ リセット

                // 5秒後に手番プレイヤーの打牌ボタンを再活性化する
                StartCoroutine(DelayEnableDahaiCorutine());
            }
            
            // 残り時間の文字色設定
            //SetTextElapsedTimeColor(canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("TextElapsedTime").gameObject);
            
            // 全プレイヤー 残り時間表示
            //TimeSpan timeSpan;
            for (int i = 0; i < playersNum; i++)
            {
                // 残り時間を再表示
                DisplayElapsedTime(i);
                //canvasClock.transform.Find("PanelDahai" + i).transform.Find("TextElapsedTime").GetComponent<Text>().text = GetElapsedTimeMMSS(i);
            }

            // カウントダウンボイス
            for (int i = 10; i >= 1; i--)
            {
                if (GetElapsedTimeFloat(turnPlayer) < i + 1)
                {
                    if (cdFlg[turnPlayer] == i && mode == MODE.PLAY)
                    {
                        PlayOneShot(cdGirlSE[i], nameof(cdGirlSE));
                        cdFlg[turnPlayer]--;
                        break;
                    }
                }
            }

            // フラグ初期化
            if (GetElapsedTimeFloat(turnPlayer) > 11f)
            {
                cdFlg[turnPlayer] = 10;
            }

        }

    }

    // 打牌ボタンを活性化する
    IEnumerator DelayEnableDahaiCorutine()
    {
        // secondで指定した秒数ループします
        yield return new WaitForSeconds(5.0f);

        canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").GetComponent<Button>().interactable = true;       // 打牌ボタン

    }

    // 初期化
    public void Init()
    {
        for (int i = 0; i < playersNum; i++)
        {
            // 打牌数 初期化
            dahaiCnt = 0;

            // カウントダウンフラグ 初期化
            cdFlg[i] = 10;

            // 鳴き、和了ボタン Enable/Disableの初期化
            boolBtnNaki[i] = true;
            boolBtnAgari[i] = true;

            // TIMEUPマーク 非表示
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").Find("ImageTimeUp").gameObject.SetActive(false);  

            // 残り時間が考慮時間以下なら考慮時間まで増やす
            if (GetElapsedTimeFloat(i) < considerTime)
            {
                elapsedTime[i] = considerTime;
                cdFlg[i] = (int)considerTime - 1;  // カウントダウンフラグを再初期化
                stopwatch[i].Reset();
            }


            // 残り時間表示
            DisplayElapsedTime(i);

            // 大きさ（通常）
            canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(1, 1);
            // 表示
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonStart").gameObject.SetActive(true);     // 開始(親)ボタン
            // 非表示
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").gameObject.SetActive(false);  // 打牌ボタン
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").gameObject.SetActive(false);  // 鳴きボタン
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").gameObject.SetActive(false); // 和了ボタン  
            canvasClock.transform.Find("PanelKaze" + i).gameObject.SetActive(false); // 自風表示
            
        }

        // サンマ
        if (playersNum == 3)
        {
            // サンマなら上家のボタン、メッセージを全て非表示
            // ボタン、持ち時間
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(false);      // 打牌パネル
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(false);     // 発声パネル  
            canvasClock.transform.Find("PanelKaze3").gameObject.SetActive(false);       // 風パネル  
            // メッセージ
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(false);     // 「親がボタンを押して開始してください」
        } else
        {
            // ヨンマなら上家のボタン、メッセージを全て表示
            // ボタン、持ち時間
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(true);      // 打牌パネル
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(true);     // 発声パネル  
            //canvasClock.transform.Find("PanelKaze3").gameObject.SetActive(true);       // 風パネル  
            // メッセージ
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(true);     // 「親がボタンを押して開始してください」
        }

        // 「親がボタンを押して開始してください」表示
        canvasClock.transform.Find("PanelStartMsg").gameObject.SetActive(true);

    }

    // 打牌ボタン
    public void ButtonDahai(int n)
    {
        PlayOneShot(dahaiSE, nameof(dahaiSE));  // 打牌の効果音

        // 鳴き、和了ボタン Enable/Disableの初期化
        for (int i = 0; i < playersNum; i++)
        {
            boolBtnNaki[i] = true;
            boolBtnAgari[i] = true;
        }

        // プレイ再開
        if (mode == MODE.TIMEUP)
        {
            // 時間切れなのに打牌ボタンが押せるということは、時間切れからの再開
            canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").Find("ImageTimeUp").gameObject.SetActive(false);  // TIMEUPマーク 非表示
            mode = MODE.PLAY;
        }

        stopwatch[n].Stop();

        dahaiCnt++;

        // フィッシャールール
        if (isFischer)
        {
            elapsedTime[n] += fischerSec;
            // フィッシャー秒数が足されて1時間を超えた
            if (GetElapsedTimeFloat(n) > 60f * 60f)
            {
                elapsedTime[n] = 60f * 60f - 1f;    // 上限は59分59秒
                stopwatch[n].Reset();
            }
            cdFlg[n] += (int)fischerSec;
        }

        Debug.Log("elapsedTime[n]=" + elapsedTime[n]);
        Debug.Log("stopwatch[n].Elapsed.TotalSeconds=" + stopwatch[n].Elapsed.TotalSeconds);
        Debug.Log("GetElapsedTimeFloat(n)=" + GetElapsedTimeFloat(n));
        Debug.Log("minElapsedTime=" + considerTime);

        // 残り時間が考慮時間以下なら考慮時間まで増やす
        if (GetElapsedTimeFloat(n) < considerTime)
        {
            elapsedTime[n] = considerTime;
            cdFlg[n] = (int)considerTime - 1;
            stopwatch[n].Reset();
        }

        // 残り時間設定
        DisplayElapsedTime(n);

        // 次プレイヤー
        int nextPlayer = (n + 1) % playersNum;

        // 次プレイヤー手番の表示
        DisplayCurrent(nextPlayer, STATUS.DAHAI);

        // 次プレイヤーの残り時間減少の前に 履歴を記録
        elapsedTimeHistory[nextPlayer].Add(elapsedTime[nextPlayer] - (float)stopwatch[nextPlayer].Elapsed.TotalSeconds);

        stopwatch[nextPlayer].Start();
        turnPlayer = nextPlayer;
        
    }


    // 鳴きボタン
    public void ButtonNaki(int n)
    {
        PlayOneShot(nakiSE, nameof(nakiSE));  // 鳴きの効果音

        if (n == turnPlayer)
        {
            // チー
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonNaki").GetComponent<Button>().interactable = false;
            boolBtnNaki[turnPlayer] = false;
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false;
            boolBtnAgari[turnPlayer] = false;
            return;
        }
        else
        {
            // ポン、ミンカン
            stopwatch[turnPlayer].Stop();   // 手番ストップ
            stopwatch[turnPlayer].Reset();  // 経過時間リセット

            // ターンプレイヤーの残り時間を履歴から戻す
            ReverseElapsedTime();

            // 残り時間を再表示
            DisplayElapsedTime(turnPlayer);

            // カウントダウンフラグを初期化
            cdFlg[turnPlayer] = (int)considerTime - 1;

            turnPlayer = n;         // 手番プレイヤー変更
            stopwatch[turnPlayer].Start();   // カウントダウン開始

            // 手番プレイヤーの表示
            DisplayCurrent(turnPlayer, STATUS.PONKAN);
            // 鳴きボタンを全て不活性（ポン、カンより優先の鳴きはない）
            for (int i = 0; i < playersNum; i++)
            {
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;
                boolBtnNaki[i] = false;
            }
            // 鳴いたプレイヤーは和了ボタンも不活性
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false;
                boolBtnAgari[turnPlayer] = false;
        }

    }

    // 和了ボタン
    public void ButtonAgari(int n)
    {
        mode = MODE.AGARI;

        if (n == turnPlayer)
        {
            // ツモ
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonNaki").GetComponent<Button>().interactable = false;
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false;
        }
        else
        {
            // ロン
            stopwatch[turnPlayer].Stop();   // 手番ストップ
            stopwatch[turnPlayer].Reset();  // 経過時間リセット

            // ターンプレイヤーの残り時間を履歴から戻す
            ReverseElapsedTime();

            // 残り時間を再表示
            DisplayElapsedTime(turnPlayer);
            //canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("TextElapsedTime").GetComponent<Text>().text = GetElapsedTimeMMSS(turnPlayer);
            //SetTextElapsedTimeColor(canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("TextElapsedTime").gameObject);
        }

        PlayOneShot(agariSE, nameof(agariSE));  // 和了の効果音

        stopwatch[n].Stop();   // ストップウォッチ ストップ

        // 不活性
        canvasClock.transform.Find("PanelHassei" + n).Find("ButtonAgari").GetComponent<Button>().interactable = false;   // 和了ボタン

        // 全て不活性
        for (int i = 0; i < playersNum; i++)
        {
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = false;   // 打牌ボタン
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // 鳴きボタン
        }
        
        // 表示
        canvasClock.transform.Find("ButtonNextStage").gameObject.SetActive(true);  // 次局ボタン
        canvasClock.transform.Find("ButtonNextStage").SetAsLastSibling();

    }

    // スタートボタン
    public void ButtonStart(int n)
    {
        turnPlayer = n;     // 手番プレイヤー
        string kaze;
        Color32 kazeColor;

        // 「親がボタンを押して開始してください」非表示
        canvasClock.transform.Find("PanelStartMsg").gameObject.SetActive(false);

        for (int i = 0; i < playersNum; i++)
        {
            // 残り時間
            elapsedTimeHistory[i] = new List<float>();
            elapsedTimeHistory[i].Add(elapsedTime[i]);          // 履歴初期値
            // Disable
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonStart").gameObject.SetActive(false);   // 開始(親)ボタン
            // Enable
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").gameObject.SetActive(true);  // 打牌ボタン
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").gameObject.SetActive(true);    // 鳴きボタン
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").gameObject.SetActive(true);   // 和了ボタン    
            canvasClock.transform.Find("PanelKaze" + i).gameObject.SetActive(true); // 自風表示
            switch (i)
            {
                case 0:
                    kaze = "東";
                    kazeColor = new Color32(255, 0, 0, 255);    // 赤文字
                    break;
                case 1:
                    kaze = "南";
                    kazeColor = new Color32(255, 255, 255, 255);  // 白文字
                    break;
                case 2:
                    kaze = "西";
                    kazeColor = new Color32(255, 255, 255, 255);  // 白文字
                    break;
                case 3:
                    kaze = "北";
                    kazeColor = new Color32(255, 255, 255, 255);  // 白文字
                    break;
                default:
                    kaze = "東";
                    kazeColor = new Color32(255, 0, 0, 255);    // 赤文字
                    break;
            }
            canvasClock.transform.Find("PanelKaze" + ((i + n) % playersNum)).Find("TextKaze").GetComponent<Text>().text = kaze;
            canvasClock.transform.Find("PanelKaze" + ((i + n) % playersNum)).Find("TextKaze").GetComponent<Text>().color = kazeColor;
        }
        
        stopwatch[turnPlayer].Start();

        PlayOneShot(startSE, nameof(startSE));  // 対局開始の効果音

        // ボタンの表示・活性
        DisplayCurrent(turnPlayer, STATUS.START);

        mode = MODE.PLAY;
    }

    // 次局ボタン
    public void ButtonNextStage()
    {
        canvasClock.transform.Find("ButtonNextStage").gameObject.SetActive(false);
        Init();
    }

    // 3線リーダー
    public void Button3LineLeader()
    {
        stopwatch[turnPlayer].Stop();

        // 設定画面 初期化
        canvasConfig.transform.GetComponent<CanvasConfig>().Init();

        // バージョン表示
        canvasConfig.transform.Find("PanelConfig").Find("TextTitle").Find("TextVersion").GetComponent<Text>().text = "Ver." + Application.version;

        // 設定画面表示
        canvasConfig.transform.gameObject.SetActive(true);

#if ENABLE_ADMOB
        canvasConfig.transform.GetComponent<AdBannerManager>().bannerView.Show();
#endif

    }


    // 現在の状態からボタンの表示・非表示、活性・不活性を設定
    private void DisplayCurrent(int n, STATUS status)
    {
        for (int i = 0; i < playersNum; i++)
        {
            // 残り時間表示
            DisplayElapsedTime(i);

            if (i == n)
            {
                // 手番プレイヤー
                canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = true;    // 打牌ボタン
                canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(2, 2);
                canvasClock.transform.Find("PanelDahai" + i).SetAsLastSibling();
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = true;    // 鳴きボタン
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = true;   // 和了ボタン
                if (status == STATUS.START)
                {
                    // 開局直後
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;    // 鳴きボタン
                    boolBtnNaki[i] = false;
                }
            }
/*            else if(i == (n + 1) % 4)
            {
                // 手番プレイヤーの下家
            }
            else if(i == (n + 1) % 4)
            {
                // 手番プレイヤーの対家
            }*/
            else if(i == (n + (playersNum - 1)) % playersNum)
            {
                // 手番プレイヤーの上家
                canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = false;   // 打牌ボタン
                canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(1, 1);
                if (status == STATUS.CHII)
                {
                    // チーなら上家は鳴き、和了ボタンは押せない
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // 鳴きボタン
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // 和了ボタン
                    boolBtnAgari[i] = false;
                }
                else if (status == STATUS.PONKAN)
                {
                    // ポンカンは上家はチーしてなければ和了ボタンが押せる
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = boolBtnNaki[i];  // 和了ボタン
                }
                else if (status == STATUS.DAHAI)
                {
                    // 普通に順番が来たのなら、鳴き、和了ボタンは押せない
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // 鳴きボタン
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // 和了ボタン
                    boolBtnAgari[i] = false;
                }

                if (status == STATUS.START)
                {
                    // 開局直後
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // 鳴きボタン
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // 和了ボタン
                    boolBtnAgari[i] = false;
                }
            }
            else
            {
                // 手番じゃないプレイヤー
                canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = false;   // 打牌ボタン
                canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(1, 1);
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = boolBtnNaki[i];    // 鳴きボタン
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = boolBtnAgari[i];   // 和了ボタン
                if (status == STATUS.START)
                {
                    // 開局直後
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // 鳴きボタン
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // 和了ボタン
                    boolBtnAgari[i] = false;
                }
            }

        }
    }

    // 残り時間表示
    private void DisplayElapsedTime(int n)
    {
        canvasClock.transform.Find("PanelDahai" + n).Find("TextElapsedTime").GetComponent<Text>().text = GetElapsedTimeMMSS(n);
        SetTextElapsedTimeColor(canvasClock.transform.Find("PanelDahai" + n).Find("TextElapsedTime").gameObject);
    }

    // ターンプレイヤーの残り時間を履歴から戻す
    private void ReverseElapsedTime()
    {
        if (elapsedTimeHistory[turnPlayer].Count > 0)
        {
            elapsedTime[turnPlayer] = elapsedTimeHistory[turnPlayer].Last();
            elapsedTimeHistory[turnPlayer].RemoveAt(elapsedTimeHistory[turnPlayer].Count - 1);
        }
    }

    // 残り時間の文字色設定
    private void SetTextElapsedTimeColor(GameObject textElapsedTimeObj)
    {
        string text = textElapsedTimeObj.GetComponent<Text>().text;
        int sec;

        // 残り時間1分以上
        if (text.Substring(0, 2) != "00")
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 255, 255, 255);  // 白文字
            return;
        }

        // 残り秒数
        sec = int.Parse(text.Substring(text.Length - 2));

        // 残り時間の文字色
        if (sec <= 5)
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 0, 0, 255);  // 赤文字
        } else if (sec <= 10)
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 255, 0, 255);  // 黄文字
        } else
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 255, 255, 255);  // 白文字
        }
    }

    // 残り時間取得（mm:ss）
    private string GetElapsedTimeMMSS(int player)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime[player] - (float)stopwatch[player].Elapsed.TotalSeconds);
        string ret = canvasClock.transform.Find("PanelDahai" + player).Find("TextElapsedTime").GetComponent<Text>().text = timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2");

        return ret;
    }

    // 残り時間取得（float）
    private float GetElapsedTimeFloat(int player)
    {
        return elapsedTime[player] - (float)stopwatch[player].Elapsed.TotalSeconds;
    }

    // 効果音・ボイスを鳴らす
    private void PlayOneShot(AudioClip audioClip, string valName)
    {
        Debug.Log("audioClip valName = " + valName);

        bool isSnd;
        switch (valName)
        {
            case "dahaiSE":
                if (PlayerPrefs.HasKey(CanvasConfig.KEY_SND_DAHAI_BOOL))
                {
                    isSnd = PlayerPrefs.GetInt(CanvasConfig.KEY_SND_DAHAI_BOOL) == (int)CanvasConfig.Bool.TRUE ? true : false;
                }
                else
                {
                    isSnd = CanvasConfig.DEF_SND_DAHAI_BOOL;
                }
                break;
            case "nakiSE":
                if (PlayerPrefs.HasKey(CanvasConfig.KEY_SND_NAKI_BOOL))
                {
                    isSnd = PlayerPrefs.GetInt(CanvasConfig.KEY_SND_NAKI_BOOL) == (int)CanvasConfig.Bool.TRUE ? true : false;
                }
                else
                {
                    isSnd = CanvasConfig.DEF_SND_NAKI_BOOL;
                }
                break;
            case "agariSE":
                if (PlayerPrefs.HasKey(CanvasConfig.KEY_SND_AGARI_BOOL))
                {
                    isSnd = PlayerPrefs.GetInt(CanvasConfig.KEY_SND_AGARI_BOOL) == (int)CanvasConfig.Bool.TRUE ? true : false;
                }
                else
                {
                    isSnd = CanvasConfig.DEF_SND_AGARI_BOOL;
                }
                break;
            case "cdGirlSE":
            case "timeUpSE":
                if (PlayerPrefs.HasKey(CanvasConfig.KEY_SND_CNTDOWN_BOOL))
                {
                    isSnd = PlayerPrefs.GetInt(CanvasConfig.KEY_SND_CNTDOWN_BOOL) == (int)CanvasConfig.Bool.TRUE ? true : false;
                }
                else
                {
                    isSnd = CanvasConfig.DEF_SND_CNTDOWN_BOOL;
                }
                break;
            case "startSE":
                if (PlayerPrefs.HasKey(CanvasConfig.KEY_SND_START_BOOL))
                {
                    isSnd = PlayerPrefs.GetInt(CanvasConfig.KEY_SND_START_BOOL) == (int)CanvasConfig.Bool.TRUE ? true : false;
                }
                else
                {
                    isSnd = CanvasConfig.DEF_SND_START_BOOL;
                }
                break;
            default:
                isSnd = false;
                break;
        }

        if (isSnd)
        {
            audioSource.PlayOneShot(audioClip);   // 効果音・ボイス
        }

        Debug.Log("isSnd = " + isSnd);

    }

    

}
