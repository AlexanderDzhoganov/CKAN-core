using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using EasyHook;

namespace CKAN.VFS
{
    public class FileMonInterface : MarshalByRefObject
    {
        public void IsInstalled(Int32 InClientPID)
        {
            Console.WriteLine("FileMon has been installed in target {0}.\r\n", InClientPID);
        }

        public void OnCreateFile(Int32 InClientPID, String[] InFileNames)
        {
            for (int i = 0; i < InFileNames.Length; i++)
            {
                Console.WriteLine(InFileNames[i]);
            }
        }

        public void ReportException(Exception InInfo)
        {
            Console.WriteLine("The target process has reported an error:\r\n" + InInfo.ToString());
        }

        public void Ping()
        {
        }
    }

    class Injector
    {
        static String ChannelName = null;

        public static void StartInjectedKSPInstance(string ksp_path, string ksp_args = "")
        {
            Config.Register(
            "CKAN KSP Injector",
            "FileMon.exe",
            "FileMonInject.dll");

            RemoteHooking.IpcCreateServer<FileMonInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);

            int targetPID;
            RemoteHooking.CreateAndInject(ksp_path, ksp_args, 0, "FileMonInject.dll", "FileMonInject.dll", out targetPID, ChannelName);
        }
    }
}
