// Specter panels

// -- region Layer panel
L.Control.LayersControl = L.Control.extend({
    includes: L.Mixin.Events,
    statics: {
        baseClass: 'layer-panel'
    },
    options: {
        collapsed: true,
        position: 'topright',
        autoZIndex: true
    },
    initialize: function (baseLayers, defaultLayer, options) {
        var i, j;

        L.setOptions(this, options);
        if (typeof baseLayers == 'string') {
            var result = this._getBaseLayers(baseLayers);
            baseLayers = result.baseLayers;
            defaultLayer = result.defaultLayer;
        }

        this._layers = {};
        this._lastZIndex = 0;
        this._handlingClick = false;
        this._groupList = [];
        this._currentId = 0;
        this._defaultLayer = defaultLayer;
        this._lastTouchItem = null;

        for (i = 0; i < baseLayers.length; i++) {
            for (j = 0; j < baseLayers[i].Layers.length; j++) {
                this._addLayer(baseLayers[i].Layers[j], baseLayers[i], false, baseLayers[i].Layers[j].Image);
            }
        }
    },
    onAdd: function (map) {
        this._map = map;
        this._initLayout();
        this._update();

        var control = L.control.attribution({
            position: 'bottomright',
            prefix: ''
        }).addTo(map);
        control._container.style.background = 'none';
        this._attributionVendor = attributionVendor = L.DomUtil.create('div', 'attribution-vendor-maps', control._container);
        this._logoVendor = L.DomUtil.create('div', 'logo-vendor-maps', attributionVendor);
        this._infoVendor = L.DomUtil.create('div', 'info-vendor-maps', attributionVendor);

        map
		    .on('layeradd', this._onLayerChange, this)
		    .on('layerremove', this._onLayerChange, this)
            .on('maploaded', this._onLayerChange, this)
            .on('vendorattributionready', this._addAttribution, this);

        return this._container;
    },
    onRemove: function (map) {
        map
		    .off('layeradd', this._onLayerChange, this)
		    .off('layerremove', this._onLayerChange, this);
    },
    addBaseLayer: function (layer, name) {
        this._addLayer(layer, name);
        this._update();
        return this;
    },
    addOverlay: function (layer, name) {
        this._addLayer(layer, name, true);
        this._update();
        return this;
    },
    removeLayer: function (layer) {
        var id = L.stamp(layer);
        delete this._layers[id];
        this._update();
        return this;
    },
    currentLayer: function () { return this._layers[this._currentId]; },
    _getBaseLayers: function (url) {
        var maps = $.ajax({
            type: "POST",
            url: "/Map/GetMapLayers",
            data: {},
            async: false,
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            success: function (data) {
                baseLayers = data.baseLayers;
                defaultLayer = data.defaultLayer;
                return baseLayers, defaultLayer;
            },
            error: function (xhr) { }
        });
        return { baseLayers: baseLayers, defaultLayer: defaultLayer };
    },
    _initLayout: function () {
        var className = 'leaflet-control-layers',
		    container = this._container = L.DomUtil.create('div', className);

        //L.DomUtil.addClass(container, 'leaflet-control-layers-expanded');
        //Makes this work on IE10 Touch devices by stopping it from firing a mouseout event when the touch is released
        container.setAttribute('aria-haspopup', true);
        container.style.padding = '0';

        if (!L.Browser.touch) {
            L.DomEvent
				.disableClickPropagation(container)
				.disableScrollPropagation(container);
        } else {
            L.DomEvent.on(container, 'click', L.DomEvent.stopPropagation);
        }

        var form = this._form = L.DomUtil.create('form', className + '-list');

        if (this.options.collapsed) {
            var link = this._layersLink = L.DomUtil.create('a', className + '-toggle', container);
            link.href = '#';
            link.title = 'Layers';

            if (!L.Browser.android) {
                L.DomEvent
                    .on(container, 'mouseover', this._expand, this)
                    .on(container, 'mouseout', this._collapse, this);
            }

            if (L.Browser.touch) {
                L.DomEvent
				    .on(link, 'click', L.DomEvent.stop)
				    .on(link, 'click', this._expand, this);
            }
            else {
                L.DomEvent.on(link, 'focus', this._expand, this);
            }
            //Work around for Firefox android issue https://github.com/Leaflet/Leaflet/issues/2033
            //L.DomEvent.on(form, 'click', function () {
            //    setTimeout(L.bind(this._onMouseMenuItem/*this._onClick*/, this), 0);
            //}, this);

            this._map.on('click', this._collapse, this);
            // TODO keyboard accessibility
        } else {
            this._expand();
        }

        this._baseLayersList = L.DomUtil.create('div', className + '-base', form);
        this._separator = L.DomUtil.create('div', className + '-separator', form);
        this._overlaysList = L.DomUtil.create('div', className + '-overlays', form);

        container.appendChild(form);
    },
    _addLayer: function (layerObj, group, overlay, image) {
        var i, j, layer,
            options = {}; //{ attribution: "&copy; <a rel='nofollow' href='http://specter.ddns.net:8095'><img src='/content/images/logo-big.png' style='height: 20px;'/></a>" };

        var map = group.GroupName.slice(0, group.GroupName.indexOf(' ')).toLowerCase();
        var type = layerObj.Type;
        switch (map) {
            case "yandex":
                layer = new L.Yandex(type, options);
                break;
            case "google":
                layer = new L.Google(type, options);
                break;
            case "osm":
                layer = new L.TileLayer("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
                    attribution: options.attribution + '<img alt="OpenStreetMap logo" class="logo" src="http://www.openstreetmap.org/assets/osm_logo-7ff517e28cb4213a8bda5c17423f8d44.png"> Map tiles by <a href="http://stamen.com">Stamen Design</a>, under <a href="http://creativecommons.org/licenses/by/3.0">CC BY 3.0</a>. Data by <a href="http://openstreetmap.org">OpenStreetMap</a>, under <a href="http://creativecommons.org/licenses/by-sa/3.0">CC BY SA</a>.'
                });
                break;
            case "2gis":
                //layer = new L.DGis2(options);
                layer = new L.DGis(options); // new L.TileLayer("//tile1.maps.2gis.com/tiles?x={x}&y={y}&z={z}&v=1", {
                break;
        }

        var id = L.stamp(layer);

        this._layers[id] = {
            layer: layer,
            name: layerObj.Name,
            overlay: overlay,
            image: layerObj.Image
        };

        if (group) {
            var groupId = this._groupList.indexOf(group);

            // if not find the group search for the name
            if (groupId === -1) {
                for (g in this._groupList) {
                    if (this._groupList[g].name == group.GroupName) {
                        groupId = g;
                        break;
                    }
                }
            }

            if (groupId === -1) {
                groupId = this._groupList.length;
                this._groupList[groupId] = {
                    name: group.GroupName,
                    expanded: group.Expanded,
                    layers: []
                }
                //groupId = this._groupList.push(group) - 1;
                //this._groupList[groupId].created = false;
            }
            this._groupList[groupId].layers.push(layer);

            this._layers[id].group = {
                name: group.GroupName,
                id: groupId,
                expanded: group.Expanded
            };
        }

        if (this.options.autoZIndex && layer.setZIndex) {
            this._lastZIndex++;
            layer.setZIndex(this._lastZIndex);
        }
    },
    _update: function () {
        if (!this._container) {
            return;
        }

        this._baseLayersList.innerHTML = '';
        this._overlaysList.innerHTML = '';

        var baseLayersPresent = false,
		    overlaysPresent = false,
		    i, obj;

        this._baseLayersMenu = L.DomUtil.create('ul', L.Control.LayersControl.baseClass, this._baseLayersList);
        for (i = 0; i < this._groupList.length; i++) {
            obj = this._groupList[i];
            this._addItem(obj);
            overlaysPresent = overlaysPresent || obj.overlay;
            baseLayersPresent = baseLayersPresent || !obj.overlay;
        }

        this._separator.style.display = overlaysPresent && baseLayersPresent ? '' : 'none';
    },
    _onLayerChange: function (e) {
        //if (!e.layer || e.type == 'layerremove' || (layer = this._layers[e.layer._leaflet_id]) == undefined) { return; }

        //var attributions = [], i, timer,
        //    controlAttribution = e.target._controlCorners.bottomright.querySelector('div.leaflet-control-attribution'),
        //    layerName = layer.group.name.match(/(yandex|google|osm|2gis)/i);

        //if (layerName.length <= 0) { return; }

        //layer = e.layer._container;
        ////controlAttribution.innerHTML = '';

        //switch (layerName[0].toLowerCase()) {
        //    case 'yandex':
        //        this._map.on('vendorattributionready', function (e) {
        //            this._addAttribution(e);
        //        });
        //        break;
        //    case 'google':
        //        this._map.on('vendorattributionready', this._addAttribution, this);
        //        break;
        //    case 'osm':
        //        break;
        //    case '2gis':
        //        attributions.push(layer.querySelector('.DGControlsCopyright'));
        //        break;
        //    default:
        //        return;
        //}

        //var obj = this._layers[L.stamp(e.layer)];

        //if (!obj) { return; }

        //if (!this._handlingClick) {
        //    this._update();
        //}

        //var type = obj.overlay ?
        //	(e.type === 'layeradd' ? 'overlayadd' : 'overlayremove') :
        //	(e.type === 'layeradd' ? 'baselayerchange' : null);

        //if (type) {
        //    this._map.fire(type, obj);
        //}
    },
    // IE7 bugs out if you create a radio dynamically, so you have to do it this hacky way (see http://bit.ly/PqYLBe)
    _createRadioElement: function (name, checked) {

        var radioHtml = '<input type="radio" class="leaflet-control-layers-selector" name="' + name + '"';
        if (checked) {
            radioHtml += ' checked="checked"';
        }
        radioHtml += '/>';

        var radioFragment = document.createElement('div');
        radioFragment.innerHTML = radioHtml;

        return radioFragment.firstChild;
    },
    _addAttribution: function (attribution) {
        this._logoVendor.innerHTML = this._infoVendor.innerHTML = '';

        if (!isEmpty(attribution.logo)) {
            this._logoVendor.appendChild(attribution.logo);
        }
        for (i = 0; i < attribution.lines.length; i++) {
            this._infoVendor.appendChild(attribution.lines[i]);
        }
    },
    _addSubItems: function (obj, group) {
        var item = document.createElement('ul'),
            name, icon, i;
        item.setAttribute('class', 'leaflet-bar');

        for (i = 0; i < obj.length; i++) {
            subItem = L.DomUtil.create('li', 'layer-item');
            //wrap = L.DomUtil.create('span', 'layer-menu-subitem', subItem); // document.createElement('span');
            name = document.createElement('div');
            name.innerHTML = this._layers[obj[i]._leaflet_id].name;
            if (this._layers[obj[i]._leaflet_id].image) {
                image = document.createElement('img');
                image.setAttribute('src', this._layers[obj[i]._leaflet_id].image);
                subItem.appendChild(image);
                L.DomUtil.addClass(subItem, 'leaflet-bar');
            } else {
                item.style.paddingBottom = '0';
                subItem.className += ' no-image';
                //name.setAttribute('class', 'ame-without-image');
            }

            L.DomEvent
                .on(subItem, 'click', this._onClick, this);
            //.off(subItem, 'mouseover')
            //.off(subItem, 'mouseout');

            subItem.appendChild(name);
            subItem.layerId = L.stamp(obj[i]);
            if (this._layers[obj[i]._leaflet_id].group.name == this._defaultLayer.group && this._layers[obj[i]._leaflet_id].name == this._defaultLayer.name) {
                L.DomUtil.addClass(subItem, 'actived');
                this._currentId = subItem.layerId;
            }
            item.appendChild(subItem);
        }
        return item;
    },
    _addItem: function (obj) {
        var label = document.createElement('label'),
            item = document.createElement('li'),
		    input, name, i

        if (obj.overlay) {
            input = document.createElement('input');
            input.type = 'checkbox';
            input.className = 'leaflet-control-layers-selector';
            //input.defaultChecked = checked;
        } else {
            if (obj.name == undefined) { return; }

            L.DomEvent
                .stopPropagation(item)
                .disableClickPropagation(item)
                .disableScrollPropagation(item);

            if (obj.expanded) {
                item.innerHTML = '<div>' + obj.name + '</div>';
                item.appendChild(this._addSubItems(obj.layers));
            } else {
                L.DomUtil.addClass(item, 'layer-item')
                name = document.createElement('div');
                name.innerHTML = obj.name;
                L.DomEvent.on(item, 'click', this._onClick, this);
                var layer = {};
                for (i in obj.layers) {
                    layer = obj.layers[i];
                    break;
                }
                item.layerId = L.stamp(layer);
                if (obj.name == this._defaultLayer.group && this._layers[layer._leaflet_id].name == this._defaultLayer.name) {
                    L.DomUtil.addClass(item, 'actived');
                    this._currentId = item.layerId;
                }
                item.appendChild(name);
            }
        }

        var container = obj.overlay ? this._overlaysList : this._baseLayersMenu;

        container.appendChild(item);

        return item;
    },
    _onInputClick: function () {
        var i, input, obj,
		    inputs = this._form.getElementsByTagName('input'),
		    inputsLen = inputs.length;

        this._handlingClick = true;

        for (i = 0; i < inputsLen; i++) {
            input = inputs[i];
            obj = this._layers[input.layerId];

            if (input.checked && !this._map.hasLayer(obj.layer)) {
                this._map.addLayer(obj.layer);

            } else if (!input.checked && this._map.hasLayer(obj.layer)) {
                this._map.removeLayer(obj.layer);
            }
        }

        this._handlingClick = false;

        this._refocusOnMap();
    },
    _onClick: function (e) {
        var i, input, obj,
            actived = this._form.querySelector('.layer-item.actived'),
            activedParent = $(actived).parents('li'),
            layer = this.currentLayer().layer,
            item = e.currentTarget,
            obj = this._layers[item.layerId],
            parent = $(item).parents('li');

        if (L.Browser.touch) {
            this._collapse(this);
        }

        this._handlingClick = true;

        if (!this._map.hasLayer(obj.layer)) {
            //L.DomUtil.removeClass(actived, 'actived');
            this._map.removeLayer(layer);
            layer = obj.layer;
            var t = this._map.addLayer(layer);
            this._currentId = layer._leaflet_id;
            //L.DomUtil.addClass(item, 'actived');
            //if (parent.length > 0) {
            //    L.DomUtil.addClass(parent[0], 'actived');
            //}
        }

        this._handlingClick = false;

        this._refocusOnMap();
    },
    _expand: function (e) {
        //this._container.style.background = 'none';
        //this._container.boxShadow = 'none';
        var icon = document.getElementsByClassName('leaflet-control-layers-toggle');
        if (icon[0]) { icon[0].style.display = 'none'; }
        var form = document.getElementsByClassName('leaflet-control-layers-list')[0];
        if (form) { form.style.display = 'block'; }
    },
    _collapse: function (e) {
        var icon = document.getElementsByClassName('leaflet-control-layers-toggle');
        if (icon[0]) { icon[0].style.display = 'block'; }
        var form = document.getElementsByClassName('leaflet-control-layers-list')[0];
        if (form) { form.style.display = 'none'; }
        //this._container.style.background = '';
        //this._container.boxShadow = '';
    },
});
L.Control.layersControl = function (baseLayers, overlays, options) {
    return new L.Control.LayersControl(baseLayers, overlays, options);
};
// -- end Layer panel

// --- region Panel ---
L.Panel = (L.Layer || L.Class).extend({
    statics: {
        baseClass: 'specter-panel'
    },
    includes: L.Mixin.Events,
    options: {
        className: '',
        orientation: 'portrait', //landscape
        title: {
            text: '',
            classes: '',
            actions: false
        },
        position: 'left',
        tooltip: false,
        fitToPage: false,
        state: 'collapsed',
        buttonExpand: true,
        filter: function () { return true; },
        actions: [],
        scrollbar: {
            handler: false,
            options: {}
        },
        templateItem: '<div></div>'
    },
    initialize: function (options) {
        this.options = $.extend(true, {}, this.options, options);

        var baseClass = L.Panel.baseClass,
            className = this.options.className,
            options = this.options;

        this._currentItem = null;

        this._listeners = {};
        this._actions = [];
        this._tooltips = [];

        // --- Container ---
        this.container = container = L.DomUtil.create('div', baseClass + "-" + options.orientation);
        L.DomUtil.addClass(container, options.state + ' leaflet-bar');
        L.DomUtil.addClass(container, className);
        L.DomUtil.addClass(container, this.options.position);
        if (!L.Browser.touch) {
            L.DomEvent
                .disableClickPropagation(container)
                .disableScrollPropagation(container);
        }
        // ??? to check
        if (options.state == 'collapsed') {
            this.container.style.display = 'none';
        }
        // --- end Container ---
        // --- Title ---
        if (options.title) {
            this.title = title = L.DomUtil.create('div', className + '-title', container);
            L.DomUtil.addClass(title, options.title.classes);
            if (options.title.text && options.title.text.length > 0) {
                title.innerHTML = this.options.title.text;
            }
            if (options.title.actions) {
                for (i in options.title.actions) {
                    var action = options.title.actions[i];
                    switch (action) {
                        case 'expand-collapse':
                            item = L.DomUtil.create('span', 'icon-' + action, this.title);
                            L.DomEvent.on(title, 'click', this.visible, this);

                            // --- Expand button ---
                            var button = L.DomUtil.create('div', options.className + '-button', container);
                            L.DomEvent.on(button, 'click', this.visible, this)
                            // --- end Expand button ---
                            break;
                        case 'close':
                            item = L.DomUtil.create('span', 'icon-' + action, this.title);
                            L.DomEvent.on(item, 'click', this.close, this);
                            break;
                        case 'updown':
                            item = L.DomUtil.create('span', 'icon-' + action, this.title);
                            L.DomEvent.on(item, 'click', this.topbottom, this);
                            break;
                        case 'leftright':
                            item = L.DomUtil.create('span', 'icon-' + action, this.title);
                            L.DomEvent.on(item, 'click', this.leftright, this);
                            break;
                    }
                }
            }

            //L.DomEvent.on(title, 'click', this._close, this);
            if (!L.Browser.touch) {
                L.DomEvent
                    .disableClickPropagation(title)
                    .disableScrollPropagation(title);
            }
        } else {
            title = false;
        }
        // --- end Title ---

        // --- Item list ---
        this.items = items = L.DomUtil.create('div', options.className + '-items', container);
        // --- end Item list ---

        // --- Footer ---
        if (options.footer) {
            var footer = L.DomUtil.create('div', options.className + '-footer', container);
            if (!L.Browser.touch) {
                L.DomEvent
                    .disableClickPropagation(footer)
                    .disableScrollPropagation(footer);
            }
        }
        // -- end Footer ---

        // --- Scrollbar ---
        this.scrollbar = options.scrollbar.handler != false ? options.scrollbar.handler : false;
        if (this.scrollbar) {
            this.wrapper = $(this.items).wrap('<div class="wrapper-scrollbar" style="overflow:hidden;"></div>').parent();
            this.scrollbar = new this.scrollbar(this.wrapper[0], options.scrollbar.options);
        }
        // --- end Scrollbar ---

        // --- Events for touch and no touch ---
        // --- end Events for touch and no touch ---

        window.onresize = this._onResizeWindow.bind(this);
    },
    _initLayout: function () {
    },
    _update: function (force) {
        if (!this.container) {
            return;
        }

        if (this.scrollbar) {
            this.scrollbar.refresh(force);
        }
    },
    _onResizeWindow: function () {
        this._update();
    },
    addTo: function (container) {
        this._initLayout();

        // --- Add to map layer ---
        if (container && container.hasOwnProperty('_panes')) {
            this._map = container;
            var controlCorner = this._map._controlCorners.topleft;
            if (this.options.position == 'left' && controlCorner.children.length > 0) {
                controlCorner.insertBefore(this.container, controlCorner.firstChild);
            } else {
                controlCorner.appendChild(this.container);
            }
            L.DomUtil.addClass(controlCorner, L.Panel.baseClass);

            if (this.options.orientation == 'portrait' && this.options.fitToPage && this.scrollbar) {
                $(this.container).wrap('<div class="warp-specter-panel"></div>');
            }

            this._update();
            return this;
        }
        // --- end Add to map layer ---

        // --- Add to other element after map ---
        $(this.container).insertAfter($('.leaflet-container').first());
        // --- end Add to other element after map ---

        this._update();
        return this;
    },
    tools: this._tools,
    removeFrom: function (map) {
        var i, child, controlCorner;

        if (this.options.position == 'left') {
            controlCorner = this._map._controlCorners.topleft;
        } else {
            controlCorner = this._map._controlCorners.topright;
        }
        L.DomUtil.removeClass(controlCorner, "object-panel-shift-" + this.options.position);
        this._map = null;
        return this;
    },
    currentItem: function () { return this._currentItem; },
    _open: function () {
        if (L.DomUtil.hasClass(this.container, 'collapsed')) {
            L.DomUtil.addClass(this.container, 'expanded');
            L.DomUtil.removeClass(this.container, 'collapsed');
            if (this._scroll != undefined) {
                this.visibleScroll(true);
            }
        }
        this._fire('opening');
        return this;
    },
    select: function (e) {
        var i, child,
            obj = e.currentTarget,
            id = $(obj).data("id"),
            tool = $('.tool-panel');

        if (this._currentItem != null && L.DomUtil.hasClass(this._currentItem, 'active')) {
            L.DomUtil.removeClass(this._currentItem, 'active')
            var currentItemId = $(this._currentItem).data('id');
            this._currentItem = null;
            if (currentItemId == id) {
                this._fire("selectedItem", 0);
                this._actions['select'].call(this, 0);
                return this;
            }
        }

        this._currentItem = obj;
        L.DomUtil.addClass(obj, 'active');

        //for (i = this._itemList.length - 1; i >= 0; i--) {
        //    child = this._itemList[i];
        //    if (child.getAttribute('data-id') == id) {
        //        L.DomUtil.addClass(child, 'active');
        //        this._currentItem = child;
        //        break;
        //    }
        //}

        //Raise event
        if (id != null) {
            this._actions['select'].call(this, id);
            this._fire('selectedItem', id)
        }
        return this;
    },
    visible: function () {
        if (L.DomUtil.hasClass(this.container, 'expanded')) {
            L.DomUtil.addClass(this.container, 'collapsed');
            L.DomUtil.removeClass(this.container, 'expanded');
            this._fire('visible', 'collapsed');
        } else {
            L.DomUtil.addClass(this.container, 'expanded');
            L.DomUtil.removeClass(this.container, 'collapsed')
            this._fire('visible', 'expanded');
            this._update();
            if (this.scrollbar) {
                this.scrollbar.refresh();
            }
        }
        return this;
    },
    leftright: function () {
        if (L.DomUtil.hasClass(this.container, 'left')) {
            L.DomUtil.removeClass(this.container, 'left');
            L.DomUtil.addClass(this.container, 'right');
        } else {
            L.DomUtil.removeClass(this.container, 'right');
            L.DomUtil.addClass(this.container, 'left');
        }
    },
    topbottom: function () {
        if (L.DomUtil.hasClass(this.container, 'bottom')) {
            L.DomUtil.removeClass(this.container, 'bottom');
            L.DomUtil.addClass(this.container, 'top');
        } else {
            L.DomUtil.removeClass(this.container, 'top');
            L.DomUtil.addClass(this.container, 'bottom');
        }
    },
    addListener: function (type, listener) {
        if (typeof this._listeners[type] == "undefined") {
            this._listeners[type] = [];
        }

        this._listeners[type].push(listener);
    },
    removeListener: function (type, listener) {
        if (this._listeners[type] instanceof Array) {
            var listeners = this._listeners[type];
            for (var i = 0, len = listeners.length; i < len; i++) {
                if (listeners[i] === listener) {
                    listeners.splice(i, 1);
                    break;
                }
            }
        }
    },
    visibleScroll: function (visible) {
        this._actions['scrollVisible'].call(this, visible);
        //this._scroll.cursorVisible(visible != undefined ? visible : false);
    },
    addActions: function (action) {
        this._actions = action;
        return this;
    },
    _fire: function (event, value) {
        if (typeof event == "string") {
            event = {
                type: event,
                value: value
            };
        }
        if (!event.target) {
            event.target = this;
        }

        if (!event.type) {  //falsy
            throw new Error("Event object missing 'type' property.");
        }

        if (this._listeners[event.type] instanceof Array) {
            var listeners = this._listeners[event.type];
            for (var i = 0, len = listeners.length; i < len; i++) {
                listeners[i].call(this, event);
            }
        }
    },
    _enableMapControl: function (e) {
        this._map.scrollWheelZoom.enable();
        //this._map.dragging.enable();
    },
    _disableMapControl: function (e) {
        this._map.scrollWheelZoom.disable();
        //this._map.dragging.disable();
    },
    _mouseEnter: function (e) {
        var tooltip, target = e.target,
            xy = L.Browser.ie ? ElemCoords(target) : target.getBoundingClientRect(),
            pos = L.point([xy.left + target.offsetWidth - 10, xy.top + target.offsetHeight / 2]);

        if (this.options.tooltip) {
            var text = $(target).data("name");
            if (text) {
                tooltip = new L.Tooltip(this.container);
                tooltip.updateContent({ text: text });
                tooltip.updatePosition(pos, true);
                var timer = setTimeout(L.Util.bind(function (e) {
                    tooltip.dispose();
                }, this), 2000);
                this._tooltips.push({ timerId: timer, handler: tooltip });
                //console.log('%cel: %s, pos: %s', 'color: blue', e.toElement, pos.x + '-' + pos.y);
            }
        }
    },
    _mouseLeave: function (e) {
        if (this.options.tooltip) {
            var count = this._tooltips.length;
            for (var i = 0; i < count; i++) {
                var tooltip = this._tooltips.pop();
                clearTimeout(tooltip.timerId);
                tooltip.handler.dispose();
            }
        }
    },
});
L.panel = function (options) {
    return new L.Panel(options);
};
// --- end Panel ---

// --- Object panel ---
L.Panel.Objects = L.Panel.extend({
    //init: function () {
    //    this.superclass = this.__proto__.__proto__;
    //},
    addItem: function (value) {
        var item = L.DomUtil.create('div', 'object-item panel-bar-flat', this.items),
            image = $(L.DomUtil.create('img', 'item-photo', item)),
            name = L.DomUtil.create('div', 'item-name', item);

        $(name).scrollText(item);

        $(item).data('id', value.id);
        image.attr('src', value.url);
        image.data('big-photo', value.urlBig);

        name.innerHTML = value.name;
        //this._itemList.push(item);
        L.DomEvent.on(item, 'click', this.select, this);
        if (!L.Browser.touch) {
            L.DomEvent.on(item, 'mouseover', this._disableMapControl, this);
            L.DomEvent.on(item, 'mouseout', this._enableMapControl, this);
        } else {
            this._map.scrollWheelZoom.disable();
            //L.DomEvent.off(item, 'dragging', L.DomEvent.stopPropagation);
            //this._map.dragging.disable();
        }
        // Raise event
        this._fire('addItem', value);
    },
    ajaxLoad: function (items) {
        if (typeof items != "string") {
            return this; 
        }
        $.ajax({
            type: "POST",
            url: items,
            data: {},
            async: true,
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            success: this.loadItems.bind(this, this),
            error: function (xhr) { }
        }, this);
        return this;
    },
    loadItems: function (container, data) {
        var objects = data.objects,
            options = container.options,
            i, count = objects.length;

        for (i = 0; i < count; i++) {
            container.addItem(objects[i]);
        }
        //this.superclass._update();
        this._update();
        //setTimeout(function (that) { that._update(); }, 100, this);
    },
});
L.panel.objects = function (options) {
    var panel = new L.Panel.Objects(options);
    //panel.init();
    return panel;
}
// --- end Object panel ---

// --- Tools panel ---
L.Panel.Tool = L.Panel.extend({
    options: {
        position: 'bottom',
        state: 'expanded',
        title: {
            width: 16,
            height: 16
        },
        item: {
            width: 36,
            height: 36
        }
    },
    loadItems: function (data) {
        var width = 0,
            height = 0,
            item;

        var items = L.DomUtil.create('ul', this.options.className + '-items', this.container);
        for (var li in data) {
            item = L.DomUtil.create('li', '', items);
            for (var _property in data[li]) {
                if (_property != 'items') {
                    item.setAttribute(_property, data[li][_property]);
                } else if (_property == 'items') {
                    this._createMenu(data[li][_property], item);
                }
            }
            if (!data[li].hasOwnProperty('items')) {
                L.DomUtil.create('span', 'popup-item', item);
            }
            if (this.options.orientation == 'landscape') {
                width += this.options.item.width;
            } else {
                height += this.options.item.height;
            }
        }

        if (height == 0) {
            $(this.container).css('width', width + this.options.title.width);
            $(this.container).css('height', this.options.item.height);
        } else {
            $(this.container).css('width', this.options.item.width + 'px;');
            $(this.container).css('height', height + 'px;' + this.options.title.height);
        }
        return this;
    },
    _update: function() {},
    open: function (id) {
        this.id = id;
        $(this.container).show('clip', {}, 300);
        L.DomUtil.addClass(this.container, 'expanded');
        L.DomUtil.removeClass(this.container, 'collapsed');
        if (this._scroll) {
            this.visibleScroll(true);
        }
    },
    close: function () {
        this.container.setAttribute('data-id', 0);
        if (this._scroll) {
            this.visibleScroll(false);
        }
        $(this.container).hide('clip', {}, 300);
        L.DomUtil.addClass(this.container, 'collapsed');
        L.DomUtil.removeClass(this.container, 'expanded');

        if (this.objectPanel._currentItem) {
            L.DomUtil.removeClass(this.objectPanel._currentItem, 'active');
        }
    },
    objectPanel: this._objectPanel,
    doAction: function (e) {
        doAction(this.id, this._getAction(e), getData(e.currentTarget, 'name').locale(), this);
    },
    // Utils
    _createMenu: function (items, parent) {
        var i, _attribute,
            _ul = document.createElement('ul');
        parent.appendChild(_ul);

        for (i = 0; i < items.length; i++) {
            var item = L.DomUtil.create('li', '', _ul);
            for (_attribute in items[i]) {
                item.setAttribute(_attribute, items[i][_attribute]);
                if (items[i].hasOwnProperty('data-action')) {
                    L.DomEvent
                        .on(item, 'click', this.doAction, this)
                        .on(item, 'mouseenter', this._mouseEnter, this)
                        .on(item, 'mouseleave', this._mouseLeave, this);
                }
            }
        }
        return _ul;
    },
    _getAction: function(e) {
        var type = getData(e.currentTarget, 'type');
        return {
            name: getData(e.currentTarget, 'action').toLowerCase(),
            type: type ? type.toLowerCase() : false
        };
    },
    //_mouseEnter: function (e) {
    //    var tooltip, target = e.target,
    //        xy = L.Browser.ie ? ElemCoords(target) : target.getBoundingClientRect(),
    //        pos = L.point([xy.left + target.offsetWidth, xy.top + target.offsetHeight / 2]);

    //    tooltip = new L.Tooltip(this.container);
    //    tooltip.updateContent({ text: 'Test' });
    //    tooltip.updatePosition(pos, true);
    //    setTimeout(L.Util.bind(function (e) {
    //        tooltip.dispose();
    //    }, this), 2000);

    //    console.log('%cel: %s, pos: %s', 'color: blue', e.toElement, pos.x + '-' + pos.y);
    //}
});
L.panel.tool = function (options) {
    return new L.Panel.Tool(options);
};
// --- end Tools panel ---

function ElemCoords(obj) {
    var curleft = 0;
    var curtop = 0;
    if (obj.offsetParent) {
        while (1) {
            curleft += obj.offsetLeft;
            curtop += obj.offsetTop;
            if (!obj.offsetParent)
                break;
            obj = obj.offsetParent;
        }
    }
    else if (obj.x || obj.y) {
        curleft += obj.x;
        curtop += obj.y;
    }
    return { "left": curleft, "top": curtop };
}