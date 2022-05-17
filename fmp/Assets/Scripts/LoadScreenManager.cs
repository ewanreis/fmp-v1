using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
public class LoadScreenManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadSlider;
    public void LoadScene(int index) => StartCoroutine(LoadScreenAsync(index));
    IEnumerator LoadScreenAsync(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            loadSlider.value = operation.progress;
            yield return null;
        }
    }
}
