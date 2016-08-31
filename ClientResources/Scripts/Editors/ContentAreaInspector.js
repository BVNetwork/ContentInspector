define([
        "dojo/_base/declare",
        "dojo/_base/lang",
        "dojo/aspect",
        "epi-cms/contentediting/editors/ContentAreaEditor",
        "./_ContentAreaTreeInspector"
],
    function (
        declare,
        lang,
        aspect,
        _ContentAreaEditor,
        _ContentAreaTreeInspector
    ) {
        return declare("contentareainspector.editors.contentAreaInspector", [_ContentAreaEditor], {

            /* ***** ***** ***** ***** ***** */
            /* overriden method              */
            /* ***** ***** ***** ***** ***** */
            _createTree: function () {
                this.tree = new _ContentAreaTreeInspector({
                    accept: this.allowedDndTypes,
                    reject: this.restrictedDndTypes,
                    contextMenu: this.contextMenu,
                    model: this.treeModel,
                    readOnly: this.readOnly
                }).placeAt(this.domNode, "first");

                this.tree.own(
                    aspect.after(this.tree.dndController, "onDndEnd", lang.hitch(this, "focus"))
                );
            }
        });
    });