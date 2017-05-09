using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{

    public class VNHandler : MonoBehaviour
    {
        public bool VNMode = false;
        public int VNFontSize = 16;

        private Text Text;
        private RectTransform Textbox;

        private TextboxSettings VNSay;

        private Text CharacterName;

        // Use this for initialization
        void Start()
        {
            Text = GameObject.Find("Text").GetComponent<Text>();
            Textbox = GameObject.Find("Text").GetComponent<RectTransform>();
            if(VNMode)
                CharacterName = (GameObject.Find("CharName") != null ? GameObject.Find("CharName").GetComponent<Text>() : null);
            
            VNSay = new TextboxSettings(67.5f, 49.5f, 154.5f, 80.0f);

            if(VNMode == true)
            {
                Text.fontSize = VNFontSize;
                Text.resizeTextMaxSize = VNFontSize;
                Textbox.offsetMin = new Vector2(VNSay.L, VNSay.B);
                Textbox.offsetMax = new Vector2(-VNSay.R, -VNSay.T);
            }
        }

        public void UpdateCharacterName(M22.script_character _input)
        {
            if (CharacterName == null) return;
            CharacterName.text = _input.name;
            CharacterName.color = _input.color;
        }

        public void ClearCharacterName()
        {
            if(CharacterName != null) CharacterName.text = "";
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class TextboxSettings
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

[CustomEditor(typeof(M22.VNHandler))]
public class VNHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as M22.VNHandler;

        myScript.VNMode = GUILayout.Toggle(myScript.VNMode, "VN Mode");

        if (myScript.VNMode)
            myScript.VNFontSize = EditorGUILayout.IntSlider("Font size:", myScript.VNFontSize, 8, 64);

    }
}