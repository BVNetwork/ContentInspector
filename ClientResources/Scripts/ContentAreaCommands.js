define([
    "dojo/_base/lang",
    "epi-cms/contentediting/command/ContentAreaCommands",
    "contentareainspector/Commands/Inspect"
], function (
    lang,
    ContentAreaCommands,
    Inspect
) {
    var originalMethod = ContentAreaCommands.prototype.postscript;

    lang.mixin(ContentAreaCommands.prototype, {

        postscript: function () {
            originalMethod.call(this);
            var inspectCommand = new Inspect();
            //this.commands.push(inspectCommand);
            this.commands[0].category = null;
            this.commands.splice(1, 0, inspectCommand);
        }
    });
    ContentAreaCommands.prototype.postscript.nom = "postscript";
});