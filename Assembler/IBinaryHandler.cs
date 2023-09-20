
namespace Assembler
{
    public interface IBinaryHandler
    {
        /// <summary>
        /// Takes ASM string and converts to a string of binary instructions.
        /// </summary>
        /// <param name="asmText">Asm string to convert.</param>
        /// <returns>string of binary instructions.</returns>
        string AsmToBinaryText(string asmText);
    }
}
