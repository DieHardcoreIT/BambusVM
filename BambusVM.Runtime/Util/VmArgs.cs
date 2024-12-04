using System.Collections.Generic;

namespace BambusVM.Runtime.Util;

public class VmArgs
{
    /// <summary>
    /// A dictionary that stores arguments with integer keys and dynamic values.
    /// This property provides access to the internal collection used for managing arguments
    /// within the <see cref="VmArgs"/> class.
    /// </summary>
    private Dictionary<int, dynamic> Args { get; }

    /// <summary>
    /// Represents a collection of arguments for the Bambus virtual machine.
    /// Provides functionality to store, update, and retrieve arguments using an integer index.
    /// </summary>
    public VmArgs()
    {
        Args = new Dictionary<int, dynamic>();
    }

    /// <summary>
    /// Represents the arguments passed to a Bambus virtual machine instance.
    /// </summary>
    public VmArgs(object[] pm)
    {
        Args = new Dictionary<int, dynamic>();
        for (var i = 0; i < pm.Length; i++)
            Args[i] = pm[i];
    }

    /// <summary>
    /// Updates the value at the specified index in the virtual machine's arguments collection.
    /// </summary>
    /// <param name="index">The index of the argument to update.</param>
    /// <param name="value">The new value to set at the specified index.</param>
    public void Update(int index, dynamic value)
    {
        Args[index] = value;
    }

    /// <summary>
    /// Retrieves a value from the arguments dictionary based on the provided index.
    /// </summary>
    /// <param name="index">The index key to locate the value within the arguments.</param>
    /// <returns>The dynamic value associated with the specified index.</returns>
    public dynamic Get(int index)
    {
        return Args[index];
    }
}