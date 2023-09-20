
namespace Assembler
{
    /// <summary>
    /// This class is responsible for handling ASM language
    /// It will convert ASM expressions to binary expressions and store them in collections available for lookup
    /// </summary>
    public class AsmHandler
    {
        public Dictionary<string, string> AInstructions = new Dictionary<string, string>();
        public Dictionary<string, string> CInstructions = new Dictionary<string, string>();
        public Dictionary<string, string> DefinedSymbols = new Dictionary<string, string>();
        public Dictionary<string, string> UserDefinedSymbols = new Dictionary<string, string>();
        public Dictionary<string, string> UserDefinedLabels = new Dictionary<string, string>();

        /// <summary>
        /// Populates collections of binary values based on ASM text provided in parameter.
        /// </summary>
        /// <param name="asmText">ASM text to convert to binary.</param>
        public void MapAsmToBinaryCollections(string asmText)
        {
            var asmLines = asmText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            AddLabelValues(asmLines, UserDefinedLabels);
            for (int i = 0; i < asmLines.Length; i++)
            {
                if (IsLineUserDefinedSymbol(asmLines[i], AsmValues.SymbolReferences, UserDefinedLabels))
                {
                    UserDefinedAsmToBinary(asmLines[i], UserDefinedSymbols);
                }
                if (IsLinePredefinedSymbol(asmLines[i], AsmValues.SymbolReferences))
                {
                    PredefinedAsmToBinary(asmLines[i], DefinedSymbols);
                }
                if (IsLineAInstruction(asmLines[i]))
                {
                    AInstructionAsmToBinary(asmLines[i], AInstructions);
                }
                if (IsLineCInstruction(asmLines[i]))
                {
                    CInstructionAsmToBinary(asmLines[i], CInstructions);
                }
            }
        }

        /// <summary>
        /// Converts User defined ASM instruction line to binary value and adds to collection
        /// </summary>
        /// <param name="line">Line to convert</param>
        /// <param name="collection">Collection to add binary value to</param>
        private void UserDefinedAsmToBinary(string line, Dictionary<string, string> collection)
        {
            var definedSymbol = GetUserDefinedSymbol(line.Trim(), collection);
            collection.TryAdd(definedSymbol.label, definedSymbol.binary);
        }

        /// <summary>
        /// Converts predefined ASM instruction line to binary value and adds to collection.
        /// </summary>
        /// <param name="line">Line to convert</param>
        /// <param name="collection">Collection to add binary value to</param>
        private void PredefinedAsmToBinary(string line, Dictionary<string, string> collection)
        {
            var predefinedSymbol = GetPreDefinedSymbol(line.Trim());
            collection.TryAdd(line.Trim(), predefinedSymbol.binary);
        }

        /// <summary>
        /// Converts ASM A instruction line to binary value and adds to collection.
        /// </summary>
        /// <param name="line">Line to convert</param>
        /// <param name="collection">Collection to add binary value to.</param>
        private void AInstructionAsmToBinary(string line, Dictionary<string, string> collection)
        {
            var aInstruction = GetAInstruction(line);
            collection.TryAdd(aInstruction.label, aInstruction.binary);
        }

        /// <summary>
        /// Converts ASM C instruction line to binary value and adds to collection.
        /// </summary>
        /// <param name="line">Line to convert</param>
        /// <param name="collection">Collection to add binary value to.</param>
        private void CInstructionAsmToBinary(string line, Dictionary<string, string> collection)
        {
            var cInstruction = GetCInstruction(line);
            collection.TryAdd(cInstruction.label, cInstruction.binary);
        }

        /// <summary>
        /// Gets pre defined symbol value based on collection of defined symbols.
        /// </summary>
        /// <param name="line">Line to check for value.</param>
        /// <returns>Tuple containing translated label, and its binary value.</returns>
        private (string label, string binary) GetPreDefinedSymbol(string line)
        {
            var trimmedLine = line.Trim().Remove(0, 1);
            AsmValues.SymbolReferences.TryGetValue(trimmedLine, out var symbol);
            int value = Convert.ToInt16(symbol);
            var binaryValue = Convert.ToString(value, 2);
            binaryValue = binaryValue.PadLeft(16, '0');
            return (line, binaryValue);
        }

        /// <summary>
        /// Gets user defined symbol value based on current collection, and converts to binary.
        /// </summary>
        /// <param name="line">User defined variable line.</param>
        /// <param name="collection">User defined variables</param>
        /// <returns>Tuple containing translated label, and its binary value.</returns>
        private (string label, string binary) GetUserDefinedSymbol(string line, Dictionary<string, string> collection)
        {
            int value = GetUserDefinedSymbolValue(collection);
            var binaryValue = Convert.ToString(value, 2);
            binaryValue = binaryValue.PadLeft(16, '0');
            return (line.Trim(), binaryValue);
        }

        /// <summary>
        /// Finds A instruction value by parsing line to int.
        /// </summary>
        /// <param name="line">A instruction line</param>
        /// <returns>Tuple containing translated label, and its binary value.</returns>
        private (string label, string binary) GetAInstruction(string line)
        {
            var trimmedLine = line.Trim().Remove(0, 1);
            var value = int.Parse(trimmedLine);
            int value16 = Convert.ToInt16(value);
            var binaryValue = Convert.ToString(value16, 2);
            return ("@" + trimmedLine, binaryValue.PadLeft(16, '0'));
        }

        /// <summary>
        /// Constructs a binary value based on a C instruction text lines composition
        /// </summary>
        /// <param name="line">C instruction line</param>
        /// <returns>Tuple with label that was translated, and its binary value.</returns>
        private (string label, string binary) GetCInstruction(string line)
        {
            var trimmedLine = line.Trim();
            string cDest = CInstructionHelper.GetCDest(trimmedLine);
            string cJump = CInstructionHelper.GetCJump(trimmedLine);
            string cComp = CInstructionHelper.GetCComp(trimmedLine);

            return (line.Trim(), CInstructionHelper.CombineCInstruction(cDest, cJump, cComp));
        }

        /// <summary>
        /// Loops through text lines, and adds and adds to label collection if line is a label
        /// Value for label is line count - label collection count, because label definition line is not translated to binary.
        /// </summary>
        /// <param name="textLines">string array containing ASM lines.</param>
        /// <param name="collection">label collection</param>
        private void AddLabelValues(string[] textLines, Dictionary<string, string> collection)
        {
            for (int i = 0; i < textLines.Length; i++)
            {
                var trimmed = textLines[i].Trim();
                if (trimmed.StartsWith('('))
                {
                    var formattedStr = trimmed.Replace('(', ' ');
                    formattedStr = formattedStr.Replace(')', ' ');
                    formattedStr = formattedStr.Trim();
                    var binaryValue = Convert.ToString(i - collection.Count, 2);
                    binaryValue = binaryValue.PadLeft(16, '0');
                    collection.TryAdd("@" + formattedStr, binaryValue);
                }
            }
        }

        /// <summary>
        /// Returns the value for a user defined symbol based on collection of current defined user variables
        /// User variables starts from 16.
        /// </summary>
        /// <param name="collection">Collection of current user defined variables.</param>
        /// <returns>Value for user defined variable.</returns>
        private int GetUserDefinedSymbolValue(Dictionary<string, string> collection)
        {
            return collection.Any() ? 16 + collection.Count() : 16;
        }

        /// <summary>
        /// Checks if line is a user defined symbol by checking if it's nothing else.
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <param name="predefSymbols">collection of predefined symbols</param>
        /// <param name="userDefLabels">collection of user defined labels.</param>
        /// <returns>False if line does not start with @ or is not a variable, True otherwise.</returns>
        private bool IsLineUserDefinedSymbol(string line, Dictionary<string, string> predefSymbols, Dictionary<string, string> userDefLabels)
        {
            var trimmedLine = line.Trim();
            if (!trimmedLine.StartsWith('@'))
            {
                return false;
            }
            var isInt = int.TryParse(trimmedLine.TrimStart().Remove(0, 1), out int result);
            var isPredefined = predefSymbols.ContainsKey(trimmedLine.Remove(0, 1));
            var isLabel = userDefLabels.ContainsKey(trimmedLine);
            if (isInt || isPredefined || isLabel)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if current line is a predefined symbol, based on a collection of predefined symbols.
        /// </summary>
        /// <param name="line">line to check</param>
        /// <param name="predefSymbols">predefined symbol collection to compare to.</param>
        /// <returns>True if line is predefined symbol, false if not.</returns>
        private bool IsLinePredefinedSymbol(string line, Dictionary<string, string> predefSymbols)
        {
            var formattedLine = line.Trim().Remove(0, 1);
            return predefSymbols.TryGetValue(formattedLine, out var _);
        }

        /// <summary>
        /// Checks if current line is an A instruction based on it being parsable to int.
        /// A instructions always start with @, therefore we remove the first letter and try to parse.
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <returns>True if line is A instruction, false if not.</returns>
        private bool IsLineAInstruction(string line)
        {
            var formattedLine = line.Trim().Remove(0, 1);
            return int.TryParse(formattedLine, out var _);
        }

        /// <summary>
        /// Checks if current line is a C instruction, based on = and ;.
        /// </summary>
        /// <param name="line">the line to check</param>
        /// <returns>True if C instruction, false if not.</returns>
        private bool IsLineCInstruction(string line)
        {
            var formatted = line.Trim();
            string[] splitLines = formatted.Split('=', ';');
            if (splitLines.Length > 1)
            {
                return true;
            }
            return false;
        }
    }
}
