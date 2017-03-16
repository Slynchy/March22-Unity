using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCompiler
{
	private static void ThisIsABreakpoint()
	{
		bool lol;
		return;
	}

    private static bool IsNewLine(string s)
    {
        return s == "\r\n";
    }
    static public List<M22.Script.line_c> CompileScript(string filename)
    {
        var result = new List<M22.Script.line_c>(); 
        var file = Resources.Load(filename) as TextAsset;
        if (!file)
        {
            Debug.Log("failed to load file");
            return result;
        }

        var scriptLines = new List<string>();
        string currentLine = "";
        for (int i = 0; i < file.text.Length; i++)
        {
            currentLine += file.text[i];
            if(file.text[i] == '\n')
            {
                // new line
                scriptLines.Add(currentLine);
                currentLine = "";
            }
		}

		scriptLines.RemoveAll (IsNewLine);
		scriptLines.
		ThisIsABreakpoint ();
        return new List<M22.Script.line_c>();
    }
}
