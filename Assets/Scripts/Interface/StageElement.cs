using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Interface
{
    public class StageElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _counter = null;
        [SerializeField] private Color _counterGrey = Color.grey;
        [SerializeField] private Image _background = null;
        [SerializeField] private Color _backgroundGrey = Color.grey;

        public void SetStage(int number, bool active)
        {
            _counter.text = number.ToString();

            _background.color = active ? Color.white : _backgroundGrey;
            _counter.color = active ? Color.white : _counterGrey;
        }

        public void SetBackground(Sprite sprite)
        {
            _background.sprite = sprite;
        }
    }
}