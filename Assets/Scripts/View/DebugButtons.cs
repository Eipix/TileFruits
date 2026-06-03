using Gameplay;
using Gameplay.Levels;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace View
{
    public class DebugButtons : MonoBehaviour
    {
        private LevelManager _levelManager;
        private GameManager _gameManager;

        [Inject]
        private void Construct(LevelManager levelManager, GameManager gameManager)
        {
            _levelManager = levelManager;
            _gameManager = gameManager;
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
