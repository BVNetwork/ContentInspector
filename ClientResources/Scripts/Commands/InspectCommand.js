define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/promise/all",
    "epi/shell/command/_Command",
    "epi-cms/_ContentContextMixin",
    "contentareainspector/InspectDialog"
], function (
    declare,
    lang,
    promiseAll,
    _Command,
    _ContentContextMixin,
    InspectDialog
) {

    return declare([_Command], {

        label: "Inspect current content",

        iconClass: "epi-iconSearch",

        canExecute: true,

        _execute: function () {
            new InspectDialog()._createTooltip(this.model.contentData);
        }
    });
});
