using System.Collections.Generic;

namespace BambusVM.Runtime.Util
{
    public class VmArgs
    {
        public VmArgs()
        {
            Args = new Dictionary<int, dynamic>();
        }

        public VmArgs(object[] pm)
        {
            Args = new Dictionary<int, dynamic>();
            for (var i = 0; i < pm.Length; i++)
                Args[i] = pm[i];
        }

        public Dictionary<int, dynamic> Args { get; }

        public void Update(int index, dynamic value)
        {
            Args[index] = value;
        }

        public dynamic Get(int index)
        {
            return Args[index];
        }
    }
}