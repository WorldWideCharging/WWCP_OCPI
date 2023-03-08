﻿/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Text;
using System.Net.Security;
using System.Collections.Concurrent;
using System.Security.Authentication;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// A delegate for filtering remote parties.
    /// </summary>
    public delegate Boolean IncludeRemoteParty(RemoteParty RemoteParty);


    /// <summary>
    /// The Common API.
    /// </summary>
    public class CommonAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("io/OCPI/");


        public const               String    DefaultLogfileName        = "OCPICommonAPI.log";


        private readonly           URL       OurBaseURL;


        /// <summary>
        /// The command values store.
        /// </summary>
        public readonly ConcurrentDictionary<Command_Id, CommandValues> CommandValueStore = new ();

        #endregion

        #region Properties

        /// <summary>
        /// The URL to your API versions endpoint.
        /// </summary>
        [Mandatory]
        public URL                  OurVersionsURL        { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails      OurBusinessDetails         { get; }

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode          OurCountryCode             { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id             OurPartyId                 { get; }

        /// <summary>
        /// Our business role.
        /// </summary>
        [Mandatory]
        public Roles                OurRole                    { get; }


        public HTTPPath?            AdditionalURLPathPrefix    { get; }

        /// <summary>
        /// Whether to keep or delete EVSEs marked as "REMOVED".
        /// </summary>
        public Func<EVSE, Boolean>  KeepRemovedEVSEs           { get; }

        /// <summary>
        /// Allow anonymous access to locations as Open Data.
        /// </summary>
        public Boolean              LocationsAsOpenData        { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?             AllowDowngrades            { get; }


        public Boolean              Disable_RootServices       { get; }

        #endregion

        #region Events

        #region (protected internal) PostCredentialsRequest   (Request)

        /// <summary>
        /// An event sent whenever a post credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a post credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCredentialsRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnPostCredentialsRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) PostCredentialsResponse  (Response)

        /// <summary>
        /// An event sent whenever a post credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a post credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCredentialsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnPostCredentialsResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion


        #region (protected internal) PutCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a put credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a put credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPutCredentialsRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PutCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a put credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a put credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPutCredentialsResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) DeleteCredentialsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a delete credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCredentialsRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnDeleteCredentialsRequest?.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request);

        #endregion

        #region (protected internal) DeleteCredentialsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a delete credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCredentialsResponse(DateTime      Timestamp,
                                                          HTTPAPI       API,
                                                          OCPIRequest   Request,
                                                          OCPIResponse  Response)

            => OnDeleteCredentialsResponse?.WhenAll(Timestamp,
                                                    API ?? this,
                                                    Request,
                                                    Response);

        #endregion

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new CommonAPI.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurBusinessDetails"></param>
        /// <param name="OurCountryCode"></param>
        /// <param name="OurPartyId"></param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// <param name="Disable_RootServices">Whether to disable / and /versions HTTP services.</param>
        /// 
        /// <param name="HTTPHostname">The HTTP hostname for all URLs within this API.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServerPort">A TCP port to listen on.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServerName">The default HTTP servername, used whenever no HTTP Host-header has been given.</param>
        /// 
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// <param name="HTTPServiceName">The name of the HTTP service.</param>
        /// <param name="HTMLTemplate">An optional HTML template.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="ServerCertificateSelector">An optional delegate to select a SSL/TLS server certificate.</param>
        /// <param name="ClientCertificateValidator">An optional delegate to verify the SSL/TLS client certificate used for authentication.</param>
        /// <param name="ClientCertificateSelector">An optional delegate to select the SSL/TLS client certificate used for authentication.</param>
        /// <param name="AllowedTLSProtocols">The SSL/TLS protocol(s) allowed for this connection.</param>
        /// 
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder">An optional delegate to build a connection identification based on IP socket information.</param>
        /// <param name="ConnectionTimeout">The TCP client timeout for all incoming client connections in seconds (default: 30 sec).</param>
        /// <param name="MaxClientConnections">The maximum number of concurrent TCP client connections (default: 4096).</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        /// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The maintenance intervall.</param>
        /// 
        /// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        /// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        /// <param name="WardenCheckEvery">The warden intervall.</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable any logging.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        /// <param name="DNSClient">The DNS client of the API.</param>
        /// <param name="Autostart">Whether to start the API automatically.</param>
        public CommonAPI(URL                                   OurVersionsURL,
                         BusinessDetails                       OurBusinessDetails,
                         CountryCode                           OurCountryCode,
                         Party_Id                              OurPartyId,

                         HTTPPath?                             AdditionalURLPathPrefix            = null,
                         Func<EVSE, Boolean>?                  KeepRemovedEVSEs                   = null,
                         Boolean                               LocationsAsOpenData                = true,
                         Boolean?                              AllowDowngrades                    = null,
                         Boolean                               Disable_RootServices               = true,

                         HTTPHostname?                         HTTPHostname                       = null,
                         String?                               ExternalDNSName                    = null,
                         IPPort?                               HTTPServerPort                     = null,
                         HTTPPath?                             BasePath                           = null,
                         String?                               HTTPServerName                     = DefaultHTTPServerName,

                         HTTPPath?                             URLPathPrefix                      = null,
                         String?                               HTTPServiceName                    = DefaultHTTPServiceName,
                         String?                               HTMLTemplate                       = null,
                         JObject?                              APIVersionHashes                   = null,

                         ServerCertificateSelectorDelegate?    ServerCertificateSelector          = null,
                         RemoteCertificateValidationCallback?  ClientCertificateValidator         = null,
                         LocalCertificateSelectionCallback?    ClientCertificateSelector          = null,
                         SslProtocols?                         AllowedTLSProtocols                = null,
                         Boolean?                              ClientCertificateRequired          = null,
                         Boolean?                              CheckCertificateRevocation         = null,

                         String?                               ServerThreadName                   = null,
                         ThreadPriority?                       ServerThreadPriority               = null,
                         Boolean?                              ServerThreadIsBackground           = null,

                         ConnectionIdBuilder?                  ConnectionIdBuilder                = null,
                         TimeSpan?                             ConnectionTimeout                  = null,
                         UInt32?                               MaxClientConnections               = null,

                         Boolean?                              DisableMaintenanceTasks            = null,
                         TimeSpan?                             MaintenanceInitialDelay            = null,
                         TimeSpan?                             MaintenanceEvery                   = null,

                         Boolean?                              DisableWardenTasks                 = null,
                         TimeSpan?                             WardenInitialDelay                 = null,
                         TimeSpan?                             WardenCheckEvery                   = null,

                         Boolean?                              IsDevelopment                      = null,
                         IEnumerable<String>?                  DevelopmentServers                 = null,
                         Boolean?                              DisableLogging                     = null,
                         String?                               LoggingPath                        = null,
                         String?                               LogfileName                        = null,
                         LogfileCreatorDelegate?               LogfileCreator                     = null,
                         DNSClient?                            DNSClient                          = null,
                         Boolean                               Autostart                          = false)

            : base(HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort  ?? DefaultHTTPServerPort,
                   BasePath,
                   HTTPServerName  ?? DefaultHTTPServerName,

                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   HTMLTemplate,
                   APIVersionHashes,

                   ServerCertificateSelector,
                   ClientCertificateValidator,
                   ClientCertificateSelector,
                   AllowedTLSProtocols,
                   ClientCertificateRequired,
                   CheckCertificateRevocation,

                   ServerThreadName,
                   ServerThreadPriority,
                   ServerThreadIsBackground,

                   ConnectionIdBuilder,
                   ConnectionTimeout,
                   MaxClientConnections,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator,
                   DNSClient,
                   Autostart)

        {

            this.OurVersionsURL           = OurVersionsURL;
            this.OurBusinessDetails       = OurBusinessDetails;
            this.OurCountryCode           = OurCountryCode;
            this.OurPartyId               = OurPartyId;

            this.OurBaseURL               = URL.Parse(OurVersionsURL.ToString().Replace("/versions", ""));
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.KeepRemovedEVSEs         = KeepRemovedEVSEs ?? (evse => true);
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;
            this.Disable_RootServices     = Disable_RootServices;

            this.remoteParties            = new Dictionary<RemoteParty_Id, RemoteParty>();
            this.Locations                = new Dictionary<Location_Id,    Location>();
            this.tariffs                  = new Dictionary<Tariff_Id,      Tariff>();
            this.chargingSessions         = new Dictionary<Session_Id,     Session>();
            this.tokenStatus              = new Dictionary<Token_Id,       TokenStatus>();
            this.chargeDetailRecords      = new Dictionary<CDR_Id,         CDR>();

            if (!Disable_RootServices)
                RegisterURLTemplates();

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Create a new CommonAPI using the given HTTP server.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurBusinessDetails"></param>
        /// <param name="OurCountryCode"></param>
        /// <param name="OurPartyId"></param>
        /// 
        /// <param name="HTTPServer">A HTTP server.</param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// <param name="Disable_RootServices">Whether to disable / and /versions HTTP services.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// 
        /// <param name="URLPathPrefix">An optional URL path prefix, used when defining URL templates.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        /// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The maintenance intervall.</param>
        /// 
        /// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        /// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        /// <param name="WardenCheckEvery">The warden intervall.</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable the log file.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        /// <param name="Autostart">Whether to start the API automatically.</param>
        public CommonAPI(URL                      OurVersionsURL,
                         BusinessDetails          OurBusinessDetails,
                         CountryCode              OurCountryCode,
                         Party_Id                 OurPartyId,
                         Roles                    OurRole,

                         HTTPServer               HTTPServer,

                         HTTPPath?                AdditionalURLPathPrefix   = null,
                         Func<EVSE, Boolean>?     KeepRemovedEVSEs          = null,
                         Boolean                  LocationsAsOpenData       = true,
                         Boolean?                 AllowDowngrades           = null,
                         Boolean                  Disable_RootServices      = false,

                         HTTPHostname?            HTTPHostname              = null,
                         String?                  ExternalDNSName           = "",
                         String?                  HTTPServiceName           = DefaultHTTPServiceName,
                         HTTPPath?                BasePath                  = null,

                         HTTPPath?                URLPathPrefix             = null,
                         JObject?                 APIVersionHashes          = null,

                         Boolean?                 DisableMaintenanceTasks   = false,
                         TimeSpan?                MaintenanceInitialDelay   = null,
                         TimeSpan?                MaintenanceEvery          = null,

                         Boolean?                 DisableWardenTasks        = false,
                         TimeSpan?                WardenInitialDelay        = null,
                         TimeSpan?                WardenCheckEvery          = null,

                         Boolean?                 IsDevelopment             = false,
                         IEnumerable<String>?     DevelopmentServers        = null,
                         Boolean?                 DisableLogging            = false,
                         String?                  LoggingPath               = null,
                         String?                  LogfileName               = DefaultLogfileName,
                         LogfileCreatorDelegate?  LogfileCreator            = null,
                         Boolean                  Autostart                 = false)



            : base(HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   BasePath,

                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   null, //HTMLTemplate,
                   APIVersionHashes,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName     ?? DefaultLogfileName,
                   LogfileCreator,
                   Autostart)

        {

            this.OurVersionsURL           = OurVersionsURL;
            this.OurBusinessDetails       = OurBusinessDetails;
            this.OurCountryCode           = OurCountryCode;
            this.OurPartyId               = OurPartyId;
            this.OurRole                  = OurRole;

            this.OurBaseURL               = URL.Parse(OurVersionsURL.ToString().Replace("/versions", ""));
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.KeepRemovedEVSEs         = KeepRemovedEVSEs ?? (evse => true);
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;
            this.Disable_RootServices     = Disable_RootServices;

            this.remoteParties            = new Dictionary<RemoteParty_Id, RemoteParty>();
            this.Locations                = new Dictionary<Location_Id,    Location>();
            this.tariffs                  = new Dictionary<Tariff_Id,      Tariff>();
            this.chargingSessions         = new Dictionary<Session_Id,     Session>();
            this.tokenStatus              = new Dictionary<Token_Id,       TokenStatus>();
            this.chargeDetailRecords      = new Dictionary<CDR_Id,         CDR>();

            // Link HTTP events...
            HTTPServer.RequestLog        += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog       += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog          += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            if (!Disable_RootServices)
                RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region GetModuleURL(ModuleId, Stuff = "")

        public URL GetModuleURL(Module_Id  ModuleId,
                                String     Stuff   = "")
        {

            return OurBaseURL + Stuff + ModuleId.ToString();

            //switch (ModuleId)
            //{

            //    case ModuleIds.CDRs:
            //        return OurBaseURL + Stuff + "cdrs";

            //    case ModuleIds.ChargingProfiles:
            //        return OurBaseURL + Stuff + "chargingprofiles";

            //    case ModuleIds.Commands:
            //        return OurBaseURL + Stuff + "commands";

            //    case ModuleIds.Credentials:
            //        return OurBaseURL + Stuff + "credentials";

            //    case ModuleIds.HubClientInfo:
            //        return OurBaseURL + Stuff + "hubclientinfo";

            //    case ModuleIds.Locations:
            //        return OurBaseURL + Stuff + "locations";

            //    case ModuleIds.Sessions:
            //        return OurBaseURL + Stuff + "sessions";

            //    case ModuleIds.Tariffs:
            //        return OurBaseURL + Stuff + "tariffs";

            //    case ModuleIds.Tokens:
            //        return OurBaseURL + Stuff + "tokens";

            //    default:
            //        return OurVersionsURL;

            //}

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS     ~/

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                     HTTPMethod.OPTIONS,
                                                                                     HTTPMethod.GET
                                                                                 },
                                                     AccessControlAllowHeaders  = "Authorization",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPrefix + "/", "cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CommonAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //this.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPrefix + "/index.html",
            //                                 URLPrefix + "/"
            //                             },
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CommonAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CommonAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            #region Text

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPContentType.TEXT_UTF8,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization",
                                                     ContentType                = HTTPContentType.TEXT_UTF8,
                                                     Content                    = "This is an Open Charge Point Interface HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion


            #region OPTIONS     ~/versions

            // ----------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions
            // ----------------------------------------------------
            this.AddOCPIMethod(Hostname,
                                HTTPMethod.OPTIONS,
                                URLPathPrefix + "versions",
                                OCPIRequestHandler: Request => {

                                    return Task.FromResult(
                                        new OCPIResponse.Builder(Request) {
                                            HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                HTTPStatusCode             = HTTPStatusCode.OK,
                                                AccessControlAllowMethods  = "OPTIONS, GET",
                                                Allow                      = new List<HTTPMethod> {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET
                                                                            },
                                                AccessControlAllowHeaders  = "Authorization",
                                                Vary                       = "Accept"
                                            }
                                        });

                                });

            #endregion

            #region GET         ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions
            // ----------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions",
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestHandler: Request => {

                                   #region Check access token

                                   if (Request.AccessInfo2.HasValue &&
                                       Request.AccessInfo2.Value.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                               StatusCode           = 2000,
                                               StatusMessage        = "Invalid or blocked access token!",
                                               HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                   AccessControlAllowMethods  = "OPTIONS, GET",
                                                   AccessControlAllowHeaders  = "Authorization"
                                               }
                                           });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new JArray(
                                                                      new VersionInformation[] {
                                                                          new VersionInformation(
                                                                              Version_Id.Parse("2.1.1"),
                                                                              URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + "/versions/2.1.1")).Replace("//", "/"))
                                                                          )
                                                                      }.Where (version => version is not null).
                                                                      Select(version => version.ToJSON())
                                                                  ),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = "OPTIONS, GET",
                                               AccessControlAllowHeaders  = "Authorization",
                                               Vary                       = "Accept"
                                           }
                                       });

                               });

            #endregion


            #region OPTIONS     ~/versions/{versionId}

            // --------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions/{id}
            // --------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.OPTIONS,
                               URLPathPrefix + "versions/{versionId}",
                               OCPIRequestHandler: Request => {

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = "OPTIONS, GET",
                                               Allow                      = new List<HTTPMethod> {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET
                                                                            },
                                               AccessControlAllowHeaders  = "Authorization",
                                               Vary                       = "Accept"
                                           }
                                       });
                               });

            #endregion

            #region GET         ~/versions/{versionId}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions/{id}
            // ---------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions/{versionId}",
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestHandler: Request => {

                                   #region Check access token

                                   if (Request.AccessInfo2.HasValue &&
                                       Request.AccessInfo2.Value.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = "OPTIONS, GET",
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Get version identification URL parameter

                                   if (Request.ParsedURLParameters.Length < 1)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is missing!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = "OPTIONS, GET",
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          });

                                   }

                                   if (!Version_Id.TryParse(Request.ParsedURLParameters[0], out var versionId))
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is invalid!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = "OPTIONS, GET",
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Only allow versionId == "2.1.1"

                                   if (versionId.ToString() != "2.1.1")
                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "This OCPI version is not supported!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                  AccessControlAllowMethods  = "OPTIONS, GET",
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          });

                                   #endregion


                                   #region Common credential endpoints...

                                   var endpoints  = new List<VersionEndpoint>() {

                                                        new VersionEndpoint(Module_Id.Credentials,
                                                                            URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "credentials")).Replace("//", "/")))

                                                    };

                                   #endregion


                                   #region The other side is a CPO...

                                   if (Request.RemoteParty?.Role == Roles.CPO)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") + 
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "emsp/locations")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "emsp/tariffs")).  Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "emsp/sessions")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "emsp/cdrs")).     Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "emsp/commands")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "emsp/tokens")).   Replace("//", "/"))));

                                   }

                                   #endregion

                                   #region We are a CPO, the other side is unauthenticated and we export locations as Open Data...

                                   if (OurRole == Roles.CPO && Request.RemoteParty is null && LocationsAsOpenData)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "cpo/locations")).Replace("//", "/"))));

                                   }

                                   #endregion

                                   #region The other side is an EMP...

                                   if (Request.RemoteParty?.Role == Roles.EMSP)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "cpo/locations")).       Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "cpo/tariffs")).         Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "cpo/sessions")).        Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "cpo/cdrs")).            Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "cpo/commands")).        Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + versionId.ToString() + "cpo/tokens")).          Replace("//", "/"))));

                                   }

                                   #endregion


                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                              StatusCode           = 1000,
                                              StatusMessage        = "Hello world!",
                                              Data                 = new VersionDetail(
                                                                         versionId,
                                                                         endpoints
                                                                     ).ToJSON(),
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                  AccessControlAllowMethods  = "OPTIONS, GET",
                                                  AccessControlAllowHeaders  = "Authorization",
                                                  Vary                       = "Accept"
                                              }
                                          });

                               });

            #endregion


            #region OPTIONS     ~/{versionId}/credentials

            // ----------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/2.1.1/credentials
            // ----------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.OPTIONS,
                               URLPathPrefix + "{versionId}/credentials",
                               OCPIRequestHandler: Request => {

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                               Allow                      = new List<HTTPMethod> {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET,
                                                                                HTTPMethod.POST,
                                                                                HTTPMethod.PUT,
                                                                                HTTPMethod.DELETE
                                                                            },
                                               AccessControlAllowHeaders  = "Authorization"
                                           }
                                       });

                               });

            #endregion

            #region GET         ~/{versionId}/credentials

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestHandler: Request => {

                                   #region Check access token

                                   if (Request.AccessInfo2.HasValue &&
                                       Request.AccessInfo2.Value.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = "OPTIONS, GET",
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new Credentials(
                                                                      Request.AccessInfo?.Token ?? AccessToken.Parse("<any>"),
                                                                      OurVersionsURL,
                                                                      OurBusinessDetails,
                                                                      OurCountryCode,
                                                                      OurPartyId
                                                                  ).ToJSON(),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                               AccessControlAllowHeaders  = "Authorization"
                                           }
                                       });


                                   //return Task.FromResult(
                                   //    new OCPIResponse.Builder(Request) {
                                   //        StatusCode           = 2000,
                                   //        StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                   //        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                   //            HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                   //            AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                   //            AccessControlAllowHeaders  = "Authorization"
                                   //        }
                                   //    });

                               });

            #endregion

            #region POST        ~/{versionId}/credentials

            // REGISTER new OCPI party!

            // -----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // -----------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.POST,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestLogger:   PostCredentialsRequest,
                               OCPIResponseLogger:  PostCredentialsResponse,
                               OCPIRequestHandler:   async Request => {

                                   var CREDENTIALS_TOKEN_A = Request.AccessToken;

                                   if (Request.RemoteParty is not null &&
                                       Request.AccessInfo.HasValue     &&
                                       Request.AccessInfo.Value.Status == AccessStatus.ALLOWED)
                                   {

                                       if (Request.AccessInfo.Value.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + CREDENTIALS_TOKEN_A.Value.ToString() + "' is already registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                          AccessControlAllowHeaders  = "Authorization"
                                                      }
                                                  };


                                       return await POSTOrPUTCredentials(Request);


                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          };

                               });

            #endregion

            #region PUT         ~/{versionId}/credentials

            // UPDATE the registration of an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.PUT,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestLogger:  PutCredentialsRequest,
                               OCPIResponseLogger: PutCredentialsResponse,
                               OCPIRequestHandler: async Request => {

                                   if (Request.AccessInfo.HasValue &&
                                       Request.AccessInfo.Value.Status == AccessStatus.ALLOWED)
                                   {

                                       // The party is not yet fully registered!
                                       if (!Request.AccessInfo.Value.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is not yet registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                          AccessControlAllowHeaders  = "Authorization"
                                                      }
                                                  };

                                       return await POSTOrPUTCredentials(Request);

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 2000,
                                                  StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                      AccessControlAllowHeaders  = "Authorization"
                                                  }
                                              };

                               });

            #endregion

            #region DELETE      ~/{versionId}/credentials

            // UNREGISTER an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.DELETE,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.JSON_UTF8,
                               OCPIRequestLogger:  DeleteCredentialsRequest,
                               OCPIResponseLogger: DeleteCredentialsResponse,
                               OCPIRequestHandler: async Request => {

                                   if (Request.AccessInfo.HasValue &&
                                       Request.AccessInfo.Value.Status == AccessStatus.ALLOWED)
                                   {

                                       #region Validations

                                       if (!Request.AccessInfo.Value.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + Request.AccessToken.Value.ToString() + "' is not registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                          AccessControlAllowHeaders  = "Authorization"
                                                      }
                                                  };

                                       #endregion


                                       //ToDo: await...
                                       RemoveAccessToken(Request.AccessToken.Value);


                                       return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 1000,
                                                  StatusMessage        = "The given access token was deleted!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                      AccessControlAllowHeaders  = "Authorization"
                                                  }
                                              };

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                                                  AccessControlAllowHeaders  = "Authorization"
                                              }
                                          };

                               });

            #endregion

        }

        #endregion


        #region (private) POSTOrPUTCredentials(Request)

        private async Task<OCPIResponse.Builder> POSTOrPUTCredentials(OCPIRequest Request)
        {

            var CREDENTIALS_TOKEN_A     = Request.AccessToken;

            #region Parse JSON

            var ErrorResponse = String.Empty;

            if (!Request.TryParseJObjectRequestBody(out JObject JSON, out OCPIResponse.Builder ResponseBuilder, AllowEmptyHTTPBody: false) ||
                !Credentials.TryParse(JSON, out var receivedCredentials, out ErrorResponse))
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not parse the credentials JSON object! " + ErrorResponse,
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion

            #region Additional security checks... (Non-Standard)

            //lock (AccessTokens)
            //{
            //    foreach (var credentialsRole in receivedCredentials.Roles)
            //    {

            //        var result = AccessTokens.Values.Where(accessToken => accessToken.Roles.Any(role => role.CountryCode == credentialsRole.CountryCode &&
            //                                                                                            role.PartyId     == credentialsRole.PartyId &&
            //                                                                                            role.Role        == credentialsRole.Role)).ToArray();

            //        if (result.Length == 0)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is unknown!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
            //                           AccessControlAllowHeaders  = "Authorization"
            //                       }
            //                   };

            //        }

            //        if (result.Length > 0 &&
            //            result.First().VersionsURL.HasValue)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is already registered!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
            //                           AccessControlAllowHeaders  = "Authorization"
            //                       }
            //                   };

            //        }

            //    }
            //}

            #endregion


            var commonClient            = new CommonClient(receivedCredentials.URL,
                                                           receivedCredentials.Token,  // CREDENTIALS_TOKEN_B
                                                           this,
                                                           DNSClient: HTTPServer.DNSClient);

            var otherVersions           = await commonClient.GetVersions();

            #region ...or send error!

            if (otherVersions.StatusCode != 1000)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not fetch VERSIONS information from '" + receivedCredentials.URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion

            var version2_1_1            = Version_Id.Parse("2.1.1");
            var justVersion2_1_1        = otherVersions.Data.Where(version => version.Id == version2_1_1).ToArray();

            #region ...or send error!

            if (justVersion2_1_1.Length == 0)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3003,
                           StatusMessage        = "Could not find OCPI v2.1.1 at '" + receivedCredentials.URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion

            var otherVersion2_1_1Details  = await commonClient.GetVersionDetails(version2_1_1);

            #region ...or send error!

            if (otherVersion2_1_1Details.StatusCode != 1000)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3001,
                           StatusMessage        = "Could not fetch v2.2 information from '" + justVersion2_1_1.First().URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

            }

            #endregion



            var CREDENTIALS_TOKEN_C = AccessToken.NewRandom();

            // Store credential of the other side!
            AddOrUpdateRemoteParty(receivedCredentials.CountryCode,
                                   receivedCredentials.PartyId,
                                   receivedCredentials.BusinessDetails,

                                   CREDENTIALS_TOKEN_C, // -------------------------------------------------- !!!

                                   receivedCredentials.Token,
                                   receivedCredentials.URL,
                                   otherVersions.Data.Select(version => version.Id),
                                   version2_1_1,

                                   null, //receivedCredentials.Role,
                                   null,
                                   AccessStatus.      ALLOWED,
                                   RemoteAccessStatus.ONLINE,
                                   PartyStatus.       ENABLED);

            //SetIncomingAccessToken(CREDENTIALS_TOKEN_C,
            //                       receivedCredentials.URL,
            //                       receivedCredentials.Roles,
            //                       AccessStatus.ALLOWED);


            RemoveAccessToken(CREDENTIALS_TOKEN_A.Value);



            return new OCPIResponse.Builder(Request) {
                           StatusCode           = 1000,
                           StatusMessage        = "Hello world!",
                           Data                 = new Credentials(
                                                      CREDENTIALS_TOKEN_C,
                                                      OurVersionsURL,
                                                      OurBusinessDetails,
                                                      OurCountryCode,
                                                      OurPartyId
                                                  ).ToJSON(),
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.OK,
                               AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                               AccessControlAllowHeaders  = "Authorization"
                           }
                       };

        }

        #endregion


        //ToDo: Wrap the following into a plugable interface!

        #region RemoteParties

        #region Data

        private readonly Dictionary<RemoteParty_Id, RemoteParty> remoteParties;

        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion

        #region (private) GetRemotePartySerializator(Request, User)

        //private RemotePartyToJSONDelegate GetRemotePartySerializator(HTTPRequest  Request,
        //                                                             User         User)
        //{

        //    return (RemoteParty,
        //            Embedded,
        //            CustomRemotePartySerializer,
        //            CustomBusinessDetailsSerializer,
        //            IncludeCryptoHash)

        //            => RemoteParty.ToJSON(Embedded,
        //                                  CustomRemotePartySerializer,
        //                                  CustomBusinessDetailsSerializer,
        //                                  IncludeCryptoHash);

        //}

        #endregion


        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode               CountryCode,
                                      Party_Id                  PartyId,
                                      Roles                     Role,
                                      BusinessDetails           BusinessDetails,

                                      AccessToken               AccessToken,

                                      AccessToken               RemoteAccessToken,
                                      URL                       RemoteVersionsURL,
                                      IEnumerable<Version_Id>?  RemoteVersionIds            = null,
                                      Version_Id?               SelectedVersionId           = null,

                                      Boolean?                  AccessTokenBase64Encoding   = null,
                                      AccessStatus              AccessStatus                = AccessStatus.      ALLOWED,
                                      RemoteAccessStatus?       RemoteStatus                = RemoteAccessStatus.ONLINE,
                                      PartyStatus               PartyStatus                 = PartyStatus.       ENABLED)
        {

            lock (remoteParties)
            {

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessToken,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     AccessStatus,
                                                     RemoteStatus,
                                                     PartyStatus);

                remoteParties.Add(newRemoteParty.Id,
                                  newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }

        }

        #endregion

        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode      CountryCode,
                                      Party_Id         PartyId,
                                      Roles            Role,
                                      BusinessDetails  BusinessDetails,

                                      AccessToken      AccessToken,
                                      AccessStatus     AccessStatus   = AccessStatus.ALLOWED,

                                      PartyStatus      PartyStatus    = PartyStatus. ENABLED)
        {

            lock (remoteParties)
            {

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessToken,
                                                     AccessStatus,

                                                     PartyStatus);

                remoteParties.Add(newRemoteParty.Id,
                                  newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }

        }

        #endregion

        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode               CountryCode,
                                      Party_Id                  PartyId,
                                      Roles                     Role,
                                      BusinessDetails           BusinessDetails,

                                      AccessToken               RemoteAccessToken,
                                      URL                       RemoteVersionsURL,
                                      IEnumerable<Version_Id>?  RemoteVersionIds            = null,
                                      Version_Id?               SelectedVersionId           = null,

                                      Boolean?                  AccessTokenBase64Encoding   = null,
                                      RemoteAccessStatus?       RemoteStatus                = RemoteAccessStatus.UNKNOWN,
                                      PartyStatus               PartyStatus                 = PartyStatus.       ENABLED)
        {

            lock (remoteParties)
            {

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     RemoteStatus,
                                                     PartyStatus);

                remoteParties.Add(newRemoteParty.Id,
                                  newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }

        }

        #endregion

        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode                    CountryCode,
                                      Party_Id                       PartyId,
                                      Roles                          Role,
                                      BusinessDetails                BusinessDetails,

                                      IEnumerable<AccessInfoStatus>       AccessInfos,
                                      IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                                      PartyStatus                    Status        = PartyStatus.ENABLED,
                                      DateTime?                      LastUpdated   = null)
        {

            lock (remoteParties)
            {

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessInfos,
                                                     RemoteAccessInfos,

                                                     Status,
                                                     LastUpdated);

                remoteParties.Add(newRemoteParty.Id,
                                  newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }

        }

        #endregion


        #region AddOrUpdateRemoteParty(...)

        public Boolean AddOrUpdateRemoteParty(CountryCode               CountryCode,
                                              Party_Id                  PartyId,
                                              BusinessDetails           BusinessDetails,

                                              AccessToken               AccessToken,

                                              AccessToken               RemoteAccessToken,
                                              URL                       RemoteVersionsURL,
                                              IEnumerable<Version_Id>?  RemoteVersionIds            = null,
                                              Version_Id?               SelectedVersionId           = null,

                                              Roles?                    Role                        = null,

                                              Boolean?                  AccessTokenBase64Encoding   = null,
                                              AccessStatus              AccessStatus                = AccessStatus.      ALLOWED,
                                              RemoteAccessStatus?       RemoteStatus                = RemoteAccessStatus.ONLINE,
                                              PartyStatus               PartyStatus                 = PartyStatus.       ENABLED)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                                                                                party.PartyId     == PartyId))
                {

                    Role = remoteParty.Role;

                    remoteParties.Remove(remoteParty.Id);

                }

                if (!Role.HasValue)
                    return false;

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role.Value,
                                                     BusinessDetails,

                                                     AccessToken,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     AccessStatus,
                                                     RemoteStatus,
                                                     PartyStatus);

                remoteParties.Add(newRemoteParty.Id,
                                  newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addOrUpdateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public Boolean AddOrUpdateRemoteParty(CountryCode      CountryCode,
                                              Party_Id         PartyId,
                                              Roles            Role,
                                              BusinessDetails  BusinessDetails,

                                              AccessToken      AccessToken,
                                              AccessStatus     AccessStatus   = AccessStatus.ALLOWED,

                                              PartyStatus      PartyStatus    = PartyStatus. ENABLED)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                                                                                party.PartyId     == PartyId))
                {
                    remoteParties.Remove(remoteParty.Id);
                }

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     AccessToken,
                                                     AccessStatus,

                                                     PartyStatus);

                remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addOrUpdateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public Boolean AddOrUpdateRemoteParty(CountryCode               CountryCode,
                                              Party_Id                  PartyId,
                                              Roles                     Role,
                                              BusinessDetails           BusinessDetails,

                                              AccessToken               RemoteAccessToken,
                                              URL                       RemoteVersionsURL,
                                              IEnumerable<Version_Id>?  RemoteVersionIds            = null,
                                              Version_Id?               SelectedVersionId           = null,

                                              Boolean?                  AccessTokenBase64Encoding   = null,
                                              RemoteAccessStatus?       RemoteStatus                = RemoteAccessStatus.UNKNOWN,
                                              PartyStatus               PartyStatus                 = PartyStatus.       ENABLED)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(party => party.CountryCode == CountryCode &&
                                                                                party.PartyId     == PartyId))
                {
                    remoteParties.Remove(remoteParty.Id);
                }

                var newRemoteParty = new RemoteParty(CountryCode,
                                                     PartyId,
                                                     Role,
                                                     BusinessDetails,

                                                     RemoteAccessToken,
                                                     RemoteVersionsURL,
                                                     RemoteVersionIds,
                                                     SelectedVersionId,

                                                     AccessTokenBase64Encoding,
                                                     RemoteStatus,
                                                     PartyStatus);

                remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                File.AppendAllText(LogfileName,
                                   new JObject(new JProperty("addOrUpdateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                   Encoding.UTF8);

                return true;

            }

        }

        #endregion


        #region ContainsRemoteParty(RemotePartyId)

        /// <summary>
        /// Whether this API contains a remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public Boolean ContainsRemoteParty(RemoteParty_Id RemotePartyId)
        {

            try
            {

                //RemotePartiesSemaphore.Wait();

                return remoteParties.ContainsKey(RemotePartyId);

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region GetRemoteParty     (RemotePartyId)

        /// <summary>
        /// Get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public RemoteParty? GetRemoteParty(RemoteParty_Id RemotePartyId)
        {

            try
            {

                //await RemotePartiesSemaphore.WaitAsync();

                if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty))
                    return remoteParty;

                return null;

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region TryGetRemoteParty  (RemotePartyId, out RemoteParty)

        /// <summary>
        /// Try to get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        /// <param name="RemoteParty">The defibrillator.</param>
        public Boolean TryGetRemoteParty(RemoteParty_Id    RemotePartyId,
                                         out RemoteParty?  RemoteParty)
        {

            try
            {

                //RemotePartiesSemaphore.Wait();

                if (remoteParties.TryGetValue(RemotePartyId, out RemoteParty))
                    return true;

                RemoteParty = null;
                return false;

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region GetRemoteParties   (IncludeFilter = null)

        /// <summary>
        /// Get all remote parties machting the given optional filter.
        /// </summary>
        /// <param name="IncludeFilter">A delegate for filtering remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(IncludeRemoteParty? IncludeFilter = null)
        {

            try
            {

                //await RemotePartiesSemaphore.WaitAsync();

                return IncludeFilter is null
                           ? remoteParties.Values.ToArray()
                           : remoteParties.Values.Where(remoteParty => IncludeFilter(remoteParty)).ToArray();

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion

        #region GetRemoteParties   (Role)

        /// <summary>
        /// Get all remote parties having the given role.
        /// </summary>
        /// <param name="Role">The role of the remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(Roles Role)
        {

            try
            {

                //await RemotePartiesSemaphore.WaitAsync();

                return remoteParties.Values.Where(remoteParty => remoteParty.Role == Role).ToArray();

            }
            finally
            {
                //RemotePartiesSemaphore.Release();
            }

        }

        #endregion


        #region RemoveRemoteParty(RemoteParty)

        public Boolean RemoveRemoteParty(RemoteParty RemoteParty)
        {

            lock (remoteParties)
            {

                if (remoteParties.Remove(RemoteParty.Id, out var remoteParty))
                {

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region RemoveRemoteParty(RemotePartyId)

        public Boolean RemoveRemoteParty(RemoteParty_Id RemotePartyId)
        {

            lock (remoteParties)
            {

                if (remoteParties.Remove(RemotePartyId, out var remoteParty))
                {

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                    return true;

                }

                return false;

            }

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId)

        public Boolean RemoveRemoteParty(CountryCode  CountryCode,
                                         Party_Id     PartyId)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                                                      remoteParty.PartyId     == PartyId))
                {

                    remoteParties.Remove(remoteParty.Id);

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                }

                return true;

            }

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, AccessToken)

        public Boolean RemoveRemoteParty(CountryCode  CountryCode,
                                         Party_Id     PartyId,
                                         AccessToken  AccessToken)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                                                      remoteParty.PartyId     == PartyId     &&
                                                                                      remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
                {

                    remoteParties.Remove(remoteParty.Id);

                    File.AppendAllText(LogfileName,
                                       new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                       Encoding.UTF8);

                }

                return true;

            }

        }

        #endregion

        #endregion

        #region AccessTokens

        // An access token might be used by more than one CountryCode + PartyId + Role combination!

        #region RemoveAccessToken(AccessToken)

        public CommonAPI RemoveAccessToken(AccessToken AccessToken)
        {

            lock (remoteParties)
            {

                foreach (var remoteParty in remoteParties.Values.Where(party => party.AccessInfoStatus.Any(accessInfo => accessInfo.Token == AccessToken)))
                {

                    if (remoteParty.AccessInfoStatus.Count() <= 1)
                    {

                        remoteParties.Remove(remoteParty.Id);

                        File.AppendAllText(LogfileName,
                                           new JObject(new JProperty("removeRemoteParty", remoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                           Encoding.UTF8);

                    }

                    else
                    {

                        remoteParties.Remove(remoteParty.Id);

                        var newRemoteParty = new RemoteParty(
                                                 remoteParty.CountryCode,
                                                 remoteParty.PartyId,
                                                 remoteParty.Role,
                                                 remoteParty.BusinessDetails,
                                                 remoteParty.AccessInfoStatus.Where(accessInfo => accessInfo.Token != AccessToken),
                                                 remoteParty.RemoteAccessInfos,
                                                 remoteParty.Status
                                             );

                        remoteParties.Add(newRemoteParty.Id, newRemoteParty);

                        File.AppendAllText(LogfileName,
                                           new JObject(new JProperty("updateRemoteParty", newRemoteParty.ToJSON(true))).ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine,
                                           Encoding.UTF8);

                    }

                }

                return this;

            }

        }

        #endregion

        #region TryGetAccessInfo(AccessToken, out AccessInfo2)

        public Boolean TryGetAccessInfo(AccessToken AccessToken, out AccessInfoStatus AccessInfo2)
        {

            lock (remoteParties)
            {

                var accessInfos = remoteParties.Values.Where     (remoteParty => remoteParty.AccessInfoStatus.Any(accessInfo => accessInfo.Token == AccessToken)).
                                                       SelectMany(remoteParty => remoteParty.AccessInfoStatus).
                                                       ToArray();

                if (accessInfos.Length == 1)
                {
                    AccessInfo2 = accessInfos.First();
                    return true;
                }

                AccessInfo2 = default;
                return false;

            }

        }

        #endregion

        #region GetAccessInfos(AccessToken)

        public IEnumerable<AccessInfoStatus> GetAccessInfos(AccessToken AccessToken)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where     (remoteParty => remoteParty.AccessInfoStatus.Any(accessInfo => accessInfo.Token == AccessToken)).
                                            SelectMany(remoteParty => remoteParty.AccessInfoStatus).
                                            ToArray();

            }
        }

        #endregion

        #region GetAccessInfos(AccessToken, AccessStatus)

        public IEnumerable<AccessInfoStatus> GetAccessInfos(AccessToken   AccessToken,
                                                       AccessStatus  AccessStatus)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where     (remoteParty => remoteParty.AccessInfoStatus.Any(accessInfo => accessInfo.Token  == AccessToken &&
                                                                                                               accessInfo.Status == AccessStatus)).
                                            SelectMany(remoteParty => remoteParty.AccessInfoStatus).
                                            ToArray();

            }
        }

        #endregion


        #region GetRemoteParties(AccessToken)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken AccessToken)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where(remoteParty => remoteParty.AccessInfoStatus.Any(accessInfo => accessInfo.Token == AccessToken)).
                                             ToArray();

            }
        }

        #endregion

        #region GetRemoteParties(AccessToken, AccessStatus)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken   AccessToken,
                                                         AccessStatus  AccessStatus)
        {
            lock (remoteParties)
            {

                return remoteParties.Values.Where(remoteParty => remoteParty.AccessInfoStatus.Any(accessInfo => accessInfo.Token  == AccessToken &&
                                                                                                           accessInfo.Status == AccessStatus)).
                                             ToArray();

            }
        }

        #endregion

        #region GetRemoteParties(AccessToken, out RemoteParties)

        public Boolean TryGetRemoteParties(AccessToken                   AccessToken,
                                           out IEnumerable<RemoteParty>  RemoteParties)
        {
            lock (remoteParties)
            {

                RemoteParties = remoteParties.Values.Where(remoteParty => remoteParty.AccessInfoStatus.Any(accessInfo => accessInfo.Token == AccessToken)).
                                                     ToArray();

                return RemoteParties.Any();

            }
        }

        #endregion

        #endregion


        // Add last modified timestamp to locations!

        #region Locations

        private readonly Dictionary<Location_Id , Location> Locations;


        public delegate Task OnLocationAddedDelegate(Location Location);

        public event OnLocationAddedDelegate? OnLocationAdded;


        public delegate Task OnLocationChangedDelegate(Location Location);

        public event OnLocationChangedDelegate? OnLocationChanged;


        #region AddLocation           (Location, SkipNotifications = false)

        public Location AddLocation(Location  Location,
                                    Boolean   SkipNotifications   = false)
        {

            lock (Locations)
            {


                if (!Locations.ContainsKey(Location.Id))
                {

                    Locations.Add(Location.Id, Location);

                    if (!SkipNotifications)
                    {

                        var OnLocationAddedLocal = OnLocationAdded;
                        if (OnLocationAddedLocal is not null)
                        {
                            try
                            {
                                OnLocationAddedLocal(Location).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                    return Location;

                }

                throw new ArgumentException("The given location already exists!");

            }

        }

        #endregion

        #region AddLocationIfNotExists(Location, SkipNotifications = false)

        public Location AddLocationIfNotExists(Location  Location,
                                               Boolean   SkipNotifications   = false)
        {

            lock (Locations)
            {

                if (!Locations.ContainsKey(Location.Id))
                {

                    Locations.Add(Location.Id, Location);

                    if (!SkipNotifications)
                    {

                        var OnLocationAddedLocal = OnLocationAdded;
                        if (OnLocationAddedLocal is not null)
                        {
                            try
                            {
                                OnLocationAddedLocal(Location).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddLocationIfNotExists), " ", nameof(OnLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                }

                return Location;

            }

        }

        #endregion

        #region AddOrUpdateLocation   (newOrUpdatedLocation, AllowDowngrades = false)

        private AddOrUpdateResult<Location> __addOrUpdateLocation(Location  newOrUpdatedLocation,
                                                                  Boolean?  AllowDowngrades = false)
        {

            if (Locations.TryGetValue(newOrUpdatedLocation.Id, out var existingLocation))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedLocation.LastUpdated <= existingLocation.LastUpdated)
                {
                    return AddOrUpdateResult<Location>.Failed(newOrUpdatedLocation,
                                                              "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");
                }

                Locations[newOrUpdatedLocation.Id] = newOrUpdatedLocation;

                var OnLocationChangedLocal = OnLocationChanged;
                if (OnLocationChangedLocal is not null)
                {
                    try
                    {
                        OnLocationChangedLocal(newOrUpdatedLocation).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                var OnEVSEChangedLocal = OnEVSEChanged;
                if (OnEVSEChangedLocal is not null)
                {
                    try
                    {
                        foreach (var evse in newOrUpdatedLocation.EVSEs)
                            OnEVSEChangedLocal(evse).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Location>.Success(newOrUpdatedLocation,
                                                           WasCreated: false);

            }

            Locations.Add(newOrUpdatedLocation.Id, newOrUpdatedLocation);

            var OnLocationAddedLocal = OnLocationAdded;
            if (OnLocationAddedLocal is not null)
            {
                try
                {
                    OnLocationAddedLocal(newOrUpdatedLocation).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationAdded), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace);
                }
            }

            return AddOrUpdateResult<Location>.Success(newOrUpdatedLocation,
                                                       WasCreated: true);

        }

        public async Task<AddOrUpdateResult<Location>> AddOrUpdateLocation(Location  newOrUpdatedLocation,
                                                                           Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedLocation is null)
                return AddOrUpdateResult<Location>.Failed(newOrUpdatedLocation,
                                                          "The given location must not be null!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
            {
                return __addOrUpdateLocation(newOrUpdatedLocation,
                                             AllowDowngrades);
            }

        }

        #endregion

        #region UpdateLocation        (Location)

        public Location? UpdateLocation(Location Location)
        {

            lock (Locations)
            {

                if (Locations.ContainsKey(Location.Id))
                {

                    Locations[Location.Id] = Location;

                    var OnEVSEChangedLocal = OnEVSEChanged;
                    if (OnEVSEChangedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                OnEVSEChangedLocal(evse).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Location;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchLocation      (Location, LocationPatch, AllowDowngrades = false)

        public async Task<PatchResult<Location>> TryPatchLocation(Location  Location,
                                                                  JObject   LocationPatch,
                                                                  Boolean?  AllowDowngrades = false)
        {

            if (!LocationPatch.HasValues)
                return PatchResult<Location>.Failed(Location,
                                                    "The given location patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
            {

                if (Locations.TryGetValue(Location.Id, out var existingLocation))
                {

                    var patchResult = existingLocation.TryPatch(LocationPatch,
                                                                AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        Locations[Location.Id] = patchResult.PatchedData;

                        var OnLocationChangedLocal = OnLocationChanged;
                        if (OnLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnLocationChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchLocation), " ", nameof(OnLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                        //ToDo: MayBe nothing changed here... perhaps test for changes before sending events!
                        var OnEVSEChangedLocal = OnEVSEChanged;
                        if (OnEVSEChangedLocal is not null)
                        {
                            try
                            {
                                foreach (var evse in patchResult.PatchedData.EVSEs)
                                    OnEVSEChangedLocal(evse).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchLocation), " ", nameof(OnEVSEChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                    return patchResult;

                }

                else
                    return PatchResult<Location>.Failed(Location,
                                                        "The given location does not exist!");

            }

        }

        #endregion



        public delegate Task OnEVSEAddedDelegate(EVSE EVSE);

        public event OnEVSEAddedDelegate? OnEVSEAdded;


        public delegate Task OnEVSEChangedDelegate(EVSE EVSE);

        public event OnEVSEChangedDelegate? OnEVSEChanged;

        public delegate Task OnEVSEStatusChangedDelegate(DateTime Timestamp, EVSE EVSE, StatusType OldEVSEStatus, StatusType NewEVSEStatus);

        public event OnEVSEStatusChangedDelegate? OnEVSEStatusChanged;


        public delegate Task OnEVSERemovedDelegate(EVSE EVSE);

        public event OnEVSERemovedDelegate? OnEVSERemoved;


        #region AddOrUpdateEVSE       (Location, newOrUpdatedEVSE, AllowDowngrades = false)

        private AddOrUpdateResult<EVSE> __addOrUpdateEVSE(Location  Location,
                                                          EVSE      newOrUpdatedEVSE,
                                                          Boolean?  AllowDowngrades = false)
        {

            Location.TryGetEVSE(newOrUpdatedEVSE.UId, out var existingEVSE);

            if (existingEVSE is not null)
            {
                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedEVSE.LastUpdated < existingEVSE.LastUpdated)
                {
                    return AddOrUpdateResult<EVSE>.Failed(newOrUpdatedEVSE,
                                                          "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");
                }
            }


            Location.SetEVSE(newOrUpdatedEVSE);

            // Update location timestamp!
            var builder = Location.ToBuilder();
            builder.LastUpdated = newOrUpdatedEVSE.LastUpdated;
            __addOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);

            var OnLocationChangedLocal = OnLocationChanged;
            if (OnLocationChangedLocal is not null)
            {
                try
                {
                    OnLocationChangedLocal(newOrUpdatedEVSE.ParentLocation).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnLocationChanged), ": ",
                                Environment.NewLine, e.Message,
                                e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                }
            }


            if (existingEVSE is not null)
            {

                if (existingEVSE.Status != StatusType.REMOVED)
                {

                    var OnEVSEChangedLocal = OnEVSEChanged;
                    if (OnEVSEChangedLocal is not null)
                    {
                        try
                        {
                            OnEVSEChangedLocal(newOrUpdatedEVSE).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                        }
                    }


                    if (existingEVSE.Status != newOrUpdatedEVSE.Status)
                    {
                        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        if (OnEVSEStatusChangedLocal is not null)
                        {
                            try
                            {

                                OnEVSEStatusChangedLocal(Timestamp.Now,
                                                         newOrUpdatedEVSE,
                                                         existingEVSE.Status,
                                                         newOrUpdatedEVSE.Status).Wait();

                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            e.StackTrace is not null
                                                ? Environment.NewLine + e.StackTrace
                                                : String.Empty);
                            }
                        }
                    }

                }
                else
                {

                    if (!KeepRemovedEVSEs(newOrUpdatedEVSE))
                        Location.RemoveEVSE(newOrUpdatedEVSE);

                    var OnEVSERemovedLocal = OnEVSERemoved;
                    if (OnEVSERemovedLocal is not null)
                    {
                        try
                        {
                            OnEVSERemovedLocal(newOrUpdatedEVSE).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSERemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                        }
                    }

                }
            }
            else
            {
                var OnEVSEAddedLocal = OnEVSEAdded;
                if (OnEVSEAddedLocal is not null)
                {
                    try
                    {
                        OnEVSEAddedLocal(newOrUpdatedEVSE).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                    }
                }
            }

            return AddOrUpdateResult<EVSE>.Success(newOrUpdatedEVSE,
                                                   WasCreated: existingEVSE is null);

        }

        public async Task<AddOrUpdateResult<EVSE>> AddOrUpdateEVSE(Location  Location,
                                                                   EVSE      newOrUpdatedEVSE,
                                                                   Boolean?  AllowDowngrades = false)
        {

            // ToDo: Remove me and add a proper 'lock' mechanism!
            //await Task.Delay(1);

            //lock (Locations)
            //{
                return __addOrUpdateEVSE(Location,
                                         newOrUpdatedEVSE,
                                         (AllowDowngrades ?? this.AllowDowngrades) == false);
            //}

        }

        #endregion

        #region TryPatchEVSE          (Location, EVSE, EVSEPatch,  AllowDowngrades = false)

        public async Task<PatchResult<EVSE>> TryPatchEVSE(Location  Location,
                                                          EVSE      EVSE,
                                                          JObject   EVSEPatch,
                                                          Boolean?  AllowDowngrades = false)
        {

            if (!EVSEPatch.HasValues)
                return PatchResult<EVSE>.Failed(EVSE,
                                                "The given EVSE patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
            {

                var patchResult        = EVSE.TryPatch(EVSEPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false);

                var justAStatusChange  = EVSEPatch.Children().Count() == 2 && EVSEPatch.ContainsKey("status") && EVSEPatch.ContainsKey("last_updated");

                if (patchResult.IsSuccess)
                {

                    if (patchResult.PatchedData.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                        Location.SetEVSE   (patchResult.PatchedData);
                    else
                        Location.RemoveEVSE(patchResult.PatchedData);

                    // Update location timestamp!
                    var builder = Location.ToBuilder();
                    builder.LastUpdated = patchResult.PatchedData.LastUpdated;
                    __addOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);


                    if (EVSE.Status != StatusType.REMOVED)
                    {

                        if (justAStatusChange)
                        {

                            DebugX.Log("EVSE status change: " + EVSE.EVSEId + " => " + patchResult.PatchedData.Status);

                            var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                            if (OnEVSEStatusChangedLocal is not null)
                            {
                                try
                                {

                                    OnEVSEStatusChangedLocal(patchResult.PatchedData.LastUpdated,
                                                             EVSE,
                                                             EVSE.Status,
                                                             patchResult.PatchedData.Status).Wait();

                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                e.StackTrace is not null
                                                    ? Environment.NewLine + e.StackTrace
                                                    : String.Empty);
                                }
                            }

                        }
                        else
                        {

                            var OnEVSEChangedLocal = OnEVSEChanged;
                            if (OnEVSEChangedLocal is not null)
                            {
                                try
                                {
                                    OnEVSEChangedLocal(patchResult.PatchedData).Wait();
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchEVSE), " ", nameof(OnEVSEChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                e.StackTrace is not null
                                                    ? Environment.NewLine + e.StackTrace
                                                    : String.Empty);
                                }
                            }

                        }

                    }
                    else
                    {

                        var OnEVSERemovedLocal = OnEVSERemoved;
                        if (OnEVSERemovedLocal is not null)
                        {
                            try
                            {
                                OnEVSERemovedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchEVSE), " ", nameof(OnEVSERemoved), ": ",
                                            Environment.NewLine, e.Message,
                                            e.StackTrace is not null
                                                ? Environment.NewLine + e.StackTrace
                                                : String.Empty);
                            }
                        }

                    }

                }

                return patchResult;

            }

        }

        #endregion


        #region AddOrUpdateConnector  (Location, EVSE, newOrUpdatedConnector,     AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Connector>> AddOrUpdateConnector(Location   Location,
                                                                             EVSE       EVSE,
                                                                             Connector  newOrUpdatedConnector,
                                                                             Boolean?   AllowDowngrades = false)
        {

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
            {

                var ConnectorExistedBefore = EVSE.TryGetConnector(newOrUpdatedConnector.Id, out Connector existingConnector);

                if (existingConnector is not null)
                {
                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedConnector.LastUpdated < existingConnector.LastUpdated)
                    {
                        return AddOrUpdateResult<Connector>.Failed(newOrUpdatedConnector,
                                                                   "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                    }
                }

                EVSE.UpdateConnector(newOrUpdatedConnector);

                // Update EVSE/location timestamps!
                var evseBuilder     = EVSE.ToBuilder();
                evseBuilder.LastUpdated = newOrUpdatedConnector.LastUpdated;
                __addOrUpdateEVSE    (Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);


                var OnLocationChangedLocal = OnLocationChanged;
                if (OnLocationChangedLocal is not null)
                {
                    try
                    {
                        OnLocationChangedLocal(newOrUpdatedConnector.ParentEVSE.ParentLocation).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateConnector), " ", nameof(OnLocationChanged), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }


                return AddOrUpdateResult<Connector>.Success(newOrUpdatedConnector,
                                                            WasCreated: !ConnectorExistedBefore);

            }

        }

        #endregion

        #region TryPatchConnector     (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false)

        public async Task<PatchResult<Connector>> TryPatchConnector(Location   Location,
                                                                    EVSE       EVSE,
                                                                    Connector  Connector,
                                                                    JObject    ConnectorPatch,
                                                                    Boolean?   AllowDowngrades = false)
        {

            if (!ConnectorPatch.HasValues)
                return PatchResult<Connector>.Failed(Connector,
                                                     "The given connector patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (Locations)
            {

                var patchResult = Connector.TryPatch(ConnectorPatch,
                                                     AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess)
                {

                    EVSE.UpdateConnector(patchResult.PatchedData);

                    // Update EVSE/location timestamps!
                    var evseBuilder     = EVSE.ToBuilder();
                    evseBuilder.LastUpdated = patchResult.PatchedData.LastUpdated;
                    __addOrUpdateEVSE    (Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);

                }

                return patchResult;

            }

        }

        #endregion


        #region LocationExists(LocationId)

        public Boolean LocationExists(Location_Id LocationId)
        {

            lock (Locations)
            {

                return Locations.ContainsKey(LocationId);

            }

        }

        #endregion

        #region TryGetLocation(LocationId, out Location)

        public Boolean TryGetLocation(Location_Id    LocationId,
                                      out Location?  Location)
        {

            lock (Locations)
            {

                if (Locations.TryGetValue(LocationId, out Location))
                    return true;

                Location = null;
                return false;

            }

        }

        #endregion

        #region GetLocations  (IncludeLocation = null)

        public IEnumerable<Location> GetLocations(Func<Location, Boolean>? IncludeLocation = null)
        {

            lock (Locations)
            {

                return IncludeLocation is null
                           ? Locations.Values.ToArray()
                           : Locations.Values.Where(IncludeLocation).ToArray();

            }

        }

        #endregion

        #region GetLocations  (CountryCode, PartyId)

        public IEnumerable<Location> GetLocations(CountryCode  CountryCode,
                                                  Party_Id     PartyId)
        {

            lock (Locations)
            {

                return Locations.Values.Where(location => location.CountryCode == CountryCode &&
                                                          location.PartyId     == PartyId).
                                               ToArray();

            }

        }

        #endregion


        #region RemoveLocation    (Location)

        public Boolean RemoveLocation(Location Location)
        {

            lock (Locations)
            {

                return Locations.Remove(Location.Id);

            }

        }

        #endregion

        #region RemoveLocation    (LocationId)

        public Boolean RemoveLocation(Location_Id LocationId)
        {

            lock (Locations)
            {

                return Locations.Remove(LocationId);

            }

        }

        #endregion

        #region RemoveAllLocations(IncludeSessions = null)

        /// <summary>
        /// Remove all matching locations.
        /// </summary>
        /// <param name="IncludeLocations">An optional location filter.</param>
        public void RemoveAllLocations(Func<Location, Boolean>? IncludeLocations = null)
        {

            lock (Locations)
            {

                if (IncludeLocations is null)
                    Locations.Clear();

                else
                {

                    var locationsToDelete = Locations.Values.Where(IncludeLocations).ToArray();

                    foreach (var location in locationsToDelete)
                        Locations.Remove(location.Id);

                }

            }

        }

        #endregion

        #region RemoveAllLocations(CountryCode, PartyId)

        /// <summary>
        /// Remove all locations owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllLocations(CountryCode  CountryCode,
                                       Party_Id     PartyId)
        {

            lock (Locations)
            {

                var locationsToDelete = Locations.Values.Where(location => CountryCode == location.CountryCode &&
                                                                           PartyId     == location.PartyId).
                                                               ToArray();

                foreach (var location in locationsToDelete)
                    Locations.Remove(location.Id);

            }

        }

        #endregion

        #endregion

        #region Tariffs

        private readonly Dictionary<Tariff_Id , Tariff> tariffs;


        public delegate Task OnTariffAddedDelegate(Tariff Tariff);

        public event OnTariffAddedDelegate? OnTariffAdded;


        public delegate Task OnTariffChangedDelegate(Tariff Tariff);

        public event OnTariffChangedDelegate? OnTariffChanged;


        #region AddTariff           (Tariff)

        public Tariff AddTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (tariffs)
            {

                if (!tariffs.ContainsKey(Tariff.Id))
                {

                    tariffs.Add(Tariff.Id, Tariff);

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddTariff), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Tariff;

                }

                throw new ArgumentException("The given tariff already exists!");

            }

        }

        #endregion

        #region AddTariffIfNotExists(Tariff)

        public Tariff AddTariffIfNotExists(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (tariffs)
            {

                if (!tariffs.ContainsKey(Tariff.Id))
                {

                    tariffs.Add(Tariff.Id, Tariff);

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddTariffIfNotExists), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                }

                return Tariff;

            }

        }

        #endregion

        #region AddOrUpdateTariff   (newOrUpdatedTariff, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Tariff>> AddOrUpdateTariff(Tariff    newOrUpdatedTariff,
                                                                       Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedTariff is null)
                throw new ArgumentNullException(nameof(newOrUpdatedTariff), "The given charging tariff must not be null!");

            lock (tariffs)
            {

                if (tariffs.TryGetValue(newOrUpdatedTariff.Id, out var existingTariff))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedTariff.LastUpdated <= existingTariff.LastUpdated)
                    {
                        return AddOrUpdateResult<Tariff>.Failed(newOrUpdatedTariff,
                                                                "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");
                    }

                    tariffs[newOrUpdatedTariff.Id] = newOrUpdatedTariff;

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(newOrUpdatedTariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return AddOrUpdateResult<Tariff>.Success(newOrUpdatedTariff,
                                                             WasCreated: false);

                }

                tariffs.Add(newOrUpdatedTariff.Id, newOrUpdatedTariff);

                var OnTariffAddedLocal = OnTariffAdded;
                if (OnTariffAddedLocal is not null)
                {
                    try
                    {
                        OnTariffAddedLocal(newOrUpdatedTariff).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Tariff>.Success(newOrUpdatedTariff,
                                                         WasCreated: true);

            }

        }

        #endregion

        #region UpdateTariff        (Tariff)

        public Tariff UpdateTariff(Tariff Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            lock (tariffs)
            {

                if (tariffs.ContainsKey(Tariff.Id))
                {

                    tariffs[Tariff.Id] = Tariff;

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Tariff;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchTariff      (Tariff, TariffPatch, AllowDowngrades = false)

        public async Task<PatchResult<Tariff>> TryPatchTariff(Tariff    Tariff,
                                                              JObject   TariffPatch,
                                                              Boolean?  AllowDowngrades = false)
        {

            if (Tariff is null)
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff must not be null!");

            if (TariffPatch is null || !TariffPatch.HasValues)
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (tariffs)
            {

                if (tariffs.TryGetValue(Tariff.Id, out var tariff))
                {

                    var patchResult = tariff.TryPatch(TariffPatch,
                                                      AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        tariffs[Tariff.Id] = patchResult.PatchedData;

                        var OnTariffChangedLocal = OnTariffChanged;
                        if (OnTariffChangedLocal is not null)
                        {
                            try
                            {
                                OnTariffChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchTariff), " ", nameof(OnTariffChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }


                    return patchResult;

                }

                else
                    return PatchResult<Tariff>.Failed(Tariff,
                                                      "The given charging tariff does not exist!");

            }

        }

        #endregion


        #region TariffExists(TariffId)

        public Boolean TariffExists(Tariff_Id TariffId)
        {

            lock (tariffs)
            {
                return tariffs.ContainsKey(TariffId);
            }

        }

        #endregion

        #region TryGetTariff(TariffId, out Tariff)

        public Boolean TryGetTariff(Tariff_Id    TariffId,
                                    out Tariff?  Tariff)
        {

            lock (tariffs)
            {

                if (tariffs.TryGetValue(TariffId, out Tariff))
                    return true;

                Tariff = null;
                return false;

            }

        }

        #endregion

        #region GetTariffs  (IncludeTariff = null)

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, Boolean>? IncludeTariff = null)
        {

            lock (tariffs)
            {

                return IncludeTariff is null
                           ? tariffs.Values.                     ToArray()
                           : tariffs.Values.Where(IncludeTariff).ToArray();

            }

        }

        #endregion

        #region GetTariffs  (CountryCode, PartyId)

        public IEnumerable<Tariff> GetTariffs(CountryCode  CountryCode,
                                              Party_Id     PartyId)
        {

            lock (tariffs)
            {

                return tariffs.Values.Where(tariff => tariff.CountryCode == CountryCode &&
                                                      tariff.PartyId     == PartyId).
                                      ToArray();

            }

        }

        #endregion


        #region RemoveTariff    (Tariff)

        public Boolean RemoveTariff(Tariff Tariff)
        {

            lock (tariffs)
            {

                return tariffs.Remove(Tariff.Id);

            }

        }

        #endregion

        #region RemoveTariff    (TariffId)

        public Boolean RemoveTariff(Tariff_Id TariffId)
        {

            lock (tariffs)
            {

                return tariffs.Remove(TariffId);

            }

        }

        #endregion

        #region RemoveAllTariffs(IncludeTariffs = null)

        /// <summary>
        /// Remove all matching tariffs.
        /// </summary>
        /// <param name="IncludeSessions">An optional charging session filter.</param>
        public void RemoveAllTariffs(Func<Tariff, Boolean>? IncludeTariffs = null)
        {

            lock (tariffs)
            {

                if (IncludeTariffs is null)
                    tariffs.Clear();

                else
                {

                    var tariffsToDelete = tariffs.Values.Where(IncludeTariffs).ToArray();

                    foreach (var tariff in tariffsToDelete)
                        tariffs.Remove(tariff.Id);

                }

            }

        }

        #endregion

        #region RemoveAllTariffs(CountryCode, PartyId)

        /// <summary>
        /// Remove all charging tariffs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllTariffs(CountryCode  CountryCode,
                                     Party_Id     PartyId)
        {

            lock (tariffs)
            {

                var tariffsToDelete = tariffs.Values.Where  (tariffs => CountryCode == tariffs.CountryCode &&
                                                                        PartyId     == tariffs.PartyId).
                                                    ToArray();

                foreach (var tariff in tariffsToDelete)
                    tariffs.Remove(tariff.Id);

            }

        }

        #endregion

        #endregion

        #region Sessions

        private readonly Dictionary<Session_Id , Session> chargingSessions;


        public delegate Task OnSessionCreatedDelegate(Session Session);

        public event OnSessionCreatedDelegate? OnSessionCreated;

        public delegate Task OnSessionChangedDelegate(Session Session);

        public event OnSessionChangedDelegate? OnSessionChanged;


        #region AddSession           (Session)

        public Session AddSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (chargingSessions)
            {

                if (!chargingSessions.ContainsKey(Session.Id))
                {

                    chargingSessions.Add(Session.Id, Session);

                    var OnSessionCreatedLocal = OnSessionCreated;
                    if (OnSessionCreatedLocal is not null)
                    {
                        try
                        {
                            OnSessionCreatedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddSession), " ", nameof(OnSessionCreated), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Session;

                }

                throw new ArgumentException("The given session already exists!");

            }

        }

        #endregion

        #region AddSessionIfNotExists(Session)

        public Session AddSessionIfNotExists(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (chargingSessions)
            {

                if (!chargingSessions.ContainsKey(Session.Id))
                {

                    chargingSessions.Add(Session.Id, Session);

                    var OnSessionCreatedLocal = OnSessionCreated;
                    if (OnSessionCreatedLocal is not null)
                    {
                        try
                        {
                            OnSessionCreatedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddSessionIfNotExists), " ", nameof(OnSessionCreated), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                }

                return Session;

            }

        }

        #endregion

        #region AddOrUpdateSession   (newOrUpdatedSession, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Session>> AddOrUpdateSession(Session   newOrUpdatedSession,
                                                                         Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedSession is null)
                throw new ArgumentNullException(nameof(newOrUpdatedSession), "The given charging session must not be null!");

            lock (chargingSessions)
            {

                if (chargingSessions.TryGetValue(newOrUpdatedSession.Id, out Session existingSession))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedSession.LastUpdated <= existingSession.LastUpdated)
                    {
                        return AddOrUpdateResult<Session>.Failed(newOrUpdatedSession,
                                                                 "The 'lastUpdated' timestamp of the new charging session must be newer then the timestamp of the existing session!");
                    }

                    chargingSessions[newOrUpdatedSession.Id] = newOrUpdatedSession;

                    var OnSessionChangedLocal = OnSessionChanged;
                    if (OnSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnSessionChangedLocal(newOrUpdatedSession).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return AddOrUpdateResult<Session>.Success(newOrUpdatedSession,
                                                              WasCreated: false);

                }

                chargingSessions.Add(newOrUpdatedSession.Id, newOrUpdatedSession);

                var OnSessionCreatedLocal = OnSessionCreated;
                if (OnSessionCreatedLocal is not null)
                {
                    try
                    {
                        OnSessionCreatedLocal(newOrUpdatedSession).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateSession), " ", nameof(OnSessionCreated), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Session>.Success(newOrUpdatedSession,
                                                          WasCreated: true);

            }

        }

        #endregion

        #region UpdateSession        (Session)

        public Session UpdateSession(Session Session)
        {

            if (Session is null)
                throw new ArgumentNullException(nameof(Session), "The given session must not be null!");

            lock (chargingSessions)
            {

                if (chargingSessions.ContainsKey(Session.Id))
                {

                    chargingSessions[Session.Id] = Session;

                    var OnSessionChangedLocal = OnSessionChanged;
                    if (OnSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnSessionChangedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Session;

                }

                return null;

            }

        }

        #endregion


        #region TryPatchSession      (Session, SessionPatch, AllowDowngrades = false)

        public async Task<PatchResult<Session>> TryPatchSession(Session   Session,
                                                                JObject   SessionPatch,
                                                                Boolean?  AllowDowngrades = false)
        {

            if (Session is null)
                return PatchResult<Session>.Failed(Session,
                                                   "The given charging session must not be null!");

            if (SessionPatch is null || !SessionPatch.HasValues)
                return PatchResult<Session>.Failed(Session,
                                                   "The given charging session patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (chargingSessions)
            {

                if (chargingSessions.TryGetValue(Session.Id, out var session))
                {

                    var patchResult = session.TryPatch(SessionPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        chargingSessions[Session.Id] = patchResult.PatchedData;

                        var OnSessionChangedLocal = OnSessionChanged;
                        if (OnSessionChangedLocal is not null)
                        {
                            try
                            {
                                OnSessionChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchSession), " ", nameof(OnSessionChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                    return patchResult;

                }

                else
                    return PatchResult<Session>.Failed(Session,
                                                       "The given charging session does not exist!");

            }

        }

        #endregion


        #region SessionExists(SessionId)

        public Boolean SessionExists(Session_Id SessionId)
        {

            lock (chargingSessions)
            {

                return chargingSessions.ContainsKey(SessionId);

            }

        }

        #endregion

        #region TryGetSession(CountryCode, PartyId, SessionId, out Session)

        public Boolean TryGetSession(Session_Id    SessionId,
                                     out Session?  Session)
        {

            lock (chargingSessions)
            {
                if (chargingSessions.TryGetValue(SessionId, out Session))
                    return true;

                Session = null;
                return false;

            }

        }

        #endregion

        #region GetSessions  (IncludeSession = null)

        public IEnumerable<Session> GetSessions(Func<Session, Boolean>? IncludeSession = null)
        {

            lock (chargingSessions)
            {

                return IncludeSession is null
                           ? chargingSessions.Values.                      ToArray()
                           : chargingSessions.Values.Where(IncludeSession).ToArray();

            }

        }

        #endregion

        #region GetSessions  (CountryCode, PartyId)

        public IEnumerable<Session> GetSessions(CountryCode  CountryCode,
                                                Party_Id     PartyId)
        {

            lock (chargingSessions)
            {

                return chargingSessions.Values.Where(session => session.CountryCode == CountryCode &&
                                                                session.PartyId     == PartyId).
                                               ToArray();

            }

        }

        #endregion


        #region RemoveSession    (Session)

        public Boolean RemoveSession(Session Session)
        {

            lock (chargingSessions)
            {

                return chargingSessions.Remove(Session.Id);

            }

        }

        #endregion

        #region RemoveSession    (SessionId)

        public Boolean RemoveSession(Session_Id SessionId)
        {

            lock (chargingSessions)
            {

                return chargingSessions.Remove(SessionId);

            }

        }

        #endregion

        #region RemoveAllSessions(IncludeSessions = null)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessions">An optional charging session filter.</param>
        public void RemoveAllSessions(Func<Session, Boolean>? IncludeSessions = null)
        {

            lock (chargingSessions)
            {

                if (IncludeSessions is null)
                    chargingSessions.Clear();

                else
                {

                    var sessionsToDelete = chargingSessions.Values.Where(IncludeSessions).ToArray();

                    foreach (var session in sessionsToDelete)
                        chargingSessions.Remove(session.Id);

                }

            }

        }

        #endregion

        #region RemoveAllSessions(CountryCode, PartyId)

        /// <summary>
        /// Remove all charging sessions owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllSessions(CountryCode  CountryCode,
                                      Party_Id     PartyId)
        {

            lock (chargingSessions)
            {

                var sessionsToDelete = chargingSessions.Values.Where  (session => CountryCode == session.CountryCode &&
                                                                                  PartyId     == session.PartyId).
                                                               ToArray();

                foreach (var session in sessionsToDelete)
                    chargingSessions.Remove(session.Id);

            }

        }

        #endregion

        #endregion

        #region Tokens

        private readonly Dictionary<Token_Id, TokenStatus> tokenStatus;


        public delegate Task OnTokenAddedDelegate(Token Token);

        public event OnTokenAddedDelegate? OnTokenAdded;


        public delegate Task OnTokenChangedDelegate(Token Token);

        public event OnTokenChangedDelegate? OnTokenChanged;


        public delegate Task<TokenStatus> OnVerifyTokenDelegate(Token_Id TokenId);

        public event OnVerifyTokenDelegate? OnVerifyToken;


        #region AddToken           (Token, Status = AllowedTypes.ALLOWED)

        public Token AddToken(Token         Token,
                              AllowedType?  Status = null)
        {

            Status ??= AllowedType.ALLOWED;

            lock (tokenStatus)
            {

                if (!tokenStatus.ContainsKey(Token.Id))
                {

                    tokenStatus.Add(Token.Id, new TokenStatus(Token, Status.Value));

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddToken), " ", nameof(OnTokenAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return Token;

                }

                throw new ArgumentException("The given token already exists!");

            }

        }

        #endregion

        #region AddTokenIfNotExists(Token, Status = AllowedTypes.ALLOWED)

        public Token AddTokenIfNotExists(Token         Token,
                                         AllowedType?  Status = null)
        {

            Status ??= AllowedType.ALLOWED;

            lock (tokenStatus)
            {

                if (!tokenStatus.ContainsKey(Token.Id))
                {

                    tokenStatus.Add(Token.Id, new TokenStatus(Token, Status.Value));

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddTokenIfNotExists), " ", nameof(OnTokenAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                }

                return Token;

            }

        }

        #endregion

        #region AddOrUpdateToken   (newOrUpdatedToken, Status = AllowedTypes.ALLOWED, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Token>> AddOrUpdateToken(Token         newOrUpdatedToken,
                                                                     AllowedType?  Status           = null,
                                                                     Boolean?      AllowDowngrades  = false)
        {

            Status ??= AllowedType.ALLOWED;

            if (newOrUpdatedToken is null)
                throw new ArgumentNullException(nameof(newOrUpdatedToken), "The given token must not be null!");

            lock (tokenStatus)
            {

                if (tokenStatus.TryGetValue(newOrUpdatedToken.Id, out TokenStatus existingTokenStatus))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedToken.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                    {
                        return AddOrUpdateResult<Token>.Failed(newOrUpdatedToken,
                                                               "The 'lastUpdated' timestamp of the new charging token must be newer then the timestamp of the existing token!");
                    }

                    tokenStatus[newOrUpdatedToken.Id] = new TokenStatus(newOrUpdatedToken,
                                                                   Status.Value);

                    var OnTokenChangedLocal = OnTokenChanged;
                    if (OnTokenChangedLocal is not null)
                    {
                        try
                        {
                            OnTokenChangedLocal(newOrUpdatedToken).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateToken), " ", nameof(OnTokenChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return AddOrUpdateResult<Token>.Success(newOrUpdatedToken,
                                                            WasCreated: false);

                }

                tokenStatus.Add(newOrUpdatedToken.Id, new TokenStatus(newOrUpdatedToken,
                                                                 Status.Value));

                var OnTokenAddedLocal = OnTokenAdded;
                if (OnTokenAddedLocal is not null)
                {
                    try
                    {
                        OnTokenAddedLocal(newOrUpdatedToken).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateToken), " ", nameof(OnTokenAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<Token>.Success(newOrUpdatedToken,
                                                        WasCreated: true);

            }

        }

        #endregion


        #region TryPatchToken      (Token, TokenPatch, AllowDowngrades = false)

        public async Task<PatchResult<Token>> TryPatchToken(Token     Token,
                                                            JObject   TokenPatch,
                                                            Boolean?  AllowDowngrades = false)
        {

            if (Token is null)
                return PatchResult<Token>.Failed(Token,
                                                 "The given token must not be null!");

            if (TokenPatch is null || !TokenPatch.HasValues)
                return PatchResult<Token>.Failed(Token,
                                                 "The given token patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            lock (tokenStatus)
            {

                if (tokenStatus.TryGetValue(Token.Id, out var _tokenStatus))
                {

                    var patchResult = _tokenStatus.Token.TryPatch(TokenPatch,
                                                                  AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        tokenStatus[Token.Id] = new TokenStatus(patchResult.PatchedData,
                                                                _tokenStatus.Status);

                        var OnTokenChangedLocal = OnTokenChanged;
                        if (OnTokenChangedLocal is not null)
                        {
                            try
                            {
                                OnTokenChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(TryPatchToken), " ", nameof(OnTokenChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace);
                            }
                        }

                    }

                    return patchResult;

                }

                else
                    return PatchResult<Token>.Failed(Token,
                                                      "The given charging token does not exist!");

            }

        }

        #endregion


        #region TokenExists(TokenId)

        public Boolean TokenExists(Token_Id TokenId)
        {

            lock (tokenStatus)
            {
                return tokenStatus.ContainsKey(TokenId);
            }

        }

        #endregion

        #region TryGetToken(TokenId, out TokenWithStatus)

        public Boolean TryGetToken(Token_Id         TokenId,
                                   out TokenStatus  TokenWithStatus)
        {

            lock (tokenStatus)
            {

                if (tokenStatus.TryGetValue(TokenId, out TokenWithStatus))
                    return true;

                var VerifyTokenLocal = OnVerifyToken;
                if (VerifyTokenLocal is not null)
                {

                    try
                    {

                        var result = VerifyTokenLocal(TokenId).Result;

                        TokenWithStatus = result;

                        return true;

                    } catch (Exception e)
                    {

                    }

                }

                TokenWithStatus = default;
                return false;

            }

        }

        #endregion

        #region GetTokens  (IncludeToken)

        public IEnumerable<TokenStatus> GetTokens(Func<Token, Boolean> IncludeToken)
        {

            lock (tokenStatus)
            {

                return tokenStatus.Values.Where  (tokenStatus => IncludeToken(tokenStatus.Token)).
                                          ToArray();

            }

        }

        #endregion

        #region GetTokens  (IncludeTokenStatus = null)

        public IEnumerable<TokenStatus> GetTokens(Func<TokenStatus, Boolean>? IncludeTokenStatus = null)
        {

            lock (tokenStatus)
            {

                return IncludeTokenStatus is null
                           ? tokenStatus.Values.                          ToArray()
                           : tokenStatus.Values.Where(IncludeTokenStatus).ToArray();

            }

        }

        #endregion

        #region GetTokens  (CountryCode, PartyId)

        public IEnumerable<TokenStatus> GetTokens(CountryCode  CountryCode,
                                                  Party_Id     PartyId)
        {

            lock (tokenStatus)
            {

                return tokenStatus.Values.Where  (tokenStatus => tokenStatus.Token.CountryCode == CountryCode &&
                                                                 tokenStatus.Token.PartyId     == PartyId).
                                          ToArray();

            }

        }

        #endregion


        #region RemoveToken    (Token)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="Token">A token.</param>
        public Boolean RemoveToken(Token Token)
        {

            lock (tokenStatus)
            {

                return tokenStatus.Remove(Token.Id);

            }

        }

        #endregion

        #region RemoveToken    (TokenId)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="TokenId">A unique identification of a token.</param>
        public Boolean RemoveToken(Token_Id TokenId)
        {

            lock (tokenStatus)
            {

                return tokenStatus.Remove(TokenId);

            }

        }

        #endregion

        #region RemoveAllTokens(IncludeTokens = null)

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        /// <param name="IncludeCDRs">An optional token filter.</param>
        public void RemoveAllTokens(Func<Token, Boolean>? IncludeTokens = null)
        {

            lock (tokenStatus)
            {

                if (IncludeTokens is null)
                    tokenStatus.Clear();

                else
                {

                    var tokensToDelete = tokenStatus.Values.Where  (tokenStatus => IncludeTokens(tokenStatus.Token)).
                                                            Select (tokenStatus => tokenStatus.Token).
                                                            ToArray();

                    foreach (var token in tokensToDelete)
                        tokenStatus.Remove(token.Id);

                }

            }

        }

        #endregion

        #region RemoveAllTokens(CountryCode, PartyId)

        /// <summary>
        /// Remove all tokens owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllTokens(CountryCode  CountryCode,
                                    Party_Id     PartyId)
        {

            lock (tokenStatus)
            {

                var tokensToDelete = tokenStatus.Values.Where  (tokenStatus => CountryCode == tokenStatus.Token.CountryCode &&
                                                                               PartyId     == tokenStatus.Token.PartyId).
                                                        Select (tokenStatus => tokenStatus.Token).
                                                        ToArray();

                foreach (var token in tokensToDelete)
                    tokenStatus.Remove(token.Id);

            }

        }

        #endregion

        #endregion

        #region ChargeDetailRecords

        private readonly Dictionary<CDR_Id, CDR> chargeDetailRecords;


        public delegate Task OnChargeDetailRecordAddedDelegate(CDR CDR);

        public event OnChargeDetailRecordAddedDelegate? OnChargeDetailRecordAdded;


        public delegate Task OnChargeDetailRecordChangedDelegate(CDR CDR);

        public event OnChargeDetailRecordChangedDelegate? OnChargeDetailRecordChanged;


        #region AddCDR           (CDR)

        public CDR AddCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (chargeDetailRecords)
            {

                if (!chargeDetailRecords.ContainsKey(CDR.Id))
                {

                    chargeDetailRecords.Add(CDR.Id, CDR);

                    var OnChargeDetailRecordAddedLocal = OnChargeDetailRecordAdded;
                    if (OnChargeDetailRecordAddedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddCDR), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return CDR;

                }

                throw new ArgumentException("The given charge detail record already exists!");

            }

        }

        #endregion

        #region AddCDRIfNotExists(CDR)

        public CDR AddCDRIfNotExists(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (chargeDetailRecords)
            {

                if (!chargeDetailRecords.ContainsKey(CDR.Id))
                {

                    chargeDetailRecords.Add(CDR.Id, CDR);

                    var OnChargeDetailRecordAddedLocal = OnChargeDetailRecordAdded;
                    if (OnChargeDetailRecordAddedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddCDRIfNotExists), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                }

                return CDR;

            }

        }

        #endregion

        #region AddOrUpdateCDR   (newOrUpdatedCDR, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<CDR>> AddOrUpdateCDR(CDR       newOrUpdatedCDR,
                                                                 Boolean?  AllowDowngrades = false)
        {

            if (newOrUpdatedCDR is null)
                throw new ArgumentNullException(nameof(newOrUpdatedCDR), "The given charge detail record must not be null!");

            lock (chargeDetailRecords)
            {

                if (chargeDetailRecords.TryGetValue(newOrUpdatedCDR.Id, out var existingCDR))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedCDR.LastUpdated <= existingCDR.LastUpdated)
                    {
                        return AddOrUpdateResult<CDR>.Failed(newOrUpdatedCDR,
                                                             "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");
                    }

                    chargeDetailRecords[newOrUpdatedCDR.Id] = newOrUpdatedCDR;

                    var OnChargeDetailRecordChangedLocal = OnChargeDetailRecordChanged;
                    if (OnChargeDetailRecordChangedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordChangedLocal(newOrUpdatedCDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateCDR), " ", nameof(OnChargeDetailRecordChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return AddOrUpdateResult<CDR>.Success(newOrUpdatedCDR,
                                                          WasCreated: false);

                }

                chargeDetailRecords.Add(newOrUpdatedCDR.Id, newOrUpdatedCDR);

                var OnChargeDetailRecordAddedLocal = OnChargeDetailRecordAdded;
                if (OnChargeDetailRecordAddedLocal is not null)
                {
                    try
                    {
                        OnChargeDetailRecordAddedLocal(newOrUpdatedCDR).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(AddOrUpdateCDR), " ", nameof(OnChargeDetailRecordAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace);
                    }
                }

                return AddOrUpdateResult<CDR>.Success(newOrUpdatedCDR,
                                                      WasCreated: true);

            }

        }

        #endregion

        #region UpdateCDR        (CDR)

        public CDR UpdateCDR(CDR CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            lock (chargeDetailRecords)
            {

                if (chargeDetailRecords.ContainsKey(CDR.Id))
                {

                    chargeDetailRecords[CDR.Id] = CDR;

                    var OnChargeDetailRecordChangedLocal = OnChargeDetailRecordChanged;
                    if (OnChargeDetailRecordChangedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordChangedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT("OCPI v", Version.Number, " ", nameof(CommonAPI), " ", nameof(UpdateCDR), " ", nameof(OnChargeDetailRecordChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace);
                        }
                    }

                    return CDR;

                }

                return null;

            }

        }

        #endregion


        #region CDRExists(CDRId)

        public Boolean CDRExists(CDR_Id CDRId)
        {

            lock (chargeDetailRecords)
            {
                return chargeDetailRecords.ContainsKey(CDRId);
            }

        }

        #endregion

        #region TryGetCDR(CDRId, out CDR)

        public Boolean TryGetCDR(CDR_Id    CDRId,
                                 out CDR?  CDR)
        {

            lock (chargeDetailRecords)
            {

                if (chargeDetailRecords.TryGetValue(CDRId, out CDR))
                    return true;

                CDR = null;
                return false;

            }

        }

        #endregion

        #region GetCDRs  (IncludeCDRs = null)

        /// <summary>
        /// Return all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">An optional charge detail record filter.</param>
        public IEnumerable<CDR> GetCDRs(Func<CDR, Boolean>? IncludeCDRs = null)
        {

            lock (chargeDetailRecords)
            {

                return IncludeCDRs is null
                           ? chargeDetailRecords.Values.                   ToArray()
                           : chargeDetailRecords.Values.Where(IncludeCDRs).ToArray();

            }

        }

        #endregion

        #region GetCDRs  (CountryCode, PartyId)

        /// <summary>
        /// Return all charge detail records owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public IEnumerable<CDR> GetCDRs(CountryCode  CountryCode,
                                        Party_Id     PartyId)
        {

            lock (chargeDetailRecords)
            {

                return chargeDetailRecords.Values.Where  (cdr => cdr.CountryCode == CountryCode &&
                                                                 cdr.PartyId     == PartyId).
                                                  ToArray();

            }

        }

        #endregion


        #region RemoveCDR    (CDR)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDR">A charge detail record.</param>
        public Boolean RemoveCDR(CDR CDR)
        {

            lock (chargeDetailRecords)
            {

                return chargeDetailRecords.Remove(CDR.Id);

            }

        }

        #endregion

        #region RemoveCDR    (CDRId)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDRId">A unique identification of a charge detail record.</param>
        public Boolean RemoveCDR(CDR_Id CDRId)
        {

            lock (chargeDetailRecords)
            {

                return chargeDetailRecords.Remove(CDRId);

            }

        }

        #endregion

        #region RemoveAllCDRs(IncludeCDRs = null)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">An optional charge detail record filter.</param>
        public void RemoveAllCDRs(Func<CDR, Boolean>? IncludeCDRs = null)
        {

            lock (chargeDetailRecords)
            {

                if (IncludeCDRs is null)
                    chargeDetailRecords.Clear();

                else
                {

                    var cdrsToDelete = chargeDetailRecords.Values.Where(IncludeCDRs).ToArray();

                    foreach (var cdr in cdrsToDelete)
                        chargeDetailRecords.Remove(cdr.Id);

                }

            }

        }

        #endregion

        #region RemoveAllCDRs(CountryCode, PartyId)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public void RemoveAllCDRs(CountryCode  CountryCode,
                                  Party_Id     PartyId)
        {

            lock (chargeDetailRecords)
            {

                var sessionsToDelete = chargingSessions.Values.Where  (cdr => CountryCode == cdr.CountryCode &&
                                                                              PartyId     == cdr.PartyId).
                                                               ToArray();

                foreach (var session in sessionsToDelete)
                    chargingSessions.Remove(session.Id);

            }

        }

        #endregion

        #endregion



        #region Start()

        public void Start()
        {

            lock (HTTPServer)
            {

                if (!HTTPServer.IsStarted)
                    HTTPServer.Start();

                #region Send 'Open Data API restarted'-e-mail...

                //var Message0 = new HTMLEMailBuilder() {
                //    From        = _APIEMailAddress,
                //    To          = _APIAdminEMail,
                //    Subject     = "Open Data API '" + _ServiceName + "' restarted! at " + Timestamp.Now.ToString(),
                //    PlainText   = "Open Data API '" + _ServiceName + "' restarted! at " + Timestamp.Now.ToString(),
                //    HTMLText    = "Open Data API <b>'" + _ServiceName + "'</b> restarted! at " + Timestamp.Now.ToString(),
                //    Passphrase  = _APIPassphrase
                //};
                //
                //var SMTPTask = _APISMTPClient.Send(Message0);
                //SMTPTask.Wait();

                //var r = SMTPTask.Result;

                #endregion

                //SendStarted(this, Timestamp.Now);

            }

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        public new void Shutdown(String?  Message   = null,
                                 Boolean  Wait      = true)
        {

            lock (HTTPServer)
            {

                HTTPServer.Shutdown(Message,
                                    Wait);

                //SendCompleted(this, Timestamp.Now, Message);

            }

        }

        #endregion

    }

}
