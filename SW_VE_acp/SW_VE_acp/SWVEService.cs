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
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using iFactor.GeocodeService;
using iFactor.ImageryService;
using iFactor.RouteService;
using Security.Windows.Forms;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using BingMapsRESTToolkit;

namespace iFactor
{
    /// <summary>
    /// This is the class that exposes the various services to the Magik ACP code.
    /// </summary>
    class SWVEService : SWUSER.SWACP
    {
        // bitmapCaches grouped by vendorName
        private Dictionary<string, Dictionary<string, Dictionary<string, Bitmap>>> vendorBitmapCaches = new Dictionary<string, Dictionary<string, Dictionary<string, Bitmap>>>();
        private Dictionary<string, JObject> mapboxMetadataCache = new Dictionary<string, JObject>();
        // use this to keep track of the last tile server number used (e.g., tile0,tile1,tile2, etc.)
        private int BingServerNumber = 0;
        private int MapboxServerNumber = 0;
        private char OSMServerName = 'a';
        

        private Object renderingLock = new Object();

        private int TraceLevel = 0;

        private int tileRenderingBlockSize;
        private int tileRenderingCount = 0;
        private string vendorModes;
        private string[] otherVendorModes;
        private string customerID;
        private int revisionNumber;
        private string UserWebProxyDataKey = "WebMapsConnectorWebProxyString.config";
        private string EMPTY_STRING = "||||empty||||";
        private DateTime weatherBugTimer = DateTime.Now;
        private int TileTimeoutMilliseconds;
        private string WeatherBugAPIKey;
        private string SpatialStreamAPIKey;
        private string BingAPIKey;
        private List<String> bingMapsRequestUris = new List<String>();
        private List<String> mapboxRequestUris = new List<String>();


        private Dictionary<string, Bitmap> attributionBitmapCache = new Dictionary<string, Bitmap>();

        // datasetName,tileName,quadkey = copyrightStatement
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> attributionCopyrightCache = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        private string httpwebrequest_user_agent = null;
        private string GoogleSignedKey = null;
        private string GoogleClientIDString = null;
        private string GoogleRoot = null;

        // need to specify the en-US culture for when we are converting Google Geocoding/ReverseGeocoding/Routing 
        // lat/lng results into doubles because those values are always returned in en-US culture regardless
        // of where in the world the request was initiated from.
        System.Globalization.CultureInfo ciEnUS = new System.Globalization.CultureInfo("en-US");

        //static Regex regex_4326 = new Regex(@"srs=EPSG:4326");
        static Regex regex_3785 = new Regex(@"srs=epsg\:3785");

        public SWVEService(bool doDebug)
        {
           IWebProxy proxy = getWebProxy();

            revisionNumber = Properties.Settings.Default.revision;
            TileTimeoutMilliseconds = Properties.Settings.Default.TileTimeoutMilliseconds;

            XmlDocument licenseInfoXML = new XmlDocument();

            WeatherBugAPIKey = Properties.Settings.Default.weatherBugAPIKey;
            
            SpatialStreamAPIKey = Properties.Settings.Default.spatialStreamAPIKey;
            
            BingAPIKey = Properties.Settings.Default.bingAPIKey.Trim();
            
            GoogleRoot = Properties.Settings.Default.GoogleRoot;


            customerID = "WebMapsConnector";
            char separator = ',';
            char[] modeSeparators = { separator };
            vendorModes = Properties.Settings.Default.vendorModes;
            otherVendorModes = vendorModes.Split(modeSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (proxy != null)
            {
                // setting the DefaultWebProxy affects all HttpWebRequest and Web Service calls.
                WebRequest.DefaultWebProxy = proxy;

            }


            if (doDebug == false)
            {
                if (!this.acpVerifyConnection("sw_ve_acp"))
                {
                    Environment.Exit(0);
                }
                this.acpEstablishProtocol(0, 1);
            }
        }

        public SWVEService()
        {
        }


        public int GetMethodCommand()
        {
            return this.acpGetInt();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns True if a command was processed successfully.  A handled exception will also return True.  If ProcessRequest
        /// receives cmd=0, then return False.</returns>
        public bool ProcessRequest()
        {
            DateTime startTime;

            int cmd = GetMethodCommand();
            Trace(2,"cmd: " + cmd);

            try
            {
                switch (cmd)
                {
                    case 0:
                        return false;
                    case 1:
                        SetTraceLevel();
                        return true;
                    case 2:
                        startTime = DateTime.Now;
                        BingMetadata();
                        return true;
                    case 3:
                        startTime = DateTime.Now;
                        BingGeocode();
                        return true;
                    case 4:
                        startTime = DateTime.Now;
                        GoogleGeocode();
                        return true;

                    case 5:
                        startTime = DateTime.Now;
                        BingReverseGeocode();
                        return true;
                    case 7:
                        startTime = DateTime.Now;
                        BingRoute();
                        return true;
                    case 8:
                        startTime = DateTime.Now;
                        GoogleReverseGeocode();
                        return true;
                    case 9:
                        startTime = DateTime.Now;
                        GoogleRoute();
                        return true;
                    case 11:
                        startTime = DateTime.Now;
                        MapquestRoute();
                        return true;
                    case 12:
                        startTime = DateTime.Now;
                        MapquestGeocode();
                        return true;
                    case 13:
                        startTime = DateTime.Now;
                        MapquestReverseGeocode();
                        return true;
                    case 1000:
                        GetKML();
                        return true;
                    case 2000:
                        SetUserData();
                        return true;
                    case 2001:
                        GetUserData();
                        return true;
                    case 2002:
                        startTime = DateTime.Now;
                        GetOSMData();
                        return true;
                    case 2003:
                        GetSpatialStreamParcelData();
                        return true;
                    case 2004:
                        GetRequest();
                        return true;
                    case 3000:
                        RenderTilesOnCanvas();
                        return true;
                    case 4000:
                        GetMapboxMetadata();
                        return true;
                    case 4001:
                        GetMapboxGridData();
                        return true;
                    case 9996:
                        GetBirdsEyeQuadkeys();
                        return true;
                    case 9998:
                        GetValidVendorDatasets();
                        return true;
                    case 9999:
                        ShowLicenseInfo();
                        return true;
                    default:
                        Trace(2,"GEN - Unknown command received:" + cmd);
                        return true;
                }
            }
            catch (Exception e)
            {
                Trace(2,"ERROR: " + e.Message);
                Trace(2,e.StackTrace);
                return true;
            }
        }



        private void GetSpatialStreamParcelData()
        {
            string Longitude = this.acpGetString();

            string Latitude = this.acpGetString();

            string filename = this.acpGetString();

            try
            {
                string URL = "http://parcelstream.com/query.aspx?Geofilter=POINT(" + Longitude + "%20" + Latitude+")";

                Trace(2,"GetSpatialStreamParcelData() URL : " + URL);

                WebRequest request = HttpWebRequest.Create(URL);
                WebResponse response = request.GetResponse();

                Stream s = response.GetResponseStream();

                StreamReader oReader = new StreamReader(s);

                StreamWriter oWriter = new StreamWriter(filename);
                oWriter.Write(oReader.ReadToEnd());

                oWriter.Close();
                oReader.Close();
                s.Close();
                response.Close();
                
                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
            }

        }

        private void GetRequest()
        {
            string url = this.acpGetString();
            string filename = this.acpGetString();

            try
            {
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();

                Stream s = response.GetResponseStream();

                StreamReader oReader = new StreamReader(s);

                StreamWriter oWriter = new StreamWriter(filename);
                oWriter.Write(oReader.ReadToEnd());

                oWriter.Close();
                oReader.Close();
                s.Close();
                response.Close();

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
            }

        }

        private void GetOSMData()
        {
            float minLon;
            float minLat;
            float maxLon;
            float maxLat;
            
            minLon = this.acpGetFloat();
            minLat = this.acpGetFloat();
            maxLon = this.acpGetFloat();
            maxLat = this.acpGetFloat();

            string XAPIPredicate = this.acpGetString();



            string filename = this.acpGetString();

            try
            {
                string URL;

                // if we have a XAPIPredicate, then we *have* to use the XAPI interface...
                if (XAPIPredicate != EMPTY_STRING)
                {
                    URL = "http://www.informationfreeway.org/api/0.6/map?bbox=" + minLon + "," + minLat + "," + maxLon + "," + maxLat + XAPIPredicate;
                }
                else
                {
                    // ... otherwise we can use the regular OSM API interface
                    URL = "http://api.openstreetmap.org/api/0.6/map?bbox=" + minLon + "," + minLat + "," + maxLon + "," + maxLat;
                }

                Trace(2,"GetOSMData() URL : " + URL);

                WebRequest request = HttpWebRequest.Create(URL);
                WebResponse response = request.GetResponse();

                Stream s = response.GetResponseStream();

                StreamReader oReader = new StreamReader(s);

                StreamWriter oWriter = new StreamWriter(filename);
                oWriter.Write(oReader.ReadToEnd());

                oWriter.Close();
                oReader.Close();
                s.Close();
                response.Close();
                
                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
            }


        }

        private void SetTraceLevel()
        {
            TraceLevel = this.acpGetInt();
            writeError("Trace Level set to: " + TraceLevel);
        }

        private void Trace(int level,string info)
        {
            if (TraceLevel >= level)
            {
                writeError(level + ": " + info);
            }
        }


        private void writeErrorCode(int errorCode)
        {
            writeError("Web Maps Connector Error code " + errorCode + ".  Please contact support@ifactorconsulting.com and reference this error code.");
        }

        /// <summary>
        /// Can be used to write output to the Magik prompt.
        /// </summary>
        /// <param name="aString"></param>
        private void writeError(String aString)
        {
            Console.Error.WriteLine(aString);
        }


        public void BingMetadata()
        {
            double lat, lon;
            int zoom;
            MapStyle type;
            lat = this.acpGetDouble();
            lon = this.acpGetDouble();
            zoom = this.acpGetInt();
            type = (MapStyle)this.acpGetInt();

            string separator = this.acpGetString();

            string log;
            log = "Lat=" + lat + " Lon=" + lon + " Zoom=" + zoom + " type = " + type;
            Trace(2,"Start Metadata request with " + log);
            try
            {
                string result = RequestImageryMetadata(lat, lon, zoom, type, separator);
                // Send the file(s)
                this.acpPutString(result);
                // Flag it a success
                this.acpPutBool(true);
            }
            catch (Exception e)
            {
                Trace(2,"Failed with " + e.Message);
                string err = "META - Error " + e.Message + " [Lat=" + lat + " Lon=" + lon + " Zoom=" + zoom + "]";
                this.acpPutString(err);
                this.acpPutBool(false);
            }
        }

        /// <summary>
        /// processes the Reverse Geocode by reading from ACP and writing to ACP results.
        /// </summary>
        public void BingReverseGeocode()
        {
            double lat, lon;
            lat = this.acpGetDouble();
            lon = this.acpGetDouble();

            try
            {
                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            string log;
            log = "Lat=" + lat + " Lon=" + lon;
            Trace(2,"Start Bing Reverse Geocode request with " + log);
            try
            {
                string result = BingReverseGeocode(lat, lon);

                Trace(2,"result: " + result);
                // Send the result
                this.acpPutString(result);
                // Flag it a success
                this.acpPutBool(true);
            }
            catch (Exception e)
            {
                Trace(2,"Failed with " + e.Message);
                string err = " Error " + e.Message + " [Lat=" + lat + " Lon=" + lon + "]";
                this.acpPutString(err);
                this.acpPutBool(false);
            }
        }

        public void GoogleReverseGeocode()
        {
            double lat, lon;
            lat = this.acpGetDouble();
            lon = this.acpGetDouble();

            XmlDocument doc = new XmlDocument();

            try
            {
                string aURL = GoogleRoot+ "/maps/api/geocode/xml?latlng=" + lat + "," + lon + "&sensor=false";

                aURL = SignForGoogleAPI_if_appropriate(aURL);

                Trace(3, "GoogleReverseGeocode: " + aURL);

                doc.Load(aURL);

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            this.acpPutInt(doc.DocumentElement.GetElementsByTagName("result").Count);
            this.acpFlush();

            foreach (XmlElement result in doc.DocumentElement.GetElementsByTagName("result"))
            {
                string formatted_address = result.GetElementsByTagName("formatted_address")[0].InnerText;
                Trace(2, "  Description: " + formatted_address);

                this.acpPutString(formatted_address);

                this.acpPutInt(result.GetElementsByTagName("address_component").Count);
                this.acpFlush();

                foreach (XmlElement addressComponent in result.GetElementsByTagName("address_component"))
                {

                    this.acpPutString(addressComponent.GetElementsByTagName("long_name")[0].InnerText);
                    this.acpPutString(addressComponent.GetElementsByTagName("short_name")[0].InnerText);

                    this.acpPutInt(addressComponent.GetElementsByTagName("type").Count);
                    this.acpFlush();

                    foreach (XmlElement type in addressComponent.GetElementsByTagName("type"))
                    {
                        this.acpPutString(type.InnerText);
                    }
                }

                // lat/lng values are always returned with decimal point (as opposed to decimal comma) so we need
                // to ensure that we always use the en-US culture for decoding those values.
                this.acpPutDouble(Convert.ToDouble(result.SelectSingleNode("geometry/location/lat").InnerText,ciEnUS));
                this.acpPutDouble(Convert.ToDouble(result.SelectSingleNode("geometry/location/lng").InnerText,ciEnUS));
                this.acpPutString(result.SelectSingleNode("geometry/location_type").InnerText);

            }
        }

        public void MapquestReverseGeocode()
        {
            double lat, lon;
            lat = this.acpGetDouble();
            lon = this.acpGetDouble();

            XmlDocument doc = new XmlDocument();

            try
            {
                string aURL = "http://open.mapquestapi.com/nominatim/v1/reverse?format=xml&lat="+lat+"&lon="+lon;

                Trace(3, "MapquestReverseGeocode: " + aURL);

                doc.Load(aURL);

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            this.acpPutString(doc.DocumentElement.SelectSingleNode("/reversegeocode/result").InnerText);
            this.acpFlush();
        }

        private string getBingAPIKey()
        {
            if ((BingAPIKey == null) || (BingAPIKey.Length == 0))
            {
                throw new Exception("Missing Bing Map Key");
            }
            else
            {
                return BingAPIKey;
            }
        }

        public void BingGeocode()
        {
            string query = this.acpGetString();

            BingMapsRESTToolkit.Response response = null;

            try
            {

                string url = "http://dev.virtualearth.net/REST/v1/Locations/" + query + "?key=" + getBingAPIKey();

                using (var client = new WebClient())
                {
                    string responseString = client.DownloadString(url);

                    response = JsonConvert.DeserializeObject<BingMapsRESTToolkit.Response>(responseString);
                }

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            BingMapsRESTToolkit.Location[] locationResources = response.ResourceSets[0].Resources;

            this.acpPutInt(locationResources.Length);
            this.acpFlush();

            foreach (BingMapsRESTToolkit.Location location in locationResources)
            {
                Trace(2, "Sending Result " + location.ToString());
                //  aResult.EntityType
                //  aResult.BestView 
                //  aResult.MatchCodes 
                Trace(2, "  Description: " + location.Name);

                this.acpPutString(location.Name);
                this.acpPutString(location.Confidence.ToString());

                if (location.EntityType == null)
                {
                    this.acpPutString("null");
                }
                else
                {
                    this.acpPutString(location.EntityType.ToString());
                }

                int locationsLength = location.GeocodePoints.Length;
                this.acpPutInt(locationsLength);
                this.acpFlush();

                foreach (BingMapsRESTToolkit.Point point in location.GeocodePoints)
                {
                    double latitude = point.Coordinates[0];
                    double longitude = point.Coordinates[1];
                    Trace(2, "  Latitude:    " + latitude);
                    Trace(2, "  Longitude:   " + longitude);

                    this.acpPutDouble(latitude);
                    this.acpPutDouble(longitude);
                    this.acpPutString(point.CalculationMethod);

                    this.acpFlush();
                }

                int MatchCodesLength = location.MatchCodes.Length;
                this.acpPutInt(MatchCodesLength);
                this.acpFlush();

                foreach (String aMatchCode in location.MatchCodes)
                {
                    Trace(2, "  MatchCode:    " + aMatchCode);

                    this.acpPutString(aMatchCode);

                    this.acpFlush();
                }

            }
        }
        public void GoogleGeocode()
        {
            string query = this.acpGetString();

            XmlDocument doc= new XmlDocument();

            try
            {
                string aURL = GoogleRoot +"/maps/api/geocode/xml?address="+ query + "&sensor=false";

                aURL = SignForGoogleAPI_if_appropriate(aURL);

                Trace(3, "  GoogleGeocode URL: " + aURL);

                doc.Load(aURL);

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            this.acpPutInt(doc.DocumentElement.GetElementsByTagName("result").Count);
            this.acpFlush();

            foreach (XmlElement result in doc.DocumentElement.GetElementsByTagName("result"))
            {
                string formatted_address = result.GetElementsByTagName("formatted_address")[0].InnerText;
                Trace(2, "  Description: " + formatted_address);

                this.acpPutString(formatted_address);

                this.acpPutInt(result.GetElementsByTagName("address_component").Count);
                this.acpFlush();

                foreach (XmlElement addressComponent in result.GetElementsByTagName("address_component"))
                {

                    this.acpPutString(addressComponent.GetElementsByTagName("long_name")[0].InnerText);
                    this.acpPutString(addressComponent.GetElementsByTagName("short_name")[0].InnerText);

                    this.acpPutInt(addressComponent.GetElementsByTagName("type").Count);
                    this.acpFlush();

                    foreach (XmlElement type in addressComponent.GetElementsByTagName("type"))
                    {
                        this.acpPutString(type.InnerText);
                    }
                }

                // lat/lng values are always returned with decimal point (as opposed to decimal comma) so we need
                // to ensure that we always use the en-US culture for decoding those values.
                this.acpPutDouble(Convert.ToDouble(result.SelectSingleNode("geometry/location/lat").InnerText,ciEnUS));
                this.acpPutDouble(Convert.ToDouble(result.SelectSingleNode("geometry/location/lng").InnerText,ciEnUS));
                this.acpPutString(result.SelectSingleNode("geometry/location_type").InnerText);

            }
        }

        public void MapquestGeocode()
        {
            string query = this.acpGetString();

            XmlDocument doc = new XmlDocument();

            try
            {
                string aURL = "http://open.mapquestapi.com/nominatim/v1/search?format=xml&q="+query+"&addressdetails=1";

                Trace(3, "  MapquestGeocode URL: " + aURL);

                doc.Load(aURL);

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            this.acpPutInt(doc.DocumentElement.SelectNodes("/searchresults/place").Count);
            this.acpFlush();

            foreach (XmlElement result in doc.DocumentElement.SelectNodes("/searchresults/place"))
            {
                string formatted_address = result.Attributes["display_name"].Value;
                Trace(2, "  Description: " + formatted_address);

                this.acpPutString(formatted_address);
                this.acpPutString(result.Attributes["class"].Value);
                this.acpPutString(result.Attributes["type"].Value);
                // lat/lng values are always returned with decimal point (as opposed to decimal comma) so we need
                // to ensure that we always use the en-US culture for decoding those values.
                this.acpPutDouble(Convert.ToDouble(result.Attributes["lat"].Value, ciEnUS));
                this.acpPutDouble(Convert.ToDouble(result.Attributes["lon"].Value, ciEnUS));
            }
        }


        private void GetScaledSize(double scale_factor, int width, int height, out int adjWidth, out int adjHeight)
        {
            if (scale_factor < 0.01) { scale_factor = 1; }
            adjWidth = (int)(width * (scale_factor));
            adjHeight = (int)(height * (scale_factor));
            return;
        }


        public string BingReverseGeocode(double latitude, double longitude)
        {
            try
            {
                BingMapsRESTToolkit.Response response = null;

                string url = "http://dev.virtualearth.net/REST/v1/Locations/" + latitude + "," + longitude + "?key=" + getBingAPIKey();

                using (var client = new WebClient())
                {
                    string responseString = client.DownloadString(url);

                    response = JsonConvert.DeserializeObject<BingMapsRESTToolkit.Response>(responseString);
                }

                return response.ResourceSets[0].Resources[0].Address.FormattedAddress;
            }
            catch (Exception ex)
            {
                Trace(2, "ERROR: " + ex.Message);
                Trace(2, ex.StackTrace);
                return "An exception occurred: " + ex.Message;
            }

        }

        private string RequestImageryMetadata(double lat, double lon, int zoom, ImageryService.MapStyle type, string separator)
        {
            try
            {
                ImageryMetadataRequest metadataRequest = new iFactor.ImageryService.ImageryMetadataRequest();

                // Set credentials using a valid Virtual Earth Token
                metadataRequest.Credentials = new iFactor.ImageryService.Credentials();
                metadataRequest.Credentials.ApplicationId = getBingAPIKey();

                // Set the imagery metadata request options
                iFactor.ImageryService.Location centerLocation = new iFactor.ImageryService.Location();
                centerLocation.Latitude = lat;
                centerLocation.Longitude = lon;

                metadataRequest.Options = new ImageryMetadataOptions();
                metadataRequest.Options.Location = centerLocation;
                metadataRequest.Options.ZoomLevel = zoom;

                metadataRequest.Style = type;

                // Make the imagery metadata request 
                ImageryServiceClient imageryService = new iFactor.ImageryService.ImageryServiceClient();

                ImageryMetadataResponse metadataResponse = imageryService.GetImageryMetadata(metadataRequest);

                ImageryMetadataResult result = metadataResponse.Results[0];

                string result_str = separator;

                result_str = result_str + metadataResponse.Results[0].Vintage.From.ToString() +
                             separator + metadataResponse.Results[0].Vintage.To.ToString();

                return result_str;

            }
            catch (Exception ex)
            {
                Trace(2, "ERROR: " + ex.Message);
                Trace(2, ex.StackTrace);
                return "An exception occurred: " + ex.Message;
            }
        }


        private IWebProxy getWebProxy()
        {

            // there is no need to cache the Web Proxy because we only calculate it once
            // when this service is started.

            // first see if the webProxy info was overridden for this user...
            
            // by default, always try to get default WebProxy from the registry first...
            IWebProxy proxy = WebRequest.GetSystemWebProxy();

            Trace(3,"getWebProxy()  DefaultWebProxy from registry: " + proxy.ToString());

            // ... then check to see if we have somehow overwritten the WebProxy setting in the WMC configurations
            string webProxyAddress = GetUserData(UserWebProxyDataKey);

            if (webProxyAddress == "")
            {
                // ... if not, then use the default setting.
                webProxyAddress = Properties.Settings.Default.webProxyAddress;
            }
            else
            {
                Trace(3,"getWebProxy() found a web proxy address specific to this user: " + webProxyAddress);
            }


            if ((webProxyAddress != null) && (webProxyAddress.Length > 0))
            {
                Trace(3,"getWebProxy()  using this WMC-specified webProxy: " + webProxyAddress);
                proxy = new WebProxy(webProxyAddress);
            }

            if (Properties.Settings.Default.webProxyPromptUserForCredentials == true)
            {
                using (UserCredentialsDialog dialog = new UserCredentialsDialog("wmc_web_proxy"))
                {
                    
                    dialog.Caption = StringResource.web_proxy_credentials_required;
                    dialog.SaveChecked = true;
                    dialog.Message = string.Format(StringResource.please_enter_your_credentials_for_web_proxy, webProxyAddress);
                    dialog.Flags = UserCredentialsDialogFlags.ShowSaveCheckbox |
                                    UserCredentialsDialogFlags.GenericCredentials |
                                    UserCredentialsDialogFlags.AlwaysShowUI |
                                    UserCredentialsDialogFlags.ExpectConfirmation;


                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        proxy.Credentials = new System.Net.NetworkCredential(dialog.User, dialog.PasswordToString());
                    }

                    dialog.ConfirmCredentials(true);
                    }
            }
            else
            {
                proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                //proxy.UseDefaultCredentials = true;
            }

            String proxyUriTest = "http://www.ifactorconsulting.com/";

            if (!proxyUriTest.Equals(proxy.GetProxy(new Uri(proxyUriTest)).AbsoluteUri))
            {
                writeError("Using proxy: " + proxy.GetProxy(new Uri(proxyUriTest)).AbsoluteUri);
            }
            else
            {
                writeError("Using proxy: NONE");
            }

            return proxy;
        }

        private void GetValidVendorDatasets()
        {
            this.acpPutInt(otherVendorModes.Length);

            foreach (string mode in otherVendorModes)
            {
                this.acpPutString(mode);
            }

            this.acpFlush();
        }

        private void ShowLicenseInfo()
        {
            this.acpPutString(customerID);
            this.acpPutString("PERPETUAL");
            this.acpPutString(vendorModes);
        }

        private void GetKML()
        {
            string kmlURL = this.acpGetString();

            string description;
            string geomtype;

            KMLib.KMLRoot root = KMLib.KMLRoot.Load(kmlURL);

            this.acpPutInt(root.Document.List.Count);

            foreach (KMLib.Feature.Placemark feat in root.Document.List)
            {
                this.acpPutString(feat.name);
                description = (feat.description == null) ? "" : feat.description;
                this.acpPutString(description);

                geomtype = feat.GeometryType.ToString();
                this.acpPutString(geomtype);

                if (geomtype == "Point")
                {
                    acpPutKMLPointGeometry(feat.Point);
                }
                else if (geomtype == "LineString")
                {
                    acpPutKMLLineStringGeometry(feat.LineString);
                }
                else if (geomtype == "Polygon")
                {
                    acpPutKMLPolygonGeometry(feat.Polygon);
                }

            }

        }

        private void acpPutKMLPointGeometry(KMLib.Geometry.KmlPoint aPoint)
        {
            this.acpPutDouble(aPoint.Longitude);
            this.acpPutDouble(aPoint.Latitude);
        }

        private void acpPutKMLLineStringGeometry(KMLib.LineString aLineString)
        {
            this.acpPutInt(aLineString.coordinates.Count);

            foreach(Core.Geometry.Point3D aPoint in aLineString.coordinates)
            {
                this.acpPutDouble(aPoint.X);
                this.acpPutDouble(aPoint.Y);
            }
        }

        private void acpPutKMLPolygonGeometry(KMLib.Polygon aPolygon)
        {
            KMLib.Coordinates coords;

            // Outer Boundary
            coords = aPolygon.OuterBoundaryIs.LinearRing.Coordinates;

            this.acpPutInt(coords.Count);
            foreach (Core.Geometry.Point3D aPoint in coords)
            {
                this.acpPutDouble(aPoint.X);
                this.acpPutDouble(aPoint.Y);
            }

            // Inner Boundary
            coords = aPolygon.InnerBoundaryIs.LinearRing.Coordinates;

            this.acpPutInt(coords.Count);
            foreach (Core.Geometry.Point3D aPoint in coords)
            {
                this.acpPutDouble(aPoint.X);
                this.acpPutDouble(aPoint.Y);
            }
        }

        private void BingRoute()
        {
            Trace(2, "BingRoute()");
            try
            {
                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            try
            {
                RouteService.RouteRequest routeRequest = new RouteService.RouteRequest();

                // Set the credentials using a valid Bing Maps token
                routeRequest.Credentials = new RouteService.Credentials();
                routeRequest.Credentials.ApplicationId = getBingAPIKey();
                routeRequest.Options = new RouteService.RouteOptions();
                routeRequest.Options.RoutePathType = RoutePathType.Points;

                // get total number of points
                int numberOfIntermediatePoints = this.acpGetInt();

                RouteService.Waypoint[] waypoints = new RouteService.Waypoint[numberOfIntermediatePoints + 2];

                int wpIdx = 0;
                // START POINT
                waypoints[wpIdx] = new RouteService.Waypoint();
                waypoints[wpIdx].Description = this.acpGetString();
                waypoints[wpIdx].Location = new RouteService.Location();
                waypoints[wpIdx].Location.Latitude = this.acpGetDouble();
                waypoints[wpIdx].Location.Longitude = this.acpGetDouble();

                Trace(2,"  Waypoint #"+wpIdx+" Description: '" +waypoints[wpIdx].Description+ "' Latitude: " + waypoints[wpIdx].Location.Latitude+ "  Longitude: " +waypoints[wpIdx].Location.Longitude); 

                // INTERMEDIATE POINTS
                for (int i = 0; i < numberOfIntermediatePoints; i++)
                {
                    wpIdx++;
                    waypoints[wpIdx] = new RouteService.Waypoint();
                    waypoints[wpIdx].Description = this.acpGetString();
                    waypoints[wpIdx].Location = new RouteService.Location();
                    waypoints[wpIdx].Location.Latitude = this.acpGetDouble();
                    waypoints[wpIdx].Location.Longitude = this.acpGetDouble();
                
                    Trace(2, "  Waypoint #" + wpIdx + " Description: '" + waypoints[wpIdx].Description + "' Latitude: " + waypoints[wpIdx].Location.Latitude + "  Longitude: " + waypoints[wpIdx].Location.Longitude);
                }

                // END POINT
                wpIdx++;
                waypoints[wpIdx] = new RouteService.Waypoint();
                waypoints[wpIdx].Description = this.acpGetString();
                waypoints[wpIdx].Location = new RouteService.Location();
                waypoints[wpIdx].Location.Latitude = this.acpGetDouble();
                waypoints[wpIdx].Location.Longitude = this.acpGetDouble();

                Trace(2, "  Waypoint #" + wpIdx + " Description: '" + waypoints[wpIdx].Description + "' Latitude: " + waypoints[wpIdx].Location.Latitude + "  Longitude: " + waypoints[wpIdx].Location.Longitude); 


                routeRequest.Waypoints = waypoints;

                // Make the calculate route request
                RouteServiceClient routeService = new RouteServiceClient();
               
                RouteService.RouteResponse routeResponse = routeService.CalculateRoute(routeRequest);

                RouteService.RouteResult result = routeResponse.Result;

                // Iterate through each itinerary item to get the route directions
                iFactor.RouteService.RouteLeg[] legs = result.Legs;
                int legLength = legs.Length;
                this.acpPutInt(legLength);

                foreach (iFactor.RouteService.RouteLeg leg in legs)
                {
                    int itineraryLength = leg.Itinerary.Length;

                    this.acpPutInt(itineraryLength);

                    foreach (RouteService.ItineraryItem itItem in leg.Itinerary)
                    {
                        RouteService.Location loc = itItem.Location;

                        this.acpPutDouble(loc.Longitude);
                        this.acpPutDouble(loc.Latitude);
                        this.acpPutString(itItem.Text);
                        // by default, the distance unit type is "Kilometer".  If we explicitly
                        // change it in this C# code, then we also need to change the corresponding
                        // Magik code.
                        this.acpPutDouble(itItem.Summary.Distance);
                        this.acpPutLong(itItem.Summary.TimeInSeconds);
                    }
                }

                RouteSummary aSummary = result.Summary;

                // by default, the distance unit type is "Kilometer".  If we explicitly
                // change it in this C# code, then we also need to change the corresponding
                // Magik code.
                this.acpPutDouble(aSummary.Distance);
                this.acpPutLong(aSummary.TimeInSeconds);

                int pathLength = result.RoutePath.Points.Length;
                this.acpPutInt(pathLength);

                foreach (RouteService.Location loc in result.RoutePath.Points)
                {
                    this.acpPutDouble(loc.Longitude);
                    this.acpPutDouble(loc.Latitude);
                }


                this.acpFlush();
            }
            catch (Exception ex)
            {
                Trace(2,ex.ToString());
            }

        }

        private void GoogleRoute()
        {
            double originLat = this.acpGetDouble();
            double originLong = this.acpGetDouble();
            double destinationLat = this.acpGetDouble();
            double destinationLong = this.acpGetDouble();

            // gather up the direction information...
            XmlDocument doc = new XmlDocument();

            try
            {
                string aURL = GoogleRoot + "/maps/api/directions/xml?" +
                              "&sensor=false" +
                              "&origin=" + originLat + "," + originLong +
                              "&destination=" + destinationLat + "," + destinationLong;

                aURL = SignForGoogleAPI_if_appropriate(aURL);

                Trace(3,"GOOGLE ROUTING: " + aURL);

                doc.Load(aURL);

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            XmlElement route = (XmlElement)doc.DocumentElement.GetElementsByTagName("route")[0];

            this.acpPutInt(route.GetElementsByTagName("leg").Count);
            this.acpFlush();

            foreach (XmlElement leg in route.GetElementsByTagName("leg"))
            {
                this.acpPutInt(leg.GetElementsByTagName("step").Count);
                this.acpFlush();

                foreach (XmlElement aStep in leg.GetElementsByTagName("step"))
                {
                    // lat/lng values are always returned with decimal point (as opposed to decimal comma) so we need
                    // to ensure that we always use the en-US culture for decoding those values.
                    this.acpPutDouble(Convert.ToDouble(aStep.SelectSingleNode("start_location/lng").InnerText,ciEnUS));
                    this.acpPutDouble(Convert.ToDouble(aStep.SelectSingleNode("start_location/lat").InnerText,ciEnUS));
                    this.acpPutString(aStep.SelectSingleNode("html_instructions").InnerText);
                    this.acpPutDouble(Convert.ToDouble(aStep.SelectSingleNode("distance/value").InnerText,ciEnUS));
                    this.acpPutLong(Convert.ToInt16(aStep.SelectSingleNode("duration/value").InnerText));
                }
                this.acpPutDouble(Convert.ToDouble(leg.SelectSingleNode("distance/value").InnerText,ciEnUS));
                this.acpPutLong(Convert.ToInt64(leg.SelectSingleNode("duration/value").InnerText));
            }

            double[] coords = decodeGooglePolyline(route.SelectSingleNode("overview_polyline/points").InnerText);
            this.acpPutInt(coords.Length);
            this.acpFlush();
            foreach (double c in coords)
            {
                this.acpPutDouble(c);
            }
        }

        // uses google's decodePolyline algorithm
        double[] decodeGooglePolyline(string polyline)
        {
            // thanks to http://www.supware.net/other-fun-stuff/googlemapping/ for the code.

            if (polyline == null || polyline == "") return null;

            char[] polylinechars = polyline.ToCharArray();
            int index = 0;
            ArrayList points = new ArrayList();
            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylinechars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    //next5bits = Convert.ToInt32(polylinechars[index++]) - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                // lat/lng values are always returned with decimal point (as opposed to decimal comma) so we need
                // to ensure that we always use the en-US culture for decoding those values.
                points.Add(Convert.ToDouble(currentLat,ciEnUS) / 100000.0);
                points.Add(Convert.ToDouble(currentLng,ciEnUS) / 100000.0);
            }

            return (double[])points.ToArray(typeof(Double));
        }

        private void MapquestRoute()
        {
            double originLat = this.acpGetDouble();
            double originLong = this.acpGetDouble();
            double destinationLat = this.acpGetDouble();
            double destinationLong = this.acpGetDouble();

            // gather up the direction information...
            XmlDocument doc = new XmlDocument();

            try
            {
                string from = originLat + "," + originLong;
                string to = destinationLat + "," + destinationLong;
                string routeType = "fastest";

                string aURL = "http://open.mapquestapi.com/directions/v1/route?outFormat=xml&generalize=2&from="+from+"&to="+to+"&routeType="+routeType + "&narrativeType=html&destinationManeuverDisplay=false&unit=k&shapeFormat=raw";

                Trace(2, "MAPQUEST ROUTING: " + aURL);

                doc.Load(aURL);

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }

            XmlElement route = (XmlElement)doc.DocumentElement.GetElementsByTagName("route")[0];
            XmlElement legsElement = (XmlElement)route.GetElementsByTagName("legs")[0];

            this.acpPutInt(legsElement.GetElementsByTagName("leg").Count);
            this.acpFlush();

            foreach (XmlElement leg in legsElement.GetElementsByTagName("leg"))
            {
                XmlElement maneuversElement = (XmlElement)leg.GetElementsByTagName("maneuvers")[0];

                this.acpPutInt(maneuversElement.GetElementsByTagName("maneuver").Count);
                this.acpFlush();

                foreach (XmlElement aManeuver in maneuversElement.GetElementsByTagName("maneuver"))
                {
                    // lat/lng values are always returned with decimal point (as opposed to decimal comma) so we need
                    // to ensure that we always use the en-US culture for decoding those values.
                    this.acpPutDouble(Convert.ToDouble(aManeuver.SelectSingleNode("startPoint/lng").InnerText, ciEnUS));
                    this.acpPutDouble(Convert.ToDouble(aManeuver.SelectSingleNode("startPoint/lat").InnerText, ciEnUS));
                    this.acpPutString(aManeuver.SelectSingleNode("narrative").InnerText);
                    this.acpPutDouble(Convert.ToDouble(aManeuver.SelectSingleNode("distance").InnerText, ciEnUS));
                    this.acpPutLong(Convert.ToInt64(aManeuver.SelectSingleNode("time").InnerText));
                }
                this.acpPutDouble(Convert.ToDouble(leg.SelectSingleNode("distance").InnerText, ciEnUS));
                this.acpPutLong(Convert.ToInt64(leg.SelectSingleNode("time").InnerText));
            }

            XmlElement shapePoints = (XmlElement)route.SelectSingleNode("shape/shapePoints");
            
            this.acpPutInt(shapePoints.GetElementsByTagName("latLng").Count * 2);

            this.acpFlush();

            foreach (XmlElement aLatLng in shapePoints.GetElementsByTagName("latLng"))
            {
                this.acpPutDouble(Convert.ToDouble(aLatLng.SelectSingleNode("lat").InnerText, ciEnUS));
                this.acpPutDouble(Convert.ToDouble(aLatLng.SelectSingleNode("lng").InnerText, ciEnUS));
            }
        }



        private string ResolveURIforQuadkey(string quadkey, string vendorDataset, string vendorTileType, string extraRenderingInfo)
        {
            Trace(2,"quadkey: " + quadkey + "  vendorDataset: " + vendorDataset + "  vendorTileType: " + vendorTileType);

            switch (vendorDataset)
            {
                case "arcgis_dataset":
                    return ArcGISResolveURIforQuadkey(quadkey, vendorTileType,extraRenderingInfo);

                case "basemapat_dataset":
                    return BasemapatResolveURIforQuadkey(quadkey, vendorTileType);

                case "bing_dataset":
                    return BingResolveURIforQuadkey(quadkey, vendorTileType);

                case "birdseye_dataset":
                    return BirdseyeResolveURIforQuadkey(quadkey, vendorTileType);

                case "weatherbug_dataset":
                    return WeatherBugResolveURIforQuadkey(quadkey, vendorTileType,extraRenderingInfo);

                case "google_dataset":
                    return GoogleResolveURIforQuadkey(quadkey, vendorTileType,extraRenderingInfo);

                case "mapbox_dataset":
                    return MapboxResolveURIforQuadkey(quadkey, vendorTileType);

                case "osm_dataset":
                    return OSMResolveURIforQuadkey(quadkey, vendorTileType);

                case "mapquest_dataset":
                    return MapQuestResolveURIforQuadkey(quadkey, vendorTileType);

                case "spatialstream_dataset":
                    return SpatialStreamResolveURIforQuadkey(quadkey, vendorTileType);

                case "wms_dataset":
                    return WMSResolveURIforQuadkey(quadkey, vendorTileType,extraRenderingInfo);

                default:
                    return "";
            }
        }


        private string BingResolveURIforQuadkey(string quadkey, string vendorTileType)
        {
            if (vendorTileType == "traffic_tile")
            {
                // if we use ONLY server 0, then it works fairly well.  Using additional servers sometimes drops tiles.
                return "http://t0.tiles.virtualearth.net/tiles/dp/content?p=tf&a=" + quadkey + "&token=" + getBingAPIKey();
            }
            else
            {
                BingServerNumber += 1;

                if (BingServerNumber >= bingMapsRequestUris.Count)
                {
                    // in the past it seemed like using multiple Microsoft servers caused tile holes.  At ShoMe Power, we discovered that using
                    // a single server actually caused timeouts and holes.  So we are reverting to spreading out the load amongst multiple
                    // servers.  If we still get tile holes with multiple servers, then we need to figure out a different approach.
                    BingServerNumber = 0;
                }



                return Regex.Replace(bingMapsRequestUris[BingServerNumber], "{quadkey}", quadkey);
            }
        }



        private string MapboxResolveURIforQuadkey(string quadkey, string vendorTileType)
        {
            MapboxServerNumber += 1;

            if (MapboxServerNumber >= mapboxRequestUris.Count)
            {
                MapboxServerNumber = 0;
            }

            int tileX = 0;
            int tileY = 0;
            int zoomLevel = 0;

            TileSystem.QuadKeyToTileXY(quadkey, out tileX, out tileY, out zoomLevel);

            String url = mapboxRequestUris[MapboxServerNumber];
            url = Regex.Replace(url, "{z}", zoomLevel.ToString());
            url = Regex.Replace(url, "{x}", tileX.ToString());
            url = Regex.Replace(url, "{y}", tileY.ToString());

            return url;
        }


        private string BirdseyeResolveURIforQuadkey(string quadkey, string vendorTileType)
        {
            writeError("QUADKEY: " + quadkey);
            string code = "";

            switch (vendorTileType)
            {
                case "birdseye":
                    code = "svi";
                    break;
                case "birdseye_labelled":
                    code = "svl";
                    break;
                default:
                    return "";
            }

            BingServerNumber += 1;

            if (BingServerNumber > 7)
            {
                // in the past it seemed like using multiple Microsoft servers caused tile holes.  At ShoMe Power, we discovered that using
                // a single server actually caused timeouts and holes.  So we are reverting to spreading out the load amongst multiple
                // servers.  If we still get tile holes with multiple servers, then we need to figure out a different approach.
                BingServerNumber = 0;
            }

            string[] info = quadkey.Split('|');

            return "http://ecn.t" + BingServerNumber + ".tiles.virtualearth.net/tiles/" + code + info[1] + "?g=681"+info[2]+"&n=z";
        }

        private void GetBirdsEyeQuadkeys()
        {
            double longitude = this.acpGetDouble();

            double latitude = this.acpGetDouble();
            int zoomLevel = this.acpGetInt();
            string vendorTileType = this.acpGetString();

            ImageryMetadataBirdseyeResult result = null ;
            string imageUri = null;
            try
            {

                ImageryMetadataRequest metadataRequest = new iFactor.ImageryService.ImageryMetadataRequest();

                // Set credentials using a valid Virtual Earth Token
                metadataRequest.Credentials = new iFactor.ImageryService.Credentials();
                metadataRequest.Credentials.ApplicationId = getBingAPIKey();

                // Set the imagery metadata request options
                iFactor.ImageryService.Location centerLocation = new iFactor.ImageryService.Location();
                centerLocation.Latitude = latitude;
                centerLocation.Longitude = longitude;

                metadataRequest.Options = new ImageryMetadataOptions();
                metadataRequest.Options.Location = centerLocation;
                metadataRequest.Options.ZoomLevel = zoomLevel;

                switch (vendorTileType)
                {
                    case "birdseye_labelled":
                        metadataRequest.Style = ImageryService.MapStyle.BirdseyeWithLabels;
                        break;
                    default:
                        metadataRequest.Style = ImageryService.MapStyle.Birdseye;
                        break;
                }            

                // Make the imagery metadata request 
                ImageryServiceClient imageryService = new iFactor.ImageryService.ImageryServiceClient();

                ImageryMetadataResponse metadataResponse = imageryService.GetImageryMetadata(metadataRequest);
                result = (ImageryMetadataBirdseyeResult)metadataResponse.Results[0];

                imageUri = result.ImageUri;
                string[] subdomain = result.ImageUriSubdomains;

                imageUri = imageUri.Replace("{subdomain}", subdomain[0]).Replace("{token}", getBingAPIKey());

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
            }

            this.acpPutString(imageUri);
            this.acpPutInt(result.ImageSize.Width);
            this.acpPutInt(result.ImageSize.Height);
            this.acpPutInt(result.TilesX);
            this.acpPutInt(result.TilesY);
            
        }

        private string GoogleResolveURIforQuadkey(string quadkey, string vendorTileType, string extraInfo)
        {
            string url = "";

            string mapType = "";

            if (vendorTileType == "streetview_tile")
            {
                int tileX = 0;
                int tileY = 0;
                int zoomLevel = 0;

                TileSystem.QuadKeyToTileXY(quadkey, out tileX, out tileY, out zoomLevel);

                url = "https://mts2.google.com/mapslt?lyrs=lmc:photos_outside_streetview_116,svv&x="+tileX+"&y="+tileY+"&z="+zoomLevel+"&w=256&h=256&gl=us&hl=x-local&style=40";
            }
            else if (vendorTileType == "streetview")
            {
                url = "http://maps.googleapis.com/maps/api/streetview?sensor=false&" + quadkey;
                url = SignForGoogleAPI_if_appropriate(url);
            }
            else
            {
                switch (vendorTileType)
                {
                    case "aerial_tile":
                        mapType = "satellite";
                        break;
                    case "aerial_labelled_tile":
                        mapType = "hybrid";
                        break;
                    case "road_tile":
                    case "road_tile_transparent":
                        mapType = "roadmap";
                        break;
                    case "terrain_tile":
                        mapType = "terrain";
                        break;

                }

                url = GoogleRoot + "/maps/api/staticmap?&maptype=" + mapType + "&sensor=false" + quadkey.Split('|')[1] + "&" + extraInfo;

                url = SignForGoogleAPI_if_appropriate(url);
            }
            Trace(3,"In GoogleResolveURIforQuadkey, the url = " + url);
            return url;
        }


        private string BasemapatResolveURIforQuadkey(string quadkey, string vendorTileType)
        {
            int tileX = 0;
            int tileY = 0;
            int zoomLevel = 0;

            TileSystem.QuadKeyToTileXY(quadkey, out tileX, out tileY, out zoomLevel);

            return "http://maps.wien.gv.at/basemap/geolandbasemap/normal/google3857/" + zoomLevel + "/" + tileY + "/" + tileX + ".jpeg";
        }

        private string MapQuestResolveURIforQuadkey(string quadkey, string vendorTileType)
        {
            string url = "";
            int tileX = 0;
            int tileY = 0;
            int zoomLevel = 0;

            TileSystem.QuadKeyToTileXY(quadkey, out tileX, out tileY, out zoomLevel);

            switch (vendorTileType)
            {
                case "tile":
                    url = "http://otile1.mqcdn.com/tiles/1.0.0/osm/";
                    break;
                case "aerial_tile":
                    url = "http://oatile1.mqcdn.com/tiles/1.0.0/sat/";
                    break;
            }

            return url + quadkey.Length + "/" + tileX + "/" + tileY + ".png";
        }

 
        private string OSMResolveURIforQuadkey(string quadkey, string vendorTileType)
        {
            string url = "";
            int tileX=0;
            int tileY=0;
            int zoomLevel=0;

            TileSystem.QuadKeyToTileXY(quadkey,out tileX,out tileY,out zoomLevel);

            if (OSMServerName == 'd')
            {
                OSMServerName = 'a';
            }

            switch (vendorTileType)
            {
                case "mapnik_tile":
                case "mapnik_tile_transparent":
                    url = "http://" + OSMServerName + ".tile.openstreetmap.org/";
                    break;
                case "osmarender_tile":
                case "osmarender_tile_transparent":
                    url = "http://" + OSMServerName + ".tah.openstreetmap.org/Tiles/tile/";
                    break;
                case "cyclemap_tile":
                    url = "http://"+OSMServerName+".andy.sandbox.cloudmade.com/tiles/cycle/";
                    break;
            }

            OSMServerName = (char)((int)OSMServerName + 1);

            return url + quadkey.Length + "/" + tileX + "/" + tileY + ".png";
        }


        private string SpatialStreamResolveURIforQuadkey(string quadkey, string vendorTileType)
        {
            return "http://parcelstream.com/VEParcelTileServer.aspx?layers=parcels&tileid=" + quadkey;
        }


        private string WeatherBugResolveURIforQuadkey(string quadkey, string vendorTileType,string extraRenderingInfo)
        {
            // "lid" values:
            //  52 = Radar
            //  45 = IR Satellite
            //  46 = Visible Satellite

            String code="";

            if (vendorTileType == "lightning_strikes")
            {
                return "http://i.wxbug.net/GEOP/Tiles/GetTile_v2.aspx?pid=881dfa92-8192-410d-892f-87c0fc46870c&api_key="+WeatherBugAPIKey+"&layer=lightning&initialdelay=900000&timespan=300000&qk=" + quadkey + "&featureFilter=CloudToGround&" + extraRenderingInfo;
            }

            switch (vendorTileType)
            {
                case "doppler_radar":
                    code = "52";
                    break;
                case "ir_satellite":
                    code = "45";
                    break;
                case "visible_satellite":
                    code = "46";
                    break;
            }
            return "http://i.wxbug.net/GEOP/Tiles/GetTile_v2.aspx?pid=158460f5-a2b9-4e5c-b0c0-d503e9b6e752&lid="+code+"&c=0&fq=0&api_key="+WeatherBugAPIKey+"&qk="+quadkey+"&"+extraRenderingInfo;
        }

        private string ArcGISResolveURIforQuadkey(string quadkey, string vendorTileType, string extraRenderingInfo)
        {
            string url;

            int tileX = 0;
            int tileY = 0;
            int zoomLevel = 0;

            TileSystem.QuadKeyToTileXY(quadkey, out tileX, out tileY, out zoomLevel);

            switch (vendorTileType)
            {
                //request from pre-stored tiles
                case "world_street_map":
                    url = extraRenderingInfo + "/ArcGIS/rest/services/World_Street_Map/MapServer/tile/"+ zoomLevel + "/" + tileY + "/" + tileX;
                    break;
                case "world_imagery":
                    url = extraRenderingInfo + "/ArcGIS/rest/services/World_Imagery/MapServer/tile/" + zoomLevel + "/" + tileY + "/" + tileX;
                    break;
                default:
                    url = "";
                    break;
            }

            return url;
        }


        private string WMSResolveURIforQuadkey(string quadkey, string vendorTileType, string extraRenderingInfo)
        {
            string url;

            double ulY;
            double ulX;
            double lrY;
            double lrX;

            if (regex_3785.IsMatch(extraRenderingInfo.ToLower()))
            {
                TileSystem.QuadKeyToLRMercator(quadkey, out lrX, out lrY);
                TileSystem.QuadKeyToULMercator(quadkey, out ulX, out ulY);
            }
            else
            {
                TileSystem.QuadKeyToULLatLong(quadkey, out ulY, out ulX);
                TileSystem.QuadKeyToLRLatLong(quadkey, out lrY, out lrX);
            }

            url = extraRenderingInfo + "&WIDTH=256&HEIGHT=256&BBOX=" + ulX + "," + lrY + "," + lrX + "," + ulY;

            return url;
        }

        private void prepareMapboxMetadata(String extraRenderingInfo) {
            String[] parts = extraRenderingInfo.Split('.');

            JObject jO;

            if (mapboxMetadataCache.ContainsKey(extraRenderingInfo))
            {
                jO = mapboxMetadataCache[extraRenderingInfo];
            }
            else
            {
                String json_string = GetMapboxMetadata(parts[0], parts[1]);
                jO = JObject.Parse(json_string);
                mapboxMetadataCache[extraRenderingInfo] = jO;
            }

            mapboxRequestUris.Clear();

            foreach (JToken aPOI in jO["tiles"])
            {
                mapboxRequestUris.Add((String)aPOI);
            }
        }

        private void GetMapboxMetadata()
        {
            string account = this.acpGetString();
            string handle = this.acpGetString();

            string json_string = null;
            try
            {
                json_string = GetMapboxMetadata(account, handle);

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
            }

            this.acpPutString(json_string);
        }

        /// <summary>
        /// because we are accessing Bing Map tiles directly, Bing requires us first to make a metadata request
        /// on each map refresh in order for them to track the request against the Bing Map Key.
        /// </summary>
        private String GetMapboxMetadata(String account, String handle)
        {
            string metadataURI = "http://api.tiles.mapbox.com/v3/"+account+"."+handle+".json";

            WebRequest request = HttpWebRequest.Create(metadataURI);
            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();

            StreamReader readStream = new StreamReader(s);

            String aString = readStream.ReadToEnd();

            readStream.Close();
            s.Close();

            return aString;
        }

        private void GetMapboxGridData()
        {
            String account = this.acpGetString();
            String handle = this.acpGetString();
            int tileX = this.acpGetInt();
            int tileY = this.acpGetInt();
            int zoomLevel = this.acpGetInt();
            int pixelX = this.acpGetInt();
            int pixelY = this.acpGetInt();

            string json_string = "";
            try
            {
                JObject jO = JObject.Parse(GetMapboxMetadata(account, handle));

                String url = (String)jO["grids"].First;

                url = Regex.Replace(url, "{z}", zoomLevel.ToString());
                url = Regex.Replace(url, "{x}", tileX.ToString());
                url = Regex.Replace(url, "{y}", tileY.ToString());

                writeError("URL: " + url);

                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();

                Stream s = response.GetResponseStream();

                StreamReader readStream = new StreamReader(s);

                String js = readStream.ReadToEnd();

                readStream.Close();
                s.Close();

                String newJS = js.Substring(5,js.Length - 7);
                JObject grid = JObject.Parse(newJS);
                writeError("GRID: " + grid.ToString());
                //writeError(grid.ToString());
                String row = (String)grid["grid"][pixelY / 4];
                //writeError("ROW: " + row);
                char cell = row[pixelX / 4];
                int key = (int)cell;

                writeError("Cell: " + cell.ToString() + "  " + key.ToString());

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
            }

            this.acpPutString(json_string);
        }

        /// <summary>
        /// because we are accessing Bing Map tiles directly, Bing requires us first to make a metadata request
        /// on each map refresh in order for them to track the request against the Bing Map Key.
        /// </summary>
        private void getBingMetadata(String vendorTileType)
        {
            String imagerySet = "";

            switch (vendorTileType)
            {
                case "aerial_tile":
                    imagerySet = "Aerial";
                    break;
                case "aerial_labelled_tile":
                    imagerySet = "AerialWithLabels";
                    break;
                case "road_tile":
                case "road_tile_transparent":
                    imagerySet = "Road";
                    break;
                default:
                    imagerySet = "Road";
                    break;
            }

            string metadataURI = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + imagerySet + "?o=xml&key=" + getBingAPIKey();

            WebRequest request = HttpWebRequest.Create(metadataURI);
            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();

            StreamReader readStream = new StreamReader(s);

            String xmlString = readStream.ReadToEnd();

            XmlDocument metadataXML = new XmlDocument();

            metadataXML.LoadXml(xmlString);
            readStream.Close();
            s.Close();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(metadataXML.NameTable);
            nsmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1");

            //for some reason, at LGEKU, the bingLogoUrl was hanging. So I have moved the 
            // logo to a resource directory.
            //String bingLogoUrl = metadataXML.SelectSingleNode("//rest:BrandLogoUri", nsmgr).InnerXml;
            //writeError(bingLogoUrl);

            String bingImageUrl = HttpUtility.UrlDecode(metadataXML.SelectSingleNode("//rest:ImageUrl", nsmgr).InnerXml);

            bingImageUrl = Regex.Replace(bingImageUrl, "{culture}", Properties.Settings.Default.BingMapCulture);

            XmlNodeList bingSubdomains = metadataXML.SelectNodes("//rest:ImageUrlSubdomains/rest:string", nsmgr);

            bingMapsRequestUris.Clear();

            foreach (XmlNode sd in bingSubdomains)
            {
                bingMapsRequestUris.Add(HttpUtility.HtmlDecode(Regex.Replace(bingImageUrl, "{subdomain}", sd.InnerXml)));
            }
        }
        private void PrepareCanvasForVendor(string vendorDataset, string vendorTileType, out Dictionary<string, Bitmap> bitmapCache, 
                                            out string transactionType,out ImageAttributes anImageAttributes,out Nullable<Color> bmpTransparentColor,string extraRenderingInfo, 
                                            float opacity)
        {
            Trace(2,"PrepareForVendor()  vendorDataset: " + vendorDataset + "  vendorTileType: " + vendorTileType + "  extraRenderingInfo: " + extraRenderingInfo +
                     "   Opacity: " + opacity);

            string vendorTileKey = vendorTileType + extraRenderingInfo;

            anImageAttributes = new ImageAttributes();
            anImageAttributes.SetWrapMode(WrapMode.TileFlipXY); // used in conjunction with Bicubic interpolation, we have the best removal of tile edge artifacts.

            bmpTransparentColor = null;

            // return a bitmap cache unique for the current vendorDataset/vendorTileKey combination.
            if (!vendorBitmapCaches.ContainsKey(vendorDataset))
            {
                vendorBitmapCaches[vendorDataset] = new Dictionary<string, Dictionary<string, Bitmap>>();
            }

            if (!vendorBitmapCaches[vendorDataset].ContainsKey(vendorTileKey))
            {
                vendorBitmapCaches[vendorDataset][vendorTileKey] = new Dictionary<string, Bitmap>();
            }


            // now send back the appropriate cache.
            bitmapCache = vendorBitmapCaches[vendorDataset][vendorTileKey];

            transactionType = "";

            if (opacity < 1.0)
            {
                ColorMatrix cMatrix = new ColorMatrix();
                cMatrix.Matrix33 = opacity; //opacity 0 = completely transparent, 1 = completely opaque
                anImageAttributes.SetColorMatrix(cMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            }

            switch (vendorDataset)
            {
                case "arcgis_dataset":
                    transactionType = "ArcGIS Map Refresh";
                    break;
                case "basemapat_dataset":
                    transactionType = "basemap.at Map Refresh";
                    break;
                case "bing_dataset":

                    // calling getBingMetadata() correctly registers with Bing that we are making a map request.
                    getBingMetadata(vendorTileType);

                    transactionType = "Bing Map Refresh";

                    switch (vendorTileType)
                    {
                        case "road_tile_transparent":
                            bmpTransparentColor = Color.FromArgb(247, 244, 242);
                            break;
                        case "traffic_tile":
                            // for now, don't ever cache Traffic information.
                            bitmapCache.Clear();
                            break;
                    }

                    break;
                case "birdseye_dataset":
                    transactionType = "BirdsEye Map Refresh";
                    break;
                case "google_dataset":
                    transactionType = "Google Map Refresh";
                    break;
                case "mapbox_dataset":

                    prepareMapboxMetadata(extraRenderingInfo);
                    transactionType = "MapBox Map Refresh";

                    break;
                case "mbtiles_dataset":
                    transactionType = "MBTiles Map Refresh";
                    break;
                case "osm_dataset":
                    transactionType = "Open Street Map Refresh";

                    switch (vendorTileType)
                    {
                        case "mapnik_tile_transparent":
                            bmpTransparentColor = Color.FromArgb(241, 238, 232);
                            break;
                        case "osmarender_tile_transparent":
                            bmpTransparentColor = Color.FromArgb(248, 248, 248);
                            break;
                    }

                    break;
                case "weatherbug_dataset":
                    //if (DateTime.Now.Subtract(weatherBugTimer).Seconds > 10)
                    //{
                        // the weatherbug radar tiles seem to refresh every 5 minutes so it seems
                        // reasonable that we cache it for 2 minutes for improved performance.
                        //Trace(2, "Clearing weatherBug cache for " + vendorTileKey);
                        bitmapCache.Clear();
                        //weatherBugTimer = DateTime.Now;
                    //}
                    transactionType = "WeatherBug Map Refresh";
                    break;
                case "mapquest_dataset":
                    transactionType = "MapQuest Map Refresh";
                    break;
                case "spatialstream_dataset":
                    transactionType = "SpatialStream Map Refresh";
                    break;
                case "wms_dataset":
                    transactionType = "WMS Map Refresh";
                    break;
                default:
                    break;
            }

            // clean up the cache if necessary
            int count = 0;

            foreach (Dictionary<string, Dictionary<string, Bitmap>> iVendorCache in vendorBitmapCaches.Values)
            {
                foreach (Dictionary<string, Bitmap> iTileTypeCache in iVendorCache.Values)
                {
                    count += iTileTypeCache.Count;
                }
            }

            if (count > 200)
            {
                Trace(1,"!!!! Total bitmapCache count '' has exceeded 200.  Cleaning things up...");

                foreach (Dictionary<string, Dictionary<string, Bitmap>> iVendorCache in vendorBitmapCaches.Values)
                {
                    foreach (Dictionary<string, Bitmap> iTileTypeCache in iVendorCache.Values)
                    {
                        iTileTypeCache.Clear();
                    }
                }
            }


            Trace(2,"!!!! bitmapCache Count: " + bitmapCache.Count);

        }

        private void RequestQuadkeyTile(RequestState aState, int tryCount)
        {
            RequestQuadkeyTile(aState.Quadkey, aState.VendorDatasetName, aState.VendorTileType, aState.ExtraRenderingInfo, aState.aGraphic,
                                aState.aMatrix, aState.BitmapCache, aState.anImageAttribute, aState.bmpTransparentColor, tryCount);
        }

        private string GetUserAgent()
        {
            if (httpwebrequest_user_agent == null)
            {
                httpwebrequest_user_agent = Properties.Settings.Default.httpwebrequest_user_agent;
            }

            return httpwebrequest_user_agent;
        }

        /// <summary>
        /// Builds/Sends the Asynchronous GET request that will retrieve the tile for quadkey.
        /// </summary>
        /// <param name="quadkey"></param>
        /// <param name="vendorDatasetName"></param>
        /// <param name="vendorTileType"></param>
        /// <param name="extraRenderingInfo"></param>
        /// <param name="gr"></param>
        /// <param name="aTransform"></param>
        /// <param name="bitmapCache"></param>
        /// <param name="anImageAttributes"></param>
        /// <param name="bmpTransparentColor"></param>
        private void RequestQuadkeyTile(String quadkey,String vendorDatasetName, String vendorTileType, String extraRenderingInfo, Graphics gr, Matrix aTransform,
                                        Dictionary<string, Bitmap> bitmapCache, ImageAttributes anImageAttributes, Nullable<Color>  bmpTransparentColor, int tryCount)
        {
            if (vendorDatasetName == "mbtiles_dataset")
            {
                RequestState state = new RequestState(null, gr, aTransform, null, quadkey, bitmapCache, anImageAttributes, bmpTransparentColor,
                                                      vendorDatasetName, vendorTileType, extraRenderingInfo, tryCount);

                GetFromMBTile(state);

            }
            else
            {
                String aUri = ResolveURIforQuadkey(quadkey, vendorDatasetName, vendorTileType, extraRenderingInfo);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(aUri);
                request.UserAgent = GetUserAgent();

                RequestState state = new RequestState(request, gr, aTransform, aUri, quadkey, bitmapCache, anImageAttributes, bmpTransparentColor,
                                                      vendorDatasetName,vendorTileType,extraRenderingInfo,tryCount);

                IAsyncResult result = request.BeginGetResponse(new AsyncCallback(GetTileCallback), state);
                Trace(2, "REQUEST: " + aUri);

                //Register the timeout callback (10 second timeout)
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TileTimeoutCallback), state, (TileTimeoutMilliseconds), true);
            }
        }

        private string acpGetStringLarge()
        {
            int numberOfStringParts;
            string returnValue = "";
            string aVal;

            numberOfStringParts = this.acpGetInt();

            for (int i = 0; i < numberOfStringParts; i++)
            {
                aVal = this.acpGetString();
                writeError("aVal = '"+aVal+"'");
                returnValue = returnValue + aVal;
            }
            
            writeError("returnValue: " + returnValue);
            return returnValue;
        }

        /// <summary>
        /// Renders the specified set of quadkeys on a canvas in an efficient asynchronous fashion.
        /// </summary>
        private void RenderTilesOnCanvas()
        {
            DateTime startTime = DateTime.Now;

            // get the initial canvas file information

            // a PNG file that we use to get the canvas FROM smallworld
            string canvasFileName = this.acpGetString();

            // a BMP file that we use to send the canvas TO smallworld
            string bmpfname = this.acpGetString();

            // Vendor Name
            string vendorDatasetName = this.acpGetString();

            // Vendor Tile Type
            string vendorTileType = this.acpGetString();

            // Opacity
            float opacity = this.acpGetFloat();
            
            // Extra Rendering Info
            // Google Static Maps API allows large "extra_string".  We need
		    // to use put_chars16_large() for that because the ACP chokes on
			// large strings.  This still needs some more work so we leave
			// it commented out for now.
            //string extraRenderingInfo = this.acpGetStringLarge();
            string extraRenderingInfo = this.acpGetString();

            // get the number of tiles that we should render as a block before telling Magik that the next canvas is available.
            tileRenderingBlockSize = this.acpGetInt();

            Boolean isPlotting = (this.acpGetString() == "plotting");

            // get the canvas size
            int Width = this.acpGetInt();
            int Height = this.acpGetInt();

            // get the quadkey information
            int tileCount = this.acpGetInt();
            String[] quadkeys = new String[tileCount];
            Dictionary<String,Matrix> quadkeyTransforms = new Dictionary<String,Matrix>();

            string quadkey;
            
            for (int i = 0; i < tileCount; i++)
            {
                // get the Quadkey from Magik...
                quadkey = quadkeys[i] = this.acpGetString();
                quadkeyTransforms[quadkey] = this.acpGetTransform();
            }

            Bitmap canvasPhoto;
            Graphics gr;

            // transactionCode is what we use at the end of this method to log the transaction to the WMCLogger.
            string transactionType;
            Dictionary<string, Bitmap> bitmapCache = null;
            ImageAttributes anImageAttributes = null;
            Nullable<Color> bmpTransparentColor = null;
            Queue<String> quadkeyQueue = new Queue<string>();

            try
            {
                // validate the vendor dataset
                PrepareCanvasForVendor(vendorDatasetName, vendorTileType, out bitmapCache, out transactionType,out anImageAttributes,out bmpTransparentColor,extraRenderingInfo,opacity);

                // get the canvas image and render that as the first layer on the "gr" Graphics object.
                WebRequest WR = WebRequest.Create(canvasFileName);
                WebResponse returnValue = WR.GetResponse();
                System.IO.Stream receiveStream = returnValue.GetResponseStream();

                Bitmap bmpFromVendor = new Bitmap(receiveStream);

                returnValue.Close();
                receiveStream.Close();

                canvasPhoto = new Bitmap(Width, Height,System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                canvasPhoto.SetResolution(bmpFromVendor.HorizontalResolution,
                                        bmpFromVendor.VerticalResolution);

                gr = Graphics.FromImage(canvasPhoto);

                if (isPlotting)
                {
                    // if we are in plotting mode then the bmpFromVendor contains the color that should be used for the background.
                    gr.Clear(bmpFromVendor.GetPixel(1, 1));
                }
                else
                {
                    //gr.Clear(Color.Red);
                }
                
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic; // Bicubic seems to work best with anImageAttributes.SetWrapMode(WrapMode.TileFlipXY) to remove edge artifacts.

                gr.DrawImage(bmpFromVendor, new System.Drawing.Rectangle(0, 0, Width, Height),
                            new System.Drawing.Rectangle(0, 0, Width, Height),
                            GraphicsUnit.Pixel);

                // send all non-cached bitmap requests to the cloud...   
                for (int i = 0; i < tileCount; i++)
                {
                    quadkey = quadkeys[i];

                    quadkeyQueue.Enqueue(quadkey);

                    if (!bitmapCache.ContainsKey(quadkey))
                    {
                        RequestQuadkeyTile(quadkey, vendorDatasetName, vendorTileType, extraRenderingInfo, gr, quadkeyTransforms[quadkey], 
                                           bitmapCache, anImageAttributes, bmpTransparentColor,1);
                    }
                }

                this.acpPutTryResult();
            }
            catch (Exception e)
            {
                this.acpPutTryResult(e);
                Trace(2, "ERROR: " + e.Message);
                Trace(2, e.StackTrace);
                throw e;
            }


            // now we continuously loop waiting for requests from Magik.  If Magik is ready for another
            string magikRequest;

            int filenameCounter = 0;
            string newBmpFname;
            Boolean renderComplete = false;

            tileRenderingCount = 0;

            do
            {
                magikRequest = this.acpGetString();

                Trace(10,"Magik says: " + magikRequest);

                if (magikRequest == "next")
                {
                    int quadkeyQueueSize = quadkeyQueue.Count;

                    if (quadkeyQueueSize == 0)
                    {
                        // if we have no more quadkeys to process, then we can tell Magik that the rendering is "done"...
                        //writeError("DONE??");
                        this.acpPutString("done");
                        this.acpFlush();
                        renderComplete = true;
                        break;
                    }

                    // loop over each quadkey in the queue once.
                    bool anythingRendered = false;
                    for (int i = 0; i < quadkeyQueueSize; i++)
                    {
                        quadkey = quadkeyQueue.Dequeue();

                        //writeError("QUADKEY: " + quadkey + "containsKey? " + bitmapCache.ContainsKey(quadkey));

                        if (bitmapCache.ContainsKey(quadkey))
                        {
                            tileRenderingCount += 1;

                            if (bitmapCache[quadkey] == null)
                            {
                                // then we know this quadkey returned with an error so there
                                // is no need to render it.  Because we know this, we don't
                                // need to return the quadkey to the quadkeyQueue.
                                Trace(2, "FOUND NULL IN CACHE: " + quadkey);
                            }
                            else
                            {
                                // here we know that the quadkey found an actual bitmap so
                                // we can try rendering it now.
                                Trace(2, "RENDERING!!!! FOUND BMP IN CACHE "+ vendorDatasetName+"/"+vendorTileType + "/" +extraRenderingInfo+ ": " + quadkey);
                                RenderTileOnCanvas(gr, bitmapCache[quadkey], quadkeyTransforms[quadkey], anImageAttributes, bmpTransparentColor,quadkey);

                                anythingRendered = true;

                                // because we rendered successfully, we also don't need to return this
                                // quadkey to the queue.

                                // break out of this loop so that we can tell Magik about the newly-rendered tile.
                                break;
                            }

                        }
                        else
                        {
                            // if the quadkey was not in the cache yet, then put the quadkey
                            // back at the end of the queue so that we can try again on the 
                            // next request from Magik.
                            quadkeyQueue.Enqueue(quadkey);
                        }
                    }

                    if ((anythingRendered) & ((tileRenderingCount >= tileRenderingBlockSize) | (quadkeyQueue.Count == 0)))
                    {
                        // always send a unique filename so that we don't run into
                        // resource conflicts in Magik.  It will be up to Magik to 
                        // unlink this file.
                        filenameCounter += 1;
                        newBmpFname = bmpfname + filenameCounter;

                        // before we save the file, add any necessary attribution/logos
                        AddAttribution(gr,vendorDatasetName,vendorTileType);

                        if (!isPlotting)
                        {
                            // now save the file
                            canvasPhoto.Save(newBmpFname, System.Drawing.Imaging.ImageFormat.Bmp);
                        }

                        this.acpPutString("available");
                        this.acpPutString(newBmpFname);
                        this.acpFlush();

                        if (isPlotting)
                        {
                            // if we are in plotting mode, then we need to listen for some more information
                            // from Magik about how to carve up the canvasPhoto into chunks that Magik class
                            // simple_grid can handle.
                            int numberOfClips = this.acpGetInt();
                            int clipWidth;
                            int clipHeight;
                            int clip_x_start;
                            int clip_y_start;
                            
                            for (int idx = 0; idx < numberOfClips; idx++)
                            {
                                clip_x_start = this.acpGetInt();
                                clip_y_start = this.acpGetInt();
                                clipWidth = this.acpGetInt();
                                clipHeight = this.acpGetInt();

                                Trace(2,"clip_start ("+clip_x_start+","+clip_y_start+")");

                                Bitmap clipBitmap = new Bitmap(clipWidth, clipHeight);
                                Graphics clipGraphics = Graphics.FromImage(clipBitmap);

                                System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(clip_x_start, clip_y_start, clipWidth, clipHeight);

                                clipGraphics.DrawImage(canvasPhoto, 0, 0, srcRect, GraphicsUnit.Pixel);

                                clipBitmap.Save(newBmpFname, System.Drawing.Imaging.ImageFormat.Bmp);

                                this.acpPutString(newBmpFname);

                                clipGraphics.Dispose();
                                clipBitmap.Dispose();
                            }
                        }

                        // get the number of tiles that we should render as a block before telling Magik that the next canvas is available.
                        tileRenderingBlockSize = this.acpGetInt();

                        tileRenderingCount = 0;
                    }
                    else
                    {
                        // we looped through ALL the quadkeys in the queue one time and none
                        // were rendered, so we need to tell the Magik code to wait and try again 
                        // a bit later.

                        this.acpPutString("wait");
                        this.acpFlush();
                    }


                }
                else if (magikRequest == "quit")
                {
                    // this "quit" request will come if the Magik side of things was interrupted.
                    // we want to be able to exit gracefully without killing the ACP executable.
                    // "quit" means that Magik is not expecting any further ACP interaction.
                    break;
                }

            } while (true);

            anImageAttributes.Dispose();

            if (renderComplete)
            {
                // to prevent the "divided by 0" error
                if (tileCount == 0)
                {
                    tileCount = 1;
                }
            }
        }

        private void AddAttribution(Graphics gr, string vendorDatasetName, string vendorTileType)
        {
            gr.ResetTransform();

            String attribution;
            System.Drawing.Point insertPoint;
            System.Drawing.Point? insertBitmapPoint = null;
            Font aFont;
            StringAlignment aStringAlignment;
            Brush fillBrush;
            string bitmapCacheKey = "";
            string bitmapURL = "";
            Boolean isLogoInResources = false;

            // one of "text", "bitmap", "both"
            String attributionType;

            switch (vendorDatasetName)
            {
                case "osm_dataset":
                    attribution = "\u00A9 OpenStreetMap.org and contributors, CC-BY-SA";
                    insertPoint = new System.Drawing.Point(((int)gr.VisibleClipBounds.Width) - 2, ((int)gr.VisibleClipBounds.Height) - 14);
                    aFont = new Font("Arial", 8);
                    aStringAlignment = StringAlignment.Far;
                    fillBrush = Brushes.Black;
                    attributionType = "text";
                    break;
                case "mapquest_dataset":

                    attribution = "Tiles Courtesy of MapQuest (http://www.mapquest.com)\n";
                    switch (vendorTileType)
                    {
                        case "tile":
                            attribution += "\u00A9 Mapquest - Map data \u00A9 OpenStreetMap.org and contributors, CC-BY-SA";
                            break;
                        case "aerial_tile":
                            attribution += "Portions Courtesy NASA/JPL-Caltech and U.S. Depart. of Agriculture, Farm Service Agency";
                            break;
                    }


                    insertPoint = new System.Drawing.Point(((int)gr.VisibleClipBounds.Width) - 2, ((int)gr.VisibleClipBounds.Height) - 28);
                    aFont = new Font("Arial", 8);
                    aStringAlignment = StringAlignment.Far;
                    fillBrush = Brushes.Black;


                    bitmapCacheKey = "mapquest_logo";
                    bitmapURL = "http://open.mapquest.co.uk/cdn/toolkit/lite/images/questy.png";
                    insertBitmapPoint = new System.Drawing.Point(2, ((int)gr.VisibleClipBounds.Height) - 40);

                    attributionType = "both";
                    break;

                case "bing_dataset":
                case "birdseye_dataset":
                    attribution = "\u00A9 Microsoft Corporation";
                    insertPoint = new System.Drawing.Point(2, ((int)gr.VisibleClipBounds.Height) - 14);
                    aFont = new Font("Arial", 8);
                    aStringAlignment = StringAlignment.Near;
                    fillBrush = Brushes.Black;

                    bitmapCacheKey = "poweredByBing";
                    bitmapURL = "bing_logo";
                    isLogoInResources = true;

                    insertBitmapPoint = new System.Drawing.Point(2, ((int)gr.VisibleClipBounds.Height) - 31);

                    attributionType = "bitmap";
                    break;

                default:
                    return;
            }

            Trace(2, "bitmapCacheKey: " + bitmapCacheKey);
            Trace(2, "bitmapURL: " + bitmapURL);
            Trace(2, "attributionType: " + attributionType);

            if ((attributionType == "text") | (attributionType == "both"))
            {
                StringFormat aStringFormat = new StringFormat();
                aStringFormat.Alignment = aStringAlignment;

                gr.DrawString(attribution, aFont, fillBrush, insertPoint, aStringFormat);
            }

            if ((attributionType == "bitmap") | (attributionType == "both"))
            {
                Bitmap logo = null;
                if (!attributionBitmapCache.ContainsKey(bitmapCacheKey))
                {
                    if (isLogoInResources)
                    {
                        Trace(3, "getting logo '" + bitmapURL + "' from Resources");
                        logo = (Bitmap)Properties.Resources.ResourceManager.GetObject(bitmapURL);
                    }
                    else
                    {
                        Trace(3, "getting logo '" + bitmapURL + "' from HTTP request");
                        WebRequest request = (HttpWebRequest)WebRequest.Create(bitmapURL);
                        WebResponse response = request.GetResponse();
                        Stream s = response.GetResponseStream();
                        logo = new Bitmap(s);
                    }

                    attributionBitmapCache[bitmapCacheKey] = logo;
                }
                else
                {
                    logo = attributionBitmapCache[bitmapCacheKey];
                }

                int Width = logo.Width;
                int Height = logo.Height;

                gr.DrawImage(logo, (System.Drawing.Point)insertBitmapPoint);
            }
        }

        private int id = 0;
        private int getID() {
            return id += 1;
        }

        private void TileTimeoutCallback(object state, bool timedOut)
        {
            String id = getID().ToString();

            RequestState aState = (RequestState)state;

            Trace(2, id + " timeout callback: " + aState.SiteUrl);
            Trace(2, id + " timedout : " + timedOut);

            if (timedOut)
            {
                aState.Request.Abort();

                if (aState.tryCount > 2)
                {
                    Trace(2, id + " Attempt " + aState.tryCount + " failed.  Caching " + aState.Quadkey + " as NULL");
                    aState.BitmapCache[aState.Quadkey] = null;
                }
                else
                {
                    Trace(2, id + " Attempt " + aState.tryCount + " failed.  Trying again.");
                    RequestQuadkeyTile(aState, aState.tryCount + 1);
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// Accesses the MBTiles file for tile information
        /// </summary>
        /// <param name="result"></param>
        private void GetFromMBTile(RequestState aState)
        {
            int zl = 0;
            int x = 0;
            int y = 0;

            TileSystem.QuadKeyToTMSTileXY(aState.Quadkey, out x, out y, out zl);


            string exe = @"sqlite3.exe";
            string db = aState.ExtraRenderingInfo;
            string sql = string.Format("select hex(tile_data) from tiles where zoom_level={0} and tile_column={1} and tile_row={2}", zl, x, y);

            Process p = new Process();
            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = db + " " + '"' + sql + '"';
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            try
            {
                // cache the tile so that the rendering thread can draw this on the graphics canvas.
                SoapHexBinary shb = SoapHexBinary.Parse(output);
                byte[] a = shb.Value;

                if (a.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(a, 0, a.Length);
                    ms.Write(a, 0, a.Length);
                    Bitmap bmpTile = new Bitmap(ms);

                    aState.BitmapCache[aState.Quadkey] = bmpTile;
                }
                else
                {
                    // this mbtile has no tile at this quadkey so we tell the cache there is nothing here.
                    aState.BitmapCache[aState.Quadkey] = null;
                }
            }
            catch (Exception e)
            {
                writeError("Exception: " + e);
                sql = string.Format("select * from tiles where zoom_level={0} and tile_column={1} and tile_row={2}", zl, x, y);
                writeError(sql);
                p.StartInfo.Arguments = db + " " + '"' + sql + '"';
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                writeError("OUTPUT:");
                writeError(output);
                throw e;
            }
        }

        /// <summary>
        /// handles the GET request callback by reading the bitmap from the response stream and
        /// then drawing it on the graphic with the appropriate Matrix transform.
        /// </summary>
        private void GetTileCallback(IAsyncResult result)
        {
            String id = getID().ToString();

            RequestState aState = (RequestState)result.AsyncState;
            WebRequest request = (WebRequest)aState.Request;

            Trace(2, "----" + id + "-------------------------");
            Trace(2, id + " GetTileCallback(): " + aState.SiteUrl);

            HttpWebResponse returnValue;
            
            try
            {
                returnValue = (HttpWebResponse)request.EndGetResponse(result);
                Trace(2, id + " returnValue: " + returnValue.ToString());
                Trace(2, id + " StatusCode: " + returnValue.StatusCode);
                
                foreach (String header in returnValue.Headers)
                {
                    Trace(2, id + "    " + header);
                }


            }
            catch(WebException WebEx)
            {
                WebExceptionStatus status = WebEx.Status;

                Trace(2, id + " WebException!!!: " + status.ToString());

                if (status == WebExceptionStatus.ProtocolError)
                {
                    // Get HttpWebResponse so that you can check the HTTP status code.
                    HttpWebResponse httpResponse = (HttpWebResponse)WebEx.Response;
                    int StatusCode = (int)httpResponse.StatusCode;
                    Trace(2, id + " The server returned protocol error: " + StatusCode + " - " + httpResponse.StatusCode);

                    if (TraceLevel >= 3)
                    {
                        WebResponse resp = WebEx.Response;
                        using(StreamReader sr = new StreamReader(resp.GetResponseStream()))
                        {
                            Trace(3,sr.ReadToEnd());
                        }
                    }

                    if (StatusCode == 404)
                    {
                        // if file not found, that means that we can put a NULL in the cache so we don't try
                        // to ask for this tile again.
                        aState.BitmapCache[aState.Quadkey] = null;
                        Trace(2, id + " caching the tile as NULL");
                    }
                    else
                    {
                        // otherwise it would be nice to retry the request
                        // for the quadkey one more time.
                        if (aState.tryCount > 2)
                        {
                            Trace(2, id + " Attempt " + aState.tryCount + " failed.  Caching " + aState.Quadkey + " as NULL");
                            aState.BitmapCache[aState.Quadkey] = null;
                        }
                        else
                        {
                            Trace(2, id + " Attempt " + aState.tryCount + " failed.  Trying again.");
                            RequestQuadkeyTile(aState, aState.tryCount + 1);
                        }
                    }

                }

                Trace(2, WebEx.StackTrace);
                return;
            }

            System.IO.Stream receiveStream = returnValue.GetResponseStream();

            Bitmap bmpTile = new Bitmap(receiveStream);

            receiveStream.Close();
            returnValue.Close();

            Trace(2, id + " RETURN : " + request.RequestUri);
            
            // cache the tile so that the rendering thread can draw this on the graphics canvas.

            //writeError("!!!! Caching Tile " + aState.Quadkey + "...." + bmpTile.ToString());
            aState.BitmapCache[aState.Quadkey] = bmpTile;

        }

        /// <summary>
        /// Render bmpTile on gr using aTransform
        /// </summary>
        private void RenderTileOnCanvas(Graphics gr, Bitmap bmpTile, Matrix aTransform, ImageAttributes anImageAttribute, Nullable<Color> bmpTransparentColor, String quadkey)
        {
            lock (renderingLock)
            {


                if (bmpTransparentColor != null)
                {
                    bmpTile.MakeTransparent((Color)bmpTransparentColor);
                }

                int Width = bmpTile.Width;
                int Height = bmpTile.Height;

                System.Drawing.Point center=new System.Drawing.Point();

                gr.ResetTransform();
                
                gr.MultiplyTransform(aTransform);

                gr.DrawImage(bmpTile, new System.Drawing.Rectangle(0, 0, Width, Height),
                    0,0,Width, Height,GraphicsUnit.Pixel,anImageAttribute);


                if (TraceLevel >= 1000)
                {
                    Pen p = new Pen(Color.Gray, 1.0f);
                    System.Drawing.Rectangle outline = new System.Drawing.Rectangle(0, 0, 256, 256);
                    gr.DrawRectangle(p,outline);
                    p.Dispose();

                    Pen p2 = new Pen(Color.Red, 1.0f);
                    GraphicsPath textPath = new GraphicsPath();

                    StringFormat aStringFormat = new StringFormat();
                    aStringFormat.Alignment = StringAlignment.Center;

                    center = new System.Drawing.Point(Width / (int)2, Height / (int)2);
                    textPath.AddString(quadkey, new FontFamily("Arial"), (int)FontStyle.Bold, 15, center, aStringFormat);

                    gr.FillPath(Brushes.Wheat, textPath);
                    gr.DrawPath(p2, textPath);

                    p2.Dispose();
                    textPath.Dispose();
                }


            }
        }

        
        private void SetUserData()
        {
            string dataKey = this.acpGetString();

            string dataValue = this.acpGetString();

            if (dataValue == EMPTY_STRING)
            {
                dataValue = "";
            }

            SetUserData(dataKey, dataValue);
        }

        /// <summary>
        /// Saves dataValue into a file called dataKey in the application's IsolatedStorage.
        /// </summary>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        private void SetUserData(string dataKey,string dataValue)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(dataKey, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.Write(dataValue);
                        sw.Close();
                    }
                }
            }
        }


        private void GetUserData()
        {
            string dataKey = this.acpGetString();

            string userData = GetUserData(dataKey);

            if (userData == null)
            {
                userData = "";
            }

            if (userData == "")
            {
                userData = EMPTY_STRING;
            }

            this.acpPutString(userData);
        }

        /// <summary>
        /// retrives the string stored in the IsolatedStorage file 'dataKey'.
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        private string GetUserData(string dataKey)
        {
            string data = String.Empty;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                try
                {

                    using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(dataKey, FileMode.Open, isf))
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            string lineOfData = String.Empty;
                            while ((lineOfData = sr.ReadLine()) != null)
                                data += lineOfData;
                        }
                    }
                }
                catch (Exception e)
                {
                    // do nothing because the file doesn't exist.
                    Trace(2, "ERROR: " + e.Message);
                    Trace(2, e.StackTrace);
                }

            }
            return data;
        }

        /// <summary>
        /// put this as the last line at the end of a TRY block before the catch.
        /// </summary>
        private void acpPutTryResult()
        {
            this.acpPutBool(true);
        }

        /// <summary>
        /// put this inside the CATCH block.
        /// </summary>
        private void acpPutTryResult(Exception e)
        {
            this.acpPutBool(false);
            this.acpPutString(e.Message);
            this.acpPutString(e.StackTrace);
        }

        /// <summary>
        /// Returns a Matrix based on the affine Transform received from Magik.
        /// </summary>
        /// <returns></returns>
        private Matrix acpGetTransform()
        {
            return new Matrix(this.acpGetFloat(), this.acpGetFloat(), this.acpGetFloat(), this.acpGetFloat(), this.acpGetFloat(), this.acpGetFloat());
        }

        private string SignForGoogleAPI_if_appropriate(string url)
        {
            GoogleSignedKey = Properties.Settings.Default.GoogleSigningKey;
            GoogleClientIDString = Properties.Settings.Default.GoogleClientID;

            Trace(4, "GOOGLE API Signing '" + url + "'");
            return SignForGoogleAPI(url, GoogleSignedKey);
        }

        private string SignForGoogleAPI(string url, string keyString)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();

            // converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
            string usablePrivateKey = keyString.Replace("-", "+").Replace("_", "/");
            byte[] privateKeyBytes = Convert.FromBase64String(usablePrivateKey);

            Uri uri = new Uri(url + "&client=" + GoogleClientIDString + "&channel=" + customerID);
            byte[] encodedPathAndQueryBytes = encoding.GetBytes(uri.LocalPath + uri.Query);

            // compute the hash
            HMACSHA1 algorithm = new HMACSHA1(privateKeyBytes);
            byte[] hash = algorithm.ComputeHash(encodedPathAndQueryBytes);

            // convert the bytes to string and make url-safe by replacing '+' and '/' characters
            string signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

            // Add the signature to the existing URI.
            return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
        }
    }
}
