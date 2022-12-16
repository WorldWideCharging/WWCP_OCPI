﻿/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extensions methods for token types.
    /// </summary>
    public static class TokenTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a token type.
        /// </summary>
        /// <param name="Text">A text representation of a token type.</param>
        public static TokenTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return TokenTypes.OTHER;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a token type.
        /// </summary>
        /// <param name="Text">A text representation of a token type.</param>
        public static TokenTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out TokenType)

        /// <summary>
        /// Try to parse the given text as a token type.
        /// </summary>
        /// <param name="Text">A text representation of a token type.</param>
        /// <param name="TokenType">The parsed token type.</param>
        public static Boolean TryParse(String Text, out TokenTypes TokenType)
        {
            switch (Text.Trim().ToUpper())
            {

                case "RFID":
                    TokenType = TokenTypes.RFID;
                    return true;

                default:
                    TokenType = TokenTypes.OTHER;
                    return false;

            }
        }

        #endregion

        #region AsText(this TokenType)

        public static String AsText(this TokenTypes TokenType)

            => TokenType switch {
                   TokenTypes.RFID  => "RFID",
                   _                => "OTHER"
               };

        #endregion

    }


    /// <summary>
    /// Token types.
    /// </summary>
    public enum TokenTypes
    {

        /// <summary>
        /// Other type of token.
        /// </summary>
        OTHER,

        /// <summary>
        /// RFID Token.
        /// </summary>
        RFID

    }

}
