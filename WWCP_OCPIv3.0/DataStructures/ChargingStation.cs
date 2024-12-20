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

using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A charging station.
    /// </summary>
    public class ChargingStation : PartyIssuedObjectReference<ChargingStation_Id>,
                                   IEquatable<ChargingStation>,
                                   IComparable<ChargingStation>,
                                   IComparable,
                                   IEnumerable<EVSE>
    {

        #region Data

        private readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent location of this ChargingStation.
        /// </summary>
        public Location?                        ParentLocation             { get; internal set; }

        /// <summary>
        /// The enumeration of EVSEs attached to this charging station.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE>                EVSEs                      { get; private set; }

        /// <summary>
        /// The enumeration of connector identifications attached to this ChargingStation.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_UId>            EVSEUIds
            => EVSEs.Select(evse => evse.UId);

        /// <summary>
        /// The enumeration of functionalities that the ChargingStation is capable of.
        /// </summary>
        [Optional]
        public IEnumerable<Capability>          Capabilities               { get; }

        /// <summary>
        /// The optional floor level on which the ChargingStation is located (in garage buildings)
        /// in the locally displayed numbering scheme.
        /// string(4)
        /// </summary>
        [Optional]
        public String?                          FloorLevel                 { get; }

        /// <summary>
        /// The optional geographical location of the ChargingStation.
        /// </summary>
        [Optional]
        public GeoCoordinate?                   Coordinates                { get; }

        /// <summary>
        /// The optional number/string printed on the outside of the ChargingStation for visual identification.
        /// string(16)
        /// </summary>
        [Optional]
        public String?                          PhysicalReference          { get; }

        /// <summary>
        /// The optional multi-language human-readable directions when more detailed
        /// information on how to reach the ChargingStation from the location is required.
        /// </summary>
        [Optional]
        public IEnumerable<DisplayText>         Directions                 { get; }

        /// <summary>
        /// The optional enumeration of images related to the ChargingStation such as photos or logos.
        /// </summary>
        [Optional]
        public IEnumerable<Image>               Images                     { get; }

        /// <summary>
        /// The optional (uplink) energy meter for the entire charging station.
        /// </summary>
        [Optional, NonStandard]
        public EnergyMeter?                     EnergyMeter                { get; }

        /// <summary>
        /// The timestamp when this ChargingStation was created.
        /// </summary>
        [Mandatory, NonStandard("Pagination")]
        public DateTime                         Created                    { get; }

        /// <summary>
        /// Timestamp when this ChargingStation was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                         LastUpdated                { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this ChargingStation.
        /// </summary>
        public String                           ETag                       { get; private set; }

        #endregion

        #region Constructor(s)

        #region (internal) ChargingStation(ParentLocation, UId, Status, Connectors, ... )

        /// <summary>
        /// Create a new ChargingStation.
        /// </summary>
        /// <param name="ParentLocation">The parent location of this ChargingStation.</param>
        /// 
        /// <param name="UId">An unique identification of the ChargingStation within the CPOs platform. For interoperability please make sure, that the internal ChargingStation UId has the same value as the official ChargingStation Id!</param>
        /// <param name="Status">A current status of the ChargingStation.</param>
        /// <param name="Connectors">An enumeration of available connectors attached to this ChargingStation.</param>
        /// 
        /// <param name="ChargingStationId">The official unique identification of the ChargingStation. For interoperability please make sure, that the internal ChargingStation UId has the same value as the official ChargingStation Id!</param>
        /// <param name="StatusSchedule">An enumeration of planned future status of the ChargingStation.</param>
        /// <param name="Capabilities">An enumeration of functionalities that the ChargingStation is capable of.</param>
        /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
        /// <param name="FloorLevel">An optional floor level on which the ChargingStation is located (in garage buildings) in the locally displayed numbering scheme.</param>
        /// <param name="Coordinates">An optional geographical location of the ChargingStation.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the ChargingStation for visual identification.</param>
        /// <param name="Directions">An optional multi-language human-readable directions when more detailed information on how to reach the ChargingStation from the location is required.</param>
        /// <param name="ParkingRestrictions">An optional enumeration of restrictions that apply to the parking spot.</param>
        /// <param name="Images">An optional enumeration of images related to the ChargingStation such as photos or logos.</param>
        /// 
        /// <param name="Created">The optional timestamp when this ChargingStation was created.</param>
        /// <param name="LastUpdated">The optional timestamp when this ChargingStation was last updated (or created).</param>
        /// 
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom ChargingStation JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        internal ChargingStation(Location?                                                     ParentLocation,
                                 ChargingStation_Id                                            Id,

                                 IEnumerable<EVSE>?                                            EVSEs                                        = null,
                                 IEnumerable<Capability>?                                      Capabilities                                 = null,
                                 String?                                                       FloorLevel                                   = null,
                                 GeoCoordinate?                                                Coordinates                                  = null,
                                 String?                                                       PhysicalReference                            = null,
                                 IEnumerable<DisplayText>?                                     Directions                                   = null,
                                 IEnumerable<Image>?                                           Images                                       = null,
                                 EnergyMeter?                                                  EnergyMeter                                  = null,

                                 DateTime?                                                     Created                                      = null,
                                 DateTime?                                                     LastUpdated                                  = null,

                                 EMSP_Id?                                                      EMSPId                                       = null,
                                 CustomJObjectSerializerDelegate<ChargingStation>?             CustomChargingStationSerializer              = null,
                                 CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                                 CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                                 CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                                 CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                                 CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                                 CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                                 CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                                 CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null)

            : base(Id)

        {

            this.ParentLocation        = ParentLocation;

            this.EVSEs                 = EVSEs?.       Distinct() ?? [];
            this.Capabilities          = Capabilities?.Distinct() ?? [];
            this.FloorLevel            = FloorLevel?.       Trim();
            this.Coordinates           = Coordinates;
            this.PhysicalReference     = PhysicalReference?.Trim();
            this.Directions            = Directions?.  Distinct() ?? [];
            this.Images                = Images?.      Distinct() ?? [];
            this.EnergyMeter           = EnergyMeter;

            this.Created               = Created                  ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated           = LastUpdated              ?? Created     ?? Timestamp.Now;

            foreach (var evse in this.EVSEs)
                evse.ParentChargingStation = this;

            this.ETag                  = CalcSHA256Hash(EMSPId,
                                                        CustomChargingStationSerializer,
                                                        CustomStatusScheduleSerializer,
                                                        CustomEVSESerializer,
                                                        CustomConnectorSerializer,
                                                        CustomEnergyMeterSerializer,
                                                        CustomTransparencySoftwareStatusSerializer,
                                                        CustomTransparencySoftwareSerializer,
                                                        CustomDisplayTextSerializer,
                                                        CustomImageSerializer);

        }

        #endregion

        #region ChargingStation(UId, Status, Connectors, ... )

        /// <summary>
        /// Create a new ChargingStation.
        /// </summary>
        /// <param name="UId">An unique identification of the ChargingStation within the CPOs platform. For interoperability please make sure, that the internal ChargingStation UId has the same value as the official ChargingStation Id!</param>
        /// <param name="Status">A current status of the ChargingStation.</param>
        /// <param name="Connectors">An enumeration of available connectors attached to this ChargingStation.</param>
        /// 
        /// <param name="ChargingStationId">The official unique identification of the ChargingStation. For interoperability please make sure, that the internal ChargingStation UId has the same value as the official ChargingStation Id!</param>
        /// <param name="StatusSchedule">An enumeration of planned future status of the ChargingStation.</param>
        /// <param name="Capabilities">An enumeration of functionalities that the ChargingStation is capable of.</param>
        /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
        /// <param name="FloorLevel">An optional floor level on which the ChargingStation is located (in garage buildings) in the locally displayed numbering scheme.</param>
        /// <param name="Coordinates">An optional geographical location of the ChargingStation.</param>
        /// <param name="PhysicalReference">An optional number/string printed on the outside of the ChargingStation for visual identification.</param>
        /// <param name="Directions">An optional multi-language human-readable directions when more detailed information on how to reach the ChargingStation from the location is required.</param>
        /// <param name="ParkingRestrictions">An optional enumeration of restrictions that apply to the parking spot.</param>
        /// <param name="Images">An optional enumeration of images related to the ChargingStation such as photos or logos.</param>
        /// 
        /// <param name="Created">The optional timestamp when this ChargingStation was created.</param>
        /// <param name="LastUpdated">The optional timestamp when this ChargingStation was last updated (or created).</param>
        /// 
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom ChargingStation JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public ChargingStation(ChargingStation_Id                                            Id,

                               IEnumerable<EVSE>?                                            EVSEs                                        = null,
                               IEnumerable<Capability>?                                      Capabilities                                 = null,
                               String?                                                       FloorLevel                                   = null,
                               GeoCoordinate?                                                Coordinates                                  = null,
                               String?                                                       PhysicalReference                            = null,
                               IEnumerable<DisplayText>?                                     Directions                                   = null,
                               IEnumerable<Image>?                                           Images                                       = null,
                               EnergyMeter?                                                  EnergyMeter                                  = null,

                               DateTime?                                                     Created                                      = null,
                               DateTime?                                                     LastUpdated                                  = null,

                               EMSP_Id?                                                      EMSPId                                       = null,
                               CustomJObjectSerializerDelegate<ChargingStation>?             CustomChargingStationSerializer              = null,
                               CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                               CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                               CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                               CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                               CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                               CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                               CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                               CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null)

            : this(null,
                   Id,

                   EVSEs,
                   Capabilities,
                   FloorLevel,
                   Coordinates,
                   PhysicalReference,
                   Directions,
                   Images,
                   EnergyMeter,

                   Created,
                   LastUpdated,

                   EMSPId,
                   CustomChargingStationSerializer,
                   CustomStatusScheduleSerializer,
                   CustomEVSESerializer,
                   CustomConnectorSerializer,
                   CustomEnergyMeterSerializer,
                   CustomTransparencySoftwareStatusSerializer,
                   CustomTransparencySoftwareSerializer,
                   CustomDisplayTextSerializer,
                   CustomImageSerializer)

            { }

        #endregion

        #endregion


        #region (static) Parse   (JSON, ChargingStationUIdURL = null, CustomChargingStationParser = null)

        /// <summary>
        /// Parse the given JSON representation of an ChargingStation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingStationUIdURL">An optional ChargingStation identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomChargingStationParser">A delegate to parse custom ChargingStation JSON objects.</param>
        public static ChargingStation Parse(JObject                             JSON,
                                 ChargingStation_Id?                           ChargingStationUIdURL         = null,
                                 CustomJObjectParserDelegate<ChargingStation>?  CustomChargingStationParser   = null)
        {

            if (TryParse(JSON,
                         out var evse,
                         out var errorResponse,
                         ChargingStationUIdURL,
                         CustomChargingStationParser))
            {
                return evse;
            }

            throw new ArgumentException("The given JSON representation of an ChargingStation is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingStation, out ErrorResponse, ChargingStationUIdURL = null, CustomChargingStationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an ChargingStation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingStation">The parsed ChargingStation.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out ChargingStation?    ChargingStation,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out ChargingStation,
                        out ErrorResponse,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an ChargingStation.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingStation">The parsed ChargingStation.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ChargingStationUIdURL">An optional ChargingStation identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomChargingStationParser">A delegate to parse custom ChargingStation JSON objects.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out ChargingStation?      ChargingStation,
                                       [NotNullWhen(false)] out String?    ErrorResponse,
                                       ChargingStation_Id?                           ChargingStationUIdURL,
                                       CustomJObjectParserDelegate<ChargingStation>?  CustomChargingStationParser   = null)
        {

            try
            {

                ChargingStation = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse UId                    [optional]

                if (JSON.ParseOptional("uid",
                                       "internal ChargingStation identification",
                                       ChargingStation_Id.TryParse,
                                       out ChargingStation_Id? ChargingStationUIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!ChargingStationUIdURL.HasValue && !ChargingStationUIdBody.HasValue)
                {
                    ErrorResponse = "The ChargingStation identification is missing!";
                    return false;
                }

                if (ChargingStationUIdURL.HasValue && ChargingStationUIdBody.HasValue && ChargingStationUIdURL.Value != ChargingStationUIdBody.Value)
                {
                    ErrorResponse = "The optional ChargingStation identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse EVSEs                  [optional]

                if (!JSON.ParseMandatoryJSON<EVSE, EVSE_UId>("connectors",
                                                             "connectors",
                                                             EVSE.TryParse,
                                                             out IEnumerable<EVSE> EVSEs,
                                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Capabilities           [optional]

                if (JSON.ParseOptionalHashSet("capabilities",
                                              "capabilities",
                                              Capability.TryParse,
                                              out HashSet<Capability> Capabilities,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse FloorLevel             [optional]

                var FloorLevel = JSON.GetString("floor_level");

                #endregion

                #region Parse Coordinates            [optional]

                if (JSON.ParseOptionalJSON("coordinates",
                                           "geo coordinates",
                                           GeoCoordinate.TryParse,
                                           out GeoCoordinate? Coordinates,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PhysicalReference      [optional]

                var PhysicalReference = JSON.GetString("physical_reference");

                #endregion

                #region Parse Directions             [optional]

                if (JSON.ParseOptionalJSON("directions",
                                           "directions",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Directions,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Images                 [optional]

                if (JSON.ParseOptionalJSON("images",
                                           "images",
                                           Image.TryParse,
                                           out IEnumerable<Image> Images,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMeter            [optional]

                if (JSON.ParseOptionalJSON("energy_meter",
                                           "energy meter",
                                           OCPI.EnergyMeter.TryParse,
                                           out EnergyMeter EnergyMeter,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created                [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTime? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated            [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChargingStation = new ChargingStation(

                                      ChargingStationUIdBody ?? ChargingStationUIdURL!.Value,

                                      EVSEs,
                                      Capabilities,
                                      FloorLevel,
                                      Coordinates,
                                      PhysicalReference,
                                      Directions,
                                      Images,
                                      EnergyMeter,

                                      Created,
                                      LastUpdated

                                  );


                if (CustomChargingStationParser is not null)
                    ChargingStation = CustomChargingStationParser(JSON,
                                                                  ChargingStation);

                return true;

            }
            catch (Exception e)
            {
                ChargingStation           = default;
                ErrorResponse  = "The given JSON representation of an ChargingStation is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingStationSerializer = null, CustomStatusScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom ChargingStation JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(EMSP_Id?                                                      EMSPId                                       = null,
                              CustomJObjectSerializerDelegate<ChargingStation>?             CustomChargingStationSerializer              = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                              CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                              CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                    Id.         ToString()),

                           EVSEs.Any()
                               ? new JProperty("evse",                  new JArray(EVSEs.              OrderBy(evse               => evse.UId).
                                                                                                       Select (evse               => evse.              ToJSON(EMSPId,
                                                                                                                                                               CustomEVSESerializer,
                                                                                                                                                               CustomStatusScheduleSerializer,
                                                                                                                                                               CustomConnectorSerializer,
                                                                                                                                                               CustomEnergyMeterSerializer,
                                                                                                                                                               CustomTransparencySoftwareStatusSerializer,
                                                                                                                                                               CustomTransparencySoftwareSerializer,
                                                                                                                                                               CustomDisplayTextSerializer,
                                                                                                                                                               CustomImageSerializer))))
                               : null,

                           Capabilities.Any()
                               ? new JProperty("capabilities",          new JArray(Capabilities.       Select (capability         => capability.        ToString())))
                               : null,

                           FloorLevel.IsNotNullOrEmpty()
                               ? new JProperty("floor_level",           FloorLevel)
                               : null,

                           Coordinates.HasValue
                               ? new JProperty("coordinates",           new JObject(
                                                                            new JProperty("latitude",   Coordinates.Value.Latitude. Value.ToString("0.00000##").Replace(",", ".")),
                                                                            new JProperty("longitude",  Coordinates.Value.Longitude.Value.ToString("0.00000##").Replace(",", "."))
                                                                        ))
                               : null,

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",    PhysicalReference)
                               : null,

                           Directions.Any()
                               ? new JProperty("directions",            new JArray(Directions.         Select (displayText        => displayText.       ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           Images.Any()
                               ? new JProperty("images",                new JArray(Images.             Select (image              => image.             ToJSON(CustomImageSerializer))))
                               : null,

                           EnergyMeter is not null
                               ? new JProperty("energy_meter",          EnergyMeter. ToJSON(CustomEnergyMeterSerializer,
                                                                                            CustomTransparencySoftwareStatusSerializer,
                                                                                            CustomTransparencySoftwareSerializer))
                               : null,

                           new JProperty("last_updated",                LastUpdated.ToIso8601())

                       );

            return CustomChargingStationSerializer is not null
                       ? CustomChargingStationSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ChargingStation Clone()

            => new (
                   ParentLocation,
                   Id.               Clone(),

                   EVSEs.              Select(evse               => evse.              Clone()),
                   Capabilities.       Select(capability         => capability.        Clone()),
                   FloorLevel.       CloneNullableString(),
                   Coordinates?.     Clone(),
                   PhysicalReference.CloneNullableString(),
                   Directions.         Select(displayText        => displayText.       Clone()),
                   Images.             Select(image              => image.             Clone()),
                   EnergyMeter?.     Clone(),

                   Created,
                   LastUpdated
               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject  JSON,
                                                     JObject  Patch)
        {

            foreach (var property in Patch)
            {

                if (property.Key == "uid")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'unique identification' of an ChargingStation is not allowed!");

                else if (property.Key == "connectors")
                    return PatchResult<JObject>.Failed(JSON,
                                                       "Patching the 'connectors' array of an ChargingStation is not allowed!");
                //{

                //    if (property.Value == null)
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Patching the 'connectors' array of a location to 'null' is not allowed!");

                //    else if (property.Value is JArray ConnectorsArray)
                //    {

                //        if (ConnectorsArray.Count == 0)
                //            return PatchResult<JObject>.Failed(JSON,
                //                                               "Patching the 'connectors' array of a location to '[]' is not allowed!");

                //        else
                //        {
                //            foreach (var connector in ConnectorsArray)
                //            {

                //                //ToDo: What to do with multiple ChargingStation objects having the same ChargingStationUId?
                //                if (connector is JObject ConnectorObject)
                //                {

                //                    if (ConnectorObject.ParseMandatory("id",
                //                                                       "connector identification",
                //                                                       Connector_Id.TryParse,
                //                                                       out Connector_Id  ConnectorId,
                //                                                       out String        ErrorResponse))
                //                    {

                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Patching the 'connectors' array of a location led to an error: " + ErrorResponse);

                //                    }

                //                    if (TryGetConnector(ConnectorId, out Connector Connector))
                //                    {
                //                        //Connector.Patch(ConnectorObject);
                //                    }
                //                    else
                //                    {

                //                        //ToDo: Create this "new" Connector!
                //                        return PatchResult<JObject>.Failed(JSON,
                //                                                           "Unknown connector identification!");

                //                    }

                //                }
                //                else
                //                {
                //                    return PatchResult<JObject>.Failed(JSON,
                //                                                       "Invalid JSON merge patch for 'connectors' array of a location: Data within the 'connectors' array is not a valid connector object!");
                //                }

                //            }
                //        }
                //    }

                //    else
                //    {
                //        return PatchResult<JObject>.Failed(JSON,
                //                                           "Invalid JSON merge patch for 'connectors' array of a location: JSON property 'connectors' is not an array!");
                //    }

                //}

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), ChargingStationPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject);

                            if (patchResult.IsSuccess)
                                JSON[property.Key] = patchResult.PatchedData;

                        }

                        else
                            JSON[property.Key] = subObject;

                    }

                    else
                        JSON.Add(property.Key, subObject);

                }

                //else if (property.Value is JArray subArray)
                //{
                //}

                else
                    JSON[property.Key] = property.Value;

            }

            return PatchResult<JObject>.Success(JSON);

        }

        #endregion

        #region TryPatch(ChargingStationPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representaion of this ChargingStation.
        /// </summary>
        /// <param name="ChargingStationPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<ChargingStation> TryPatch(JObject  ChargingStationPatch,
                                          Boolean  AllowDowngrades = false)
        {

            if (ChargingStationPatch is null)
                return PatchResult<ChargingStation>.Failed(this,
                                                "The given ChargingStation patch must not be null!");

            lock (patchLock)
            {

                if (ChargingStationPatch["last_updated"] is null)
                    ChargingStationPatch["last_updated"] = Timestamp.Now.ToIso8601();

                else if (AllowDowngrades == false &&
                        ChargingStationPatch["last_updated"].Type == JTokenType.Date &&
                       (ChargingStationPatch["last_updated"].Value<DateTime>().ToIso8601().CompareTo(LastUpdated.ToIso8601()) < 1))
                {
                    return PatchResult<ChargingStation>.Failed(this,
                                                    "The 'lastUpdated' timestamp of the ChargingStation patch must be newer then the timestamp of the existing ChargingStation!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), ChargingStationPatch);


                if (patchResult.IsFailed)
                    return PatchResult<ChargingStation>.Failed(this,
                                                    patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedChargingStation,
                             out var errorResponse))
                {

                    return PatchResult<ChargingStation>.Success(patchedChargingStation,
                                                     errorResponse);

                }

                else
                    return PatchResult<ChargingStation>.Failed(this,
                                                    "Invalid JSON merge patch of an ChargingStation: " + errorResponse);

            }

        }

        #endregion


        #region (internal) UpdateConnector(Connector)

        internal void UpdateConnector(EVSE EVSE)
        {

            if (EVSE is null)
                return;

            lock (EVSEs)
            {

                EVSEs = EVSEs.
                                 Where (evse => evse.UId != EVSE.UId).
                                 Concat([EVSE]);

            }

        }

        #endregion

        #region CalcSHA256Hash(CustomChargingStationSerializer = null, CustomStatusScheduleSerializer = null, ...)

        /// <summary>
        /// Calculate the SHA256 hash of the JSON representation of this location in HEX.
        /// </summary>
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom ChargingStation JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public String CalcSHA256Hash(EMSP_Id?                                                      EMSPId                                       = null,
                                     CustomJObjectSerializerDelegate<ChargingStation>?             CustomChargingStationSerializer              = null,
                                     CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               = null,
                                     CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                                     CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    = null,
                                     CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                                     CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null,
                                     CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  = null,
                                     CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        = null)
        {

            this.ETag = SHA256.HashData(ToJSON(EMSPId,
                                               CustomChargingStationSerializer,
                                               CustomStatusScheduleSerializer,
                                               CustomEVSESerializer,
                                               CustomConnectorSerializer,
                                               CustomEnergyMeterSerializer,
                                               CustomTransparencySoftwareStatusSerializer,
                                               CustomTransparencySoftwareSerializer,
                                               CustomDisplayTextSerializer,
                                               CustomImageSerializer).ToUTF8Bytes()).ToBase64();

            return this.ETag;

        }

        #endregion


        #region EVSEExists(EVSEId)

        /// <summary>
        /// Checks whether any connector having the given connector identification exists.
        /// </summary>
        /// <param name="EVSEId">A connector identification.</param>
        public Boolean EVSEExists(EVSE_UId EVSEId)
        {

            lock (EVSEs)
            {
                foreach (var evse in EVSEs)
                {
                    if (evse.UId == EVSEId)
                        return true;
                }
            }

            return false;

        }

        #endregion

        #region GetEVSE   (EVSEId)

        /// <summary>
        /// Return the connector having the given connector identification.
        /// </summary>
        /// <param name="EVSEId">A connector identification.</param>
        public EVSE? GetEVSE(EVSE_UId EVSEId)
        {

            if (TryGetEVSE(EVSEId, out var evse))
                return evse;

            return null;

        }

        #endregion

        #region TryGetEVSE(EVSEUId, out EVSE)

        /// <summary>
        /// Try to return the connector having the given connector identification.
        /// </summary>
        /// <param name="ConnectorId">A connector identification.</param>
        /// <param name="Connector">The connector having the given connector identification.</param>
        public Boolean TryGetEVSE(EVSE_UId   EVSEUId,
                                  out EVSE?  EVSE)
        {

            lock (EVSEs)
            {
                foreach (var evse in EVSEs)
                {
                    if (evse.UId == EVSEUId)
                    {
                        EVSE = evse;
                        return true;
                    }
                }
            }

            EVSE = null;
            return false;

        }

        #endregion

        #region IEnumerable<Connectors> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => EVSEs.GetEnumerator();

        public IEnumerator<EVSE> GetEnumerator()
            => EVSEs.GetEnumerator();

        #endregion


        #region Operator overloading

        #region Operator == (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An ChargingStation.</param>
        /// <param name="ChargingStation2">Another ChargingStation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingStation? ChargingStation1,
                                           ChargingStation? ChargingStation2)
        {

            if (Object.ReferenceEquals(ChargingStation1, ChargingStation2))
                return true;

            if (ChargingStation1 is null || ChargingStation2 is null)
                return false;

            return ChargingStation1.Equals(ChargingStation2);

        }

        #endregion

        #region Operator != (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An ChargingStation.</param>
        /// <param name="ChargingStation2">Another ChargingStation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingStation? ChargingStation1,
                                           ChargingStation? ChargingStation2)

            => !(ChargingStation1 == ChargingStation2);

        #endregion

        #region Operator <  (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An ChargingStation.</param>
        /// <param name="ChargingStation2">Another ChargingStation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingStation? ChargingStation1,
                                          ChargingStation? ChargingStation2)

            => ChargingStation1 is null
                   ? throw new ArgumentNullException(nameof(ChargingStation1), "The given ChargingStation must not be null!")
                   : ChargingStation1.CompareTo(ChargingStation2) < 0;

        #endregion

        #region Operator <= (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An ChargingStation.</param>
        /// <param name="ChargingStation2">Another ChargingStation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingStation? ChargingStation1,
                                           ChargingStation? ChargingStation2)

            => !(ChargingStation1 > ChargingStation2);

        #endregion

        #region Operator >  (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An ChargingStation.</param>
        /// <param name="ChargingStation2">Another ChargingStation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingStation? ChargingStation1,
                                          ChargingStation? ChargingStation2)

            => ChargingStation1 is null
                   ? throw new ArgumentNullException(nameof(ChargingStation1), "The given ChargingStation must not be null!")
                   : ChargingStation1.CompareTo(ChargingStation2) > 0;

        #endregion

        #region Operator >= (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An ChargingStation.</param>
        /// <param name="ChargingStation2">Another ChargingStation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingStation? ChargingStation1,
                                           ChargingStation? ChargingStation2)

            => !(ChargingStation1 < ChargingStation2);

        #endregion

        #endregion

        #region IComparable<ChargingStation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging stations.
        /// </summary>
        /// <param name="Object">A charging station to compare with.</param>
        public override Int32 CompareTo(Object? Object)

            => Object is ChargingStation chargingStation
                   ? CompareTo(chargingStation)
                   : throw new ArgumentException("The given object is not a charging station!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingStation)

        /// <summary>
        /// Compares two charging stations.
        /// </summary>
        /// <param name="ChargingStation">A charging station to compare with.</param>
        public Int32 CompareTo(ChargingStation? ChargingStation)
        {

            if (ChargingStation is null)
                throw new ArgumentNullException(nameof(ChargingStation), "The given charging station must not be null!");

            var c = Id.         CompareTo(ChargingStation.Id);

            if (c == 0)
                c = LastUpdated.CompareTo(ChargingStation.LastUpdated);

            if (c == 0)
                c = ETag.       CompareTo(ChargingStation.ETag);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingStation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging stations for equality.
        /// </summary>
        /// <param name="Object">A charging station to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingStation chargingStation &&
                   Equals(chargingStation);

        #endregion

        #region Equals(ChargingStation)

        /// <summary>
        /// Compares two charging stations for equality.
        /// </summary>
        /// <param name="ChargingStation">An ChargingStation to compare with.</param>
        public Boolean Equals(ChargingStation? ChargingStation)

            => ChargingStation is not null &&

               Id.                     Equals(ChargingStation.Id)                      &&
               LastUpdated.ToIso8601().Equals(ChargingStation.LastUpdated.ToIso8601()) &&

            ((!Coordinates.      HasValue    && !ChargingStation.Coordinates.      HasValue)    ||
              (Coordinates.      HasValue    &&  ChargingStation.Coordinates.      HasValue    && Coordinates.Value.Equals(ChargingStation.Coordinates.Value))) &&

             ((FloorLevel        is     null &&  ChargingStation.FloorLevel        is     null) ||
              (FloorLevel        is not null &&  ChargingStation.FloorLevel        is not null && FloorLevel.       Equals(ChargingStation.FloorLevel)))        &&

             ((PhysicalReference is     null &&  ChargingStation.PhysicalReference is     null) ||
              (PhysicalReference is not null &&  ChargingStation.PhysicalReference is not null && PhysicalReference.Equals(ChargingStation.PhysicalReference))) &&

             ((EnergyMeter       is     null &&  ChargingStation.EnergyMeter       is     null) ||
              (EnergyMeter       is not null &&  ChargingStation.EnergyMeter       is not null && EnergyMeter.      Equals(ChargingStation.EnergyMeter)))       &&

               EVSEs.              Count().Equals(ChargingStation.EVSEs.              Count()) &&
               Capabilities.       Count().Equals(ChargingStation.Capabilities.       Count()) &&
               Directions.         Count().Equals(ChargingStation.Directions.         Count()) &&
               Images.             Count().Equals(ChargingStation.Images.             Count()) &&

               EVSEs.              All(connector          => ChargingStation.EVSEs.              Contains(connector))          &&
               Capabilities.       All(capabilityType     => ChargingStation.Capabilities.       Contains(capabilityType))     &&
               Directions.         All(displayText        => ChargingStation.Directions.         Contains(displayText))        &&
               Images.             All(image              => ChargingStation.Images.             Contains(image));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private Int32? cachedHashCode;

        private readonly Object hashSync = new ();

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {

            if (cachedHashCode.HasValue)
                return cachedHashCode.Value;

            lock (hashSync)
            {

                unchecked
                {

                    cachedHashCode = Id.                GetHashCode()       * 43 ^
                                     EVSEs.             CalcHashCode()      * 37 ^
                                     Capabilities.      CalcHashCode()      * 23 ^
                                    (FloorLevel?.       GetHashCode() ?? 0) * 17 ^
                                    (Coordinates?.      GetHashCode() ?? 0) * 13 ^
                                    (PhysicalReference?.GetHashCode() ?? 0) * 11 ^
                                     Directions.        CalcHashCode()      *  7 ^
                                     Images.            CalcHashCode()      *  3 ^
                                    (EnergyMeter?.      GetHashCode() ?? 0) * 19 ^
                                     LastUpdated.       GetHashCode();

                    return cachedHashCode.Value;

                }

            }

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,

                   ", ",
                   EVSEs.Count(), " EVSE(s), ",

                   LastUpdated.ToIso8601()

               );

        #endregion


        #region ToBuilder(NewChargingStationUId = null)

        /// <summary>
        /// Return a builder for this ChargingStation.
        /// </summary>
        /// <param name="NewChargingStationUId">An optional new ChargingStation identification.</param>
        public Builder ToBuilder(ChargingStation_Id? NewChargingStationUId = null)

            => new (ParentLocation,
                    NewChargingStationUId ?? Id,

                    EVSEs,
                    Capabilities,
                    FloorLevel,
                    Coordinates,
                    PhysicalReference,
                    Directions,
                    Images,
                    EnergyMeter,

                    Created,
                    LastUpdated);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An ChargingStation builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The parent location of this ChargingStation.
            /// </summary>
            public Location?                        ParentLocation             { get; set; }

            /// <summary>
            /// The unique identification of the ChargingStation within the CPOs platform.
            /// For interoperability please make sure, that the ChargingStation UId has the same value as the official ChargingStation Id!
            /// </summary>
            [Mandatory]
            public ChargingStation_Id?              Id                        { get; set; }

            /// <summary>
            /// The enumeration of available connectors attached to this ChargingStation.
            /// </summary>
            [Mandatory]
            public HashSet<EVSE>                    EVSEs                 { get; }

            /// <summary>
            /// The enumeration of connector identifications attached to this ChargingStation.
            /// </summary>
            [Optional]
            public IEnumerable<EVSE_UId>            EVSEUIds
                => EVSEs.Select(evse => evse.UId);

            /// <summary>
            /// The enumeration of functionalities that the ChargingStation is capable of.
            /// </summary>
            [Optional]
            public HashSet<Capability>              Capabilities               { get; }

            /// <summary>
            /// The optional floor level on which the ChargingStation is located (in garage buildings)
            /// in the locally displayed numbering scheme.
            /// string(4)
            /// </summary>
            [Optional]
            public String?                          FloorLevel                 { get; set; }

            /// <summary>
            /// The optional geographical location of the ChargingStation.
            /// </summary>
            [Optional]
            public GeoCoordinate?                   Coordinates                { get; set; }

            /// <summary>
            /// The optional number/string printed on the outside of the ChargingStation for visual identification.
            /// string(16)
            /// </summary>
            [Optional]
            public String?                          PhysicalReference          { get; set; }

            /// <summary>
            /// The optional multi-language human-readable directions when more detailed
            /// information on how to reach the ChargingStation from the location is required.
            /// </summary>
            [Optional]
            public HashSet<DisplayText>             Directions                 { get; }

            /// <summary>
            /// The optional enumeration of images related to the ChargingStation such as photos or logos.
            /// </summary>
            [Optional]
            public HashSet<Image>                   Images                     { get; }

            /// <summary>
            /// The optional energy meter, e.g. for the German calibration law.
            /// </summary>
            [Optional, NonStandard]
            public EnergyMeter?                     EnergyMeter                { get; set; }

            /// <summary>
            /// The timestamp when this ChargingStation was created.
            /// </summary>
            [Mandatory, NonStandard("Pagination")]
            public DateTime                         Created                    { get; set; }

            /// <summary>
            /// Timestamp when this ChargingStation was last updated (or created).
            /// </summary>
            [Mandatory]
            public DateTime                         LastUpdated                { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new ChargingStation builder.
            /// </summary>
            /// <param name="ParentLocation">The parent location of this ChargingStation.</param>
            /// 
            /// <param name="UId">An unique identification of the ChargingStation within the CPOs platform. For interoperability please make sure, that the ChargingStation UId has the same value as the official ChargingStation Id!</param>
            /// <param name="Status">A current status of the ChargingStation.</param>
            /// <param name="Connectors">An enumeration of available connectors attached to this ChargingStation.</param>
            /// 
            /// <param name="ChargingStationId">The official unique identification of the ChargingStation. For interoperability please make sure, that the official ChargingStation Id has the same value as the internal ChargingStation UId!</param>
            /// <param name="StatusSchedule">An enumeration of planned future status of the ChargingStation.</param>
            /// <param name="Capabilities">An enumeration of functionalities that the ChargingStation is capable of.</param>
            /// <param name="EnergyMeter">An optional energy meter, e.g. for the German calibration law.</param>
            /// <param name="FloorLevel">An optional floor level on which the ChargingStation is located (in garage buildings) in the locally displayed numbering scheme.</param>
            /// <param name="Coordinates">An optional geographical location of the ChargingStation.</param>
            /// <param name="PhysicalReference">An optional number/string printed on the outside of the ChargingStation for visual identification.</param>
            /// <param name="Directions">An optional multi-language human-readable directions when more detailed information on how to reach the ChargingStation from the location is required.</param>
            /// <param name="ParkingRestrictions">An optional enumeration of restrictions that apply to the parking spot.</param>
            /// <param name="Images">An optional enumeration of images related to the ChargingStation such as photos or logos.</param>
            /// 
            /// <param name="Created">The optional timestamp when this ChargingStation was created.</param>
            /// <param name="LastUpdated">The optional timestamp when this ChargingStation was last updated (or created).</param>
            internal Builder(Location?                  ParentLocation      = null,
                             ChargingStation_Id?        Id                  = null,

                             IEnumerable<EVSE>?         EVSEs               = null,
                             IEnumerable<Capability>?   Capabilities        = null,
                             String?                    FloorLevel          = null,
                             GeoCoordinate?             Coordinates         = null,
                             String?                    PhysicalReference   = null,
                             IEnumerable<DisplayText>?  Directions          = null,
                             IEnumerable<Image>?        Images              = null,
                             EnergyMeter?               EnergyMeter         = null,

                             DateTime?                  Created             = null,
                             DateTime?                  LastUpdated         = null)

            {

                this.ParentLocation        = ParentLocation;
                this.Id                    = Id;

                this.EVSEs                 = EVSEs        is not null ? new HashSet<EVSE>       (EVSEs)        : [];
                this.Capabilities          = Capabilities is not null ? new HashSet<Capability> (Capabilities) : [];
                this.FloorLevel            = FloorLevel;
                this.Coordinates           = Coordinates;
                this.PhysicalReference     = PhysicalReference;
                this.Directions            = Directions   is not null ? new HashSet<DisplayText>(Directions)   : [];
                this.Images                = Images       is not null ? new HashSet<Image>      (Images)       : [];
                this.EnergyMeter           = EnergyMeter;

                this.Created               = Created     ?? LastUpdated ?? Timestamp.Now;
                this.LastUpdated           = LastUpdated ?? Created     ?? Timestamp.Now;

            }

            #endregion


            public Builder SetConnector(EVSE EVSE)
            {

                // ChargingStation.UpdateConnector(newOrUpdatedConnector);
                var newConnectors = EVSEs.Where(connector => connector.UId != EVSE.UId).ToHashSet();
                EVSEs.Clear();

                foreach (var newConnector in newConnectors)
                    EVSEs.Add(newConnector);

                EVSEs.Add(EVSE);

                return this;

            }


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the ChargingStation.
            /// </summary>
            public static implicit operator ChargingStation?(Builder? Builder)

                => Builder?.ToImmutable(out _);


            /// <summary>
            /// Return an immutable version of the ChargingStation.
            /// </summary>
            /// <param name="Warnings"></param>
            public ChargingStation? ToImmutable(out IEnumerable<Warning> Warnings)
            {

                var warnings = new List<Warning>();

                if (!Id.   HasValue)
                    warnings.Add(Warning.Create("The charging station identification must not be null or empty!"));

                Warnings = warnings;

                return warnings.Count != 0

                           ? null

                           : new ChargingStation(

                                 ParentLocation,
                                 Id.    Value,

                                 EVSEs,
                                 Capabilities,
                                 FloorLevel,
                                 Coordinates,
                                 PhysicalReference,
                                 Directions,
                                 Images,
                                 EnergyMeter,

                                 Created,
                                 LastUpdated

                             );

            }

            #endregion

        }

        #endregion


    }

}