///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
///<reference path="../defaults/defaults.ts" />
function StartVersionDetails(versionId) {
    const common = GetDefaults();
    common.topLeft.innerHTML = "/version/details";
    common.menuVersions.style.backgroundColor = "#CCCCCC";
    const versionDetailInfosDiv = document.getElementById("versionDetailInfos");
    const accessTokenInput = versionDetailInfosDiv.querySelector("#accessToken");
    const accessTokenButton = versionDetailInfosDiv.querySelector("#accessTokenButton");
    const versionDetailsDiv = versionDetailInfosDiv.querySelector("#versionDetails");
    const versionIdDiv = versionDetailsDiv.querySelector("#versionId");
    const endpointsDiv = versionDetailsDiv.querySelector("#endpoints");
    accessTokenButton.onclick = () => {
        var newAccessToken = accessTokenInput.value;
        if (newAccessToken !== "")
            localStorage.setItem("OCPIAccessToken", newAccessToken);
        else
            localStorage.removeItem("OCPIAccessToken");
        location.reload();
    };
    OCPIGet(window.location.href, // == "/versions/2.2"
    (status, response) => {
        try {
            const ocpiResponse = JSON.parse(response);
            if ((ocpiResponse === null || ocpiResponse === void 0 ? void 0 : ocpiResponse.data) != undefined &&
                (ocpiResponse === null || ocpiResponse === void 0 ? void 0 : ocpiResponse.data) != null) {
                const versionDetails = ocpiResponse.data;
                versionIdDiv.innerHTML = "Version " + versionDetails.version;
                for (const endpoint of versionDetails.endpoints) {
                    const endpointDiv = endpointsDiv.appendChild(document.createElement('a'));
                    endpointDiv.className = "endpoint";
                    endpointDiv.href = endpoint.url;
                    endpointDiv.innerHTML = endpoint.identifier + (versionDetails.version.startsWith("2.2") ? "/" + endpoint.role : "") + "<br /><span class=\"url\">" + endpoint.url + "</span>";
                }
            }
        }
        catch (exception) {
        }
    }, (status, statusText, response) => {
    });
}
//# sourceMappingURL=versionDetails.js.map