using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMasterScript : MonoBehaviour
{
    public GameObject o1;
    public GameObject o2;
    public GameObject o3;
    public GameObject o4;
    public GameObject os;
    public InputField username;
    public InputField pswd;
    public Text loginConfirm;
    // Start is called before the first frame update
    void Start()
    {
        o1.SetActive(false);
        o2.SetActive(false);
        o3.SetActive(false);
        o4.SetActive(false);
        os.SetActive(false);
        pswd.contentType = InputField.ContentType.Password;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene()
    {
        StartCoroutine(LoadNewScene());
    }

    public void OpenOver1()
    {
        o1.SetActive(true);
        o2.SetActive(false);
        o3.SetActive(false);
        o4.SetActive(false);
    }

    public void OpenOver2()
    {
        o1.SetActive(false);
        o2.SetActive(true);
        o3.SetActive(false);
        o4.SetActive(false);
    }

    public void OpenOver3()
    {
        o1.SetActive(false);
        o2.SetActive(false);
        o3.SetActive(true);
        o4.SetActive(false);
    }

    public void OpenOver4()
    {
        o1.SetActive(false);
        o2.SetActive(false);
        o3.SetActive(false);
        o4.SetActive(true);
    }

    public void OverRes()
    {
        o4.SetActive(false);
        o1.SetActive(true);
        o2.SetActive(false);
        o3.SetActive(false);
    }

    public void closeOVer1()
    {
        o1.SetActive(false);
    }

    public void closeOVer2()
    {
        o2.SetActive(false);
    }
    public void closeOVer3()
    {
        o3.SetActive(false);
    }
    public void closeOVer4()
    {
        o4.SetActive(false);
    }

    public void Settings()
    {
        bool settingsSwitch;
        settingsSwitch = os.activeSelf == true ? false : true;
        os.SetActive(settingsSwitch);
    }

    public void ReturnLogin()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator LoadNewScene()
    {
        loginConfirm.text += "Hi " + username.text + ", connecting you to the server...";
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }
}
