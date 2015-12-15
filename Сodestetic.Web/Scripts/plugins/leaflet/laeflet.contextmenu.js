L.Map.include({
	openContextMenu: function (contextMenu, latlng, options) { // (ContextMenu) or (String || HTMLElement, LatLng[, Object])
		this.closeContextMenu();

		if (!(contextMenu instanceof L.ContextMenu)) {
			var content = contextMenu;

			contextMenu = new L.ContextMenu(options)
			    .setLatLng(latlng)
			    .setContent(content);
		}
		contextMenu._isOpen = true;

		this._contextMenu = contextMenu;
		return this.addLayer(contextMenu);
	},
	closeContextMenu: function (contextMenu) {
		if (!contextMenu || contextMenu === this._contextMenu) {
			contextMenu = this._contextMenu;
			this._contextMenu = null;
		}
		if (contextMenu) {
			this.removeLayer(contextMenu);
			contextMenu._isOpen = false;
		}
		return this;
	}
});
// --- Leaflet context menu ---
L.ContextMenu = L.Class.extend({
	includes: L.Mixin.Events,
	options: {
		minWidth: 50,
		maxWidth: 300,
		// maxHeight: null,
		autoPan: true,
		closeButton: true,
		offset: [0, 7],
		autoPanPadding: [5, 5],
		// autoPanPaddingTopLeft: null,
		// autoPanPaddingBottomRight: null,
		keepInView: false,
		className: '',
		zoomAnimation: true,
		orientation: 'landscape'
	},
	initialize: function (options, source) {
		L.setOptions(this, options);

		this._source = source;
		this._animated = L.Browser.any3d && this.options.zoomAnimation;
		this._isOpen = false;
	},
	onAdd: function (map) {
		this._map = map;

		if (!this._container) {
			this._initLayout();
		}

		var animFade = map.options.fadeAnimation;

		if (animFade) {
			L.DomUtil.setOpacity(this._container, 0);
		}
		map._panes.popupPane.appendChild(this._container);

		map.on(this._getEvents(), this);

		this.update();

		if (animFade) {
			L.DomUtil.setOpacity(this._container, 1);
		}

		this.fire('open');

		map.fire('contextmenuopen', { contextMenu: this });

		if (this._source) {
			this._source.fire('contextmenuopen', { contextMenu: this });
		}
	},
	addTo: function (map) {
		map.addLayer(this);
		return this;
	},
	openOn: function (map) {
	    this._map = map;
	    map.openContextMenu(this);
		return this;
	},
	onRemove: function (map) {
		map._panes.popupPane.removeChild(this._container);

		L.Util.falseFn(this._container.offsetWidth); // force reflow

		map.off(this._getEvents(), this);

		if (map.options.fadeAnimation) {
			L.DomUtil.setOpacity(this._container, 0);
		}

		this._map = null;

		this.fire('close');

		map.fire('contextmenuclose', { contextMenu: this });

		if (this._source) {
			this._source.fire('contextmenuclose', { contextMenu: this });
		}
	},
	getLatLng: function () {
		return this._latlng;
	},
	setLatLng: function (latlng) {
		this._latlng = L.latLng(latlng);
		if (this._map) {
			this._updatePosition();
			this._adjustPan();
		}
		return this;
	},
	getContent: function () {
		return this._content;
	},
	setContextMenu: function (contentMenu) {
	    this._content = contentMenu;
		this.update();
		return this;
	},
	update: function () {
		if (!this._map) { return; }

		this._container.style.visibility = 'hidden';

		this._updateContent();
		this._updateLayout();
		this._updatePosition();

		this._container.style.visibility = '';

		this._adjustPan();
	},
	_getEvents: function () {
		var events = {
			viewreset: this._updatePosition
		};

		if (this._animated) {
			events.zoomanim = this._zoomAnimation;
		}
		if ('closeOnClick' in this.options ? this.options.closeOnClick : this._map.options.closeContextMenuOnClick) {
			events.preclick = this._close;
		}
		if (this.options.keepInView) {
			events.moveend = this._adjustPan;
		}

		return events;
	},
	_close: function () {
	    if (this._map) {
	        L.DomEvent
                .off(this._wrapper, 'click', this._selectItem, this)
                .off(this._wrapper, 'mouseleave', this._mouseEnter, this);

		    this._map.closeContextMenu(this);
		}
	},
	_initLayout: function () {
		var prefix = 'leaflet-context-menu',
			containerClass = prefix + ' ' + this.options.className + ' leaflet-zoom-' +
			        (this._animated ? 'animated' : 'hide') +
					' ' + prefix + '-' + this.options.orientation,
			container = this._container = L.DomUtil.create('div', containerClass),
			closeButton;

		if (this.options.closeButton) {
			closeButton = this._closeButton =
			        L.DomUtil.create('a', prefix + '-close-button', container);
			closeButton.href = '#close';
			closeButton.innerHTML = '&#215;';
			L.DomEvent.disableClickPropagation(closeButton);

			L.DomEvent.on(closeButton, 'click', this._onCloseButtonClick, this);
		}

		var wrapper = this._wrapper =
		        L.DomUtil.create('div', prefix + '-content-wrapper', container);
		L.DomEvent.disableClickPropagation(wrapper);

		this._contentNode = L.DomUtil.create('div', prefix + '-content', wrapper);
		
		var menu = this._menu = L.DomUtil.create('ul', prefix, this._contentNode);

		L.DomEvent.disableScrollPropagation(this._contentNode);
		L.DomEvent.on(wrapper, 'contextmenu', L.DomEvent.stopPropagation);

		//this._tipContainer = L.DomUtil.create('div', prefix + '-tip-container', container);
	    //this._tip = L.DomUtil.create('div', prefix + '-tip', this._tipContainer);

		L.DomEvent.on(wrapper, 'mouseenter', this._mouseEnter, this);
	},
	_updateContent: function () {
		if (!this._content) { return; }

		if (typeof this._content === 'string') {
		    this._menu.innerHTML = this._content;
		    //this._contentNode.innerHTML = this._content;
		} else {
			while (this._contentNode.hasChildNodes()) {
				this._contentNode.removeChild(this._contentNode.firstChild);
			}
			this._contentNode.appendChild(this._content);
		}
		this.fire('contentupdate');
	},
	_updateLayout: function () {
		var container = this._contentNode,
		    style = container.style;

		style.width = '';
		style.whiteSpace = 'nowrap';

		var width = container.offsetWidth;
		width = Math.min(width, this.options.maxWidth);
		width = Math.max(width, this.options.minWidth);

		style.width = (width + 1) + 'px';
		style.whiteSpace = '';

		style.height = '';

		var height = container.offsetHeight,
		    maxHeight = this.options.maxHeight,
		    scrolledClass = 'leaflet-context-menu-scrolled';

		if (maxHeight && height > maxHeight) {
			style.height = maxHeight + 'px';
			L.DomUtil.addClass(container, scrolledClass);
		} else {
			L.DomUtil.removeClass(container, scrolledClass);
		}

		this._containerWidth = this._container.offsetWidth;
	},
	_updatePosition: function () {
		if (!this._map) { return; }

		var pos = this._map.latLngToLayerPoint(this._latlng),
		    animated = this._animated,
		    offset = L.point(this.options.offset);

		if (animated) {
			L.DomUtil.setPosition(this._container, pos);
		}

		this._containerBottom = -offset.y - (animated ? 0 : pos.y);
		this._containerLeft = -Math.round(this._containerWidth / 2) + offset.x + (animated ? 0 : pos.x);

		// bottom position the context menu in case the height of the context menu changes (images loading etc)
		this._container.style.bottom = this._containerBottom + 'px';
		this._container.style.left = this._containerLeft + 'px';
	},
	_zoomAnimation: function (opt) {
		var pos = this._map._latLngToNewLayerPoint(this._latlng, opt.zoom, opt.center);

		L.DomUtil.setPosition(this._container, pos);
	},
	_adjustPan: function () {
		if (!this.options.autoPan) { return; }

		var map = this._map,
		    containerHeight = this._container.offsetHeight,
		    containerWidth = this._containerWidth,

		    layerPos = new L.Point(this._containerLeft, -containerHeight - this._containerBottom);

		if (this._animated) {
			layerPos._add(L.DomUtil.getPosition(this._container));
		}

		var containerPos = map.layerPointToContainerPoint(layerPos),
		    padding = L.point(this.options.autoPanPadding),
		    paddingTL = L.point(this.options.autoPanPaddingTopLeft || padding),
		    paddingBR = L.point(this.options.autoPanPaddingBottomRight || padding),
		    size = map.getSize(),
		    dx = 0,
		    dy = 0;

		if (containerPos.x + containerWidth + paddingBR.x > size.x) { // right
			dx = containerPos.x + containerWidth - size.x + paddingBR.x;
		}
		if (containerPos.x - dx - paddingTL.x < 0) { // left
			dx = containerPos.x - paddingTL.x;
		}
		if (containerPos.y + containerHeight + paddingBR.y > size.y) { // bottom
			dy = containerPos.y + containerHeight - size.y + paddingBR.y;
		}
		if (containerPos.y - dy - paddingTL.y < 0) { // top
			dy = containerPos.y - paddingTL.y;
		}

		if (dx || dy) {
			map
			    .fire('autopanstart')
			    .panBy([dx, dy]);
		}
	},
	_onCloseButtonClick: function (e) {
		this._close();
		L.DomEvent.stop(e);
	},
	_mouseEnter: function (e) {
	    L.DomEvent
            .on(this._wrapper, 'mouseleave', this._mouseLeave, this)
            .on(this._wrapper, 'click', this._selectItem, this)
	        .off(this._wrapper, 'mouseenter', this._mouseEnter, this);
	    return false;
	},
	_mouseLeave: function (e) {
	    this._close();
	    return false;
	},
	_selectItem: function (e) {
	    var target = e.target;
	    if (target.parentNode.tagName != 'LI') {
	        this._close();
	    }
	    return false;
	}
});
L.contextMenu = function (options, source) {
	return new L.ContextMenu(options, source);
};
// --- end Leaflet context menu ---