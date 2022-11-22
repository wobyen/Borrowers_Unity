using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class NotificationHandler : MonoBehaviour
{

    TextMeshProUGUI notificationText;
    public GUI_TextObject textObject;

    bool messageOver;

    int timer;

    // Start is called before the first frame update
    void Start()
    {
        notificationText = GetComponent<TextMeshProUGUI>();
        notificationText.DOFade(0, .01f);

    }

    // Update is called once per frame

    // private void Update()
    // {
    //     if (messageOver)
    //     {
    //         StopAllCoroutines();
    //         messageOver = false;
    //     }
    // }


    // public IEnumerator TextFadeInOut()
    // {

    //     notificationText.DOFade(100, 1);

    //     notificationText.text = textObject.textDisplay;

    //     yield return new WaitForSeconds(5);

    //     notificationText.DOFade(0, 2);


    //     messageOver = true;
    //     yield return null;
    // }


}
