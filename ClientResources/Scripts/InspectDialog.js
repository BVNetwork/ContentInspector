define([
    "dojo/_base/declare",
    "dojo/promise/all",
    "dojo/topic",
    "dojo/when",
    "dojo/on",
     "dojo/dom",
     "dojo/dom-attr",
    "dijit/popup",
     "dojo/query",
    "dijit/Dialog",
     "dijit/TooltipDialog",
    "contentinspector/rasterizeHTML.allinone",
    "epi-cms/ApplicationSettings"
], function (
    declare,
    promiseAll,
    topic,
    when,on, dom, attr, popup, query, Dialog, TooltipDialog,
    rasterizeHtml, ApplicationSettings
) {
    return declare([], {

        rasterizeBaseUrl:"",

        _createTooltip: function (model) {
            var self = this;
            var dialog = new Dialog({
                title: model.name,
                style: "min-width:600px;",
                class: "inspector_dialog",
                href: "/ContentInspector/" + model.contentLink
            });

            dialog.set('onDownloadEnd', function () {
                query(".inspector_preview_button", dialog.domNode).connect("onclick", function () {
                    var previewbtn = this;
                    if (!dojo.hasClass(previewbtn, "epi-icon--active")) {
                        dojo.addClass(previewbtn, "epi-icon--active");
                        var previewUrl = attr.get(this, "data-previewUrl");
                        var isImage = attr.get(this, "data-type") === "Image";
                        var loaderdiv = document.createElement("div");
                        loaderdiv.innerHTML = "<span class=\"dijitContentPaneLoading\"><span class=\"dijitInline dijitIconLoading\"></span>Loading preview...</span>";
                        loaderdiv.className = "inspector_preview_loader";

                        var previewTooltip = new TooltipDialog({
                            connectId: [dialog.id],
                            content: loaderdiv,
                            onHide: function () {
                                dojo.removeClass(previewbtn, "epi-icon--active");
                            }
                        });
                        dialog.own(previewTooltip);
                        popup.open({
                            popup: previewTooltip,
                            around: dom.byId(dialog.id),
                            orient: ["after-centered"]
                        });
                        var previewContent;

                        if (isImage) {
                            previewContent = document.createElement("img");
                            previewContent.src = previewUrl;
                            previewContent.setAttribute("style", "max-width: 500px;");
                            previewTooltip.set("content", previewContent);
                        }
                        else {
                            previewContent = document.createElement("canvas");
                            previewContent.height = 400;
                            previewContent.width = 500;
                            var context = previewContent.getContext("2d");
                            context.scale(0.5, 0.5);
                            rasterizeHtml.drawURL(previewUrl, previewContent, { height: 800, width: 1000, baseUrl: self.rasterizeBaseUrl }).then(function success(renderResult) {
                                previewTooltip.set("content", previewContent);
                            });
                        }
                        on(previewbtn, "click", function () {
                            popup.close(previewTooltip);
                        });
                        dialog.connect(dialog, "hide", function (e) {
                            popup.close(previewTooltip);
                        });
                        previewTooltip.connect(previewTooltip, "hide", function (e) {
                            previewbtn.destroy();
                        });
                    }
                });
            });
            dialog.show();
        }
    });
});
