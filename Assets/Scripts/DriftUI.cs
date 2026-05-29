using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DriftUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Wheel")]
    [SerializeField] private RectTransform ring;
    [SerializeField] private RectTransform needle;
    [SerializeField] private Image successZone;
    [SerializeField] private TMP_Text multiplierText;

    [SerializeField] private TMP_Text failText;

    [Header("Spin")]
    [SerializeField] private float spinSpeed = 450f;

    [Header("Success Zone")]
    [Range(0f, 360f)]
    [SerializeField] private float successCenter = 0f;

    [Range(5f, 180f)]
    [SerializeField] private float successSize = 40f;

    private float currentAngle;

    public bool DidFullRotation { get; private set; }

    // ─────────────────────────────────────────────────────────────

    private void Start()
    {
        BuildSuccessZoneVisual();
    }

    // ─────────────────────────────────────────────────────────────

    public void Show()
    {
        currentAngle = 0f;

        DidFullRotation = false;

        UpdateNeedle();

        root.SetActive(true);

        // Restore normal drift UI
        ring.gameObject.SetActive(true);
        successZone.gameObject.SetActive(true); 
        needle.gameObject.SetActive(true);
        multiplierText.gameObject.SetActive(true);

        failText.gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────

    public void Hide()
    {
        root.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────

    public void RotateNeedle()
    {
        float previousAngle = currentAngle;

        currentAngle +=
            spinSpeed *
            Time.unscaledDeltaTime;

        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;

            DidFullRotation = true;
        }

        UpdateNeedle();
    }

    // ─────────────────────────────────────────────────────────────

    private void UpdateNeedle()
    {
        if (needle == null)
            return;

        needle.localRotation =
            Quaternion.Euler(
                0,
                0,
                -(currentAngle + 90)
            );
    }

    // ─────────────────────────────────────────────────────────────

    public bool IsInSuccessZone()
    {
        float half =
            successSize * 0.5f;

        float min =
            successCenter - half;

        float max =
            successCenter + half;

        return currentAngle - 270 >= min &&
               currentAngle - 270 <= max;
    }

    // ─────────────────────────────────────────────────────────────

    public void ResetRotation()
    {
        currentAngle = 0f;

        DidFullRotation = false;

        RotateNeedle();
    }

    // ─────────────────────────────────────────────────────────────

    public void SetMultiplier(int value)
    {
        if (multiplierText == null)
            return;

        multiplierText.text = "x" + value;
    }

    // ─────────────────────────────────────────────────────────────

    public void ShowFailOnly(string message)
    {
        root.SetActive(true);

        ring.gameObject.SetActive(false);
        successZone.gameObject.SetActive(false);
        needle.gameObject.SetActive(false);
        multiplierText.gameObject.SetActive(false);

        failText.text = message;
        failText.gameObject.SetActive(true);
    }


    public void HideFailReason() 
    { 
        if (failText == null) return; 

        failText.gameObject.SetActive(false); 
    }

    // ─────────────────────────────────────────────────────────────
    // VISUAL SUCCESS ZONE
    // ─────────────────────────────────────────────────────────────

    private void BuildSuccessZoneVisual()
    {
        if (successZone == null)
            return;

        successZone.type = Image.Type.Filled;

        successZone.fillMethod =
            Image.FillMethod.Radial360;

        successZone.fillOrigin = 2;

        successZone.fillClockwise = true;

        successZone.fillAmount =
            successSize / 360f;

        successZone.rectTransform.localRotation =
            Quaternion.Euler(
                0,
                0,
                -successCenter + successSize * 0.5f
            );
    }
}
