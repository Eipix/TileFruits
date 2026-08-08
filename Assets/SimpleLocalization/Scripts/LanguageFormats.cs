namespace Assets.SimpleLocalization.Scripts
{
    public struct Formats
    {
        public readonly ELanguage Enumerator;
        public readonly string ISO_639_1;
        public readonly string Translated;

        public Formats(ELanguage enumerator, string iso_639_1, string translated)
        {
            Enumerator = enumerator;
            ISO_639_1 = iso_639_1;
            Translated = translated;
        }
    }
}
