/*
#### This file is part of Magik Components for SWAF.
##
##     This library is free software; you can redistribute it and/or
##     modify it under the terms of the GNU Lesser General Public
##     License as published by the Free Software Foundation; either
##     version 2.1 of the License, or (at your option) any later version.
##
##     Magik Components for SWAF is distributed in the hope that it will be useful,
##     but WITHOUT ANY WARRANTY; without even the implied warranty of
##     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
##     Lesser General Public License for more details.
##
##     You should have received a copy of the GNU Lesser General Public
##     License along with this library; if not, write to the Free Software
##     Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;


///<summary
/// Namespace for SW .NET classes
///</summary>
namespace SWUSER
{
    ///<summary
    /// v1.0 by Pedro Miranda
    /// Class that implements the ACP .NET side for GE Smallword GIS.
    /// This class was developed by a need of accessing some webservices by the core smallworld product.
    /// There where several ways of doing that, namelly using ole_client to instanciate some COM components
    /// but it would be a huge job to manually format the SOAP packages and process the reply SOAP message. 
    /// The other option would be to create an C/C++ ACP to do the job, but once again it would require an great amout of work to do just a simple 
    /// webservice call. The ideal was to use the .NET framework in conjunction to some VisualStudio wizards that
    /// would abstract the low level SOAP/HTTP protocol for invoking an webservice into a class method invocation.
    /// Althoug this class can be used to implement .NET ACP's for other porpuses, if time is an critical issue in getting the job done the ACP
    /// should be still done in C.
    ///</summary>
    public class SWACP
    {
        private Stream stdin;
        private Stream stdout;
        private static bool autoFlush = true;
        private const int ACP_ID_LENGTH = 64;

        ///<summary
        /// SWACP Class constructor
        ///</summary>
        public SWACP()
        {
            stdin = System.Console.OpenStandardInput();
            stdout = System.Console.OpenStandardOutput();
            autoFlush = true;
        }

        ///<summary
        /// Sets or unsets the output stream autoflush.
        /// By default the autoflush is set to true.
        ///</summary>
        public bool acpSetAutoFlush(bool newval){
             bool old = autoFlush;
             autoFlush = newval;
             return old;
        }

        ///<summary
        /// Flushes the output stream.
        ///</summary>
        public void acpFlush()
        {
            stdout.Flush();
        }

        ///<summary
        /// Gets an array of bytes(8 bits) from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of bytes read.
        ///</summary>
        private int acpGet(byte[] buffer)
        {
            if (autoFlush) acpFlush();
            return stdin.Read(buffer, 0, buffer.Length);
        }

        ///<summary
        /// Gets a byte(8 bits) from the standard input.
        ///</summary>
        public byte acpGetByte()
        {
            if (autoFlush) acpFlush();
            int val = stdin.ReadByte();
            if (val == -1) Environment.Exit(0);
            return (byte)val;
        }

        ///<summary
        /// Gets an array of bytes(8 bits) from the standard input.
        ///</summary>
        public byte[] acpGetBytes()
        {
            ushort count = acpGetUShort();
            if (count == 0)
                return null;
            byte[] buffer = new byte[count];
            acpGet(buffer);
            return buffer;
        }

        ///<summary
        /// Gets an array of unsigned short(16 bits) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetUShorts(ushort[] buffer)
        {
            if (buffer == null)
                return 0;
            byte[] buff = new byte[sizeof(ushort) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(ushort);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets an array of short(16 bits) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetShorts(short[] buffer)
        {
            if (buffer == null)
                return 0;

            byte[] buff = new byte[sizeof(short) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(short);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets an array of unsigned integer(32 bits) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetUInts(uint[] buffer)
        {
            if (buffer == null)
                return 0;

            byte[] buff = new byte[sizeof(uint) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(uint);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets an array of integer(32 bits) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetInts(int[] buffer)
        {
            if (buffer == null)
                return 0;

            byte[] buff = new byte[sizeof(int) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(int);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets an array of unsigned longs(64 bits) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetULongs(ulong[] buffer)
        {
            if (buffer == null)
                return 0;

            byte[] buff = new byte[sizeof(ulong) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(ulong);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets an array of longs(64 bits) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetLongs(long[] buffer)
        {
            if (buffer == null)
                return 0;

            byte[] buff = new byte[sizeof(long) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(long);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets an array of floats(single precision) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetFloats(float[] buffer)
        {
            if (buffer == null)
                return 0;

            byte[] buff = new byte[sizeof(float) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(float);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets an array of doubles(double precision) values from the standard input.
        /// The size of the buffer is defined is is Lenght property so it must be
        /// supplied by the caller.
        /// Returns the amout of data really read.
        ///</summary>
        public int acpGetDoubles(double[] buffer)
        {
            if (buffer == null)
                return 0;

            byte[] buff = new byte[sizeof(double) * buffer.Length];
            int ret_val = acpGet(buff) / sizeof(double);
            Buffer.BlockCopy(buff, 0, buffer, 0, buff.Length);
            return ret_val;
        }

        ///<summary
        /// Gets a short(16 bits) from the standard input.
        ///</summary>
        public short acpGetShort()
        {
            short[] x = new short[1];
            acpGetShorts(x);
            return x[0];
        }

        ///<summary
        /// Gets a unsigned short(16 bits) from the standard input.
        ///</summary>
        public ushort acpGetUShort()
        {            
            ushort[] x = new ushort[1];
            acpGetUShorts(x);
            return x[0];
        }

        ///<summary
        /// Gets a integer(32 bits) from the standard input.
        ///</summary>
        public int acpGetInt()
        {            
            int[] x = new int[1];
            acpGetInts(x);
            return x[0];
        }

        ///<summary
        /// Gets a unsigned integer(32 bits) from the standard input.
        ///</summary>
        public uint acpGetUInt()
        {
            uint[] x = new uint[1];
            acpGetUInts(x);
            return x[0];
        }

        ///<summary
        /// Gets a long(64 bits) from the standard input.
        ///</summary>
        public long acpGetLong()
        {
            long[] x = new long[1];
            acpGetLongs(x);
            return x[0];
        }

        ///<summary
        /// Gets a unsigned long(64 bits) from the standard input.
        ///</summary>
        public ulong acpGetULong()
        {
            ulong[] x = new ulong[1];
            acpGetULongs(x);
            return x[0];
        }

        ///<summary
        /// Gets a float(single precision) from the standard input.
        ///</summary>
        public float acpGetFloat()
        {
            float[] x = new float[1];
            acpGetFloats(x);
            return x[0];
        }

        ///<summary
        /// Gets a double(double precision) from the standard input.
        ///</summary>
        public double acpGetDouble()
        {
            double[] x = new double[1];
            acpGetDoubles(x);
            return x[0];
        }

        ///<summary
        /// Gets a string(unicode) from the standard input.
        ///</summary>
        public string acpGetString()
        {
            ushort count = acpGetUShort();
            if (count == 0)
                return null;

            count *= sizeof(char);
            byte[] s = new byte[count];
            acpGet(s);
            System.Text.Encoding enc = System.Text.Encoding.Unicode;
            string ss = enc.GetString(s);
            return ss;
        }

        ///<summary
        /// Gets a string(ASCII) from the standard input as a array of bytes.
        ///</summary>
        public byte[] acpGetString8()
        {            
            ushort count = acpGetUShort();
            if (count == 0)
                return null;

            byte[] s = new byte[count];

            acpGet(s);
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string ss = enc.GetString(s);
            return s;
        }

        ///<summary
        /// Gets a string(unicode) from the standard input as a array of characters.
        ///</summary>
        public char[] acpGetString16()
        {                        
            ushort count = acpGetUShort();
            if (count == 0)
                return null;

            count *= sizeof(char);
            byte[] s = new byte[count];
            acpGet(s);
            System.Text.Encoding enc = System.Text.Encoding.Unicode;
            string ss = enc.GetString(s);
            return ss.ToCharArray();
        }

        ///<summary
        /// Gets a boolean from the standard input.
        ///</summary>
        public bool acpGetBool() 
        {            
            byte val = acpGetByte();
            return val != 0;
        }

        ///<summary
        /// Verifies the connection string.
        /// Returns true if verified or false otherwise.
        ///</summary>
        public bool acpVerifyConnection(string s)
        {
            byte[] buff = acpGetString8();
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string str2 = enc.GetString(buff);
            int res = s.CompareTo(str2);

            byte[] res_b = BitConverter.GetBytes(res);
            acpPutByte(res_b[0]);
            acpFlush();
            return res==0;
        }

        ///<summary
        /// Establishes the protocol version used my this entity and the smallworld
        /// counterpart.
        /// Returns the Smallworld counterpart protocol version.
        ///</summary>
        public ushort acpEstablishProtocol(ushort min, ushort max)
        {
            ushort inval;
            while (true) {
	            inval = acpGetUShort();
                
	            if (inval < min || inval > max) {
	                acpPutBool(true);
	                acpPutUShort(min);
	                acpPutUShort(max);
	                acpFlush();
	            } else break;
            }
            acpPutBool(false);
            acpFlush();
            return inval;
        }

        ///<summary
        /// Puts an array of Elements in the output stream.
        ///</summary>
        private void acpPut(byte[] buffer, int elementSize, int nElements)
        {
            nElements = Math.Abs(nElements) * Math.Abs(elementSize);
            stdout.Write(buffer, 0, nElements <= buffer.Length ? nElements : buffer.Length);
        }

        ///<summary
        /// Puts a boolean in the output stream.
        ///</summary>
        public void acpPutBool(bool x)
        {
            byte[] b = new byte[1];
            if (x) b[0] = 1; else b[0] = 0;
            acpPut(b, b.Length,b.Length);
        }

        ///<summary
        /// Puts a byte(8bits) in the output stream.
        ///</summary>
        public void acpPutByte(byte b)
        {
            stdout.WriteByte(b);
        }

        ///<summary
        /// Puts an array of bytes(8bits) in the output stream.
        ///</summary>
        public void acpPutBytes(byte[] buffer)
        {
            acpPutUShort((ushort)buffer.Length);
            acpPut(buffer, sizeof(byte), buffer.Length);
        }

        ///<summary
        /// Puts an array of shorts(16 bits) in the output stream.
        ///</summary>
        public void acpPutShorts(short[] buffer)
        {
            byte[] buff = new byte[sizeof(short) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(short), buffer.Length);
        }

        ///<summary
        /// Puts an array of unsigned shorts(16 bits) in the output stream.
        ///</summary>
        public void acpPutUShorts(ushort[] buffer)
        {
            byte[] buff = new byte[sizeof(ushort) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(ushort), buffer.Length);
        }

        ///<summary
        /// Puts an array of integers(32 bits) in the output stream.
        ///</summary>
        public void acpPutInts(int[] buffer)
        {
            byte[] buff = new byte[sizeof(int) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(int), buffer.Length);

        }

        ///<summary
        /// Puts an array of unsigned integers(32 bits) in the output stream.
        ///</summary>
        public void acpPutUInts(uint[] buffer)
        {
            byte[] buff = new byte[sizeof(uint) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(uint), buffer.Length);
        }

        ///<summary
        /// Puts an array of floats(single precision) in the output stream.
        ///</summary>
        public void acpPutFloats(float[] buffer)
        {
            byte[] buff = new byte[sizeof(float) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(float), buffer.Length);
        }

        ///<summary
        /// Puts an array of doubles(double precision) in the output stream.
        ///</summary>
        public void acpPutDoubles(double[] buffer)
        {
            byte[] buff = new byte[sizeof(double) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(double), buffer.Length);
        }

        ///<summary
        /// Puts an array of longs(64 bits) in the output stream.
        ///</summary>
        public void acpPutLongs(long[] buffer)
        {
            byte[] buff = new byte[sizeof(long) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(long), buffer.Length);
        }

        ///<summary
        /// Puts an array of unsigned longs(64 bits) in the output stream.
        ///</summary>
        public void acpPutULongs(ulong[] buffer) 
        {
            byte[] buff = new byte[sizeof(ulong) * buffer.Length];
            Buffer.BlockCopy(buffer, 0, buff, 0, buff.Length);
            acpPut(buff, sizeof(ulong), buffer.Length);
        }

        ///<summary
        /// Puts a short(16 bits) in the output stream.
        ///</summary>
        public void acpPutShort(short x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a unsigned short(16 bits) in the output stream.
        ///</summary>
        public void acpPutUShort(ushort x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a integer(32 bits) in the output stream.
        ///</summary>
        public void acpPutInt(int x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a unsigned integer(32 bits) in the output stream.
        ///</summary>
        public void acpPutUInt(uint x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a float(single precision) in the output stream.
        ///</summary>
        public void acpPutFloat(float x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a double(double precision) in the output stream.
        ///</summary>
        public void acpPutDouble(double x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a array of bytes as a string(ASCII) in the output stream.
        ///</summary>
        public void acpPutString8(byte[] s)
        {
            if (s == null)
                return;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string ss = enc.GetString(s);
            acpPutUShort((ushort)s.Length);
            acpPut(s, sizeof(byte), s.Length);
        }

        ///<summary
        /// Puts a array of chars as a string(unicode) in the output stream.
        ///</summary>
        public void acpPutString16(char[] s)
        {
            if (s == null)
                return;
            string ss = new string(s);
            byte[] buff = new byte[sizeof(char)*s.Length];
            Buffer.BlockCopy(s, 0, buff, 0, buff.Length);

            acpPutUShort((ushort)s.Length);
            acpPut(buff, sizeof(char), s.Length);
        }

        ///<summary
        /// Puts a long(64 bits) in the output stream.
        ///</summary>
        public void acpPutLong(long x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a unsigned long(64 bits) in the output stream.
        ///</summary>
        public void acpPutULong(ulong x)
        {
            byte[] buff = BitConverter.GetBytes(x);
            acpPut(buff, sizeof(byte), buff.Length);
        }

        ///<summary
        /// Puts a a string(unicode) in the output stream.
        ///</summary>
        public void acpPutString(string s)
        {
            if (s == null)
                return;
            acpPutString16(s.ToCharArray());
        }
    }
}
