using Gameplay;
using Gameplay.Levels;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace View
{
    public class DebugButtons : MonoBehaviour
    {
        [SerializeField, Range(0, 1), OnValueChanged(nameof(ChangeTimeScale))]
        private float _timeScale;
        
        private LevelManager _levelManager;
        private GameManager _gameManager;

        [Inject]
        private void Construct(LevelManager levelManager, GameManager gameManager)
        {
            _levelManager = levelManager;
            _gameManager = gameManager;
        }

        private void ChangeTimeScale()
        {
            Time.timeScale = _timeScale;
        }
        
        [Button]
        private void Restart()
        {
            _gameManager.Restart();
            Debug.Log($"Restarted Level with index {_levelManager.LevelIndex + 1}, id {_levelManager.CurrentLevel.Id}");
        }

        [Button]
        private void StartLevel()
        {
            _levelManager.StartLevel();
            Debug.Log($"Started Level with index {_levelManager.LevelIndex + 1}, id {_levelManager.CurrentLevel.Id}");
        }
    }
}
