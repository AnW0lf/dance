using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class RewardMiniGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label = null;
    [SerializeField] private string _statement = string.Empty;
    [SerializeField] private int _keyCount = 3;

    public int KeyCount { get => _keyCount; set => _keyCount = value; }

    public void UpdateLabel()
    {
        _label.text = string.Format(_statement, _keyCount);
    }
}
