define([
    "dojo",
    "dojo/_base/declare",
    "dojo/_base/array",
    "epi/dependency",
    "epi/shell/command/_CommandProviderMixin",
    "epi/shell/ClipboardManager",
    "epi/shell/selection",
    "epi-cms/widget/ContentTreeStoreModel",
    "epi-cms/widget/ContentTreeModelConfirmation",
    "contentinspector/Commands/InspectCommand"
], function (
    dojo,
    declare,
    array,
    dependency,
    _CommandProviderMixin,
    ClipboardManager,
    Selection,
    ContentTreeStoreModel,
    ContentTreeModelConfirmation,
    InspectCommand
) {
    return declare([_CommandProviderMixin], {

        constructor: function (settings) {
            this.inherited(arguments);
            this.add("commands", new InspectCommand(settings));
        }
    });
});