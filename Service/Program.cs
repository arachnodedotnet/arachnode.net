#region License : arachnode.net

// // Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
// //  
// // Permission is hereby granted, upon purchase, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, merge and modify copies of the Software, and to permit persons
// // to whom the Software is furnished to do so, subject to the following
// // conditions:
// // 
// // LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

#endregion

namespace Arachnode.Service
{
    internal static class Program
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        /// <summary>
        /// 	The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            if (Debugger.IsAttached)
            {
                AllocConsole();

                Service1 service1 = new Service1();

                service1.DebugOnStart(null);
            }
            else
            {
                //If you receive a TypeInitializationException as the InnerException, copy ConnectionStrings.config from \Console\bin\[Debug/Release] to \Service\bin\[Debug/Release]...
                //If you receive an InvalidConfigurationException as the InnerException, start the Console project and reset the database and perform initial setup tasks, or examine cfg.Configuration for missing configuration settings...

                ServiceBase[] ServicesToRun;

                // More than one user Service may run within the same process. To add
                // another service to this process, change the following line to
                // create a second service object. For example,
                //
                //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
                //
                ServicesToRun = new ServiceBase[] {new Service1()};

                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}