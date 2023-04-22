﻿/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.UnitTests
{

    [TestFixture]
    public class EMSPTests : ANodeTests
    {

        #region EMSP_GetVersions_Test()

        /// <summary>
        /// EMSP GetVersions Test.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersions_Test()
        {

            var graphDefinedCPO = emspWebAPI1?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersions();

                // GET /versions HTTP/1.1
                // Date:                          Sun, 25 Dec 2022 23:16:30 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // User-Agent:                    GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                  Wz8f2E9YKMj7Wdx6pQ3YjbQhd86EYG
                // X-Correlation-ID:              2AYdzYY6zK6Y59hS5bn3n8A3Kt7C6x

                // HTTP/1.1 200 OK
                // Date:                          Sun, 25 Dec 2022 23:16:31 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                165
                // X-Request-ID:                  Wz8f2E9YKMj7Wdx6pQ3YjbQhd86EYG
                // X-Correlation-ID:              2AYdzYY6zK6Y59hS5bn3n8A3Kt7C6x
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://127.0.0.1:7234/versions/2.1.1"
                //     }],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-25T23:16:31.228Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response.StatusCode);
                Assert.AreEqual ("Hello world!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual (1, response.Data.Count());

                var version = versions.First();
                Assert.IsTrue   (version.Id == Version.Id);
                Assert.AreEqual (cpoVersionsAPIURL + Version.Id.ToString(), version.URL);

            }

        }

        #endregion

        #region EMSP_GetVersion_Test()

        /// <summary>
        /// EMSP GetVersion Test 01.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_Test()
        {

            var graphDefinedCPO = emspWebAPI1?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersionDetails(Version.Id);

                // GET /versions/2.2.1 HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 00:36:20 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // User-Agent:                    GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:                  21Sj4CSUttCvUE7t8YdttY31YA1nzx
                // X-Correlation-ID:              jGrrQ7pEb541dCnUx12SEp3KG88YjG

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 00:36:21 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                965
                // X-Request-ID:                  21Sj4CSUttCvUE7t8YdttY31YA1nzx
                // X-Correlation-ID:              jGrrQ7pEb541dCnUx12SEp3KG88YjG
                // 
                // {
                //     "data": {
                //         "version": "2.2.1",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/credentials"
                //             },
                //             {
                //                 "identifier":  "credentials",
                //                 "role":        "RECEIVER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/cpo/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/cpo/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/cpo/sessions"
                //             },
                //             {
                //                 "identifier":  "chargingprofiles",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/cpo/chargingprofiles"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "role":        "SENDER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/cpo/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "role":        "RECEIVER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/cpo/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "role":        "RECEIVER",
                //                 "url":         "http://127.0.0.1:7234/2.2.1/cpo/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-26T00:36:21.228Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response.StatusCode);
                Assert.AreEqual ("Hello world!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                var versionDetail  = response.Data;
                Assert.IsNotNull(versionDetail);

                var versionId      = versionDetail.VersionId;
                Assert.IsTrue   (versionId == Version.Id);

                var endpoints      = versionDetail.Endpoints;
                Assert.AreEqual (9, endpoints.Count());

                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "credentials"      && endpoint.Role == InterfaceRoles.SENDER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "credentials"      && endpoint.Role == InterfaceRoles.RECEIVER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "locations"        && endpoint.Role == InterfaceRoles.SENDER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "tariffs"          && endpoint.Role == InterfaceRoles.SENDER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "sessions"         && endpoint.Role == InterfaceRoles.SENDER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "chargingprofiles" && endpoint.Role == InterfaceRoles.SENDER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "cdrs"             && endpoint.Role == InterfaceRoles.SENDER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "commands"         && endpoint.Role == InterfaceRoles.RECEIVER));
                Assert.IsTrue(versionDetail.Endpoints.Any(endpoint => endpoint.Identifier.ToString() == "tokens"           && endpoint.Role == InterfaceRoles.RECEIVER));

            }

        }

        #endregion

        #region EMSP_GetVersion_UnknownVersion_Test()

        /// <summary>
        /// EMSP GetVersion Unknown Version Test.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_UnknownVersion_Test()
        {

            var graphDefinedCPO = emspWebAPI1?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response = await graphDefinedCPO.GetVersionDetails(Version_Id.Parse("0.7"));

                // GET /versions/0.7 HTTP/1.1
                // Date:              Mon, 26 Dec 2022 00:36:20 GMT
                // Accept:            application/json; charset=utf-8;q=1
                // Host:              127.0.0.1:7234
                // Authorization:     Token xxxxxx
                // User-Agent:        GraphDefined OCPI HTTP Client v1.0
                // X-Request-ID:      21Sj4CSUttCvUE7t8YdttY31YA1nzx
                // X-Correlation-ID:  jGrrQ7pEb541dCnUx12SEp3KG88YjG

                Assert.IsNotNull(response);

                Assert.AreEqual (-1,                                response.StatusCode); // local error!
                Assert.AreEqual ("Unkown version identification!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                // There is not HTTP response, as this is a local error!
                Assert.IsNull   (response.HTTPResponse);
                Assert.IsNull   (response.RequestId);
                Assert.IsNull   (response.CorrelationId);

            }

        }

        #endregion

        #region EMSP_GetVersion_UnknownVersion_viaHTTP_Test()

        /// <summary>
        /// EMSP GetVersion Unknown Version via HTTP Test.
        /// </summary>
        [Test]
        public async Task EMSP_GetVersion_UnknownVersion_viaHTTP_Test()
        {

            var graphDefinedCPO = emspWebAPI1?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var httpResponse = await TestHelpers.JSONRequest(URL.Parse("http://127.0.0.1:7235/versions/0.7"),
                                                                 "xxxxxx");

                // GET /versions/0.7 HTTP/1.1
                // Date:                          Sat, 22 Apr 2023 11:54:54 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7235
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  1234
                // X-Correlation-ID:              5678

                // HTTP/1.1 404 Not Found
                // Date:                          Sat, 22 Apr 2023 11:54:54 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                114
                // X-Request-ID:                  1234
                // X-Correlation-ID:              5678
                // 
                // {
                //     "status_code":      2000,
                //     "status_message":  "This OCPI version is not supported!",
                //     "timestamp":       "2023-04-22T11:54:54.800Z"
                // }

                Assert.IsNotNull(httpResponse);
                Assert.IsTrue   (httpResponse.ContentLength > 0);

                var response = OCPIResponse.Parse(httpResponse,
                                                  Request_Id.    Parse("12340"),
                                                  Correlation_Id.Parse("56780"));

                Assert.AreEqual (2000,                                   response.StatusCode);
                Assert.AreEqual ("This OCPI version is not supported!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsNotNull(response.HTTPResponse);

            }

        }

        #endregion


        #region EMSP_GetCredentials_Test1()

        /// <summary>
        /// EMSP GetCredentials Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_Test1()
        {

            var graphDefinedCPO = emspWebAPI1?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response1 = await graphDefinedCPO.GetVersions();
                var response2 = await graphDefinedCPO.GetCredentials();

                // GET /2.2.1/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365
                // 
                // {
                //    "data": {
                //        "token":         "xxxxxx",
                //        "url":           "http://127.0.0.1:7234/versions",
                //        "business_details": {
                //            "name":           "GraphDefined CPO Services",
                //            "website":        "https://www.graphdefined.com/cpo"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GEF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T10:29:49.143Z"
                //}

                Assert.IsNotNull(response2);
                Assert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response2.StatusCode);
                Assert.AreEqual ("Hello world!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.IsNotNull(credentials);
                Assert.AreEqual("xxxxxx",                            credentials.Token.                    ToString());
                Assert.AreEqual("http://127.0.0.1:7234/versions",    credentials.URL.                      ToString());
                Assert.AreEqual("DE",                                credentials.Roles.First().CountryCode.ToString());
                Assert.AreEqual("GEF",                               credentials.Roles.First().PartyId.    ToString());

                var businessDetails = credentials.Roles.First().BusinessDetails;
                Assert.IsNotNull(businessDetails);
                Assert.AreEqual("GraphDefined CPO Services",         businessDetails.Name);
                Assert.AreEqual("https://www.graphdefined.com/cpo",  businessDetails.Website.    ToString());

            }

        }

        #endregion

        #region EMSP_GetCredentials_Test2()

        /// <summary>
        /// EMSP GetCredentials Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetCredentials_Test2()
        {

            var graphDefinedCPO = emspWebAPI2?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (graphDefinedCPO is not null)
            {

                var response1 = await graphDefinedCPO.GetVersions();
                var response2 = await graphDefinedCPO.GetCredentials();

                // GET /2.2.1/credentials HTTP/1.1
                // Date:                          Mon, 26 Dec 2022 10:29:48 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  27ExnKK8fK3vhQvYA8dY7A2rx9KUtC
                // X-Correlation-ID:              325WrvpzKh6f238Mb4Ex17v1612365
                // 
                // {
                //    "data": {
                //        "token":         "xxxxxx",
                //        "url":           "http://127.0.0.1:7234/versions",
                //        "business_details": {
                //            "name":           "GraphDefined CPO Services",
                //            "website":        "https://www.graphdefined.com/cpo"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GEF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T10:29:49.143Z"
                //}

                Assert.IsNotNull(response2);
                Assert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response2.StatusCode);
                Assert.AreEqual ("Hello world!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.IsNotNull(credentials);
                Assert.AreEqual("eeeeee",                            credentials.Token.                    ToString());
                Assert.AreEqual("http://127.0.0.1:7234/versions",    credentials.URL.                      ToString());
                Assert.AreEqual("DE",                                credentials.Roles.First().CountryCode.ToString());
                Assert.AreEqual("GEF",                               credentials.Roles.First().PartyId.    ToString());

                var businessDetails = credentials.Roles.First().BusinessDetails;
                Assert.IsNotNull(businessDetails);
                Assert.AreEqual("GraphDefined CPO Services",         businessDetails.Name);
                Assert.AreEqual("https://www.graphdefined.com/cpo",  businessDetails.Website.    ToString());

            }

        }

        #endregion


        #region EMSP_GetLocations_Test1()

        /// <summary>
        /// EMSP GetLocations Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocations_Test1()
        {

            var graphDefinedCPO = emspWebAPI1?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (cpoWebAPI       is not null &&
                graphDefinedCPO is not null)
            {

                #region Define Location1

                cpoWebAPI.CommonAPI.AddLocation(new Location(
                                                    CountryCode.Parse("DE"),
                                                    Party_Id.Parse("GEF"),
                                                    Location_Id.Parse("LOC0001"),
                                                    true,
                                                    "Biberweg 18",
                                                    "Jena",
                                                    Country.Germany,
                                                    GeoCoordinate.Parse(10, 20),
                                                    "Europe/Berlin",
                                                    null,
                                                    "Location 0001",
                                                    "07749",
                                                    "Thüringen",
                                                    new[] {
                                                        new AdditionalGeoLocation(
                                                            Latitude.Parse(11),
                                                            Longitude.Parse(22),
                                                            Name: DisplayText.Create(Languages.de, "Postkasten")
                                                        )
                                                    },
                                                    ParkingType.PARKING_LOT,
                                                    new[] {
                                                        new EVSE(
                                                            EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                            StatusType.AVAILABLE,
                                                            new[] {
                                                                new Connector(
                                                                    Connector_Id.Parse("1"),
                                                                    ConnectorType.IEC_62196_T2,
                                                                    ConnectorFormats.SOCKET,
                                                                    PowerTypes.AC_3_PHASE,
                                                                    400,
                                                                    30,
                                                                    12,
                                                                    new[] {
                                                                        Tariff_Id.Parse("DE*GEF*T0001"),
                                                                        Tariff_Id.Parse("DE*GEF*T0002")
                                                                    },
                                                                    URL.Parse("https://open.charging.cloud/terms"),
                                                                    DateTime.Parse("2020-09-21")
                                                                ),
                                                                new Connector(
                                                                    Connector_Id.Parse("2"),
                                                                    ConnectorType.IEC_62196_T2_COMBO,
                                                                    ConnectorFormats.CABLE,
                                                                    PowerTypes.AC_3_PHASE,
                                                                    400,
                                                                    20,
                                                                    8,
                                                                    new[] {
                                                                        Tariff_Id.Parse("DE*GEF*T0003"),
                                                                        Tariff_Id.Parse("DE*GEF*T0004")
                                                                    },
                                                                    URL.Parse("https://open.charging.cloud/terms"),
                                                                    DateTime.Parse("2020-09-22")
                                                                )
                                                            },
                                                            EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                            new[] {
                                                                new StatusSchedule(
                                                                    StatusType.INOPERATIVE,
                                                                    DateTime.Parse("2020-09-23"),
                                                                    DateTime.Parse("2020-09-24")
                                                                ),
                                                                new StatusSchedule(
                                                                    StatusType.OUTOFORDER,
                                                                    DateTime.Parse("2020-12-30"),
                                                                    DateTime.Parse("2020-12-31")
                                                                )
                                                            },
                                                            new[] {
                                                                Capability.RFID_READER,
                                                                Capability.RESERVABLE
                                                            },

                                                            // OCPI Computer Science Extensions
                                                            new EnergyMeter(
                                                                Meter_Id.Parse("Meter0815"),
                                                                "EnergyMeter Model #1",
                                                                null,
                                                                "hw. v1.80",
                                                                "fw. v1.20",
                                                                "Energy Metering Services",
                                                                null,
                                                                null,
                                                                null
                                                            ),

                                                            "1. Stock",
                                                            GeoCoordinate.Parse(10.1, 20.2),
                                                            "Ladestation #1",
                                                            new[] {
                                                                DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                DisplayText.Create(Languages.en, "Ken sent me!")
                                                            },
                                                            new ParkingRestrictions[] {
                                                                ParkingRestrictions.EV_ONLY,
                                                                ParkingRestrictions.PLUGGED
                                                            },
                                                            new[] {
                                                                new Image(
                                                                    URL.Parse("http://example.com/pinguine.jpg"),
                                                                    ImageFileType.jpeg,
                                                                    ImageCategory.OPERATOR,
                                                                    100,
                                                                    150,
                                                                    URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                )
                                                            },
                                                            DateTime.Parse("2020-09-22")
                                                        )
                                                    },
                                                    new[] {
                                                        new DisplayText(Languages.de, "Hallo Welt!"),
                                                        new DisplayText(Languages.en, "Hello world!")
                                                    },
                                                    new BusinessDetails(
                                                        "Open Charging Cloud",
                                                        URL.Parse("https://open.charging.cloud"),
                                                        new Image(
                                                            URL.Parse("http://open.charging.cloud/logo.svg"),
                                                            ImageFileType.svg,
                                                            ImageCategory.OPERATOR,
                                                            1000,
                                                            1500,
                                                            URL.Parse("http://open.charging.cloud/logo_small.svg")
                                                        )
                                                    ),
                                                    new BusinessDetails(
                                                        "GraphDefined GmbH",
                                                        URL.Parse("https://www.graphdefined.com"),
                                                        new Image(
                                                            URL.Parse("http://www.graphdefined.com/logo.png"),
                                                            ImageFileType.png,
                                                            ImageCategory.OPERATOR,
                                                            2000,
                                                            3000,
                                                            URL.Parse("http://www.graphdefined.com/logo_small.png")
                                                        )
                                                    ),
                                                    new BusinessDetails(
                                                        "Achim Friedland",
                                                        URL.Parse("https://ahzf.de"),
                                                        new Image(
                                                            URL.Parse("http://ahzf.de/logo.gif"),
                                                            ImageFileType.gif,
                                                            ImageCategory.OWNER,
                                                            3000,
                                                            4500,
                                                            URL.Parse("http://ahzf.de/logo_small.gif")
                                                        )
                                                    ),
                                                    new[] {
                                                        Facilities.CAFE
                                                    },
                                                    new Hours(
                                                        new[] {
                                                            new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                                            new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                                            new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                                            new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                                            new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                                        },
                                                        new[] {
                                                            new ExceptionalPeriod(
                                                                DateTime.Parse("2020-09-21T00:00:00Z"),
                                                                DateTime.Parse("2020-09-22T00:00:00Z")
                                                            )
                                                        },
                                                        new[] {
                                                            new ExceptionalPeriod(
                                                                DateTime.Parse("2020-12-24T00:00:00Z"),
                                                                DateTime.Parse("2020-12-26T00:00:00Z")
                                                            )
                                                        }
                                                    ),
                                                    false,
                                                    new[] {
                                                        new Image(
                                                            URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                                            ImageFileType.jpeg,
                                                            ImageCategory.LOCATION,
                                                            200,
                                                            400,
                                                            URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                                        )
                                                    },
                                                    new(
                                                        true,
                                                        new[] {
                                                            new EnergySource(
                                                                EnergySourceCategory.SOLAR,
                                                                80
                                                            ),
                                                            new EnergySource(
                                                                EnergySourceCategory.WIND,
                                                                20
                                                            )
                                                        },
                                                        new[] {
                                                            new EnvironmentalImpact(
                                                                EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                0.1
                                                            )
                                                        },
                                                        "Stadtwerke Jena-Ost",
                                                        "New Green Deal"
                                                    ),
                                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                                ),
                                                SkipNotifications: true
                                            );

                #endregion


                var response = await graphDefinedCPO.GetLocations();

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // Connection:                    close
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882

                // HTTP/1.1 200 OK
                // Date:                          Tue, 18 Apr 2023 03:41:28 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // X-Total-Count:                 1
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                5494
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882
                // 
                // {
                //     "data":            [{"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefiend","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"}],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-18T03:41:28.838Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response.StatusCode);
                Assert.AreEqual ("Hello world!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsTrue   (response.Data.First().EVSEs.First().Connectors.First().TariffIds.Contains(Tariff_Id.Parse("DE*GEF*T0001")));

                //Assert.IsNotNull(response.Request);

            }

        }

        #endregion

        #region EMSP_GetLocations_Test2()

        /// <summary>
        /// EMSP GetLocations Test 2.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocations_Test2()
        {

            var graphDefinedCPO = emspWebAPI2?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (cpoWebAPI       is not null &&
                graphDefinedCPO is not null)
            {

                #region Define Location1

                cpoWebAPI.CommonAPI.AddLocation(new Location(
                                                    CountryCode.Parse("DE"),
                                                    Party_Id.Parse("GEF"),
                                                    Location_Id.Parse("LOC0001"),
                                                    true,
                                                    "Biberweg 18",
                                                    "Jena",
                                                    Country.Germany,
                                                    GeoCoordinate.Parse(10, 20),
                                                    "Europe/Berlin",
                                                    null,
                                                    "Location 0001",
                                                    "07749",
                                                    "Thüringen",
                                                    new[] {
                                                        new AdditionalGeoLocation(
                                                            Latitude.Parse(11),
                                                            Longitude.Parse(22),
                                                            Name: DisplayText.Create(Languages.de, "Postkasten")
                                                        )
                                                    },
                                                    ParkingType.PARKING_LOT,
                                                    new[] {
                                                        new EVSE(
                                                            EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                            StatusType.AVAILABLE,
                                                            new[] {
                                                                new Connector(
                                                                    Connector_Id.Parse("1"),
                                                                    ConnectorType.IEC_62196_T2,
                                                                    ConnectorFormats.SOCKET,
                                                                    PowerTypes.AC_3_PHASE,
                                                                    400,
                                                                    30,
                                                                    12,
                                                                    new Tariff_Id[] {
                                                                        Tariff_Id.Parse("DE*GEF*T0001"),
                                                                        Tariff_Id.Parse("DE*GEF*T0002")
                                                                    },
                                                                    URL.Parse("https://open.charging.cloud/terms"),
                                                                    DateTime.Parse("2020-09-21")
                                                                ),
                                                                new Connector(
                                                                    Connector_Id.Parse("2"),
                                                                    ConnectorType.IEC_62196_T2_COMBO,
                                                                    ConnectorFormats.CABLE,
                                                                    PowerTypes.AC_3_PHASE,
                                                                    400,
                                                                    20,
                                                                    8,
                                                                    new Tariff_Id[] {
                                                                        Tariff_Id.Parse("DE*GEF*T0003"),
                                                                        Tariff_Id.Parse("DE*GEF*T0004")
                                                                    },
                                                                    URL.Parse("https://open.charging.cloud/terms"),
                                                                    DateTime.Parse("2020-09-22")
                                                                )
                                                            },
                                                            EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                            new[] {
                                                                new StatusSchedule(
                                                                    StatusType.INOPERATIVE,
                                                                    DateTime.Parse("2020-09-23"),
                                                                    DateTime.Parse("2020-09-24")
                                                                ),
                                                                new StatusSchedule(
                                                                    StatusType.OUTOFORDER,
                                                                    DateTime.Parse("2020-12-30"),
                                                                    DateTime.Parse("2020-12-31")
                                                                )
                                                            },
                                                            new[] {
                                                                Capability.RFID_READER,
                                                                Capability.RESERVABLE
                                                            },

                                                            // OCPI Computer Science Extensions
                                                            new EnergyMeter(
                                                                Meter_Id.Parse("Meter0815"),
                                                                "EnergyMeter Model #1",
                                                                null,
                                                                "hw. v1.80",
                                                                "fw. v1.20",
                                                                "Energy Metering Services",
                                                                null,
                                                                null,
                                                                null
                                                            ),

                                                            "1. Stock",
                                                            GeoCoordinate.Parse(10.1, 20.2),
                                                            "Ladestation #1",
                                                            new[] {
                                                                DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                DisplayText.Create(Languages.en, "Ken sent me!")
                                                            },
                                                            new ParkingRestrictions[] {
                                                                ParkingRestrictions.EV_ONLY,
                                                                ParkingRestrictions.PLUGGED
                                                            },
                                                            new[] {
                                                                new Image(
                                                                    URL.Parse("http://example.com/pinguine.jpg"),
                                                                    ImageFileType.jpeg,
                                                                    ImageCategory.OPERATOR,
                                                                    100,
                                                                    150,
                                                                    URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                )
                                                            },
                                                            DateTime.Parse("2020-09-22")
                                                        )
                                                    },
                                                    new[] {
                                                        new DisplayText(Languages.de, "Hallo Welt!"),
                                                        new DisplayText(Languages.en, "Hello world!")
                                                    },
                                                    new BusinessDetails(
                                                        "Open Charging Cloud",
                                                        URL.Parse("https://open.charging.cloud"),
                                                        new Image(
                                                            URL.Parse("http://open.charging.cloud/logo.svg"),
                                                            ImageFileType.svg,
                                                            ImageCategory.OPERATOR,
                                                            1000,
                                                            1500,
                                                            URL.Parse("http://open.charging.cloud/logo_small.svg")
                                                        )
                                                    ),
                                                    new BusinessDetails(
                                                        "GraphDefined GmbH",
                                                        URL.Parse("https://www.graphdefined.com"),
                                                        new Image(
                                                            URL.Parse("http://www.graphdefined.com/logo.png"),
                                                            ImageFileType.png,
                                                            ImageCategory.OPERATOR,
                                                            2000,
                                                            3000,
                                                            URL.Parse("http://www.graphdefined.com/logo_small.png")
                                                        )
                                                    ),
                                                    new BusinessDetails(
                                                        "Achim Friedland",
                                                        URL.Parse("https://ahzf.de"),
                                                        new Image(
                                                            URL.Parse("http://ahzf.de/logo.gif"),
                                                            ImageFileType.gif,
                                                            ImageCategory.OWNER,
                                                            3000,
                                                            4500,
                                                            URL.Parse("http://ahzf.de/logo_small.gif")
                                                        )
                                                    ),
                                                    new[] {
                                                        Facilities.CAFE
                                                    },
                                                    new Hours(
                                                        new[] {
                                                            new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                                            new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                                            new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                                            new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                                            new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                                        },
                                                        new[] {
                                                            new ExceptionalPeriod(
                                                                DateTime.Parse("2020-09-21T00:00:00Z"),
                                                                DateTime.Parse("2020-09-22T00:00:00Z")
                                                            )
                                                        },
                                                        new[] {
                                                            new ExceptionalPeriod(
                                                                DateTime.Parse("2020-12-24T00:00:00Z"),
                                                                DateTime.Parse("2020-12-26T00:00:00Z")
                                                            )
                                                        }
                                                    ),
                                                    false,
                                                    new[] {
                                                        new Image(
                                                            URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                                            ImageFileType.jpeg,
                                                            ImageCategory.LOCATION,
                                                            200,
                                                            400,
                                                            URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                                        )
                                                    },
                                                    new(
                                                        true,
                                                        new[] {
                                                            new EnergySource(
                                                                EnergySourceCategory.SOLAR,
                                                                80
                                                            ),
                                                            new EnergySource(
                                                                EnergySourceCategory.WIND,
                                                                20
                                                            )
                                                        },
                                                        new[] {
                                                            new EnvironmentalImpact(
                                                                EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                0.1
                                                            )
                                                        },
                                                        "Stadtwerke Jena-Ost",
                                                        "New Green Deal"
                                                    ),
                                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                                ),
                                                SkipNotifications: true
                                            );

                #endregion


                var response = await graphDefinedCPO.GetLocations();

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // Connection:                    close
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882

                // HTTP/1.1 200 OK
                // Date:                          Tue, 18 Apr 2023 03:41:28 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // X-Total-Count:                 1
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                5494
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882
                // 
                // {
                //     "data":            [{"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefiend","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"}],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-18T03:41:28.838Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response.StatusCode);
                Assert.AreEqual ("Hello world!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

            }

        }

        #endregion


        #region EMSP_GetLocations_TariffDelegate_Test1()

        /// <summary>
        /// EMSP GetLocations using a tariff delegate, Test 1.
        /// </summary>
        [Test]
        public async Task EMSP_GetLocations_TariffDelegate_Test1()
        {

            var graphDefinedCPO = emspWebAPI1?.GetEMSPClient(
                                      CountryCode: CountryCode.Parse("DE"),
                                      PartyId:     Party_Id.   Parse("GEF")
                                  );

            Assert.IsNotNull(graphDefinedCPO);

            if (cpoWebAPI       is not null &&
                graphDefinedCPO is not null)
            {

                cpoWebAPI.CommonAPI.GetTariffIdsDelegate += (CPOCountryCode,
                                                             CPOPartyId,
                                                             Location,
                                                             EVSEUId,
                                                             ConnectorId,
                                                             EMPId) => {

                    if (EMPId.ToString() == "DE-GDF")
                        return new[] { Tariff_Id.Parse("AC1") };

                    // Will be called during a lot of internal calculations!
                    else
                        return Array.Empty<Tariff_Id>();

                };

                #region Define Location1

                cpoWebAPI.CommonAPI.AddLocation(new Location(
                                                    CountryCode.Parse("DE"),
                                                    Party_Id.Parse("GEF"),
                                                    Location_Id.Parse("LOC0001"),
                                                    true,
                                                    "Biberweg 18",
                                                    "Jena",
                                                    Country.Germany,
                                                    GeoCoordinate.Parse(10, 20),
                                                    "Europe/Berlin",
                                                    null,
                                                    "Location 0001",
                                                    "07749",
                                                    "Thüringen",
                                                    new[] {
                                                        new AdditionalGeoLocation(
                                                            Latitude.Parse(11),
                                                            Longitude.Parse(22),
                                                            Name: DisplayText.Create(Languages.de, "Postkasten")
                                                        )
                                                    },
                                                    ParkingType.PARKING_LOT,
                                                    new[] {
                                                        new EVSE(
                                                            EVSE_UId.Parse("DE*GEF*E*LOC0001*1"),
                                                            StatusType.AVAILABLE,
                                                            new[] {
                                                                new Connector(
                                                                    Connector_Id.Parse("1"),
                                                                    ConnectorType.IEC_62196_T2,
                                                                    ConnectorFormats.SOCKET,
                                                                    PowerTypes.AC_3_PHASE,
                                                                    400,
                                                                    30,
                                                                    12,
                                                                    null,
                                                                    URL.Parse("https://open.charging.cloud/terms"),
                                                                    DateTime.Parse("2020-09-21")
                                                                ),
                                                                new Connector(
                                                                    Connector_Id.Parse("2"),
                                                                    ConnectorType.IEC_62196_T2_COMBO,
                                                                    ConnectorFormats.CABLE,
                                                                    PowerTypes.AC_3_PHASE,
                                                                    400,
                                                                    20,
                                                                    8,
                                                                    null,
                                                                    URL.Parse("https://open.charging.cloud/terms"),
                                                                    DateTime.Parse("2020-09-22")
                                                                )
                                                            },
                                                            EVSE_Id.Parse("DE*GEF*E*LOC0001*1"),
                                                            new[] {
                                                                new StatusSchedule(
                                                                    StatusType.INOPERATIVE,
                                                                    DateTime.Parse("2020-09-23"),
                                                                    DateTime.Parse("2020-09-24")
                                                                ),
                                                                new StatusSchedule(
                                                                    StatusType.OUTOFORDER,
                                                                    DateTime.Parse("2020-12-30"),
                                                                    DateTime.Parse("2020-12-31")
                                                                )
                                                            },
                                                            new[] {
                                                                Capability.RFID_READER,
                                                                Capability.RESERVABLE
                                                            },

                                                            // OCPI Computer Science Extensions
                                                            new EnergyMeter(
                                                                Meter_Id.Parse("Meter0815"),
                                                                "EnergyMeter Model #1",
                                                                null,
                                                                "hw. v1.80",
                                                                "fw. v1.20",
                                                                "Energy Metering Services",
                                                                null,
                                                                null,
                                                                null
                                                            ),

                                                            "1. Stock",
                                                            GeoCoordinate.Parse(10.1, 20.2),
                                                            "Ladestation #1",
                                                            new[] {
                                                                DisplayText.Create(Languages.de, "Bitte klingeln!"),
                                                                DisplayText.Create(Languages.en, "Ken sent me!")
                                                            },
                                                            new ParkingRestrictions[] {
                                                                ParkingRestrictions.EV_ONLY,
                                                                ParkingRestrictions.PLUGGED
                                                            },
                                                            new[] {
                                                                new Image(
                                                                    URL.Parse("http://example.com/pinguine.jpg"),
                                                                    ImageFileType.jpeg,
                                                                    ImageCategory.OPERATOR,
                                                                    100,
                                                                    150,
                                                                    URL.Parse("http://example.com/kleine_pinguine.jpg")
                                                                )
                                                            },
                                                            DateTime.Parse("2020-09-22")
                                                        )
                                                    },
                                                    new[] {
                                                        new DisplayText(Languages.de, "Hallo Welt!"),
                                                        new DisplayText(Languages.en, "Hello world!")
                                                    },
                                                    new BusinessDetails(
                                                        "Open Charging Cloud",
                                                        URL.Parse("https://open.charging.cloud"),
                                                        new Image(
                                                            URL.Parse("http://open.charging.cloud/logo.svg"),
                                                            ImageFileType.svg,
                                                            ImageCategory.OPERATOR,
                                                            1000,
                                                            1500,
                                                            URL.Parse("http://open.charging.cloud/logo_small.svg")
                                                        )
                                                    ),
                                                    new BusinessDetails(
                                                        "GraphDefined GmbH",
                                                        URL.Parse("https://www.graphdefined.com"),
                                                        new Image(
                                                            URL.Parse("http://www.graphdefined.com/logo.png"),
                                                            ImageFileType.png,
                                                            ImageCategory.OPERATOR,
                                                            2000,
                                                            3000,
                                                            URL.Parse("http://www.graphdefined.com/logo_small.png")
                                                        )
                                                    ),
                                                    new BusinessDetails(
                                                        "Achim Friedland",
                                                        URL.Parse("https://ahzf.de"),
                                                        new Image(
                                                            URL.Parse("http://ahzf.de/logo.gif"),
                                                            ImageFileType.gif,
                                                            ImageCategory.OWNER,
                                                            3000,
                                                            4500,
                                                            URL.Parse("http://ahzf.de/logo_small.gif")
                                                        )
                                                    ),
                                                    new[] {
                                                        Facilities.CAFE
                                                    },
                                                    new Hours(
                                                        new[] {
                                                            new RegularHours(DayOfWeek.Monday,    new HourMin(08, 00), new HourMin(15, 00)),
                                                            new RegularHours(DayOfWeek.Tuesday,   new HourMin(09, 00), new HourMin(16, 00)),
                                                            new RegularHours(DayOfWeek.Wednesday, new HourMin(10, 00), new HourMin(17, 00)),
                                                            new RegularHours(DayOfWeek.Thursday,  new HourMin(11, 00), new HourMin(18, 00)),
                                                            new RegularHours(DayOfWeek.Friday,    new HourMin(12, 00), new HourMin(19, 00))
                                                        },
                                                        new[] {
                                                            new ExceptionalPeriod(
                                                                DateTime.Parse("2020-09-21T00:00:00Z"),
                                                                DateTime.Parse("2020-09-22T00:00:00Z")
                                                            )
                                                        },
                                                        new[] {
                                                            new ExceptionalPeriod(
                                                                DateTime.Parse("2020-12-24T00:00:00Z"),
                                                                DateTime.Parse("2020-12-26T00:00:00Z")
                                                            )
                                                        }
                                                    ),
                                                    false,
                                                    new[] {
                                                        new Image(
                                                            URL.Parse("http://open.charging.cloud/locations/location0001.jpg"),
                                                            ImageFileType.jpeg,
                                                            ImageCategory.LOCATION,
                                                            200,
                                                            400,
                                                            URL.Parse("http://open.charging.cloud/locations/location0001s.jpg")
                                                        )
                                                    },
                                                    new(
                                                        true,
                                                        new[] {
                                                            new EnergySource(
                                                                EnergySourceCategory.SOLAR,
                                                                80
                                                            ),
                                                            new EnergySource(
                                                                EnergySourceCategory.WIND,
                                                                20
                                                            )
                                                        },
                                                        new[] {
                                                            new EnvironmentalImpact(
                                                                EnvironmentalImpactCategory.CARBON_DIOXIDE,
                                                                0.1
                                                            )
                                                        },
                                                        "Stadtwerke Jena-Ost",
                                                        "New Green Deal"
                                                    ),
                                                    DateTime.Parse("2020-09-21T00:00:00Z")
                                                ),
                                                SkipNotifications: true
                                            );

                #endregion


                var response = await graphDefinedCPO.GetLocations();

                // GET /2.2.1/cpo/locations HTTP/1.1
                // Date:                          Tue, 18 Apr 2023 03:41:25 GMT
                // Accept:                        application/json; charset=utf-8;q=1
                // Host:                          127.0.0.1:7234
                // Authorization:                 Token xxxxxx
                // Connection:                    close
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882

                // HTTP/1.1 200 OK
                // Date:                          Tue, 18 Apr 2023 03:41:28 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // X-Total-Count:                 1
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                5494
                // X-Request-ID:                  1pKzWGQQd7EhYr9t7zf8dA7njt7W9h
                // X-Correlation-ID:              z5EKhQU7nK9UhYCMnCU4869zt5G882
                // 
                // {
                //     "data":            [{"id":"LOC0001","location_type":"PARKING_LOT","name":"Location 0001","address":"Biberweg 18","city":"Jena","postal_code":"07749","country":"DEU","coordinates":{"latitude":"10.00000","longitude":"20.00000"},"related_locations":[{"latitude":"11","longitude":"22","name":{"language":"de","text":"Postkasten"}}],"evses":[{"uid":"DE*GEF*E*LOC0001*1","evse_id":"DE*GEF*E*LOC0001*1","status":"AVAILABLE","status_schedule":[{"period_begin":"2020-09-22T22:00:00.000Z","period_end":"2020-09-23T22:00:00.000Z","status":"INOPERATIVE"},{"period_begin":"2020-12-29T23:00:00.000Z","period_end":"2020-12-30T23:00:00.000Z","status":"OUTOFORDER"}],"capabilities":["RFID_READER","RESERVABLE"],"connectors":[{"id":"1","standard":"IEC_62196_T2","format":"SOCKET","power_type":"AC_3_PHASE","voltage":400,"amperage":30,"tariff_id":"DE*GEF*T0001","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-20T22:00:00.000Z"},{"id":"2","standard":"IEC_62196_T2_COMBO","format":"CABLE","power_type":"AC_3_PHASE","voltage":400,"amperage":20,"tariff_id":"DE*GEF*T0003","terms_and_conditions":"https://open.charging.cloud/terms","last_updated":"2020-09-21T22:00:00.000Z"}],"energy_meter":{"id":"Meter0815","model":"EnergyMeter Model #1","hardware_version":"hw. v1.80","firmware_version":"fw. v1.20","manufacturer":"Energy Metering Services","transparency_softwares":[{"transparency_software":{"name":"Chargy Transparency Software Desktop Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyDesktopApp"},"legal_status":"GermanCalibrationLaw","certificate":"cert","certificate_issuer":"German PTB","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"},{"transparency_software":{"name":"Chargy Transparency Software Mobile Application","version":"v1.00","open_source_license":{"id":"AGPL-3.0","description":[{"language":"en","text":"GNU Affero General Public License version 3"}],"urls":["https://www.gnu.org/licenses/agpl-3.0.html","https://opensource.org/license/agpl-v3/"]},"vendor":"GraphDefined GmbH","logo":"https://open.charging.cloud/logo.svg","how_to_use":"https://open.charging.cloud/Chargy/howto","more_information":"https://open.charging.cloud/Chargy","source_code_repository":"https://github.com/OpenChargingCloud/ChargyMobileApp"},"legal_status":"ForInformationOnly","certificate":"no cert","certificate_issuer":"GraphDefiend","not_before":"2019-04-01T00:00:00.000Z","not_after":"2030-01-01T00:00:00.000Z"}],"last_updated":"2023-04-18T03:41:24.847Z"},"floor_level":"1. Stock","coordinates":{"latitude":"10.10000","longitude":"20.20000"},"physical_reference":"Ladestation #1","directions":[{"language":"de","text":"Bitte klingeln!"},{"language":"en","text":"Ken sent me!"}],"parking_restrictions":["EV_ONLY","PLUGGED"],"images":[{"url":"http://example.com/pinguine.jpg","category":"OPERATOR","type":"jpeg","thumbnail":"http://example.com/kleine_pinguine.jpg","width":100,"height":150}],"last_updated":"2020-09-21T22:00:00.000Z"}],"directions":[{"language":"de","text":"Hallo Welt!"},{"language":"en","text":"Hello world!"}],"operator":{"name":"Open Charging Cloud","website":"https://open.charging.cloud","logo":{"url":"http://open.charging.cloud/logo.svg","category":"OPERATOR","type":"svg","thumbnail":"http://open.charging.cloud/logo_small.svg","width":1000,"height":1500}},"suboperator":{"name":"GraphDefined GmbH","website":"https://www.graphdefined.com","logo":{"url":"http://www.graphdefined.com/logo.png","category":"OPERATOR","type":"png","thumbnail":"http://www.graphdefined.com/logo_small.png","width":2000,"height":3000}},"owner":{"name":"Achim Friedland","website":"https://ahzf.de","logo":{"url":"http://ahzf.de/logo.gif","category":"OWNER","type":"gif","thumbnail":"http://ahzf.de/logo_small.gif","width":3000,"height":4500}},"facilities":["CAFE"],"time_zone":"Europe/Berlin","opening_times":{"twentyfourseven":false,"regular_hours":[{"weekday":1,"period_begin":"08:00","period_end":"15:00"},{"weekday":2,"period_begin":"09:00","period_end":"16:00"},{"weekday":3,"period_begin":"10:00","period_end":"17:00"},{"weekday":4,"period_begin":"11:00","period_end":"18:00"},{"weekday":5,"period_begin":"12:00","period_end":"19:00"}],"exceptional_openings":[{"period_begin":"2020-09-21T00:00:00.000Z","period_end":"2020-09-22T00:00:00.000Z"}],"exceptional_closings":[{"period_begin":"2020-12-24T00:00:00.000Z","period_end":"2020-12-26T00:00:00.000Z"}]},"charging_when_closed":false,"images":[{"url":"http://open.charging.cloud/locations/location0001.jpg","category":"LOCATION","type":"jpeg","thumbnail":"http://open.charging.cloud/locations/location0001s.jpg","width":200,"height":400}],"energy_mix":{"is_green_energy":true,"energy_sources":[{"source":"SOLAR","percentage":80.0},{"source":"WIND","percentage":20.0}],"environ_impact":[{"category":"CARBON_DIOXIDE","amount":0.1}],"supplier_name":"Stadtwerke Jena-Ost","energy_product_name":"New Green Deal"},"last_updated":"2020-09-21T00:00:00.000Z"}],
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2023-04-18T03:41:28.838Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (200,             response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response.StatusCode);
                Assert.AreEqual ("Hello world!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response.Timestamp < TimeSpan.FromSeconds(10));

                Assert.IsTrue   (response.Data.First().EVSEs.First().Connectors.First().TariffIds.Contains(Tariff_Id.Parse("AC1")));

                //Assert.IsNotNull(response.Request);

            }

        }

        #endregion


    }

}