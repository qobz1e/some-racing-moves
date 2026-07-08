using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatCellStart : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Button button;
    [SerializeField] Image icon;

    public Button Button => button;
    public TMP_Text LevelText => levelText;
    public TMP_Text PriceText => priceText;
    public Image Icon => icon;

    public void SetTitle(string title)
    {
        titleText.text = title;
    }
}