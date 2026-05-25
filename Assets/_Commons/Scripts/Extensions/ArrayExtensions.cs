
namespace Commons.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] array, T item)
        {
            int length = array.Length;
            var newArray = new T[length + 1];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = array[i];
            }

            newArray[length] = item;
            return newArray;
        }

        public static T[] Remove<T>(this T[] array, T item)
        {
            int index = System.Array.IndexOf(array, item);

            if (index < 0)
                return (T[])array.Clone();

            T[] newArray = new T[array.Length - 1];

            for (int i = 0, j = 0; i < array.Length; i++)
            {
                if (i == index)
                    continue;

                newArray[j++] = array[i];
            }

            return newArray;
        }
    }
}
