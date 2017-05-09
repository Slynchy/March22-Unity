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
            black = Resources.Load("black") as Sprite;
            white = Resources.Load("white") as Sprite;
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
