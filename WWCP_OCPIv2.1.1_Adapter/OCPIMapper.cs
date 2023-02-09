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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE identifications to OCPI EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_Id     WWCPEVSEId_2_EVSEId_Delegate               (WWCP.EVSE_Id           EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSEs to OCPI EVSEs.
    /// </summary>
    /// <param name="WWCPEVSE">A WWCP EVSE.</param>
    /// <param name="OCPIEVSE">An OCPI EVSE.</param>
    public delegate EVSE        WWCPEVSE_2_EVSE_Delegate                   (WWCP.IEVSE             WWCPEVSE,
                                                                            EVSE                   OCPIEVSE);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP EVSE status updates to OCPI EVSE status types.
    /// </summary>
    /// <param name="WWCPEVSEStatusUpdate">A WWCP EVSE status update.</param>
    /// <param name="OCPIStatusType">An OICP status type.</param>
    public delegate StatusType  WWCPEVSEStatusUpdate_2_StatusType_Delegate (WWCP.EVSEStatusUpdate  WWCPEVSEStatusUpdate,
                                                                            StatusType             OCPIStatusType);

    /// <summary>
    /// A delegate which allows you to modify the convertion from WWCP charge detail records to OICP charge detail records.
    /// </summary>
    /// <param name="WWCPChargeDetailRecord">A WWCP charge detail record.</param>
    /// <param name="OCIPCDR">An OICP charge detail record.</param>
    public delegate CDR         WWCPChargeDetailRecord_2_CDR_Delegate      (WWCP.ChargeDetailRecord  WWCPChargeDetailRecord,
                                                                            CDR                      OCIPCDR);


    /// <summary>
    /// Helper methods to map OCPI data structures to
    /// WWCP data structures and vice versa.
    /// </summary>
    public static class OCPIMapper
    {

        #region AsWWCPEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert the given OCPI EVSE/connector status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI EVSE/connector status.</param>
        public static WWCP.EVSEStatusTypes AsWWCPEVSEStatus(this StatusType EVSEStatus)
        {

            if (EVSEStatus == StatusType.AVAILABLE)
                return WWCP.EVSEStatusTypes.Available;

            if (EVSEStatus == StatusType.BLOCKED)
                return WWCP.EVSEStatusTypes.OutOfService;

            if (EVSEStatus == StatusType.CHARGING)
                return WWCP.EVSEStatusTypes.Charging;

            if (EVSEStatus == StatusType.INOPERATIVE)
                return WWCP.EVSEStatusTypes.OutOfService;

            if (EVSEStatus == StatusType.OUTOFORDER)
                return WWCP.EVSEStatusTypes.Error;

            //if (EVSEStatus == StatusType.PLANNED)
            //    return WWCP.EVSEStatusTypes.Planned;

            //if (EVSEStatus == StatusType.REMOVED)
            //    return WWCP.EVSEStatusTypes.Removed;

            if (EVSEStatus == StatusType.RESERVED)
                return WWCP.EVSEStatusTypes.Reserved;

            return WWCP.EVSEStatusTypes.Unspecified;

        }

        #endregion

        #region AsOCPIEVSEStatus(this EVSEStatus)

        /// <summary>
        /// Convert an OCPI v2.0 EVSE status into a corresponding WWCP EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An OCPI v2.0 EVSE status.</param>
        /// <returns>The corresponding WWCP EVSE status.</returns>
        public static StatusType AsOCPIEVSEStatus(this WWCP.EVSEStatusTypes EVSEStatus)
        {

            //case WWCP.EVSEStatusTypes.Planned:
            //    return OCPIv2_2.EVSEStatusType.Planned;

            //case WWCP.EVSEStatusTypes.InDeployment:
            //    return OCPIv2_2.EVSEStatusType.Planned;

            if (EVSEStatus == WWCP.EVSEStatusTypes.Available)
                return StatusType.AVAILABLE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Charging)
                return StatusType.CHARGING;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Error)
                return StatusType.OUTOFORDER;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.OutOfService)
                return StatusType.INOPERATIVE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Offline)
                return StatusType.UNKNOWN;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Reserved)
                return StatusType.RESERVED;

            //case WWCP.EVSEStatusTypes.Private:
            //    return OCPIv2_2.EVSEStatusType.Unknown;

            //else if (EVSEStatus == WWCP.EVSEStatusTypes.UnknownEVSE)
            //    return StatusType.REMOVED;

            else
                return StatusType.UNKNOWN;

        }

        #endregion


        #region ToOCPI(this ChargingPools)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools)
        {

            var locations = new HashSet<Location>();

            foreach (var pool in ChargingPools)
            {
                try
                {

                    //locations.Add(new Location(
                    //                  CountryCode.Parse("DE"),
                    //                  Party_Id.   Parse("GEF"),
                    //                  Location_Id.Parse("..."),
                    //                  true,
                    //                  pool.Address.Street + pool.Address.HouseNumber,
                    //                  pool.Address.City.FirstText(),
                    //                  pool.Address.Country,
                    //                  pool.GeoLocation ?? default,
                    //                  "timezone",
                    //                  null,
                    //                  "name",
                    //                  pool.Address.PostalCode,
                    //                  pool.Address.Country.CountryName.FirstText(),
                    //                  null,
                    //                  null,
                    //                  Array.Empty<EVSE>(),
                    //                  new DisplayText[] {
                    //                      new DisplayText(Languages.en, "directions")
                    //                  },
                    //                  new BusinessDetails(
                    //                      pool.Operator.Name.FirstText(),
                    //                      URL.Parse(pool.Operator.Homepage),
                    //                      null
                    //                  ),
                    //                  null,
                    //                  null,
                    //                  Array.Empty<Facilities>(),
                    //                  null,
                    //                  null,
                    //                  null,
                    //                  null,
                    //                  Timestamp.Now)
                    //              );

                }
                catch (Exception)
                { }
            }

            return locations;

        }

        #endregion

    }

}