using System.Collections.Generic;

namespace BambusVM.Runtime.Util;

public class VmLocal
{
    /// <summary>
    /// Represents a collection of dynamically-typed variables stored in a dictionary.
    /// The keys are integers, which act as the index for accessing each variable value.
    /// This property is primarily used for managing the variables within the virtual machine's local scope.
    /// </summary>
    private Dictionary<int, dynamic> Vars { get; }

    /// <summary>
    /// Represents the local variables of a virtual machine instance, indexed by integers.
    /// </summary>
    public VmLocal()
    {
        Vars = new Dictionary<int, dynamic>();
        for (var i = 0; i < 50; i++)
            Vars.Add(i, null);
    }

    /// <summary>
    /// Updates the value of the variable at the specified index in the local variables dictionary.
    /// </summary>
    /// <param name="index">The index of the local variable to update.</param>
    /// <param name="value">The new value to be set for the local variable at the specified index.</param>
    public void Update(int index, dynamic value)
    {
        Vars[index] = value;
    }

    /// <summary>
    /// Retrieves a dynamic value from the local variable storage specified by the given index.
    /// </summary>
    /// <param name="index">The index of the local variable to retrieve.</param>
    /// <returns>The dynamic value stored at the specified index, or null if the index is not found.</returns>
    public dynamic Get(int index)
    {
        return !Vars.TryGetValue(index, out var var) ? null : var;
    }
}