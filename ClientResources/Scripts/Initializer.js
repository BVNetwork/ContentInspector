define([
    "dojo/_base/declare",
    "epi/_Module",'epi/dependency',
    "contentinspector/InspectorContentAreaCommandProvider",
    "contentinspector/InspectorCommandProvider"
], function (declare, _Module,dependency,ContentAreaCommands, InspectorCommandProvider) {

    return declare([_Module],
    {
        initialize: function () {
            this.inherited(arguments);
            // summary:
            //      Initializes the favorite module.
            // tags:
            //      public
            var commandregistry = dependency.resolve("epi.globalcommandregistry");
            commandregistry.registerProvider('epi.cms.contentdetailsmenu', new InspectorCommandProvider());
        }

    });

});