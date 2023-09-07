using TMPro;
using UnityEngine;

public class UICatalogItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    public TextMeshProUGUI Name { get => _name; set => _name = value; }
}
