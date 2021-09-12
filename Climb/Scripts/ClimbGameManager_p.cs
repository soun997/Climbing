using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ClimbGameManager_p : MonoBehaviour
{

    #region Singleton
    public static ClimbGameManager_p instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        date = DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분 ss초");
        date = DBManager.SqlFormat(date);
        userID = "'userID'";
        gameID = "'21climb'";
        maxScore = 0;
        MaxScore_Text.text = "최고점수 : " + maxScore;
    }
    #endregion

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
    public GameObject highscore_obj;

    public static float playtime; // 플레이타임
    public int score;   // 점수
    public static int grabbed; // 올바른 홀드 잡은 횟수
    public static bool isGrabbed;  // 
    public int _success;

    #endregion

    #region 상지가동범위 newOne
    public Image newLeft;   // 왼쪽
    public Image newRight;   // 오른쪽
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

    #region 튜토리얼 관련
    public GameObject scriptBox;
    public Text[] txt;
    public int idx;
    public GameObject btn;
    public GameObject finish_btn;

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
    #endregion

    enum Pos {
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

        //Serial.instance.Passive();  // 수동모드 시작

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
            highscore_obj.SetActive(false);
            row = 1;
            col = 2;
            easy.SetActive(false);
            normal.SetActive(false);
            hard.SetActive(false);
            isPractice = false;
            timerStart = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        cond1 = false;
        cond2 = false;
               
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
        _success = 0;

        sMin = slider.minValue;
        sMax = slider.maxValue;
        sMid = (slider.minValue + slider.maxValue) / 2;
        slider.value = sMid;

        if (!isTutorial)
        {
            timerStart = true;
            selectPanel.SetActive(true);

            DBManager.DataBaseRead(string.Format("SELECT * FROM Game WHERE userID = {0} and gameID = {1} ORDER BY gameScore DESC ", userID, gameID));
            while (DBManager.dataReader.Read())
            {
                maxScore = DBManager.dataReader.GetInt32(8);
                break;
            }
            DBManager.DBClose();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        

    }

    // Update is called once per frame
    void Update()
    {
        sVal = slider.value;

        if (!isTutorial)
        {
            tutorial1.SetActive(false);
        }       

        if (!Player_p.isFinished && (isStageSelected || isTutorial))
        {
            if (slider.value <= (sMid - (slider.maxValue - slider.minValue) * 1.0 / 6.0 * stage) + 5f && !cond1)
            {
                cond1 = true;
                warning.SetActive(true);
            }

            // 왼쪽 슬라이더 값 다시 0으로 -> 위로 이동
            if (slider.value <= (sMid + 5f) && slider.value >= (sMid - 5f) && cond1 == true)
            {
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

            if (slider.value >= (sMid + (slider.maxValue - slider.minValue) * 1.0 / 6.0 * stage) - 5f && !cond2)
            {
                cond2 = true;
                warning.SetActive(true);
            }

            // 오른쪽 슬라이더 값 다시 0으로 -> 오른쪽으로 이동
            if (slider.value <= (sMid + 5f) && slider.value >= (sMid - 5f) && cond2 == true)
            {
                SoundManager.instance.GrabSound();
                anim.SetBool("rightarrow", true);
                cond2 = false;
                isGrabbed = true;
                warning.SetActive(false);
            }
            else
            {
                anim.SetBool("rightarrow", false);
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

            // 홀드 출력하고 3초간 카운트 다운
            countdown_txt.gameObject.SetActive(true);
            for (int i = 3; i > 0; i--)
            {
                countdown_txt.text = "" + i;
                yield return new WaitForSeconds(1.0f);
            }
            countdown_txt.gameObject.SetActive(false);

            isDone = true;
            StartCoroutine("PassiveControl");
        }     
    }

    IEnumerator PassiveControl()
    {
        yield return new WaitForSeconds(2.0f);
        int lAngle = (int)(sMid - (slider.maxValue - slider.minValue) * 1.0 / 6.0 * stage);
        int rAngle = (int)(sMid + (slider.maxValue - slider.minValue) * 1.0 / 6.0 * stage);
        int isTut;
        if (isTutorial) isTut = 0;
        else isTut = 1;

            for (int i = 0; i < (stage * 2 * isTut) + 3; i++)
        {
            Debug.Log("route : " + route[i]);
            if (route[i] == Pos.up)
            {
                //Serial.instance.Pa_Motor("0" + Serial.instance.WriteAngle(lAngle) + "2000");
                slider.value = lAngle;  // 테스트
                yield return new WaitForSeconds(2.0f);
                //Serial.instance.Pa_Motor("1" + Serial.instance.WriteAngle(sMid) + "2000");
                slider.value = sMid;   // 테스트
                yield return new WaitForSeconds(2.0f);
            }
            else if (route[i] == Pos.right)
            {
                //Serial.instance.Pa_Motor("1" + Serial.instance.WriteAngle(rAngle) + "2000");
                slider.value = rAngle;  // 테스트
                yield return new WaitForSeconds(2.0f);
                //Serial.instance.Pa_Motor("0" + Serial.instance.WriteAngle(sMid) + "2000");
                slider.value = sMid;   // 테스트
                yield return new WaitForSeconds(2.0f);
            }
        }

        //Serial.instance.Pa_Motor("0" + Serial.instance.WriteAngle(lAngle) + "2000");
        slider.value = lAngle;  // 테스트
        yield return new WaitForSeconds(2.0f);
        //Serial.instance.Pa_Motor("1" + Serial.instance.WriteAngle(sMid) + "2000");
        slider.value = sMid;   // 테스트
        yield return new WaitForSeconds(2.0f);    

        //Serial.instance.Pa_Motor("0" + Serial.instance.WriteAngle(lAngle) + "2000");
        slider.value = lAngle;  // 테스트
        yield return new WaitForSeconds(5.5f);
        //Serial.instance.Pa_Motor("1" + Serial.instance.WriteAngle(rAngle) + "2000");
        slider.value = rAngle;  // 테스트
        yield return new WaitForSeconds(5.5f);
    }
    // 결과화면
    public void ShowResult(int success)
    {
        playtime = (float)(Math.Truncate(playtime * 100) / 100); // 소수점 2째자리까지 나타내주는 코드 - 이하 버림
        _success = success;
        // 게임 실패
        if (success == 0)
        {
            SoundManager.instance.Fail();
            finishtime_txt.text = "게임시간 : " + playtime;
            score = 0;
            score_txt.text = "점수 : " + 0;   // 점수는 0으로
            resultPanel.SetActive(true);

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
        _wall.sprite = _easy;
        stage = 1;
        isStageSelected = true;
        selectPanel.SetActive(false);
        easy.SetActive(true);
        normal.SetActive(false);
        hard.SetActive(false);
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
                     new Vector3(prevHold.transform.position.x + 0.88f, prevHold.transform.position.y, prevHold.transform.position.z),
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
        _wall.sprite = _normal;
        stage = 2;
        isStageSelected = true;
        selectPanel.SetActive(false);
        easy.SetActive(false);
        normal.SetActive(true);
        hard.SetActive(false);
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
        _wall.sprite = _hard;
        stage = 3;
        isStageSelected = true;
        selectPanel.SetActive(false);
        easy.SetActive(false);
        normal.SetActive(false);
        hard.SetActive(true);
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
                     new Vector3(prevHold.transform.position.x + 0.88f, prevHold.transform.position.y, prevHold.transform.position.z),
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
            route.Clear();
            route.Add(Pos.right); route.Add(Pos.right); route.Add(Pos.up);
            tutorial_holds.transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine("PassiveControl");

        }

        if (idx == 12)
        {
            slider.value = sMid;
        }

        if (idx == 14)
        {
            cond1 = false;
            cond2 = false;            
            player.transform.position = new Vector2(-4.455f, -3.7296f);
            Player_p.cond1 = false;
            Player_p.cond2 = false;
            Player_p.isFinished = false;
            route.Clear();
            route.Add(Pos.right); route.Add(Pos.up); route.Add(Pos.right);
            StartCoroutine("Practice");
        }

        if (idx == 15)
        {
            Time.timeScale = 0.0f;
            isPractice = false;
            Player_p.isResultShowing = true;
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

        tutorial_holds.transform.GetChild(0).gameObject.SetActive(false);
        tutorial_holds.transform.GetChild(1).gameObject.SetActive(true);

        // 마음의 준비(카운트 다운)
        countdown_txt.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdown_txt.text = "" + i;
            yield return new WaitForSeconds(1.0f);
        }
        countdown_txt.gameObject.SetActive(false);

        timerStart = true;
        isPractice = true;
        StartCoroutine("PassiveControl");
    }
}
