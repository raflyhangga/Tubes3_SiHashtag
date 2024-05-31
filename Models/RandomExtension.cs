using System;

public static class RandomExtension
{
    private static Random random = new Random();
    public static object Choice (this Random rand, params object[] array){
        int randomInt = random.Next(0, array.Length);
        return array[randomInt];
    }
}