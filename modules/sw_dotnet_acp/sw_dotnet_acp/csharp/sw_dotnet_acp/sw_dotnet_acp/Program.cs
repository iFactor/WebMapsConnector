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
using SWUSER;

namespace sw_dotnet_acp
{
    class TheDotNetAcp
    {
        private SWACP TheACPClass;
        private Stream stderr;
        private StreamWriter stderr_w;

        TheDotNetAcp()
        {
            TheACPClass = new SWACP();
            stderr = System.Console.OpenStandardError();
            stderr_w = new StreamWriter(stderr);
        }

        static void Main(string[] args)
        {
            TheDotNetAcp _self = null;
            try
            {
                _self = new TheDotNetAcp();
                _self.Perform();
            }
            finally
            {
            }
        }

        public void Perform()
        {
            if (!TheACPClass.acpVerifyConnection("sw_dotnet_acp"))
            {
                Environment.Exit(0);
            }
            TheACPClass.acpEstablishProtocol(0, 1);
            echo();
        }

        private void echo()
        {
            //strings echo test
            byte[] bts = TheACPClass.acpGetString8();
            TheACPClass.acpPutString8(bts);
            string s = TheACPClass.acpGetString();
            TheACPClass.acpPutString(s);
            //byte tests
            byte b = TheACPClass.acpGetByte();
            TheACPClass.acpPutByte(b);
            b = TheACPClass.acpGetByte();
            TheACPClass.acpPutByte(b);
            //short tests
            short i = TheACPClass.acpGetShort();
            TheACPClass.acpPutShort(i);
            ushort ui = TheACPClass.acpGetUShort();
            TheACPClass.acpPutUShort(ui);
            //integer tests
            int ii = TheACPClass.acpGetInt();
            TheACPClass.acpPutInt(ii);
            uint uii = TheACPClass.acpGetUInt();
            TheACPClass.acpPutUInt(uii);
            //long tests
            long l = TheACPClass.acpGetLong();
            TheACPClass.acpPutLong(l);
            ulong ul = TheACPClass.acpGetULong();
            TheACPClass.acpPutULong(ul);
            //float test
            float f = TheACPClass.acpGetFloat();
            TheACPClass.acpPutFloat(f);
            //double test
            double d = TheACPClass.acpGetDouble();
            TheACPClass.acpPutDouble(d);
            //boolean test
            bool bl = TheACPClass.acpGetBool();
            TheACPClass.acpPutBool(bl);
            
            byte[] bb = TheACPClass.acpGetBytes();
            stderr_w.Write("#DEBUG: ");
            stderr_w.WriteLine(bb.Length);
            stderr_w.Flush();

            TheACPClass.acpPutBytes(bb);
            TheACPClass.acpFlush();
            
        }
    }
}
