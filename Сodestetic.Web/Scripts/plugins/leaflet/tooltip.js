L.Tooltip = L.Class.extend({
    initialize: function (mainContainer) {
        if (mainContainer instanceof L.Map) {
            this._map = mainContainer;
            this._popupPane = mainContainer._panes.popupPane;
            this._container = mainContainer.options.drawControlTooltips ? L.DomUtil.create('div', 'leaflet-draw-tooltip', this._popupPane) : null;
        } else {
            this._map = false;
            this._mainContainer = mainContainer;
            this._container = L.DomUtil.create('div', 'leaflet-draw-tooltip', mainContainer);
        }
        this._singleLineLabel = false;
	},
	dispose: function () {
	    if (this._container) {
	        if (this._map) {
	            this._popupPane.removeChild(this._container);
	        } else {
	            $(this._container).detach();
	        }
			this._container = null;
		}
	},
	updateContent: function (labelText) {
		if (!this._container) {
			return this;
		}
		labelText.subtext = labelText.subtext || '';

		// update the vertical position (only if changed)
		if (labelText.subtext.length === 0 && !this._singleLineLabel) {
			L.DomUtil.addClass(this._container, 'leaflet-draw-tooltip-single');
			this._singleLineLabel = true;
		}
		else if (labelText.subtext.length > 0 && this._singleLineLabel) {
			L.DomUtil.removeClass(this._container, 'leaflet-draw-tooltip-single');
			this._singleLineLabel = false;
		}

		this._container.innerHTML =
			(labelText.subtext.length > 0 ? '<span class="leaflet-draw-tooltip-subtext">' + labelText.subtext + '</span>' + '<br />' : '') +
			'<span>' + labelText.text + '</span>';

		return this;
	},
	updatePosition: function (position, disable3D) {
	    var pos,
            tooltipContainer = this._container;

	    if (this._map) {
	        latlng = position;
	        pos = this._map.latLngToLayerPoint(latlng);
	    } else {
	        pos = position;
	        tooltipContainer.style.position = 'fixed';
	    }

		if (this._container) {
			tooltipContainer.style.visibility = 'inherit';
			L.DomUtil.setPosition(tooltipContainer, pos, disable3D);
		}

		return this;
	},
	showAsError: function () {
		if (this._container) {
			L.DomUtil.addClass(this._container, 'leaflet-error-draw-tooltip');
		}
		return this;
	},
	removeError: function () {
		if (this._container) {
			L.DomUtil.removeClass(this._container, 'leaflet-error-draw-tooltip');
		}
		return this;
	}
});
