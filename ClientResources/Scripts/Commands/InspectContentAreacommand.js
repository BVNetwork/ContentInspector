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

        constructor: function () {
            this.contentActionSupport = this.contentActionSupport || ContentActionSupport;
            this.inspectDialog = new InspectDialog();
        },

        _execute: function () {
            var content = this.model;
            this.inspectDialog._createTooltip(content);
        }
    });
});
