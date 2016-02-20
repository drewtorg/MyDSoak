using SharedObjects;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommSub
{
    public class ProcessAddressBook
    {
        public ConcurrentDictionary<int, ProcessInfo> Proccesses { get; set; }
        public ConcurrentDictionary<string, int> EndPoints{ get; set; }

        public ProcessAddressBook()
        {
            Proccesses = new ConcurrentDictionary<int, ProcessInfo>();
            EndPoints = new ConcurrentDictionary<string, int>();
        }

        public void Add(ProcessInfo processInfo)
        {
            Proccesses.TryAdd(processInfo.ProcessId, processInfo);
            EndPoints.TryAdd(processInfo.EndPoint.ToString(), processInfo.ProcessId);
        }
    }
}
