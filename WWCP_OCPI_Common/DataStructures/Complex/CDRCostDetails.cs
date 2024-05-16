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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// CDR Cost details.
    /// </summary>
    /// <param name="TotalEnergy">The total amount of energy charged.</param>
    /// <param name="TotalTime">The total amount of time.</param>
    /// 
    /// <param name="TotalFlatCost">The total flat cost.</param>
    /// <param name="TotalEnergyCost">The total energy cost.</param>
    /// <param name="TotalTimeCost">The total time cost.</param>
    /// <param name="TotalCost">The total cost.</param>
    /// 
    /// <param name="BilledFlatElements">The billed flat elements.</param>
    /// <param name="BilledEnergyElements">The billed energy elements.</param>
    /// <param name="BilledTimeElements">The billed time elements.</param>
    public class CDRCostDetails(Decimal                                          TotalEnergy            = 0,
                                TimeSpan?                                        TotalTime              = null,

                                Decimal                                          TotalFlatCost          = 0,
                                Decimal                                          TotalEnergyCost        = 0,
                                Decimal                                          TotalTimeCost          = 0,
                                Decimal                                          TotalCost              = 0,

                                Dictionary<String, CDRCostDetails.FlatCosts>?    BilledFlatElements     = null,
                                Dictionary<String, CDRCostDetails.EnergyCosts>?  BilledEnergyElements   = null,
                                Dictionary<String, CDRCostDetails.TimeCosts>?    BilledTimeElements     = null)
    {


        #region (class) FlatCosts

        public class FlatCosts
        {

            public Decimal   Price           { get; set; }


            #region ToJSON()

            /// <summary>
            /// Return a JSON representation of this object.
            /// </summary>
            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("price",   Price)
                   );

            #endregion

        }

        #endregion

        #region (class) EnergyCosts

        public class EnergyCosts
        {

            public Decimal   Energy          { get; set; }
            public UInt32    StepSize        { get; set; }
            public Decimal   Price           { get; set; }
            public Decimal   BilledEnergy    { get; set; }
            public Decimal   EnergyCost      { get; set; }


            #region ToJSON()

            /// <summary>
            /// Return a JSON representation of this object.
            /// </summary>
            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("energy",          Energy),
                       new JProperty("step_size",       StepSize),
                       new JProperty("price",           Price),
                       new JProperty("billed_energy",   BilledEnergy),
                       new JProperty("energy_cost",     EnergyCost)
                   );

            #endregion

        }

        #endregion

        #region (class) TimeCosts

        public class TimeCosts
        {

            public TimeSpan  Time            { get; set; }
            public UInt32    StepSize        { get; set; }
            public Decimal   Price           { get; set; }
            public TimeSpan  BilledTime      { get; set; }
            public Decimal   TimeCost        { get; set; }


            #region ToJSON()

            /// <summary>
            /// Return a JSON representation of this object.
            /// </summary>
            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("time",          Time),
                       new JProperty("step_size",     StepSize),
                       new JProperty("price",         Price),
                       new JProperty("billed_time",   BilledTime),
                       new JProperty("time_cost",     TimeCost)
                   );

            #endregion

        }

        #endregion


        #region Properties

        public Decimal   TotalEnergy        { get; set; }  = TotalEnergy;
        public TimeSpan  TotalTime          { get; set; }  = TotalTime ?? TimeSpan.Zero;

        public Decimal   BilledEnergy       { get; set; }
        public TimeSpan  BilledTime         { get; set; }

        public Decimal   TotalCost          { get; set; }  = TotalCost;
        public Decimal   TotalEnergyCost    { get; set; }  = TotalEnergyCost;
        public Decimal   TotalTimeCost      { get; set; }  = TotalTimeCost;
        public Decimal   TotalFlatCost      { get; set; }  = TotalFlatCost;


        public Dictionary<String, EnergyCosts>  BilledEnergyElements    { get; } = BilledEnergyElements ?? [];

        public Dictionary<String, TimeCosts>    BilledTimeElements      { get; } = BilledTimeElements   ?? [];

        public Dictionary<String, FlatCosts>    BilledFlatElements      { get; } = BilledFlatElements   ?? [];

        #endregion


        #region BillEnergy (StepSize, Energy, Price)

        public void BillEnergy(UInt32 StepSize, Decimal  Energy, Decimal Price)
        {

            if (!BilledEnergyElements.TryGetValue($"{StepSize}-{Price}", out var energy))
            {
                energy = new EnergyCosts {
                             StepSize  = StepSize,
                             Price     = Price
                         };
                BilledEnergyElements.Add($"{StepSize}-{Price}", energy);
            }

            energy.Energy += Energy;

        }

        #endregion

        #region BillTime   (StepSize, Time,   Price)

        public void BillTime  (UInt32 StepSize, TimeSpan Time,   Decimal Price)
        {

            if (!BilledTimeElements.TryGetValue($"{StepSize}-{Price}", out var time))
            {
                time = new TimeCosts {
                           StepSize  = StepSize,
                           Price     = Price
                       };
                BilledTimeElements.Add($"{StepSize}-{Price}", time);
            }

            time.Time += Time;

        }

        #endregion

        #region BillFlat   (                  Price)

        public void BillFlat  (Decimal Price)
        {

            // A FLAT cost is a cost that is not dependent on the amount of energy
            // or time used and thus will only be billed once!
            // When there are multiple different flat costs, they will be summed up!
            if (!BilledFlatElements.TryGetValue($"{Price}", out var flat))
            {
                flat = new FlatCosts {
                           Price     = Price
                       };
                BilledFlatElements.Add($"{Price}", flat);
            }

        }

        #endregion


        #region ToJSON(CustomCDRCostDetailsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRCostDetailsSerializer">A delegate to serialize custom CDRCostDetails JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDRCostDetails>?  CustomCDRCostDetailsSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("total_energy",             TotalEnergy),
                                 new JProperty("total_time",               TotalTime.TotalHours),

                                 new JProperty("total_flat_cost",          TotalFlatCost),
                                 new JProperty("total_energy_cost",        TotalEnergyCost),
                                 new JProperty("total_time_cost",          TotalTimeCost),
                                 new JProperty("total_cost",               TotalCost),

                           BilledFlatElements.  Count != 0
                               ? new JProperty("billed_flat_elements",     new JArray(BilledFlatElements.  Select(billedFlatElement   => billedFlatElement.  Value.ToJSON())))
                               : null,

                           BilledEnergyElements.Count != 0
                               ? new JProperty("billed_energy_elements",   new JArray(BilledEnergyElements.Select(billedEnergyElement => billedEnergyElement.Value.ToJSON())))
                               : null,

                           BilledTimeElements.Count   != 0
                               ? new JProperty("billed_time_elements",     new JArray(BilledTimeElements.  Select(billedTimeElement   => billedTimeElement.  Value.ToJSON())))
                               : null

                       );

            return CustomCDRCostDetailsSerializer is not null
                       ? CustomCDRCostDetailsSerializer(this, json)
                       : json;

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public CDRCostDetails Clone()

            => new (

                   TotalEnergy,
                   TotalTime,

                   TotalFlatCost,
                   TotalEnergyCost,
                   TotalTimeCost,
                   TotalCost,

                   BilledFlatElements.  ToDictionary(),
                   BilledEnergyElements.ToDictionary(),
                   BilledTimeElements.  ToDictionary()

               );

        #endregion


    }

}
