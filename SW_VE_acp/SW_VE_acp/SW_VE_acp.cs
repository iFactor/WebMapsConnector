//-----------------------------------------------------------------------------------------------
// Web Maps Connector (which shows web maps as layers in Smallworld(TM) Core Spatial Technology)
// Copyright (C) 2017 KUBRA
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// You may contact KUBRA at https://kubra.com/contact-us/
//-----------------------------------------------------------------------------------------------
﻿#define TRACE

using System;
using System.IO;
using System.Diagnostics;

namespace iFactor
{
    /// <summary>
    /// This class is called by if_ve_acp.magik
    /// </summary>
    /// <remarks>
    /// This is the executable side of the if_ve_acp ACP and is used to make requests to the Virtual Earth services.
    /// </remarks>
    class SW_VE_acp 
    {

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            bool doDebug = false;

            if (args.Length > 0 && args[0] == "debug")
            {
                doDebug = true;
            }

            SWVEService service = new SWVEService(doDebug);
            bool stay = true;

            // keep processing requests until the code encounters a problem.
            do
            {
                stay = service.ProcessRequest();
            } while (stay == true);

        }
         

    }
}
