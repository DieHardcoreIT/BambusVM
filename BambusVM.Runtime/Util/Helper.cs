using System;
using System.Reflection;
using System.Windows.Forms;

namespace BambusVM.Runtime.Util;

public class Helper
{
    /// <summary>
    /// Retrieves the parameters for a method call, converting the values from the current context stack to the required parameter types.
    /// </summary>
    /// <param name="ctx">The context containing the stack from which parameters are retrieved.</param>
    /// <param name="pi">An array of <see cref="ParameterInfo"/> objects representing the parameters of the method.</param>
    /// <returns>An array of objects that match the types specified in the <paramref name="pi"/> array, ready to be used as method parameters.</returns>
    public static object[] GetMethodParameters(Context ctx, ParameterInfo[] pi)
    {
        // If no parameters are expected, return an empty array.
        if (pi.Length == 0)
            return new object[] { };

        // Initialize an array to hold the parameters to be returned.
        var objectList = new object[pi.Length];

        // Loop through the parameters in reverse order (stack behavior).
        for (var i = pi.Length - 1; i >= 0; i--)
        {
            var type = pi[i].ParameterType;
            var val = ctx.Stack.Pop(); // Pop the value from the context stack.
            try
            {
                // Attempt to convert the value to the expected parameter type.
                objectList[i] = Convert.ChangeType(val, type);
            }
            catch
            {
                try
                {
                    // If conversion fails, attempt to parse it as an enum.
                    objectList[i] = Enum.Parse(type, val.ToString());
                }
                catch (Exception e)
                {
                    // Show an error message if parsing fails and exit the application.
                    MessageBox.Show("BambusVM failed to parse a parameter: " + nameof(type), "BambusVM",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
        }

        return objectList;
    }

    /// <summary>
    /// Reads the prefix character from the given string, converts it to an integer,
    /// and returns the result.
    /// </summary>
    /// <param name="txt">The text from which the prefix will be read.</param>
    /// <returns>The integer representation of the first character in the text.</returns>
    public static int ReadPrefix(string txt)
    {
        return int.Parse(txt[0].ToString());
    }

    /// <summary>
    /// Parses a character at a specified index in the given string as an integer.
    /// </summary>
    /// <param name="txt">The input string containing the character to be parsed.</param>
    /// <param name="idx">The index position of the character in the string to be parsed.</param>
    /// <returns>An integer representation of the specified character.</returns>
    public static int ReadPrefix(string txt, int idx)
    {
        return int.Parse(txt[idx].ToString());
    }
}