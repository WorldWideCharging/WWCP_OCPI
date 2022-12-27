﻿/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    [TestFixture]
    public class CPOTests : ANodeTests
    {

        #region CPO_GetVersions_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_Test()
        {

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response = await graphDefinedEMSP.GetVersions();

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
                // X-Request-ID:                  nM7djM37h56hQz8t8hKMznnhGYj3CK
                // X-Correlation-ID:              53YKxAnt2zM9bGp2AvjK6t83txbCK3
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
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

                //Assert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual (1, response.Data.Count());

                var version = versions.First();
                Assert.AreEqual (Version_Id.Parse("2.1.1"),    version.Id);
                Assert.AreEqual (emspVersionsAPIURL + "2.1.1", version.URL);

            }

        }

        #endregion

        #region CPO_GetVersions_UnknownToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_UnknownToken_Test()
        {

            #region Change Access Token

            cpoWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));

            var result = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("aaaaaa"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("bbbbbb"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response = await graphDefinedEMSP.GetVersions();

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
                // X-Request-ID:                  nM7djM37h56hQz8t8hKMznnhGYj3CK
                // X-Correlation-ID:              53YKxAnt2zM9bGp2AvjK6t83txbCK3
                // 
                // {
                //     "data": [{
                //         "version":  "2.1.1",
                //         "url":      "http://127.0.0.1:7135/versions/2.1.1"
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

                //Assert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual (1, response.Data.Count());

                var version = versions.First();
                Assert.AreEqual (Version_Id.Parse("2.1.1"),    version.Id);
                Assert.AreEqual (emspVersionsAPIURL + "2.1.1", version.URL);

            }

        }

        #endregion

        #region CPO_GetVersions_BlockedToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersions_BlockedToken_Test()
        {

            #region Block Access Token

            cpoWebAPI. CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));
            emspWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

            var addEMSPResult = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("xxxxxx"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("yyyyyy"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addEMSPResult);


            var addCPOResult = emspWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GEF"),
                Role:               Roles.      CPO,
                BusinessDetails:    new BusinessDetails("GraphDefined CPO Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("yyyyyy"),
                                            AccessStatus.BLOCKED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("xxxxxx"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addCPOResult);

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response = await graphDefinedEMSP.GetVersions();

                // HTTP/1.1 403 Forbidden
                // Date:                          Sun, 25 Dec 2022 22:44:01 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                111
                // X-Request-ID:                  KtUbA39Ad22pQ2Qt1MEShtEUrpfS14
                // X-Correlation-ID:              1ppv79Yj5QU69E2j9vG4z6GSb46r8z
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "Invalid or blocked access token!",
                //     "timestamp":      "2022-12-25T22:44:01.747Z"
                // }

                Assert.IsNotNull(response);
                Assert.AreEqual (403,                                 response.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (2000,                                response.StatusCode);
                Assert.AreEqual ("Invalid or blocked access token!",  response.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var versions = response.Data;
                Assert.IsNotNull(versions);
                Assert.AreEqual (0, versions.Count());

            }

        }

        #endregion


        #region CPO_GetVersion_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetVersion_Test()
        {

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetVersionDetails(Version_Id.Parse("2.1.1"));

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 00:36:21 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET
                // Access-Control-Allow-Headers:  Authorization
                // Vary:                          Accept
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                653
                // X-Request-ID:                  MpG9fA2Mjr89K16r82phMA18r83CS9
                // X-Correlation-ID:              S1p1rKhhh96vEK8A8t84Sht382KE8f
                // 
                // {
                //     "data": {
                //         "version": "2.1.1",
                //         "endpoints": [
                //             {
                //                 "identifier":  "credentials",
                //                 "url":         "http://127.0.0.1:7135/2.1.1/credentials"
                //             },
                //             {
                //                 "identifier":  "locations",
                //                 "url":         "http://127.0.0.1:7135/2.1.1/emsp/locations"
                //             },
                //             {
                //                 "identifier":  "tariffs",
                //                 "url":         "http://127.0.0.1:7135/2.1.1/emsp/tariffs"
                //             },
                //             {
                //                 "identifier":  "sessions",
                //                 "url":         "http://127.0.0.1:7135/2.1.1/emsp/sessions"
                //             },
                //             {
                //                 "identifier":  "cdrs",
                //                 "url":         "http://127.0.0.1:7135/2.1.1/emsp/cdrs"
                //             },
                //             {
                //                 "identifier":  "commands",
                //                 "url":         "http://127.0.0.1:7135/2.1.1/emsp/commands"
                //             },
                //             {
                //                 "identifier":  "tokens",
                //                 "url":         "http://127.0.0.1:7135/2.1.1/emsp/tokens"
                //             }
                //         ]
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-26T00:36:21.259Z"
                // }

                Assert.IsNotNull(response2);
                Assert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response2.StatusCode);
                Assert.AreEqual ("Hello world!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var versionDetail = response2.Data;
                Assert.IsNotNull(versionDetail);

                var endpoints = versionDetail.Endpoints;
                Assert.AreEqual (7, endpoints.Count());
                //Assert.AreEqual(Version_Id.Parse("2.1.1"), endpoints.Id);
                //Assert.AreEqual(emspVersionsAPIURL + "2.1.1", endpoints.URL);


            }

        }

        #endregion


        #region CPO_GetCredentials_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_Test()
        {

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 10:29:49 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  7AYph123pWAUt7j1Ad3n1jh1G279xG
                // X-Correlation-ID:              jhz1GGj3j83SE7Wrf42p8hM82rM3A3
                // 
                // {
                //    "data": {
                //        "token":         "yyyyyy",
                //        "url":           "http://127.0.0.1:7135/versions",
                //        "business_details": {
                //            "name":           "GraphDefined EMSP Services",
                //            "website":        "https://www.graphdefined.com/emsp"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GDF"
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
                Assert.AreEqual("yyyyyy",                             credentials.    Token.      ToString());
                Assert.AreEqual("http://127.0.0.1:7135/versions",     credentials.    URL.        ToString());
                Assert.AreEqual("DE",                                 credentials.    CountryCode.ToString());
                Assert.AreEqual("GDF",                                credentials.    PartyId.    ToString());

                var businessDetails = credentials.BusinessDetails;
                Assert.IsNotNull(businessDetails);
                Assert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                Assert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.    ToString());

            }

        }

        #endregion

        #region CPO_GetCredentials_UnknownToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_UnknownToken_Test()
        {

            #region Change Access Token

            cpoWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));

            var result = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("aaaaaa"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("bbbbbb"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                // HTTP/1.1 200 OK
                // Date:                          Mon, 26 Dec 2022 15:14:30 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                296
                // X-Request-ID:                  7AYph123pWAUt7j1Ad3n1jh1G279xG
                // X-Correlation-ID:              jhz1GGj3j83SE7Wrf42p8hM82rM3A3
                // 
                // {
                //    "data": {
                //        "token":         "<any>",
                //        "url":           "http://127.0.0.1:7135/versions",
                //        "business_details": {
                //            "name":           "GraphDefined EMSP Services",
                //            "website":        "https://www.graphdefined.com/emsp"
                //        },
                //        "country_code":  "DE",
                //        "party_id":      "GDF"
                //    },
                //    "status_code":      1000,
                //    "status_message":  "Hello world!",
                //    "timestamp":       "2022-12-26T15:14:30.143Z"
                //}

                Assert.IsNotNull(response2);
                Assert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response2.StatusCode);
                Assert.AreEqual ("Hello world!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.IsNotNull(credentials);
                Assert.AreEqual("<any>",                              credentials.    Token.      ToString());
                Assert.AreEqual("http://127.0.0.1:7135/versions",     credentials.    URL.        ToString());
                Assert.AreEqual("DE",                                 credentials.    CountryCode.ToString());
                Assert.AreEqual("GDF",                                credentials.    PartyId.    ToString());

                var businessDetails = credentials.BusinessDetails;
                Assert.IsNotNull(businessDetails);
                Assert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                Assert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.    ToString());

            }

        }

        #endregion

        #region CPO_GetCredentials_BlockedToken1_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_BlockedToken1_Test()
        {

            #region Block Access Token

            cpoWebAPI. CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));
            emspWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

            var addEMSPResult = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("xxxxxx"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("yyyyyy"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addEMSPResult);


            var addCPOResult = emspWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GEF"),
                Role:               Roles.      CPO,
                BusinessDetails:    new BusinessDetails("GraphDefined CPO Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("yyyyyy"),
                                            AccessStatus.BLOCKED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("xxxxxx"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addCPOResult);

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials();

                Assert.IsNotNull(response2);
                Assert.IsNull   (response2.HTTPResponse);
                Assert.AreEqual (-1,                         response2.StatusCode);
                Assert.AreEqual ("No versionId available!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.IsNull(credentials);
                //Assert.AreEqual("<any>",                              credentials.    Token.      ToString());
                //Assert.AreEqual("http://127.0.0.1:7135/versions",     credentials.    URL.        ToString());
                //Assert.AreEqual("DE",                                 credentials.    CountryCode.ToString());
                //Assert.AreEqual("GDF",                                credentials.    PartyId.    ToString());

                //var businessDetails = credentials.BusinessDetails;
                //Assert.IsNotNull(businessDetails);
                //Assert.AreEqual("GraphDefined EMSP Services",         businessDetails.Name);
                //Assert.AreEqual("https://www.graphdefined.com/emsp",  businessDetails.Website.    ToString());

            }

        }

        #endregion

        #region CPO_GetCredentials_BlockedToken2_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_BlockedToken2_Test()
        {

            #region Block Access Token

            cpoWebAPI. CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));
            emspWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

            var addEMSPResult = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("xxxxxx"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("yyyyyy"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addEMSPResult);


            var addCPOResult = emspWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GEF"),
                Role:               Roles.      CPO,
                BusinessDetails:    new BusinessDetails("GraphDefined CPO Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("yyyyyy"),
                                            AccessStatus.BLOCKED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("xxxxxx"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addCPOResult);

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.GetCredentials(Version_Id.Parse("2.1.1"));

                Assert.IsNotNull(response2);
                Assert.IsNull   (response2.HTTPResponse);
                Assert.AreEqual (-1,                          response2.StatusCode);
                Assert.AreEqual ("No remote URL available!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var credentials = response2.Data;
                Assert.IsNull(credentials);

            }

        }

        #endregion

        #region CPO_GetCredentials_BlockedToken3_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_GetCredentials_BlockedToken3_Test()
        {

            #region Block Access Token

            cpoWebAPI. CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));
            emspWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GEF"));

            var addEMSPResult = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("xxxxxx"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("yyyyyy"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addEMSPResult);


            var addCPOResult = emspWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GEF"),
                Role:               Roles.      CPO,
                BusinessDetails:    new BusinessDetails("GraphDefined CPO Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("yyyyyy"),
                                            AccessStatus.BLOCKED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("xxxxxx"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            Assert.IsTrue(addCPOResult);

            #endregion

            var httpResponse = await TestHelpers.JSONRequest(URL.Parse("http://127.0.0.1:7135/2.1.1/credentials"),
                                                             "yyyyyy");

            // HTTP/1.1 403 Forbidden
            // Date:                          Mon, 26 Dec 2022 15:43:44 GMT
            // Access-Control-Allow-Methods:  OPTIONS, GET
            // Access-Control-Allow-Headers:  Authorization
            // Server:                        GraphDefined Hermod HTTP Server v1.0
            // Access-Control-Allow-Origin:   *
            // Connection:                    close
            // Content-Type:                  application/json; charset=utf-8
            // Content-Length:                111
            // X-Request-ID:                  1234
            // X-Correlation-ID:              5678
            // 
            // {
            //     "status_code":      2000,
            //     "status_message":  "Invalid or blocked access token!",
            //     "timestamp":       "2022-12-26T15:43:44.533Z"
            // }

            Assert.IsNotNull(httpResponse);
            Assert.AreEqual (403,  httpResponse.HTTPStatusCode.Code);

            var jsonResponse = JObject.Parse(httpResponse.HTTPBodyAsUTF8String);
            Assert.IsNotNull(jsonResponse);

            Assert.AreEqual (2000,                                jsonResponse["status_code"]?.   Value<Int32>());
            Assert.AreEqual ("Invalid or blocked access token!",  jsonResponse["status_message"]?.Value<String>());
            Assert.IsTrue   (Timestamp.Now - jsonResponse["timestamp"]?.Value<DateTime>() < TimeSpan.FromSeconds(10));

            //Assert.IsNotNull(response.Request);

        }

        #endregion


        #region CPO_PutCredentials_NotYetRegistered_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutCredentials_NotYetRegistered_Test()
        {

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.PutCredentials(
                                                           new Credentials(
                                                               AccessToken.Parse("nnnnnn"),
                                                               URL.Parse("http://example.org/versions"),
                                                               new BusinessDetails(
                                                                   "Example Org",
                                                                   URL.Parse("http://example.org")
                                                               ),
                                                               CountryCode.Parse("DE"),
                                                               Party_Id.   Parse("EXP")
                                                           )
                                                       );

                // HTTP/1.1 405 Method Not Allowed
                // Date:                          Mon, 26 Dec 2022 15:29:55 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                151
                // X-Request-ID:                  zd4A4h2Kp6vY28nYQC1j616bd6569d
                // X-Correlation-ID:              An625Y7Yv1Un5K8AS33G2pUCbpCpjC
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "You need to be registered before trying to invoke this protected method!",
                //     "timestamp":      "2022-12-26T15:29:55.424Z"
                // }

                Assert.IsNotNull(response2);
                Assert.AreEqual (405,                                                                         response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (2000,                                                                        response2.StatusCode);
                Assert.AreEqual ("You need to be registered before trying to invoke this protected method!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                Assert.IsNull   (response2.Data);

            }

        }

        #endregion

        #region CPO_PutCredentials_UnknownToken_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_PutCredentials_UnknownToken_Test()
        {

            #region Change Access Token

            cpoWebAPI.CommonAPI.RemoveRemoteParty(CountryCode.Parse("DE"), Party_Id.Parse("GDF"));

            var result = cpoWebAPI.CommonAPI.AddRemoteParty(
                CountryCode:        CountryCode.Parse("DE"),
                PartyId:            Party_Id.   Parse("GDF"),
                Role:               Roles.      EMSP,
                BusinessDetails:    new BusinessDetails("GraphDefined EMSP Services"),
                AccessInfos:        new AccessInfo2[] {
                                        new AccessInfo2(
                                            AccessToken.Parse("aaaaaa"),
                                            AccessStatus.ALLOWED
                                        )
                                    },
                RemoteAccessInfos:  new RemoteAccessInfo[] {
                                        new RemoteAccessInfo(
                                            AccessToken:        AccessToken.Parse("bbbbbb"),
                                            VersionsURL:        emspVersionsAPIURL,
                                            VersionIds:         new Version_Id[] {
                                                                    Version_Id.Parse("2.1.1")
                                                                },
                                            SelectedVersionId:  Version_Id.Parse("2.1.1"),
                                            Status:             RemoteAccessStatus.ONLINE
                                        )
                                    },
                Status:             PartyStatus.ENABLED
            );

            #endregion

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var response1 = await graphDefinedEMSP.GetVersions();
                var response2 = await graphDefinedEMSP.PutCredentials(
                                                           new Credentials(
                                                               AccessToken.Parse("nnnnnn"),
                                                               URL.Parse("http://example.org/versions"),
                                                               new BusinessDetails(
                                                                   "Example Org",
                                                                   URL.Parse("http://example.org")
                                                               ),
                                                               CountryCode.Parse("DE"),
                                                               Party_Id.   Parse("EXP")
                                                           )
                                                       );

                // HTTP/1.1 405 Method Not Allowed
                // Date:                          Mon, 26 Dec 2022 15:29:55 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                151
                // X-Request-ID:                  zd4A4h2Kp6vY28nYQC1j616bd6569d
                // X-Correlation-ID:              An625Y7Yv1Un5K8AS33G2pUCbpCpjC
                // 
                // {
                //     "status_code":     2000,
                //     "status_message": "You need to be registered before trying to invoke this protected method!",
                //     "timestamp":      "2022-12-26T15:29:55.424Z"
                // }

                Assert.IsNotNull(response2);
                Assert.AreEqual (405,                                                                         response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (2000,                                                                        response2.StatusCode);
                Assert.AreEqual ("You need to be registered before trying to invoke this protected method!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now - response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                Assert.IsNull   (response2.Data);

            }

        }

        #endregion


        #region CPO_Register_RR_Test()

        /// <summary>
        /// CPO Test 01.
        /// </summary>
        [Test]
        public async Task CPO_Register_RR_Test()
        {

            var graphDefinedEMSP = cpoWebAPI?.GetCPOClient(
                                       CountryCode: CountryCode.Parse("DE"),
                                       PartyId:     Party_Id.   Parse("GDF")
                                   );

            Assert.IsNotNull(graphDefinedEMSP);

            if (graphDefinedEMSP is not null)
            {

                var remoteAccessInfoOld  = cpoWebAPI?.CommonAPI.RemoteParties.First().RemoteAccessInfos.First();
                var accessInfoOld        = emspWebAPI.CommonAPI.RemoteParties.First().AccessInfo.       First();

                var response1            = await graphDefinedEMSP.GetVersions();
                var response2            = await graphDefinedEMSP.Register();

                // HTTP/1.1 200 OK
                // Date:                          Tue, 27 Dec 2022 09:16:14 GMT
                // Access-Control-Allow-Methods:  OPTIONS, GET, POST, PUT, DELETE
                // Access-Control-Allow-Headers:  Authorization
                // Server:                        GraphDefined Hermod HTTP Server v1.0
                // Access-Control-Allow-Origin:   *
                // Connection:                    close
                // Content-Type:                  application/json; charset=utf-8
                // Content-Length:                340
                // X-Request-ID:                  vzY6jY44Gjf995rAC14487f3K1Wpr5
                // X-Correlation-ID:              Ev63U91239t523E1Q9xb41fA3dCzC4
                // 
                // {
                //     "data": {
                //         "token":         "tM1bM71zW39f9W46WM5K9W46h6rYtdYfK1nS46rjA6Cf9ffY9h",
                //         "url":           "http://127.0.0.1:7135/versions",
                //         "business_details": {
                //             "name":            "GraphDefined EMSP Services",
                //             "website":         "https://www.graphdefined.com/emsp"
                //         },
                //         "country_code":    "DE",
                //         "party_id":        "GDF"
                //     },
                //     "status_code":      1000,
                //     "status_message":  "Hello world!",
                //     "timestamp":       "2022-12-27T09:16:14.632Z"
                // }

                Assert.IsNotNull(response2);
                Assert.AreEqual (200,             response2.HTTPResponse?.HTTPStatusCode.Code);
                Assert.AreEqual (1000,            response2.StatusCode);
                Assert.AreEqual ("Hello world!",  response2.StatusMessage);
                Assert.IsTrue   (Timestamp.Now -  response2.Timestamp < TimeSpan.FromSeconds(10));

                //Assert.IsNotNull(response.Request);

                var remoteAccessInfoNew  = cpoWebAPI?.CommonAPI.RemoteParties.First().RemoteAccessInfos.First();
                Assert.IsNotNull  (remoteAccessInfoNew);
                Assert.AreNotEqual(remoteAccessInfoOld?.AccessToken.ToString(),  remoteAccessInfoNew?.AccessToken.ToString());

                var accessInfoNew        = emspWebAPI.CommonAPI.RemoteParties.First().AccessInfo.       First();

            }

        }

        #endregion


    }

}
