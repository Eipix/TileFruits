using Gameplay;
using Gameplay.Tray;

namespace Generator.DistributionStrategies.Base
{
    public readonly struct DistributionSettings
    {
        public readonly TileMap TileMap;
        public readonly TileList TileList;
        public readonly TileTraySettings TraySettings;

        public DistributionSettings(TileMap map, TileList tileList, TileTraySettings tileTraySettings)
            => (TileMap, TileList, TraySettings) = (map, tileList, tileTraySettings);
    }
}
