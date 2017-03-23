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

        void Awake()
        {
            loadedBackgrounds = new Dictionary<string, Sprite>();
        }

        public static bool LoadBackground(string name)
        {
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
