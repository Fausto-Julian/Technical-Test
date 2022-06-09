
using System.Collections;
using UnityEngine;

public class ChangePanel : MonoBehaviour
{
    [SerializeField] private GameObject sceneToHide;
    [SerializeField] private GameObject sceneToShow;
    [SerializeField] private float fadeTime;

    public void OnChangePanel()
    {
        FadeInOut();
    }

    private void FadeInOut() 
    {
        StartCoroutine(DoFade(sceneToHide, 1, 0));
        sceneToShow.SetActive(true);
        StartCoroutine(DoFade(sceneToShow, 0, 1));
    }

    IEnumerator DoFade(GameObject sceneToChange, float startAlpha ,float endAlpha)
    {
        sceneToChange.SetActive(true);
        CanvasGroup sceneToChangeCanvGroup = sceneToChange.GetComponent<CanvasGroup>();
        float counter = 0;

        while (counter < fadeTime)
        {
            counter += Time.deltaTime;
            sceneToChangeCanvGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, counter / fadeTime);

            yield return null;
        }

        if (sceneToChangeCanvGroup.alpha <= 0.01f)
        {
            sceneToChange.SetActive(false);
        }
    }
}