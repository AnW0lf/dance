using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Interface
{
    public class StageController : MonoBehaviour
    {
        [SerializeField] private Sprite _currentStageIcon = null;
        [SerializeField] private StageElement[] _elements = null;

        private void Start()
        {
            int levelNumber = Player.Instance.LevelNumber;
            int elementIndex = (levelNumber - 1) % _elements.Length;

            for (int i = 0; i < _elements.Length; i++)
            {
                _elements[i].SetStage(levelNumber + i - elementIndex, i >= elementIndex);
                if (i == elementIndex) _elements[i].SetBackground(_currentStageIcon);
            }
        }
    }
}