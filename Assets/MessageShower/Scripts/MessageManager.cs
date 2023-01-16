using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;

    public MessageShowerSettings settings;

    public TextMeshProUGUI messageText;
    
    Vector3 tempLocalPosition;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

         tempLocalPosition = messageText.transform.localPosition;
    }
    public void Show(string message)
    {
        StopAllCoroutines();
        
        messageText.transform.localPosition = tempLocalPosition;

        StartCoroutine(ShowIE(message));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) { Show("Wait " + Random.Range(0, 999)); }
    }
    private IEnumerator ShowIE(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        Vector3 tempLocalPosition = messageText.transform.localPosition;
        yield return StartCoroutine(Lerp(tempLocalPosition + (Vector3.up * settings.upRange), settings.duration));

        messageText.gameObject.SetActive(false);
    }

    IEnumerator Lerp(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = messageText.transform.localPosition;
        while (time < duration)
        {
            messageText.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        messageText.transform.localPosition = targetPosition;
    }
}
