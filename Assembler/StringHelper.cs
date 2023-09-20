using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler
{
    public static class StringHelper
    {
        /// <summary>
        /// Removes text comments from ASM string
        /// </summary>
        /// <param name="asmText">ASM string to remove comments from</param>
        /// <returns>formatted string.</returns>
        public static string RemoveComments(string asmText)
        {
            asmText = Regex.Replace(asmText, "(\\/\\/((?!\\*\\/).)*)(?!\\*\\/)[^\\r\\n]", "");
            return asmText;
        }
    }
}
