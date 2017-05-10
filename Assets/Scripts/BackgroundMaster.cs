using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace M22
{
    class BackgroundMaster : MonoBehaviour
    {
        static Dictionary<string, Sprite> loadedBackgrounds;
        static private Sprite black;
        static private Sprite white;

        void Awake()
        {
            Texture2D tempTex = Resources.Load("black") as Texture2D;
            black = Sprite.Create(tempTex, new Rect(0, 0, tempTex.width, tempTex.height), new Vector2(0, 0));
            tempTex = Resources.Load("white") as Texture2D;
            white = Sprite.Create(tempTex, new Rect(0, 0, tempTex.width, tempTex.height), new Vector2(0, 0));
            if (black == null || white == null) Debug.LogError("Failed to load black or white texture from Resources!");
            loadedBackgrounds = new Dictionary<string, Sprite>();
        }

        public static bool LoadBackground(string name)
        {
            if (name == "black")
                return true;
            else if (name == "white")
                return true;
            if (loadedBackgrounds.ContainsKey(name) == true)
                return true;
            string filename = "Backgrounds/" + name;
            Texture2D temp = Resources.Load(filename) as Texture2D;
            if (temp)
            {
                Sprite tempSpr = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0, 0));
                loadedBackgrounds.Add(name, tempSpr);
                return true;
            }
            else
            {
                Debug.LogError("Failed to load background: " + filename);
                return false;
            }
        }

        static public Sprite GetBackground(string file)
        {
            if (file == "black")
                return black;
            else if (file == "white")
                return white;
            if (loadedBackgrounds.ContainsKey(file))
            {
                Sprite returnVal;
                loadedBackgrounds.TryGetValue(file, out returnVal);
                return returnVal;
            }
            else
            {
                Debug.LogError("Failed to find background: " + file);
                return new Sprite();
            }
        }

        void UnloadBackgrounds()
        {
            loadedBackgrounds.Clear();
        }
    }
}
