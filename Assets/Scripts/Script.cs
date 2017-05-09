using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace M22
{
    namespace Script
    {
        public class Script
        {
            private List<line_c> compiledLines;
            private List<Texture2D> loadedBackgrounds;

            public line_c GetLine(int index)
            {
                if (compiledLines.Count == 0 || index >= compiledLines.Count)
                {
                    line_c temp = new line_c();
                    temp.m_lineType = LINETYPE.NUM_OF_LINETYPES;
                    return temp;
                }
                if (index < 0) return compiledLines[0];
                return compiledLines[index];
            }

            public int Length()
            {
                return compiledLines.Count;
            }

            public int AddLine(line_c _input)
            {
                compiledLines.Add(_input);
                return compiledLines.Count-1;
            }

            public int AddBackground(Texture2D _input)
            {
                loadedBackgrounds.Add(_input);
                return loadedBackgrounds.Count - 1;
            }

            public int AddBackground(string _input)
            {
                Texture2D temp = Resources.Load<Texture2D>("Backgrounds/" + _input);
                if (temp == null)
                    Debug.LogError("Failed to load background: " + _input);
                loadedBackgrounds.Add(temp);
                return loadedBackgrounds.Count - 1;
            }

            public Texture2D GetBackground(string _input)
            {
                foreach (var item in loadedBackgrounds)
                {
                    if (item.name == _input) return item;
                }
                return default(Texture2D);
            }

            public Script()
            {
                compiledLines = new List<line_c>();
                loadedBackgrounds = new List<Texture2D>();
            }

            ~Script()
            {
                compiledLines.Clear();
                loadedBackgrounds.Clear();
            }
        }
    }
}