using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Interface
{
    public class GameScreen : MonoBehaviour
    {
        [SerializeField] private bool _visible = false;
        [SerializeField] private Image _tapToPlayZone = null;
        [SerializeField] private LevelScript _levelScript = null;
        [SerializeField] private MovePanel[] _panels = null;

        public bool Visible
        {
            get => _visible;
            private set
            {
                _visible = value;
                foreach (var panel in _panels) panel.Visible = _visible;
                _tapToPlayZone.raycastTarget = _visible;
            }
        }

        private void Start()
        {
            Visible = true;
        }

        public void StartGame()
        {
            Visible = false;
            _levelScript.StartLevel();
        }

        public void EndGame()
        {
            Visible = true;
        }
    }
}