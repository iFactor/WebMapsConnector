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
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace iFactor
{
    class RequestState
    {
        public WebRequest Request; // holds the request
        public object Data; // store any data in this
        public string SiteUrl; // holds the UrlString to match up results (Database lookup, etc).
        public Graphics aGraphic;
        public Matrix aMatrix;
        public string Quadkey;
        public Dictionary<string, Bitmap> BitmapCache;
        public ImageAttributes anImageAttribute;
        public Nullable<Color> bmpTransparentColor;
        public int tryCount;
        public String VendorDatasetName;
        public String VendorTileType;
        public String ExtraRenderingInfo;

        public RequestState(WebRequest request, object data, string siteUrl)
        {
            this.Request = request;
            this.Data = data;
            this.SiteUrl = siteUrl;
        }
        public RequestState(WebRequest request, Graphics aGraphic, Matrix aMatrix, string siteUrl, string quadkey, Dictionary<string, Bitmap> bitmapCache,
                            ImageAttributes anImageAttribute,Nullable<Color> bmpTransparentColor, 
                            string vendorDatasetName, string vendorTileType, string extraRenderingInfo, int tryCount)
        {
            this.Request = request;
            this.aGraphic = aGraphic;
            this.aMatrix = aMatrix;
            this.SiteUrl = siteUrl;
            this.Quadkey = quadkey;
            this.BitmapCache = bitmapCache;
            this.anImageAttribute = anImageAttribute;
            this.bmpTransparentColor = bmpTransparentColor;
            this.tryCount = tryCount;
            this.VendorDatasetName = vendorDatasetName;
            this.VendorTileType = vendorTileType;
            this.ExtraRenderingInfo = extraRenderingInfo;
        }
    }
}
