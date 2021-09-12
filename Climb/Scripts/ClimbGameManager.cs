using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ClimbGameManager : MonoBehaviour
{
    #region 애니메이션
    public Animator anim;
    public GameObject player;
    #endregion

    #region 슬라이더
    public Slider slider;
    public static float sMin, sMax, sVal, sMid;
    bool cond1; // 왼쪽 슬라이더 
    bool cond2; // 오른쪽 슬라이더
    #endregion

    #region 게임 난이도 선택
    // 배경 sprite 변경
    public Sprite _easy;
    public Sprite _normal;
    public Sprite _hard;
    public SpriteRenderer _wall;

    public GameObject selectPanel;  // 난이도 선택 판넬
    public static int stage;   // 난이도(1, 2, 3)
    public GameObject easy;
    public GameObject normal;
    public GameObject hard;
    public GameObject FakeHold;
    int r;

    #endregion

    #region UI들
    public GameObject pausePanel;   // 일시정지 
    public GameObject resultPanel;  // 결과화면
    public GameObject readyPanel;   // 게임준비(설명, 카운트다운)
    public GameObject warning;  // 목표 각도 달성 후 0도로 돌려놔야 할 때 띄울 Panel
    public GameObject pauseWarning; // 게임 중지 시 나오는 경고 Panel
    public Text countdown_txt;  // 카운트다운
    public Text playtime_txt;   // 플레이타임
    public Text finishtime_txt; // 클리어타임
    public Text score_txt;  // 점수
    public GameObject highscore;

    public static float playtime; // 플레이타임
    public int score;   // 점수
    public int _success;
    public static int grabbed; // 올바른 홀드 잡은 횟수
    public static bool isGrabbed;  // 

    #endregion

    #region 상지가동범위 newOne
    public GameObject circleGraph;

    float left_max;
    float left_min;
    float right_max;
    float right_min;

    #endregion

    #region DB 변수
    public Text MaxScore_Text;
    public Text test;
    public Text test_Result;
    public int maxScore;
    public string date;
    public string userID;
    public string gameID;
    public string sql;
    #endregion

    #region 카메라 이동
    public GameObject P;
    Transform PT;
    public Camera C;
    Transform CT;
    bool isDone;

    #endregion

    #region 튜토리얼 캐릭터
    public GameObject tutor;
    public Image face;
    public Image face2;
    public Sprite smile;
    public Sprite umm;
    public Sprite hmm;
    public Sprite lookright;
    #endregion

    #region 말풍선
    public GameObject scriptBox;
    public Text[] txt;
    public int idx;
    public GameObject btn;
    public GameObject finish_btn;
    #endregion

    public Button pause_btn;
    public GameObject emphasize_pause;
    public GameObject emphasize_time;
    public GameObject arrow1;
    public GameObject arrow2;

    public GameObject tutorial_holds;
    public GameObject init_hold;
    public GameObject tutorial1;
    public GameObject tutorial2;
    public GameObject scoretut;

    bool isPractice;
    bool timerStart;
    public static bool isTutorial;
    public bool isStageSelected;

    #region 게이지
    // 게이지
    public GameObject gauge;
    public Image leftCircle_Red, rightCircle_Red, leftCircle_Blue, rightCircle_Blue;
    public Text gaugeText;
    bool isMax;
    public bool squeeze;

    public GameObject startPanel;
    public Text startText;
    public GameObject gamePanel;
    bool isCali;
    public Image sLeftCircle_Red, sRightCircle_Red;
    public Slider sSlider;
    public GameObject countObject;

    // check : 0 외전 / check : 1 영점 / check : 2 내전
    public int check;
    #endregion

    #region Singleton
    public static ClimbGameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        date = DBManager.SqlFormat(date);
        userID = "'userID'";
        gameID = "'11climb'";
        maxScore = 0;
        MaxScore_Text.text = "최고점수 : " + maxScore;
    }
    #endregion

    enum Pos
    {
        up,
        right
    };
    List<Pos> route = new List<Pos>();
    public int row, col;
    public GameObject prevHold;  // 시작홀드 받아옴
    public GameObject holdPrefab;
    public GameObject holdParent;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.0f;
        Debug.Log(isTutorial);

        /*// Serial 운동 설정
        if (Data.Instance.GameID.Substring(0, 1) == "1") // 능동운동
            Serial.instance.Active();
        else if (Data.Instance.GameID.Substring(0, 1) == "4") // 저항운동
            Serial.instance.Resistance();*/

        if (isTutorial)
        {
            stage = 1;
            idx = 0;
            tutorial1.SetActive(true);
            scriptBox.SetActive(true);
            txt[idx].gameObject.SetActive(true);
            idx++;

            Time.timeScale = 1.0f;
            finish_btn.SetActive(false);
            pause_btn.interactable = false;
            scoretut.SetActive(false);
            init_hold.SetActive(true);
            highscore.SetActive(false);

            isPractice = false;
            timerStart = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        cond1 = false;
        cond2 = false;

        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        resultPanel.SetActive(false);
        countdown_txt.gameObject.SetActive(false);

        PT = P.transform;
        CT = C.transform;

        isDone = false;

        playtime = 0f;
        score = 100;
        grabbed = 0;
        isGrabbed = false;
        isStageSelected = false;

        sMin = slider.minValue;
        sMax = slider.maxValue;
        sMid = (sMax - sMin) / 2;
        slider.value = sMid;

        isMax = false;
        squeeze = false;
        check = 1;

        gauge.SetActive(false);
        leftCircle_Red = gauge.transform.GetChild(1).GetComponent<Image>();
        rightCircle_Red = gauge.transform.GetChild(2).GetComponent<Image>();
        leftCircle_Blue = gauge.transform.GetChild(3).GetComponent<Image>();
        rightCircle_Blue = gauge.transform.GetChild(4).GetComponent<Image>();
        leftCircle_Red.fillAmount = 0f;
        rightCircle_Red.fillAmount = 0f;
        leftCircle_Blue.fillAmount = 0f;
        rightCircle_Blue.fillAmount = 0f;

        if (!isTutorial)
        {
            timerStart = true;
            startPanel.SetActive(true);
            isCali = true;
            Time.timeScale = 1f;

            DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
            while (DBManager.dataReader.Read())
            {
                maxScore = DBManager.dataReader.GetInt32(8);
                break;
            }
            DBManager.DBClose();
        }
    }

    public float Normalize(float val, float max, float min)
    {
        print("정규화 : " + (val - min) / (max - min));
        print($"val : {val} / max : {max} / min : {min}");
        return Mathf.Abs((val - min) / (max - min));
    }

    IEnumerator Calibration()
    {
        countObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        startPanel.SetActive(false);
        selectPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        print("check : " + check);
        if (!(Serial.instance.angle > 250f || Serial.instance.angle < 4f))
        {
            // 초반에 중간값 맞추는 코드
            if (isCali)
            {
                sVal = sSlider.value;
                sMax = sSlider.maxValue;
                sMin = sSlider.minValue;
                sMid = (sSlider.maxValue - sSlider.minValue) / 2f;

                // 내전 -> 왼쪽 반원 붉은색으로 채워짐
                if (sVal > sMid)
                {
                    sRightCircle_Red.fillAmount = 0f;
                    sLeftCircle_Red.fillAmount = Normalize(sVal, sMax, sMid);
                    startText.text = "원이 하얀색이 되도록 기기를 돌려주세요,\n(반시계 방향으로!)";
                }
                // 중간값
                else if (sVal == sMid)
                {
                    sLeftCircle_Red.fillAmount = 0f;
                    sRightCircle_Red.fillAmount = 0f;
                }
                // 외전 -> 오른쪽 반원 붉은색으로 채워짐
                else
                {
                    sLeftCircle_Red.fillAmount = 0f;
                    sRightCircle_Red.fillAmount = 1 - Normalize(sVal, sMid, sMin);
                    startText.text = "원이 하얀색이 되도록 기기를 돌려주세요,\n(시계 방향으로!)";
                }

                if (sVal == sMid)
                {
                    isCali = false;
                    StartCoroutine("Calibration");
                }
            }

            // 본 게임 진행
            else
            {
                sMin = slider.minValue;
                sMax = slider.maxValue;
                sMid = (sMax - sMin) / 2;
                sVal = slider.value;
                
                // Min, Max 값 찍은 상태로 유지
                if (squeeze)
                {
                    leftCircle_Blue.fillAmount = 0f;
                    rightCircle_Blue.fillAmount = 0f;
                    print("cond1상태 : " + Player.cond1);
                    if (!Player.cond1)
                    {
                        if (slider.value > sMid)
                        {
                            gaugeText.text = "시계방향!";
                            check = 2;
                            rightCircle_Red.fillAmount = 0f;
                            leftCircle_Red.fillAmount = Normalize(slider.value, sMax, sMid);
                        }
                        else if (slider.value == sMid)
                        {
                            check = 1;
                            leftCircle_Red.fillAmount = 0f;
                            rightCircle_Red.fillAmount = 0f;
                        }
                        else if (slider.value < sMid)
                        {
                            check = 0;
                            gaugeText.text = "반시계방향!";
                            rightCircle_Red.fillAmount = 0f;
                        }
                    }
                    else
                    {
                        leftCircle_Red.fillAmount = 1f;
                        if (slider.value > sMid)
                        {
                            gaugeText.text = "시계방향!";
                            check = 2;
                            rightCircle_Red.fillAmount = 0f;
                        }
                        else if (slider.value == sMid)
                        {
                            check = 1;
                            leftCircle_Red.fillAmount = 0f;
                            rightCircle_Red.fillAmount = 0f;
                        }
                        else if (slider.value < sMid)
                        {
                            check = 0;
                            gaugeText.text = "반시계방향!";
                            rightCircle_Red.fillAmount = 1 - Normalize(slider.value, sMid, sMin);
                        }
                    }
                }

                // 그냥 이동만 할 때
                else
                {
                    // 내전 -> 왼쪽 반원 붉은색으로 채워짐
                    if (sVal > sMid)    // S.i.angle이 maxValue 넘어가면 씹음
                    {
                        check = 2;
                        gaugeText.text = "시계방향!";
                        if (isMax)
                        {
                            leftCircle_Red.fillAmount = 0f;
                            rightCircle_Red.fillAmount = 0f;
                            rightCircle_Blue.fillAmount = 0f;
                            leftCircle_Blue.fillAmount = Normalize(slider.value, sMax, sMid);
                        }
                        else
                        {
                            leftCircle_Blue.fillAmount = 0f;
                            rightCircle_Blue.fillAmount = 0f;
                            rightCircle_Red.fillAmount = 0f;
                            leftCircle_Red.fillAmount = Normalize(slider.value, sMax, sMid);
                        }
                    }
                    // 중간값
                    else if (sVal == sMid)
                    {
                        check = 1;
                        leftCircle_Red.fillAmount = 0f;
                        rightCircle_Red.fillAmount = 0f;
                        leftCircle_Blue.fillAmount = 0f;
                        rightCircle_Blue.fillAmount = 0f;
                    }
                    // 외전 -> 오른쪽 반원 붉은색으로 채워짐
                    else if (slider.value < sMid)
                    {
                        check = 0;
                        gaugeText.text = "반시계방향!";
                        if (isMax)
                        {
                            leftCircle_Red.fillAmount = 0f;
                            rightCircle_Red.fillAmount = 0f;
                            leftCircle_Blue.fillAmount = 0f;
                            rightCircle_Blue.fillAmount = 1 - Normalize(slider.value, sMid, sMin);
                        }
                        else
                        {
                            leftCircle_Blue.fillAmount = 0f;
                            rightCircle_Blue.fillAmount = 0f;
                            leftCircle_Red.fillAmount = 0f;
                            rightCircle_Red.fillAmount = 1 - Normalize(slider.value, sMid, sMin);
                        }

                    }
                }
                if (!isTutorial)
                {
                    tutorial1.SetActive(false);
                }

                if (!Player.isFinished && (isStageSelected || isTutorial))
                {
                    // 내전
                    if (sVal >= slider.maxValue && !cond1)
                    {
                        print("cond1 만족");
                        cond1 = true;
                        warning.SetActive(true);
                        // 왼쪽 붉은색 반원 비움
                        leftCircle_Red.fillAmount = 0f;
                        // 왼쪽 푸른색 반원 채움
                        leftCircle_Blue.fillAmount = 1f;
                        isMax = true;
                    }
                    // 왼쪽 슬라이더 값 다시 0으로 -> 위로 이동
                    else if (sMid - 5f <= sVal && sVal <= sMid + 5f && cond1)
                    {
                        isMax = false;
                        leftCircle_Blue.fillAmount = 0f;
                        SoundManager.instance.GrabSound();
                        anim.SetBool("uparrow", true);
                        cond1 = false;
                        isGrabbed = true;
                        warning.SetActive(false);
                    }
                    else
                    {
                        anim.SetBool("uparrow", false);
                    }

                    // 외전
                    if (sVal <= slider.minValue && !cond2)
                    {
                        print("cond2 만족");
                        cond2 = true;
                        warning.SetActive(true);
                        // 왼쪽 붉은색 반원 비움
                        rightCircle_Red.fillAmount = 0f;
                        // 왼쪽 푸른색 반원 채움
                        rightCircle_Blue.fillAmount = 1f;
                        isMax = true;
                    }
                    // 오른쪽 슬라이더 값 다시 0으로 -> 오른쪽으로 이동
                    else if (sMid - 5f <= sVal && sVal <= sMid + 5f && cond2)
                    {
                        isMax = false;
                        SoundManager.instance.GrabSound();
                        anim.SetBool("rightarrow", true);
                        cond2 = false;
                        isGrabbed = true;
                        warning.SetActive(false);
                        leftCircle_Red.fillAmount = 0f;
                        rightCircle_Red.fillAmount = 0f;
                        rightCircle_Blue.fillAmount = 0f;
                    }
                    else
                    {
                        anim.SetBool("rightarrow", false);
                    }
                }
            }
        }
        else
        {
            print("범위를 초과했습니다!!!");
            if (isCali)
            {
                sRightCircle_Red.fillAmount = 1f;
            }
        }
        
    }

    void FixedUpdate()
    {
        // 플레이타임
        if (isDone && !isTutorial)
        {
            playtime += Time.deltaTime;
            string _playtime = playtime.ToString("N2") + "초";
            playtime_txt.text = "시간 : " + _playtime;

            if (0 < playtime && playtime <= 25)
                score = 100;
            else if (25 < playtime && playtime <= 30)
                score = 90;
            else if (30 < playtime && playtime <= 35)
                score = 80;
            else if (35 < playtime && playtime <= 40)
                score = 70;
            else if (40 < playtime && playtime <= 45)
                score = 60;
            else
                score = 50;
        }

        else if (timerStart && isTutorial)
        {
            playtime += Time.deltaTime;
            string _playtime = playtime.ToString("N2") + "초";
            playtime_txt.text = "시간 : " + _playtime;

            if (0 < playtime && playtime <= 25)
                score = 100;
            else if (25 < playtime && playtime <= 30)
                score = 90;
            else if (30 < playtime && playtime <= 35)
                score = 80;
            else if (35 < playtime && playtime <= 40)
                score = 70;
            else if (40 < playtime && playtime <= 45)
                score = 60;
            else
                score = 50;
        }
        else
        {
            playtime_txt.text = "시간 : 0.00초";
        }

        // 카메라 이동
        if (isDone)
        {
            CT.position = Vector3.Lerp(CT.position, PT.position, 2f * Time.deltaTime);
            CT.Translate(0, 0, -0.13f); //카메라 이동          
        }

    }

    // 게임 준비(게임 설명, 카운트 다운, 홀드 배치)
    IEnumerator Ready()
    {
        if (isTutorial)
        {
            tutorial1.SetActive(false);              
            tutor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-110, 350);
            isDone = true;
            yield return new WaitForSeconds(1.6f);
            tutorial1.SetActive(true);           
        }
        else
        {
            // 게임설명패널 3초간 보여주고 끔
            yield return new WaitForSeconds(3.0f);
            readyPanel.SetActive(false);

            // 홀드 출력하고 5초간 카운트 다운
            countdown_txt.gameObject.SetActive(true);
            for (int i = 5; i > 0; i--)
            {
                countdown_txt.text = "" + i;
                yield return new WaitForSeconds(1.0f);
            }
            countdown_txt.gameObject.SetActive(false);
            FakeHold.SetActive(true);
            isDone = true;
            yield return new WaitForSeconds(1f);
            gauge.SetActive(true);
        }     
    }

    // 결과화면
    public void ShowResult(int success)
    {
        _success = success;
        playtime = (float)(Math.Truncate(playtime * 100) / 100); // 소수점 2째자리까지 나타내주는 코드 - 이하 버림

        // 게임 실패
        if (success == 0)
        {
            SoundManager.instance.Fail();
            finishtime_txt.text = "게임시간 : " + playtime;
            score = 0;
            score_txt.text = "점수 : " + 0;   // 점수는 0으로
            resultPanel.SetActive(true);
            Device.instance.ResultCircle(circleGraph);

           /* string sql = string.Format("Insert into Game VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                date, userID, gameID, "'홀번호'", "'x'", Device.instance.values[0], Device.instance.values[-1], playtime, score);
            Debug.Log("sqlite.Instance.date : " + date);
            DBManager.DatabaseSQLAdd(sql);*/

            Time.timeScale = 0.0f;
        }

        // 게임 성공
        if (success == 1)
        {
            SoundManager.instance.Clear();
            finishtime_txt.text = "게임시간 : " + playtime;
            score = score + 10 * grabbed;
            score_txt.text = "점수 : " + score;   // 획득한 점수 출력
            resultPanel.SetActive(true);
            Device.instance.ResultCircle(circleGraph);

            /*string sql = string.Format("Insert into Game VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                date, userID, gameID, "'홀번호'", "'x'", Device.instance.values[0], Device.instance.values[-1], playtime, score);
            Debug.Log("sqlite.Instance.date : " + date);
            DBManager.DatabaseSQLAdd(sql);*/
            Time.timeScale = 0.0f;
        }

        /*Device.instance.ResultCircle(circleGraph);
        Serial.instance.End();*/

    }

    // 난이도
    public void Easy()
    {
        gamePanel.SetActive(true);
        selectPanel.SetActive(false);

        _wall.sprite = _easy;
        stage = 1;
        isStageSelected = true;
        
        easy.SetActive(true);
        normal.SetActive(false);
        hard.SetActive(false);
        FakeHold = easy.transform.GetChild(2).gameObject;
        row = 2;
        col = 3;
        holdParent = easy.transform.GetChild(0).gameObject;
        for (int i = 0; i < 5; i++)
        {
            Pos p = (Pos)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Pos)).Length);
            Debug.Log(p);
            if (p == Pos.up && row != 0)
            {
                prevHold = Instantiate(holdPrefab, new Vector3(
                    prevHold.transform.position.x, prevHold.transform.position.y + 0.73f, prevHold.transform.position.z),
                    Quaternion.Euler(0, 0, 45), easy.transform
                    );
                row--;
                route.Add(p);
            }
            else if (p == Pos.right && col != 0)
            {
                prevHold = Instantiate(holdPrefab,
                     new Vector3(prevHold.transform.position.x + 0.893f, prevHold.transform.position.y, prevHold.transform.position.z),
                     Quaternion.Euler(0, 0, 45), easy.transform
                     );
                col--;
                route.Add(p);
            }
            else
                i--;
        }
        readyPanel.SetActive(true);
        Time.timeScale = 1.0f;
        StartCoroutine("Ready");

    }

    public void Normal()
    {
        gamePanel.SetActive(true);
        selectPanel.SetActive(false);
        _wall.sprite = _normal;
        stage = 2;
        isStageSelected = true;

        easy.SetActive(false);
        normal.SetActive(true);
        hard.SetActive(false);
        FakeHold = normal.transform.GetChild(2).gameObject;
        row = 3;
        col = 4;
        holdParent = normal.transform.GetChild(0).gameObject;
        for (int i = 0; i < 7; i++)
        {
            Pos p = (Pos)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Pos)).Length);
            Debug.Log(p);
            if (p == Pos.up && row != 0)
            {
                prevHold = Instantiate(holdPrefab, new Vector3(
                    prevHold.transform.position.x, prevHold.transform.position.y + 0.73f, prevHold.transform.position.z),
                    Quaternion.Euler(0, 0, 45), normal.transform
                    );
                route.Add(p);
                row--;
            }
            else if (p == Pos.right && col != 0)
            {
                prevHold = Instantiate(holdPrefab,
                     new Vector3(prevHold.transform.position.x + 0.88f, prevHold.transform.position.y, prevHold.transform.position.z),
                     Quaternion.Euler(0, 0, 45), normal.transform
                     );
                route.Add(p);
                col--;
            }
            else
                i--;


        }
        readyPanel.SetActive(true);
        Time.timeScale = 1.0f;
        StartCoroutine("Ready");
    }
    public void Hard()
    {
        gamePanel.SetActive(true);
        selectPanel.SetActive(false);

        _wall.sprite = _hard;
        stage = 3;
        isStageSelected = true;

        easy.SetActive(false);
        normal.SetActive(false);
        hard.SetActive(true);
        FakeHold = hard.transform.GetChild(2).gameObject;
        row = 4;
        col = 5;
        holdParent = hard.transform.GetChild(0).gameObject;
        for (int i = 0; i < 9; i++)
        {
            Pos p = (Pos)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Pos)).Length);
            Debug.Log(p);
            if (p == Pos.up && row != 0)
            {
                prevHold = Instantiate(holdPrefab, new Vector3(
                    prevHold.transform.position.x, prevHold.transform.position.y + 0.73f, prevHold.transform.position.z),
                    Quaternion.Euler(0, 0, 45), hard.transform
                    );
                row--;
                route.Add(p);
            }
            else if (p == Pos.right && col != 0)
            {
                prevHold = Instantiate(holdPrefab,
                     new Vector3(prevHold.transform.position.x + 0.895f, prevHold.transform.position.y, prevHold.transform.position.z),
                     Quaternion.Euler(0, 0, 45), hard.transform
                     );
                col--;
                route.Add(p);
            }
            else
                i--;
        }
        readyPanel.SetActive(true);
        Time.timeScale = 1.0f;
        StartCoroutine("Ready");


    }

    // 일시정지
    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0.0f;
    }

    // 게임재개
    public void Resume()
    {
        pauseWarning.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // 튜토리얼 진행
    public void Tutorial1()
    {

        txt[idx - 1].gameObject.SetActive(false);
        txt[idx].gameObject.SetActive(true);

        if (idx == 1)
            face.sprite = umm;
        if (idx == 2)
            face.sprite = smile;

        if (idx == 3)
        {
            StartCoroutine("Ready");
        }

        if (idx == 8)
        {
            tutorial1.SetActive(false);
            tutorial_holds.transform.GetChild(0).gameObject.SetActive(true);           
        }

        if (idx == 11)
            face.sprite = umm;
        if (idx == 12)
        {
            face.sprite = smile;
            slider.value = sMid;
        }

        if (idx == 14)
        {
            cond1 = false;
            cond2 = false;            
            player.transform.position = new Vector2(-4.455f, -3.7296f);
            Player.cond1 = false;
            Player.cond2 = false;
            Player.isFinished = false;
            StartCoroutine("Practice");
        }

        if (idx == 15)
        {
            Time.timeScale = 0.0f;
            isPractice = false;
            Player.isResultShowing = true;
            emphasize_pause.SetActive(true);
            arrow1.SetActive(true);
        }
        else
        {
            emphasize_pause.SetActive(false);
            arrow1.SetActive(false);
        }

        if (idx == 16 || idx == 17)
        {
            if (idx == 17)
            {
                scoretut.SetActive(true);
            }
            emphasize_time.SetActive(true);
            arrow2.SetActive(true);
            pause_btn.interactable = true;
        }
        else
        {
            emphasize_time.SetActive(false);
            arrow2.SetActive(false);
            scoretut.SetActive(false);
        }

        if (idx == 18)
            arrow2.SetActive(false);

        if (idx == 19)
        {
            tutorial1.SetActive(false);
        }

        if (idx == 21)
            face.sprite = umm;

        if (idx == 22)
            face.sprite = smile;

        if (idx == 24)
        {
            btn.SetActive(false);
            playtime = 0f;
            finish_btn.SetActive(true);
        }

        idx++;
    }

    public void Tutorial2()
    {
        face.sprite = smile;
        tutorial2.SetActive(false);
    }

    IEnumerator Practice()
    {
        tutorial1.SetActive(false);
        yield return new WaitForSeconds(1.0f);

        // 플레이어 위치 이동
        player.transform.position = new Vector2(-4.455f, -3.7296f);

        // 처음에는 정상 홀드들만 보여줌
        tutorial_holds.transform.GetChild(0).gameObject.SetActive(false);
        tutorial_holds.transform.GetChild(1).gameObject.SetActive(true);
        tutorial_holds.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        tutorial_holds.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);

        // 홀드 경로를 외울 시간을 줌 (카운트다운)
        countdown_txt.gameObject.SetActive(true);
        for (int i = 5; i > 0; i--)
        {
            countdown_txt.text = "" + i;
            yield return new WaitForSeconds(1.0f);
        }
        countdown_txt.gameObject.SetActive(false);

        timerStart = true;
        isPractice = true;

        // 가짜 홀드들도 보여줌
        tutorial_holds.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
    }
}
