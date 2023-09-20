// See https://aka.ms/new-console-template for more information
using Assembler;

// ASM text file to convert to HACK
string text = File.ReadAllText("C:\\Users\\Benjamin Roesdal\\Desktop\\nand2tetris\\projects\\06\\pong\\Pong.asm");

// Converts ASM file to binary text.
IBinaryHandler binaryMapper = new BinaryHandler();
var binaryText = binaryMapper.AsmToBinaryText(text);

// Location for HACK file to be saved.
File.WriteAllText("C:\\Users\\Benjamin Roesdal\\Desktop\\nand2tetris\\tools\\pong.hack", binaryText);

Console.WriteLine(binaryText);