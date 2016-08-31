define([
        "dojo/_base/array",
        "dojo/_base/connect",
        "dojo/_base/declare",
        "dojo/_base/lang",
        "dojo/query",
        "dojo/dom-class",
        "dojo/on",
        "dojo/dom",
        "dijit/popup",
        "dijit/TooltipDialog",
        "epi/epi",
        "epi/dependency",
        "epi-cms/contentediting/editors/_ContentAreaTree"
],
    function (
        array,
        connect,
        declare,
        lang,
        query,
        domClass,
        on,
        dom,
        popup,
        TooltipDialog,
        epi,
        dependency,
        _ContentAreaTree
    ) {
        return declare("contentareainspector.editors._ContentAreaTreeWithPreview", [_ContentAreaTree], {

            /* ***** ***** ***** ***** ***** */
            /* overriden method              */
            /* ***** ***** ***** ***** ***** */
            _createTreeNode: function (model) {
                var node = this.inherited(arguments);;
                lang.hitch(this, this._modifyNode(node, model));
                return node;
            },

            _modifyNode: function (node, model) {
                // locate default icon
               
                var imgNode = query("img.dijitIcon", node.domNode);
                
                if (imgNode == null || imgNode.length == 0) {
                    return;
                }

                var spanLabel = query("span.dijitTreeLabel", node.domNode);
                if (spanLabel == null || spanLabel.length == 0) {
                    return;
                }

                this._resolveContentData(model.item.contentLink, lang.hitch(this, function (content) {
                    if (content.thumbnailUrl) {
                          
                    // setup image
                    imgNode = imgNode[0];
                    domClass.add(imgNode, "epi-thumbnail");
                    domClass.remove(imgNode, "epi-iconObjectImage");
                    domClass.remove(imgNode, "dijitTreeIcon");
                    imgNode.src = content.thumbnailUrl;

                    // setup span
                    spanLabel = spanLabel[0];
                    domClass.remove(spanLabel, "dijitTreeLabel");
                    domClass.add(spanLabel, "dojoxEllipsis");
                    }
                    // preview on dblclick 
                    //on(imgNode, 'dblclick', function () {
                    //    window.open(content.previewUrl, '_blank');
                    //});

                    this._createTooltip(node, imgNode , content);
                }));
            },

            _resolveContentDataLight: function (contentlink, callback) {
                var registry = dependency.resolve("epi.storeregistry");
                var store = registry.get("epi.cms.content.light");
                if (!contentlink) {
                    return null;
                }

                var contentData;
                dojo.when(store.get(contentlink), function (returnValue) {
                    contentData = returnValue;
                    callback(contentData);
                });
                return contentData;
            },
            _resolveContentData: function (contentlink, callback) {
                var registry = dependency.resolve("epi.storeregistry");
                var store = registry.get("epi.cms.contentdata");
                if (!contentlink) {
                    return null;
                }

                var contentData;
                dojo.when(store.get(contentlink), function (returnValue) {
                    contentData = returnValue;
                    callback(contentData);
                });
                return contentData;
            },

            _createTooltip: function (node, imgNode, content) {
                var createHtml = function () {

                    var result = null;
                    $.ajax({
                        type: "GET",
                        async: false,
                        url: "/contentareainspector/" + content.contentLink,
                        success: function (data) {
                            result = data;
                        }
                    });
                 return result;
                };
               
                node.imageTooltip = new TooltipDialog({
                    connectId: [node.id],
                   // content:'<a id="close-dialog" href="#">Close</a>' +  createHtml(),
                  //  content:createHtml(),
                    //onShow : function() {
                    //    dojo.connect(dojo.byId('close-dialog'),
                    //        "onclick",
                    //        function(evt) {
                    //            dojo.stopEvent(evt);
                    //            dijit.popup.close(node.imageTooltip);
                    //        });
                    //},
                    onMouseLeave: function () {
                         //   popup.close(node.imageTooltip);
                    }
                });

             
                on(imgNode, 'click', function () {
                    node.imageTooltip.set("content", createHtml()),
                    popup.open({
                        popup: node.imageTooltip,
                      //  content: createHtml(),
                        //around: dom.byId(node.id),
                        //orient: ["above-centered"],
                        around: dom.byId(node.id),
                        orient: ["after-centered"],
                        onCancel: function () { popup.close(node.imageTooltip); }
                    });
                });
            }
        });
    });