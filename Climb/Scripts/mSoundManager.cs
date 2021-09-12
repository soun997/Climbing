using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip game_start;
    public AudioClip game_quit;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);  // 소리가 잘리지 않게 하기 위해 오브젝트 살려둠
    }

    // Update is called once per frame
    void Update()
    {
        // 게임이 시작되면 오브젝트 삭제되도록
        if (ClimbGameManager.playtime > 0.0f)
        {
            Destroy(gameObject);
        }
    }

    public void GameStart()
    {
        audioSource.PlayOneShot(game_start);
    }

    public void GameQuit()
    {
        audioSource.PlayOneShot(game_quit);
    }
}
