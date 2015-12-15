// --- Override DivIcon ---
L.DivStyledIcon = L.Icon.extend({
    options: {
        iconSize: [12, 12], // also can be set through CSS
        /*
		iconAnchor: (Point)
		popupAnchor: (Point)
		html: (String)
		bgPos: (Point)
		*/
        className: 'leaflet-div-icon',
        html: false
    },
    createIcon: function (oldIcon) {
        var div = (oldIcon && oldIcon.tagName === 'DIV') ? oldIcon : document.createElement('div'),
		    options = this.options;

        if (options.html !== false) {
            div.innerHTML = options.html;
        } else {
            div.innerHTML = '';
        }

        if (options.bgPos) {
            div.style.backgroundPosition =
			        (-options.bgPos.x) + 'px ' + (-options.bgPos.y) + 'px';
        }

        this._setIconStyles(div, 'icon');

        if (options.style) {
            this._setCSSStyle(div, options.style);
        }

        return div;
    },
    createShadow: function () {
        return null;
    },
    _setCSSStyle: function (div, css) {
        var style = div.style.cssText;
        css += css.lastIndexOf(';') ? '' : ';';
        style += style.lastIndexOf(';') ? css : ';' + css;
        div.style.cssText = style;
    }
});
L.divIcon = function (options) {
    return new L.DivIcon(options);
};
// --- end Override DivIcon ---
