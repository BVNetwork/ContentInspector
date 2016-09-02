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
                href: "/contentareainspector/" + that.model.contentLink,
                //   onDownloadEnd: dojo.hitch(this, this.initializeDialog, this._doSomethingCrazy(this))
                //   onDownloadEnd: this._doSomethingCrazy(this)

            });
            dialog.set('onDownloadEnd', function () {

                var arg = this;
                console.log(dialog.domNode);
              //  var nodelist = query(".inspector_preview_button", dialog.domNode);

                query(".inspector_preview_button", dialog.domNode).connect("onclick", function () {
                    var previewbtn = this;
                    var id = attr.get(this, "data-id");
                    var previewUrl = attr.get(this, "data-previewUrl");
                    // var item = that._store.get(5);
               //     var contentData = that._store.get(id);
                    //when(that._store.get(id), function (returnValue) {
                    //    console.log(id);
                    //    contentData = returnValue;
                    //    callback(contentData);
                    //});
                  //  console.log(contentData);
                    var canvas = document.createElement("canvas");
                    canvas.height = 400;
                    canvas.width = 500;
                    //   console.log('burg.3..');
                    var context = canvas.getContext("2d");
                    context.scale(0.5, 0.5);

                    rasterizeHtml.drawURL(previewUrl, canvas, { height: 800, width: 1000 });
                    // rasterizeHtml.drawHTML("<h1>ah</h1>", content);
                    //  console.log('raz');
                    console.log(previewbtn.id);
                    var previewTooltip = new TooltipDialog({
                        connectId: [dialog.id],
                        content: canvas
                    });
                    arg.own(previewTooltip);

                    popup.open({
                        popup: previewTooltip,
                        around: dom.byId(dialog.id),
                        orient: ["after-centered"]
                    });

                    //  alert("This button is hooked up!");
                });
                //   var previewbtn = nodelist[0];


                //    console.log(contentData);
                //if (isImage) {
                //    content = document.createElement("img");
                //    content.src = previewUrl;
                //    content.setAttribute("style", "max-width: 500px;");
                //} else {



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
