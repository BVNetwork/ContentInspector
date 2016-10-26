define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/topic",
    "dojo/when",
    "epi",
    "epi/dependency",
    "epi-cms/contentediting/command/_ContentAreaCommand",
    "epi-cms/contentediting/ContentActionSupport",
    "contentinspector/InspectDialog"
], function (declare, lang,  topic, when, epi, dependency, _ContentAreaCommand, ContentActionSupport, InspectDialog) {

    return declare([_ContentAreaCommand], {
        name: "Inspect",
        label: "Inspect",
        tooltip: "Inspect",

        // iconClass: [readonly] String
        //      The icon class of the command to be used in visual elements.
        iconClass: "epi-iconSearch",

        inspectDialog: null,

        canExecute: true,

        // category: [readonly] String
        //      A category which hints that this item should be displayed with a separator.
        category: "menuWithSeparator",

        constructor: function (settings) {
            this.contentActionSupport = this.contentActionSupport || ContentActionSupport;
            this.inspectDialog = new InspectDialog();
            this.inspectDialog.rasterizeBaseUrl = settings.rasterizeBaseUrl;
        },

        _execute: function () {          
            var content = this.model;
            this.inspectDialog._createTooltip(content);
        },

        _onModelChange: function () {
            this.set("canExecute", this.model.contentLink != null);
        }
    });
});
