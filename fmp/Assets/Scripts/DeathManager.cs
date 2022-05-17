using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class DeathManager : MonoBehaviour
{
    public TMP_Text roundCounter;

    private int lastRound;
    private IEnumerator Start()
    {
        PlayerSFXManager.deadSFX = true;
        lastRound = PlayerPrefs.GetInt("lastRoundReached", 0);
        roundCounter.text = $"You reached round {lastRound}";
        yield return new WaitForSeconds(7f);
        SceneManager.LoadSceneAsync(0);
    }
}
