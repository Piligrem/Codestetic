// --- Get color from HexA
String.prototype.toColor = function (type) {
    var value = this.toString(),
        rgba = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/g.exec(value);
    if (!rgba) {
        rgba = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/g.exec(value);
        rgba.push('FF');
    }
    switch (type) {
        case 'rgba':
            return 'rgba(' + parseInt(rgba[1], 16) + ',' + parseInt(rgba[2], 16) + ',' + parseInt(rgba[3], 16) + ',' + parseInt(rgba[4], 16) / 255 + ')';
        case 'hex':
            return {
                color: rgba[0].substr(0, 7),
                opacity: value.length == 9 ? parseInt(this.substr(7, 2), 16) / 255 : 1
            }
        case 'lum':
            return colorLuminance(rgba[0].substr(0, 7), value.length == 9 ? parseInt(this.substr(7, 2), 16) / 255 : 1);
    }
    return '#000000';

    function colorLuminance(hex, lum) {
        // validate hex string
        hex = String(hex).replace(/[^0-9a-f]/gi, '');
        if (hex.length < 6) {
            hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
        }
        lum = lum || 0;

        // convert to decimal and change luminosity
        var rgb = "#", c, i;
        for (i = 0; i < 3; i++) {
            c = parseInt(hex.substr(i * 2, 2), 16);
            c = Math.round(Math.min(Math.max(0, 255 * (1 - lum) + (c * lum)), 255)).toString(16);
            rgb += ("00" + c).substr(c.length);
        }

        return rgb;
    }
};

String.prototype.camelCase = function() {
    return (this || '').toLowerCase().replace(/(\b|-)\w/g, function (m) {
        return m.toUpperCase().replace(/-/, '');
    });
}

function getOffset(elem) {
    if (elem.getBoundingClientRect) {
        // "РїСЂР°РІРёР»СЊРЅС‹Р№" РІР°СЂРёР°РЅС‚
        return getOffsetRect(elem)
    } else {
        // РїСѓСЃС‚СЊ СЂР°Р±РѕС‚Р°РµС‚ С…РѕС‚СЊ РєР°Рє-С‚Рѕ
        return getOffsetSum(elem)
    }
}

function getOffsetSum(elem) {
    var top=0, left=0
    while(elem) {
        top = top + parseInt(elem.offsetTop)
        left = left + parseInt(elem.offsetLeft)
        elem = elem.offsetParent
    }

    return {top: top, left: left}
}

function getOffsetRect(elem) {
    // (1)
    var box = elem.getBoundingClientRect()

    // (2)
    var body = document.body
    var docElem = document.documentElement

    // (3)
    var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop
    var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft

    // (4)
    var clientTop = docElem.clientTop || body.clientTop || 0
    var clientLeft = docElem.clientLeft || body.clientLeft || 0

    // (5)
    var top  = box.top +  scrollTop - clientTop
    var left = box.left + scrollLeft - clientLeft

    return { top: Math.round(top), left: Math.round(left) }
}