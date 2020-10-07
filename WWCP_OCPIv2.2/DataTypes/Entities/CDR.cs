﻿/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The CDR object describes the charging session and its costs,
    /// how these costs are composed, etc.
    /// </summary>
    public class CDR : IHasId<CDR_Id>,
                       IEquatable<CDR>,
                       IComparable<CDR>,
                       IComparable
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this charge detail record.
        /// </summary>
        [Optional]
        public CountryCode                  CountryCode                 { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this charge detail record (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public Party_Id                     PartyId                     { get; }

        /// <summary>
        /// The identification of the charge detail record within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public CDR_Id                       Id                          { get; }


        /// <summary>
        /// Start timestamp of the charging session, or in-case of a reservation
        /// (before the start of a session) the start of the reservation.
        /// </summary>
        [Mandatory]
        public DateTime                     Start                       { get; }

        /// <summary>
        /// The timestamp when the session was completed/finished, charging might
        /// have finished before the session ends, for example:
        /// EV is full, but parking cost also has to be paid.
        /// </summary>
        [Mandatory]
        public DateTime                     End                         { get; }

        /// <summary>
        /// Unique ID of the Session for which this CDR is sent. Is only allowed to be omitted
        /// when the CPO has not implemented the sessions module or this charge detail record
        /// is the result of a reservation that never became a charging session, thus no OCPI session.
        /// </summary>
        [Optional]
        public Session_Id?                  SessionId                   { get; }

        /// <summary>
        /// Token used to start this charging session, includes all the relevant information
        /// to identify the unique token.
        /// </summary>
        [Mandatory]
        public CDRToken                     CDRToken                    { get; }

        /// <summary>
        /// Method used for authentication.
        /// </summary>
        [Mandatory]
        public AuthMethods                  AuthMethod                  { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP. When the eMSP provided an
        /// authorization_reference in either: real-time authorization or StartSession,
        /// this field SHALL contain the same value. When different authorization_reference
        /// values have been given by the eMSP that are relevant to this Session, the last
        /// given value SHALL be used here.
        /// </summary>
        public AuthorizationReference?      AuthorizationReference      { get; }

        /// <summary>
        /// Location where the charging session took place, including only the relevant
        /// EVSE and connector.
        /// </summary>
        [Mandatory]
        public CDRLocation                  Location                    { get; }

        /// <summary>
        /// Optional identification of the kWh energy meter.
        /// </summary>
        [Optional]
        public Meter_Id?                    MeterId                     { get; }

        /// <summary>
        /// ISO 4217 code of the currency used for this CDR.
        /// </summary>
        [Mandatory]
        public Currency                     Currency                    { get; }

        /// <summary>
        /// Enumeration of relevant tariffs.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff>          Tariffs                     { get; }

        /// <summary>
        /// Enumeration of charging periods that make up this charging session.
        /// A session consist of 1 or more periodes with, each period has a
        /// different relevant Tariff.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingPeriod>  ChargingPeriods             { get; }

        /// <summary>
        /// Signed data that belongs to this charging session.
        /// </summary>
        [Optional]
        public SignedData?                  SignedData                  { get; }

        /// <summary>
        /// Total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Mandatory]
        public Price                        TotalCosts                  { get; }

        /// <summary>
        /// Total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalFixedCosts             { get; }

        /// <summary>
        /// Total energy charged, in kWh.
        /// </summary>
        [Mandatory]
        public Decimal                      TotalEnergy                 { get; }

        /// <summary>
        /// Total sum of all the cost of all the energy used, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalEnergyCost             { get; }


        /// <summary>
        /// Total duration of the charging session (including the duration of charging and not charging), in hours.
        /// </summary>
        [Mandatory]
        public Decimal                      TotalTime                   { get; }

        /// <summary>
        /// Total sum of all the cost related to duration of charging during this transaction, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalTimeCost               { get; }


        /// <summary>
        /// Total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV), in hours.
        /// </summary>
        [Optional]
        public Decimal?                     TotalParkingTime            { get; }

        /// <summary>
        /// Total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV), in hours.
        /// </summary>
        [Optional]
        public Decimal                      TotalChargingTime
            => TotalTime - (TotalParkingTime ?? 0);

        /// <summary>
        /// Total sum of all the cost related to parking of this transaction, including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalParkingCost            { get; }

        /// <summary>
        /// Total sum of all the cost related to a reservation of a Charge Point, including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public Price?                       TotalReservationCost        { get; }

        /// <summary>
        /// Optional remark, can be used to provide addition human
        /// readable information to the charge detail record, for example:
        /// reason why a transaction was stopped.
        /// </summary>
        [Optional]
        public String                       Remark                      { get; }

        /// <summary>
        /// This field can be used to reference an invoice, that will later be send for this CDR. Making it easier to link a CDR to a given invoice. Maybe even group CDRs that will be on the same invoice.
        /// </summary>
        [Optional]
        public InvoiceReference_Id?         InvoiceReferenceId          { get; }


        /// <summary>
        /// When set to true, this is a Credit CDR, and the field credit_reference_id needs to be set as well.
        /// </summary>
        [Optional]
        public Boolean?                     Credit                      { get; }

        /// <summary>
        /// Is required to be set for a Credit CDR. This SHALL contain the id of the CDR for which this is a Credit CDR.
        /// </summary>
        [Optional]
        public CreditReference_Id?          CreditReferenceId           { get; }


        /// <summary>
        /// Timestamp when this charge detail record was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                     LastUpdated                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record describing the charging session and its costs,
        /// how these costs are composed, etc.
        /// </summary>
        public CDR(CountryCode                  CountryCode,
                   Party_Id                     PartyId,
                   CDR_Id                       Id,
                   DateTime                     Start,
                   DateTime                     End,
                   CDRToken                     CDRToken,
                   AuthMethods                  AuthMethod,
                   CDRLocation                  Location,
                   Currency                     Currency,
                   IEnumerable<ChargingPeriod>  ChargingPeriods,
                   Price                        TotalCosts,
                   Decimal                      TotalEnergy,
                   Decimal                      TotalTime,

                   Session_Id?                  SessionId                = null,
                   AuthorizationReference?      AuthorizationReference   = null,
                   Meter_Id?                    MeterId                  = null,
                   IEnumerable<Tariff>          Tariffs                  = null,
                   SignedData?                  SignedData               = null,
                   Price?                       TotalFixedCosts          = null,
                   Price?                       TotalEnergyCost          = null,
                   Price?                       TotalTimeCost            = null,
                   Decimal?                     TotalParkingTime         = null,
                   Price?                       TotalParkingCost         = null,
                   Price?                       TotalReservationCost     = null,
                   String                       Remark                   = null,
                   InvoiceReference_Id?         InvoiceReferenceId       = null,
                   Boolean?                     Credit                   = null,
                   CreditReference_Id?          CreditReferenceId        = null,

                   DateTime?                    LastUpdated              = null)

        {

            #region Initial checks

            if (!ChargingPeriods.SafeAny())
                throw new ArgumentNullException(nameof(ChargingPeriods),  "The given enumeration of charging periods must not be null or empty!");

            #endregion

            this.CountryCode              = CountryCode;
            this.PartyId                  = PartyId;
            this.Id                       = Id;
            this.Start                    = Start;
            this.End                      = End;
            this.CDRToken                 = CDRToken;
            this.AuthMethod               = AuthMethod;
            this.Location                 = Location ?? throw new ArgumentNullException(nameof(Location),  "The given charging location must not be null!");
            this.Currency                 = Currency;
            this.ChargingPeriods          = ChargingPeriods;
            this.TotalCosts               = TotalCosts;
            this.TotalEnergy              = TotalEnergy;
            this.TotalTime                = TotalTime;

            this.SessionId                = SessionId;
            this.AuthorizationReference   = AuthorizationReference;
            this.MeterId                  = MeterId;
            this.Tariffs                  = Tariffs;
            this.SignedData               = SignedData;
            this.TotalFixedCosts          = TotalFixedCosts;
            this.TotalEnergyCost          = TotalEnergyCost;
            this.TotalTimeCost            = TotalTimeCost;
            this.TotalParkingTime         = TotalParkingTime;
            this.TotalParkingCost         = TotalParkingCost;
            this.TotalReservationCost     = TotalReservationCost;
            this.Remark                   = Remark;
            this.InvoiceReferenceId       = InvoiceReferenceId;
            this.Credit                   = Credit;
            this.CreditReferenceId        = CreditReferenceId;

            this.LastUpdated              = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region (static) Parse   (JSON, CDRIdURL = null, CustomCDRParser = null)

        /// <summary>
        /// Parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDRIdURL">An optional CDR identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom CDR JSON objects.</param>
        public static CDR Parse(JObject                               JSON,
                                    CountryCode?                          CountryCodeURL        = null,
                                    Party_Id?                             PartyIdURL            = null,
                                    CDR_Id?                           CDRIdURL          = null,
                                    CustomJObjectParserDelegate<CDR>  CustomCDRParser   = null)
        {

            if (TryParse(JSON,
                         out CDR  CDR,
                         out String   ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         CDRIdURL,
                         CustomCDRParser))
            {
                return CDR;
            }

            throw new ArgumentException("The given JSON representation of a CDR is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CDRIdURL = null, CustomCDRParser = null)

        /// <summary>
        /// Parse the given text representation of a CDR.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CDRIdURL">An optional CDR identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom CDR JSON objects.</param>
        public static CDR Parse(String                                Text,
                                    CountryCode?                          CountryCodeURL        = null,
                                    Party_Id?                             PartyIdURL            = null,
                                    CDR_Id?                           CDRIdURL          = null,
                                    CustomJObjectParserDelegate<CDR>  CustomCDRParser   = null)
        {

            if (TryParse(Text,
                         out CDR  CDR,
                         out String   ErrorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         CDRIdURL,
                         CustomCDRParser))
            {
                return CDR;
            }

            throw new ArgumentException("The given text representation of a CDR is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out CDR, out ErrorResponse, CDRIdURL = null, CustomCDRParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed CDR.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out CDR  CDR,
                                       out String   ErrorResponse)

            => TryParse(JSON,
                        out CDR,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed CDR.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="CDRIdURL">An optional CDR identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom CDR JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       out CDR                           CDR,
                                       out String                            ErrorResponse,
                                       CountryCode?                          CountryCodeURL        = null,
                                       Party_Id?                             PartyIdURL            = null,
                                       CDR_Id?                           CDRIdURL          = null,
                                       CustomJObjectParserDelegate<CDR>  CustomCDRParser   = null)
        {

            try
            {

                CDR = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode           [optional]

                if (JSON.ParseOptionalStruct("country_code",
                                             "country code",
                                             CountryCode.TryParse,
                                             out CountryCode? CountryCodeBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!CountryCodeURL.HasValue && !CountryCodeBody.HasValue)
                {
                    ErrorResponse = "The country code is missing!";
                    return false;
                }

                if (CountryCodeURL.HasValue && CountryCodeBody.HasValue && CountryCodeURL.Value != CountryCodeBody.Value)
                {
                    ErrorResponse = "The optional country code given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse PartyIdURL            [optional]

                if (JSON.ParseOptionalStruct("party_id",
                                             "party identification",
                                             Party_Id.TryParse,
                                             out Party_Id? PartyIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!PartyIdURL.HasValue && !PartyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && PartyIdBody.HasValue && PartyIdURL.Value != PartyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Id                    [optional]

                if (JSON.ParseOptionalStruct("id",
                                             "CDR identification",
                                             CDR_Id.TryParse,
                                             out CDR_Id? CDRIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!CDRIdURL.HasValue && !CDRIdBody.HasValue)
                {
                    ErrorResponse = "The CDR identification is missing!";
                    return false;
                }

                if (CDRIdURL.HasValue && CDRIdBody.HasValue && CDRIdURL.Value != CDRIdBody.Value)
                {
                    ErrorResponse = "The optional CDR identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Publish               [mandatory]

                if (!JSON.ParseMandatory("publish",
                                         "publish",
                                         out Boolean Publish,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Address               [mandatory]

                if (!JSON.ParseMandatoryText("address",
                                             "address",
                                             out String Address,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse City                  [mandatory]

                if (!JSON.ParseMandatoryText("city",
                                             "city",
                                             out String City,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Country               [mandatory]

                if (!JSON.ParseMandatoryText("country",
                                             "country",
                                             out String Country,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Coordinates           [mandatory]

                //if (!JSON.ParseMandatoryJSON("coordinates",
                //                             "geo coordinates",
                //                             GeoCoordinate.TryParse,
                //                             out GeoCoordinate Coordinates,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                #endregion

                #region Parse TimeZone              [mandatory]

                if (!JSON.ParseMandatoryText("time_zone",
                                             "time zone",
                                             out String TimeZone,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse PublishTokenTypes     [optional]

                if (JSON.ParseOptionalJSON("publish_allowed_to",
                                           "publish allowed to",
                                           PublishTokenType.TryParse,
                                           out IEnumerable<PublishTokenType> PublishTokenTypes,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Name                  [optional]

                var Name = JSON.GetString("name");

                #endregion

                #region Parse PostalCode            [optional]

                var PostalCode = JSON.GetString("postal_code");

                #endregion

                #region Parse State                 [optional]

                var State = JSON.GetString("state");

                #endregion

                #region Parse RelatedCDRs      [optional]

                //if (JSON.ParseOptionalJSON("related_CDRs",
                //                           "related CDRs",
                //                           AdditionalGeoCDR.TryParse,
                //                           out IEnumerable<AdditionalGeoCDR> RelatedCDRs,
                //                           out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                #endregion

                #region Parse ParkingType           [optional]

                if (JSON.ParseOptionalEnum("parking_type",
                                           "parking type",
                                           out ParkingTypes? ParkingType,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse EVSEs                 [optional]

                if (JSON.ParseOptionalJSON("evses",
                                           "evses",
                                           EVSE.TryParse,
                                           out IEnumerable<EVSE> EVSEs,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Directions            [optional]

                if (JSON.ParseOptionalJSON("directions",
                                           "multi-language directions",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Directions,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Operator              [optional]

                if (JSON.ParseOptionalJSON("operator",
                                           "operator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Operator,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Suboperator           [optional]

                if (JSON.ParseOptionalJSON("suboperator",
                                           "suboperator",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Suboperator,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Owner                 [optional]

                if (JSON.ParseOptionalJSON("owner",
                                           "owner",
                                           BusinessDetails.TryParse,
                                           out BusinessDetails Owner,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Facilities            [optional]

                if (JSON.ParseOptionalEnums("facilities",
                                            "facilities",
                                            out IEnumerable<Facilities> Facilities,
                                            out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse OpeningTimes          [optional]

                if (JSON.ParseOptionalJSON("opening_times",
                                           "opening times",
                                           Hours.TryParse,
                                           out Hours OpeningTimes,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse ChargingWhenClosed    [optional]

                if (JSON.ParseOptional("charging_when_closed",
                                       "charging when closed",
                                       out Boolean? ChargingWhenClosed,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Images                [optional]

                if (JSON.ParseOptionalJSON("images",
                                           "images",
                                           Image.TryParse,
                                           out IEnumerable<Image> Images,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse EnergyMix             [optional]

                if (JSON.ParseOptionalJSON("energy_mix",
                                           "energy mix",
                                           OCPIv2_2.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                #region Parse LastUpdated           [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTime LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                //CDR = new CDR(CountryCodeBody ?? CountryCodeURL.Value,
                //                        PartyIdBody     ?? PartyIdURL.Value,
                //                        CDRIdBody  ?? CDRIdURL.Value,
                //                        Publish,
                //                        Address?.   Trim(),
                //                        City?.      Trim(),
                //                        Country?.   Trim(),
                //                        Coordinates,
                //                        TimeZone?.  Trim(),

                //                        PublishTokenTypes,
                //                        Name?.      Trim(),
                //                        PostalCode?.Trim(),
                //                        State?.     Trim(),
                //                        RelatedCDRs?.Distinct(),
                //                        ParkingType,
                //                        EVSEs?.           Distinct(),
                //                        Directions?.      Distinct(),
                //                        Operator,
                //                        Suboperator,
                //                        Owner,
                //                        Facilities?.      Distinct(),
                //                        OpeningTimes,
                //                        ChargingWhenClosed,
                //                        Images?.          Distinct(),
                //                        EnergyMix,
                //                        LastUpdated);

                CDR = null;

                if (CustomCDRParser != null)
                    CDR = CustomCDRParser(JSON,
                                                  CDR);

                return true;

            }
            catch (Exception e)
            {
                CDR        = default;
                ErrorResponse  = "The given JSON representation of a CDR is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out CDR, out ErrorResponse, CDRIdURL = null, CustomCDRParser = null)

        /// <summary>
        /// Try to parse the given text representation of a CDR.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CDR">The parsed CDR.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CDRIdURL">An optional CDR identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom CDR JSON objects.</param>
        public static Boolean TryParse(String                                Text,
                                       out CDR                           CDR,
                                       out String                            ErrorResponse,
                                       CountryCode?                          CountryCodeURL        = null,
                                       Party_Id?                             PartyIdURL            = null,
                                       CDR_Id?                           CDRIdURL          = null,
                                       CustomJObjectParserDelegate<CDR>  CustomCDRParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out CDR,
                                out ErrorResponse,
                                CountryCodeURL,
                                PartyIdURL,
                                CDRIdURL,
                                CustomCDRParser);

            }
            catch (Exception e)
            {
                CDR        = null;
                ErrorResponse  = "The given text representation of a CDR is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRSerializer">A delegate to serialize custom CDR JSON objects.</param>
        /// <param name="CustomPublishTokenTypeSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoCDRSerializer">A delegate to serialize custom additional geo CDR JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom connector JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomHoursSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDR>                CustomCDRSerializer                 = null,
                              CustomJObjectSerializerDelegate<PublishTokenType>       CustomPublishTokenTypeSerializer        = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>  CustomAdditionalGeoCDRSerializer    = null,
                              CustomJObjectSerializerDelegate<EVSE>                   CustomEVSESerializer                    = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>         CustomStatusScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<Connector>              CustomConnectorSerializer               = null,
                              CustomJObjectSerializerDelegate<DisplayText>            CustomDisplayTextSerializer             = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>        CustomBusinessDetailsSerializer         = null,
                              CustomJObjectSerializerDelegate<Hours>                  CustomHoursSerializer                   = null,
                              CustomJObjectSerializerDelegate<Image>                  CustomImageSerializer                   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",                    CountryCode.ToString()),
                           new JProperty("party_id",                        PartyId.    ToString()),
                           new JProperty("id",                              Id.         ToString()),
                           //new JProperty("publish",                         Publish),

                           //Publish == false && PublishAllowedTo.SafeAny()
                           //    ? new JProperty("publish_allowed_to",        new JArray(PublishAllowedTo.Select(publishAllowedTo => publishAllowedTo.ToJSON(CustomPublishTokenTypeSerializer))))
                           //    : null,

                           //Name.IsNotNullOrEmpty()
                           //    ? new JProperty("name",                      Name)
                           //    : null,

                           //new JProperty("address",                         Address),
                           //new JProperty("city",                            City),

                           //PostalCode.IsNotNullOrEmpty()
                           //    ? new JProperty("postal_code",               PostalCode)
                           //    : null,

                           //State.IsNotNullOrEmpty()
                           //    ? new JProperty("state",                     State)
                           //    : null,

                           //new JProperty("country",                         Country),
                           //new JProperty("coordinates",                     new JObject(
                           //                                                     new JProperty("latitude",  Coordinates.Latitude. Value.ToString()),
                           //                                                     new JProperty("longitude", Coordinates.Longitude.Value.ToString())
                           //                                                 )),

                           //RelatedCDRs.SafeAny()
                           //    ? new JProperty("related_CDRs",         new JArray(RelatedCDRs.Select(CDR => CDR.ToJSON(CustomAdditionalGeoCDRSerializer))))
                           //    : null,

                           //ParkingType.HasValue
                           //    ? new JProperty("parking_type",              ParkingType.Value.ToString())
                           //    : null,

                           //EVSEs.SafeAny()
                           //    ? new JProperty("evses",                     new JArray(EVSEs.Select(evse => evse.ToJSON(CustomEVSESerializer,
                           //                                                                                             CustomStatusScheduleSerializer,
                           //                                                                                             CustomConnectorSerializer))))
                           //    : null,

                           //Directions.SafeAny()
                           //    ? new JProperty("directions",                new JArray(Directions.Select(evse => evse.ToJSON(CustomDisplayTextSerializer))))
                           //    : null,

                           //Operator != null
                           //    ? new JProperty("operator",                  Operator.   ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //SubOperator != null
                           //    ? new JProperty("suboperator",               SubOperator.ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //Owner != null
                           //    ? new JProperty("owner",                     Owner.      ToJSON(CustomBusinessDetailsSerializer))
                           //    : null,

                           //Facilities.SafeAny()
                           //    ? new JProperty("facilities",                new JArray(Facilities.Select(facility => facility.ToString())))
                           //    : null,

                           //new JProperty("time_zone",                       Timezone),

                           //OpeningTimes != null
                           //    ? new JProperty("opening_times",             OpeningTimes.ToJSON(CustomHoursSerializer))
                           //    : null,

                           //ChargingWhenClosed.HasValue
                           //    ? new JProperty("charging_when_closed",      ChargingWhenClosed.Value)
                           //    : null,

                           //Images.SafeAny()
                           //    ? new JProperty("images",                    new JArray(Images.Select(image => image.ToJSON(CustomImageSerializer))))
                           //    : null,

                           //EnergyMix != null
                           //    ? new JProperty("energy_mix",                EnergyMix.ToJSON())
                           //    : null,

                           new JProperty("last_updated",                    LastUpdated.ToIso8601())

                       );

            return CustomCDRSerializer != null
                       ? CustomCDRSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDR CDR1,
                                           CDR CDR2)
        {

            if (Object.ReferenceEquals(CDR1, CDR2))
                return true;

            if (CDR1 is null || CDR2 is null)
                return false;

            return CDR1.Equals(CDR2);

        }

        #endregion

        #region Operator != (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDR CDR1,
                                           CDR CDR2)

            => !(CDR1 == CDR2);

        #endregion

        #region Operator <  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR CDR1,
                                          CDR CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) < 0;

        #endregion

        #region Operator <= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDR CDR1,
                                           CDR CDR2)

            => !(CDR1 > CDR2);

        #endregion

        #region Operator >  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR CDR1,
                                          CDR CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) > 0;

        #endregion

        #region Operator >= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDR CDR1,
                                           CDR CDR2)

            => !(CDR1 < CDR2);

        #endregion

        #endregion

        #region IComparable<CDR> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDR cdr
                   ? CompareTo(cdr)
                   : throw new ArgumentException("The given object is not a charge detail record!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDR)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR">An CDR to compare with.</param>
        public Int32 CompareTo(CDR CDR)

            => CDR is null
                   ? throw new ArgumentNullException(nameof(CDR), "The given CDR must not be null!")
                   : Id.CompareTo(CDR.Id);

        #endregion

        #endregion

        #region IEquatable<CDR> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDR cdr &&
                   Equals(cdr);

        #endregion

        #region Equals(CDR)

        /// <summary>
        /// Compares two CDRs for equality.
        /// </summary>
        /// <param name="CDR">An CDR to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDR CDR)

            => !(CDR is null) &&
                   Id.Equals(CDR.Id);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
