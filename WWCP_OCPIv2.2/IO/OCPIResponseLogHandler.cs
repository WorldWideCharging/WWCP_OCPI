﻿/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// The delegate for OCPI/HTTP response logs.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the incoming request.</param>
    /// <param name="OCPIAPI">The sending OCPI/HTTP API.</param>
    /// <param name="Request">The incoming OCPI/HTTP request.</param>
    /// <param name="Response">The outgoing OCPI/HTTP response.</param>
    public delegate Task OCPIResponseLogHandler(DateTime      Timestamp,
                                                HTTPAPI       OCPIAPI,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response);

}
