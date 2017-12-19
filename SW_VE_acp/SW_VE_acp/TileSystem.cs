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
    static class TileSystem
    {
        private const double EarthRadius = 6378137;
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level
        /// of detail.
        /// </summary>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The map width and height in pixels.</returns>
        public static uint MapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }

        /// <summary>
        /// Determines the ground resolution (in meters per pixel) at a specified
        /// latitude and level of detail.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// ground resolution.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The ground resolution, in meters per pixel.</returns>
        public static double GroundResolution(double latitude, int levelOfDetail)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            return Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * EarthRadius / MapSize(levelOfDetail);
        }

        /// <summary>
        /// Determines the map scale at a specified latitude, level of detail,
        /// and screen resolution.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// map scale.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="screenDpi">Resolution of the screen, in dots per inch.</param>
        /// <returns>The map scale, expressed as the denominator N of the ratio 1 : N.</returns>
        public static double MapScale(double latitude, int levelOfDetail, int screenDpi)
        {
            return GroundResolution(latitude, levelOfDetail) * screenDpi / 0.0254;
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="latitude">Latitude of the point, in degrees.</param>
        /// <param name="longitude">Longitude of the point, in degrees.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="pixelX">Output parameter receiving the X coordinate in pixels.</param>
        /// <param name="pixelY">Output parameter receiving the Y coordinate in pixels.</param>
        public static void LatLongToPixelXY(double latitude, double longitude, int levelOfDetail, out int pixelX, out int pixelY)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            uint mapSize = MapSize(levelOfDetail);
            pixelX = (int)Clip(x * mapSize + 0.5, 0, mapSize - 1);
            pixelY = (int)Clip(y * mapSize + 0.5, 0, mapSize - 1);
        }

        /// <summary>
        /// Converts a pixel from pixel XY coordinates at a specified level of detail
        /// into latitude/longitude WGS-84 coordinates (in degrees).
        /// </summary>
        /// <param name="pixelX">X coordinate of the point, in pixels.</param>
        /// <param name="pixelY">Y coordinates of the point, in pixels.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="latitude">Output parameter receiving the latitude in degrees.</param>
        /// <param name="longitude">Output parameter receiving the longitude in degrees.</param>
        public static void PixelXYToLatLong(int pixelX, int pixelY, int levelOfDetail, out double latitude, out double longitude)
        {

            double mapSize = MapSize(levelOfDetail);
            double x = (Clip(pixelX, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (Clip(pixelY, 0, mapSize - 1) / mapSize);

            latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            longitude = 360 * x;
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates of the tile containing
        /// the specified pixel.
        /// </summary>
        /// <param name="pixelX">Pixel X coordinate.</param>
        /// <param name="pixelY">Pixel Y coordinate.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        public static void PixelXYToTileXY(int pixelX, int pixelY, out int tileX, out int tileY)
        {
            tileX = pixelX / 256;
            tileY = pixelY / 256;
        }

        /// <summary>
        /// Converts tile XY coordinates into pixel XY coordinates of the upper-left pixel
        /// of the specified tile.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="pixelX">Output parameter receiving the pixel X coordinate.</param>
        /// <param name="pixelY">Output parameter receiving the pixel Y coordinate.</param>
        public static void TileXYToPixelXY(int tileX, int tileY, out int pixelX, out int pixelY)
        {
            pixelX = tileX * 256;
            pixelY = tileY * 256;
        }

        /// <summary>
        /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        public static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }

        /// <summary>
        /// Converts a QuadKey into TMS tile XY coordinates.  The TMS tile coordinates have 0,0 tile in the bottom left corner 
        /// compared to the Bing/Google/OSM tile coordinates which have the 0,0 tile in the top left corner.  The only reason
        /// we currently need TMS coordinates in WMC is to support the MBTiles format.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        /// <param name="levelOfDetail">Output parameter receiving the level of detail.</param>
        public static void QuadKeyToTMSTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail)
        {
            QuadKeyToTileXY(quadKey, out tileX, out tileY, out levelOfDetail);

            // need to flip the tileY so that 0,0 is at the bottom left corner.
            int numberOfTiles = (int)Math.Pow(2, quadKey.Length) - 1;

            tileY = numberOfTiles - tileY;
        }

        /// <summary>
        /// Converts a QuadKey into tile XY coordinates.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        /// <param name="levelOfDetail">Output parameter receiving the level of detail.</param>
        public static void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail)
        {
            tileX = tileY = 0;
            levelOfDetail = quadKey.Length;
            for (int i = levelOfDetail; i > 0; i--)
            {
                int mask = 1 << (i - 1);
                switch (quadKey[levelOfDetail - i])
                {
                    case '0':
                        break;

                    case '1':
                        tileX |= mask;
                        break;

                    case '2':
                        tileY |= mask;
                        break;

                    case '3':
                        tileX |= mask;
                        tileY |= mask;
                        break;

                    default:
                        throw new ArgumentException("Invalid QuadKey digit sequence.");
                }
            }
        }

        public static void QuadKeyToPixelXY(string quadKey, out int pixelX, out int pixelY, out int levelOfDetail)
        {
            //(int tileX, int tileY, out int pixelX, out int pixelY)
            int tileX;
            int tileY;

            QuadKeyToTileXY(quadKey, out tileX, out tileY, out levelOfDetail);

            TileXYToPixelXY(tileX, tileY, out pixelX, out pixelY);

        }

        /// <summary>
        /// Converts a QuadKey into Lat/Long coordinates of the Lower Right Corner.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="latitude">Output parameter receiving the latitude in degrees.</param>
        /// <param name="longitude">Output parameter receiving the longitude in degrees.</param>
        public static void QuadKeyToLRLatLong(string quadKey, out double latitude, out double longitude)
        {
            int tileX;
            int tileY;
            int pixelX;
            int pixelY;
            int levelOfDetail;

            QuadKeyToTileXY(quadKey, out tileX, out tileY, out levelOfDetail);
            // returns coordinate of upperleft corner of quadKey
            TileXYToPixelXY(tileX+1, tileY+1, out pixelX, out pixelY);
            PixelXYToLatLong(pixelX, pixelY, levelOfDetail, out latitude, out longitude);
        }

        /// <summary>
        /// Converts a QuadKey into EPSG:900913 coordinates of the Lower Right Corner.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="x">Output parameter receiving the x coord in meters.</param>
        /// <param name="y">Output parameter receiving the y coord in meters.</param>
        public static void QuadKeyToLRMercator(string quadKey, out double x, out double y)
        {
            double lat;
            double lon;

            QuadKeyToLRLatLong(quadKey,out lat, out lon);

            int tilesize = 256;
            double initialResolution = 2 * Math.PI * 6378137 / tilesize;
            double originShift = 2 * Math.PI * 6378137 / 2.0;

            x = lon * originShift / 180.0;
            y = Math.Log( Math.Tan((90 + lat) * Math.PI / 360.0 )) / (Math.PI / 180.0);
            y = y * originShift / 180.0;

        }

        /// <summary>
        /// Converts a QuadKey into Lat/Long coordinates of Up Left Corner.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="latitude">Output parameter receiving the latitude in degrees.</param>
        /// <param name="longitude">Output parameter receiving the longitude in degrees.</param>
        public static void QuadKeyToULLatLong(string quadKey, out double latitude, out double longitude)
        {
            int tileX;
            int tileY;
            int pixelX;
            int pixelY;
            int levelOfDetail;

            QuadKeyToTileXY(quadKey, out tileX, out tileY, out levelOfDetail);
            // returns coordinate of upperleft corner of quadKey
            TileXYToPixelXY(tileX, tileY, out pixelX, out pixelY);
            PixelXYToLatLong(pixelX, pixelY, levelOfDetail, out latitude, out longitude);
        }

        /// <summary>
        /// Converts a QuadKey into EPSG:900913 coordinates of the Upper Left Corner.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="x">Output parameter receiving the x coord in meters.</param>
        /// <param name="y">Output parameter receiving the y coord in meters.</param>
        public static void QuadKeyToULMercator(string quadKey, out double x, out double y)
        {
            double lat;
            double lon;

            QuadKeyToULLatLong(quadKey, out lat, out lon);

            int tilesize = 256;
            double initialResolution = 2 * Math.PI * 6378137 / tilesize;
            double originShift = 2 * Math.PI * 6378137 / 2.0;

            x = lon * originShift / 180.0;
            y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            y = y * originShift / 180.0;

        }

    }
}
