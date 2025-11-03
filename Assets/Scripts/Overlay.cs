using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Map
{
    public class Overlay : MonoBehaviour
    {
        private float alpha = 0f;

        // Use this for initialization
        private void Start()
        {
            // Start off with the canvas being transparent
            gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, alpha);
        }

        public void FadeOut(float time)
        {
            StartCoroutine(changeValueOverTime(0, 1, time));
        }

        private IEnumerator changeValueOverTime(float fromVal, float toVal, float duration)
        {
            var counter = 0f;

            while (counter < duration)
            {
                if (Time.timeScale == 0)
                    counter += Time.unscaledDeltaTime;
                else
                    counter += Time.deltaTime;

                // Interpolate from transparent to dark over a set period of time
                alpha = Mathf.Lerp(0f, 1f, counter / duration);
                gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, alpha);

                yield return null;
            }
        }
    }
}