using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sonar.AutoSwitch.Services.Win32;

public class NetworkHelper
{
// https://msdn2.microsoft.com/en-us/library/aa366386.aspx
    public enum TCP_TABLE_CLASS
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    }

    public static IEnumerable<int> GetPortById(int pid, bool isRemote = true)
    {
        var mibTcprowOwnerPids = new IPHelperWrapper().GetAllTCPv6Connections();
        foreach (var mibTcprowOwnerPid in mibTcprowOwnerPids)
        {
            if (mibTcprowOwnerPid.owningPid == pid)
            {
                int portById = GetPort(isRemote ? mibTcprowOwnerPid.remotePort : mibTcprowOwnerPid.localPort);
                if (portById == 0)
                    continue;
                yield return portById;
            }
        }
    }

    public static int GetPort(byte[] bytes)
    {
        ushort num = BitConverter.ToUInt16(bytes, 0);
        return (int) (((num & 0xFF000000) >> 8) | ((num & 0x00FF0000) << 8) | ((num & 0x0000FF00) >> 8) |
                      ((num & 0x000000FF) << 8));
    }

    // https://msdn2.microsoft.com/en-us/library/aa366913.aspx
    [StructLayout(LayoutKind.Sequential)]
    public struct
        MIB_TCPROW_OWNER_PID
    {
        public uint state;
        public uint localAddr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] localPort;

        public uint remoteAddr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] remotePort;

        public uint owningPid;
    }

// https://msdn2.microsoft.com/en-us/library/aa366921.aspx
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPTABLE_OWNER_PID
    {
        public uint dwNumEntries;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCPROW_OWNER_PID[] table;
    }

// https://msdn.microsoft.com/en-us/library/aa366896
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCP6ROW_OWNER_PID
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] localAddr;

        public uint localScopeId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] localPort;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] remoteAddr;

        public uint remoteScopeId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] remotePort;

        public uint state;
        public uint owningPid;
    }

// https://msdn.microsoft.com/en-us/library/windows/desktop/aa366905
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCP6TABLE_OWNER_PID
    {
        public uint dwNumEntries;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCP6ROW_OWNER_PID[] table;
    }

    public static class IPHelperAPI
    {
        [DllImport("iphlpapi.dll", SetLastError = true)]
        internal static extern uint GetExtendedTcpTable(
            IntPtr tcpTable,
            ref int tcpTableLength,
            bool sort,
            int ipVersion,
            TCP_TABLE_CLASS tcpTableType,
            int reserved = 0);
    }

    public class IPHelperWrapper : IDisposable
    {
        public const int AF_INET = 2; // IP_v4 = System.Net.Sockets.AddressFamily.InterNetwork
        public const int AF_INET6 = 23; // IP_v6 = System.Net.Sockets.AddressFamily.InterNetworkV6

        // Creates a new wrapper for the local machine
        public IPHelperWrapper()
        {
        }

        // Disposes of this wrapper
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<MIB_TCPROW_OWNER_PID> GetAllTCPv4Connections()
        {
            return GetTCPConnections<MIB_TCPROW_OWNER_PID, MIB_TCPTABLE_OWNER_PID>(AF_INET);
        }

        public List<MIB_TCP6ROW_OWNER_PID> GetAllTCPv6Connections()
        {
            return GetTCPConnections<MIB_TCP6ROW_OWNER_PID, MIB_TCP6TABLE_OWNER_PID>(AF_INET6);
        }

        public List<IPR> GetTCPConnections<IPR, IPT>(int ipVersion)
        {
            //IPR = Row Type, IPT = Table Type

            IPR[] tableRows;
            int buffSize = 0;
            var dwNumEntriesField = typeof(IPT).GetField("dwNumEntries");

            // how much memory do we need?
            uint ret = IPHelperAPI.GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, ipVersion,
                TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            IntPtr tcpTablePtr = Marshal.AllocHGlobal(buffSize);

            try
            {
                ret = IPHelperAPI.GetExtendedTcpTable(tcpTablePtr, ref buffSize, true, ipVersion,
                    TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
                if (ret != 0) return new List<IPR>();

                // get the number of entries in the table
                IPT table = (IPT) Marshal.PtrToStructure(tcpTablePtr, typeof(IPT));
                int rowStructSize = Marshal.SizeOf(typeof(IPR));
                uint numEntries = (uint) dwNumEntriesField.GetValue(table);

                // buffer we will be returning
                tableRows = new IPR[numEntries];

                IntPtr rowPtr = (IntPtr) ((long) tcpTablePtr + 4);
                for (int i = 0; i < numEntries; i++)
                {
                    IPR tcpRow = (IPR) Marshal.PtrToStructure(rowPtr, typeof(IPR));
                    tableRows[i] = tcpRow;
                    rowPtr = (IntPtr) ((long) rowPtr + rowStructSize); // next entry
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(tcpTablePtr);
            }

            return tableRows != null ? tableRows.ToList() : new List<IPR>();
        }

        // Occurs on destruction of the Wrapper
        ~IPHelperWrapper()
        {
            Dispose();
        }
    } // wrapper class
}