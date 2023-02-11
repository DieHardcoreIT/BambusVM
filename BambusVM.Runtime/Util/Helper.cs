using System;
using System.Reflection;
using System.Windows.Forms;

namespace BambusVM.Runtime.Util
{
    public class Helper
    {
        public static object[] GetMethodParameters(Context ctx, ParameterInfo[] pi)
        {
            if (pi.Length == 0)
                return new object[] { };

            var objectList = new object[pi.Length];

            for (var i = pi.Length - 1; i >= 0; i--)
            {
                var type = pi[i].ParameterType;
                dynamic val = ctx.Stack.Pop();
                try
                {
                    objectList[i] = Convert.ChangeType(val, type);
                }
                catch
                {
                    try
                    {
                        objectList[i] = Enum.Parse(type, val.ToString());
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("BambusVM failed to parse a parameter: " + nameof(type), "BambusVM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
            }

            return objectList;
        }

        public static int ReadPrefix(string txt)
        {
            return int.Parse(txt[0].ToString());
        }

        public static int ReadPrefix(string txt, int idx)
        {
            return int.Parse(txt[idx].ToString());
        }
    }
}