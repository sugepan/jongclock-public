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
    public AudioClip dahaiSE;   // �Ŕv�̌��ʉ�
    public AudioClip nakiSE;    // ���̌��ʉ�
    public AudioClip agariSE;   // �a���̌��ʉ�
    public AudioClip timeUpSE;  // ���Ԑ؂�̌��ʉ�
    public AudioClip startSE;   // �΋ǊJ�n�̌��ʉ�
    public AudioClip[] cdGirlSE = new AudioClip[11];  // ���C�ȏ��̎q�̃J�E���g�_�E��

    public enum MODE   // ���[�h��`
    {
        READY,                  // �J�n�O
        PLAY,                   // �v���C��
        AGARI,                  // �a��
        RYUKYOKU,               // ����
        TIMEUP,                 // ���Ԑ؂�
        PAUSE,                  // �ꎞ��~
    };
    public MODE mode = MODE.READY;

    enum STATUS     // ��Ԓ�`
    {
        START,
        DAHAI,
        CHII,
        PONKAN,
    };

    // �l������
    public float considerTime;

    // �t�B�b�V���[���[��
    public bool isFischer;
    public float fischerSec;  // 1��ő�����b��

    // �ő�Ŕv��
    const int MAX_DAHAI_CNT = 70;
    int dahaiCnt = 0;

    private AudioSource audioSource;    // �I�[�f�B�I�\�[�X

    private int[] cdFlg = new int[] { 10, 10, 10, 10 };  // �J�E���g�_�E���t���O

    public static int playersNum;     // �v���C���[�l��

    public int turnPlayer;     // �N�̔Ԃ�

    public float[] elapsedTime = new float[4]; // �c�莞��
    List<float>[] elapsedTimeHistory = new List<float>[4];      // �c�莞�Ԃ̗���
    public System.Diagnostics.Stopwatch[] stopwatch = new System.Diagnostics.Stopwatch[4];  // �X�g�b�v�E�H�b�`
    bool[] boolBtnNaki = new bool[] {true, true, true, true};
    bool[] boolBtnAgari = new bool[] { true, true, true, true};

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();

        // �v���C���[�l��
        playersNum = PlayerPrefs.GetInt(CanvasConfig.KEY_PLAYERS_NUM, 4);

        for (int i = 0; i < 4; i++)
        {
            // �X�g�b�v�E�H�b�` ������
            stopwatch[i] = new System.Diagnostics.Stopwatch();

            // �c�莞�� ������
            elapsedTime[i] = PlayerPrefs.GetInt(CanvasConfig.KEY_ELAPSED_MIN_ + i, CanvasConfig.DEF_ELAPSED_MIN) * 60f;
            // �l������ ������
            considerTime = PlayerPrefs.GetInt(CanvasConfig.KEY_CONSIDER_SEC, CanvasConfig.DEF_CONSIDER_SEC);
            // �t�B�b�V���[���[��
            isFischer = PlayerPrefs.GetInt(CanvasConfig.KEY_FISCHER_BOOL, CanvasConfig.DEF_FISCHER_BOOL ? (int)CanvasConfig.Bool.TRUE : (int)CanvasConfig.Bool.FALSE) == (int)CanvasConfig.Bool.TRUE ? true : false;
            fischerSec = PlayerPrefs.GetInt(CanvasConfig.KEY_FISCHER_SEC, CanvasConfig.DEF_FISCHER_SEC);  // 1��ő�����b��
        }

        // ������
        Init();

    }

    // Update is called once per frame
    void Update()
    {

        // �v���C��
        if (mode == MODE.PLAY)
        {

            //timeSpan = TimeSpan.FromSeconds(elapsedTime[turnPlayer] - (float)stopwatch[turnPlayer].Elapsed.TotalSeconds);
            if (GetElapsedTimeFloat(turnPlayer) <= 0f)
            {
                // ���Ԑ؂�
                mode = MODE.TIMEUP;

                PlayOneShot(timeUpSE, nameof(timeUpSE));  // ���Ԑ؂�̌��ʉ�

                canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").Find("ImageTimeUp").gameObject.SetActive(true);  // TIMEUP�}�[�N

                stopwatch[turnPlayer].Stop();   // �X�g�b�v�E�H�b�` �X�g�b�v

                // ��ԃv���C���[�̃{�^���������Ȃ�����
                canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").GetComponent<Button>().interactable = false;  // �Ŕv�{�^��
                canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonNaki").GetComponent<Button>().interactable = false;  // ���{�^��
                canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false; // �a���{�^��

                // �S���̖��{�^���������Ȃ�����
                for (int i = 0; i < 4; i++)
                {
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;  // ���{�^��
                }

                elapsedTime[turnPlayer] = 0f;
                stopwatch[turnPlayer].Reset();   // �X�g�b�v�E�H�b�` ���Z�b�g

                // 5�b��Ɏ�ԃv���C���[�̑Ŕv�{�^�����Ċ���������
                StartCoroutine(DelayEnableDahaiCorutine());
            }
            
            // �c�莞�Ԃ̕����F�ݒ�
            //SetTextElapsedTimeColor(canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("TextElapsedTime").gameObject);
            
            // �S�v���C���[ �c�莞�ԕ\��
            //TimeSpan timeSpan;
            for (int i = 0; i < playersNum; i++)
            {
                // �c�莞�Ԃ��ĕ\��
                DisplayElapsedTime(i);
                //canvasClock.transform.Find("PanelDahai" + i).transform.Find("TextElapsedTime").GetComponent<Text>().text = GetElapsedTimeMMSS(i);
            }

            // �J�E���g�_�E���{�C�X
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

            // �t���O������
            if (GetElapsedTimeFloat(turnPlayer) > 11f)
            {
                cdFlg[turnPlayer] = 10;
            }

        }

    }

    // �Ŕv�{�^��������������
    IEnumerator DelayEnableDahaiCorutine()
    {
        // second�Ŏw�肵���b�����[�v���܂�
        yield return new WaitForSeconds(5.0f);

        canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").GetComponent<Button>().interactable = true;       // �Ŕv�{�^��

    }

    // ������
    public void Init()
    {
        for (int i = 0; i < playersNum; i++)
        {
            // �Ŕv�� ������
            dahaiCnt = 0;

            // �J�E���g�_�E���t���O ������
            cdFlg[i] = 10;

            // ���A�a���{�^�� Enable/Disable�̏�����
            boolBtnNaki[i] = true;
            boolBtnAgari[i] = true;

            // TIMEUP�}�[�N ��\��
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").Find("ImageTimeUp").gameObject.SetActive(false);  

            // �c�莞�Ԃ��l�����Ԉȉ��Ȃ�l�����Ԃ܂ő��₷
            if (GetElapsedTimeFloat(i) < considerTime)
            {
                elapsedTime[i] = considerTime;
                cdFlg[i] = (int)considerTime - 1;  // �J�E���g�_�E���t���O���ď�����
                stopwatch[i].Reset();
            }


            // �c�莞�ԕ\��
            DisplayElapsedTime(i);

            // �傫���i�ʏ�j
            canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(1, 1);
            // �\��
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonStart").gameObject.SetActive(true);     // �J�n(�e)�{�^��
            // ��\��
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").gameObject.SetActive(false);  // �Ŕv�{�^��
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").gameObject.SetActive(false);  // ���{�^��
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").gameObject.SetActive(false); // �a���{�^��  
            canvasClock.transform.Find("PanelKaze" + i).gameObject.SetActive(false); // �����\��
            
        }

        // �T���}
        if (playersNum == 3)
        {
            // �T���}�Ȃ��Ƃ̃{�^���A���b�Z�[�W��S�Ĕ�\��
            // �{�^���A��������
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(false);      // �Ŕv�p�l��
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(false);     // �����p�l��  
            canvasClock.transform.Find("PanelKaze3").gameObject.SetActive(false);       // ���p�l��  
            // ���b�Z�[�W
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(false);     // �u�e���{�^���������ĊJ�n���Ă��������v
        } else
        {
            // �����}�Ȃ��Ƃ̃{�^���A���b�Z�[�W��S�ĕ\��
            // �{�^���A��������
            canvasClock.transform.Find("PanelDahai3").gameObject.SetActive(true);      // �Ŕv�p�l��
            canvasClock.transform.Find("PanelHassei3").gameObject.SetActive(true);     // �����p�l��  
            //canvasClock.transform.Find("PanelKaze3").gameObject.SetActive(true);       // ���p�l��  
            // ���b�Z�[�W
            canvasClock.transform.Find("PanelStartMsg").Find("Text (Legacy) (3)").gameObject.SetActive(true);     // �u�e���{�^���������ĊJ�n���Ă��������v
        }

        // �u�e���{�^���������ĊJ�n���Ă��������v�\��
        canvasClock.transform.Find("PanelStartMsg").gameObject.SetActive(true);

    }

    // �Ŕv�{�^��
    public void ButtonDahai(int n)
    {
        PlayOneShot(dahaiSE, nameof(dahaiSE));  // �Ŕv�̌��ʉ�

        // ���A�a���{�^�� Enable/Disable�̏�����
        for (int i = 0; i < playersNum; i++)
        {
            boolBtnNaki[i] = true;
            boolBtnAgari[i] = true;
        }

        // �v���C�ĊJ
        if (mode == MODE.TIMEUP)
        {
            // ���Ԑ؂�Ȃ̂ɑŔv�{�^����������Ƃ������Ƃ́A���Ԑ؂ꂩ��̍ĊJ
            canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("ButtonDahai").Find("ImageTimeUp").gameObject.SetActive(false);  // TIMEUP�}�[�N ��\��
            mode = MODE.PLAY;
        }

        stopwatch[n].Stop();

        dahaiCnt++;

        // �t�B�b�V���[���[��
        if (isFischer)
        {
            elapsedTime[n] += fischerSec;
            // �t�B�b�V���[�b�����������1���Ԃ𒴂���
            if (GetElapsedTimeFloat(n) > 60f * 60f)
            {
                elapsedTime[n] = 60f * 60f - 1f;    // �����59��59�b
                stopwatch[n].Reset();
            }
            cdFlg[n] += (int)fischerSec;
        }

        Debug.Log("elapsedTime[n]=" + elapsedTime[n]);
        Debug.Log("stopwatch[n].Elapsed.TotalSeconds=" + stopwatch[n].Elapsed.TotalSeconds);
        Debug.Log("GetElapsedTimeFloat(n)=" + GetElapsedTimeFloat(n));
        Debug.Log("minElapsedTime=" + considerTime);

        // �c�莞�Ԃ��l�����Ԉȉ��Ȃ�l�����Ԃ܂ő��₷
        if (GetElapsedTimeFloat(n) < considerTime)
        {
            elapsedTime[n] = considerTime;
            cdFlg[n] = (int)considerTime - 1;
            stopwatch[n].Reset();
        }

        // �c�莞�Ԑݒ�
        DisplayElapsedTime(n);

        // ���v���C���[
        int nextPlayer = (n + 1) % playersNum;

        // ���v���C���[��Ԃ̕\��
        DisplayCurrent(nextPlayer, STATUS.DAHAI);

        // ���v���C���[�̎c�莞�Ԍ����̑O�� �������L�^
        elapsedTimeHistory[nextPlayer].Add(elapsedTime[nextPlayer] - (float)stopwatch[nextPlayer].Elapsed.TotalSeconds);

        stopwatch[nextPlayer].Start();
        turnPlayer = nextPlayer;
        
    }


    // ���{�^��
    public void ButtonNaki(int n)
    {
        PlayOneShot(nakiSE, nameof(nakiSE));  // ���̌��ʉ�

        if (n == turnPlayer)
        {
            // �`�[
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonNaki").GetComponent<Button>().interactable = false;
            boolBtnNaki[turnPlayer] = false;
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false;
            boolBtnAgari[turnPlayer] = false;
            return;
        }
        else
        {
            // �|���A�~���J��
            stopwatch[turnPlayer].Stop();   // ��ԃX�g�b�v
            stopwatch[turnPlayer].Reset();  // �o�ߎ��ԃ��Z�b�g

            // �^�[���v���C���[�̎c�莞�Ԃ𗚗�����߂�
            ReverseElapsedTime();

            // �c�莞�Ԃ��ĕ\��
            DisplayElapsedTime(turnPlayer);

            // �J�E���g�_�E���t���O��������
            cdFlg[turnPlayer] = (int)considerTime - 1;

            turnPlayer = n;         // ��ԃv���C���[�ύX
            stopwatch[turnPlayer].Start();   // �J�E���g�_�E���J�n

            // ��ԃv���C���[�̕\��
            DisplayCurrent(turnPlayer, STATUS.PONKAN);
            // ���{�^����S�ĕs�����i�|���A�J�����D��̖��͂Ȃ��j
            for (int i = 0; i < playersNum; i++)
            {
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;
                boolBtnNaki[i] = false;
            }
            // �����v���C���[�͘a���{�^�����s����
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false;
                boolBtnAgari[turnPlayer] = false;
        }

    }

    // �a���{�^��
    public void ButtonAgari(int n)
    {
        mode = MODE.AGARI;

        if (n == turnPlayer)
        {
            // �c��
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonNaki").GetComponent<Button>().interactable = false;
            canvasClock.transform.Find("PanelHassei" + turnPlayer).Find("ButtonAgari").GetComponent<Button>().interactable = false;
        }
        else
        {
            // ����
            stopwatch[turnPlayer].Stop();   // ��ԃX�g�b�v
            stopwatch[turnPlayer].Reset();  // �o�ߎ��ԃ��Z�b�g

            // �^�[���v���C���[�̎c�莞�Ԃ𗚗�����߂�
            ReverseElapsedTime();

            // �c�莞�Ԃ��ĕ\��
            DisplayElapsedTime(turnPlayer);
            //canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("TextElapsedTime").GetComponent<Text>().text = GetElapsedTimeMMSS(turnPlayer);
            //SetTextElapsedTimeColor(canvasClock.transform.Find("PanelDahai" + turnPlayer).Find("TextElapsedTime").gameObject);
        }

        PlayOneShot(agariSE, nameof(agariSE));  // �a���̌��ʉ�

        stopwatch[n].Stop();   // �X�g�b�v�E�H�b�` �X�g�b�v

        // �s����
        canvasClock.transform.Find("PanelHassei" + n).Find("ButtonAgari").GetComponent<Button>().interactable = false;   // �a���{�^��

        // �S�ĕs����
        for (int i = 0; i < playersNum; i++)
        {
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = false;   // �Ŕv�{�^��
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // ���{�^��
        }
        
        // �\��
        canvasClock.transform.Find("ButtonNextStage").gameObject.SetActive(true);  // ���ǃ{�^��
        canvasClock.transform.Find("ButtonNextStage").SetAsLastSibling();

    }

    // �X�^�[�g�{�^��
    public void ButtonStart(int n)
    {
        turnPlayer = n;     // ��ԃv���C���[
        string kaze;
        Color32 kazeColor;

        // �u�e���{�^���������ĊJ�n���Ă��������v��\��
        canvasClock.transform.Find("PanelStartMsg").gameObject.SetActive(false);

        for (int i = 0; i < playersNum; i++)
        {
            // �c�莞��
            elapsedTimeHistory[i] = new List<float>();
            elapsedTimeHistory[i].Add(elapsedTime[i]);          // ���������l
            // Disable
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonStart").gameObject.SetActive(false);   // �J�n(�e)�{�^��
            // Enable
            canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").gameObject.SetActive(true);  // �Ŕv�{�^��
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").gameObject.SetActive(true);    // ���{�^��
            canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").gameObject.SetActive(true);   // �a���{�^��    
            canvasClock.transform.Find("PanelKaze" + i).gameObject.SetActive(true); // �����\��
            switch (i)
            {
                case 0:
                    kaze = "��";
                    kazeColor = new Color32(255, 0, 0, 255);    // �ԕ���
                    break;
                case 1:
                    kaze = "��";
                    kazeColor = new Color32(255, 255, 255, 255);  // ������
                    break;
                case 2:
                    kaze = "��";
                    kazeColor = new Color32(255, 255, 255, 255);  // ������
                    break;
                case 3:
                    kaze = "�k";
                    kazeColor = new Color32(255, 255, 255, 255);  // ������
                    break;
                default:
                    kaze = "��";
                    kazeColor = new Color32(255, 0, 0, 255);    // �ԕ���
                    break;
            }
            canvasClock.transform.Find("PanelKaze" + ((i + n) % playersNum)).Find("TextKaze").GetComponent<Text>().text = kaze;
            canvasClock.transform.Find("PanelKaze" + ((i + n) % playersNum)).Find("TextKaze").GetComponent<Text>().color = kazeColor;
        }
        
        stopwatch[turnPlayer].Start();

        PlayOneShot(startSE, nameof(startSE));  // �΋ǊJ�n�̌��ʉ�

        // �{�^���̕\���E����
        DisplayCurrent(turnPlayer, STATUS.START);

        mode = MODE.PLAY;
    }

    // ���ǃ{�^��
    public void ButtonNextStage()
    {
        canvasClock.transform.Find("ButtonNextStage").gameObject.SetActive(false);
        Init();
    }

    // 3�����[�_�[
    public void Button3LineLeader()
    {
        stopwatch[turnPlayer].Stop();

        // �ݒ��� ������
        canvasConfig.transform.GetComponent<CanvasConfig>().Init();

        // �o�[�W�����\��
        canvasConfig.transform.Find("PanelConfig").Find("TextTitle").Find("TextVersion").GetComponent<Text>().text = "Ver." + Application.version;

        // �ݒ��ʕ\��
        canvasConfig.transform.gameObject.SetActive(true);

#if ENABLE_ADMOB
        canvasConfig.transform.GetComponent<AdBannerManager>().bannerView.Show();
#endif

    }


    // ���݂̏�Ԃ���{�^���̕\���E��\���A�����E�s������ݒ�
    private void DisplayCurrent(int n, STATUS status)
    {
        for (int i = 0; i < playersNum; i++)
        {
            // �c�莞�ԕ\��
            DisplayElapsedTime(i);

            if (i == n)
            {
                // ��ԃv���C���[
                canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = true;    // �Ŕv�{�^��
                canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(2, 2);
                canvasClock.transform.Find("PanelDahai" + i).SetAsLastSibling();
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = true;    // ���{�^��
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = true;   // �a���{�^��
                if (status == STATUS.START)
                {
                    // �J�ǒ���
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;    // ���{�^��
                    boolBtnNaki[i] = false;
                }
            }
/*            else if(i == (n + 1) % 4)
            {
                // ��ԃv���C���[�̉���
            }
            else if(i == (n + 1) % 4)
            {
                // ��ԃv���C���[�̑Ή�
            }*/
            else if(i == (n + (playersNum - 1)) % playersNum)
            {
                // ��ԃv���C���[�̏��
                canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = false;   // �Ŕv�{�^��
                canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(1, 1);
                if (status == STATUS.CHII)
                {
                    // �`�[�Ȃ��Ƃ͖��A�a���{�^���͉����Ȃ�
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // ���{�^��
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // �a���{�^��
                    boolBtnAgari[i] = false;
                }
                else if (status == STATUS.PONKAN)
                {
                    // �|���J���͏�Ƃ̓`�[���ĂȂ���Θa���{�^����������
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = boolBtnNaki[i];  // �a���{�^��
                }
                else if (status == STATUS.DAHAI)
                {
                    // ���ʂɏ��Ԃ������̂Ȃ�A���A�a���{�^���͉����Ȃ�
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // ���{�^��
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // �a���{�^��
                    boolBtnAgari[i] = false;
                }

                if (status == STATUS.START)
                {
                    // �J�ǒ���
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // ���{�^��
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // �a���{�^��
                    boolBtnAgari[i] = false;
                }
            }
            else
            {
                // ��Ԃ���Ȃ��v���C���[
                canvasClock.transform.Find("PanelDahai" + i).Find("ButtonDahai").GetComponent<Button>().interactable = false;   // �Ŕv�{�^��
                canvasClock.transform.Find("PanelDahai" + i).transform.localScale = new Vector2(1, 1);
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = boolBtnNaki[i];    // ���{�^��
                canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = boolBtnAgari[i];   // �a���{�^��
                if (status == STATUS.START)
                {
                    // �J�ǒ���
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonNaki").GetComponent<Button>().interactable = false;   // ���{�^��
                    boolBtnNaki[i] = false;
                    canvasClock.transform.Find("PanelHassei" + i).Find("ButtonAgari").GetComponent<Button>().interactable = false;  // �a���{�^��
                    boolBtnAgari[i] = false;
                }
            }

        }
    }

    // �c�莞�ԕ\��
    private void DisplayElapsedTime(int n)
    {
        canvasClock.transform.Find("PanelDahai" + n).Find("TextElapsedTime").GetComponent<Text>().text = GetElapsedTimeMMSS(n);
        SetTextElapsedTimeColor(canvasClock.transform.Find("PanelDahai" + n).Find("TextElapsedTime").gameObject);
    }

    // �^�[���v���C���[�̎c�莞�Ԃ𗚗�����߂�
    private void ReverseElapsedTime()
    {
        if (elapsedTimeHistory[turnPlayer].Count > 0)
        {
            elapsedTime[turnPlayer] = elapsedTimeHistory[turnPlayer].Last();
            elapsedTimeHistory[turnPlayer].RemoveAt(elapsedTimeHistory[turnPlayer].Count - 1);
        }
    }

    // �c�莞�Ԃ̕����F�ݒ�
    private void SetTextElapsedTimeColor(GameObject textElapsedTimeObj)
    {
        string text = textElapsedTimeObj.GetComponent<Text>().text;
        int sec;

        // �c�莞��1���ȏ�
        if (text.Substring(0, 2) != "00")
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 255, 255, 255);  // ������
            return;
        }

        // �c��b��
        sec = int.Parse(text.Substring(text.Length - 2));

        // �c�莞�Ԃ̕����F
        if (sec <= 5)
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 0, 0, 255);  // �ԕ���
        } else if (sec <= 10)
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 255, 0, 255);  // ������
        } else
        {
            textElapsedTimeObj.GetComponent<Text>().color = new Color32(255, 255, 255, 255);  // ������
        }
    }

    // �c�莞�Ԏ擾�imm:ss�j
    private string GetElapsedTimeMMSS(int player)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime[player] - (float)stopwatch[player].Elapsed.TotalSeconds);
        string ret = canvasClock.transform.Find("PanelDahai" + player).Find("TextElapsedTime").GetComponent<Text>().text = timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2");

        return ret;
    }

    // �c�莞�Ԏ擾�ifloat�j
    private float GetElapsedTimeFloat(int player)
    {
        return elapsedTime[player] - (float)stopwatch[player].Elapsed.TotalSeconds;
    }

    // ���ʉ��E�{�C�X��炷
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
            audioSource.PlayOneShot(audioClip);   // ���ʉ��E�{�C�X
        }

        Debug.Log("isSnd = " + isSnd);

    }

    

}
