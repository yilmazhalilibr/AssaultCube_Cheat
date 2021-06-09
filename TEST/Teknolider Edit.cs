using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Teknolider
{

    public static class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr process, IntPtr address, byte[] buffer, uint size, ref uint written);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr process, IntPtr address, byte[] buffer, uint size, ref uint read);

        [Flags]
        public enum ProcessAccessType
        {
            PROCESS_TERMINATE = (0x0001),
            PROCESS_CREATE_THREAD = (0x0002),
            PROCESS_SET_SESSIONID = (0x0004),
            PROCESS_VM_OPERATION = (0x0008),
            PROCESS_VM_READ = (0x0010),
            PROCESS_VM_WRITE = (0x0020),
            PROCESS_DUP_HANDLE = (0x0040),
            PROCESS_CREATE_PROCESS = (0x0080),
            PROCESS_SET_QUOTA = (0x0100),
            PROCESS_SET_INFORMATION = (0x0200),
            PROCESS_QUERY_INFORMATION = (0x0400)
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            [MarshalAs(UnmanagedType.U4)]ProcessAccessType access,
            [MarshalAs(UnmanagedType.Bool)]bool inheritHandler, uint processId);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr objectHandle);
    }
}
namespace Teknolider
{
    public class Bellek : IDisposable
    {
        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, Byte[] lpBuffer, long nSize, long lpNumberOfBytesRead);

        private Process process;
        private IntPtr processHandle;
        private bool isDisposed;
        //---------------------------------------------TEKNOLIDER ------------------------------------------------------
        public Bellek(string procesIsim)
        {
            if (procesIsim == null || procesIsim == "")
            { return; }
            System.Diagnostics.Process[] pGecici = System.Diagnostics.Process.GetProcessesByName(procesIsim);
            if (pGecici.Length > 0) //aciksa
                process = pGecici[0];


            if (process == null)
                throw new ArgumentNullException("process"); // benim hata aldýgým yer burasý

            this.process = process;
            processHandle = Win32.OpenProcess(
                Win32.ProcessAccessType.PROCESS_VM_READ | Win32.ProcessAccessType.PROCESS_VM_WRITE |
                Win32.ProcessAccessType.PROCESS_VM_OPERATION, true, (uint)process.Id);
            if (processHandle == IntPtr.Zero)
                throw new InvalidOperationException("Could not open the process");

        }
        public Bellek(Process process)
        { 
            if (process == null)
                throw new ArgumentNullException("process");

            this.process = process;
            processHandle = Win32.OpenProcess(
                Win32.ProcessAccessType.PROCESS_VM_READ | Win32.ProcessAccessType.PROCESS_VM_WRITE |
                Win32.ProcessAccessType.PROCESS_VM_OPERATION, true, (uint)process.Id);
            if (processHandle == IntPtr.Zero)
                throw new InvalidOperationException("Could not open the process");
        }

        #region DisposeIslemleri

        ~Bellek()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (isDisposed)
                return;
            Win32.CloseHandle(processHandle);
            process = null;
            processHandle = IntPtr.Zero;
            isDisposed = true;
        }

        #endregion
        #region Properties
        public Process Process
        {
            get
            {
                return process;
            }
        }
        #endregion
        #region YAZ_INT
        #region YAZ_OFFSET_INT
        public int Int_Yaz_Offset(long ana_Gecici, long offset, long istenenDeger)
        {
            try
            {
                ana_Gecici = ReadInt32((IntPtr)ana_Gecici) + offset;
                WriteInt32((IntPtr)ana_Gecici, istenenDeger);
                return 1;
            }
            catch
            {
                return -11111; /*hata çýkarsa -11111 döndürüyoruz.*/
            }
        }   
        public int Int_Yaz_Offset(long ana_Gecici, string offsetler, long istenenDeger)
        {
            try
            {
                string[] offsetlerDize = offsetler.Split('+');// + yardýmýyla offsetleri ayýrdýk.
                int offSetTane = offsetlerDize.Length;
                int[] adres = new int[offSetTane]; //offsetleri int olarak burada tutuyoruz

                for (int i = 0; i < offSetTane; i++)
                {
                    adres[i] = int.Parse(offsetlerDize[i], System.Globalization.NumberStyles.HexNumber);
                }

                foreach (int offsetttt in adres)
                    ana_Gecici = ReadInt32((IntPtr)ana_Gecici) + offsetttt;


                WriteInt32((IntPtr)ana_Gecici, istenenDeger);
                return 1;
            }
            catch
            {
                return -11111; /*hata çýkarsa -11111 döndürüyoruz.*/
            }
        }
        #endregion
        #region YAZ_OFFSETSIZ_INT
        public void Int_Yaz_Offsetsiz(long address, long deger)
        {
            WriteInt32((IntPtr)address, deger);
        }
        #endregion
        #endregion
        #region YAZ_STRING(yazi)
        #region YAZ_OFFSET_STRING
        public void String_Yaz_Offset(long ana_Gecici, long offset,string  deger, int uzunluk)
        {
            ana_Gecici = ReadInt32((IntPtr)ana_Gecici) + offset;
            WriteString((IntPtr)ana_Gecici, deger, uzunluk);
        }       
        #endregion
        #endregion
        #region OKU_GERIDONUS_INT
        #region OKU_OFFSET
        public int Int_OKU_Offset(long ana_Gecici, int offset)
        {
            int geridondur = 0;
            geridondur = ReadInt32((IntPtr)ana_Gecici) + offset;
            return ReadInt32((IntPtr)geridondur);
        }
        public int Int_OKU_Offset(long ana_Gecici, string offsetler)
        {
            try
            {
                string[] offsetlerDize = offsetler.Split('+');// + yardýmýyla offsetleri ayýrdýk.
                int offSetTane = offsetlerDize.Length;
                int[] adres = new int[offSetTane]; //offsetleri int olarak burada tutuyoruz

                for (int i = 0; i < offSetTane; i++)
                {
                    adres[i] = int.Parse(offsetlerDize[i], System.Globalization.NumberStyles.HexNumber);
                }

                foreach (int offsetttt in adres)
                    ana_Gecici = ReadInt32((IntPtr)ana_Gecici) + offsetttt;

                return ReadInt32((IntPtr)ana_Gecici);
            }
            catch { return -11111; /*hata çýkarsa -11111 döndürüyoruz.*/}
        }
        public int Int_OKU_Offset(long ana_Gecici, int[] offsetler)
        {
            IntPtr anaAdres = (IntPtr)ana_Gecici;
            if (anaAdres == IntPtr.Zero)
                throw new ArgumentException("0 girilmiþ, hata.");


            if (offsetler.Length > 0 && offsetler != null)
            {
                foreach (int offset in offsetler)
                {
                    ana_Gecici = ReadInt32((IntPtr)ana_Gecici) + offset;
                }
            }
            return ReadInt32((IntPtr)ana_Gecici);
        }

        #endregion
        #region OKU_OFFSETSIZ

        public int Int_Oku_Offsetsiz(long address)
        {
            byte[] buffer = new byte[4];
            ReadMemory((IntPtr)address, buffer, 4);//int adresi INTPTR yaptýk.
            return BitConverter.ToInt32(buffer, 0);
        }
        #endregion
        #endregion
        #region OKU_GERIDONUS_STRING(yazi)
        public string String_OKU_Offsetsiz(long adres, int uzunluk)
        {
            IntPtr ad = (IntPtr)adres;
            byte[] buffer = new byte[uzunluk];
            ReadProcessMemory(processHandle, ad, buffer, uzunluk, 0);
            return Encoding.UTF8.GetString(buffer); ;
        }
        struct vut
        {
            [DllImport("kernel32.dll")]
            public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, 
                ref  Byte lpBuffer,
                long nSize,
                ref int lpNumberOfBytesRead);

        }
        public string String_OKU_Offset(long adres, long offset, int uzunluk)
        {
            IntPtr render = (IntPtr)(ReadInt32((IntPtr)adres) + offset);
            byte[] buffer = new byte[uzunluk];
            ReadProcessMemory(processHandle, render, buffer, uzunluk, 0);
            return Encoding.UTF8.GetString(buffer);
        }
        public string String_OKU_Offset(long adres, string offsetler, int uzunluk)
        {
            string[] offsets = offsetler.Split('+');
            long[] adresLER = new long[offsets.Length];
            byte[] buffer = new byte[uzunluk];
            for (int i = 0; i < offsets.Length; i++)
            {
                adresLER[i] = int.Parse(offsets[i], System.Globalization.NumberStyles.HexNumber);
            }

            foreach (long offsetttt in adresLER)
                adres = ReadInt32((IntPtr)adres) + offsetttt;

            ReadProcessMemory(processHandle, (IntPtr)adres, buffer, uzunluk, 0);
            return Encoding.UTF8.GetString(buffer);
        }
        #endregion
        #region ___PROGRAM__ELLEMEYIN___
        void ReadMemory(IntPtr address, byte[] buffer, int size)
        {
            try
            {
                if (isDisposed)
                    throw new ObjectDisposedException("Bellek");
                if (buffer == null)
                    throw new ArgumentNullException("buffer");
                if (size <= 0)
                    throw new ArgumentException("Size must be greater than zero");
                if (address == IntPtr.Zero)
                    throw new ArgumentException("Invalid address");

                uint read = 0;
                if (!Win32.ReadProcessMemory(processHandle, address, buffer, (uint)size, ref read) ||
                    read != size)
                    throw new AccessViolationException();
            }
            catch { }
        }
        void WriteMemory(IntPtr address, byte[] buffer, int size)
        {
            try
            {
                if (isDisposed)
                    throw new ObjectDisposedException("Bellek");
                if (buffer == null)
                    throw new ArgumentNullException("buffer");
                if (size <= 0)
                    throw new ArgumentException("Size must be greater than zero");
                if (address == IntPtr.Zero)
                    throw new ArgumentException("Invalid address");

                uint write = 0;
                if (!Win32.WriteProcessMemory(processHandle, address, buffer, (uint)size, ref write) ||
                    write != size)
                    throw new AccessViolationException();
            }
            catch { }
        }
        int ReadInt32(IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadMemory(address, buffer, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
        uint ReadUInt32(IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadMemory(address, buffer, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }
        float ReadFloat(IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadMemory(address, buffer, 4);
            return BitConverter.ToSingle(buffer, 0);
        }
        double ReadDouble(IntPtr address)
        {
            byte[] buffer = new byte[8];
            ReadMemory(address, buffer, 8);
            return BitConverter.ToDouble(buffer, 0);
        }
        void WriteUInt32(IntPtr address, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteMemory(address, buffer, 4);
        }
        void WriteInt32(IntPtr address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteMemory(address, buffer, 4);
        }
        void WriteInt32(IntPtr address, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteMemory(address, buffer, 4);
        }
        void WriteFloat(IntPtr address, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteMemory(address, buffer, 4);
        }
        void WriteDouble(IntPtr address, double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteMemory(address, buffer, 8);
        }
        void WriteString(IntPtr adres, string deger, int uzunluk)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(deger);
            WriteMemory(adres, buffer, uzunluk);
        }
        #endregion
    }
}