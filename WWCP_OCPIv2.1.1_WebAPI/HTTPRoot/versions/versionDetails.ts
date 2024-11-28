﻿///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
///<reference path="../defaults/defaults.ts" />

function StartVersionDetails(versionId: string) {

    const common                               = GetDefaults();
    common.topLeft.innerHTML                   = "/version/details"
    common.menuVersions.style.backgroundColor  = "#CCCCCC";
    common.menuVersions.href                   = "../versions";

    const versionDetailInfosDiv                = document.getElementById("versionDetailInfos")                as HTMLDivElement;

    const accessTokenEncoding                  = versionDetailInfosDiv.querySelector("#accessTokenEncoding")  as HTMLInputElement;
    const accessTokenInput                     = versionDetailInfosDiv.querySelector("#accessTokenInput")     as HTMLInputElement;
    const accessTokenButton                    = versionDetailInfosDiv.querySelector("#accessTokenButton")    as HTMLButtonElement;

    const versionDetailsDiv                    = versionDetailInfosDiv.querySelector("#versionDetails")       as HTMLDivElement;
    const versionIdDiv                         = versionDetailsDiv.    querySelector("#versionId")            as HTMLDivElement;
    const endpointsDiv                         = versionDetailsDiv.    querySelector("#endpoints")            as HTMLDivElement;

    accessTokenButton.onclick = () => {

        if (accessTokenEncoding.checked)
            localStorage.setItem("ocpiAccessTokenEncoding", "base64");
        else
            localStorage.removeItem("ocpiAccessTokenEncoding");


        var newAccessToken = accessTokenInput.value;

        if (newAccessToken !== "")
            localStorage.setItem("ocpiAccessToken", newAccessToken);
        else
            localStorage.removeItem("ocpiAccessToken");


        location.reload();

    };

    OCPIGet(window.location.href, // == "/versions/2.2"

            (status, response) => {

                try
                {

                    const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                    if (ocpiResponse?.data != undefined &&
                        ocpiResponse?.data != null)
                    {

                        const versionDetails = ocpiResponse.data as IVersionDetails;

                        versionIdDiv.innerHTML = "Version " + versionDetails.version;

                        for (const endpoint of versionDetails.endpoints) {

                            const endpointDiv      = endpointsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                            endpointDiv.className  = "endpoint";
                            endpointDiv.href       = endpoint.url;
                            endpointDiv.innerHTML  = endpoint.identifier + (versionDetails.version.startsWith("2.2") ? "/" + endpoint.role : "") + "<br /><span class=\"url\">" + endpoint.url + "</span>";

                        }

                    }

                }
                catch (exception) {
                }

            },

            (status, statusText, response) => {
            }

    );

}
