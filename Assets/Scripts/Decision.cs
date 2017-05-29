using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Decision : MonoBehaviour {

    private List<string> choiceStrings;
    private List<string> choiceFlags;

    private Text[] choiceTxt;
    private List<Button> choiceButtons;

    private M22.ScriptMaster scriptMastRef;

    public void Initialize(
        string _choice1string,
        string _choice1flag,
        string _choice2string,
        string _choice2flag
        )
    {
        choiceStrings = new List<string>();
        choiceStrings.Add(_choice1string);
        choiceStrings.Add(_choice2string);
        choiceStrings.Add("");

        choiceFlags = new List<string>();
        choiceFlags.Add(_choice1flag);
        choiceFlags.Add(_choice2flag);
        choiceFlags.Add("");
    }

    public void Initialize(
        string _choice1string,
        string _choice1flag,
        string _choice2string,
        string _choice2flag,
        string _choice3string,
        string _choice3flag
        )
    {
        choiceStrings = new List<string>();
        choiceStrings.Add(_choice1string);
        choiceStrings.Add(_choice2string);
        choiceStrings.Add(_choice3string);

        choiceFlags = new List<string>();
        choiceFlags.Add(_choice1flag);
        choiceFlags.Add(_choice2flag);
        choiceFlags.Add(_choice3flag);
    }

    void Start () {
        choiceTxt = GetComponentsInChildren<Text>();
        for (int i = 0; i < 3; i++)
            choiceTxt[i].text = choiceStrings[i];

        choiceButtons = new List<Button>();
        foreach (var item in choiceTxt)
        {
            choiceButtons.Add(item.GetComponentInParent<Button>());
            choiceButtons[choiceButtons.Count-1].onClick.AddListener(delegate () { HandleChoice(item.text); });
        }

        // i.e. it equals nothing
        if(choiceStrings[2].Equals(""))
        {
            Destroy(choiceButtons[2].gameObject);
            choiceButtons.RemoveAt(2);
        }

        scriptMastRef = Camera.main.GetComponent<M22.ScriptMaster>();
    }
    
    void HandleChoice(string _choice)
    {
        Debug.Log(_choice);
        Destroy(this.gameObject);
        int stringIndex = choiceStrings.FindIndex(a => a == _choice);
        if (scriptMastRef.SCRIPT_FLAGS.Contains(choiceFlags[stringIndex]))
        {
            // flag exists? shouldn't happen
            Debug.LogError("MakeDecision error; setting flag that already exists! Use unique identifiers in your decisions!");
        }
        else
        {
            scriptMastRef.SCRIPT_FLAGS.Add(choiceFlags[stringIndex]);
        }
        scriptMastRef.NextLine();
    }
}
