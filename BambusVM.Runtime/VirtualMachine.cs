using BambusVM.Runtime.Util;
using System.Linq;

namespace BambusVM.Runtime
{
    public class VirtualMachine
    {
        public static object Execute(string key, string iv, string vmString, object[] pm)
        {
            //decrypt
            var decryptedVmString = AesHelper.DecryptDataWithAes(vmString, key, iv);

            //split string to list
            var instructionList = decryptedVmString.Split(';').ToList();

            //run "vm"
            var ctx = new Context(instructionList, pm);
            return ctx.Run();
        }
    }
}