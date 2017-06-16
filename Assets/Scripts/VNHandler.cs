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
        public int NovelFontSize = 34;

        private Sprite TextboxNarrative;
        private Sprite TextboxDialogue;
        private Sprite TextboxNovel;
        public GameObject CharacterPrefab;

        private Text Text;
        private RectTransform Textrect;

        private Image Textbox;

        private TextboxSettings VN;
        private TextboxSettings VNSay;

        private TextboxSettings Novel;
        private TextboxSettings NovelTxt;

        private Text CharacterName;

        static private Dictionary<string, Character> loadedCharacters;

        private void Awake()
        {
            TextboxNarrative = Resources.Load<Sprite>("TextBoxes/bg-narration");
            TextboxDialogue = Resources.Load<Sprite>("TextBoxes/bg-say");
            TextboxNovel = Resources.Load<Sprite>("TextBoxes/textbox_big");

            if (TextboxDialogue == null || TextboxNarrative == null || TextboxNovel == null)
                Debug.LogError("Failed to load a textbox! Check your \"Resources/Textboxes\" folder!");
        }
        
        void Start()
        {
            loadedCharacters = new Dictionary<string, Character>();
            Text = GameObject.Find("M22ScriptText").GetComponent<Text>();
            Textrect = GameObject.Find("M22ScriptText").GetComponent<RectTransform>();
            Textbox = GameObject.Find("Textbox").GetComponent<Image>();
            CharacterName = (GameObject.Find("CharName") != null ? GameObject.Find("CharName").GetComponent<Text>() : null);

            VN = new TextboxSettings(
                0, 
                0, 
                200, 
                400, 
                0, 0, 1, 0);
            VNSay = new TextboxSettings(67.5f, 49.5f, 134, 64);

            Novel = new TextboxSettings(
                0,
                0,
                0,
                0,
                0, 0, 1, 1);
            NovelTxt = new TextboxSettings(50, 50, 80, 80);

            if (VNMode == true)
            {
                SetTextBoxToVN();
            }
            else
            {
                SetTextBoxToIN();
            }
        }

        private void SetTextBoxToVN()
        {
            if (System.String.Equals(CharacterName.text, ""))
                Textbox.sprite = TextboxNarrative;
            else
                Textbox.sprite = TextboxDialogue;

            RectTransform temp = Textbox.GetComponent<RectTransform>();
            temp.anchorMin = new Vector2(VN.AnchMinX, VN.AnchMinY);
            temp.anchorMax = new Vector2(VN.AnchMaxX, VN.AnchMaxY);
            temp.anchoredPosition = new Vector2(0, VN.T);
            temp.sizeDelta = new Vector2(0, VN.B);
            Text.fontSize = VNFontSize;
            Text.resizeTextMaxSize = VNFontSize;
            Textrect.offsetMin = new Vector2(VNSay.L, VNSay.B);
            Textrect.offsetMax = new Vector2(-VNSay.R, -VNSay.T);
            CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 1);
        }

        private void SetTextBoxToIN()
        {
            Textbox.sprite = TextboxNovel;
            RectTransform temp = Textbox.GetComponent<RectTransform>();
            temp.anchorMin = new Vector2(Novel.AnchMinX, Novel.AnchMinY);
            temp.anchorMax = new Vector2(Novel.AnchMaxX, Novel.AnchMaxY);
            temp.anchoredPosition = new Vector2(0, Novel.T);
            temp.sizeDelta = new Vector2(0, Novel.B);

            Text.fontSize = NovelFontSize;
            Text.resizeTextMaxSize = NovelFontSize;
            Textrect.anchorMax = new Vector2(1, 1);
            Textrect.offsetMin = new Vector2(NovelTxt.L, NovelTxt.B);
            Textrect.offsetMax = new Vector2(-NovelTxt.R, -NovelTxt.T);
            CharacterName.color = new Color(CharacterName.color.r, CharacterName.color.g, CharacterName.color.b, 0);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                ToggleVNMode();
            }
        }

        public void ToggleVNMode()
        {
            VNMode = !VNMode;
            if(VNMode == true)
            {
                SetTextBoxToVN();
            }
            else
            {
                SetTextBoxToIN();
            }
        }

        public void UpdateCharacterName(M22.script_character _input)
        {
            if (CharacterName == null || VNMode == false) return;
            CharacterName.text = _input.name;
            CharacterName.color = _input.color;
            Textbox.sprite = TextboxDialogue;

        }

        public void ClearCharacterName()
        {
            if (CharacterName == null || VNMode == false) return;
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

            GameObject tempGO = GameObject.Find(_charname);
            if(tempGO == null)
            {
                tempGO = GameObject.Instantiate<GameObject>(CharacterPrefab, GameObject.Find("Characters").transform);
                tempGO.name = _charname; //+ "-" + _modifier;
            }
            tempGO.GetComponent<Image>().sprite = tempSpr;
            tempGO.GetComponent<RectTransform>().offsetMin = new Vector2(tempGO.GetComponent<RectTransform>().offsetMin.x + _x, tempGO.GetComponent<RectTransform>().offsetMin.y);
        }
    }

    public struct TextboxSettings
    {
        public float L, R, T, B;
        public float AnchMinX, AnchMinY, AnchMaxX, AnchMaxY;
        public TextboxSettings(float _L, float _R, float _T, float _B)
        {
            L = _L;
            R = _R;
            T = _T;
            B = _B;
            AnchMinX = 0;
            AnchMinY = 0;
            AnchMaxX = 0;
            AnchMaxY = 0;
        }
        public TextboxSettings(float _L, float _R, float _T, float _B, float _AnchMinX, float _AnchMinY, float _AnchMaxX, float _AnchMaxY)
        {
            L = _L;
            R = _R;
            T = _T;
            B = _B;
            AnchMinX = _AnchMinX;
            AnchMinY = _AnchMinY;
            AnchMaxX = _AnchMaxX;
            AnchMaxY = _AnchMaxY;
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
            myScript.CharacterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", myScript.CharacterPrefab, typeof(GameObject), false);
        }
        else
        {
            myScript.NovelFontSize = EditorGUILayout.IntSlider("Font size:", myScript.NovelFontSize, 8, 64);
        }

    }
}
#endif