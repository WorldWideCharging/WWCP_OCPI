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

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.HTTP
{

    #region OnGetVersionsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get versions request will be send.
    /// </summary>
    public delegate Task OnGetVersionsRequestDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get versions request had been received.
    /// </summary>
    public delegate Task OnGetVersionsResponseDelegate(DateTime                                    LogTimestamp,
                                                       DateTime                                    RequestTimestamp,
                                                       CommonClient                                Sender,
                                                       String                                      SenderId,
                                                       EventTracking_Id                            EventTrackingId,

                                                       //Partner_Id                                  PartnerId,
                                                       //Operator_Id                                 OperatorId,
                                                       //ChargingPool_Id                             ChargingPoolId,
                                                       //DateTime                                    StatusEventDate,
                                                       //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                       //Transaction_Id?                             TransactionId,
                                                       //DateTime?                                   AvailabilityStatusUntil,
                                                       //String                                      AvailabilityStatusComment,

                                                       TimeSpan                                    RequestTimeout,
                                                       //SetChargingPoolAvailabilityStatusResponse   Result,
                                                       TimeSpan                                    Duration);

    #endregion

    #region OnGetVersionDetailsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get version details request will be send.
    /// </summary>
    public delegate Task OnGetVersionDetailsRequestDelegate(DateTime                                    LogTimestamp,
                                                            DateTime                                    RequestTimestamp,
                                                            CommonClient                                Sender,
                                                            String                                      SenderId,
                                                            EventTracking_Id                            EventTrackingId,

                                                            //Partner_Id                                  PartnerId,
                                                            //Operator_Id                                 OperatorId,
                                                            //ChargingPool_Id                             ChargingPoolId,
                                                            //DateTime                                    StatusEventDate,
                                                            //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                            //Transaction_Id?                             TransactionId,
                                                            //DateTime?                                   AvailabilityStatusUntil,
                                                            //String                                      AvailabilityStatusComment,

                                                            TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get version details request had been received.
    /// </summary>
    public delegate Task OnGetVersionDetailsResponseDelegate(DateTime                                    LogTimestamp,
                                                             DateTime                                    RequestTimestamp,
                                                             CommonClient                                Sender,
                                                             String                                      SenderId,
                                                             EventTracking_Id                            EventTrackingId,

                                                             //Partner_Id                                  PartnerId,
                                                             //Operator_Id                                 OperatorId,
                                                             //ChargingPool_Id                             ChargingPoolId,
                                                             //DateTime                                    StatusEventDate,
                                                             //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                             //Transaction_Id?                             TransactionId,
                                                             //DateTime?                                   AvailabilityStatusUntil,
                                                             //String                                      AvailabilityStatusComment,

                                                             TimeSpan                                    RequestTimeout,
                                                             //SetChargingPoolAvailabilityStatusResponse   Result,
                                                             TimeSpan                                    Duration);

    #endregion


    #region OnGetCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get credentials request will be send.
    /// </summary>
    public delegate Task OnGetCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                         DateTime                                    RequestTimestamp,
                                                         CommonClient                                Sender,
                                                         String                                      SenderId,
                                                         EventTracking_Id                            EventTrackingId,

                                                         //Partner_Id                                  PartnerId,
                                                         //Operator_Id                                 OperatorId,
                                                         //ChargingPool_Id                             ChargingPoolId,
                                                         //DateTime                                    StatusEventDate,
                                                         //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                         //Transaction_Id?                             TransactionId,
                                                         //DateTime?                                   AvailabilityStatusUntil,
                                                         //String                                      AvailabilityStatusComment,

                                                         TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get credentials request had been received.
    /// </summary>
    public delegate Task OnGetCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          //Partner_Id                                  PartnerId,
                                                          //Operator_Id                                 OperatorId,
                                                          //ChargingPool_Id                             ChargingPoolId,
                                                          //DateTime                                    StatusEventDate,
                                                          //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                          //Transaction_Id?                             TransactionId,
                                                          //DateTime?                                   AvailabilityStatusUntil,
                                                          //String                                      AvailabilityStatusComment,

                                                          TimeSpan                                    RequestTimeout,
                                                          //SetChargingPoolAvailabilityStatusResponse   Result,
                                                          TimeSpan                                    Duration);

    #endregion

    #region OnPostCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a put credentials request will be send.
    /// </summary>
    public delegate Task OnPostCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          //Partner_Id                                  PartnerId,
                                                          //Operator_Id                                 OperatorId,
                                                          //ChargingPool_Id                             ChargingPoolId,
                                                          //DateTime                                    StatusEventDate,
                                                          //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                          //Transaction_Id?                             TransactionId,
                                                          //DateTime?                                   AvailabilityStatusUntil,
                                                          //String                                      AvailabilityStatusComment,

                                                          TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put credentials request had been received.
    /// </summary>
    public delegate Task OnPostCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                           DateTime                                    RequestTimestamp,
                                                           CommonClient                                Sender,
                                                           String                                      SenderId,
                                                           EventTracking_Id                            EventTrackingId,

                                                           //Partner_Id                                  PartnerId,
                                                           //Operator_Id                                 OperatorId,
                                                           //ChargingPool_Id                             ChargingPoolId,
                                                           //DateTime                                    StatusEventDate,
                                                           //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                           //Transaction_Id?                             TransactionId,
                                                           //DateTime?                                   AvailabilityStatusUntil,
                                                           //String                                      AvailabilityStatusComment,

                                                           TimeSpan                                    RequestTimeout,
                                                           //SetChargingPoolAvailabilityStatusResponse   Result,
                                                           TimeSpan                                    Duration);

    #endregion

    #region OnPutCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a put credentials request will be send.
    /// </summary>
    public delegate Task OnPutCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                         DateTime                                    RequestTimestamp,
                                                         CommonClient                                Sender,
                                                         String                                      SenderId,
                                                         EventTracking_Id                            EventTrackingId,

                                                         //Partner_Id                                  PartnerId,
                                                         //Operator_Id                                 OperatorId,
                                                         //ChargingPool_Id                             ChargingPoolId,
                                                         //DateTime                                    StatusEventDate,
                                                         //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                         //Transaction_Id?                             TransactionId,
                                                         //DateTime?                                   AvailabilityStatusUntil,
                                                         //String                                      AvailabilityStatusComment,

                                                         TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put credentials request had been received.
    /// </summary>
    public delegate Task OnPutCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          //Partner_Id                                  PartnerId,
                                                          //Operator_Id                                 OperatorId,
                                                          //ChargingPool_Id                             ChargingPoolId,
                                                          //DateTime                                    StatusEventDate,
                                                          //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                          //Transaction_Id?                             TransactionId,
                                                          //DateTime?                                   AvailabilityStatusUntil,
                                                          //String                                      AvailabilityStatusComment,

                                                          TimeSpan                                    RequestTimeout,
                                                          //SetChargingPoolAvailabilityStatusResponse   Result,
                                                          TimeSpan                                    Duration);

    #endregion

    #region OnDeleteCredentialsRequest/-Response

    /// <summary>
    /// A delegate called whenever a put credentials request will be send.
    /// </summary>
    public delegate Task OnDeleteCredentialsRequestDelegate(DateTime                                    LogTimestamp,
                                                            DateTime                                    RequestTimestamp,
                                                            CommonClient                                Sender,
                                                            String                                      SenderId,
                                                            EventTracking_Id                            EventTrackingId,

                                                            //Partner_Id                                  PartnerId,
                                                            //Operator_Id                                 OperatorId,
                                                            //ChargingPool_Id                             ChargingPoolId,
                                                            //DateTime                                    StatusEventDate,
                                                            //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                            //Transaction_Id?                             TransactionId,
                                                            //DateTime?                                   AvailabilityStatusUntil,
                                                            //String                                      AvailabilityStatusComment,

                                                            TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put credentials request had been received.
    /// </summary>
    public delegate Task OnDeleteCredentialsResponseDelegate(DateTime                                    LogTimestamp,
                                                             DateTime                                    RequestTimestamp,
                                                             CommonClient                                Sender,
                                                             String                                      SenderId,
                                                             EventTracking_Id                            EventTrackingId,

                                                             //Partner_Id                                  PartnerId,
                                                             //Operator_Id                                 OperatorId,
                                                             //ChargingPool_Id                             ChargingPoolId,
                                                             //DateTime                                    StatusEventDate,
                                                             //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                             //Transaction_Id?                             TransactionId,
                                                             //DateTime?                                   AvailabilityStatusUntil,
                                                             //String                                      AvailabilityStatusComment,

                                                             TimeSpan                                    RequestTimeout,
                                                             //SetChargingPoolAvailabilityStatusResponse   Result,
                                                             TimeSpan                                    Duration);

    #endregion


    #region OnRegisterRequest/-Response

    /// <summary>
    /// A delegate called whenever a registration request will be send.
    /// </summary>
    public delegate Task OnRegisterRequestDelegate (DateTime                                    LogTimestamp,
                                                    DateTime                                    RequestTimestamp,
                                                    CommonClient                                Sender,
                                                    String                                      SenderId,
                                                    EventTracking_Id                            EventTrackingId,
                                                   
                                                    //Partner_Id                                  PartnerId,
                                                    //Operator_Id                                 OperatorId,
                                                    //ChargingPool_Id                             ChargingPoolId,
                                                    //DateTime                                    StatusEventDate,
                                                    //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                    //Transaction_Id?                             TransactionId,
                                                    //DateTime?                                   AvailabilityStatusUntil,
                                                    //String                                      AvailabilityStatusComment,

                                                    TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a registration request had been received.
    /// </summary>
    public delegate Task OnRegisterResponseDelegate(DateTime                                    LogTimestamp,
                                                    DateTime                                    RequestTimestamp,
                                                    CommonClient                                Sender,
                                                    String                                      SenderId,
                                                    EventTracking_Id                            EventTrackingId,

                                                    //Partner_Id                                  PartnerId,
                                                    //Operator_Id                                 OperatorId,
                                                    //ChargingPool_Id                             ChargingPoolId,
                                                    //DateTime                                    StatusEventDate,
                                                    //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                    //Transaction_Id?                             TransactionId,
                                                    //DateTime?                                   AvailabilityStatusUntil,
                                                    //String                                      AvailabilityStatusComment,

                                                    TimeSpan                                    RequestTimeout,
                                                    //SetChargingPoolAvailabilityStatusResponse   Result,
                                                    TimeSpan                                    Duration);

    #endregion


    /// <summary>
    /// The OCPI common client.
    /// </summary>
    public partial class CommonClient : AHTTPClient
    {

        #region (class) CommonAPICounters

        public class CommonAPICounters
        {

            public APICounterValues  GetVersions    { get; }
            public APICounterValues  Register       { get; }

            public CommonAPICounters(APICounterValues?  GetVersions   = null,
                                     APICounterValues?  Register      = null)
            {

                this.GetVersions  = GetVersions ?? new APICounterValues();
                this.Register     = Register    ?? new APICounterValues();
            }

            public virtual JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("GetVersions",  GetVersions.ToJSON()),
                       new JProperty("Register",     Register.   ToJSON())
                   );

        }

        #endregion


        #region Data

        protected          HTTPTokenAuthentication                TokenAuth;

        protected readonly Dictionary<Version_Id, URL>            Versions         = new ();

        protected readonly Dictionary<Version_Id, VersionDetail>  VersionDetails   = new ();


        protected Newtonsoft.Json.Formatting JSONFormat = Newtonsoft.Json.Formatting.Indented;

        #endregion

        #region Properties

        /// <summary>
        /// The remote party.
        /// </summary>
        public RemoteParty                          RemoteParty                   { get; }

        /// <summary>
        /// The access token.
        /// (Might be updated during the REGISTRATION process!)
        /// </summary>
        public AccessToken                          AccessToken                   { get; private set; }

        /// <summary>
        /// The selected OCPI version.
        /// </summary>
        public Version_Id?                          SelectedOCPIVersionId         { get; set; }

        /// <summary>
        /// The remote URL of the VERSIONS endpoint to connect to.
        /// </summary>
        public URL                                  RemoteVersionsURL             { get; }

        /// <summary>
        /// My Common API.
        /// </summary>
        public CommonAPI                            MyCommonAPI                   { get; }

        /// <summary>
        /// CPO client event counters.
        /// </summary>
        public CommonAPICounters                       Counters                      { get; }

        /// <summary>
        /// The attached HTTP client logger.
        /// </summary>
        public new Logger?                          HTTPLogger
        {
            get
            {
                return base.HTTPLogger as Logger;
            }
            set
            {
                base.HTTPLogger = value;
            }
        }

        #endregion

        #region Events

        #region OnGetVersionsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all versions will be send.
        /// </summary>
        public event OnGetVersionsRequestDelegate?   OnGetVersionsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all versions will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetVersionsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all versions HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetVersionsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all versions request had been received.
        /// </summary>
        public event OnGetVersionsResponseDelegate?  OnGetVersionsResponse;

        #endregion

        #region OnGetVersionDetailsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting version details will be send.
        /// </summary>
        public event OnGetVersionsRequestDelegate?   OnGetVersionDetailsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting version details will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetVersionDetailsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting version details HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetVersionDetailsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting version details request had been received.
        /// </summary>
        public event OnGetVersionsResponseDelegate?  OnGetVersionDetailsResponse;

        #endregion


        #region OnGetCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting credentials will be send.
        /// </summary>
        public event OnGetCredentialsRequestDelegate?   OnGetCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?           OnGetCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?          OnGetCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting credentials request had been received.
        /// </summary>
        public event OnGetCredentialsResponseDelegate?  OnGetCredentialsResponse;

        #endregion

        #region OnPostCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request posting credentials will be send.
        /// </summary>
        public event OnPostCredentialsRequestDelegate?   OnPostCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request posting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?            OnPostCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a posting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?           OnPostCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a posting credentials request had been received.
        /// </summary>
        public event OnPostCredentialsResponseDelegate?  OnPostCredentialsResponse;

        #endregion

        #region OnPutCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting credentials will be send.
        /// </summary>
        public event OnPutCredentialsRequestDelegate?   OnPutCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?           OnPutCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?          OnPutCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting credentials request had been received.
        /// </summary>
        public event OnPutCredentialsResponseDelegate?  OnPutCredentialsResponse;

        #endregion

        #region OnDeleteCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request deleting credentials will be send.
        /// </summary>
        public event OnDeleteCredentialsRequestDelegate?   OnDeleteCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request deleting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?              OnDeleteCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a deleting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?             OnDeleteCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a deleting credentials request had been received.
        /// </summary>
        public event OnDeleteCredentialsResponseDelegate?  OnDeleteCredentialsResponse;

        #endregion


        #region OnRegisterRequest/-Response

        /// <summary>
        /// An event fired whenever a registration request will be send.
        /// </summary>
        public event OnRegisterRequestDelegate?   OnRegisterRequest;

        /// <summary>
        /// An event fired whenever a HTTP registration request will be send.
        /// </summary>
        public event ClientRequestLogHandler?     OnRegisterHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a HTTP registration request had been received.
        /// </summary>
        public event ClientResponseLogHandler?    OnRegisterHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a registration request had been received.
        /// </summary>
        public event OnRegisterResponseDelegate?  OnRegisterResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region CommonClient(RemoteParty, MyCommonAPI, ...)

        /// <summary>
        /// Create a new OCPI Common client.
        /// </summary>
        /// <param name="RemoteParty">The remote party.</param>
        /// <param name="MyCommonAPI">My Common API.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CommonClient(RemoteParty              RemoteParty,
                            CommonAPI                MyCommonAPI,
                            HTTPHostname?            VirtualHostname             = null,
                            String?                  Description                 = null,
                            HTTPClientLogger?        HTTPLogger                  = null,

                            Boolean?                 DisableLogging              = false,
                            String?                  LoggingPath                 = null,
                            String?                  LoggingContext              = null,
                            LogfileCreatorDelegate?  LogfileCreator              = null,
                            DNSClient?               DNSClient                   = null)

            : base(RemoteParty.RemoteAccessInfos.First().VersionsURL,
                   VirtualHostname,
                   Description,
                   RemoteParty.RemoteCertificateValidator,
                   RemoteParty.ClientCertificateSelector,
                   RemoteParty.ClientCert,
                   RemoteParty.TLSProtocol,
                   RemoteParty.PreferIPv4,
                   RemoteParty.HTTPUserAgent      ?? DefaultHTTPUserAgent,
                   RemoteParty.RequestTimeout,
                   RemoteParty.TransmissionRetryDelay,
                   RemoteParty.MaxNumberOfRetries ?? DefaultMaxNumberOfRetries,
                   RemoteParty.UseHTTPPipelining,
                   DisableLogging,
                   HTTPLogger,
                   DNSClient)

        {

            this.RemoteParty        = RemoteParty;
            this.AccessToken        = RemoteParty.RemoteAccessInfos.First().AccessToken;
            this.RemoteVersionsURL  = RemoteParty.RemoteAccessInfos.First().VersionsURL;
            this.TokenAuth          = new HTTPTokenAuthentication(RemoteParty.RemoteAccessInfos.First().AccessTokenIsBase64Encoded
                                                                      ? RemoteParty.RemoteAccessInfos.First().AccessToken.ToString().ToBase64()
                                                                      : RemoteParty.RemoteAccessInfos.First().AccessToken.ToString());
            this.MyCommonAPI        = MyCommonAPI;

            this.Counters           = new CommonAPICounters();

            base.HTTPLogger         = this.DisableLogging == false
                                          ? new Logger(
                                                this,
                                                LoggingPath,
                                                LoggingContext,
                                                LogfileCreator
                                            )
                                          : null;

        }

        #endregion

        #region CommonClient(RemoteVersionsURL, AccessToken, MyCommonAPI, ...)

        /// <summary>
        /// Create a new OCPI Common client.
        /// </summary>
        /// <param name="RemoteVersionsURL">The remote URL of the VERSIONS endpoint to connect to.</param>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="MyCommonAPI">My Common API.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CommonClient(URL                                   RemoteVersionsURL,
                            AccessToken                           AccessToken,
                            CommonAPI                             MyCommonAPI,
                            HTTPHostname?                         VirtualHostname              = null,
                            String?                               Description                  = null,
                            RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                            LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                            X509Certificate?                      ClientCert                   = null,
                            SslProtocols?                         TLSProtocol                  = null,
                            Boolean?                              PreferIPv4                   = null,
                            String?                               HTTPUserAgent                = null,
                            TimeSpan?                             RequestTimeout               = null,
                            TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                            UInt16?                               MaxNumberOfRetries           = null,
                            Boolean?                              UseHTTPPipelining            = null,
                            HTTPClientLogger?                     HTTPLogger                   = null,
                            Boolean                               AccessTokenBase64Encoding    = true,

                            Boolean?                              DisableLogging               = false,
                            String?                               LoggingPath                  = null,
                            String?                               LoggingContext               = null,
                            LogfileCreatorDelegate?               LogfileCreator               = null,
                            DNSClient?                            DNSClient                    = null)

            : base(RemoteVersionsURL,
                   VirtualHostname,
                   Description,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   PreferIPv4,
                   HTTPUserAgent      ?? DefaultHTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries ?? DefaultMaxNumberOfRetries,
                   UseHTTPPipelining,
                   DisableLogging,
                   HTTPLogger,
                   DNSClient)

        {

            this.RemoteParty        = new RemoteParty(
                                          Id:                          RemoteParty_Id.Unknown,
                                          Roles:                       Array.Empty<CredentialsRole>(),

                                          RemoteAccessToken:           AccessToken,
                                          RemoteVersionsURL:           RemoteVersionsURL,
                                          AccessTokenBase64Encoding:   AccessTokenBase64Encoding,
                                          RemoteStatus:                RemoteAccessStatus.ONLINE,
                                          Status:                      PartyStatus.ENABLED
                                      );

            this.TokenAuth          = new HTTPTokenAuthentication(AccessTokenBase64Encoding
                                                                      ? AccessToken.ToString().ToBase64()
                                                                      : AccessToken.ToString());
            this.AccessToken        = AccessToken;
            this.RemoteVersionsURL  = RemoteVersionsURL;
            this.MyCommonAPI        = MyCommonAPI;

            this.Counters           = new CommonAPICounters();

            base.HTTPLogger         = this.DisableLogging == false
                                          ? new Logger(this,
                                                       LoggingPath,
                                                       LoggingContext,
                                                       LogfileCreator)
                                          : null;

        }

        #endregion

        #endregion


        #region ToJSON()

        public virtual JObject ToJSON()
            => ToJSON(nameof(CommonClient));

        protected JObject ToJSON(String ClientType)
        {

            return JSONObject.Create(

                       //new JProperty("countryCode",              CountryCode.ToString()),
                       //new JProperty("partyId",                  PartyId.    ToString()),
                       //new JProperty("role",                     Role.       ToString()),

                       new JProperty("type",                     ClientType),

                       Description.IsNotNullOrEmpty()
                           ? new JProperty("description",        Description)
                           : null,

                       new JProperty("remoteVersionsURL",        RemoteVersionsURL.    ToString()),
                       new JProperty("accessToken",              AccessToken.          ToString()),

                       VirtualHostname.HasValue
                           ? new JProperty("virtualHostname",    VirtualHostname.Value.ToString())
                           : null,

                       new JProperty("requestTimeout",           RequestTimeout.TotalSeconds),

                       new JProperty("maxNumberOfRetries",       MaxNumberOfRetries),

                       Versions.SafeAny()
                           ? new JProperty("versions",           new JObject(Versions.Select(version => new JProperty(version.Key.  ToString(),
                                                                                                                      version.Value.ToString()))))
                           : null,

                       SelectedOCPIVersionId.HasValue
                           ? new JProperty("selectedVersionId",  SelectedOCPIVersionId.ToString())
                           : null,

                       VersionDetails.SafeAny()
                           ? new JProperty("versionDetails",     new JArray(VersionDetails.Values.Select(versionDetail => versionDetail.ToJSON())))
                           : null

                   );

        }

        #endregion


        #region GetVersions(...)

        /// <summary>
        /// Get versions.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<VersionInformation>>>

            GetVersions(Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken   CancellationToken   = default,
                        EventTracking_Id?   EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            #region Send OnGetVersionsRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                Counters.GetVersions.IncRequests_OK();

                //if (OnGetVersionsRequest != null)
                //    await Task.WhenAll(OnGetVersionsRequest.GetInvocationList().
                //                       Cast<OnGetVersionsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnGetVersionsRequest));
            }

            #endregion


            Byte                                          TransmissionRetry = 0;
            OCPIResponse<IEnumerable<VersionInformation>> response;

            do
            {

                try
                {

                    var requestId      = RequestId     ?? Request_Id.    NewRandom();
                    var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                    // ToDo: Add request logging!


                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(RemoteURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocol,
                                                             PreferIPv4,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                           Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                  RemoteVersionsURL.Path,
                                                                                  requestbuilder => {
                                                                                      //requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                      requestbuilder.Authorization = TokenAuth;
                                                                                      requestbuilder.Connection    = "close";
                                                                                      requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                      requestbuilder.Set("X-Request-ID",      requestId);
                                                                                      requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                  }),

                                                 RequestLogDelegate:   OnGetVersionsHTTPRequest,
                                                 ResponseLogDelegate:  OnGetVersionsHTTPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                           ConfigureAwait(false);

                    #endregion

                    #region Documentation

                    // {
                    //   "data": [
                    //     {
                    //       "version": "2.2",
                    //       "url":     "https://example.com/ocpi/versions/2.2/"
                    //     }
                    //   ],
                    //   "status_code": 1000,
                    //   "timestamp":  "2020-10-05T21:15:30.134Z"
                    // }

                    #endregion

                    response = OCPIResponse<VersionInformation>.ParseJArray(httpResponse,
                                                                            requestId,
                                                                            correlationId,
                                                                            json => VersionInformation.Parse(json));

                    switch (response.StatusCode)
                    {

                        case 1000:

                            lock (Versions)
                            {
                                Versions.Clear();
                                response.Data.ForEach(version => Versions.Add(version.Id, version.URL));
                            }

                            break;

                    }

                }

                catch (Exception e)
                {
                    response = OCPIResponse<IEnumerable<VersionInformation>>.Exception(e);
                }


            }
            while ((response.HTTPResponse.HTTPStatusCode.IsServerError ||
                    response.HTTPResponse.HTTPStatusCode == HTTPStatusCode.RequestTimeout) &&
                   TransmissionRetry++ < MaxNumberOfRetries);


            #region Send OnGetVersionsResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetVersionsResponse != null)
                //    await Task.WhenAll(OnGetVersionsResponse.GetInvocationList().
                //                       Cast<OnGetVersionsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnGetVersionsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetVersionDetails(VersionId, ...)

        /// <summary>
        /// Get versions.
        /// </summary>
        /// <param name="VersionId">The requested version.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Version_Id, VersionDetail>>

            GetVersionDetails(Version_Id?        VersionId             = null,
                              Boolean            SetAsDefaultVersion   = true,
                              Request_Id?        RequestId             = null,
                              Correlation_Id?    CorrelationId         = null,

                              DateTime?          Timestamp             = null,
                              CancellationToken  CancellationToken     = default,
                              EventTracking_Id?  EventTrackingId       = null,
                              TimeSpan?          RequestTimeout        = null)

        {

            #region Send OnGetVersionDetailsRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                Counters.GetVersions.IncRequests_OK();

                //if (OnGetVersionsRequest != null)
                //    await Task.WhenAll(OnGetVersionsRequest.GetInvocationList().
                //                       Cast<OnGetVersionsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnGetVersionDetailsRequest));
            }

            #endregion


            OCPIResponse<Version_Id, VersionDetail> response;

            var versionId = VersionId ?? SelectedOCPIVersionId;

            if (!versionId.HasValue)
                response = OCPIResponse<Version_Id, VersionDetail>.Error("No version identification available!");

            else
            {

                if (!Versions.ContainsKey(versionId.Value))
                    await GetVersions();

                if (!Versions.ContainsKey(versionId.Value))
                    response = OCPIResponse<Version_Id, VersionDetail>.Error("Unkown version identification!");

                else
                {

                    try
                    {

                        var requestId = RequestId ?? Request_Id.NewRandom();
                        var correlationId = CorrelationId ?? Correlation_Id.NewRandom();

                        // ToDo: Add request logging!


                        #region Upstream HTTP request...

                        var httpResponse = await new HTTPSClient(Versions[versionId.Value],
                                                                 VirtualHostname,
                                                                 Description,
                                                                 RemoteCertificateValidator,
                                                                 ClientCertificateSelector,
                                                                 ClientCert,
                                                                 TLSProtocol,
                                                                 PreferIPv4,
                                                                 HTTPUserAgent,
                                                                 RequestTimeout,
                                                                 TransmissionRetryDelay,
                                                                 MaxNumberOfRetries,
                                                                 UseHTTPPipelining,
                                                                 DisableLogging,
                                                                 HTTPLogger,
                                                                 DNSClient).

                                                   Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                          Versions[versionId.Value].Path,
                                                                                          requestbuilder =>
                                                                                          {
                                                                                              //requestbuilder.Host           = HTTPHostname.Parse(Versions[VersionId].Hostname + (Versions[VersionId].Port.HasValue ? Versions[VersionId].Port.Value.ToString() : ""));
                                                                                              requestbuilder.Authorization = TokenAuth;
                                                                                              requestbuilder.Connection = "close";
                                                                                              requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                              requestbuilder.Set("X-Request-ID", requestId);
                                                                                              requestbuilder.Set("X-Correlation-ID", correlationId);
                                                                                          }),

                                                         RequestLogDelegate:   OnGetVersionDetailsHTTPRequest,
                                                         ResponseLogDelegate:  OnGetVersionDetailsHTTPResponse,
                                                         CancellationToken:    CancellationToken,
                                                         EventTrackingId:      EventTrackingId,
                                                         RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                                   ConfigureAwait(false);

                        #endregion

                        #region Documentation

                        // {
                        //   "data": {
                        //     "version": "2.2",
                        //     "endpoints": [
                        //       {
                        //         "identifier": "sessions",
                        //         "role":       "SENDER",
                        //         "url":        "https://example.com/ocpi/cpo/2.2/sessions/"
                        //       },
                        //       {
                        //         "identifier": "tariffs",
                        //         "role":       "SENDER",
                        //         "url":        "https://example.com/ocpi/cpo/2.2/tariffs/"
                        //       },
                        //       {
                        //         "identifier": "hubclientinfo",
                        //         "role":       "RECEIVER",
                        //         "url":        "https://example.com/ocpi/cpo/2.2/hubclientinfo/"
                        //       },
                        //       {
                        //         "identifier": "locations",
                        //         "role":       "SENDER",
                        //         "url":        "https://example.com/ocpi/cpo/2.2/locations/"
                        //       },
                        //       {
                        //         "identifier": "tokens",
                        //         "role":       "RECEIVER",
                        //         "url":        "https://example.com/ocpi/cpo/2.2/tokens/"
                        //       },
                        //       {
                        //         "identifier": "commands",
                        //         "role":       "RECEIVER",
                        //         "url":        "https://example.com/ocpi/cpo/2.2/commands/"
                        //       },
                        //       {
                        //         "identifier": "credentials",
                        //         "role":       "RECEIVER",
                        //         "url":        "https://example.com/ocpi/2.2/credentials/"
                        //       },
                        //       {
                        //         "identifier": "credentials",
                        //         "role":       "SENDER",
                        //         "url":        "https://example.com/ocpi/2.2/credentials/"
                        //       }
                        //     ]
                        //   },
                        //   "status_code": 1000
                        // }

                        #endregion

                        response = OCPIResponse<Version_Id, VersionDetail>.ParseJObject(versionId.Value,
                                                                                        httpResponse,
                                                                                        requestId,
                                                                                        correlationId,
                                                                                        json => VersionDetail.Parse(json, null));

                        switch (response.StatusCode)
                        {

                            case 1000:

                                lock (VersionDetails)
                                {

                                    if (VersionDetails.ContainsKey(versionId.Value))
                                        VersionDetails.Remove(versionId.Value);

                                    VersionDetails.Add(versionId.Value, response.Data);

                                    if (SetAsDefaultVersion)
                                        SelectedOCPIVersionId = VersionId;

                                }

                                break;

                        }

                    }

                    catch (Exception e)
                    {
                        response = OCPIResponse<Version_Id, VersionDetail>.Exception(e);
                    }

                }

            }


            #region Send OnGetVersionDetailsResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetVersionsResponse != null)
                //    await Task.WhenAll(OnGetVersionsResponse.GetInvocationList().
                //                       Cast<OnGetVersionsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnGetVersionDetailsResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetRemoteURL(ModuleId, InterfaceRole, VersionId = null)

        public async Task<URL?> GetRemoteURL(Module_Id       ModuleId,
                                             InterfaceRoles  InterfaceRole,
                                             Version_Id?     VersionId   = null)
        {

            var versionId = VersionId ?? SelectedOCPIVersionId;

            if (!versionId.HasValue)
            {

                if (VersionDetails.Any())
                    SelectedOCPIVersionId = versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                else
                {

                    await GetVersions();

                    if (Versions.Any())
                    {
                        SelectedOCPIVersionId = versionId = Versions.Keys.OrderByDescending(id => id).First();
                        await GetVersionDetails(versionId.Value);
                    }

                }

            }

            if (versionId.     HasValue &&
                VersionDetails.TryGetValue(versionId.Value, out var versionDetails))
            {
                foreach (var endpoint in versionDetails.Endpoints)
                {
                    if (endpoint.Identifier == ModuleId &&
                        endpoint.Role       == InterfaceRole)
                    {
                        return endpoint.URL;
                    }
                }
            }

            return new URL?();

        }

        #endregion


        #region GetCredentials   (...)

        /// <summary>
        /// Get our credentials from the remote API.
        /// </summary>
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Credentials>>

            GetCredentials(Version_Id?         VersionId           = null,
                           Request_Id?         RequestId           = null,
                           Correlation_Id?     CorrelationId       = null,

                           DateTime?           Timestamp           = null,
                           CancellationToken   CancellationToken   = default,
                           EventTracking_Id?   EventTrackingId     = null,
                           TimeSpan?           RequestTimeout      = null)

        {

            #region Send OnGetCredentialsHTTPRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                //Counters.GetLocations.IncRequests();

                //if (OnGetLocationsRequest != null)
                //    await Task.WhenAll(OnGetLocationsRequest.GetInvocationList().
                //                       Cast<OnGetLocationsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnGetCredentialsHTTPRequest));
            }

            #endregion


            OCPIResponse<Credentials> response;

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();
                var remoteURL      = await GetRemoteURL(Module_Id.Credentials,
                                                        InterfaceRoles.RECEIVER,
                                                        versionId);

                // Might be updated!
                versionId ??= SelectedOCPIVersionId;

                if      (!versionId.HasValue)
                    response = OCPIResponse<Credentials>.Error("No versionId available!");

                else if (!remoteURL.HasValue)
                    response = OCPIResponse<Credentials>.Error("No remote URL available!");

                else
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocol,
                                                             PreferIPv4,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                              RequestLogDelegate:   OnGetCredentialsHTTPRequest,
                                              ResponseLogDelegate:  OnGetCredentialsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Credentials>.ParseJObject(httpResponse,
                                                                      requestId,
                                                                      correlationId,
                                                                      json => Credentials.Parse(json));

                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Credentials>.Exception(e);
            }


            #region Send OnGetCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetLocationsResponse != null)
                //    await Task.WhenAll(OnGetLocationsResponse.GetInvocationList().
                //                       Cast<OnGetLocationsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnGetCredentialsHTTPResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PostCredentials  (Credentials, ...)

        /// <summary>
        /// Post our credentials onto the remote API.
        /// </summary>
        /// <param name="Credentials">The credentials to store/post at/onto the remote API.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Credentials>>

            PostCredentials(Credentials         Credentials,

                            Version_Id?         VersionId           = null,
                            Request_Id?         RequestId           = null,
                            Correlation_Id?     CorrelationId       = null,

                            DateTime?           Timestamp           = null,
                            CancellationToken   CancellationToken   = default,
                            EventTracking_Id?   EventTrackingId     = null,
                            TimeSpan?           RequestTimeout      = null)

        {

            #region Send OnPostCredentialsHTTPRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                //Counters.PostLocations.IncRequests();

                //if (OnPostLocationsRequest != null)
                //    await Task.WhenAll(OnPostLocationsRequest.PostInvocationList().
                //                       Cast<OnPostLocationsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnPostCredentialsHTTPRequest));
            }

            #endregion


            OCPIResponse<Credentials> response;

            try
            {

                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();
                var remoteURL      = await GetRemoteURL(Module_Id.Credentials,
                                                        InterfaceRoles.RECEIVER,
                                                        VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocol,
                                                             PreferIPv4,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = TokenAuth;
                                                                                         requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content        = Credentials.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection     = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                              RequestLogDelegate:   OnPostCredentialsHTTPRequest,
                                              ResponseLogDelegate:  OnPostCredentialsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Credentials>.ParseJObject(httpResponse,
                                                                      requestId,
                                                                      correlationId,
                                                                      json => Credentials.Parse(json));

                }

                else
                    response = OCPIResponse<String, Credentials>.Error("No remote URL available!");

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Credentials>.Exception(e);
            }


            #region Send OnPostCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPostLocationsResponse != null)
                //    await Task.WhenAll(OnPostLocationsResponse.PostInvocationList().
                //                       Cast<OnPostLocationsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnPostCredentialsHTTPResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutCredentials   (Credentials, ...)

        /// <summary>
        /// Put our credentials onto the remote API.
        /// </summary>
        /// <param name="Credentials">The credentials to store/put at/onto the remote API.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Credentials>>

            PutCredentials(Credentials         Credentials,

                           Version_Id?         VersionId           = null,
                           Request_Id?         RequestId           = null,
                           Correlation_Id?     CorrelationId       = null,

                           DateTime?           Timestamp           = null,
                           CancellationToken   CancellationToken   = default,
                           EventTracking_Id?   EventTrackingId     = null,
                           TimeSpan?           RequestTimeout      = null)

        {

            #region Send OnPutCredentialsHTTPRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                //Counters.PutLocations.IncRequests();

                //if (OnPutLocationsRequest != null)
                //    await Task.WhenAll(OnPutLocationsRequest.PutInvocationList().
                //                       Cast<OnPutLocationsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnPutCredentialsHTTPRequest));
            }

            #endregion


            OCPIResponse<Credentials> response;

            try
            {

                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();
                var remoteURL      = await GetRemoteURL(Module_Id.Credentials,
                                                        InterfaceRoles.RECEIVER,
                                                        VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocol,
                                                             PreferIPv4,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = TokenAuth;
                                                                                         requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content        = Credentials.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection     = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                              RequestLogDelegate:   OnPutCredentialsHTTPRequest,
                                              ResponseLogDelegate:  OnPutCredentialsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Credentials>.ParseJObject(httpResponse,
                                                                      requestId,
                                                                      correlationId,
                                                                      json => Credentials.Parse(json));

                    if (response.Data is not null)
                    {

                        TokenAuth = new HTTPTokenAuthentication(response.Data.Token.ToString().ToBase64());

                        var oldRemoteParty = this.RemoteParty;

                        // Validate, that neither the credentials roles had not been changed!
                        if (oldRemoteParty is not null &&
                            oldRemoteParty.Roles.Count() == response.Data.Roles.Count())
                        {

                            var failed = false;

                            foreach (var receivedCredentialsRole in response.Data.Roles)
                            {

                                CredentialsRole? existingCredentialsRole = null;

                                foreach (var oldCredentialsRole in oldRemoteParty.Roles)
                                {
                                    if (oldCredentialsRole.CountryCode == receivedCredentialsRole.CountryCode &&
                                        oldCredentialsRole.PartyId     == receivedCredentialsRole.PartyId &&
                                        oldCredentialsRole.Role        == receivedCredentialsRole.Role)
                                    {
                                        existingCredentialsRole = receivedCredentialsRole;
                                        break;
                                    }
                                }

                                if (existingCredentialsRole is null)
                                    failed = true;

                            }

                            if (!failed)
                            {

                                // Only the access token and the business details are allowed to be changed!
                                var result = await MyCommonAPI.AddOrUpdateRemoteParty(Id:                 oldRemoteParty.Id,
                                                                                      CredentialsRoles:   response.Data.Roles,

                                                                                      AccessToken:        Credentials.Token,
                                                                                      AccessStatus:       AccessStatus.ALLOWED,

                                                                                      RemoteAccessToken:  response.Data.Token,
                                                                                      RemoteVersionsURL:  response.Data.URL,
                                                                                      RemoteVersionIds:   new Version_Id[] { Version_Id.Parse("2.2") },
                                                                                      SelectedVersionId:  Version_Id.Parse("2.2"),

                                                                                      PartyStatus:        PartyStatus.ENABLED,
                                                                                      RemoteStatus:       RemoteAccessStatus.ONLINE);

                                if (!result)
                                    DebugX.Log("Illegal AddOrUpdateRemoteParty(...) after PutCredentials(...)!");

                            }

                        }

                    }

                }

                else
                    response = OCPIResponse<String, Credentials>.Error("No remote URL available!");

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Credentials>.Exception(e);
            }


            #region Send OnPutCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutLocationsResponse != null)
                //    await Task.WhenAll(OnPutLocationsResponse.PutInvocationList().
                //                       Cast<OnPutLocationsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnPutCredentialsHTTPResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteCredentials(Credentials, ...)

        /// <summary>
        /// Remove our credentials from the remote API.
        /// </summary>
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse>

            DeleteCredentials(Version_Id?        VersionId           = null,
                              Request_Id?        RequestId           = null,
                              Correlation_Id?    CorrelationId       = null,

                              DateTime?          Timestamp           = null,
                              CancellationToken  CancellationToken   = default,
                              EventTracking_Id?  EventTrackingId     = null,
                              TimeSpan?          RequestTimeout      = null)

        {

            #region Send OnDeleteCredentialsHTTPRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                //Counters.DeleteLocations.IncRequests();

                //if (OnDeleteLocationsRequest != null)
                //    await Task.WhenAll(OnDeleteLocationsRequest.DeleteInvocationList().
                //                       Cast<OnDeleteLocationsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnDeleteCredentialsHTTPRequest));
            }

            #endregion


            OCPIResponse response;

            try
            {

                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();
                var remoteURL      = await GetRemoteURL(Module_Id.Credentials,
                                                        InterfaceRoles.RECEIVER,
                                                        VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocol,
                                                             PreferIPv4,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.DELETE,
                                                                                     remoteURL.Value.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = TokenAuth;
                                                                                         //requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         //requestbuilder.Content        = Credentials.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection     = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                              RequestLogDelegate:   OnDeleteCredentialsHTTPRequest,
                                              ResponseLogDelegate:  OnDeleteCredentialsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse.Parse(httpResponse,
                                                  requestId,
                                                  correlationId);

                }

                else
                    response = OCPIResponse.Error("No remote URL available!");

            }

            catch (Exception e)
            {
                response = OCPIResponse<String>.Exception(e);
            }


            #region Send OnDeleteCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnDeleteLocationsResponse != null)
                //    await Task.WhenAll(OnDeleteLocationsResponse.DeleteInvocationList().
                //                       Cast<OnDeleteLocationsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnDeleteCredentialsHTTPResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region Register(VersionId, ...)

        //  1. We create <CREDENTIALS_TOKEN_A> and associate it with <CountryCode> + <PartyId>.
        //  2. We send <CREDENTIALS_TOKEN_A> and <VERSIONS endpoint> to the other party... e.g. via e-mail.
        //
        //    waiting...
        //
        //  3. Other party fetches <VERSIONS endpoint> using <CREDENTIALS_TOKEN_A> and stores all versions endpoint information.
        //  4. Other party chooses a common version, fetches version details from the found <VERSION DETAIL endpoint> using <CREDENTIALS_TOKEN_A> and stores all version endpoint information.
        //  5. Other party generates <CREDENTIALS_TOKEN_B> and POSTs its choosen version (as part of the request URL), its <VERSIONS endpoint> and this <CREDENTIALS_TOKEN_B>.
        //
        // >>> Here we are in a synchronous HTTP POST request! >>>
        //    6. We verify that the version the other side has choosen is at least valid.
        //    7. We fetch <VERSIONS> from the given <VERSIONS endpoint> using <CREDENTIALS_TOKEN_B> and store all versions endpoint information.
        //    8. We verify that the version the other side has choosen can be used.
        //    9. We fetch <VERSION DETAIL> from the given <VERSION DETAIL endpoint> for the common version using <CREDENTIALS_TOKEN_B> and store all versions endpoint information.
        //   10. We generate <CREDENTIALS_TOKEN_C> and return this as response to the POST request in 5.
        //
        //   In case the Receiver platform that cannot find the endpoints it expects, then it is expected to respond to the request with the status code 3003.
        //   On any error: Reply with HTTP Status Code 3001
        //
        // 11. Other party will replace <CREDENTIALS_TOKEN_A> with <CREDENTIALS_TOKEN_C>.


        /// <summary>
        /// Post the given credentials.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Credentials>>

            Register(Version_Id?                    VersionId             = null,
                     Boolean                        SetAsDefaultVersion   = true,
                     AccessToken?                   CredentialTokenB      = null,

                     Request_Id?                    RequestId             = null,
                     Correlation_Id?                CorrelationId         = null,

                     DateTime?                      Timestamp             = null,
                     CancellationToken              CancellationToken     = default,
                     EventTracking_Id?              EventTrackingId       = null,
                     TimeSpan?                      RequestTimeout        = null)

        {

            OCPIResponse<Credentials> response;

            #region Send OnRegisterRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                //Counters.GetLocations.IncRequests();

                //if (OnGetLocationsRequest != null)
                //    await Task.WhenAll(OnGetLocationsRequest.GetInvocationList().
                //                       Cast<OnGetLocationsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnRegisterRequest));
            }

            #endregion


            try
            {

                var versionId         = VersionId        ?? SelectedOCPIVersionId;
                var credentialTokenB  = CredentialTokenB ?? AccessToken.NewRandom();

                var remoteURL         = await GetRemoteURL(Module_Id.Credentials,
                                                           InterfaceRoles.RECEIVER,
                                                           versionId);

                // Might be updated!
                versionId ??= SelectedOCPIVersionId;

                if      (!versionId.    HasValue)
                    response = OCPIResponse<String, Credentials>.Error("No version identification available!");

                else if (!remoteURL.HasValue)
                    response = OCPIResponse<String, Credentials>.Error("No remote URL available!");

                else
                {

                    var requestId          = RequestId     ?? Request_Id.NewRandom();
                    var correlationId      = CorrelationId ?? Correlation_Id.NewRandom();
                    var credentials        = new Credentials(credentialTokenB,
                                                             MyCommonAPI.OurVersionsURL,
                                                             MyCommonAPI.OurCredentialRoles);

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocol,
                                                             PreferIPv4,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization  = TokenAuth;
                                                                                         requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content        = credentials.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection     = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnRegisterHTTPRequest,
                                                      ResponseLogDelegate:  OnRegisterHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       TimeSpan.FromMinutes(5)).// RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);


                    // {
                    //     "data": {
                    //         "token":                     "e~!Kgf457pApk5b&vG93K-<MQ#T&Q)io!S)HfQxSGyb#*b6beZP#36kF55~zhuq]",
                    //         "url":                       "https://example.com/ocpi/versions",
                    //         "roles": [{
                    //             "role":                  "CPO",
                    //             "business_details": {
                    //                 "name":              "Example Corp."
                    //             },
                    //             "party_id":              "EXP",
                    //             "country_code":          "DE"
                    //         }]
                    //     },
                    //     "status_code": 1000,
                    //     "timestamp": "2020-11-07T15:33:49.481Z"
                    // }

                    #endregion

                    response               = OCPIResponse<Credentials>.ParseJObject(httpResponse,
                                                                                    requestId,
                                                                                    correlationId,
                                                                                    json => Credentials.Parse(json));

                    if (response.Data is not null)
                    {

                        SelectedOCPIVersionId  = versionId;
                        TokenAuth              = new HTTPTokenAuthentication(response.Data.Token.ToString().ToBase64());

                        var oldRemoteParty     = this.RemoteParty;

                        // Validate, that neither the credentials roles had not been changed!
                        if (oldRemoteParty is not null &&
                            oldRemoteParty.Roles.Count() == response.Data.Roles.Count())
                        {

                            var failed = false;

                            foreach (var receivedCredentialsRole in response.Data.Roles)
                            {

                                CredentialsRole? existingCredentialsRole = null;

                                foreach (var oldCredentialsRole in oldRemoteParty.Roles)
                                {
                                    if (oldCredentialsRole.CountryCode == receivedCredentialsRole.CountryCode &&
                                        oldCredentialsRole.PartyId     == receivedCredentialsRole.PartyId &&
                                        oldCredentialsRole.Role        == receivedCredentialsRole.Role)
                                    {
                                        existingCredentialsRole = receivedCredentialsRole;
                                        break;
                                    }
                                }

                                if (existingCredentialsRole is null)
                                    failed = true;

                            }

                            if (!failed)
                            {

                                // Only the access token and the business details are allowed to be changed!
                                var result = await MyCommonAPI.AddOrUpdateRemoteParty(Id:                 oldRemoteParty.Id,
                                                                                      CredentialsRoles:   response.Data.Roles,

                                                                                      AccessToken:        credentialTokenB,
                                                                                      AccessStatus:       AccessStatus.ALLOWED,

                                                                                      RemoteAccessToken:  response.Data.Token,
                                                                                      RemoteVersionsURL:  response.Data.URL,
                                                                                      RemoteVersionIds:   new Version_Id[] { versionId.Value },
                                                                                      SelectedVersionId:  versionId.Value,

                                                                                      PartyStatus:        PartyStatus.ENABLED,
                                                                                      RemoteStatus:       RemoteAccessStatus.ONLINE);

                                if (!result)
                                    DebugX.Log("Illegal AddOrUpdateRemoteParty(...) after PutCredentials(...)!");

                            }

                        }

                    }

                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Credentials>.Exception(e);
            }


            #region Send OnRegisterResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetLocationsResponse != null)
                //    await Task.WhenAll(OnGetLocationsResponse.GetInvocationList().
                //                       Cast<OnGetLocationsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnRegisterResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        { }

        #endregion

    }

}
