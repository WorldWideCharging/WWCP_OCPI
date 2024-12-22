﻿/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.UnitTests.RoamingTests
{

    [TestFixture]
    public class RoamingTests : ARoamingTests
    {

        #region Add_ChargingLocationsAndEVSEs_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the remote OCPI module.
        /// </summary>
        [Test]
        public async Task Add_ChargingLocationsAndEVSEs_Test1()
        {

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                graphDefinedCSO    is not null &&
                graphDefinedEMP    is not null &&
                exampleEMP         is not null)
            {

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create("Test pool #1"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #1"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult1);

                var chargingPool1  = addChargingPoolResult1.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool1);

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create("Test pool #2"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #2"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult2);

                var chargingPool2  = addChargingPoolResult2.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool2);

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create("Test station #1A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult1);

                var chargingStation1  = addChargingStationResult1.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create("Test station #1B"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult2);

                var chargingStation2  = addChargingStationResult2.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create("Test station #2A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult3);

                var chargingStation3  = addChargingStationResult3.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                // Will call OCPICSOAdapter.AddStaticData(EVSE, ...)!
                var addEVSE1Result1 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #1A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A1"),

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        Id:             ChargingConnector_Id.Parse(1),
                                                                        Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        Lockable:       true,
                                                                        CableAttached:  true,
                                                                        CableLength:    Meter.ParseM(3.0)
                                                                    )
                                                                ],

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                Assert.That(addEVSE1Result1,         Is.Not.Null);
                Assert.That(addEVSE1Result1.Result,  Is.EqualTo(org.GraphDefined.Vanaheimr.Illias.CommandResult.Success));

                var evse1 = addEVSE1Result1.EVSE;
                Assert.That(evse1,                   Is.Not.Null);

                if (chargingStation1 is not null && evse1 is not null)
                {

                    var ocpiLocation1  = cpoAdapter!.CommonAPI.GetLocations().FirstOrDefault(location => location.Name         == chargingPool1!.Name.FirstText());
                    Assert.That(ocpiLocation1,  Is.Not.Null);

                    var ocpiStation1   = ocpiLocation1!.ChargingPool.         FirstOrDefault(station  => station.Id.ToString() == chargingStation1.Id.ToString());
                    Assert.That(ocpiStation1,   Is.Not.Null);

                    var ocpiEVSE1      = ocpiStation1!.                       FirstOrDefault(evse     => evse.  UId.ToString() == evse1.           Id.ToString());
                    Assert.That(ocpiEVSE1,      Is.Not.Null);

                    if (ocpiEVSE1 is not null)
                    {

                        Assert.That(ocpiEVSE1.EVSEId.ToString(), Is.EqualTo(evse1.Id.ToString()));

                    }

                }

                Assert.That(cpoAdapter!.CommonAPI.GetLocations().SelectMany(location => location.ChargingPool).SelectMany(station => station.EVSEs).Count(), Is.EqualTo(1));

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create("Test EVSE #1A2"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A2"),

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        Id:             ChargingConnector_Id.Parse(1),
                                                                        Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        Lockable:       true,
                                                                        CableAttached:  true,
                                                                        CableLength:    Meter.ParseM(3.0)
                                                                    )
                                                                ],

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result2);

                var evse2 = addEVSE1Result2.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create("Test EVSE #1B1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1B1"),

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        Id:             ChargingConnector_Id.Parse(1),
                                                                        Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        Lockable:       true,
                                                                        CableAttached:  true,
                                                                        CableLength:    Meter.ParseM(3.0)
                                                                    )
                                                                ],

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result3);

                var evse3 = addEVSE1Result3.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #2A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #2A1"),

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        Id:             ChargingConnector_Id.Parse(1),
                                                                        Plug:           ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        Lockable:       true,
                                                                        CableAttached:  true,
                                                                        CableLength:    Meter.ParseM(3.0)
                                                                    )
                                                                ],

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result4);

                var evse4 = addEVSE1Result4.EVSE;
                ClassicAssert.IsNotNull(evse4);

                #endregion


                //ToDo: There seems to be a timing issue!
                await Task.Delay(500);


                #region Validate, that all locations had been sent to the CPO OCPI module

                var allLocationsAtCPO = cpoAdapter.CommonAPI.GetLocations().ToArray();
                Assert.That(allLocationsAtCPO,                  Is.Not.Null);
                Assert.That(allLocationsAtCPO.Length,           Is.EqualTo(2));

                #endregion

                #region Validate, that all charging stations had been sent to the CPO OCPI module

                var allChargingStationsAtCPO = cpoAdapter.CommonAPI.GetLocations().SelectMany(location => location.ChargingPool).ToArray();
                Assert.That(allChargingStationsAtCPO,           Is.Not.Null);
                Assert.That(allChargingStationsAtCPO.Length,    Is.EqualTo(3));

                #endregion

                #region Validate, that all EVSEs had been sent to the CPO OCPI module

                var allEVSEsAtCPO = cpoAdapter.CommonAPI.GetLocations().SelectMany(location => location.ChargingPool.SelectMany(station => station.EVSEs)).ToArray();
                Assert.That(allEVSEsAtCPO,                      Is.Not.Null);
                Assert.That(allEVSEsAtCPO.Length,               Is.EqualTo(4));

                #endregion

                #region Validate, that both locations have the correct number of Charging Stations and EVSEs

                if (cpoAdapter.CommonAPI.TryGetLocation(Party_Idv3.   Parse("DEGEF"),
                                                        Location_Id.Parse(chargingPool1!.Id.ToString()),
                                                        out var location1) &&
                    location1 is not null)
                {

                    Assert.That(location1.ChargingPool.Count(),                                       Is.EqualTo(2));
                    Assert.That(location1.ChargingPool.SelectMany(station => station.EVSEs).Count(),  Is.EqualTo(3));

                }
                else
                    Assert.Fail("location1 was not found!");


                if (cpoAdapter.CommonAPI.TryGetLocation(Party_Idv3.   Parse("DEGEF"),
                                                        Location_Id.Parse(chargingPool2!.Id.ToString()),
                                                        out var location2) &&
                    location2 is not null)
                {

                    Assert.That(location2.ChargingPool.Count(),                                       Is.EqualTo(1));
                    Assert.That(location2.ChargingPool.SelectMany(station => station.EVSEs).Count(),  Is.EqualTo(1));

                }
                else
                    Assert.Fail("location1 was not found!");

                #endregion


                var remoteURL = URL.Parse("http://127.0.0.1:3473/ocpi/v3.0//locations");

                #region Validate via HTTP (OpenData, no authorization)

                {

                    var httpResponse = await new HTTPSClient(remoteURL).
                                                  Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                         remoteURL.Path,
                                                                                         RequestBuilder: requestBuilder => {
                                                                                             requestBuilder.Connection = ConnectionType.Close;
                                                                                             requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                             requestBuilder.Set("X-Request-ID",      "123");
                                                                                             requestBuilder.Set("X-Correlation-ID",  "123");
                                                                                         })).
                                                  ConfigureAwait(false);

                    ClassicAssert.IsNotNull(httpResponse);
                    ClassicAssert.AreEqual (200,             httpResponse.HTTPStatusCode.Code);

                    var ocpiResponse  = JObject.Parse(httpResponse.HTTPBody.ToUTF8String());

                    ClassicAssert.AreEqual (1000,            ocpiResponse!["status_code"]!.   Value<Int32>() );
                    ClassicAssert.AreEqual ("Hello world!",  ocpiResponse!["status_message"]!.Value<String>());
                    ClassicAssert.IsTrue   (Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10));

                    var jsonLocations = ocpiResponse!["data"] as JArray;
                    ClassicAssert.IsNotNull(jsonLocations);
                    ClassicAssert.AreEqual(2, jsonLocations!.Count);

                    var jsonLocation1 = jsonLocations[0] as JObject;
                    var jsonLocation2 = jsonLocations[1] as JObject;
                    ClassicAssert.IsNotNull(jsonLocation1);
                    ClassicAssert.IsNotNull(jsonLocation2);

                    var jsonEVSEs1    = jsonLocation1!["evses"] as JArray;
                    var jsonEVSEs2    = jsonLocation2!["evses"] as JArray;
                    ClassicAssert.IsNotNull(jsonEVSEs1);
                    ClassicAssert.IsNotNull(jsonEVSEs2);

                    ClassicAssert.AreEqual(3, jsonEVSEs1!.Count);
                    ClassicAssert.AreEqual(1, jsonEVSEs2!.Count);

                }

                #endregion

                #region Validate via HTTP (with authorization)

                await cpoAdapter.CommonAPI.AddRemoteParty(
                          Id:                RemoteParty_Id.Parse("DE-GDF_EMSP"),
                          CredentialsRoles:  [
                                                 new CredentialsRole(
                                                     PartyId:          Party_Idv3.Parse("DEGDF"),
                                                     Role:             Roles.EMSP,
                                                     BusinessDetails:  new BusinessDetails(
                                                                           "GraphDefined EMSP"
                                                                       )
                                                 )
                                             ],
                          AccessToken:       AccessToken.Parse("1234xyz"),
                          AccessStatus:      AccessStatus.ALLOWED,
                          PartyStatus:       PartyStatus.ENABLED
                      );

                {

                    var httpResponse = await new HTTPSClient(remoteURL).
                                                  Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                         remoteURL.Path,
                                                                                         RequestBuilder: requestBuilder => {
                                                                                             requestBuilder.Authorization  = HTTPTokenAuthentication.Parse("1234xyz");
                                                                                             requestBuilder.Connection     = ConnectionType.Close;
                                                                                             requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                             requestBuilder.Set("X-Request-ID",      "123");
                                                                                             requestBuilder.Set("X-Correlation-ID",  "123");
                                                                                         })).
                                                  ConfigureAwait(false);

                    ClassicAssert.IsNotNull(httpResponse);
                    ClassicAssert.AreEqual (200,             httpResponse.HTTPStatusCode.Code);

                    var ocpiResponse  = JObject.Parse(httpResponse.HTTPBody.ToUTF8String());

                    ClassicAssert.AreEqual (1000,            ocpiResponse!["status_code"]!.   Value<Int32>() );
                    ClassicAssert.AreEqual ("Hello world!",  ocpiResponse!["status_message"]!.Value<String>());
                    ClassicAssert.IsTrue   (Timestamp.Now -  httpResponse.Timestamp < TimeSpan.FromSeconds(10));

                    var jsonLocations = ocpiResponse!["data"] as JArray;
                    ClassicAssert.IsNotNull(jsonLocations);
                    ClassicAssert.AreEqual(2, jsonLocations!.Count);

                    var jsonLocation1 = jsonLocations[0] as JObject;
                    var jsonLocation2 = jsonLocations[1] as JObject;
                    ClassicAssert.IsNotNull(jsonLocation1);
                    ClassicAssert.IsNotNull(jsonLocation2);

                    var jsonEVSEs1    = jsonLocation1!["evses"] as JArray;
                    var jsonEVSEs2    = jsonLocation2!["evses"] as JArray;
                    ClassicAssert.IsNotNull(jsonEVSEs1);
                    ClassicAssert.IsNotNull(jsonEVSEs2);

                    ClassicAssert.AreEqual(3, jsonEVSEs1!.Count);
                    ClassicAssert.AreEqual(1, jsonEVSEs2!.Count);

                }

                #endregion

            }

        }

        #endregion

        #region Update_ChargingLocationsAndEVSEs_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs and update their static data.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_ChargingLocationsAndEVSEs_Test1()
        {

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                graphDefinedCSO    is not null &&
                graphDefinedEMP    is not null &&
                exampleEMP         is not null)
            {

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create("Test pool #1"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #1"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult1);

                var chargingPool1  = addChargingPoolResult1.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool1);

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create("Test pool #2"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #2"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult2);

                var chargingPool2  = addChargingPoolResult2.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool2);

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create("Test station #1A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult1);

                var chargingStation1  = addChargingStationResult1.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create("Test station #1B"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult2);

                var chargingStation2  = addChargingStationResult2.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create("Test station #2A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult3);

                var chargingStation3  = addChargingStationResult3.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                var addEVSE1Result1 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #1A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.CHAdeMO
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result1);

                var evse1     = addEVSE1Result1.EVSE;
                ClassicAssert.IsNotNull(evse1);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create("Test EVSE #1A2"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A2"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Outlet
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result2);

                var evse2     = addEVSE1Result2.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create("Test EVSE #1B1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1B1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.TypeFSchuko
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result3);

                var evse3     = addEVSE1Result3.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #2A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #2A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        CableAttached: true
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result4);

                var evse4     = addEVSE1Result4.EVSE;
                ClassicAssert.IsNotNull(evse4);

                #endregion



                #region Validate, that locations had been sent to the OCPI module

                var allLocations  = cpoCommonAPI.GetLocations().ToArray();
                ClassicAssert.IsNotNull(allLocations);
                ClassicAssert.AreEqual (2, allLocations.Length);

                #endregion

                #region Validate, that charging stations had been sent to the OCPI module

                var allChargingStations = cpoCommonAPI.GetLocations().SelectMany(location => location.ChargingPool).ToArray();
                ClassicAssert.IsNotNull(allChargingStations);
                ClassicAssert.AreEqual(3, allChargingStations.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the OCPI module

                var allEVSEs      = cpoCommonAPI.GetLocations().SelectMany(location => location.ChargingPool.SelectMany(station => station.EVSEs)).ToArray();
                ClassicAssert.IsNotNull(allEVSEs);
                ClassicAssert.AreEqual (4, allEVSEs.Length);

                #endregion

                #region Validate, that both locations have Stations and EVSEs

                if (cpoCommonAPI.TryGetLocation(chargingPool1.Operator.Id.ToOCPI(),
                                                Location_Id.Parse(chargingPool1.Id.ToString()),
                                                out var location1) &&
                    location1 is not null)
                {

                    ClassicAssert.AreEqual(2, location1.ChargingPool.Count());
                    ClassicAssert.AreEqual(3, location1.ChargingPool.SelectMany(station => station.EVSEs).Count());

                }
                else
                    Assert.Fail("location1 was not found!");


                if (cpoCommonAPI.TryGetLocation(chargingPool2.Operator.Id.ToOCPI(),
                                                Location_Id.Parse(chargingPool2.Id.ToString()),
                                                out var location2) &&
                    location2 is not null)
                {

                    ClassicAssert.AreEqual(1, location2.ChargingPool.Count());
                    ClassicAssert.AreEqual(1, location2.ChargingPool.SelectMany(station => station.EVSEs).Count());

                }
                else
                    Assert.Fail("location2 was not found!");

                #endregion



                #region Update Add DE*GEF*POOL2, DE*GEF*STATION*2*A, DE*GEF*POOL2

                var updatedPoolProperties     = new List<PropertyUpdateInfo<ChargingPool_Id>>();
                var updatedStationProperties  = new List<PropertyUpdateInfo<WWCP.ChargingStation_Id>>();
                var updatedEVSEProperties     = new List<PropertyUpdateInfo<WWCP.EVSE_Id>>();

                #region Subscribe charging pool events

                chargingPool1!.OnPropertyChanged += (timestamp,
                                                     eventTrackingId,
                                                     chargingPool,
                                                     propertyName,
                                                     newValue,
                                                     oldValue,
                                                     dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>((chargingPool as IChargingPool)!.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                chargingPool1!.OnDataChanged += (timestamp,
                                                 eventTrackingId,
                                                 chargingPool,
                                                 propertyName,
                                                 newValue,
                                                 oldValue,
                                                 dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingPoolDataChanged += (timestamp,
                                                              eventTrackingId,
                                                              chargingPool,
                                                              propertyName,
                                                              newValue,
                                                              oldValue,
                                                              dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnChargingPoolDataChanged  += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingPool,
                                                                 propertyName,
                                                                 newValue,
                                                                 oldValue,
                                                                 dataSource) => {

                    updatedPoolProperties.Add(new PropertyUpdateInfo<ChargingPool_Id>(chargingPool.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe charging station events

                chargingStation1!.OnPropertyChanged += (timestamp,
                                                        eventTrackingId,
                                                        chargingStation,
                                                        propertyName,
                                                        newValue,
                                                        oldValue,
                                                        dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>((chargingStation as IChargingStation)!.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                chargingStation1!.OnDataChanged += (timestamp,
                                                    eventTrackingId,
                                                    chargingStation,
                                                    propertyName,
                                                    newValue,
                                                    oldValue,
                                                    dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnChargingStationDataChanged += (timestamp,
                                                                 eventTrackingId,
                                                                 chargingStation,
                                                                 propertyName,
                                                                 newValue,
                                                                 oldValue,
                                                                 dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnChargingStationDataChanged += (timestamp,
                                                                   eventTrackingId,
                                                                   chargingStation,
                                                                   propertyName,
                                                                   newValue,
                                                                   oldValue,
                                                                   dataSource) => {

                    updatedStationProperties.Add(new PropertyUpdateInfo<WWCP.ChargingStation_Id>(chargingStation.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe EVSE events

                evse1!.OnPropertyChanged += (timestamp,
                                             eventTrackingId,
                                             evse,
                                             propertyName,
                                             newValue,
                                             oldValue,
                                             dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>((evse as IEVSE)!.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                evse1!.OnDataChanged += (timestamp,
                                         eventTrackingId,
                                         evse,
                                         propertyName,
                                         newValue,
                                         oldValue,
                                         dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEDataChanged += (timestamp,
                                                      eventTrackingId,
                                                      evse,
                                                      propertyName,
                                                      newValue,
                                                      oldValue,
                                                      dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnEVSEDataChanged += (timestamp,
                                                        eventTrackingId,
                                                        evse,
                                                        propertyName,
                                                        newValue,
                                                        oldValue,
                                                        dataSource) => {

                    updatedEVSEProperties.Add(new PropertyUpdateInfo<WWCP.EVSE_Id>(evse.Id, propertyName, newValue, oldValue, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                cpoCommonAPI.OnLocationChanged += async (location) => { };
                cpoCommonAPI.OnEVSEChanged     += async (evse)     => { };


                chargingPool1!.Name.       Set(Languages.en, "Test pool #1 (updated)");
                chargingPool1!.Description.Set(Languages.en, "GraphDefined charging pool for tests #1 (updated)");

                ClassicAssert.AreEqual(8, updatedPoolProperties.Count);
                ClassicAssert.AreEqual("Test pool #1 (updated)",                             graphDefinedCSO.GetChargingPoolById(chargingPool1!.Id)!.Name       [Languages.en]);
                ClassicAssert.AreEqual("GraphDefined charging pool for tests #1 (updated)",  graphDefinedCSO.GetChargingPoolById(chargingPool1!.Id)!.Description[Languages.en]);

                cpoCommonAPI.TryGetLocation(chargingPool1.Operator.Id.ToOCPI(),
                                            Location_Id.Parse(chargingPool1!.Id.Suffix),
                                            out var location);

                ClassicAssert.AreEqual("Test pool #1 (updated)",                             location!.Name);
                //ClassicAssert.AreEqual("GraphDefined Charging Pool für Tests #1",            location!.Name); // Not mapped to OCPI!


                evse1.Name.Set(Languages.en, "Test EVSE #1A1 (updated)");















                //var updateChargingPoolResult2 = await graphDefinedCSO.UpdateChargingPool()


                //var updateChargingPoolResult2 = await chargingPool2.Address = new Address(

                //                                                                  Street:             "Biberweg",
                //                                                                  PostalCode:         "07749",
                //                                                                  City:               I18NString.Create(Languages.de, "Jena"),
                //                                                                  Country:            Country.Germany,

                //                                                                  HouseNumber:        "18",
                //                                                                  FloorLevel:         null,
                //                                                                  Region:             null,
                //                                                                  PostalCodeSub:      null,
                //                                                                  TimeZone:           null,
                //                                                                  OfficialLanguages:  null,
                //                                                                  Comment:            null,

                //                                                                  CustomData:         null,
                //                                                                  InternalData:       null

                //                                                              );

                //ClassicAssert.IsNotNull(addChargingPoolResult2);

                //var chargingPool2  = addChargingPoolResult2.ChargingPool;
                //ClassicAssert.IsNotNull(chargingPool2);

                #endregion



            }

        }

        #endregion

        #region Update_EVSEStatus_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs and update the EVSE status.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task Update_EVSEStatus_Test1()
        {

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                graphDefinedCSO    is not null &&
                graphDefinedEMP    is not null &&
                exampleEMP         is not null)
            {

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create("Test pool #1"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #1"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult1);

                var chargingPool1  = addChargingPoolResult1.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool1);

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create("Test pool #2"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #2"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult2);

                var chargingPool2  = addChargingPoolResult2.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool2);

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create("Test station #1A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult1);

                var chargingStation1  = addChargingStationResult1.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create("Test station #1B"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult2);

                var chargingStation2  = addChargingStationResult2.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create("Test station #2A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult3);

                var chargingStation3  = addChargingStationResult3.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                var addEVSE1Result1 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #1A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.CHAdeMO
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result1);

                var evse1     = addEVSE1Result1.EVSE;
                ClassicAssert.IsNotNull(evse1);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create("Test EVSE #1A2"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A2"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Outlet
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result2);

                var evse2     = addEVSE1Result2.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create("Test EVSE #1B1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1B1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.TypeFSchuko
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result3);

                var evse3     = addEVSE1Result3.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #2A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #2A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        CableAttached: true
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result4);

                var evse4     = addEVSE1Result4.EVSE;
                ClassicAssert.IsNotNull(evse4);

                #endregion


                var evse1_UId = evse1!.Id.ToOCPI_EVSEUId();
                ClassicAssert.IsTrue(evse1_UId.HasValue);


                #region Validate, that locations had been sent to the OCPI module

                var allLocations  = cpoCommonAPI.GetLocations().ToArray();
                ClassicAssert.IsNotNull(allLocations);
                ClassicAssert.AreEqual (2, allLocations.Length);

                #endregion

                #region Validate, that charging stations had been sent to the OCPI module

                var allChargingStations = cpoCommonAPI.GetLocations().SelectMany(location => location.ChargingPool).ToArray();
                ClassicAssert.IsNotNull(allChargingStations);
                ClassicAssert.AreEqual(3, allChargingStations.Length);

                #endregion

                #region Validate, that EVSEs had been sent to the OCPI module

                var allEVSEs      = cpoCommonAPI.GetLocations().SelectMany(location => location.ChargingPool.SelectMany(station => station.EVSEs)).ToArray();
                ClassicAssert.IsNotNull(allEVSEs);
                ClassicAssert.AreEqual (4, allEVSEs.Length);

                #endregion

                #region Validate, that both locations have EVSEs

                if (cpoCommonAPI.TryGetLocation(chargingPool1.Operator.Id.ToOCPI(),
                                                Location_Id.Parse(chargingPool1.Id.Suffix),
                                                out var location1) &&
                    location1 is not null)
                {

                    ClassicAssert.AreEqual(3, location1.ChargingPool.SelectMany(station => station.EVSEs).Count());

                }
                else
                    Assert.Fail("location1 was not found!");


                if (cpoCommonAPI.TryGetLocation(chargingPool2.Operator.Id.ToOCPI(),
                                                        Location_Id.Parse(chargingPool2.Id.Suffix),
                                                        out var location2) &&
                    location2 is not null)
                {

                    ClassicAssert.AreEqual(1, location2.ChargingPool.SelectMany(station => station.EVSEs).Count());

                }
                else
                    Assert.Fail("location2 was not found!");

                #endregion



                #region Subscribe WWCP EVSE events

                var updatedEVSEStatus         = new List<EVSEStatusUpdate>();

                evse1!.OnStatusChanged += (timestamp,
                                           eventTrackingId,
                                           evse,
                                           newEVSEStatus,
                                           oldEVSEStatus,
                                           dataSource) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                graphDefinedCSO.OnEVSEStatusChanged += (timestamp,
                                                        eventTrackingId,
                                                        evse,
                                                        newEVSEStatus,
                                                        oldEVSEStatus,
                                                        dataSource) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                csoRoamingNetwork.OnEVSEStatusChanged += (timestamp,
                                                          eventTrackingId,
                                                          evse,
                                                          newEVSEStatus,
                                                          oldEVSEStatus,
                                                          dataSource) => {

                    updatedEVSEStatus.Add(new EVSEStatusUpdate(evse.Id, newEVSEStatus, oldEVSEStatus, dataSource));
                    return Task.CompletedTask;

                };

                #endregion

                #region Subscribe OCPI EVSE events

                var updatedOCPIEVSEStatus = new List<StatusType>();

                cpoCommonAPI.OnEVSEChanged += (evse) => {

                    updatedOCPIEVSEStatus.Add(evse.Status);
                    return Task.CompletedTask;

                };

                cpoCommonAPI.OnEVSEStatusChanged += (timestamp,
                                                             evse,
                                                             oldEVSEStatus,
                                                             newEVSEStatus) => {

                    updatedOCPIEVSEStatus.Add(newEVSEStatus);
                    return Task.CompletedTask;

                };

                #endregion

                #region Update

                {
                    if (evse1_UId.HasValue &&
                        cpoCommonAPI.TryGetLocation(chargingPool1.Operator.Id.ToOCPI(),
                                                    Location_Id.Parse(chargingPool1!.Id.Suffix),
                                                    out var location) &&
                        location is not null &&
                        location.TryGetChargingStation(chargingStation1.Id.ToOCPI(), out var s1) &&
                        s1.      TryGetEVSE           (evse1_UId.Value, out var ocpiEVSE) && ocpiEVSE is not null)
                    {
                        ClassicAssert.AreEqual(StatusType.AVAILABLE, ocpiEVSE.Status);
                    }
                }



                evse1.SetStatus(EVSEStatusType.Charging);



                ClassicAssert.AreEqual(3, updatedEVSEStatus.    Count);
                ClassicAssert.AreEqual(2, updatedOCPIEVSEStatus.Count);

                ClassicAssert.AreEqual(EVSEStatusType.Charging,  graphDefinedCSO.GetEVSEById(evse1!.Id)?.Status.Value);

                {
                    if (evse1_UId.HasValue &&
                        cpoCommonAPI.TryGetLocation(chargingPool1.Operator.Id.ToOCPI(),
                                                    Location_Id.Parse(chargingPool1!.Id.Suffix),
                                                    out var location) &&
                        location is not null &&
                        location.TryGetChargingStation(chargingStation1.Id.ToOCPI(), out var s1) &&
                        s1.TryGetEVSE(evse1_UId.Value, out var ocpiEVSE) && ocpiEVSE is not null)
                    {
                        ClassicAssert.AreEqual(StatusType.CHARGING, ocpiEVSE.Status);
                    }
                }

                #endregion


            }

        }

        #endregion


        #region AuthStart_Test1()

        /// <summary>
        /// Add WWCP charging locations, stations and EVSEs and update the EVSE status.
        /// Validate that they had been sent to the OCPI module.
        /// Validate via HTTP that they are present within the OCPI module.
        /// </summary>
        [Test]
        public async Task AuthStart_Test1()
        {

            if (csoRoamingNetwork  is not null &&
                emp1RoamingNetwork is not null &&
                emp2RoamingNetwork is not null &&

                cpoCPOAPI          is not null &&
                emsp1EMSPAPI       is not null &&
                emsp2EMSPAPI       is not null &&

                graphDefinedCSO    is not null)
                //graphDefinedEMP    is not null &&
                //exampleEMP         is not null)
            {

                #region Add DE*GEF*POOL1

                var addChargingPoolResult1 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL1"),
                                                 Name:                 I18NString.Create("Test pool #1"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #1"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult1);

                var chargingPool1  = addChargingPoolResult1.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool1);

                #endregion

                #region Add DE*GEF*POOL2

                var addChargingPoolResult2 = await graphDefinedCSO.AddChargingPool(

                                                 Id:                   ChargingPool_Id.Parse("DE*GEF*POOL2"),
                                                 Name:                 I18NString.Create("Test pool #2"),
                                                 Description:          I18NString.Create("GraphDefined charging pool for tests #2"),

                                                 Address:              new org.GraphDefined.Vanaheimr.Illias.Address(

                                                                           Street:             "Biberweg",
                                                                           PostalCode:         "07749",
                                                                           City:               I18NString.Create(Languages.de, "Jena"),
                                                                           Country:            Country.Germany,

                                                                           HouseNumber:        "18",
                                                                           FloorLevel:         null,
                                                                           Region:             null,
                                                                           PostalCodeSub:      null,
                                                                           TimeZone:           null,
                                                                           OfficialLanguages:  null,
                                                                           Comment:            null,

                                                                           CustomData:         null,
                                                                           InternalData:       null

                                                                       ),
                                                 GeoLocation:          GeoCoordinate.Parse(50.93, 11.63),

                                                 InitialAdminStatus:   ChargingPoolAdminStatusTypes.Operational,
                                                 InitialStatus:        ChargingPoolStatusTypes.Available,

                                                 Configurator:         chargingPool => {
                                                                       }

                                             );

                ClassicAssert.IsNotNull(addChargingPoolResult2);

                var chargingPool2  = addChargingPoolResult2.ChargingPool;
                ClassicAssert.IsNotNull(chargingPool2);

                #endregion


                // OCPI does not have stations!

                #region Add DE*GEF*STATION*1*A

                var addChargingStationResult1 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*A"),
                                                    Name:                 I18NString.Create("Test station #1A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult1);

                var chargingStation1  = addChargingStationResult1.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation1);

                #endregion

                #region Add DE*GEF*STATION*1*B

                var addChargingStationResult2 = await chargingPool1!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*1*B"),
                                                    Name:                 I18NString.Create("Test station #1B"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #1B"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult2);

                var chargingStation2  = addChargingStationResult2.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation2);

                #endregion

                #region Add DE*GEF*STATION*2*A

                var addChargingStationResult3 = await chargingPool2!.AddChargingStation(

                                                    Id:                   WWCP.ChargingStation_Id.Parse("DE*GEF*STATION*2*A"),
                                                    Name:                 I18NString.Create("Test station #2A"),
                                                    Description:          I18NString.Create("GraphDefined charging station for tests #2A"),

                                                    GeoLocation:          GeoCoordinate.Parse(50.82, 11.52),

                                                    InitialAdminStatus:   ChargingStationAdminStatusTypes.Operational,
                                                    InitialStatus:        ChargingStationStatusTypes.Available,

                                                    Configurator:         chargingStation => {
                                                                          }

                                                );

                ClassicAssert.IsNotNull(addChargingStationResult3);

                var chargingStation3  = addChargingStationResult3.ChargingStation;
                ClassicAssert.IsNotNull(chargingStation3);

                #endregion


                #region Add EVSE DE*GEF*EVSE*1*A*1

                var addEVSE1Result1 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #1A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.CHAdeMO
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result1);

                var evse1     = addEVSE1Result1.EVSE;
                ClassicAssert.IsNotNull(evse1);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*A*2

                var addEVSE1Result2 = await chargingStation1!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*A*2"),
                                          Name:                 I18NString.Create("Test EVSE #1A2"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1A2"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Outlet
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result2);

                var evse2     = addEVSE1Result2.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*1*B*1

                var addEVSE1Result3 = await chargingStation2!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*1*B*1"),
                                          Name:                 I18NString.Create("Test EVSE #1B1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #1B1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.TypeFSchuko
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result3);

                var evse3     = addEVSE1Result3.EVSE;
                ClassicAssert.IsNotNull(evse2);

                #endregion

                #region Add EVSE DE*GEF*EVSE*2*A*1

                var addEVSE1Result4 = await chargingStation3!.AddEVSE(

                                          Id:                   WWCP.EVSE_Id.Parse("DE*GEF*EVSE*2*A*1"),
                                          Name:                 I18NString.Create("Test EVSE #2A1"),
                                          Description:          I18NString.Create("GraphDefined EVSE for tests #2A1"),

                                          InitialAdminStatus:   EVSEAdminStatusTypes.Operational,
                                          InitialStatus:        EVSEStatusType.Available,

                                          ChargingConnectors:   [
                                                                    new ChargingConnector(
                                                                        ChargingPlugTypes.Type2Connector_CableAttached,
                                                                        CableAttached: true
                                                                    )
                                                                ],

                                          Configurator:         evse => {
                                                                }

                                      );

                ClassicAssert.IsNotNull(addEVSE1Result4);

                var evse4     = addEVSE1Result4.EVSE;
                ClassicAssert.IsNotNull(evse4);

                #endregion


                emsp1EMSPAPI.OnRFIDAuthToken += async (From_CountryCode,
                                                       From_PartyId,
                                                       To_CountryCode,
                                                       To_PartyId,
                                                       tokenId,
                                                       locationReference) => {

                    return tokenId.ToString() == "11223344"

                               ? new AuthorizationInfo(
                                     Allowed:                  AllowedType.ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C12345678-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Hello world!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(23.5)
                                 )

                               : new AuthorizationInfo(
                                     Allowed:                  AllowedType.NOT_ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C12345678-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Go away!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(42)
                                 );

                };

                emsp2EMSPAPI.OnRFIDAuthToken += async (From_CountryCode,
                                                       From_PartyId,
                                                       To_CountryCode,
                                                       To_PartyId,
                                                       tokenId,
                                                       locationReference) => {

                    return tokenId.ToString() == "55667788"

                               ? new AuthorizationInfo(
                                     Allowed:                  AllowedType.ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C56781234-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Hello world!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(23.5)
                                 )

                               : new AuthorizationInfo(
                                     Allowed:                  AllowedType.NOT_ALLOWED,
                                     Token:                    new Token(
                                                                   CountryCode:      To_CountryCode,
                                                                   PartyId:          To_PartyId,
                                                                   Id:               tokenId,
                                                                   Type:             TokenType.RFID,
                                                                   ContractId:       Contract_Id.Parse("DE-GDF-C56781234-X"),
                                                                   Issuer:           "GraphDefined CA",
                                                                   IsValid:          true,
                                                                   WhitelistType:    WhitelistTypes.NEVER,
                                                                   VisualNumber:     null,
                                                                   GroupId:          null,
                                                                   UILanguage:       null,
                                                                   DefaultProfile:   null,
                                                                   EnergyContract:   null,
                                                                   LastUpdated:      null
                                                               ),
                                     Location:                 locationReference,
                                     AuthorizationReference:   null,
                                     Info:                     DisplayText.Create(Languages.en, "Go away!"),
                                     RemoteParty:              null,
                                     Runtime:                  TimeSpan.FromMilliseconds(42)
                                 );

                };


                var authStartResult1 = await csoRoamingNetwork.AuthorizeStart(
                                                 LocalAuthentication: LocalAuthentication.FromAuthToken(AuthenticationToken.NewRandom7Bytes),
                                                 ChargingLocation:    ChargingLocation.   FromEVSEId   (evse1!.Id),
                                                 ChargingProduct:     ChargingProduct.    FromId       (ChargingProduct_Id.Parse("AC1"))
                                             );

                var authStartResult2 = await csoRoamingNetwork.AuthorizeStart(
                                                 LocalAuthentication: LocalAuthentication.FromAuthToken(AuthenticationToken.Parse("11223344")),
                                                 ChargingLocation:    ChargingLocation.   FromEVSEId   (evse1!.Id),
                                                 ChargingProduct:     ChargingProduct.    FromId       (ChargingProduct_Id.Parse("AC1"))
                                             );

                var authStartResult3 = await csoRoamingNetwork.AuthorizeStart(
                                                 LocalAuthentication: LocalAuthentication.FromAuthToken(AuthenticationToken.Parse("55667788")),
                                                 ChargingLocation:    ChargingLocation.   FromEVSEId   (evse1!.Id),
                                                 ChargingProduct:     ChargingProduct.    FromId       (ChargingProduct_Id.Parse("AC1"))
                                             );

                ClassicAssert.AreEqual(AuthStartResultTypes.NotAuthorized, authStartResult1.Result);
                ClassicAssert.AreEqual(AuthStartResultTypes.Authorized,    authStartResult2.Result);
                ClassicAssert.AreEqual(AuthStartResultTypes.Authorized,    authStartResult3.Result);

            }

        }

        #endregion


    }

}
