using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System;

namespace Assets.Scripts.Interface.ChestMiniGame
{
    public class RewardMiniGame : MonoBehaviour
    {
        [Header("Label settings")]
        [SerializeField] private TextMeshProUGUI _label = null;
        [SerializeField] private string _statement = string.Empty;
        [SerializeField] private int _keyCount = 3;
        [Space(20)]
        [Header("Reward sequance")]
        [SerializeField] Reward[] _sequance = null;

        private int _index = 0;

        public int KeyCount
        { get => _keyCount; private set => _keyCount = value; }

        private void Start()
        {
            UpdateLabel();
        }

        private void OnEnable()
        {
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            _label.text = string.Format(_statement, _keyCount);
        }

        public Reward GetReward()
        {
            KeyCount--;
            Reward reward = _sequance[_index];
            _index++;
            UpdateLabel();
            return reward;
        }
    }

    public enum RewardType { NONE, MONEY, CLASSIC, JAZZ, STREET }

    [Serializable]
    public class Reward
    {
        [SerializeField] private RewardType _type = RewardType.NONE;
        [SerializeField] private int _count = 0;

        public RewardType Type => _type;
        public int Count => _count;
    }
}
