namespace Commons
{
    [System.Serializable]
    public struct Range
    {
        public float min;
        public float max;

        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float GetRandom() => UnityEngine.Random.Range(min, max);

        public float GetRandom(System.Random random) => min + ((float)random.NextDouble() * (max - min));
    }
}
