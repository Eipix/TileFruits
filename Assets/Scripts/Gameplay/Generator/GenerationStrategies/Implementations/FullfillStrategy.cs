using System;
using Generator.GenerationStrategies.Base;
using UnityEngine;

namespace Generator.GenerationStrategies.Implementations.Fullfill
{
    [Serializable]
    public class FullfillStrategy : GenerationStrategy
    {
        [field: SerializeField, Min(1)] public int Layers { get; private set; }

        protected override void OnGenerateShape()
        {
            int layers = Layers;
            
            for (int i = 0; i < layers; i++)
            {
                Map.CoverLayer(i, out _);
            }
        }
    }
}
