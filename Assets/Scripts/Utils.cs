using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixel_Slayer
{
    public static class SlayerUtils
    {
        public static Vector3 GetRandomDir()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized; // нормализованный вектор со случайными
                                                                                         // значениями по X и Y от 1 до -1
        }
    }
}
