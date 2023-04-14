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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
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

        #region ToOCPI(this EVSEStatus)

        /// <summary>
        /// Convert a WWCP EVSE status into OCPI EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">A WWCP EVSE status.</param>
        public static StatusType ToOCPI(this WWCP.EVSEStatusTypes EVSEStatus)
        {

            if      (EVSEStatus == WWCP.EVSEStatusTypes.Available)
                return StatusType.AVAILABLE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Blocked)
                return StatusType.BLOCKED;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Charging)
                return StatusType.CHARGING;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.OutOfService)
                return StatusType.INOPERATIVE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Offline)
                return StatusType.INOPERATIVE;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Error)
                return StatusType.OUTOFORDER;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.InDeployment)
                return StatusType.PLANNED;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Removed)
                return StatusType.REMOVED;

            else if (EVSEStatus == WWCP.EVSEStatusTypes.Reserved)
                return StatusType.RESERVED;

            else
                return StatusType.UNKNOWN;

        }

        #endregion


        #region ToOCPI(this I18NString)

        public static IEnumerable<DisplayText> ToOCPI(this I18NString I18NString)

            => I18NString.Select(text => new DisplayText(text.Language,
                                                         text.Text));

        #endregion

        #region ToWWCP(this DisplayTexts)

        public static I18NString ToOCPI(this IEnumerable<DisplayText> DisplayTexts)
        {

            var i18nString = I18NString.Empty;

            foreach (var displayText in DisplayTexts)
                i18nString.Set(displayText.Language,
                               displayText.Text);

            return i18nString;

        }

        #endregion


        #region ToOCPI(this OpeningTimes)

        public static Hours? ToOCPI(this OpeningTimes OpeningTimes)

            => OpeningTimes.IsOpen24Hours

                   ? Hours.TwentyFourSevenOpen(
                         OpeningTimes.ExceptionalOpenings.ToOCPI(),
                         OpeningTimes.ExceptionalClosings.ToOCPI()
                     )

                   : new (RegularHours:         null,
                          ExceptionalOpenings:  OpeningTimes.ExceptionalOpenings.ToOCPI(),
                          ExceptionalClosings:  OpeningTimes.ExceptionalClosings.ToOCPI());


        public static ExceptionalPeriod? ToOCPI(this org.GraphDefined.Vanaheimr.Illias.ExceptionalPeriod ExceptionalPeriod)

            => new ExceptionalPeriod(
                   ExceptionalPeriod.Begin,
                   ExceptionalPeriod.End
               );

        public static IEnumerable<ExceptionalPeriod> ToOCPI(this IEnumerable<org.GraphDefined.Vanaheimr.Illias.ExceptionalPeriod> ExceptionalPeriods)
        {

            var exceptionalPeriods = new List<ExceptionalPeriod>();

            foreach (var exceptionalPeriod in ExceptionalPeriods)
            {

                var converted = exceptionalPeriod.ToOCPI();

                if (converted.HasValue)
                    exceptionalPeriods.Add(converted.Value);

            }

            return exceptionalPeriods;

        }

        #endregion


        #region ToOCPI_EVSEUId(this EVSEId)

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id EVSEId)

            => EVSE_UId.TryParse(EVSEId.ToString());

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id? EVSEId)

            => EVSEId.HasValue
                   ? EVSE_UId.TryParse(EVSEId.Value.ToString())
                   : null;

        #endregion


        #region ToOCPI_EVSEId(this EVSEId)

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id EVSEId)

            => EVSE_Id.TryParse(EVSEId.ToString());

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id? EVSEId)

            => EVSEId.HasValue
                   ? EVSE_Id.TryParse(EVSEId.Value.ToString())
                   : null;

        #endregion


        #region ToOCPI_Capabilities(this ChargingStation)

        public static IEnumerable<Capability> ToOCPI_Capabilities(this WWCP.IChargingStation ChargingStation)
        {

            var capabilities = new HashSet<Capability>();

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.RFID)       ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    RFID))
                capabilities.Add(Capability.RFID_READER);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.CreditCard) ||
                ChargingStation.PaymentOptions.Contains(WWCP.PaymentOptions.CreditCard) ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    CreditCard))
                capabilities.Add(Capability.CREDIT_CARD_PAYABLE);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.DebitCard)  ||
                ChargingStation.PaymentOptions.Contains(WWCP.PaymentOptions.DebitCard) ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    DebitCard))
                capabilities.Add(Capability.DEBIT_CARD_PAYABLE);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.NFC)        ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    NFC))
                capabilities.Add(Capability.CONTACTLESS_CARD_SUPPORT);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.PINPAD)     ||
                ChargingStation.UIFeatures.    Contains(WWCP.UIFeatures.    Pinpad))
                capabilities.Add(Capability.PED_TERMINAL);

            if (ChargingStation.AuthenticationModes.Any(authenticationMode => authenticationMode is WWCP.AuthenticationModes.REMOTE))
                capabilities.Add(Capability.REMOTE_START_STOP_CAPABLE);

            // CHIP_CARD_SUPPORT (payment terminal that supports chip cards)



            if (ChargingStation.Features.Contains(WWCP.Features.Reservable))
                capabilities.Add(Capability.RESERVABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.ChargingProfilesSupported))
                capabilities.Add(Capability.CHARGING_PROFILE_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.ChargingPreferencesSupported))
                capabilities.Add(Capability.CHARGING_PREFERENCES_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.TokenGroupsSupported))
                capabilities.Add(Capability.TOKEN_GROUP_CAPABLE);

            if (ChargingStation.Features.Contains(WWCP.Features.CSOUnlockSupported))
                capabilities.Add(Capability.UNLOCK_CAPABLE);


            return capabilities;

        }

        #endregion

        #region ToOCPI             (this Facilities)

        public static IEnumerable<Facilities> ToOCPI(this IEnumerable<WWCP.Facilities> Facilities)
        {

            var facilities = new HashSet<Facilities>();

            foreach (var facility in Facilities)
            {

                switch (facility.ToString())
                {

                    case "HOTEL":           facilities.Add(OCPIv2_2.Facilities.HOTEL);            break;
                    case "RESTAURANT":      facilities.Add(OCPIv2_2.Facilities.RESTAURANT);       break;
                    case "CAFE":            facilities.Add(OCPIv2_2.Facilities.CAFE);             break;
                    case "MALL":            facilities.Add(OCPIv2_2.Facilities.MALL);             break;
                    case "SUPERMARKET":     facilities.Add(OCPIv2_2.Facilities.SUPERMARKET);      break;
                    case "SPORT":           facilities.Add(OCPIv2_2.Facilities.SPORT);            break;
                    case "RECREATION_AREA": facilities.Add(OCPIv2_2.Facilities.RECREATION_AREA);  break;
                    case "NATURE":          facilities.Add(OCPIv2_2.Facilities.NATURE);           break;
                    case "MUSEUM":          facilities.Add(OCPIv2_2.Facilities.MUSEUM);           break;
                    case "BIKE_SHARING":    facilities.Add(OCPIv2_2.Facilities.BIKE_SHARING);     break;
                    case "BUS_STOP":        facilities.Add(OCPIv2_2.Facilities.BUS_STOP);         break;
                    case "TAXI_STAND":      facilities.Add(OCPIv2_2.Facilities.TAXI_STAND);       break;
                    case "TRAM_STOP":       facilities.Add(OCPIv2_2.Facilities.TRAM_STOP);        break;
                    case "METRO_STATION":   facilities.Add(OCPIv2_2.Facilities.METRO_STATION);    break;
                    case "TRAIN_STATION":   facilities.Add(OCPIv2_2.Facilities.TRAIN_STATION);    break;
                    case "AIRPORT":         facilities.Add(OCPIv2_2.Facilities.AIRPORT);          break;
                    case "PARKING_LOT":     facilities.Add(OCPIv2_2.Facilities.PARKING_LOT);      break;
                    case "CARPOOL_PARKING": facilities.Add(OCPIv2_2.Facilities.CARPOOL_PARKING);  break;
                    case "FUEL_STATION":    facilities.Add(OCPIv2_2.Facilities.FUEL_STATION);     break;
                    case "WIFI":            facilities.Add(OCPIv2_2.Facilities.WIFI);             break;

                }

            }

            return facilities;

        }

        #endregion


        #region ToOCPI(this ChargingPool,  ref Warnings)

        public static Location? ToOCPI(this WWCP.IChargingPool  ChargingPool,
                                       ref List<Warning>        Warnings)
        {

            var location = ChargingPool.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return location;

        }

        #endregion

        #region ToOCPI(this ChargingPool,  out Warnings)

        public static Location? ToOCPI(this WWCP.IChargingPool   ChargingPool,
                                       out IEnumerable<Warning>  Warnings)
        {

            var warnings = new List<Warning>();

            if (ChargingPool.Operator is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid charging station operator!"));
                Warnings = warnings;
                return null;
            }

            if (ChargingPool.Address is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid address!"));
                Warnings = warnings;
                return null;
            }

            if (ChargingPool.GeoLocation is null)
            {
                warnings.Add(Warning.Create(Languages.en, "The given charging location must have a valid geo location!"));
                Warnings = warnings;
                return null;
            }

            try
            {

                Warnings = Array.Empty<Warning>();

                var evses = new List<EVSE>();

                foreach (var evse in ChargingPool.SelectMany(station => station.EVSEs))
                {

                    var ocpiEVSE = evse.ToOCPI(ref warnings);

                    if (ocpiEVSE is not null)
                        evses.Add(ocpiEVSE);

                }

                return new Location(

                           CountryCode:          CountryCode.Parse(ChargingPool.Id.OperatorId.CountryCode.Alpha2Code),
                           PartyId:              Party_Id.   Parse(ChargingPool.Id.OperatorId.Suffix),
                           Id:                   Location_Id.Parse(ChargingPool.Id.Suffix),
                           Publish:              true,
                     //      LocationType:         LocationType.ON_STREET, // ????
                           Address:              String.Concat(ChargingPool.Address.Street, " ", ChargingPool.Address.HouseNumber),
                           City:                 ChargingPool.Address.City.FirstText(),
                           Country:              ChargingPool.Address.Country,
                           Coordinates:          ChargingPool.GeoLocation.Value,
                           Timezone:             ChargingPool.Address.TimeZone?.ToString(),

                           PublishAllowedTo:     null,
                           Name:                 ChargingPool.Name.FirstText(),
                           PostalCode:           ChargingPool.Address.PostalCode,
                           State:                null,
                           RelatedLocations:     Array.Empty<AdditionalGeoLocation>(),
                           ParkingType:          null,
                           EVSEs:                evses,
                           Directions:           Array.Empty<DisplayText>(),
                           Operator:             new BusinessDetails(
                                                     ChargingPool.Operator.Name.FirstText(),
                                                     ChargingPool.Operator.Homepage,
                                                     ChargingPool.Operator.Logo.HasValue
                                                         ? new Image(
                                                               ChargingPool.Operator.Logo.Value,
                                                               ChargingPool.Operator.Logo.Value.Path.Substring(ChargingPool.Operator.Logo.Value.Path.LastIndexOf(".")).ToString().ToLower() switch {
                                                                   "gif"  => ImageFileType.gif,
                                                                   "png"  => ImageFileType.png,
                                                                   "svg"  => ImageFileType.svg,
                                                                   "jpeg" => ImageFileType.jpeg,
                                                                   "webp" => ImageFileType.webp,
                                                                   _      => ImageFileType.jpg
                                                               },
                                                               ImageCategory.OPERATOR
                                                           )
                                                         : null
                                                 ),
                           SubOperator:          null,
                           Owner:                null,
                           Facilities:           ChargingPool.Facilities.  ToOCPI(),
                           OpeningTimes:         ChargingPool.OpeningTimes.ToOCPI(),
                           ChargingWhenClosed:   ChargingPool.ChargingWhenClosed,
                           Images:               Array.Empty<Image>(),
                           EnergyMix:            null,

                           LastUpdated:          ChargingPool.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging pool '{ChargingPool.Id}' to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this ChargingPools, ref Warnings)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools,
                                                   ref List<Warning>                    Warnings)
        {

            var locations = ChargingPools.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return locations;

        }

        #endregion

        #region ToOCPI(this ChargingPools, out Warnings)

        public static IEnumerable<Location> ToOCPI(this IEnumerable<WWCP.ChargingPool>  ChargingPools,
                                                   out IEnumerable<Warning>             Warnings)
        {

            var warnings   = new List<Warning>();
            var locations  = new HashSet<Location>();

            foreach (var chargingPool in ChargingPools)
            {

                try
                {

                    var chargingPool2 = chargingPool.ToOCPI(out var warning);

                    if (chargingPool2 is not null)
                        locations.Add(chargingPool2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, $"Could not convert the given charging pool '{chargingPool.Id}' to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return locations;

        }

        #endregion


        #region ToOCPI(this EVSE,  ref Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE    EVSE,
                                   ref List<Warning>  Warnings)
        {

            var result = EVSE.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this EVSE,  out Warnings)

        public static EVSE? ToOCPI(this WWCP.IEVSE           EVSE,
                                   out IEnumerable<Warning>  Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                var evseUId = EVSE.Id.ToOCPI_EVSEUId();

                if (!evseUId.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE identificaton '{EVSE.Id}' could not be converted to an OCPI EVSE Unique identification!"));
                    Warnings = warnings;
                    return null;
                }


                var evseId = EVSE.Id.ToOCPI_EVSEId();

                if (!evseId.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE identificaton '{EVSE.Id}' could not be converted to an OCPI EVSE identification!"));
                    Warnings = warnings;
                    return null;
                }


                if (EVSE.ChargingStation is null)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE '{EVSE.Id}' must have a valid charging station!"));
                    Warnings = warnings;
                    return null;
                }

                if (EVSE.ChargingPool is null)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE '{EVSE.Id}' must have a valid charging pool!"));
                    Warnings = warnings;
                    return null;
                }

                var connectors   = EVSE.SocketOutlets.
                                        Select (socketOutlet => socketOutlet.ToOCPI(EVSE, ref warnings)).
                                        Where  (connector    => connector is not null).
                                        Cast<Connector>().
                                        ToArray();

                if (!connectors.Any())
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE socket outlets could not be converted to OCPI connectors!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new EVSE(

                           UId:                   evseUId.Value,
                           Status:                EVSE.Status.Value.ToOCPI(),
                           Connectors:            connectors,

                           EVSEId:                evseId,
                           StatusSchedule:        Array.Empty<StatusSchedule>(),
                           Capabilities:          EVSE.ChargingStation.ToOCPI_Capabilities(),
                           EnergyMeter:           EVSE.EnergyMeter is not null
                                                      ? EnergyMeter.Parse(EVSE.EnergyMeter.ToJSON())
                                                      : null,
                           FloorLevel:            EVSE.ChargingStation.Address?.FloorLevel ?? EVSE.ChargingPool.Address?.FloorLevel,
                           Coordinates:           EVSE.ChargingStation.GeoLocation         ?? EVSE.ChargingPool.GeoLocation,
                           PhysicalReference:     EVSE.PhysicalReference                   ?? EVSE.ChargingStation.PhysicalReference,
                           Directions:            EVSE.ChargingStation.ArrivalInstructions.ToOCPI(),
                           ParkingRestrictions:   Array.Empty<ParkingRestrictions>(),
                           Images:                Array.Empty<Image>(),

                           LastUpdated:           EVSE.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given EVSE '{EVSE.Id}' to OCPI: {ex.Message}"));
                Warnings = warnings;
            }

            return null;

        }

        #endregion

        #region ToOCPI(this EVSEs, ref Warnings)

        public static IEnumerable<EVSE> ToOCPI(this IEnumerable<WWCP.IEVSE>  EVSEs,
                                               ref List<Warning>             Warnings)
        {

            var evses = EVSEs.ToOCPI(out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return evses;

        }

        #endregion

        #region ToOCPI(this EVSEs, out Warnings)

        public static IEnumerable<EVSE> ToOCPI(this IEnumerable<WWCP.IEVSE>  EVSEs,
                                               out IEnumerable<Warning>      Warnings)
        {

            var warnings  = new List<Warning>();
            var evses     = new HashSet<EVSE>();

            foreach (var evse in EVSEs)
            {

                try
                {

                    var evse2 = evse.ToOCPI(out var warning);

                    if (evse2 is not null)
                        evses.Add(evse2);

                    if (warning is not null && warning.Any())
                        warnings.AddRange(warning);

                }
                catch (Exception ex)
                {
                    warnings.Add(Warning.Create(Languages.en, $"Could not convert the given EVSE '{evse.Id}' to OCPI: " + ex.Message));
                }

            }

            Warnings = warnings.ToArray();
            return evses;

        }

        #endregion


        #region ToOCPI(this SocketOutletId)

        public static Connector_Id? ToOCPI(this WWCP.SocketOutlet_Id SocketOutletId)

            => Connector_Id.TryParse(SocketOutletId.ToString());

        public static Connector_Id? ToOCPI(this WWCP.SocketOutlet_Id? SocketOutletId)

            => SocketOutletId.HasValue
                   ? Connector_Id.TryParse(SocketOutletId.Value.ToString())
                   : null;

        #endregion


        #region ToOCPI(this CurrentType)

        public static PowerTypes? ToOCPI(this WWCP.CurrentTypes CurrentType)

            => CurrentType switch {
                   WWCP.CurrentTypes.AC_OnePhase     => PowerTypes.AC_1_PHASE,
                   WWCP.CurrentTypes.AC_ThreePhases  => PowerTypes.AC_3_PHASE,
                   WWCP.CurrentTypes.DC              => PowerTypes.DC,
                   _                                 => null,
               };

        public static PowerTypes? ToOCPI(this WWCP.CurrentTypes? CurrentType)

            => CurrentType.HasValue
                   ? CurrentType.Value.ToOCPI()
                   : null;

        #endregion


        #region ToOCPI(this PlugType)

        public static ConnectorType? ToOCPI(this WWCP.PlugTypes PlugType)

            => PlugType switch {
                   //WWCP.PlugTypes.SmallPaddleInductive          => 
                   //WWCP.PlugTypes.LargePaddleInductive          => 
                   //WWCP.PlugTypes.AVCONConnector                => 
                   //WWCP.PlugTypes.TeslaConnector                => 
                   WWCP.PlugTypes.TESLA_Roadster                => ConnectorType.TESLA_R,
                   WWCP.PlugTypes.TESLA_ModelS                  => ConnectorType.TESLA_S,
                   //WWCP.PlugTypes.NEMA5_20                      => 
                   WWCP.PlugTypes.TypeEFrenchStandard           => ConnectorType.DOMESTIC_E,
                   WWCP.PlugTypes.TypeFSchuko                   => ConnectorType.DOMESTIC_F,
                   WWCP.PlugTypes.TypeGBritishStandard          => ConnectorType.DOMESTIC_G,
                   WWCP.PlugTypes.TypeJSwissStandard            => ConnectorType.DOMESTIC_J,
                   WWCP.PlugTypes.Type1Connector_CableAttached  => ConnectorType.IEC_62196_T1,
                   WWCP.PlugTypes.Type2Outlet                   => ConnectorType.IEC_62196_T2,
                   WWCP.PlugTypes.Type2Connector_CableAttached  => ConnectorType.IEC_62196_T2,
                   WWCP.PlugTypes.Type3Outlet                   => ConnectorType.IEC_62196_T3A,
                   WWCP.PlugTypes.IEC60309SinglePhase           => ConnectorType.IEC_60309_2_single_16,
                   WWCP.PlugTypes.IEC60309ThreePhase            => ConnectorType.IEC_60309_2_three_16,
                   WWCP.PlugTypes.CCSCombo1Plug_CableAttached   => ConnectorType.IEC_62196_T1_COMBO,
                   WWCP.PlugTypes.CCSCombo2Plug_CableAttached   => ConnectorType.IEC_62196_T2_COMBO,
                   WWCP.PlugTypes.CHAdeMO                       => ConnectorType.CHADEMO,
                   //WWCP.PlugTypes.CEE3                          => 
                   //WWCP.PlugTypes.CEE5                          => 
                   _                                            => null,
               };

        public static ConnectorType? ToOCPI(this WWCP.PlugTypes? PlugType)

            => PlugType.HasValue
                   ? PlugType.Value.ToOCPI()
                   : null;

        #endregion


        #region ToOCPI(this SocketOutlet, EVSE,  ref Warnings)

        public static Connector? ToOCPI(this WWCP.SocketOutlet  SocketOutlet,
                                        WWCP.IEVSE              EVSE,
                                        ref List<Warning>       Warnings)
        {

            var result = SocketOutlet.ToOCPI(EVSE, out var warnings);

            foreach (var warning in warnings)
                Warnings.Add(warning);

            return result;

        }

        #endregion

        #region ToOCPI(this SocketOutlet, EVSE,  out Warnings)

        public static Connector? ToOCPI(this WWCP.SocketOutlet    SocketOutlet,
                                        WWCP.IEVSE                EVSE,
                                        out IEnumerable<Warning>  Warnings)
        {

            var warnings = new List<Warning>();

            try
            {

                if (EVSE is null)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE must not be null!"));
                    Warnings = warnings;
                    return null;
                }

                var connectorId  = Connector_Id.Parse(SocketOutlet.Id.HasValue
                                                          ? SocketOutlet.Id.Value.ToString()
                                                          : "1");

                var powerType    = EVSE.CurrentType.ToOCPI();

                if (!powerType.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given EVSE current type '{EVSE.CurrentType}' could not be converted to an OCPI power type!"));
                    Warnings = warnings;
                    return null;
                }

                var standard     = SocketOutlet.Plug.ToOCPI();

                if (!standard.HasValue)
                {
                    warnings.Add(Warning.Create(Languages.en, $"The given socket outlet plug '{SocketOutlet.Plug}' could not be converted to an OCPI connector standard!"));
                    Warnings = warnings;
                    return null;
                }


                Warnings = Array.Empty<Warning>();

                return new Connector(

                           Id:                      connectorId,
                           Standard:                standard.Value,
                           Format:                  SocketOutlet.CableAttached switch {
                                                        true  => ConnectorFormats.CABLE,
                                                        _     => ConnectorFormats.SOCKET
                                                    },
                           PowerType:               powerType.Value,
                           MaxVoltage:              (UInt16) (EVSE.AverageVoltage ?? powerType.Value switch {
                                                        PowerTypes.AC_1_PHASE  => 230,
                                                        PowerTypes.AC_3_PHASE  => 230,  // Line to neutral for AC_3_PHASE: https://github.com/ocpi/ocpi/blob/master/mod_locations.asciidoc#mod_locations_connector_object
                                                        PowerTypes.DC          => 400,
                                                        _                      => 0
                                                    }),
                           MaxAmperage:             (UInt16) (EVSE.MaxCurrent     ?? powerType.Value switch {
                                                        PowerTypes.AC_1_PHASE  => 16,
                                                        PowerTypes.AC_3_PHASE  => 16,
                                                        PowerTypes.DC          => 50,
                                                        _                      => 0
                                                    }),
                           MaxElectricPower:        null,

                           TariffIds:               null,
                           TermsAndConditionsURL:   null,

                           LastUpdated:             EVSE.LastChange

                       );

            }
            catch (Exception ex)
            {
                warnings.Add(Warning.Create(Languages.en, $"Could not convert the given socket outlet to OCPI: " + ex.Message));
                Warnings = warnings;
            }

            return null;

        }

        #endregion


    }

}
