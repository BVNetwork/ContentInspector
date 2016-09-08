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
    "epi-cms/command/DeleteContent",
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
    DeleteContent,
    Inspect
) {
    return declare([_CommandProviderMixin], {

        constructor: function () {
            this.inherited(arguments);

            this._clipboardManager = new ClipboardManager();
            this._selection = new Selection();

            this._settings = {
                model: new ContentTreeModelConfirmation(new ContentTreeStoreModel()),
                clipboard: this._clipboardManager,
                selection: this._selection
            };

            this._deleteCommand = new DeleteContent(this._settings);
            this.add("commands", new Inspect(this._settings));
        }

        //updateCommandModel: function (content) {

        //    //var selected = [{ type: "epi.cms.contentdata", data: content.contentData }];
        //    //this._selection.set("data", selected);
        //    this._deleteCommand.set("isAvailable", true);
        //}
    });
});