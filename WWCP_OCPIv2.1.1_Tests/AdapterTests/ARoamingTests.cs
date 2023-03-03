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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPIv2_1_1.HTTP;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.Networking;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.UnitTests
{

    /// <summary>
    /// Roaming test defaults for a charging station operator connected
    /// to two e-mobility providers via OCPI v2.1.1.
    /// </summary>
    public abstract class ARoamingTests
    {

        #region Data

        protected  RoamingNetwork?            csoRoamingNetwork;
        protected  HTTPAPI?                   csoHTTPAPI;
        protected  CommonAPI?                 csoCommonAPI;
        protected  CPOAPI?                    csoCPOAPI;
        protected  OCPICSOAdapter?            csoAdapter;
        protected  IChargingStationOperator?  graphDefinedCSO;

        protected  RoamingNetwork?            emp1RoamingNetwork;
        protected  HTTPAPI?                   emp1HTTPAPI;
        protected  CommonAPI?                 emp1CommonAPI;
        protected  EMSPAPI?                   emp1EMSPAPI;
        protected  OCPIEMPAdapter?            emp1Adapter;
        protected  IEMobilityProvider?        graphDefinedEMP;

        protected  RoamingNetwork?            emp2RoamingNetwork;
        protected  HTTPAPI?                   emp2HTTPAPI;
        protected  CommonAPI?                 emp2CommonAPI;
        protected  EMSPAPI?                   emp2EMSPAPI;
        protected  OCPIEMPAdapter?            emp2Adapter;
        protected  IEMobilityProvider?        exampleEMP;

        #endregion

        #region Constructor(s)

        public ARoamingTests()
        {

        }

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public void SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public void SetupEachTest()
        {

            Timestamp.Reset();

            #region Create cso/emp1/emp2 roaming network

            csoRoamingNetwork = new RoamingNetwork(
                                       Id:                                  RoamingNetwork_Id.Parse("test"),
                                       Name:                                I18NString.Create(Languages.en, "CSO EV Roaming Test Network"),
                                       Description:                         I18NString.Create(Languages.en, "The EV roaming test network at the charging station operator"),
                                       InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:                       RoamingNetworkStatusTypes.Available
                                   );

            emp1RoamingNetwork   = new RoamingNetwork(
                                       Id:                                  RoamingNetwork_Id.Parse("test"),
                                       Name:                                I18NString.Create(Languages.en, "EV Roaming Test Network EMP1"),
                                       Description:                         I18NString.Create(Languages.en, "The EV roaming test network at the 1st e-mobility provider"),
                                       InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:                       RoamingNetworkStatusTypes.Available
                                   );

            emp2RoamingNetwork   = new RoamingNetwork(
                                       Id:                                  RoamingNetwork_Id.Parse("test"),
                                       Name:                                I18NString.Create(Languages.en, "EV Roaming Test Network EMP2"),
                                       Description:                         I18NString.Create(Languages.en, "The EV roaming test network at the 2nd e-mobility provider"),
                                       InitialAdminStatus:                  RoamingNetworkAdminStatusTypes.Operational,
                                       InitialStatus:                       RoamingNetworkStatusTypes.Available
                                   );

            Assert.IsNotNull(csoRoamingNetwork);
            Assert.IsNotNull(emp1RoamingNetwork);
            Assert.IsNotNull(emp2RoamingNetwork);

            #endregion

            #region Create cso/emp1/emp2 HTTP API

            csoHTTPAPI           = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3301),
                                       Autostart:                           true
                                   );

            emp1HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3401),
                                       Autostart:                           true
                                   );

            emp2HTTPAPI          = new HTTPAPI(
                                       HTTPServerPort:                      IPPort.Parse(3402),
                                       Autostart:                           true
                                   );

            Assert.IsNotNull(csoHTTPAPI);
            Assert.IsNotNull(emp1HTTPAPI);
            Assert.IsNotNull(emp2HTTPAPI);

            #endregion

            #region Create cso/emp1/emp2 OCPI Common API

            csoCommonAPI         = new CommonAPI(

                                       OurVersionsURL:                      URL.Parse("http://127.0.0.1:3301/ocpi/v2.1/versions"),
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefiend CSO",
                                                                                URL.Parse("http://www.graphdefiend.com")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GEF"),

                                       HTTPServer:                          csoHTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       DisableMaintenanceTasks:             null,
                                       MaintenanceInitialDelay:             null,
                                       MaintenanceEvery:                    null,

                                       DisableWardenTasks:                  null,
                                       WardenInitialDelay:                  null,
                                       WardenCheckEvery:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null,
                                       Autostart:                           false

                                   );

            emp1CommonAPI        = new CommonAPI(

                                       OurVersionsURL:                      URL.Parse("http://127.0.0.1:3401/ocpi/v2.1/versions"),
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "GraphDefiend EMP",
                                                                                URL.Parse("http://www.graphdefiend.com")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("GDF"),

                                       HTTPServer:                          emp1HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       DisableMaintenanceTasks:             null,
                                       MaintenanceInitialDelay:             null,
                                       MaintenanceEvery:                    null,

                                       DisableWardenTasks:                  null,
                                       WardenInitialDelay:                  null,
                                       WardenCheckEvery:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null,
                                       Autostart:                           false

                                   );

            emp2CommonAPI        = new CommonAPI(

                                       OurVersionsURL:                      URL.Parse("http://127.0.0.1:3402/ocpi/v2.1/versions"),
                                       OurBusinessDetails:                  new BusinessDetails(
                                                                                "Example EMP",
                                                                                URL.Parse("http://www.example.org")
                                                                            ),
                                       OurCountryCode:                      CountryCode.Parse("DE"),
                                       OurPartyId:                          Party_Id.   Parse("EMP"),

                                       HTTPServer:                          emp2HTTPAPI.HTTPServer,

                                       AdditionalURLPathPrefix:             null,
                                       KeepRemovedEVSEs:                    null,
                                       LocationsAsOpenData:                 true,
                                       AllowDowngrades:                     null,
                                       Disable_RootServices:                false,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       DisableMaintenanceTasks:             null,
                                       MaintenanceInitialDelay:             null,
                                       MaintenanceEvery:                    null,

                                       DisableWardenTasks:                  null,
                                       WardenInitialDelay:                  null,
                                       WardenCheckEvery:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null,
                                       Autostart:                           false

                                   );

            Assert.IsNotNull(csoCommonAPI);
            Assert.IsNotNull(emp1CommonAPI);
            Assert.IsNotNull(emp2CommonAPI);

            #endregion

            #region Create cso CPO API / emp1 EMP API / emp2 EMP API

            csoCPOAPI            = new CPOAPI(

                                       CommonAPI:                           csoCommonAPI,
                                       DefaultCountryCode:                  csoCommonAPI.OurCountryCode,
                                       DefaultPartyId:                      csoCommonAPI.OurPartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       DisableMaintenanceTasks:             null,
                                       MaintenanceInitialDelay:             null,
                                       MaintenanceEvery:                    null,

                                       DisableWardenTasks:                  null,
                                       WardenInitialDelay:                  null,
                                       WardenCheckEvery:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null,
                                       Autostart:                           false

                                   );

            emp1EMSPAPI          = new EMSPAPI(

                                       CommonAPI:                           emp1CommonAPI,
                                       DefaultCountryCode:                  emp1CommonAPI.OurCountryCode,
                                       DefaultPartyId:                      emp1CommonAPI.OurPartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       DisableMaintenanceTasks:             null,
                                       MaintenanceInitialDelay:             null,
                                       MaintenanceEvery:                    null,

                                       DisableWardenTasks:                  null,
                                       WardenInitialDelay:                  null,
                                       WardenCheckEvery:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null,
                                       Autostart:                           false

                                   );

            emp2EMSPAPI          = new EMSPAPI(

                                       CommonAPI:                           emp2CommonAPI,
                                       DefaultCountryCode:                  emp2CommonAPI.OurCountryCode,
                                       DefaultPartyId:                      emp2CommonAPI.OurPartyId,
                                       AllowDowngrades:                     null,

                                       HTTPHostname:                        null,
                                       ExternalDNSName:                     null,
                                       HTTPServiceName:                     null,
                                       BasePath:                            null,

                                       URLPathPrefix:                       HTTPPath.Parse("/ocpi/v2.1"),
                                       APIVersionHashes:                    null,

                                       DisableMaintenanceTasks:             null,
                                       MaintenanceInitialDelay:             null,
                                       MaintenanceEvery:                    null,

                                       DisableWardenTasks:                  null,
                                       WardenInitialDelay:                  null,
                                       WardenCheckEvery:                    null,

                                       IsDevelopment:                       null,
                                       DevelopmentServers:                  null,
                                       DisableLogging:                      null,
                                       LoggingPath:                         null,
                                       LogfileName:                         null,
                                       LogfileCreator:                      null,
                                       Autostart:                           false

                                   );

            Assert.IsNotNull(csoCPOAPI);
            Assert.IsNotNull(emp1EMSPAPI);
            Assert.IsNotNull(emp2EMSPAPI);

            #endregion

            #region Create cso/emp1/emp2 adapter

            csoAdapter           = csoRoamingNetwork.CreateOCPIv2_1_CSOAdapter(

                                       Id:                                  EMPRoamingProvider_Id.Parse("OCPIv2.1_CSO_" + this.csoRoamingNetwork.Id),
                                       Name:                                I18NString.Create(Languages.de, "OCPI v2.1 CSO"),
                                       Description:                         I18NString.Create(Languages.de, "OCPI v2.1 CSO Roaming"),

                                       CommonAPI:                           csoCommonAPI,
                                       DefaultCountryCode:                  csoCommonAPI.OurCountryCode,
                                       DefaultPartyId:                      csoCommonAPI.OurPartyId,

                                       CustomEVSEIdConverter:               null,
                                       CustomEVSEConverter:                 null,
                                       CustomEVSEStatusUpdateConverter:     null,
                                       CustomChargeDetailRecordConverter:   null,

                                       IncludeEVSEIds:                      null,
                                       IncludeEVSEs:                        null,
                                       IncludeChargingPoolIds:              null,
                                       IncludeChargingPools:                null,
                                       ChargeDetailRecordFilter:            null,

                                       ServiceCheckEvery:                   null,
                                       StatusCheckEvery:                    null,
                                       CDRCheckEvery:                       null,

                                       DisablePushData:                     true,
                                       DisablePushStatus:                   true,
                                       DisableAuthentication:               true,
                                       DisableSendChargeDetailRecords:      true

                                   );

            emp1Adapter          = emp1RoamingNetwork.CreateOCPIv2_1_EMPAdapter(

                                       Id:                                  CSORoamingProvider_Id.Parse("OCPIv2.1_EMP1_" + this.emp1RoamingNetwork.Id),
                                       Name:                                I18NString.Create(Languages.de, "OCPI v2.1 EMP1"),
                                       Description:                         I18NString.Create(Languages.de, "OCPI v2.1 EMP1 Roaming"),

                                       CommonAPI:                           emp1CommonAPI,
                                       DefaultCountryCode:                  emp1CommonAPI.OurCountryCode,
                                       DefaultPartyId:                      emp1CommonAPI.OurPartyId,

                                       CustomEVSEIdConverter:               null,
                                       CustomEVSEConverter:                 null,
                                       CustomEVSEStatusUpdateConverter:     null,
                                       CustomChargeDetailRecordConverter:   null,

                                       IncludeEVSEIds:                      null,
                                       IncludeEVSEs:                        null,
                                       IncludeChargingPoolIds:              null,
                                       IncludeChargingPools:                null,
                                       ChargeDetailRecordFilter:            null,

                                       ServiceCheckEvery:                   null,
                                       StatusCheckEvery:                    null,
                                       CDRCheckEvery:                       null,

                                       DisablePushData:                     true,
                                       DisablePushStatus:                   true,
                                       DisableAuthentication:               true,
                                       DisableSendChargeDetailRecords:      true

                                   );

            emp2Adapter          = csoRoamingNetwork.CreateOCPIv2_1_EMPAdapter(

                                       Id:                                  CSORoamingProvider_Id.Parse("OCPIv2.1_EMP2_" + this.emp1RoamingNetwork.Id),
                                       Name:                                I18NString.Create(Languages.de, "OCPI v2.1 EMP2"),
                                       Description:                         I18NString.Create(Languages.de, "OCPI v2.1 EMP2 Roaming"),

                                       CommonAPI:                           emp2CommonAPI,
                                       DefaultCountryCode:                  emp2CommonAPI.OurCountryCode,
                                       DefaultPartyId:                      emp2CommonAPI.OurPartyId,

                                       CustomEVSEIdConverter:               null,
                                       CustomEVSEConverter:                 null,
                                       CustomEVSEStatusUpdateConverter:     null,
                                       CustomChargeDetailRecordConverter:   null,

                                       IncludeEVSEIds:                      null,
                                       IncludeEVSEs:                        null,
                                       IncludeChargingPoolIds:              null,
                                       IncludeChargingPools:                null,
                                       ChargeDetailRecordFilter:            null,

                                       ServiceCheckEvery:                   null,
                                       StatusCheckEvery:                    null,
                                       CDRCheckEvery:                       null,

                                       DisablePushData:                     true,
                                       DisablePushStatus:                   true,
                                       DisableAuthentication:               true,
                                       DisableSendChargeDetailRecords:      true

                                   );

            Assert.IsNotNull(csoAdapter);
            Assert.IsNotNull(emp1Adapter);
            Assert.IsNotNull(emp2Adapter);

            #endregion


            graphDefinedCSO     = csoRoamingNetwork.CreateChargingStationOperator(
                                      Id:                                  ChargingStationOperator_Id.Parse("DE*GEF"),
                                      Name:                                I18NString.Create(Languages.en, "GraphDefined CSO"),
                                      Description:                         I18NString.Create(Languages.en, "GraphDefined CSO Services"),
                                      InitialAdminStatus:                  ChargingStationOperatorAdminStatusTypes.Operational,
                                      InitialStatus:                       ChargingStationOperatorStatusTypes.Available
                                  ).Result.ChargingStationOperator;

            graphDefinedEMP     = emp1RoamingNetwork.CreateEMobilityProvider(
                                      Id:                                  EMobilityProvider_Id.Parse("DE*GDF"),
                                      Name:                                I18NString.Create(Languages.en, "GraphDefined EMP"),
                                      Description:                         I18NString.Create(Languages.en, "GraphDefined EMP Services"),
                                      InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
                                      InitialStatus:                       EMobilityProviderStatusTypes.Available
                                  ).Result.EMobilityProvider;

            exampleEMP          = emp1RoamingNetwork.CreateEMobilityProvider(
                                      Id:                                  EMobilityProvider_Id.Parse("DE*EMP"),
                                      Name:                                I18NString.Create(Languages.en, "example EMP"),
                                      Description:                         I18NString.Create(Languages.en, "example EMP Services"),
                                      InitialAdminStatus:                  EMobilityProviderAdminStatusTypes.Operational,
                                      InitialStatus:                       EMobilityProviderStatusTypes.Available
                                  ).Result.EMobilityProvider;

            Assert.IsNotNull(graphDefinedCSO);
            Assert.IsNotNull(graphDefinedEMP);
            Assert.IsNotNull(exampleEMP);

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {

            csoHTTPAPI?.Shutdown();
            emp1HTTPAPI?.Shutdown();
            emp2HTTPAPI?.Shutdown();

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public void ShutdownOnce()
        {

        }

        #endregion


    }

}