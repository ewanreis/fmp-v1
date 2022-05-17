using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DamageIndicator : MonoBehaviour
{
    public TMP_Text text;
    public float lifetime = 0.6f, minDist = 2f, maxDist = 3f;

    private Vector3 iniPos, targetPos;
    private float timer;

    void Start()
    {
        transform.LookAt(2 * transform.position - Camera.main.transform.position);

        float direction = Random.rotation.eulerAngles.z;
        iniPos = transform.position;
        float dist = Random.Range(minDist, maxDist);
        targetPos = iniPos + (Quaternion.Euler(0, 0, direction) * new Vector3(dist, dist, 0f));
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float fraction = lifetime / 2f;

        if (timer > lifetime) Destroy(gameObject);
        else if (timer > fraction) text.color = Color.Lerp(text.color,
                                                          Color.clear,
                                                          (timer - fraction) / (lifetime - fraction));

        transform.position = Vector3.Lerp(iniPos,
                                          targetPos,
                                          Mathf.Sin(timer / lifetime));

        transform.localScale = Vector3.Lerp(Vector3.zero,
                                            Vector3.one,
                                            Mathf.Sin(timer / lifetime));
    }

    public void SetDamageText(float damage) => text.text = damage.ToString();
}