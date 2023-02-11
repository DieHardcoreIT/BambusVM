using System.Collections.Generic;

namespace BambusVM.Runtime.Util
{
    public class VmLocal
    {
        public VmLocal()
        {
            Vars = new Dictionary<int, dynamic>();
            for (var i = 0; i < 50; i++)
                Vars.Add(i, null);
        }

        public Dictionary<int, dynamic> Vars { get; }

        public void Update(int index, dynamic value)
        {
            Vars[index] = value;
        }

        public dynamic Get(int index)
        {
            return !Vars.ContainsKey(index) ? null : Vars[index];
        }
    }
}