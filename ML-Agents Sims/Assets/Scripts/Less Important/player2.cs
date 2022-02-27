using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
using TMPro;

public class player2 : MonoBehaviour
{

    [SerializeField]
    GameObject FPSCamera;

    [SerializeField]
    TextMeshProUGUI playerNameText;

    // Start is called before the first frame update
    void Start()
    {

       if (playerNameText)
        {

            //transform.GetComponent<MovementController>().enabled = true;
            FPSCamera.GetComponent<Camera>().enabled = true;


        }
        else
        {
           // transform.GetComponent<MovementController>().enabled = false;
            FPSCamera.GetComponent<Camera>().enabled = false;

        }


        SetPlayerUI();

    }

    void SetPlayerUI()
    {
        if (playerNameText != null)
        {
           // playerNameText.text = photonView.Owner.NickName;

        }
    }


}