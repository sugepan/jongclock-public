using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasConfig : MonoBehaviour
{
    public GameObject canvasClock;
    public GameObject canvasConfig;

    // --- �f�t�H���g�l ---

    public const int DEF_ELAPSED_MIN = 3;       // �c�莞��
    public const int DEF_CONSIDER_SEC = 15;     // �������ԏ�����̂P��̍l������
    public const bool DEF_FISCHER_BOOL = false; // �t�B�b�V���[���[���L��
    public const int DEF_FISCHER_SEC = 7;       // �t�B�b�V���[��1�育�Ƃɑ������b��

    // ���ʉ��̗L��
    public const bool DEF_SND_DAHAI_BOOL   = true;  // �Ŕv
    public const bool DEF_SND_NAKI_BOOL    = true;  // ��
    public const bool DEF_SND_AGARI_BOOL   = true;  // �a��
    public const bool DEF_SND_CNTDOWN_BOOL = true;  // �J�E���g�_�E��
    public const bool DEF_SND_START_BOOL   = true;  // �΋ǊJ�n

    // --- PlayerPrefs�L�[ ---
    
    public const string KEY_ELAPSED_MIN_ = "KEY_ELAPSED_SEC_";          // �c�莞��
    public const string KEY_CONSIDER_SEC = "KEY_CONSIDER_SEC";       // �������ԏ�����̂P��̍l������
    public const string KEY_SAMETIME_BOOL = "KEY_SAMETIME_BOOL";        // �������Ԉꗥ�i�n���f�Ȃ��j
    public const string KEY_FISCHER_BOOL = "KEY_FISCHER_BOOL";          // �t�B�b�V���[���[���L��
    public const string KEY_FISCHER_SEC = "KEY_FISCHER_SEC";            // �t�B�b�V���[��1�育�Ƃɑ������b��
    public const string KEY_PLAYERS_NUM = "KEY_PLAYERS_NUM";            // �v���C���[�l��

    // ���ʉ��̗L��
    public const string KEY_SND_DAHAI_BOOL   = "KEY_SND_DAHAI_BOOL";    // �Ŕv
    public const string KEY_SND_NAKI_BOOL    = "KEY_SND_NAKI_BOOL";     // ��
    public const string KEY_SND_AGARI_BOOL   = "KEY_SND_AGARI_BOOL";    // �a��
    public const string KEY_SND_CNTDOWN_BOOL = "KEY_SND_CNTDOWN_BOOL";  // �J�E���g�_�E��
    public const string KEY_SND_START_BOOL   = "KEY_SND_START_BOOL";    // �΋ǊJ�n

    // �u�[���l
    public enum Bool : int
    {
        TRUE  = 0,
        FALSE = 1
    }

    // ���b�Z�[�WID
    enum MsgId : int
    {
        NEW_GAME            = 0,
        SET_DEFAULT         = 1,
        WARN_ZERO_CONSIDER  = 2,
    }

    // ���ʉ��L��
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

        // --- PlaceHoler�ݒ� ---

        // ��������
        for (int i = 0; i < 4; i++)
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).Find("Placeholder").GetComponent<Text>().text = DEF_ELAPSED_MIN.ToString();
        }
        // �������ԏ�����̍l������(�b)
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").Find("Placeholder").GetComponent<Text>().text = DEF_CONSIDER_SEC.ToString();
        // �t�B�b�V���[���[���ő������b��
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").Find("Placeholder").GetComponent<Text>().text = DEF_FISCHER_SEC.ToString();
        // �������Ԉꗥ�i�n���f�Ȃ��j
        if (PlayerPrefs.GetInt(KEY_SAMETIME_BOOL, (int)Bool.FALSE) == (int)Bool.TRUE)
        {
            canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn = true;
        } else
        {
            canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn = false;
        }
        // �T���}
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

    // ��ʕ\�� �������iPlayerPrefs�̒l����ʂɔ��f�j
    public void Init()
    {
        //for (int i = 0; i < CanvasClock.playersNum; i++)
        for (int i = 0; i < 4; i++)
        {
            // ��������
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = PlayerPrefs.GetInt(KEY_ELAPSED_MIN_ + i, DEF_ELAPSED_MIN).ToString();
        }

        // �������ԏ�����̍l������
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = PlayerPrefs.GetInt(KEY_CONSIDER_SEC, DEF_CONSIDER_SEC).ToString();

        // �T���}


        // �t�B�b�V���[���[��
        canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(KEY_FISCHER_BOOL, DEF_FISCHER_BOOL ? (int)Bool.TRUE : (int)Bool.FALSE) == (int)Bool.TRUE ? true : false;
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = PlayerPrefs.GetInt(KEY_FISCHER_SEC, DEF_FISCHER_SEC).ToString();

        // ���ʉ��E�{�C�X
                // ���ʉ��E�{�C�X
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

    // �f�t�H���g�l�ɖ߂�
    private void RevertDefault()
    {
        // ��������
        //for (int i = 0; i < CanvasClock.playersNum; i++)
        for (int i = 0; i < 4; i++)
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = DEF_ELAPSED_MIN.ToString();
        }

        // �������ԏ�����̍l������
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = DEF_CONSIDER_SEC.ToString();

        // �t�B�b�V���[���[��
        canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn = DEF_FISCHER_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = DEF_FISCHER_SEC.ToString();

        // ���ʉ��E�{�C�X
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndDahai").GetComponent<Toggle>().isOn = DEF_SND_DAHAI_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndNaki").GetComponent<Toggle>().isOn = DEF_SND_NAKI_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndAgari").GetComponent<Toggle>().isOn = DEF_SND_AGARI_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndCntDown").GetComponent<Toggle>().isOn = DEF_SND_CNTDOWN_BOOL;
        canvasConfig.transform.Find("PanelConfig").Find("ToggleSndStart").GetComponent<Toggle>().isOn = DEF_SND_START_BOOL;

        // �ݒ�l��PlayerPrefs�ŕۑ�
        SetPlayerPrefs();
    }

    // ���Ǝ������Ԃ̕ύX��
    public void InputFieldET0onChanged()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
        {
            // �㉺�ΉƂ̎������Ԃ����ƂƓ�����
            string sameTime = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET0").GetComponent<InputField>().text;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text = sameTime;
        }
    }

    // �g�O���{�^���i�ꗥ�i�n���f�Ȃ��j�j
    public void ToggleSameTime()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
        {
            // �㉺�ΉƂ̎������Ԃ���͕s��
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().interactable = false;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().interactable = false;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = false;
            // �㉺�ΉƂ̎������Ԃ����ƂƓ�����
            string sameTime = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET0").GetComponent<InputField>().text;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().text = sameTime;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text = sameTime;
        }
        else
        {
            // �㉺�ΉƂ̎������Ԃ���͉�
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET1").GetComponent<InputField>().interactable = true;
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET2").GetComponent<InputField>().interactable = true;
            // �T���}�Ȃ���͕s�̂܂�
            if (!canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn) { 
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = true;
            }
        }
    }

    // �g�O���{�^���i�T���}�j
    public void ToggleSanma()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn)
        {
            // ��Ƃ̎������Ԃ���͕s��
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = false;
        } else
        {
            // �ꗥ�Ȃ���͕s�̂܂�
            if (!canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
            {
                // ��Ƃ̎������Ԃ���͉�
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().interactable = true;
                // ��Ȃ珉���l�ݒ�
                if (canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text != "")
                {
                    canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").GetComponent<InputField>().text =
                        canvasConfig.transform.Find("PanelConfig").Find("InputFieldET3").Find("Placeholder").GetComponent<Text>().text;
                }
            }
        }
    }

    // �g�O���{�^���i�t�B�b�V���[���[���j
    public void ToggleFischer()
    {
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn)
        {
            // �t�B�b�V���[�b�� ����
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().interactable = true;
        } else
        {
            // �t�B�b�V���[�b�� �s����
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().interactable = false;
        }
    }

    // �g�O���{�^���i���ʉ��E�{�C�X�j
    public void ToggleSnd(int n)
    {
        string toggle;
        string key;

        switch (n)
        {
            case (int)SndOnOff.DAHAI:           // �Ŕv
                toggle = "ToggleSndDahai";
                key = KEY_SND_DAHAI_BOOL;
                break;
            case (int)SndOnOff.NAKI:            // ��
                toggle = "ToggleSndNaki";
                key = KEY_SND_NAKI_BOOL;
                break;
            case (int)SndOnOff.AGARI:           // �a��
                toggle = "ToggleSndAgari";
                key = KEY_SND_AGARI_BOOL;
                break;
            case (int)SndOnOff.CNTDOWN:         // �J�E���g�_�E��
                toggle = "ToggleSndCntDown";
                key = KEY_SND_CNTDOWN_BOOL;
                break;
            case (int)SndOnOff.START:           // �΋ǊJ�n
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



    // �V�K�Q�[���{�^��
    public void ButtonNewGame()
    {
        // ���b�Z�[�W�{�b�N�X�\��
        DispMsgBox(MsgId.NEW_GAME);

    }

    // �V�K�Q�[������
    private void ProcNewGame()
    {
        // ��������
        for (int i = 0; i < CanvasClock.playersNum; i++)
        {
            canvasClock.transform.GetComponent<CanvasClock>().stopwatch[i].Reset();
            canvasClock.transform.GetComponent<CanvasClock>().elapsedTime[i] = float.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text) * 60f;
        }

        // �������ԏ�����̍l������
        canvasClock.transform.GetComponent<CanvasClock>().considerTime = float.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text);

        // �T���}
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSanma").GetComponent<Toggle>().isOn)
        {
            CanvasClock.playersNum = 3;
            PlayerPrefs.SetInt(KEY_PLAYERS_NUM, 3);
            // �T���}�Ȃ��Ƃ̃{�^���A���b�Z�[�W��S�Ĕ�\��
            // �{�^���A��������
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(false);      // �Ŕv�p�l��
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(false);     // �����p�l��  
            // ���b�Z�[�W
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(false);     // �u�e���{�^���������ĊJ�n���Ă��������v
        } else
        {
            CanvasClock.playersNum = 4;
            PlayerPrefs.SetInt(KEY_PLAYERS_NUM, 4);
            // �����}�Ȃ��Ƃ̃{�^���A���b�Z�[�W��S�ĕ\��
            // �{�^���A��������
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(true);      // �Ŕv�p�l��
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(true);     // �����p�l��  
            // ���b�Z�[�W
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(true);     // �u�e���{�^���������ĊJ�n���Ă��������v
        }

        // �t�B�b�V���[���[��
        canvasClock.transform.GetComponent<CanvasClock>().isFischer = canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn;
        canvasClock.transform.GetComponent<CanvasClock>().fischerSec = float.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text);

        // �ݒ�l��PlayerPrefs�ŕۑ�
        SetPlayerPrefs();

        // �V�K�Q�[��
        canvasClock.transform.GetComponent<CanvasClock>().Init();

        canvasConfig.transform.gameObject.SetActive(false);
    }

    // �f�t�H���g�ɖ߂��{�^��
    public void ButtonDefault()
    {
        DispMsgBox(MsgId.SET_DEFAULT);
    }

    // ���b�Z�[�W�{�b�N�X�\������
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
                textMsg = "�������Ԃ����Z�b�g���ĐV�����Q�[�����J�n���܂��B\n��낵���ł����H";
                buttonYes = true;
                buttonNo = true;
                buttonOk = false;
                break;
            case MsgId.SET_DEFAULT:
                textMsg = "�ݒ�������l�ɖ߂��܂��B\n��낵���ł����H";
                buttonYes = true;
                buttonNo = true;
                buttonOk = false;
                break;
            case MsgId.WARN_ZERO_CONSIDER:
                textMsg = "��������0�b�̃v���C���[������ꍇ�͍l�����Ԃ�0�b�ɂł��܂���B\n�l�����Ԃ������l�ɐݒ肵�܂����B";
                buttonYes = false;
                buttonNo = false;
                buttonOk = true;
                break;
            default:
                textMsg = "OK�{�^���������Ă�������";
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

    // ����{�^��
    public void ButtonClose()
    {
#if ENABLE_ADMOB
        adBannerManager.BannerHide();
#endif

        // ���͒l�̃`�F�b�N
        if (!ChkInputValue())
        {
            return;
        }

        // �ݒ�l��PlayerPrefs�ŕۑ�
        SetPlayerPrefs();

            // �X�g�b�v�E�H�b�` �X�^�[�g
        if (canvasClock.transform.GetComponent<CanvasClock>().mode == CanvasClock.MODE.PLAY)
        {
            canvasClock.transform.GetComponent<CanvasClock>().stopwatch[canvasClock.transform.GetComponent<CanvasClock>().turnPlayer].Start();
        }

        canvasConfig.transform.gameObject.SetActive(false);
    }

    // ���͒l�̃`�F�b�N
    public bool ChkInputValue()
    {
        string text;

        // --- �������� ---

        for (int i = 0; i < CanvasClock.playersNum; i++)
        {
            // �󗓂Ȃ�f�t�H���g�l���Z�b�g
            text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text;
            if (text == "")
            {
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = DEF_ELAPSED_MIN.ToString();
            }

            // 0�`59��
            if (int.Parse(text) < 0)
            {
                // 0���ȉ���0���ɃZ�b�g
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = "0";
            } 
            else if (int.Parse(text) > 59)
            {
                // 59���z����59���ɃZ�b�g
                canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text = "59";
            }
        }

        // --- �������ԏ�����̍l������ ---

        text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text;
        // �󗓂Ȃ�f�t�H���g�l���Z�b�g
        if (text == "")
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = DEF_CONSIDER_SEC.ToString();
        }

        // 0�`30�b
        if (int.Parse(text) == 0)
        {
            // ��������0�b�̃v���C���[�����邩�`�F�b�N
            for (int i = 0; i < CanvasClock.playersNum; i++)
            {
                text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text;
                if (int.Parse(text) == 0)
                {
                    // ��������0�b�̃v���C���[���ꍇ�A�l�����Ԃ�0�b�ɂł��Ȃ��̂Ńf�t�H���g�l���Z�b�g
                    canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = DEF_CONSIDER_SEC.ToString();
                    // ���b�Z�[�W�{�b�N�X�\��
                    DispMsgBox(MsgId.WARN_ZERO_CONSIDER);
                    return false;
                }
            }
        }
        else if (int.Parse(text) < 0)
        {
            // 0�b�ȉ���0�b�ɃZ�b�g
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = "0";
        }
        else if (int.Parse(text) > 30)
        {
            // 30�b�z����30�b�ɃZ�b�g
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text = "30";
        }

        // --- �t�B�b�V���[���[�� ---

        text = canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text;
        // �󗓂Ȃ�f�t�H���g�l���Z�b�g
        if (text == "")
        {
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = DEF_FISCHER_SEC.ToString();
        }

        // 1�`30�b
        if (int.Parse(text) < 1)
        {
            // 1�b������1�b�ɃZ�b�g
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = "1";
        } else if (int.Parse(text) > 30)
        {
            // 30�b�z����30�b�ɃZ�b�g
            canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text = "30";
        }

        return true;
    }

    // �ݒ�l��PlayerPrefs�ŕۑ�
    private void SetPlayerPrefs()
    {
        // ��������
        //for (int i = 0; i < CanvasClock.playersNum; i++)
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.SetInt(KEY_ELAPSED_MIN_ + i, int.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldET" + i).GetComponent<InputField>().text));
        }

        // �Œ���c�莞��
        PlayerPrefs.SetInt(KEY_CONSIDER_SEC, int.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldConsiderTime").GetComponent<InputField>().text));

        // �������Ԉꗥ�i�n���f�Ȃ��j
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleSameTime").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt(KEY_SAMETIME_BOOL, (int)Bool.TRUE);
        } else {
            PlayerPrefs.SetInt(KEY_SAMETIME_BOOL, (int)Bool.FALSE);
        }

        // �t�B�b�V���[���[��
        if (canvasConfig.transform.Find("PanelConfig").Find("ToggleFischer").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt(KEY_FISCHER_BOOL, (int)Bool.TRUE);
        } else {
            PlayerPrefs.SetInt(KEY_FISCHER_BOOL, (int)Bool.FALSE);
        }
        PlayerPrefs.SetInt(KEY_FISCHER_SEC, int.Parse(canvasConfig.transform.Find("PanelConfig").Find("InputFieldFischerSec").GetComponent<InputField>().text));

        // ���ʉ��E�{�C�X
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


    // --- ���b�Z�[�W�{�b�N�X�p�{�^�� ---

    // �͂��{�^��
    public void ButtonYes()
    {
        canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(false);

        switch (msgId)
        {
            case MsgId.NEW_GAME:
                // ���͒l�̃`�F�b�N
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

    // �������{�^��
    public void ButtonNo()
    {
        canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(false);
    }

    // OK�{�^��
    public void ButtonOk(int n)
    {
        if (n == 0)
        {
            // ���b�Z�[�W�{�b�N�X
            canvasConfig.transform.Find("PanelMsgBox").gameObject.SetActive(false);
        } else if (n == 1)
        {
            // �X�y�V�����T���N�X
            canvasConfig.transform.Find("PanelSpecialThanks").gameObject.SetActive(false);
        }
    }

    // SpecialThanks�{�^��
    public void ButtonThanks()
    {
        canvasConfig.transform.Find("PanelSpecialThanks").gameObject.SetActive(true);
    }

}
