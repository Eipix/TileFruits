using System;
using Constants;
using UnityEngine;

namespace Gameplay.Tray
{
    [Serializable]
    public struct TileTraySettings
    {
        [field: SerializeField, Min(MahjongConstants.TilesPerMatch)]
        public int Capacity { get; private set; }
    }
}
