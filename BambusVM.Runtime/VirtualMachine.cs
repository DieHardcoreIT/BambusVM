using System.Collections.Generic;
using BambusVM.Runtime.Util;
using System.Linq;

namespace BambusVM.Runtime;

public class VirtualMachine
{
    /// <summary>
    /// Executes the given encrypted data by decrypting it using the specified key and initialization vector,
    /// and running the resulting instructions within a context initialized with the provided parameters.
    /// </summary>
    /// <param name="encryptionKey">The encryption key in base64 format.</param>
    /// <param name="initializationVector">The initialization vector in base64 format.</param>
    /// <param name="encryptedData">The encrypted data string containing instructions.</param>
    /// <param name="parameters">An array of parameters to be used during execution.</param>
    /// <returns>The result of executing the instructions within the context.</returns>
    public static object Execute(string encryptionKey, string initializationVector, string encryptedData,
        object[] parameters)
    {
        // Decrypt the encrypted data and split it into a list of instructions
        var instructions = DecryptAndSplitInstructions(encryptedData, encryptionKey, initializationVector);

        // Initialize the execution context with the instructions and parameters
        var context = new Context(instructions, parameters);

        // Execute the context and return the result
        return context.Run();
    }

    /// <summary>
    /// Decrypts the given data using the specified key and initialization vector,
    /// and splits the result into a list of instructions.
    /// </summary>
    /// <param name="encryptedData">The encrypted data string.</param>
    /// <param name="key">The encryption key in base64 format.</param>
    /// <param name="iv">The initialization vector in base64 format.</param>
    /// <returns>A list of instructions parsed from the decrypted data.</returns>
    private static List<string> DecryptAndSplitInstructions(string encryptedData, string key, string iv)
    {
        const char InstructionSeparator = ';'; // Separator for instructions

        // Decrypt the data using AES
        var decryptedData = AesHelper.DecryptDataWithAes(encryptedData, key, iv);

        // Split the decrypted data into a list of instructions
        return decryptedData.Split(InstructionSeparator).ToList();
    }
}