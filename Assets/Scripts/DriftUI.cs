using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DriftUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Wheel")]
    [SerializeField] private RectTransform needle;

    [SerializeField] private Image successZone;

    [SerializeField] private TMP_Text multiplierText;

    [Header("Spin")]
    [SerializeField] private float spinSpeed = 540f;

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

        Hide();
    }

    // ─────────────────────────────────────────────────────────────

    public void Show()
    {
        currentAngle = 90f;

        DidFullRotation = false;

        UpdateNeedle();

        root.SetActive(true);
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

        if (currentAngle >= 450f)
        {
            currentAngle -= 450f;

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
                -currentAngle
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

        if (min < 0f)
        {
            return currentAngle >= 360f + min ||
                   currentAngle <= max;
        }

        if (max > 360f)
        {
            return currentAngle >= min ||
                   currentAngle <= max - 360f;
        }

        return currentAngle >= min &&
               currentAngle <= max;
    }

    // ─────────────────────────────────────────────────────────────

    public void ResetRotation()
    {
        currentAngle = 0f;

        DidFullRotation = false;

        UpdateNeedle();
    }

    // ─────────────────────────────────────────────────────────────

    public void SetMultiplier(int value)
    {
        if (multiplierText == null)
            return;

        multiplierText.text = "x" + value;
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
