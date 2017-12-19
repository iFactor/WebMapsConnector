/*
 * Copyright(c) 2017 Microsoft Corporation. All rights reserved. 
 * 
 * This code is licensed under the MIT License (MIT). 
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE. 
*/

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BingMapsRESTToolkit
{
    /*
     * Data Contracts generated for the Bing Maps REST service Response object.
     */

    public class Address
    {
        [JsonProperty(PropertyName = "addressLine")]
        public string AddressLine { get; set; }

        [JsonProperty(PropertyName = "adminDistrict")]
        public string AdminDistrict { get; set; }

        [JsonProperty(PropertyName = "adminDistrict2")]
        public string AdminDistrict2 { get; set; }

        [JsonProperty(PropertyName = "countryRegion")]
        public string CountryRegion { get; set; }

        [JsonProperty(PropertyName = "locality")]
        public string Locality { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "countryRegionIso2")]
        public string CountryRegionIso2 { get; set; }

        [JsonProperty(PropertyName = "formattedAddress")]
        public string FormattedAddress { get; set; }

        [JsonProperty(PropertyName = "neighborhood")]
        public string Neighborhood { get; set; }

        [JsonProperty(PropertyName = "landmark")]
        public string Landmark { get; set; }
    }

    public class CoverageArea
    {
        /// <summary>
        /// Bounding box of the coverage area. Structure [South Latitude, West Longitude, North Latitude, East Longitude]
        /// </summary>
        [JsonProperty(PropertyName = "bbox")]
        public double[] BoundingBox { get; set; }

        [JsonProperty(PropertyName = "zoomMax")]
        public int ZoomMax { get; set; }

        [JsonProperty(PropertyName = "zoomMin")]
        public int ZoomMin { get; set; }
    }

    public class BirdseyeMetadata : ImageryMetadata
    {
        [JsonProperty(PropertyName = "orientation")]
        public double Orientation { get; set; }

        [JsonProperty(PropertyName = "tilesX")]
        public int TilesX { get; set; }

        [JsonProperty(PropertyName = "tilesY")]
        public int TilesY { get; set; }
    }

    public class Detail
    {
        [JsonProperty(PropertyName = "compassDegrees")]
        public int CompassDegrees { get; set; }

        [JsonProperty(PropertyName = "maneuverType")]
        public string ManeuverType { get; set; }

        [JsonProperty(PropertyName = "startPathIndices")]
        public int[] StartPathIndices { get; set; }

        [JsonProperty(PropertyName = "endPathIndices")]
        public int[] EndPathIndices { get; set; }

        [JsonProperty(PropertyName = "roadType")]
        public string RoadType { get; set; }

        [JsonProperty(PropertyName = "locationCodes")]
        public string[] LocationCodes { get; set; }

        [JsonProperty(PropertyName = "names")]
        public string[] Names { get; set; }

        [JsonProperty(PropertyName = "mode")]
        public string Mode { get; set; }

        [JsonProperty(PropertyName = "roadShieldRequestParameters")]
        public RoadShield roadShieldRequestParameters { get; set; }
    }

    public class Generalization
    {
        [JsonProperty(PropertyName = "pathIndices")]
        public int[] PathIndices { get; set; }

        [JsonProperty(PropertyName = "latLongTolerance")]
        public double LatLongTolerance { get; set; }
    }

    public class Hint
    {
        [JsonProperty(PropertyName = "hintType")]
        public string HintType { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }

    [KnownType(typeof(StaticMapMetadata))]
    [KnownType(typeof(BirdseyeMetadata))]
    public class ImageryMetadata : Resource
    {
        [JsonProperty(PropertyName = "imageHeight")]
        public int ImageHeight { get; set; }

        [JsonProperty(PropertyName = "imageWidth")]
        public int ImageWidth { get; set; }

        [JsonProperty(PropertyName = "imageryProviders")]
        public ImageryProvider[] ImageryProviders { get; set; }

        [JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "imageUrlSubdomains")]
        public string[] ImageUrlSubdomains { get; set; }

        [JsonProperty(PropertyName = "vintageEnd")]
        public string VintageEnd { get; set; }

        [JsonProperty(PropertyName = "vintageStart")]
        public string VintageStart { get; set; }

        [JsonProperty(PropertyName = "zoomMax")]
        public int ZoomMax { get; set; }

        [JsonProperty(PropertyName = "zoomMin")]
        public int ZoomMin { get; set; }
    }

    public class ImageryProvider
    {
        [JsonProperty(PropertyName = "attribution")]
        public string Attribution { get; set; }

        [JsonProperty(PropertyName = "coverageAreas")]
        public CoverageArea[] CoverageAreas { get; set; }
    }

    public class Instruction
    {
        [JsonProperty(PropertyName = "maneuverType")]
        public string ManeuverType { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "formattedText")]
        public string FormattedText { get; set; }
    }

    public class ItineraryItem
    {
        [JsonProperty(PropertyName = "childItineraryItems")]
        public ItineraryItem[] ChildItineraryItems { get; set; }

        [JsonProperty(PropertyName = "compassDirection")]
        public string CompassDirection { get; set; }

        [JsonProperty(PropertyName = "details")]
        public Detail[] Details { get; set; }

        [JsonProperty(PropertyName = "exit")]
        public string Exit { get; set; }

        [JsonProperty(PropertyName = "hints")]
        public Hint[] Hints { get; set; }

        [JsonProperty(PropertyName = "iconType")]
        public string IconType { get; set; }

        [JsonProperty(PropertyName = "instruction")]
        public Instruction Instruction { get; set; }

        [JsonProperty(PropertyName = "maneuverPoint")]
        public Point ManeuverPoint { get; set; }

        [JsonProperty(PropertyName = "sideOfStreet")]
        public string SideOfStreet { get; set; }

        [JsonProperty(PropertyName = "signs")]
        public string[] Signs { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }

        public DateTime TimeUtc
        {
            get
            {
                if (string.IsNullOrEmpty(Time))
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTimeHelper.FromOdataJson(Time);
                }
            }
            set
            {
                if (value == null)
                {
                    Time = string.Empty;
                }
                else
                {
                    var v = DateTimeHelper.ToOdataJson(value);

                    if (v != null)
                    {
                        Time = v;
                    }
                    else
                    {
                        Time = string.Empty;
                    }
                }
            }
        }

        [JsonProperty(PropertyName = "tollZone")]
        public string TollZone { get; set; }

        [JsonProperty(PropertyName = "towardsRoadName")]
        public string TowardsRoadName { get; set; }

        [JsonProperty(PropertyName = "transitLine")]
        public TransitLine TransitLine { get; set; }

        [JsonProperty(PropertyName = "transitStopId")]
        public int TransitStopId { get; set; }

        [JsonProperty(PropertyName = "transitTerminus")]
        public string TransitTerminus { get; set; }

        [JsonProperty(PropertyName = "travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty(PropertyName = "travelDuration")]
        public double TravelDuration { get; set; }

        [JsonProperty(PropertyName = "travelMode")]
        public string TravelMode { get; set; }

        [JsonProperty(PropertyName = "warning")]
        public Warning[] Warning { get; set; }
    }

    public class Line
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "coordinates")]
        public double[][] Coordinates { get; set; }
    }

    public class Location
    {
        /// <summary>
        /// Bounding box of the response. Structure [South Latitude, West Longitude, North Latitude, East Longitude]
        /// </summary>
        [JsonProperty(PropertyName = "bbox")]
        public double[] BoundingBox { get; set; }

        [JsonProperty(PropertyName = "__type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "point")]
        public Point Point { get; set; }

        [JsonProperty(PropertyName = "entityType")]
        public string EntityType { get; set; }

        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }

        [JsonProperty(PropertyName = "confidence")]
        public string Confidence { get; set; }

        [JsonProperty(PropertyName = "matchCodes")]
        public string[] MatchCodes { get; set; }

        [JsonProperty(PropertyName = "geocodePoints")]
        public Point[] GeocodePoints { get; set; }

        [JsonProperty(PropertyName = "queryParseValues")]
        public QueryParseValue[] QueryParseValues { get; set; }
    }

    public class QueryParseValue
    {
        [JsonProperty(PropertyName = "property")]
        public string Property { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class PushpinMetdata
    {
        [JsonProperty(PropertyName = "anchor")]
        public Pixel Anchor { get; set; }

        [JsonProperty(PropertyName = "bottomRightOffset")]
        public Pixel BottomRightOffset { get; set; }

        [JsonProperty(PropertyName = "topLeftOffset")]
        public Pixel TopLeftOffset { get; set; }

        [JsonProperty(PropertyName = "point")]
        public Point Point { get; set; }
    }

    public class Pixel
    {
        [JsonProperty(PropertyName = "x")]
        public string X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public string Y { get; set; }
    }

    public class Point : Shape
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Latitude,Longitude
        /// </summary>
        [JsonProperty(PropertyName = "coordinates")]
        public double[] Coordinates { get; set; }

        [JsonProperty(PropertyName = "calculationMethod")]
        public string CalculationMethod { get; set; }

        [JsonProperty(PropertyName = "usageTypes")]
        public string[] UsageTypes { get; set; }

        public Coordinate GetCoordinate()
        {
            if (Coordinates.Length >= 2) {
                return new Coordinate(Coordinates[0], Coordinates[1]);
            }

            return null;
        }
    }

    [KnownType(typeof(Route))]
    [KnownType(typeof(TrafficIncident))]
    [KnownType(typeof(ImageryMetadata))]
    [KnownType(typeof(ElevationData))]
    [KnownType(typeof(SeaLevelData))]
    [KnownType(typeof(CompressedPointList))]
    [KnownType(typeof(GeospatialEndpointResponse))]
    public class Resource
    {
        /// <summary>
        /// Bounding box of the response. Structure [South Latitude, West Longitude, North Latitude, East Longitude]
        /// </summary>
        [JsonProperty(PropertyName = "bbox")]
        public double[] BoundingBox { get; set; }

        [JsonProperty(PropertyName = "__type")]
        public string Type { get; set; }
    }

    public class ResourceSet
    {
        [JsonProperty(PropertyName = "estimatedTotal")]
        public long EstimatedTotal { get; set; }

        [JsonProperty(PropertyName = "resources")]
        public Location[] Resources { get; set; }
    }

    public class Response
    {
        [JsonProperty(PropertyName = "copyright")]
        public string Copyright { get; set; }

        [JsonProperty(PropertyName = "brandLogoUri")]
        public string BrandLogoUri { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "statusDescription")]
        public string StatusDescription { get; set; }

        [JsonProperty(PropertyName = "authenticationResultCode")]
        public string AuthenticationResultCode { get; set; }

        [JsonProperty(PropertyName = "errorDetails")]
        public string[] errorDetails { get; set; }

        [JsonProperty(PropertyName = "traceId")]
        public string TraceId { get; set; }

        [JsonProperty(PropertyName = "resourceSets")]
        public ResourceSet[] ResourceSets { get; set; }
    }

    public class RoadShield
    {
        [JsonProperty(PropertyName = "bucket")]
        public int Bucket { get; set; }

        [JsonProperty(PropertyName = "shields")]
        public Shield[] Shields { get; set; }
    }

    public class Route : Resource
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "distanceUnit")]
        public string DistanceUnit { get; set; }

        [JsonProperty(PropertyName = "durationUnit")]
        public string DurationUnit { get; set; }

        [JsonProperty(PropertyName = "travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty(PropertyName = "travelDuration")]
        public double TravelDuration { get; set; }

        [JsonProperty(PropertyName = "travelDurationTraffic")]
        public double TravelDurationTraffic { get; set; }

        [JsonProperty(PropertyName = "trafficCongestion")]
        public string TrafficCongestion { get; set; }

        [JsonProperty(PropertyName = "trafficDataUsed")]
        public string TrafficDataUsed { get; set; }

        [JsonProperty(PropertyName = "routeLegs")]
        public RouteLeg[] RouteLegs { get; set; }

        [JsonProperty(PropertyName = "routePath")]
        public RoutePath RoutePath { get; set; }
    }

    public class RouteLeg
    {
        [JsonProperty(PropertyName = "travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty(PropertyName = "travelDuration")]
        public double TravelDuration { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public double Cost { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "actualStart")]
        public Point ActualStart { get; set; }

        [JsonProperty(PropertyName = "actualEnd")]
        public Point ActualEnd { get; set; }

        [JsonProperty(PropertyName = "startLocation")]
        public Location StartLocation { get; set; }

        [JsonProperty(PropertyName = "endLocation")]
        public Location EndLocation { get; set; }

        [JsonProperty(PropertyName = "itineraryItems")]
        public ItineraryItem[] ItineraryItems { get; set; }

        [JsonProperty(PropertyName = "routeRegion")]
        public string RouteRegion { get; set; }

        [JsonProperty(PropertyName = "routeSubLegs")]
        public RouteSubLeg[] RouteSubLegs { get; set; }
        
        [JsonProperty(PropertyName = "startTime")]
        public string StartTime { get; set; }

        public DateTime StartTimeUtc
        {
            get
            {
                if (string.IsNullOrEmpty(StartTime))
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTimeHelper.FromOdataJson(StartTime);
                }
            }
            set
            {
                if (value == null)
                {
                    StartTime = string.Empty;
                }
                else
                {
                    var v = DateTimeHelper.ToOdataJson(value);

                    if (v != null)
                    {
                        StartTime = v;
                    }
                    else
                    {
                        StartTime = string.Empty;
                    }
                }
            }
        }

        [JsonProperty(PropertyName = "endTime")]
        public string EndTime { get; set; }

        public DateTime EndTimeUtc
        {
            get
            {
                if (string.IsNullOrEmpty(EndTime))
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTimeHelper.FromOdataJson(EndTime);
                }
            }
            set
            {
                if (value == null)
                {
                    EndTime = string.Empty;
                }
                else
                {
                    var v = DateTimeHelper.ToOdataJson(value);

                    if (v != null)
                    {
                        EndTime = v;
                    }
                    else
                    {
                        EndTime = string.Empty;
                    }
                }
            }
        }

        //TODO: What is the base class?
        [JsonProperty(PropertyName = "alternateVias")]
        public object[] AlternateVias { get; set; }
    }

    public class RouteSubLeg
    {
        [JsonProperty(PropertyName = "endWaypoint")]
        public Waypoint EndWaypoint { get; set; }

        [JsonProperty(PropertyName = "startWaypoint")]
        public Waypoint StartWaypoint { get; set; }

        [JsonProperty(PropertyName = "travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty(PropertyName = "travelDuration")]
        public double TravelDuration { get; set; }

        [JsonProperty(PropertyName = "travelDurationTraffic")]
        public double TravelDurationTraffic { get; set; }
    }

    public class Waypoint : Point
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isVia")]
        public bool IsVia { get; set; }

        [JsonProperty(PropertyName = "locationIdentifier")]
        public string LocationIdentifier { get; set; }

        [JsonProperty(PropertyName = "routePathIndex")]
        public int RoutePathIndex { get; set; }
    }

    public class RoutePath
    {
        [JsonProperty(PropertyName = "line")]
        public Line Line { get; set; }

        [JsonProperty(PropertyName = "generalizations")]
        public Generalization[] Generalizations { get; set; }
    }

    [KnownType(typeof(Point))]
    public class Shape
    {
        [JsonProperty(PropertyName = "boundingBox")]
        public double[] BoundingBox { get; set; }
    }

    public class Shield
    {
        [JsonProperty(PropertyName = "labels")]
        public string[] Labels { get; set; }

        [JsonProperty(PropertyName = "roadShieldType")]
        public int RoadShieldType { get; set; }
    }

    public class StaticMapMetadata : ImageryMetadata
    {
        [JsonProperty(PropertyName = "mapCenter")]
        public Point MapCenter { get; set; }

        [JsonProperty(PropertyName = "pushpins")]
        public PushpinMetdata[] Pushpins { get; set; }

        [JsonProperty(PropertyName = "zoom")]
        public string Zoom { get; set; }
    }

    public class TrafficIncident : Resource
    {
        [JsonProperty(PropertyName = "point")]
        public Point Point { get; set; }

        [JsonProperty(PropertyName = "congestion")]
        public string Congestion { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "detour")]
        public string Detour { get; set; }

        [JsonProperty(PropertyName = "start")]
        public string Start { get; set; }

        public DateTime StartDateTimeUtc
        {
            get
            {
                if (string.IsNullOrEmpty(Start))
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTimeHelper.FromOdataJson(Start);
                }
            }
            set
            {
                if (value == null)
                {
                    Start = string.Empty;
                }
                else
                {
                    var v = DateTimeHelper.ToOdataJson(value);

                    if (v != null)
                    {
                        Start = v;
                    }
                    else
                    {
                        Start = string.Empty;
                    }
                }
            }
        }

        [JsonProperty(PropertyName = "end")]
        public string End { get; set; }

        public DateTime EndDateTimeUtc
        {
            get
            {
                if (string.IsNullOrEmpty(End))
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTimeHelper.FromOdataJson(End);
                }
            }
            set
            {
                if (value == null)
                {
                    End = string.Empty;
                }
                else
                {
                    var v = DateTimeHelper.ToOdataJson(value);

                    if (v != null)
                    {
                        End = v;
                    }
                    else
                    {
                        End = string.Empty;
                    }
                }
            }
        }

        [JsonProperty(PropertyName = "incidentId")]
        public long IncidentId { get; set; }

        [JsonProperty(PropertyName = "lane")]
        public string Lane { get; set; }

        [JsonProperty(PropertyName = "lastModified")]
        public string LastModified { get; set; }

        public DateTime LastModifiedDateTimeUtc
        {
            get
            {
                if (string.IsNullOrEmpty(LastModified))
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTimeHelper.FromOdataJson(LastModified);
                }
            }
            set
            {
                if (value == null)
                {
                    LastModified = string.Empty;
                }
                else
                {
                    var v = DateTimeHelper.ToOdataJson(value);

                    if (v != null)
                    {
                        LastModified = v;
                    }
                    else
                    {
                        LastModified = string.Empty;
                    }
                }
            }
        }

        [JsonProperty(PropertyName = "roadClosed")]
        public bool RoadClosed { get; set; }

        [JsonProperty(PropertyName = "severity")]
        public int Severity { get; set; }

        [JsonProperty(PropertyName = "toPoint")]
        public Point ToPoint { get; set; }

        [JsonProperty(PropertyName = "locationCodes")]
        public string[] LocationCodes { get; set; }

        [JsonProperty(PropertyName = "type")]
        public new int Type { get; set; }

        [JsonProperty(PropertyName = "verified")]
        public bool Verified { get; set; }
    }

    public class TransitLine
    {
        [JsonProperty(PropertyName = "verboseName")]
        public string VerboseName { get; set; }

        [JsonProperty(PropertyName = "abbreviatedName")]
        public string AbbreviatedName { get; set; }

        [JsonProperty(PropertyName = "agencyId")]
        public long AgencyId { get; set; }

        [JsonProperty(PropertyName = "agencyName")]
        public string AgencyName { get; set; }

        [JsonProperty(PropertyName = "lineColor")]
        public long LineColor { get; set; }

        [JsonProperty(PropertyName = "lineTextColor")]
        public long LineTextColor { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "providerInfo")]
        public string ProviderInfo { get; set; }
    }

    public class Warning
    {
        [JsonProperty(PropertyName = "origin")]
        public string Origin { get; set; }

        public Coordinate OriginCoordinate
        {
            get
            {
                if (string.IsNullOrEmpty(Origin))
                {
                    return null;
                }
                else
                {
                    var latLng = Origin.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    double lat, lon;

                    if(latLng.Length >= 2 && double.TryParse(latLng[0], out lat) && double.TryParse(latLng[1], out lon))
                    {
                        return new Coordinate(lat, lon);
                    }

                    return null;
                }
            }
            set
            {
                if (value == null)
                {
                    Origin = string.Empty;
                }
                else
                {
                    Origin = string.Format("{0},{1}", value.Latitude, value.Longitude);
                }
            }
        }

        [JsonProperty(PropertyName = "severity")]
        public string Severity { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }

        public Coordinate ToCoordinate
        {
            get
            {
                if (string.IsNullOrEmpty(To))
                {
                    return null;
                }
                else
                {
                    var latLng = To.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    double lat, lon;

                    if (latLng.Length >= 2 && double.TryParse(latLng[0], out lat) && double.TryParse(latLng[1], out lon))
                    {
                        return new Coordinate(lat, lon);
                    }

                    return null;
                }
            }
            set
            {
                if (value == null)
                {
                    To = string.Empty;
                }
                else
                {
                    To = string.Format("{0},{1}", value.Latitude, value.Longitude);
                }
            }
        }

        [JsonProperty(PropertyName = "warningType")]
        public string WarningType { get; set; }
    }

    public class CompressedPointList : Resource
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

    public class ElevationData : Resource
    {
        [JsonProperty(PropertyName = "elevations")]
        public int[] Elevations { get; set; }

        [JsonProperty(PropertyName = "zoomLevel")]
        public int ZoomLevel { get; set; }
    }

    public class SeaLevelData : Resource
    {
        [JsonProperty(PropertyName = "offsets")]
        public int[] Offsets { get; set; }

        [JsonProperty(PropertyName = "zoomLevel")]
        public int ZoomLevel { get; set; }
    }

    /// <summary>
    /// This response specifies:
    ///  - Whether this is a politically disputed area, such as an area claimed by more than one country.
    ///  - Whether services are available in the user’s region.
    ///  - A list of available geospatial services including endpoints and language support for each service.
    /// </summary>
    public class GeospatialEndpointResponse : Resource
    {
        /// <summary>
        /// Specifies if this area in the request is claimed by more than one country. 
        /// </summary>
        [JsonProperty(PropertyName = "isDisputedArea")]
        public bool IsDisputedArea { get; set; }

        /// <summary>
        /// Specifies if Geospatial Platform services are available in the country or region. Microsoft does not support services in embargoed areas.
        /// </summary>
        [JsonProperty(PropertyName = "isSupported")]
        public bool IsSupported { get; set; }

        /// <summary>
        /// The country or region that was used to determine service support. If you specified a User Location in 
        /// the request that is in a non-disputed country or region, this country or region is returned in the response.
        /// </summary>
        [JsonProperty(PropertyName = "ur")]
        public string UserRegion { get; set; }

        /// <summary>
        /// Information for each geospatial service that is available in the country or region and language specified in the request.
        /// </summary>
        [JsonProperty(PropertyName = "services")]
        public GeospatialService[] Services { get; set; }
    }

    /// <summary>
    /// Information for a geospatial service that is available in the country or region and language specified in the request.
    /// </summary>
    public class GeospatialService
    {
        /// <summary>
        /// The URL service endpoint to use in this region. Note that to use the service, you must typically add parameters specific to 
        /// the service. These parameters are not described in this documentation.
        /// </summary>
        [JsonProperty(PropertyName = "endpoint")]
        public string Endpoint { get; set; }

        /// <summary>
        /// Set to true if the service supports the language in the request for the region. If the language is supported, then the 
        /// service endpoint will return responses using this language. If it is not supported, then the service will use the fallback language.
        /// </summary>
        [JsonProperty(PropertyName = "fallbackLanguage")]
        public string FallbackLanguage { get; set; }

        /// <summary>
        /// Specifies the secondary or fallback language in this region or country. If the requested language is not supported 
        /// and a fallback language is not available, United States English (en-us) is used.
        /// </summary>
        [JsonProperty(PropertyName = "languageSupported")]
        public bool LanguageSupported { get; set; }

        /// <summary>
        /// An abbreviated name for the service.
        /// </summary>
        [JsonProperty(PropertyName = "serviceName")]
        public string ServiceName { get; set; }
    }
}