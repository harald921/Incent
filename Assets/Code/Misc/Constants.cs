using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Constants
{
    public static string APP_NAME = "Inscent";

    public static class Terrain
    {
        public static string CHUNK_SAVE_FOLDER = @"C:\" + APP_NAME;
        public static int CHUNK_SIZE = 64;
        public static int WORLD_SIZE = 16;
        public static int RENDER_DISTANCE = 3;
    }
}
