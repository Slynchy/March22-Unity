using System;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{
    public class TypeWriterScript : MonoBehaviour
    {

        public float TextSpeed = 25.0f;

        private float TextSpeedMultiplier = 1.0f;
        private string currentLine = "";
        private float strPos = 0.0f;
        private int currStrPos = 0;
        private Text page;
        private bool LineComplete = false;
        private ScriptMaster scriptMaster;

        private bool inLineFuncComplete = true;

        public bool SetTextSpeed(float _newSpeed)
        {
            if (_newSpeed > 0)
                TextSpeedMultiplier = _newSpeed;
            else
                return false;
            return true;
        }
        public float GetTextSpeed()
        {
            return TextSpeedMultiplier;
        }

        public void SetNewCurrentLine(string _newStr)
        {
            currentLine = _newStr;
            this.Reset(false);
        }

        public void Reset(bool _clearPage)
        {
            if (page != null && _clearPage == true)
                page.text = "";
            currStrPos = 0;
            strPos = 0.0f;
            LineComplete = false;
        }

        public void Reset(bool _clearPage, M22.ScriptMaster.VoidDelegateWithBool callbackFunc)
        {
            if (page != null && _clearPage == true)
                page.text = "";
            currStrPos = 0;
            strPos = 0.0f;
            LineComplete = false;
            callbackFunc();
        }

        public void SetParent(ScriptMaster _scriptMaster) { scriptMaster = _scriptMaster; }

        private void Awake()
        {
            Reset(true);
        }

        public bool IsLineComplete() { return LineComplete; }

        public void FinishedInlineFunction()
        {
            inLineFuncComplete = true;
        }

        void Start()
        {
            page = this.GetComponent<Text>();
        }

        void FinishLine()
        {
            page.text += currentLine.Substring(currStrPos);
            page.text += '\n';
            strPos = currentLine.Length;
            currStrPos = currentLine.Length;
            LineComplete = true;
        }

        public void FireInput()
        {
            if (currStrPos != currentLine.Length)
            {
                FinishLine();
                return;
            }
            LineComplete = false;
            strPos = 0;
            currStrPos = 0;
            // update current line to newest line in master script here
        }

        // Update is called once per frame
        void Update()
        {
            if (inLineFuncComplete == false) return;
            if (LineComplete == true) return;
            if (currentLine == null || String.Equals(currentLine, "")) return;

            if (strPos < currentLine.Length)
                strPos += TextSpeed * TextSpeedMultiplier * Time.deltaTime;

            if (Mathf.Floor(strPos) > currStrPos)
            {
                if (currStrPos < currentLine.Length)
                {
                    string substr = currentLine.Substring(currStrPos, 1);

                    if (substr.Equals("{"))
                    {
                        substr = currentLine.Substring(currStrPos, 2);
                        if (substr.Equals("{{"))
                        {
                            //inline function!
                            string function;
                            int pos = currentLine.IndexOf("}}");
                            function = currentLine.Substring(currStrPos + 2, pos - currStrPos - 2);
                            M22.line_c tempLineC = M22.ScriptCompiler.CompileLine(ref function, -1);
                            currStrPos += pos - currStrPos + 2;
                            substr = currentLine.Substring(currStrPos, 1);
                            scriptMaster.SetCurrentInlineFunction(tempLineC);
                            scriptMaster.ExecuteFunction(tempLineC, true);
                            inLineFuncComplete = false;
                            return;
                        }
                    }

                    page.text += substr;
                    currStrPos++;
                }
            }

            if (currStrPos == currentLine.Length)
            {
                page.text += '\n';
                LineComplete = true;
            }
        }
    }

}