define([
    // General application modules
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/topic",
    "dojo/when",
    "dijit/Dialog",
    "epi",
    "epi/dependency",
    // Parent class
    "epi-cms/contentediting/command/_ContentAreaCommand",
    "epi-cms/contentediting/ContentActionSupport",
    "epi/i18n!epi/cms/nls/episerver.shared.action"
], function (declare, lang, topic, when, Dialog, epi, dependency, _ContentAreaCommand, ContentActionSupport, actionStrings) {

    return declare([_ContentAreaCommand], {
        name: "Inspect",
        label: "Inspect",
        tooltip: "Inspect",

        // iconClass: [readonly] String
        //      The icon class of the command to be used in visual elements.
        iconClass: "epi-iconSearch",

        // category: [readonly] String
        //      A category which hints that this item should be displayed with a separator.
        category: "menuWithSeparator",

        constructor: function () {
            this.contentActionSupport = this.contentActionSupport || ContentActionSupport;
            var registry = dependency.resolve("epi.storeregistry");
            this._store = registry.get("epi.cms.content.light");
        },

        _execute: function () {
            this._createTooltip();
        },

        _createTooltip: function () {
            var that = this;
            var dialog = new Dialog({
                title: that.model.name,
                style: "min-width:400px;",
                class:"inspector_dialog",
                
                href: "/contentareainspector/" + that.model.contentLink
            });
            dialog.show();
        },

        _onModelValueChange: function () {
            // summary:
            //      Updates canExecute after the model value has changed.
            // tags:
            //      protected
            var item = this.model;

            if (!item || !item.contentLink) {
                this.set("canExecute", false);
                return;
            }

            var result = item && this._store.get(item.contentLink);

            // if the accessMask is available then display the label accordingly. (i.e either "View" or "Edit")
            when(result, lang.hitch(this, function (content) {
                this.set("canExecute", content && !content.isDeleted);
                //if (content && content.accessMask) {
                //    if (this.contentActionSupport.hasAccess(content.accessMask, this.contentActionSupport.accessLevel[this.contentActionSupport.action.Edit])) {
                //        this.set("label", "Inspect");
                //        this.set("iconClass", "epi-iconPen");
                //    } else {
                //        this.set("label", actionStrings.view);
                //        this.set("iconClass", "epi-iconSearch");
                //    }
                //}
            }));
        }
    });
});
