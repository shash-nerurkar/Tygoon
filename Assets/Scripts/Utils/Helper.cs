using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Helper
{
    public static float RandomFloat01 ( ) => Random.Range ( 0.0f, 1.0f );
    
    public static float RandomFloatGreaterThan0_5 ( ) => Random.Range ( 0.5f, 1.0f );
    
    public static int GetEnumLength ( Type enumType ) => Enum.GetNames ( enumType ).Length;

    
    public static Color GetRandomBrightColor ( ) => new ( RandomFloatGreaterThan0_5 ( ), RandomFloatGreaterThan0_5 ( ), RandomFloatGreaterThan0_5 ( ) );
}
