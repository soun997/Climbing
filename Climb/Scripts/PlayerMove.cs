using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameObject player;
    public Vector3 pLocalPos;
    public Vector3 pNewPos;
    public Vector3 vectorUp;
    public Vector3 vectorRight;
    public bool isMoveUp, isMoveRight;


    // Start is called before the first frame update
    void Start()
    {
        pLocalPos = player.transform.localPosition;
        vectorUp = new Vector3(0, 4.065f, 0);
        vectorRight = new Vector3(5f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        pLocalPos = player.transform.localPosition;

        if (isMoveUp)
        {
            if (Math.Round(pLocalPos.y, 1) == Math.Round(pNewPos.y, 1))
            {
                isMoveUp = false;
                //pNewPos = pLocalPos + vectorUp;
            }
            else
            {
                player.transform.localPosition = Vector3.Lerp(pLocalPos, pNewPos, 2.5f * Time.deltaTime);

            }
        }

        if (isMoveRight)
        {
            if (Math.Round(pLocalPos.x, 1) == Math.Round(pNewPos.x, 1))
            {
                isMoveRight = false;
                //pNewPos = pLocalPos + vectorRight;
            }
            else
            {
                player.transform.localPosition = Vector3.Lerp(pLocalPos, pNewPos, 2f * Time.deltaTime);

            }
        }
    }

    public void StartMoveUp()
    {
        isMoveUp = true;
        pNewPos = pLocalPos + vectorUp;
        print("움직임, " + pLocalPos);
    }

    public void StartMoveRight()
    {
        isMoveRight = true;
        pNewPos = pLocalPos + vectorRight;
        print("움직임, " + pLocalPos);
    }

}
