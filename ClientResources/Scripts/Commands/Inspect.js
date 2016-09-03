define([
    // General application modules
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/on",
    "dojo/topic",
    "dojo/when",
     "dojo/dom",
     "dojo/dom-attr",
    "dijit/popup",
     "dojo/query",
    "dijit/Dialog",
     "dijit/TooltipDialog",

    "epi",
    "epi/dependency",
    // Parent class
    "epi-cms/contentediting/command/_ContentAreaCommand",
    "epi-cms/contentediting/ContentActionSupport",
    "epi/i18n!epi/cms/nls/episerver.shared.action",
    "contentareainspector/rasterizeHTML.allinone"
], function (declare, lang, on, topic, when, dom, attr, popup, query, Dialog, TooltipDialog, epi, dependency, _ContentAreaCommand, ContentActionSupport, actionStrings, rasterizeHtml) {

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
                style: "min-width:600px;",
                class: "inspector_dialog",
                href: "/contentareainspector/" + that.model.contentLink
            });
            dialog.set('onDownloadEnd', function () {
                query(".inspector_preview_button", dialog.domNode).connect("onclick", function () {
                    var previewbtn = this;
                    var checked = attr.get(this, "data-checked");

                    if (checked === false) {
                        attr.set(this, "data-checked", true);
                    }
                    else{
                        attr.set(this, "data-checked", false);
                        var previewUrl = attr.get(this, "data-previewUrl");
                      
                        var loaderdiv = document.createElement("div");
                        loaderdiv.innerHTML = "<span class=\"dijitContentPaneLoading\"><span class=\"dijitInline dijitIconLoading\"></span>Loading preview...</span>";
                        loaderdiv.className = "inspector_preview_loader";
                       
                        var previewTooltip = new TooltipDialog({
                            connectId: [dialog.id],
                            content: loaderdiv
                        });
                        dialog.own(previewTooltip);
                        popup.open({
                            popup: previewTooltip,
                            around: dom.byId(dialog.id),
                            orient: ["after-centered"]
                        });
                        var canvas = document.createElement("canvas");
                        canvas.height = 400;
                        canvas.width = 500;
                        var context = canvas.getContext("2d");
                        context.scale(0.5, 0.5);
                        rasterizeHtml.drawURL(previewUrl, canvas, { height: 800, width: 1000 }).then(function success(renderResult) {
                            previewTooltip.set("content", canvas);
                        });
                        on(previewbtn, "click", function () {
                            popup.close(previewTooltip);
                        });
                        dialog.connect(dialog, "hide", function (e) {
                            popup.close(previewTooltip);
                        });
                    }
                });
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
