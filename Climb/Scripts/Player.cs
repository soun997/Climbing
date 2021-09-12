using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject warning1;
    public Text warning1_txt;
    public GameObject warning2;
    public Text warning2_txt;
    public static bool cond1;
    public static bool cond2;

    public float _timer;
    public float _time1;   // 홀드를 잡고 있지 않을 때 흘러가는 시간
    public float _time2;   // 가짜홀드를 잡고 있을 때 흘러가는 시간
    public Slider hold_time;    // 가짜홀드를 잡았을 경우, 일정시간안에 다른 홀드를 잡지 않으면 게임오버

    bool isGrabbing;    // 홀드를 잡고 있는지 여부
    bool isFalling; // 떨어지고 있는 중엔 콜라이더 다른 홀드에 닿아도 영향 x
    bool isFake;    // 가짜 홀드를 잡았을 때
    bool isPlaying; // 애니메이션 동작 시 숨소리 한번만 재생하도록
    public static bool isResultShowing;   // 게임 클리어 소리 한번만 재생하도록
    public static bool isFinished; // 완등 홀드를 잡았을 때 true
    public static bool isTutorial;
    public static bool tutorialComplete;

    public Collider2D hand_collider;


    // Start is called before the first frame update
    void Start()
    {
        _timer = 0f;
        isFinished = false;
        isResultShowing = false;
        cond1 = false;
        cond2 = false;

        hold_time.gameObject.SetActive(false);

        _time1 = 0f;
        _time2 = 0f;
        isGrabbing = true;
        isFalling = false;
        isFake = false;
        isPlaying = false;
        isTutorial = ClimbGameManager.isTutorial;
    }

    // Update is called once per frame
    void Update()
    {        
        // 튜토리얼일 때
        if (isTutorial)
        {
            hold_time.maxValue = 10f;
        }
        // 본 게임일 때
        else
        {           
            hold_time.maxValue = 10 + ClimbGameManager.stage;    // 가짜 홀드를 잡고 있을 수 있는 시간
            
        }

        // 가짜홀드를 잡았을 경우
        if (isFake)
        {
            // 슬라이더 보여줌
            hold_time.gameObject.SetActive(true);
            _time2 += Time.deltaTime;
            hold_time.value += Time.deltaTime;

            // 가짜홀드를 너무 오래 잡고 있으면


            // 튜토리얼일 때
            if (isTutorial)
            {
                if (_time2 >= 10)
                {
                    isGrabbing = false; // 떨어진다
                    Debug.Log(1);
                }
            }
            // 본게임일 때
            else
            {
                if (_time2 >= (10 + ClimbGameManager.stage)) // 게임 난이도에 따라
                {
                    isGrabbing = false; // 떨어진다
                    Debug.Log(1);
                }
            }            
        }

        else
        {
            _time2 = 0f;
            hold_time.gameObject.SetActive(false);
        }

        // 홀드를 잡고 있지 않다면
        if (!isGrabbing)
        {
            _time1 += Time.deltaTime;   // 얼마나 오래 잡고 있지 않았는지

            // 제한시간을 넘겼다면 -> 떨어짐!
            if (_time1 >= 0.4f)
            {
                isFalling = true;

                // 떨어질 때 비명소리 한번만 나도록
                if (!isPlaying)
                {
                    isPlaying = true;
                }

                if (!isResultShowing)
                {
                    if (!isTutorial)
                    {
                        // 캐릭터 떨어지는 모습
                        SoundManager.instance.FallingSound();
                        ClimbGameManager.instance.anim.SetBool("falling", true);
                        ClimbGameManager.instance.player.transform.Translate(new Vector2(0, -0.05f));
                        Debug.Log("떨어짐");
                        ClimbGameManager.instance.score_txt.text = "점수 : " + 0;
                        ClimbGameManager.instance.ShowResult(0);
                        isResultShowing = true;
                    }
                    else
                    {
                        _time1 = 0f;
                        _time2 = 0f;
                        isFake = false;
                        isPlaying = false;
                        hold_time.gameObject.SetActive(false);
                        SoundManager.instance.FallingSound();
                        ClimbGameManager.instance.tutorial2.SetActive(true);
                        ClimbGameManager.instance.face2.sprite = ClimbGameManager.instance.umm;
                        Debug.Log("얼굴바뀜");
                        StartCoroutine("Sleep");
                        ClimbGameManager.instance.player.transform.position = new Vector2(-4.455f, -3.7296f);
                    }
                }
            }

            else
            {
                isFalling = false;
                ClimbGameManager.instance.anim.SetBool("falling", false);
            }
        }
    }

    IEnumerator Sleep()
    {
        yield return new WaitForSecondsRealtime(1.0f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 떨어지는 중이 아니라면(떨어지는 와중에 다른 홀드에 닿아도 상관 없도록)
        if (!isFalling)
        {
            // 올바른 홀드에 닿았을 때
            if (collision.gameObject.tag == "Holds")
            {
                hold_time.value = 0;
                isGrabbing = true;
                isFake = false;
                _time1 = 0f;
                _time2 = 0f;

                // 올바른 홀드를 잡았으므로 보너스 점수 추가
                if (ClimbGameManager.isGrabbed)
                {
                    Debug.Log("10점 추가");
                    ClimbGameManager.isGrabbed = false;
                    ClimbGameManager.grabbed++;
                }
            }

            // 가짜 홀드에 닿았을 때
            if (collision.gameObject.tag == "FakeHolds")
            {
                hold_time.value = 0;
                isGrabbing = true;
                isFake = true;
                _time1 = 0f;
                _time2 = 0f;

                ClimbGameManager.isGrabbed = false;                
                Debug.Log("가짜 홀드 잡음");
            }


        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!isFalling)
        {
            if (collision.gameObject.tag == "Holds")
            {
                isGrabbing = true;
            }

            if (collision.gameObject.tag == "FakeHolds")
            {
                isGrabbing = true;
            }

            if (collision.gameObject.tag == "TopHold")
            {
                ClimbGameManager.instance.squeeze = true;
                warning1_txt.text = "시계방향으로 힘차게 돌려주세요!";
                warning2_txt.text = "반시계방향으로 힘차게 돌려주세요!";

                hold_time.value = 0;
                isGrabbing = true;
                isFake = false;
                _time1 = 0f;
                _time2 = 0f;
                isFinished = true;

                if (!cond1)
                {
                    warning1.SetActive(true);
                }

                if (ClimbGameManager.sVal >= ClimbGameManager.sMax && !cond1)
                {
                    Debug.Log("cond1만족");
                    _timer += Time.deltaTime;
                    warning1_txt.text = 5 - (int)_timer + "초 ";
                    if (_timer > 5.0f)
                    {
                        cond1 = true;
                        _timer = 0f;
                        warning1.SetActive(false);
                        warning2.SetActive(true);
                    }
                }

                if (ClimbGameManager.sVal <= ClimbGameManager.sMin && cond1 && !cond2)
                {
                    Debug.Log("cond2만족");
                    _timer += Time.deltaTime;
                    warning2_txt.text = 5 - (int)_timer + "초 ";
                    if (_timer > 5.0f)
                    {
                        cond2 = true;
                        _timer = 0f;
                        warning2.SetActive(false);
                    }
                }

                if (cond2)
                {
                    cond2 = false;
                    ClimbGameManager.isGrabbed = false;

                    if (!isResultShowing)
                    {
                        ClimbGameManager.instance.slider.value = ClimbGameManager.instance.slider.minValue;

                        if (!isTutorial)
                        {
                            ClimbGameManager.instance.ShowResult(1);
                            Debug.Log("결과창 생성");
                            isResultShowing = true;
                        }

                        else
                        {
                            Debug.Log("튜토리얼 진행");
                            ClimbGameManager.instance.tutorial1.SetActive(true);                           
                        }        
                      
                        Debug.Log("게임 클리어");
                    }
                }

            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isFalling)
        {
            if (collision.gameObject.tag == "Holds")
            {
                isGrabbing = false;
                Debug.Log("홀드 놓음");
            }

            if (collision.gameObject.tag == "FakeHolds")
            {
                isGrabbing = false;
                Debug.Log("가짜 홀드 놓음");
            }
        }

    }
}
