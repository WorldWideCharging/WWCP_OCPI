﻿///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartTariffs()
{

    function CreateLine(parent: HTMLDivElement | HTMLAnchorElement, className: string, key: string, innerHTML: string | HTMLDivElement): HTMLDivElement {

        const rowDiv = parent.appendChild(document.createElement('div')) as HTMLDivElement;
        rowDiv.className = "row";

        // key
        const keyDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        keyDiv.className = "key";
        keyDiv.innerHTML = key;

        // value
        const valueDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        valueDiv.className = "value " + className;

        if (typeof innerHTML === 'string')
            valueDiv.innerHTML = innerHTML;

        else if (innerHTML instanceof HTMLDivElement)
            valueDiv.appendChild(innerHTML);


        return rowDiv;

    }


    const common                       = GetDefaults();
    common.topLeft.innerHTML           = "/Tariffs"
    common.menuVersions.classList.add("activated");
    common.menuVersions.href           = "../../versions";

    const tariffInfosDiv               = document.getElementById("tariffInfos")                     as HTMLDivElement;
    const tariffsDiv                   = tariffInfosDiv.querySelector("#tariffs")                   as HTMLDivElement;

    const numberOfTariffsDiv           = tariffInfosDiv.querySelector("#numberOfTariffs")           as HTMLDivElement;
 
    let   totalNumberOfEVSEs           = 0;


    OCPIGet(window.location.href,

            (status, response) => {

                try
                {

                    const ocpiResponse = JSON.parse(response) as IOCPIResponse;

                    if (ocpiResponse?.data != undefined  &&
                        ocpiResponse?.data != null       &&
                        Array.isArray(ocpiResponse.data) &&
                        ocpiResponse.data.length > 0)
                    {

                        numberOfTariffsDiv.innerHTML = ocpiResponse.data.length.toString();

                        for (const tariff of (ocpiResponse.data as ITariff[])) {

                            totalNumberOfEVSEs++;

                            const tariffDiv                 = tariffsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffDiv.className             = "tariff";

                            const tariffIdDiv               = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffIdDiv.className           = "id";
                            tariffIdDiv.innerHTML           = tariff.id;

                            if (tariff.tariff_alt_url) {
                                const altURLDiv             = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                altURLDiv.className         = "altURL";
                                altURLDiv.innerHTML         = "<a href=\"" + tariff.tariff_alt_url + "\">" + tariff.tariff_alt_url + "</a>";
                            }

                            const tariffAltTextsDiv         = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffAltTextsDiv.className     = "altTexts";

                            for (const tariff_alt_text_instance of tariff.tariff_alt_text) {

                                const altTextDiv                = tariffAltTextsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                altTextDiv.className            = "altText";
                                altTextDiv.innerHTML            = "(" + tariff_alt_text_instance.language + ") " + tariff_alt_text_instance.text;

                            }

                            if (tariff.energy_mix) {
                                const altURLDiv             = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                altURLDiv.className         = "altURL";
                                altURLDiv.innerHTML         = "<a href=\"" + tariff.tariff_alt_url + "\">" + tariff.tariff_alt_url + "</a>";
                            }

                            const tariffElementsDiv         = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffElementsDiv.className     = "tariffElements";


                            for (const tariffElement of tariff.elements) {

                                const tariffElementDiv               = tariffElementsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                tariffElementDiv.className           = "tariffElement";

                                const priceComponentsDiv             = tariffElementDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                priceComponentsDiv.className         = "priceComponents";

                                for (const priceComponent of tariffElement.price_components) {

                                    const priceComponentDiv              = priceComponentsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    priceComponentDiv.className          = "priceComponent";


                                    const priceComponentTypeDiv          = priceComponentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    priceComponentTypeDiv.className      = "type";
                                    priceComponentTypeDiv.innerHTML      = priceComponent.type.toString();

                                    const priceComponentPriceDiv         = priceComponentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    priceComponentPriceDiv.className     = "price";
                                    priceComponentPriceDiv.innerHTML     = priceComponent.price.toString() + " " + tariff.currency;

                                    if (priceComponent.type !== "FLAT") {
                                        const priceComponentStepSizeDiv = priceComponentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        priceComponentStepSizeDiv.className = "stepSize";
                                        priceComponentStepSizeDiv.innerHTML = priceComponent.step_size.toString();
                                    }

                                }


                                if (tariffElement.restrictions?.start_time   ||
                                    tariffElement.restrictions?.end_time     ||
                                    tariffElement.restrictions?.start_date   ||
                                    tariffElement.restrictions?.end_date     ||
                                    tariffElement.restrictions?.min_kwh      ||
                                    tariffElement.restrictions?.max_kwh      ||
                                    tariffElement.restrictions?.min_power    ||
                                    tariffElement.restrictions?.max_power    ||
                                    tariffElement.restrictions?.min_duration ||
                                    tariffElement.restrictions?.max_duration ||
                                    tariffElement.restrictions?.day_of_week) {

                                    const tariffRestrictionsDiv = tariffElementDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    tariffRestrictionsDiv.className = "tariffRestrictions";
                                    tariffRestrictionsDiv.innerHTML = "Restrictions";

                                    const restrictionsDiv = tariffRestrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    restrictionsDiv.className = "restrictions";


                                    if (tariffElement.restrictions?.start_time) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "Start Time";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.start_time;

                                    }

                                    if (tariffElement.restrictions?.end_time) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "End Time";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.end_time;

                                    }

                                    if (tariffElement.restrictions?.start_date) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "Start Date";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.start_date;

                                    }

                                    if (tariffElement.restrictions?.end_date) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "End Date";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.end_date;

                                    }

                                    if (tariffElement.restrictions?.min_kwh) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "min kWh";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.min_kwh.toString();

                                    }

                                    if (tariffElement.restrictions?.max_kwh) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "max kWh";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.max_kwh.toString();

                                    }

                                    if (tariffElement.restrictions?.min_power) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "min power";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.min_power.toString();

                                    }

                                    if (tariffElement.restrictions?.max_power) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "max power";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.max_power.toString();

                                    }

                                    if (tariffElement.restrictions?.min_duration) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "min duration";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.min_duration.toString();

                                    }

                                    if (tariffElement.restrictions?.max_duration) {

                                        const restrictionDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionDiv.className       = "restriction";

                                        const restrictionKeyDiv        = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionKeyDiv.className    = "key";
                                        restrictionKeyDiv.innerHTML    = "max duration";

                                        const restrictionValueDiv      = restrictionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionValueDiv.className  = "value";
                                        restrictionValueDiv.innerHTML  =  tariffElement.restrictions.max_duration.toString();

                                    }

                                    if (tariffElement.restrictions?.day_of_week) {

                                        const restrictionStartTimeDiv           = restrictionsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionStartTimeDiv.className       = "restriction";

                                        const restrictionStartTimeKeyDiv        = restrictionStartTimeDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionStartTimeKeyDiv.className    = "key";
                                        restrictionStartTimeKeyDiv.innerHTML    = "day of week";

                                        const restrictionStartTimeValueDiv      = restrictionStartTimeDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                        restrictionStartTimeValueDiv.className  = "value";

                                        for (const dayOfWeek of tariffElement.restrictions.day_of_week)
                                            restrictionStartTimeValueDiv.innerHTML  += " " + dayOfWeek;

                                    }

                                }

                            }


                            const tariffLastUpdatedDiv      = tariffDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            tariffLastUpdatedDiv.className  = "lastUpdated";
                            tariffLastUpdatedDiv.innerHTML  = "Last updated: " + tariff.last_updated;

                        }

                    }

                }
                catch (exception) {
                }

            },

            (status, statusText, response) => {
            }

    );

    //var refresh = setTimeout(StartDashboard, 30000);

}