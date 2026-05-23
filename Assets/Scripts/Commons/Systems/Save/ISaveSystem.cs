namespace Commons.Systems.Save
{
    public interface ISaveSystem
    {
        void Save(string key, object value);

        T Load<T>(string key, T defaultValue = default);
    }
}
