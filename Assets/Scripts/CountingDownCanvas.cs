using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[DefaultExecutionOrder(-1)]
public class CountingDownCanvas : MonoBehaviourSingleton<CountingDownCanvas>
{
    public event System.Action<int, GameObject> OnCountedDown;

    Coroutine countingDownCoroutine;
    Coroutine countingDownCoroutine2P;
    float num = 2f;
    float num2P = 2f;
    public GameObject countingDown;
    public TextMeshProUGUI numText;
    public GameObject countingDown2P;
    public TextMeshProUGUI numText2P;

    public void CountingDown(GameObject applicant)
    {
        AvatarCard card = AvatarCard.FindSpecifiedCard(applicant);
        if (card.avatarID == 1)
            CountingDown1P(applicant);
        else if (card.avatarID == 2)
            CountingDown2P(applicant);
    }
    public void CountingDown1P(GameObject applicant)
    {
        //if (countingDownCoroutine != null) StopCoroutine(countingDownCoroutine);
        if (countingDownCoroutine != null) return;
        countingDownCoroutine = StartCoroutine(StartCountingDown1P( applicant, num));
    }
    public void CountingDown2P(GameObject applicant)
    {
        //if (countingDownCoroutine2P != null) StopCoroutine(countingDownCoroutine2P);
        if (countingDownCoroutine2P != null) return;
        countingDownCoroutine2P = StartCoroutine(StartCountingDown2P( applicant, num2P));
    }

    IEnumerator StartCountingDown1P(GameObject applicant, float _num )
    {
        numText.text = _num.ToString();
        countingDown.SetActive(true);
        while (_num > 0)
        {
            yield return new WaitForSeconds(1f);
            _num -= 1f;
            numText.text = _num.ToString();
        }
        OnCountedDown?.Invoke(1, applicant);
        countingDownCoroutine = null;
        countingDown.SetActive(false);
    }

    IEnumerator StartCountingDown2P(GameObject applicant, float _num)
    {
        numText2P.text = _num.ToString();
        countingDown2P.SetActive(true);
        while (_num > 0)
        {
            yield return new WaitForSeconds(1f);
            _num -= 1f;
            numText2P.text = _num.ToString();
        }
        OnCountedDown?.Invoke(1, applicant);
        countingDownCoroutine2P = null;
        countingDown2P.SetActive(false);
    }

}
