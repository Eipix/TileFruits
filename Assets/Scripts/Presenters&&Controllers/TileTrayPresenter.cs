using System;
using Gameplay.Tray;
using Zenject;

namespace Presenters__Controllers
{
    public class TileTrayPresenter : IInitializable, IDisposable
    {
        private readonly TileTray _tileTray;
        private readonly TileTrayView _tileTrayView;

        public TileTrayPresenter(TileTray tileTray, TileTrayView tileTrayView)
        {
            _tileTray = tileTray;
            _tileTrayView = tileTrayView;
        }

        public void Initialize()
        {
            _tileTray.Added += _tileTrayView.Insert;
            _tileTray.MatchCleared += _tileTrayView.Match;
        }

        public void Dispose()
        {
            _tileTray.Added -= _tileTrayView.Insert;
            _tileTray.MatchCleared -= _tileTrayView.Match;
        }
    }
}
