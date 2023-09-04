using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSign : MonoBehaviour
{
    [SerializeField] private Image _timerCircle;
    [SerializeField] private float _speed;

    public void StartLoading() => StartCoroutine(Loading());

    private IEnumerator Loading()
    {
        float passedTime = 0;
        while (gameObject.activeInHierarchy)
        {
            if (passedTime < _speed)
            {
                passedTime += Time.deltaTime * 2;
                _timerCircle.fillAmount = Mathf.InverseLerp(0f, 1f, passedTime / _speed);
                yield return null;
            }
            else
                passedTime = 0;
        }
    }
}
