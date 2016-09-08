define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/promise/all",
    "epi/shell/command/_Command",
    "epi-cms/_ContentContextMixin",
    "contentinspector/InspectDialog"
], function (
    declare,
    lang,
    promiseAll,
    _Command,
    _ContentContextMixin,
    InspectDialog
) {

    return declare([_Command], {

        rasterizeBaseUrl: "",

        label: "Inspect current content",

        iconClass: "epi-iconSearch",

        canExecute: true,

        _execute: function () {
            var inspectDialog = new InspectDialog();
            inspectDialog.rasterizeBaseUrl = this.rasterizeBaseUrl;    
            inspectDialog._createTooltip(this.model.contentData);
        }
    });
});
