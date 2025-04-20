using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasConfig : MonoBehaviour
{
    public GameObject canvasClock;
    public GameObject canvasConfig;

    // --- デフォルト値 ---

    public const int DEF_ELAPSED_MIN = 3;       // 残り時間
    public const int DEF_CONSIDER_SEC = 15;     // 持ち時間消化後の１手の考慮時間
    public const bool DEF_FISCHER_BOOL = false; // フィッシャールール有無
    public const int DEF_FISCHER_SEC = 7;       // フィッシャーで1手ごとに足される秒数

    // 効果音の有無
    public const bool DEF_SND_DAHAI_BOOL   = true;  // 打牌
    public const bool DEF_SND_NAKI_BOOL    = true;  // 鳴き
    public const bool DEF_SND_AGARI_BOOL   = true;  // 和了
    public const bool DEF_SND_CNTDOWN_BOOL = true;  // カウントダウン
    public const bool DEF_SND_START_BOOL   = true;  // 対局開始

    // --- PlayerPrefsキー ---
    
    public const string KEY_ELAPSED_MIN_ = "KEY_ELAPSED_SEC_";          // 残り時間
    public const string KEY_CONSIDER_SEC = "KEY_CONSIDER_SEC";       // 持ち時間消化後の１手の考慮時間
    public const string KEY_SAMETIME_BOOL = "KEY_SAMETIME_BOOL";        // 持ち時間一律（ハンデなし）
    public const string KEY_FISCHER_BOOL = "KEY_FISCHER_BOOL";          // フィッシャールール有無
    public const string KEY_FISCHER_SEC = "KEY_FISCHER_SEC";            // フィッシャーで1手ごとに足される秒数
    public const string KEY_PLAYERS_NUM = "KEY_PLAYERS_NUM";            // プレイヤー人数

    // 効果音の有無
    public const string KEY_SND_DAHAI_BOOL   = "KEY_SND_DAHAI_BOOL";    // 打牌
    public const string KEY_SND_NAKI_BOOL    = "KEY_SND_NAKI_BOOL";     // 鳴き
    public const string KEY_SND_AGARI_BOOL   = "KEY_SND_AGARI_BOOL";    // 和了
    public const string KEY_SND_CNTDOWN_BOOL = "KEY_SND_CNTDOWN_BOOL";  // カウントダウン
    public const string KEY_SND_START_BOOL   = "KEY_SND_START_BOOL";    // 対局開始

    // ブール値
    public enum Bool : int
    {
        TRUE  = 0,
        FALSE = 1
    }

    // メッセージID
    enum MsgId : int
    {
        NEW_GAME            = 0,
        SET_DEFAULT         = 1,
        WARN_ZERO_CONSIDER  = 2,
    }

    // 効果音有無
    enum SndOnOff : int
    {
        DAHAI       = 0,
        NAKI        = 1,
        AGARI       = 2,
        CNTDOWN     = 3,
        START       = 4,
    }

    MsgId msgId;
    
#if ENABLE_ADMOB
    public AdBannerManager adBannerManager;
#endif

    // Start is called before the first frame update
    void Start()
    {

#if ENABLE_ADMOB
        if (adBannerManager == null)
        {
            adBannerManager = gameObject.AddComponent<AdBannerManager>();
        }
        adBannerManager.InitializeBanner();
#endif

        // --- PlaceHoler設定 ---

        // 持ち時間
        for (int i = 0; i < 4; i++)
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).Find("Placeholder").GetComponent<Text>().text = DEF_ELAPSED_MIN.ToString();
        }
        // 持ち時間消化後の考慮時間(秒)
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").Find("Placeholder").GetComponent<Text>().text = DEF_CONSIDER_SEC.ToString();
        // フィッシャールールで足される秒数
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").Find("Placeholder").GetComponent<Text>().text = DEF_FISCHER_SEC.ToString();
        // 持ち時間一律（ハンデなし）
        if (PlayerPrefs.GetInt(KEY_SAMETIME_BOOL, (int)Bool.FALSE) == (int)Bool.TRUE)
        {
            canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn = true;
        } else
        {
            canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn = false;
        }
        // サンマ
        if (CanvasClock.playersNum == 3)
        {
            canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn = true;
        } else
        {
            canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 画面表示 初期化（PlayerPrefsの値を画面に反映）
    public void Init()
    {
        //for (int i = 0; i < CanvasClock.playersNum; i++)
        for (int i = 0; i < 4; i++)
        {
            // 持ち時間
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = PlayerPrefs.GetInt(KEY_ELAPSED_MIN_ + i, DEF_ELAPSED_MIN).ToString();
        }

        // 持ち時間消化後の考慮時間
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = PlayerPrefs.GetInt(KEY_CONSIDER_SEC, DEF_CONSIDER_SEC).ToString();

        // サンマ


        // フィッシャールール
        canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(KEY_FISCHER_BOOL, DEF_FISCHER_BOOL ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = PlayerPrefs.GetInt(KEY_FISCHER_SEC, DEF_FISCHER_SEC).ToString();

        // 効果音・ボイス
                // 効果音・ボイス
        string[] toggleSnd = new string[]{"ToggleSndDahai", "ToggleSndNaki", "ToggleSndAgari", "ToggleSndCntDown", "ToggleSndStart",};
        string[] key = new string[]{KEY_SND_DAHAI_BOOL, KEY_SND_NAKI_BOOL, KEY_SND_AGARI_BOOL, KEY_SND_CNTDOWN_BOOL, KEY_SND_START_BOOL,};
        bool[] def = new bool[]{DEF_SND_DAHAI_BOOL, DEF_SND_NAKI_BOOL, DEF_SND_AGARI_BOOL, DEF_SND_CNTDOWN_BOOL, DEF_SND_START_BOOL,};

        for (int i = 0; i < toggleSnd.Length; i++)
        {
            canvasConfig.transform.Find("PanelConfig").Find(toggleSnd[i]).GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(key[i], def[i] ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;

            Debug.Log("toggleSnd[" + i + "]=" + toggleSnd[i]);
            Debug.Log("key[" + i + "]=" + PlayerPrefs.GetInt(key[i]));
        }


/*
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndDahai").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(KEY_SND_DAHAI_BOOL, DEF_SND_DAHAI_BOOL ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndNaki").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(KEY_SND_NAKI_BOOL, DEF_SND_NAKI_BOOL ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndAgari").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(KEY_SND_AGARI_BOOL, DEF_SND_AGARI_BOOL ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndCntDown").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(KEY_SND_CNTDOWN_BOOL, DEF_SND_CNTDOWN_BOOL ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndStart").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(KEY_SND_START_BOOL, DEF_SND_START_BOOL ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;
*/
    }

    // デフォルト値に戻す
    private void RevertDefault()
    {
        // 持ち時間
        //for (int i = 0; i < CanvasClock.playersNum; i++)
        for (int i = 0; i < 4; i++)
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = DEF_ELAPSED_MIN.ToString();
        }

        // 持ち時間消化後の考慮時間
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = DEF_CONSIDER_SEC.ToString();

        // フィッシャールール
        canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn = DEF_FISCHER_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = DEF_FISCHER_SEC.ToString();

        // 効果音・ボイス
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndDahai").GetComponent<Toggle>().isOn = DEF_SND_DAHAI_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndNaki").GetComponent<Toggle>().isOn = DEF_SND_NAKI_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndAgari").GetComponent<Toggle>().isOn = DEF_SND_AGARI_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndCntDown").GetComponent<Toggle>().isOn = DEF_SND_CNTDOWN_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndStart").GetComponent<Toggle>().isOn = DEF_SND_START_BOOL;

        // 設定値をPlayerPrefsで保存
        SetPlayerPrefs();
    }

    // 自家持ち時間の変更時
    public void InputFieldET0onChanged()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
        {
            // 上下対家の持ち時間を自家と同じに
            string sameTime = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET0").GetComponent<InputField>().text;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text = sameTime;
        }
    }

    // トグルボタン（一律（ハンデなし））
    public void ToggleSameTime()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
        {
            // 上下対家の持ち時間を入力不可
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().interactable = false;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().interactable = false;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = false;
            // 上下対家の持ち時間を自家と同じに
            string sameTime = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET0").GetComponent<InputField>().text;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text = sameTime;
        }
        else
        {
            // 上下対家の持ち時間を入力可
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().interactable = true;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().interactable = true;
            // サンマなら入力不可のまま
            if (!canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn) { 
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = true;
            }
        }
    }

    // トグルボタン（サンマ）
    public void ToggleSanma()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn)
        {
            // 上家の持ち時間を入力不可
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = false;
        } else
        {
            // 一律なら入力不可のまま
            if (!canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
            {
                // 上家の持ち時間を入力可
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = true;
                // 空なら初期値設定
                if (canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text != "")
                {
                    canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text =
                        canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").Find("Placeholder").GetComponent<Text>().text;
                }
            }
        }
    }

    // トグルボタン（フィッシャールール）
    public void ToggleFischer()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn)
        {
            // フィッシャー秒数 活性
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().interactable = true;
        } else
        {
            // フィッシャー秒数 不活性
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().interactable = false;
        }
    }

    // トグルボタン（効果音・ボイス）
    public void ToggleSnd(int n)
    {
        string toggle;
        string key;

        switch (n)
        {
            case (int)SndOnOff.DAHAI:           // 打牌
                toggle = "ToggleSndDahai";
                key = KEY_SND_DAHAI_BOOL;
                break;
            case (int)SndOnOff.NAKI:            // 鳴き
                toggle = "ToggleSndNaki";
                key = KEY_SND_NAKI_BOOL;
                break;
            case (int)SndOnOff.AGARI:           // 和了
                toggle = "ToggleSndAgari";
                key = KEY_SND_AGARI_BOOL;
                break;
            case (int)SndOnOff.CNTDOWN:         // カウントダウン
                toggle = "ToggleSndCntDown";
                key = KEY_SND_CNTDOWN_BOOL;
                break;
            case (int)SndOnOff.START:           // 対局開始
                toggle = "ToggleSndStart";
                key = KEY_SND_START_BOOL;
                break;
            default:
                toggle = "ToggleSndDahai";
                key = KEY_SND_DAHAI_BOOL;
                break;
        }

        if (canvasConfig.transform.Find("PanelConfig").Find(toggle).GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt(key, (int)Bool.TRUE);
        } else {
            PlayerPrefs.SetInt(key, (int)Bool.FALSE);
        }

        Debug.Log("toggle = " + toggle);
        Debug.Log("bool = " + PlayerPrefs.GetInt(key));

    }



    // 新規ゲームボタン
    public void ButtonNewGame()
    {
        // メッセージボックス表示
        DispMsgBox(MsgId.NEW_GAME);

    }

    // 新規ゲーム処理
    private void ProcNewGame()
    {
        // 持ち時間
        for (int i = 0; i < CanvasClock.playersNum; i++)
        {
            canvasClock.transform.GetComponent<CanvasClock>().stopwatch[i].Reset();
            canvasClock.transform.GetComponent<CanvasClock>().elapsedTime[i] = float.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text) * 60f;
        }

        // 持ち時間消化後の考慮時間
        canvasClock.transform.GetComponent<CanvasClock>().considerTime = float.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text);

        // サンマ
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn)
        {
            CanvasClock.playersNum = 3;
            PlayerPrefs.SetInt(KEY_PLAYERS_NUM, 3);
            // サンマなら上家のボタン、メッセージを全て非表示
            // ボタン、持ち時間
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(false);      // 打牌パネル
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(false);     // 発声パネル  
            // メッセージ
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(false);     // 「親がボタンを押して開始してください」
        } else
        {
            CanvasClock.playersNum = 4;
            PlayerPrefs.SetInt(KEY_PLAYERS_NUM, 4);
            // ヨンマなら上家のボタン、メッセージを全て表示
            // ボタン、持ち時間
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(true);      // 打牌パネル
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(true);     // 発声パネル  
            // メッセージ
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(true);     // 「親がボタンを押して開始してください」
        }

        // フィッシャールール
        canvasClock.transform.GetComponent<CanvasClock>().isFischer = canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn;
        canvasClock.transform.GetComponent<CanvasClock>().fischerSec = float.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text);

        // 設定値をPlayerPrefsで保存
        SetPlayerPrefs();

        // 新規ゲーム
        canvasClock.transform.GetComponent<CanvasClock>().Init();

        canvasConfig.transform.gameObject.SetActive(false);
    }

    // デフォルトに戻すボタン
    public void ButtonDefault()
    {
        DispMsgBox(MsgId.SET_DEFAULT);
    }

    // メッセージボックス表示処理
    private void DispMsgBox(MsgId argMsgId)
    {
        string textMsg;
        bool buttonYes;
        bool buttonNo;
        bool buttonOk;

        msgId = argMsgId;

        canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(true);

        switch (argMsgId)
        {
            case MsgId.NEW_GAME:
                textMsg = "持ち時間をリセットして新しいゲームを開始します。\nよろしいですか？";
                buttonYes = true;
                buttonNo = true;
                buttonOk = false;
                break;
            case MsgId.SET_DEFAULT:
                textMsg = "設定を初期値に戻します。\nよろしいですか？";
                buttonYes = true;
                buttonNo = true;
                buttonOk = false;
                break;
            case MsgId.WARN_ZERO_CONSIDER:
                textMsg = "持ち時間0秒のプレイヤーがいる場合は考慮時間を0秒にできません。\n考慮時間を初期値に設定しました。";
                buttonYes = false;
                buttonNo = false;
                buttonOk = true;
                break;
            default:
                textMsg = "OKボタンを押してください";
                buttonYes = false;
                buttonNo = false;
                buttonOk = true;
                break;
        }

        canvasConfig.transform.Find("PanelMsgBox").Find("ImageMsgBox").Find("TextMessage").GetComponent<Text>().text = textMsg;
        canvasConfig.transform.Find("PanelMsgBox").Find("ImageMsgBox").Find("ButtonYes").gameObject.SetActive(buttonYes);
        canvasConfig.transform.Find("PanelMsgBox").Find("ImageMsgBox").Find("ButtonNo").gameObject.SetActive(buttonNo);
        canvasConfig.transform.Find("PanelMsgBox").Find("ImageMsgBox").Find("ButtonOk").gameObject.SetActive(buttonOk);

    }

    // 閉じるボタン
    public void ButtonClose()
    {
#if ENABLE_ADMOB
        adBannerManager.BannerHide();
#endif

        // 入力値のチェック
        if (!ChkInputValue())
        {
            return;
        }

        // 設定値をPlayerPrefsで保存
        SetPlayerPrefs();

            // ストップウォッチ スタート
        if (canvasClock.transform.GetComponent<CanvasClock>().mode == CanvasClock.MODE.PLAY)
        {
            canvasClock.transform.GetComponent<CanvasClock>().stopwatch[canvasClock.transform.GetComponent<CanvasClock>().turnPlayer].Start();
        }

        canvasConfig.transform.gameObject.SetActive(false);
    }

    // 入力値のチェック
    public bool ChkInputValue()
    {
        string text;

        // --- 持ち時間 ---

        for (int i = 0; i < CanvasClock.playersNum; i++)
        {
            // 空欄ならデフォルト値をセット
            text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text;
            if (text == "")
            {
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = DEF_ELAPSED_MIN.ToString();
            }

            // 0～59分
            if (int.Parse(text) < 0)
            {
                // 0分以下は0分にセット
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = "0";
            } 
            else if (int.Parse(text) > 59)
            {
                // 59分越えは59分にセット
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = "59";
            }
        }

        // --- 持ち時間消化後の考慮時間 ---

        text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text;
        // 空欄ならデフォルト値をセット
        if (text == "")
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = DEF_CONSIDER_SEC.ToString();
        }

        // 0～30秒
        if (int.Parse(text) == 0)
        {
            // 持ち時間0秒のプレイヤーがいるかチェック
            for (int i = 0; i < CanvasClock.playersNum; i++)
            {
                text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text;
                if (int.Parse(text) == 0)
                {
                    // 持ち時間0秒のプレイヤーが場合、考慮時間は0秒にできないのでデフォルト値をセット
                    canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = DEF_CONSIDER_SEC.ToString();
                    // メッセージボックス表示
                    DispMsgBox(MsgId.WARN_ZERO_CONSIDER);
                    return false;
                }
            }
        }
        else if (int.Parse(text) < 0)
        {
            // 0秒以下は0秒にセット
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = "0";
        }
        else if (int.Parse(text) > 30)
        {
            // 30秒越えは30秒にセット
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = "30";
        }

        // --- フィッシャールール ---

        text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text;
        // 空欄ならデフォルト値をセット
        if (text == "")
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = DEF_FISCHER_SEC.ToString();
        }

        // 1～30秒
        if (int.Parse(text) < 1)
        {
            // 1秒未満は1秒にセット
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = "1";
        } else if (int.Parse(text) > 30)
        {
            // 30秒越えは30秒にセット
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = "30";
        }

        return true;
    }

    // 設定値をPlayerPrefsで保存
    private void SetPlayerPrefs()
    {
        // 持ち時間
        //for (int i = 0; i < CanvasClock.playersNum; i++)
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.SetInt(KEY_ELAPSED_MIN_ + i, int.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text));
        }

        // 最低限残り時間
        PlayerPrefs.SetInt(KEY_CONSIDER_SEC, int.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text));

        // 持ち時間一律（ハンデなし）
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt(KEY_SAMETIME_BOOL, (int)Bool.TRUE);
        } else {
            PlayerPrefs.SetInt(KEY_SAMETIME_BOOL, (int)Bool.FALSE);
        }

        // フィッシャールール
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt(KEY_FISCHER_BOOL, (int)Bool.TRUE);
        } else {
            PlayerPrefs.SetInt(KEY_FISCHER_BOOL, (int)Bool.FALSE);
        }
        PlayerPrefs.SetInt(KEY_FISCHER_SEC, int.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text));

        // 効果音・ボイス
        string[] toggleSnd = new string[]{"ToggleSndDahai", "ToggleSndNaki", "ToggleSndAgari", "ToggleSndCntDown", "ToggleSndStart",};
        string[] key = new string[]{KEY_SND_DAHAI_BOOL, KEY_SND_NAKI_BOOL, KEY_SND_AGARI_BOOL, KEY_SND_CNTDOWN_BOOL, KEY_SND_START_BOOL,};

        for (int i = 0; i < toggleSnd.Length; i++)
        {
            if (canvasConfig.transform.Find("PanelConfig").Find(toggleSnd[i]).GetComponent<Toggle>().isOn)
            {
                PlayerPrefs.SetInt(key[i], (int)Bool.TRUE);
            } else {
                PlayerPrefs.SetInt(key[i], (int)Bool.FALSE);
            }

            Debug.Log("toggleSnd[" + i + "]=" + toggleSnd[i]);
            Debug.Log("key[" + i + "]=" + PlayerPrefs.GetInt(key[i]));
        }
    }


    // --- メッセージボックス用ボタン ---

    // はいボタン
    public void ButtonYes()
    {
        canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(false);

        switch (msgId)
        {
            case MsgId.NEW_GAME:
                // 入力値のチェック
                if (!ChkInputValue())
                {
                    return;
                }
                ProcNewGame();
#if ENABLE_ADMOB
                adBannerManager.BannerHide();
#endif
                break;
            case MsgId.SET_DEFAULT:
                RevertDefault();
                break;
            default:
                canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(false);
                break;
        }
    }

    // いいえボタン
    public void ButtonNo()
    {
        canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(false);
    }

    // OKボタン
    public void ButtonOk(int n)
    {
        if (n == 0)
        {
            // メッセージボックス
            canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(false);
        } else if (n == 1)
        {
            // スペシャルサンクス
            canvasConfig.transform.Find("PanelSpecialThanks").gameObject.SetActive(false);
        }
    }

    // SpecialThanksボタン
    public void ButtonThanks()
    {
        canvasConfig.transform.Find("PanelSpecialThanks").gameObject.SetActive(true);
    }

}
