using TMPro;
using UnityEngine;

public class CoinPopup : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float riseSpeed = 100f;

    private TMP_Text text;
    private float timer;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Setup(int amount)
    {
        text.text = $"+{amount}";
    }

    void Update()
    {
        timer += Time.deltaTime;

        transform.localPosition +=
            Vector3.up * riseSpeed * Time.deltaTime;

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}