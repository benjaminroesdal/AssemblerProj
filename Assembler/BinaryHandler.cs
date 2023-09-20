using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler
{
    public class BinaryHandler : IBinaryHandler
    {
        private readonly AsmHandler AsmHandler;
        public BinaryHandler()
        {
            AsmHandler = new AsmHandler();
        }

        /// <summary>
        /// Takes ASM string and converts to a string of binary instructions.
        /// </summary>
        /// <param name="asmText">Asm string to convert.</param>
        /// <returns>string of binary instructions.</returns>
        public string AsmToBinaryText(string asmText)
        {
            asmText = StringHelper.RemoveComments(asmText.Trim());
            AsmHandler.MapAsmToBinaryCollections(asmText.Trim());
            return WriteHackText(asmText.Trim());
        }

        /// <summary>
        /// Constructs a string based on binary instructions collections each instruction on a new line.
        /// Takes an ASM string to base the binary string on, to keep line count aligned.
        /// </summary>
        /// <param name="asmText">ASM string to base the HACK file on.</param>
        /// <returns>string of binary instructions</returns>
        private string WriteHackText(string asmText)
        {
            var asmLines = asmText.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var binaryString = "";
            foreach (var line in asmLines)
            {
                var trimmedAsmLine = line.Trim();
                if (AsmHandler.AInstructions.TryGetValue(trimmedAsmLine, out string binaryAValue))
                {
                    binaryString += binaryAValue + "\n";
                }
                if (AsmHandler.CInstructions.TryGetValue(trimmedAsmLine, out string binaryCValue))
                {
                    binaryString += binaryCValue + "\n";
                }
                if (AsmHandler.DefinedSymbols.TryGetValue(trimmedAsmLine, out string binaryPreSymbol))
                {
                    binaryString += binaryPreSymbol + "\n";
                }
                if (AsmHandler.UserDefinedSymbols.TryGetValue(trimmedAsmLine, out string binaryUserDefSymbol))
                {
                    binaryString += binaryUserDefSymbol + "\n";
                }
                if (AsmHandler.UserDefinedLabels.TryGetValue(trimmedAsmLine, out string binaryUserDefinedLabels))
                {
                    binaryString += binaryUserDefinedLabels + "\n";
                }
            }
            return binaryString;
        }
    }
}
