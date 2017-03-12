using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{

    namespace Script
    {
        public class TypeWriterScript : MonoBehaviour
        {

            public float TextSpeed = 25.0f;

            private string currentLine = "";
            private float strPos = 0.0f;
            private int currStrPos = 0;
            private Text page;
            private bool LineComplete = false;

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

            private void Awake()
            {
                Reset(true);
            }

            public bool IsLineComplete() { return LineComplete; }

            void Start()
            {
                page = this.GetComponent<Text>();
            }

            void FinishLine()
            {
                page.text += currentLine.Substring(currStrPos);
                page.text += "\n\n";
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

                if (LineComplete == true) return;
                if (currentLine == "") return;

                if (strPos < currentLine.Length)
                    strPos += TextSpeed * Time.deltaTime;

                if (Mathf.Floor(strPos) > currStrPos)
                {
                    if (currStrPos < currentLine.Length)
                    {
                        page.text += currentLine.Substring(currStrPos, 1);
                        currStrPos++;
                    }
                }

                if (currStrPos == currentLine.Length)
                {
                    page.text += "\n\n";
                    LineComplete = true;
                }
            }
        }

    }
}