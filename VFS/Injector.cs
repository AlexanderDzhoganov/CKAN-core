using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static void StartInjectedKSPInstance(string ksp_path, string ksp_args, string injected_dll_path)
        {
            // this registers ckan.exe and our injected dll in the GAC
            // requires admin privileges 
            Config.Register
            (
                "CKAN KSP Injector",
                Assembly.GetExecutingAssembly().Location,
                injected_dll_path
            );
             
            // create an IPC server which we'll use to communicate with the injected DLL
            RemoteHooking.IpcCreateServer<FileMonInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);

            // this does the actual process creation and injects our DLL
            int targetPID;
            RemoteHooking.CreateAndInject
            (
                ksp_path,
                ksp_args,
                0,
                injected_dll_path,
                injected_dll_path,
                out targetPID,
                ChannelName
            );
        }
    }
}
