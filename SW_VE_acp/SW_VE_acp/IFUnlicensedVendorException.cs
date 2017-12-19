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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iFactor
{
    /// <summary>
    /// This exception is thrown when an application is using an unlicensed Vendor Dataset
    /// </summary>
    [Serializable()]
    public class IFUnlicensedVendorException : System.Exception
    {
        /// <summary>
        /// an application is using an unlicensed Vendor Dataset
        /// </summary>
        public IFUnlicensedVendorException() { }

        /// <summary>
        /// an application is using an unlicensed Vendor Dataset
        /// </summary>
        public IFUnlicensedVendorException(string message) : base(message)
        {}

        /// <summary>
        /// an application is using an unlicensed Vendor Dataset
        /// </summary>
        public IFUnlicensedVendorException(string message, System.Exception inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        /// <summary>
        /// an application is using an unlicensed Vendor Dataset
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected IFUnlicensedVendorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

}
