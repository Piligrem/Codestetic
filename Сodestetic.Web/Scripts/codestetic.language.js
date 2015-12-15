(function () {
    if (!String.locale) {
        String.prototype.Resources = {};
        String.prototype.locale = function () {
            var key = this.toString(),
                html = [],
                arguments = arguments,
                format = String.Resources[key],
                objIndex = 0,
                reg = /\%s/,
                parts = [], i, part, object;

            format = (format ? format : key).replace(/\{[0-9]*\}/g, '%s');
            for (i = reg.exec(format) ; i; i = reg.exec(format)) {
                parts.push(format.substr(0, i[0][0] == '%' ? i.index : i.index + 1));
                parts.push('%s');
                format = format.substr(i.index + i[0].length);
            }
            parts.push(format);
            for (i = 0; i < parts.length; i++) {
                part = parts[i];
                if (part && part == '%s') {
                    object = arguments[objIndex++];
                    if (object != undefined) {
                    //    html.push('%s');
                    //} else {
                        html.push(object);
                    }
                } else {
                    html.push(part);
                }
            }
            return html.join('');
        }
    }
})();
