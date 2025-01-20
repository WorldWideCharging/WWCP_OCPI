﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Parking 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for parking space identifications.
    /// </summary>
    public static class ParkingIdExtensions
    {

        /// <summary>
        /// Indicates whether this parking space identification is null or empty.
        /// </summary>
        /// <param name="ParkingId">A parking space identification.</param>
        public static Boolean IsNullOrEmpty(this Parking_Id? ParkingId)
            => !ParkingId.HasValue || ParkingId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this parking space identification is NOT null or empty.
        /// </summary>
        /// <param name="ParkingId">A parking space identification.</param>
        public static Boolean IsNotNullOrEmpty(this Parking_Id? ParkingId)
            => ParkingId.HasValue && ParkingId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a parking space.
    /// </summary>
    public readonly struct Parking_Id : IId<Parking_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this parking space identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this parking space identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the parking space identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new parking space identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a parking space identification.</param>
        private Parking_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random parking space identification.
        /// </summary>
        public static Parking_Id NewRandom

            => Parse(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a parking space identification.
        /// </summary>
        /// <param name="Text">A text representation of a parking space identification.</param>
        public static Parking_Id Parse(String Text)
        {

            if (TryParse(Text, out var parkingId))
                return parkingId;

            throw new ArgumentException($"Invalid text representation of a parking space identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a parking space identification.
        /// </summary>
        /// <param name="Text">A text representation of a parking space identification.</param>
        public static Parking_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var parkingId))
                return parkingId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ParkingId)

        /// <summary>
        /// Try to parse the given text as a parking space identification.
        /// </summary>
        /// <param name="Text">A text representation of a parking space identification.</param>
        /// <param name="ParkingId">The parsed parking space identification.</param>
        public static Boolean TryParse(String Text, out Parking_Id ParkingId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ParkingId = new Parking_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ParkingId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this parking space identification.
        /// </summary>
        public Parking_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ParkingId1, ParkingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingId1">A parking space identification.</param>
        /// <param name="ParkingId2">Another parking space identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Parking_Id ParkingId1,
                                           Parking_Id ParkingId2)

            => ParkingId1.Equals(ParkingId2);

        #endregion

        #region Operator != (ParkingId1, ParkingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingId1">A parking space identification.</param>
        /// <param name="ParkingId2">Another parking space identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Parking_Id ParkingId1,
                                           Parking_Id ParkingId2)

            => !ParkingId1.Equals(ParkingId2);

        #endregion

        #region Operator <  (ParkingId1, ParkingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingId1">A parking space identification.</param>
        /// <param name="ParkingId2">Another parking space identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Parking_Id ParkingId1,
                                          Parking_Id ParkingId2)

            => ParkingId1.CompareTo(ParkingId2) < 0;

        #endregion

        #region Operator <= (ParkingId1, ParkingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingId1">A parking space identification.</param>
        /// <param name="ParkingId2">Another parking space identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Parking_Id ParkingId1,
                                           Parking_Id ParkingId2)

            => ParkingId1.CompareTo(ParkingId2) <= 0;

        #endregion

        #region Operator >  (ParkingId1, ParkingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingId1">A parking space identification.</param>
        /// <param name="ParkingId2">Another parking space identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Parking_Id ParkingId1,
                                          Parking_Id ParkingId2)

            => ParkingId1.CompareTo(ParkingId2) > 0;

        #endregion

        #region Operator >= (ParkingId1, ParkingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParkingId1">A parking space identification.</param>
        /// <param name="ParkingId2">Another parking space identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Parking_Id ParkingId1,
                                           Parking_Id ParkingId2)

            => ParkingId1.CompareTo(ParkingId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ParkingId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two parking space identifications.
        /// </summary>
        /// <param name="Object">A parking space identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Parking_Id parkingId
                   ? CompareTo(parkingId)
                   : throw new ArgumentException("The given object is not a parking space identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ParkingId)

        /// <summary>
        /// Compares two parking space identifications.
        /// </summary>
        /// <param name="ParkingId">A parking space identification to compare with.</param>
        public Int32 CompareTo(Parking_Id ParkingId)

            => String.Compare(InternalId,
                              ParkingId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ParkingId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two parking space identifications for equality.
        /// </summary>
        /// <param name="Object">A parking space identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Parking_Id parkingId &&
                   Equals(parkingId);

        #endregion

        #region Equals(ParkingId)

        /// <summary>
        /// Compares two parking space identifications for equality.
        /// </summary>
        /// <param name="ParkingId">A parking space identification to compare with.</param>
        public Boolean Equals(Parking_Id ParkingId)

            => String.Equals(InternalId,
                             ParkingId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}