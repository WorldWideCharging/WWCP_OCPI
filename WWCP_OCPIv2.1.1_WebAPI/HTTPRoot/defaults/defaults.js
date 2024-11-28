// https://github.com/ocpi/ocpi/blob/release-2.1.1-bugfixes/
let topLeft = null;
function GetDefaults() {
    return {
        topLeft: document.getElementById("topLeft"),
        menuVersions: document.getElementById("menuVersions")
    };
}
function EncodeToken(str) {
    var buf = [];
    for (let i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }
    return buf.join('');
}
// #region OCPIGet(RessourceURI, AccessToken, OnSuccess, OnError)
function OCPIGet(RessourceURI, OnSuccess, OnError) {
    const ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
    ajax.setRequestHeader("X-Portal", "true");
    const accessToken = localStorage.getItem("ocpiAccessToken");
    const accessTokenEncoding = localStorage.getItem("ocpiAccessTokenEncoding");
    if (localStorage.getItem("OCPIAccessToken") != null)
        ajax.setRequestHeader("Authorization", "Token " + (accessTokenEncoding === "base64" ? btoa(accessToken) : accessToken));
    ajax.onreadystatechange = function () {
        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {
            // Ok
            if (this.status >= 100 && this.status < 300) {
                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));
                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);
            }
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    ajax.send();
}
// #endregion
//# sourceMappingURL=defaults.js.map