using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace M22
{

    public class Character
    {
        // string = modifier, sprite = sprite!
        public Dictionary<string, Sprite> sprites;
        public Character()
        {
            sprites = new Dictionary<string, Sprite>();
        }
    }

    public class VNHandler : MonoBehaviour
    {
        public bool VNMode = false;
        public int VNFontSize = 16;
        public Sprite TextboxNarrative;
        public Sprite TextboxDialogue;
        public GameObject CharacterPrefab;

        private Text Text;
        private RectTransform Textrect;

        private Image Textbox;

        private TextboxSettings VNSay;

        private Text CharacterName;

        static private Dictionary<string, Character> loadedCharacters;
        
        void Start()
        {
            loadedCharacters = new Dictionary<string, Character>();
            Text = GameObject.Find("Text").GetComponent<Text>();
            Textrect = GameObject.Find("Text").GetComponent<RectTransform>();
            Textbox = GameObject.Find("Textbox").GetComponent<Image>();
            if (VNMode)
                CharacterName = (GameObject.Find("CharName") != null ? GameObject.Find("CharName").GetComponent<Text>() : null);
            
            VNSay = new TextboxSettings(67.5f, 49.5f, 154.5f, 80.0f);

            if(VNMode == true)
            {
                Text.fontSize = VNFontSize;
                Text.resizeTextMaxSize = VNFontSize;
                Textrect.offsetMin = new Vector2(VNSay.L, VNSay.B);
                Textrect.offsetMax = new Vector2(-VNSay.R, -VNSay.T);
            }
        }

        public void UpdateCharacterName(M22.script_character _input)
        {
            if (CharacterName == null) return;
            CharacterName.text = _input.name;
            CharacterName.color = _input.color;
            Textbox.sprite = TextboxDialogue;

        }

        public void ClearCharacterName()
        {
            if (CharacterName == null) return;
            CharacterName.text = "";
            Textbox.sprite = TextboxNarrative;
        }

        public void ClearCharacters()
        {
            GameObject charParent = GameObject.Find("Characters");
            for (int i = 0; i < charParent.transform.childCount; i++)
            {
                var obj = charParent.transform.GetChild(i).gameObject.GetComponent<FadeOutImage>();
                obj.FadeOut();
            }
        }

        static public bool LoadCharacter(string _charname, string _modifier)
        {
            string path = "Characters/" + _charname + "/" + _modifier;
            if (loadedCharacters.ContainsKey(_charname))
            {
                Character temp;
                loadedCharacters.TryGetValue(_charname, out temp);
                if (temp.sprites.ContainsKey(_modifier)) return true;
                Texture2D tempTex = Resources.Load(path) as Texture2D;
                Sprite tempSpr = Sprite.Create(tempTex, new Rect(0, 0, tempTex.width, tempTex.height), new Vector2(0, 0));
                if (tempSpr != null)
                {
                    temp.sprites.Add(_modifier, tempSpr);
                    return true;
                }
                else return false;
            }
            else
            {
                Character temp = new Character();
                Texture2D tempTex = Resources.Load(path) as Texture2D;
                Sprite tempSpr = Sprite.Create(tempTex, new Rect(0, 0, tempTex.width, tempTex.height), new Vector2(0, 0));
                if (tempSpr != null)
                {
                    temp.sprites.Add(_modifier, tempSpr);
                    loadedCharacters.Add(_charname, temp);
                    return true;
                }
                else return false;
            }
        }

        public void CreateCharacter(string _charname, string _modifier, int _x)
        {
            if (!VNMode) return;
            Character temp;
            loadedCharacters.TryGetValue(_charname, out temp);
            Sprite tempSpr;
            temp.sprites.TryGetValue(_modifier, out tempSpr);

            GameObject tempGO = GameObject.Instantiate<GameObject>(CharacterPrefab, GameObject.Find("Characters").transform);
            tempGO.name = _charname + "-" + _modifier;
            tempGO.GetComponent<Image>().sprite = tempSpr;
            tempGO.GetComponent<RectTransform>().offsetMin = new Vector2(tempGO.GetComponent<RectTransform>().offsetMin.x + _x, tempGO.GetComponent<RectTransform>().offsetMin.y);
        }
    }

    public struct TextboxSettings
    {
        public float L, R, T, B;
        public TextboxSettings(float _L, float _R, float _T, float _B)
        {
            L = _L;
            R = _R;
            T = _T;
            B = _B;
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(M22.VNHandler))]
public class VNHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as M22.VNHandler;

        myScript.VNMode = GUILayout.Toggle(myScript.VNMode, "VN Mode");

        if (myScript.VNMode)
        {
            myScript.VNFontSize = EditorGUILayout.IntSlider("Font size:", myScript.VNFontSize, 8, 64);
            myScript.TextboxDialogue = (Sprite)EditorGUILayout.ObjectField("Dialogue textbox", myScript.TextboxDialogue, typeof(Sprite), false);
            myScript.TextboxNarrative = (Sprite)EditorGUILayout.ObjectField("Narrative textbox", myScript.TextboxNarrative, typeof(Sprite), false);
            myScript.CharacterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", myScript.CharacterPrefab, typeof(GameObject), false);
        }

    }
}
#endif