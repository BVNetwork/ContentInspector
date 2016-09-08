define([
    "dojo/_base/declare",
     "dojo/_base/lang",
    "epi/_Module",'epi/dependency',
    //"contentinspector/InspectorContentAreaCommandProvider",
    "contentinspector/InspectorCommandProvider",
       "epi-cms/contentediting/command/ContentAreaCommands",
   "epi-cms/contentediting/editors/ContentAreaEditor",
   "contentinspector/Commands/InspectContentAreaCommand"

], function (declare, lang, _Module, dependency, InspectorCommandProvider, ContentAreaCommands, ContentAreaEditor, InspectContentAreaCommand) {

    return declare([_Module],
    {

        rasterizeBaseUrl: "",
        settings:[],

        constructor: function (settings) {
            this.settings = settings;
            this.inherited(arguments);     
            this.rasterizeBaseUrl = settings.rasterizeBaseUrl;
        },

        initialize: function () {
            this.inherited(arguments);
            var commandregistry = dependency.resolve("epi.globalcommandregistry");
            commandregistry.registerProvider('epi.cms.contentdetailsmenu', new InspectorCommandProvider(this.settings));

            // Override content area commands for on-page editing
            var inspectCommand = new InspectContentAreaCommand(this.settings);
            inspectCommand.rasterizeBaseUrl = this.rasterizeBaseUrl;
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

        }



    });

});