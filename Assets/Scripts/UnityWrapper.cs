using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace M22
{
    class UnityWrapper
    {

        public static string LoadTextFileToString(string _filename)
        {
            return (Resources.Load(_filename) as TextAsset).text;
        }

    }
}
