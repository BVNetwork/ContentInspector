define([
    "dojo/_base/lang",
    "epi-cms/contentediting/command/ContentAreaCommands",
    "epi-cms/contentediting/editors/ContentAreaEditor",
    "contentareainspector/Commands/Inspect"
], function (
    lang,
    ContentAreaCommands,
    ContentAreaEditor,
    Inspect
) {

    // Override content area commands for on-page editing
    var inspectCommand = new Inspect();
    var originalMethodContentAreaCommands = ContentAreaCommands.prototype.postscript;
    lang.mixin(ContentAreaCommands.prototype, {
        postscript: function () {
            originalMethodContentAreaCommands.call(this);
            this.commands[0].category = null;
            this.commands.splice(1, 0, inspectCommand);
        }
    });

    // override content area editor for All Properties mode
    ContentAreaCommands.prototype.postscript.nom = "postscript";
    var originalMethodContentAreaEditor = ContentAreaEditor.prototype.postMixInProperties;
    lang.mixin(ContentAreaEditor.prototype, {
        postMixInProperties: function () {

            originalMethodContentAreaEditor.apply(this);
            this.commands[0].category = null;
            this.commands.splice(1, 0, inspectCommand);
        }
    });
    ContentAreaEditor.prototype.postMixInProperties.nom = "postMixInProperties";
});