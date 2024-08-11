using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beanc16.Common.Dialogue
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogueScriptableObject", order = 100)]
    public class DialogueScriptableObject : ScriptableObject
    {
        [TextAreaAttribute]
        public List<string> lines = new List<string>();



        public override string ToString()
        {
            string lineText = "";
            lines.ForEach(line => lineText += line + "\n\t");

            return base.ToString() + "\n"
                + "{" + "\n\t"
                + "lines: [" + "\n\t"
                + lineText
                + "]" + "\n"
                + "}";
        }
    }
}
