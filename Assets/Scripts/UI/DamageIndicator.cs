using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float flashSpeed;

    private Coroutine coroutine;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.onTakeDamage += Flash;
    }

    public void Flash()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        image.enabled = true;
        image.color = new Color(1.0f, 105 / 255.0f, 105 / 255.0f);
        coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0.0f)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            image.color = new Color(1f, 100 / 255.0f, 100 / 255.0f, a);
            yield return null;
        }

        image.enabled = false;
    }
}
