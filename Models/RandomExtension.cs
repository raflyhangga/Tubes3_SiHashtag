using System;

public static class RandomExtension
{
    public static object Choice (this Random rand, params object[] array){
        int randomInt = Random.Shared.Next(0, array.Length);
        return array[randomInt];
    }
}