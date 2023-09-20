using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public static class CInstructionHelper
    {
        /// <summary>
        /// Finds C instruction Dest value based on a collection of defined values, null if none found.
        /// </summary>
        /// <param name="line">line to lookup dest value on.</param>
        /// <returns>Dest value</returns>
        public static string GetCDest(string line)
        {
            var formatted = line.Split("=");
            if (formatted.Length > 1 && AsmValues.Dest.TryGetValue(formatted[0], out var value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Finds C instruction Jump value based on defined collection of Jump values, null if none found..
        /// </summary>
        /// <param name="line">Line to get Jump value on.</param>
        /// <returns>Jump value</returns>
        public static string GetCJump(string line)
        {
            var formatted = line.Split(";");
            if (formatted.Length > 2 && AsmValues.Jump.TryGetValue(formatted[2], out var valueWithDest))
            {
                return valueWithDest;
            }
            if (formatted.Length == 2 && AsmValues.Jump.TryGetValue(formatted[1], out var valueWithoutDest))
            {
                return valueWithoutDest;
            }
            return null;
        }

        /// <summary>
        /// Finds C instruction comp value based on defined collection of comp values, null if none found.
        /// </summary>
        /// <param name="line">line to find comp value on.</param>
        /// <returns>comp value.</returns>
        public static string GetCComp(string line)
        {
            var formattedComp = line.Split("=");
            var formattedJump = line.Split(";");
            if (formattedComp.Length > 1 && AsmValues.Comp.TryGetValue(formattedComp[1], out var cValue))
            {
                return cValue;
            }
            if (formattedJump.Length > 1 && AsmValues.Comp.TryGetValue(formattedJump[0], out var jValue))
            {
                return jValue;
            }
            return null;
        }

        /// <summary>
        /// Constructs a binary value string based on dest, jump and comp values.
        /// </summary>
        /// <param name="dest">dest binary value</param>
        /// <param name="jump">jump binary value</param>
        /// <param name="comp">comp binary value</param>
        /// <returns>C instruction binary string</returns>
        public static string CombineCInstruction(string dest, string jump, string comp)
        {
            var binaryValue = "111";
            binaryValue = binaryValue + comp;
            _ = dest != null ? binaryValue = binaryValue + dest : binaryValue = binaryValue + "000";
            _ = jump != null ? binaryValue = binaryValue + jump : binaryValue = binaryValue + "000";
            return binaryValue;
        }
    }
}
