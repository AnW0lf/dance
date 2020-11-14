using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Interface
{
    public class GameScreen : MonoBehaviour
    {
        [SerializeField] private bool _visible = false;
        [SerializeField] private MovePanel _levelPanel = null;
        [SerializeField] private MovePanel _tapToPlayLabel = null;
        [SerializeField] private Image _tapToPlayZone = null;
        [SerializeField] private LevelScript _levelScript = null;

        public bool Visible
        {
            get => _visible;
            private set
            {
                _visible = value;
                _levelPanel.Visible = _visible;
                _tapToPlayLabel.Visible = _visible;
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