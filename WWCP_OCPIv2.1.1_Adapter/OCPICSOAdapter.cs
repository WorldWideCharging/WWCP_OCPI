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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using org.GraphDefined.Vanaheimr.Illias.Geometry;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPICSOAdapter : //AWWCPEMPAdapter<ChargeDetailRecord>,
                                  IEMPRoamingProvider,
                                  IEquatable <OCPICSOAdapter>,
                                  IComparable<OCPICSOAdapter>,
                                  IComparable

    {

        #region Data

        protected readonly  SemaphoreSlim  DataAndStatusLock     = new(1, 1);

        protected readonly  TimeSpan       MaxLockWaitingTime    = TimeSpan.FromSeconds(120);

        protected readonly Dictionary<IChargingPool, List<PropertyUpdateInfo>> chargingPoolsUpdateLog;

        #endregion

        #region Properties
        public CommonAPI                                    CommonAPI                            { get; }

        //public HTTPServer                                   HTTPServer                           { get; }
        //public HTTPPath                                     HTTPPathPrefix                       { get; }


        /// <summary>
        /// The global unique identification.
        /// </summary>
        [Mandatory]
        public EMPRoamingProvider_Id                        Id                                   { get; }

        /// <summary>
        /// The multi-language name.
        /// </summary>
        [Optional]
        public I18NString                                   Name                                 { get; }

        /// <summary>
        /// The multi-language description.
        /// </summary>
        [Optional]
        public I18NString                                   Description                          { get; }

        /// <summary>
        /// The roaming network.
        /// </summary>
        [Mandatory]
        public IRoamingNetwork                              RoamingNetwork                       { get; }


        /// <summary>
        /// A delegate for filtering charge detail records.
        /// </summary>
        public ChargeDetailRecordFilterDelegate             ChargeDetailRecordFilter             { get; }


        public WWCPEVSEId_2_EVSEId_Delegate?                CustomEVSEIdConverter                { get; }
        public WWCPEVSE_2_EVSE_Delegate?                    CustomEVSEConverter                  { get; }
        public WWCPEVSEStatusUpdate_2_StatusType_Delegate?  CustomEVSEStatusUpdateConverter      { get; }
        public WWCPChargeDetailRecord_2_CDR_Delegate?       CustomChargeDetailRecordConverter    { get; }





        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisablePushData                      { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisablePushAdminStatus               { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisablePushStatus                    { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisableAuthentication                { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisableSendChargeDetailRecords       { get; set; }

        #endregion

        #region Events

        // from OCPI CSO
        public event OnAuthorizeStartRequestDelegate?           OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate?          OnAuthorizeStartResponse;

        public event OnAuthorizeStopRequestDelegate?            OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate?           OnAuthorizeStopResponse;

        public event OnNewChargingSessionDelegate?              OnNewChargingSession;

        public event OnSendCDRsRequestDelegate?                 OnChargeDetailRecordRequest;
        public event OnSendCDRsResponseDelegate?                OnChargeDetailRecordResponse;
        public event OnNewChargeDetailRecordDelegate?           OnNewChargeDetailRecord;



        // from OCPI EMSP
        public event OnReserveRequestDelegate?                  OnReserveRequest;
        public event OnReserveResponseDelegate?                 OnReserveResponse;
        public event OnNewReservationDelegate?                  OnNewReservation;

        public event WWCP.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;
        public event WWCP.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;
        public event OnReservationCanceledDelegate?             OnReservationCanceled;

        public event OnRemoteStartRequestDelegate?              OnRemoteStartRequest;
        public event OnRemoteStartResponseDelegate?             OnRemoteStartResponse;

        public event OnRemoteStopRequestDelegate?               OnRemoteStopRequest;
        public event OnRemoteStopResponseDelegate?              OnRemoteStopResponse;

        public event WWCP.OnGetCDRsRequestDelegate?             OnGetChargeDetailRecordsRequest;
        public event WWCP.OnGetCDRsResponseDelegate?            OnGetChargeDetailRecordsResponse;
        public event OnSendCDRsResponseDelegate?                OnSendCDRsResponse;

        #endregion


        #region Constructor(s)

        public OCPICSOAdapter(EMPRoamingProvider_Id                        Id,
                              I18NString                                   Name,
                              I18NString                                   Description,
                              RoamingNetwork                               RoamingNetwork,

                              CommonAPI                                    CommonAPI,

                              WWCPEVSEId_2_EVSEId_Delegate?                CustomEVSEIdConverter               = null,
                              WWCPEVSE_2_EVSE_Delegate?                    CustomEVSEConverter                 = null,
                              WWCPEVSEStatusUpdate_2_StatusType_Delegate?  CustomEVSEStatusUpdateConverter     = null,
                              WWCPChargeDetailRecord_2_CDR_Delegate?       CustomChargeDetailRecordConverter   = null,

                              IncludeChargingStationOperatorIdDelegate?    IncludeChargingStationOperatorIds   = null,
                              IncludeChargingStationOperatorDelegate?      IncludeChargingStationOperators     = null,
                              IncludeChargingPoolIdDelegate?               IncludeChargingPoolIds              = null,
                              IncludeChargingPoolDelegate?                 IncludeChargingPools                = null,
                              IncludeChargingStationIdDelegate?            IncludeChargingStationIds           = null,
                              IncludeChargingStationDelegate?              IncludeChargingStations             = null,
                              IncludeEVSEIdDelegate?                       IncludeEVSEIds                      = null,
                              IncludeEVSEDelegate?                         IncludeEVSEs                        = null,
                              ChargeDetailRecordFilterDelegate?            ChargeDetailRecordFilter            = null,

                              Boolean                                      DisablePushData                     = false,
                              Boolean                                      DisablePushAdminStatus              = false,
                              Boolean                                      DisablePushStatus                   = false,
                              Boolean                                      DisableAuthentication               = false,
                              Boolean                                      DisableSendChargeDetailRecords      = false)

        {

            this.Id                                 = Id;
            this.Name                               = Name;
            this.Description                        = Description;
            this.RoamingNetwork                     = RoamingNetwork;

            this.CommonAPI                          = CommonAPI;

            this.CustomEVSEIdConverter              = CustomEVSEIdConverter;
            this.CustomEVSEConverter                = CustomEVSEConverter;
            this.CustomEVSEStatusUpdateConverter    = CustomEVSEStatusUpdateConverter;
            this.CustomChargeDetailRecordConverter  = CustomChargeDetailRecordConverter;

            this.IncludeChargingStationOperatorIds  = IncludeChargingStationOperatorIds ?? (chargingStationOperatorId  => true);
            this.IncludeChargingStationOperators    = IncludeChargingStationOperators   ?? (chargingStationOperator    => true);
            this.IncludeChargingPoolIds             = IncludeChargingPoolIds            ?? (chargingPoolId             => true);
            this.IncludeChargingPools               = IncludeChargingPools              ?? (chargingPool               => true);
            this.IncludeChargingStationIds          = IncludeChargingStationIds         ?? (chargingStationId          => true);
            this.IncludeChargingStations            = IncludeChargingStations           ?? (chargingStation            => true);
            this.IncludeEVSEIds                     = IncludeEVSEIds                    ?? (evseid                     => true);
            this.IncludeEVSEs                       = IncludeEVSEs                      ?? (evse                       => true);
            this.ChargeDetailRecordFilter           = ChargeDetailRecordFilter          ?? (chargeDetailRecord         => ChargeDetailRecordFilters.forward);

            this.DisablePushData                    = DisablePushData;
            this.DisablePushAdminStatus             = DisablePushAdminStatus;
            this.DisablePushStatus                  = DisablePushStatus;
            this.DisableAuthentication              = DisableAuthentication;
            this.DisableSendChargeDetailRecords     = DisableSendChargeDetailRecords;

            this.chargingPoolsUpdateLog             = new Dictionary<IChargingPool, List<PropertyUpdateInfo>>();

        }

        #endregion



        public IEnumerable<ChargingReservation> ChargingReservations
            => throw new NotImplementedException();

        public bool TryGetChargingReservationById(ChargingReservation_Id ReservationId, out ChargingReservation ChargingReservation)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingSessionById(ChargingSession_Id ChargingSessionId, out ChargingSession ChargingSession)
        {
            throw new NotImplementedException();
        }




        public IEnumerable<ChargingSession> ChargingSessions
            => throw new NotImplementedException();

        public TimeSpan MaxReservationDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IId AuthId => throw new NotImplementedException();

        IId ISendChargeDetailRecords.Id => throw new NotImplementedException();

        public Task<IEnumerable<ChargeDetailRecord>> GetChargeDetailRecords(DateTime From, DateTime? To = null, EMobilityProvider_Id? ProviderId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }



        #region (Set/Add/Update/Delete) Roaming network...
        Task<PushEVSEDataResult> ISendPOIData.SetStaticData(IRoamingNetwork     RoamingNetwork,
                                                            TransmissionTypes   TransmissionType,
                                                            DateTime?           Timestamp,
                                                            CancellationToken?  CancellationToken,
                                                            EventTracking_Id?   EventTrackingId,
                                                            TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.AddStaticData(IRoamingNetwork     RoamingNetwork,
                                                            TransmissionTypes   TransmissionType,
                                                            DateTime?           Timestamp,
                                                            CancellationToken?  CancellationToken,
                                                            EventTracking_Id?   EventTrackingId,
                                                            TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.UpdateStaticData(IRoamingNetwork     RoamingNetwork,
                                                               String              PropertyName,
                                                               Object?             OldValue,
                                                               Object?             NewValue,
                                                               TransmissionTypes   TransmissionType,
                                                               DateTime?           Timestamp,
                                                               CancellationToken?  CancellationToken,
                                                               EventTracking_Id?   EventTrackingId,
                                                               TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.DeleteStaticData(IRoamingNetwork     RoamingNetwork,
                                                               TransmissionTypes   TransmissionType,
                                                               DateTime?           Timestamp,
                                                               CancellationToken?  CancellationToken,
                                                               EventTracking_Id?   EventTrackingId,
                                                               TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }


        Task<PushRoamingNetworkAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<RoamingNetworkAdminStatusUpdate>  AdminStatusUpdates,
                                                                                     TransmissionTypes                             TransmissionType,
                                                                                     DateTime?                                     Timestamp,
                                                                                     CancellationToken?                            CancellationToken,
                                                                                     EventTracking_Id?                             EventTrackingId,
                                                                                     TimeSpan?                                     RequestTimeout)
        {
            return Task.FromResult(PushRoamingNetworkAdminStatusResult.NoOperation(Id, this));
        }

        Task<PushRoamingNetworkStatusResult> ISendStatus.UpdateStatus(IEnumerable<RoamingNetworkStatusUpdate>  StatusUpdates,
                                                                      TransmissionTypes                        TransmissionType,
                                                                      DateTime?                                Timestamp,
                                                                      CancellationToken?                       CancellationToken,
                                                                      EventTracking_Id?                        EventTrackingId,
                                                                      TimeSpan?                                RequestTimeout)
        {
            return Task.FromResult(PushRoamingNetworkStatusResult.NoOperation(Id, this));
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging station operator(s)...


        /// <summary>
        /// Only include charging station identifications matching the given delegate.
        /// </summary>
        public IncludeChargingStationOperatorIdDelegate  IncludeChargingStationOperatorIds    { get; }

        /// <summary>
        /// Only include charging stations matching the given delegate.
        /// </summary>
        public IncludeChargingStationOperatorDelegate    IncludeChargingStationOperators      { get; }


        Task<PushEVSEDataResult> ISendPOIData.SetStaticData(IChargingStationOperator  ChargingStationOperator,
                                                            TransmissionTypes         TransmissionType,
                                                            DateTime?                 Timestamp,
                                                            CancellationToken?        CancellationToken,
                                                            EventTracking_Id?         EventTrackingId,
                                                            TimeSpan?                 RequestTimeout)

            => Task.FromResult(PushEVSEDataResult.NoOperation(Id,
                                                              this,
                                                              null));//new IChargingStationOperator[] { ChargingStationOperator }));



        Task<PushEVSEDataResult> ISendPOIData.AddStaticData(IChargingStationOperator  ChargingStationOperator,
                                                            TransmissionTypes         TransmissionType,
                                                            DateTime?                 Timestamp,
                                                            CancellationToken?        CancellationToken,
                                                            EventTracking_Id?         EventTrackingId,
                                                            TimeSpan?                 RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.UpdateStaticData(IChargingStationOperator  ChargingStationOperator,
                                                               String                    PropertyName,
                                                               Object?                   OldValue,
                                                               Object?                   NewValue,
                                                               TransmissionTypes         TransmissionType,
                                                               DateTime?                 Timestamp,
                                                               CancellationToken?        CancellationToken,
                                                               EventTracking_Id?         EventTrackingId,
                                                               TimeSpan?                 RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.DeleteStaticData(IChargingStationOperator  ChargingStationOperator,
                                                               TransmissionTypes         TransmissionType,
                                                               DateTime?                 Timestamp,
                                                               CancellationToken?        CancellationToken,
                                                               EventTracking_Id?         EventTrackingId,
                                                               TimeSpan?                 RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }



        Task<PushEVSEDataResult> ISendPOIData.SetStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                            TransmissionTypes                      TransmissionType,
                                                            DateTime?                              Timestamp,
                                                            CancellationToken?                     CancellationToken,
                                                            EventTracking_Id?                      EventTrackingId,
                                                            TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.AddStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                            TransmissionTypes                      TransmissionType,
                                                            DateTime?                              Timestamp,
                                                            CancellationToken?                     CancellationToken,
                                                            EventTracking_Id?                      EventTrackingId,
                                                            TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.UpdateStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                               TransmissionTypes                      TransmissionType,
                                                               DateTime?                              Timestamp,
                                                               CancellationToken?                     CancellationToken,
                                                               EventTracking_Id?                      EventTrackingId,
                                                               TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.DeleteStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                               TransmissionTypes                      TransmissionType,
                                                               DateTime?                              Timestamp,
                                                               CancellationToken?                     CancellationToken,
                                                               EventTracking_Id?                      EventTrackingId,
                                                               TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }



        Task<PushChargingStationOperatorAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<ChargingStationOperatorAdminStatusUpdate>  AdminStatusUpdates,
                                                                                              TransmissionTypes                                      TransmissionType,
                                                                                              DateTime?                                              Timestamp,
                                                                                              CancellationToken?                                     CancellationToken,
                                                                                              EventTracking_Id?                                      EventTrackingId,
                                                                                              TimeSpan?                                              RequestTimeout)
        {
            return Task.FromResult(PushChargingStationOperatorAdminStatusResult.NoOperation(Id, this));
        }

        Task<PushChargingStationOperatorStatusResult> ISendStatus.UpdateStatus(IEnumerable<ChargingStationOperatorStatusUpdate>  StatusUpdates,
                                                                               TransmissionTypes                                 TransmissionType,
                                                                               DateTime?                                         Timestamp,
                                                                               CancellationToken?                                CancellationToken,
                                                                               EventTracking_Id?                                 EventTrackingId,
                                                                               TimeSpan?                                         RequestTimeout)
        {
            return Task.FromResult(PushChargingStationOperatorStatusResult.NoOperation(Id, this));
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging pool(s)...

        /// <summary>
        /// Only include charging pool identifications matching the given delegate.
        /// </summary>
        public IncludeChargingPoolIdDelegate  IncludeChargingPoolIds    { get; }

        /// <summary>
        /// Only include charging pools matching the given delegate.
        /// </summary>
        public IncludeChargingPoolDelegate    IncludeChargingPools      { get; }


        #region SetStaticData   (ChargingPool,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.SetStaticData(WWCP.IChargingPool      ChargingPool,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    if (IncludeChargingPools is null ||
                        (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {


                        var location = ChargingPool.ToOCPI(out warnings);

                        if (location is not null)
                            CommonAPI.AddLocation(location);


                        return WWCP.PushChargingPoolDataResult.Enqueued(
                                    Id,
                                    this,
                                    new IChargingPool[] {
                                        ChargingPool
                                    },
                                    "",
                                    warnings,
                                    TimeSpan.FromMilliseconds(10)
                                );

                    }
                    else
                        return WWCP.PushChargingPoolDataResult.NoOperation(
                                    Id,
                                    this,
                                    new IChargingPool[] {
                                        ChargingPool
                                    }
                                );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                                Id,
                                this,
                                new IChargingPool[] {
                                    ChargingPool
                                }
                            );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region AddStaticData   (ChargingPool,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.AddStaticData(WWCP.IChargingPool      ChargingPool,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {

                        var location = ChargingPool.ToOCPI(out warnings);

                        if (location is not null)
                        {

                            var result = CommonAPI.AddLocation(location);


                            //ToDo: Process errors!!!


                        }

                        return WWCP.PushChargingPoolDataResult.Enqueued(
                                   Id,
                                   this,
                                   new IChargingPool[] {
                                       ChargingPool
                                   },
                                   "",
                                   warnings,
                                   TimeSpan.FromMilliseconds(10)
                               );

                    }
                    else
                        return WWCP.PushChargingPoolDataResult.NoOperation(
                                   Id,
                                   this,
                                   new IChargingPool[] {
                                       ChargingPool
                                   }
                               );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                               Id,
                               this,
                               new IChargingPool[] {
                                   ChargingPool
                               }
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region UpdateStaticData(ChargingPool,  PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="PropertyName">The optional name of a charging pool property to update.</param>
        /// <param name="OldValue">The optional old value of a charging pool property to update.</param>
        /// <param name="NewValue">The optional new value of a charging pool property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<PushChargingPoolDataResult> UpdateStaticData(IChargingPool       ChargingPool,
                                                                       String?             PropertyName        = null,
                                                                       Object?             OldValue            = null,
                                                                       Object?             NewValue            = null,
                                                                       TransmissionTypes   TransmissionType    = TransmissionTypes.Enqueue,
                                                                       DateTime?           Timestamp           = null,
                                                                       CancellationToken?  CancellationToken   = null,
                                                                       EventTracking_Id?   EventTrackingId     = null,
                                                                       TimeSpan?           RequestTimeout      = null)
        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {

                        var location = ChargingPool.ToOCPI(out warnings);

                        if (location is not null)
                        {

                            var result = CommonAPI.UpdateLocation(location);

                            //ToDo: Process errors!!!

                            if (PropertyName is not null)
                            {

                                if (chargingPoolsUpdateLog.TryGetValue(ChargingPool, out var propertyUpdateInfos))
                                    propertyUpdateInfos.Add(new PropertyUpdateInfo(PropertyName, OldValue, NewValue));

                                else
                                    chargingPoolsUpdateLog.Add(ChargingPool,
                                                               new List<PropertyUpdateInfo> {
                                                                   new PropertyUpdateInfo(PropertyName, OldValue, NewValue)
                                                               });

                            }

                            return WWCP.PushChargingPoolDataResult.Enqueued(
                                       Id,
                                       this,
                                       new IChargingPool[] { ChargingPool },
                                       String.Empty,
                                       warnings,
                                       TimeSpan.Zero
                                   );

                        }

                    }

                    return WWCP.PushChargingPoolDataResult.NoOperation(
                               Id,
                               this,
                               new IChargingPool[] { ChargingPool },
                               String.Empty,
                               warnings,
                               TimeSpan.Zero
                           );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return PushChargingPoolDataResult.LockTimeout(
                       Id,
                       this,
                       new IChargingPool[] { ChargingPool },
                       "",
                       Array.Empty<Warning>(),
                       TimeSpan.Zero
                   );

        }

        #endregion

        #region DeleteStaticData(ChargingPool,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.DeleteStaticData(WWCP.IChargingPool       ChargingPool,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(PushChargingPoolDataResult.NoOperation(Id, this, new IChargingPool[] { ChargingPool }));

        }

        #endregion


        #region SetStaticData   (ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> SetStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                              WWCP.TransmissionTypes      TransmissionType,

                                                              DateTime?                   Timestamp,
                                                              CancellationToken?          CancellationToken,
                                                              EventTracking_Id?           EventTrackingId,
                                                              TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion

        #region SetStaticData   (ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> AddStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                              WWCP.TransmissionTypes      TransmissionType,

                                                              DateTime?                   Timestamp,
                                                              CancellationToken?          CancellationToken,
                                                              EventTracking_Id?           EventTrackingId,
                                                              TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion

        #region UpdateStaticData(ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> UpdateStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                                 WWCP.TransmissionTypes      TransmissionType,

                                                                 DateTime?                   Timestamp,
                                                                 CancellationToken?          CancellationToken,
                                                                 EventTracking_Id?           EventTrackingId,
                                                                 TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion

        #region DeleteStaticData(ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> DeleteStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                                 WWCP.TransmissionTypes      TransmissionType,

                                                                 DateTime?                   Timestamp,
                                                                 CancellationToken?          CancellationToken,
                                                                 EventTracking_Id?           EventTrackingId,
                                                                 TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion


        public Task<PushChargingPoolAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingPoolAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolStatusResult> UpdateStatus(IEnumerable<ChargingPoolStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging station(s)...


        /// <summary>
        /// Only include charging station identifications matching the given delegate.
        /// </summary>
        public IncludeChargingStationIdDelegate  IncludeChargingStationIds    { get; }

        /// <summary>
        /// Only include charging stations matching the given delegate.
        /// </summary>
        public IncludeChargingStationDelegate    IncludeChargingStations      { get; }


        #region SetStaticData   (ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given charging station.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.SetStaticData(WWCP.IChargingStation   ChargingStation,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var locationId = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (locationId.HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                                (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(out var warning);

                                    if (evse2 is not null)
                                        results.Add(await CommonAPI.AddOrUpdateEVSE(location, evse2));
                                    else
                                        results.Add(AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!"));

                                    if (warning is not null && warning.Any())
                                        warnings.AddRange(warning);

                                }

                            }

                        }


                        //ToDo: Process errors!!!


                    }

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                        ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new IChargingStation[] { ChargingStation })
                        : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new IChargingStation[] { ChargingStation });

        }

        #endregion

        #region AddStaticData   (ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given charging station.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.AddStaticData(WWCP.IChargingStation    ChargingStation,
                                            WWCP.TransmissionTypes   TransmissionType,

                                            DateTime?                Timestamp,
                                            CancellationToken?       CancellationToken,
                                            EventTracking_Id?        EventTrackingId,
                                            TimeSpan?                RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var locationId = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (locationId.HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(out var warning);

                                    if (evse2 is not null)
                                        results.Add(await CommonAPI.AddOrUpdateEVSE(location, evse2));
                                    else
                                        results.Add(AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!"));

                                    if (warning is not null && warning.Any())
                                        warnings.AddRange(warning);

                                }

                            }

                        }


                        //ToDo: Process errors!!!


                    }

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new IChargingStation[] { ChargingStation })
                       : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new IChargingStation[] { ChargingStation });

        }

        #endregion

        #region UpdateStaticData(ChargingStation, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the EVSE data of the given charging station within the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="PropertyName">The name of the charging station property to update.</param>
        /// <param name="OldValue">The old value of the charging station property to update.</param>
        /// <param name="NewValue">The new value of the charging station property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.UpdateStaticData(WWCP.IChargingStation    ChargingStation,
                                               String?                  PropertyName,
                                               Object?                  OldValue,
                                               Object?                  NewValue,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var locationId = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (locationId.HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(out var warning);

                                    if (evse2 is not null)
                                        results.Add(await CommonAPI.AddOrUpdateEVSE(location, evse2));
                                    else
                                        results.Add(AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!"));

                                    if (warning is not null && warning.Any())
                                        warnings.AddRange(warning);

                                }

                            }

                        }


                        //ToDo: Process errors!!!


                    }

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new IChargingStation[] { ChargingStation })
                       : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new IChargingStation[] { ChargingStation });

        }

        #endregion

        #region DeleteStaticData(ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the EVSE data of the given charging station from the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.DeleteStaticData(WWCP.IChargingStation    ChargingStation,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(PushChargingStationDataResult.NoOperation(Id, this, new IChargingStation[] { ChargingStation }));

        }

        #endregion

        public Task<PushChargingStationDataResult> SetStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> AddStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> UpdateStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> DeleteStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushChargingStationAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingStationAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationStatusResult> UpdateStatus(IEnumerable<ChargingStationStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (Set/Add/Update/Delete) EVSE(s)...

        /// <summary>
        /// Only include EVSE identifications matching the given delegate.
        /// </summary>
        public IncludeEVSEIdDelegate  IncludeEVSEIds    { get; }

        /// <summary>
        /// Only include EVSEs matching the given delegate.
        /// </summary>
        public IncludeEVSEDelegate    IncludeEVSEs      { get; }


        #region SetStaticData   (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given EVSE as new static EVSE data at the OICP server.
        /// </summary>
        /// <param name="EVSE">An EVSE to upload.</param>
        /// <param name="TransmissionType">Whether to send the EVSE directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.SetStaticData(WWCP.IEVSE              EVSE,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                          : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location, evse2);
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed("Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed("The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed("Invalid location identification!");


                    return result.IsSuccess

                               ? new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Success,
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Error,
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Success,
                                               warnings
                                           )
                                       },
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new IEVSE[] { EVSE });

        }

        #endregion

        #region AddStaticData   (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given EVSE.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.AddStaticData(IEVSE                   EVSE,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                          : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location, evse2);
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed("Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed("The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed("Invalid location identification!");


                    return result.IsSuccess

                               ? new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Success,
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Error,
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Success,
                                               warnings
                                           )
                                       },
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new IEVSE[] { EVSE });

        }

        #endregion

        #region UpdateStaticData(EVSE, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the EVSE data of the given charging pool within the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="PropertyName">The name of the charging pool property to update.</param>
        /// <param name="OldValue">The old value of the charging pool property to update.</param>
        /// <param name="NewValue">The new value of the charging pool property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.UpdateStaticData(WWCP.IEVSE              EVSE,
                                               String?                 PropertyName,
                                               Object?                 OldValue,
                                               Object?                 NewValue,
                                               WWCP.TransmissionTypes  TransmissionType,

                                               DateTime?               Timestamp,
                                               CancellationToken?      CancellationToken,
                                               EventTracking_Id?       EventTrackingId,
                                               TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                          : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location, evse2);
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed("Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed("The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed("Invalid location identification!");


                    return result.IsSuccess

                               ? new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Success,
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Error,
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Success,
                                               warnings
                                           )
                                       },
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new IEVSE[] { EVSE });

        }

        #endregion

        #region DeleteStaticData(EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the EVSE data of the given EVSE from the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.DeleteStaticData(WWCP.IEVSE               EVSE,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, new IEVSE[] { EVSE }));

        }

        #endregion


        public Task<PushEVSEDataResult> SetStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> DeleteStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        #region UpdateAdminStatus(AdminStatusUpdates, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given enumeration of EVSE admin status updates.
        /// </summary>
        /// <param name="AdminStatusUpdates">An enumeration of EVSE admin status updates.</param>
        /// <param name="TransmissionType">Whether to send the EVSE admin status updates directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushEVSEAdminStatusResult>

            WWCP.ISendAdminStatus.UpdateAdminStatus(IEnumerable<WWCP.EVSEAdminStatusUpdate>  AdminStatusUpdates,
                                                    WWCP.TransmissionTypes                   TransmissionType,

                                                    DateTime?                                Timestamp,
                                                    CancellationToken?                       CancellationToken,
                                                    EventTracking_Id?                        EventTrackingId,
                                                    TimeSpan?                                RequestTimeout)


                => Task.FromResult(WWCP.PushEVSEAdminStatusResult.NoOperation(Id, this));

        #endregion

        #region UpdateStatus     (StatusUpdates,      TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given enumeration of EVSE status updates.
        /// </summary>
        /// <param name="StatusUpdates">An enumeration of EVSE status updates.</param>
        /// <param name="TransmissionType">Whether to send the EVSE status updates directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<WWCP.PushEVSEStatusResult>

            UpdateStatus(IEnumerable<WWCP.EVSEStatusUpdate>  StatusUpdates,
                         WWCP.TransmissionTypes              TransmissionType,

                         DateTime?                           Timestamp,
                         CancellationToken?                  CancellationToken,
                         EventTracking_Id                    EventTrackingId,
                         TimeSpan?                           RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    PushEVSEStatusResult result;

                    var startTime  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                    var warnings   = new List<Warning>();
                    var results    = new List<PushEVSEStatusResult>();

                    foreach (var statusUpdate in StatusUpdates)
                    {

                        if (RoamingNetwork.TryGetEVSEById(statusUpdate.Id, out var evse) && evse is not null)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                var locationId = evse.ChargingPool is not null
                                                     ? Location_Id.TryParse(evse.ChargingPool.Id.Suffix)
                                                     : null;

                                if (locationId.HasValue)
                                {

                                    if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                        location is not null)
                                    {

                                        var evse2 = evse.ToOCPI(ref warnings);

                                        if (evse2 is not null)
                                        {

                                            var result2 = await CommonAPI.AddOrUpdateEVSE(location, evse2);

                                            result = result2.IsSuccess
                                                         ? PushEVSEStatusResult.Success(Id, this, null, warnings)
                                                         : PushEVSEStatusResult.Failed (Id, this, StatusUpdates, result2.ErrorResponse, warnings);

                                        }
                                        else
                                            result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Could not convert the given EVSE!");

                                    }
                                    else
                                        result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Unknown location identification!");

                                }
                                else
                                    result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Invalid location identification!");

                            }
                            else
                                result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "The given EVSE was filtered!");

                        }
                        else
                            result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "The given EVSE does not exist!");

                        results.Add(result);

                    }

                    return PushEVSEStatusResult.Flatten(
                               Id,
                               this,
                               results,
                               org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - startTime
                           );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEStatusResult.Enqueued   (Id, this)
                       : WWCP.PushEVSEStatusResult.LockTimeout(Id, this);

        }

        #endregion

        #endregion


        #region AuthorizeStart/-Stop

        public Task<AuthStartResult> AuthorizeStart(LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation = null, ChargingProduct ChargingProduct = null, ChargingSession_Id? SessionId = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<AuthStopResult> AuthorizeStop(ChargingSession_Id SessionId, LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Reserve

        public Task<ReservationResult> Reserve(ChargingLocation ChargingLocation, ChargingReservationLevel ReservationLevel = ChargingReservationLevel.EVSE, DateTime? StartTime = null, TimeSpan? Duration = null, ChargingReservation_Id? ReservationId = null, ChargingReservation_Id? LinkedReservationId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, ChargingProduct? ChargingProduct = null, IEnumerable<AuthenticationToken>? AuthTokens = null, IEnumerable<eMobilityAccount_Id>? eMAIds = null, IEnumerable<UInt32>? PINs = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<CancelReservationResult> CancelReservation(ChargingReservation_Id ReservationId, ChargingReservationCancellationReason Reason, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region RemoteStart/-Stop

        public Task<RemoteStartResult> RemoteStart(ChargingLocation ChargingLocation, ChargingProduct? ChargingProduct = null, ChargingReservation_Id? ReservationId = null, ChargingSession_Id? SessionId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteStopResult> RemoteStop(ChargingSession_Id SessionId, ReservationHandling? ReservationHandling = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SendChargeDetailRecords

        public Task<SendCDRsResult> SendChargeDetailRecords(IEnumerable<ChargeDetailRecord> ChargeDetailRecords, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion




        protected Boolean SkipFlushEVSEDataAndStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected Task FlushEVSEDataAndStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected Boolean SkipFlushEVSEFastStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected Task FlushEVSEFastStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected Boolean SkipFlushChargeDetailRecordsQueues()
        {
            throw new NotImplementedException();
        }

        protected Task FlushChargeDetailRecordsQueues(IEnumerable<ChargeDetailRecord> ChargeDetailsRecords)
        {
            throw new NotImplementedException();
        }



        public int CompareTo(OCPICSOAdapter? other)

            => other is null
                   ? throw new ArgumentException("The given object is not an OCPI CSO adapter!")
                   : Id.CompareTo(other.Id);

        public int CompareTo(Object? other)

            => other is OCPICSOAdapter OCPICSOAdapter
                   ? Id.CompareTo(OCPICSOAdapter.Id)
                   : throw new ArgumentException("The given object is not an OCPI CSO adapter!");


        public Boolean Equals(OCPICSOAdapter? other)

            => other is not null &&
                   Id.Equals(other.Id);

    }

}
