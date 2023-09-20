using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotCharacterWidget : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _emptySlot;
    [SerializeField] private GameObject _infoCharacterSlot;
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _levelLabel;
    [SerializeField] private TextMeshProUGUI _goldLabel;
    [SerializeField] private TextMeshProUGUI _HPLabel;
    [SerializeField] private TextMeshProUGUI _damageLabel;
    [SerializeField] private TextMeshProUGUI _experienceLabel;

    public Button SlotButton { get => _button; }
    public void ShowCharacterSlost(string name, string level, string gold, string hp, string damage, string experience)
    {
        _nameLabel.text = name;
        _levelLabel.text = level;
        _goldLabel.text = gold;
        _HPLabel.text = hp;
        _damageLabel.text = damage;
        _experienceLabel.text = experience;

        _infoCharacterSlot.SetActive(true);
        _emptySlot.SetActive(false);
    }

    public void ShowEmptySlot()
    {
        _infoCharacterSlot.SetActive(false);
        _emptySlot.SetActive(true);
    }
}
