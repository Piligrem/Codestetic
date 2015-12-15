// --- Toggle button ---
; (function ($) {
    var defaults = {
        enable: true
    };
    var ToggleButton = function (element, options) {
        this.element = $(element);
        this.options = $.extend(true, {}, defaults, this.element.data(), options);

        if (this.element.data('toggle') == undefined) {
            this.element.data('toggle', 'off');
        }

        if (this.options.enable) {
            this.enable();
        } else {
            this.disable();
        }

        this.update(this.element);

        this.element.on('mousedown.togglebutton touchstart.togglebutton', $.proxy(this.mousedown, this));

        $($.proxy(function () {
            this.element.trigger('create');
        }, this));
    }
    ToggleButton.prototype = {
        constructor: ToggleButton,
        mousedown: function (e) {
            if (this.isDisabled()) { return false; }

            var target = $(e.currentTarget);

            if (target.data('toggle') == 'off') {
                target.data('toggle', 'on');
            } else {
                target.data('toggle', 'off');
            }
            this.update(target);
            return false;
        },
        destroy: function () { },
        getValue: function () {
            return this.element.data('value');
        },
        update: function (element) {
            var value = 'enable';

            if (element.data('toggle') == 'on') {
                element
                    .data('value', true)
                    .addClass('active');
            } else {
                element
                    .data('value', false)
                    .removeClass('active');
                value = 'disable';
            }

            this.element.trigger({
                type: 'state',
                state: value,
                value: this.getValue()
            });
            return this;
        },
        isDisabled: function () {
            return (this.element.prop('disabled') === true);
        },
        enable: function () {
            this.element.prop('disabled', false);
            this.element.trigger({
                type: 'enable',
                value: this.getValue()
            });
            this.element.trigger({
                type: 'state',
                state: 'enable',
                value: this.getValue()
            });
            return true;
        },
        disable: function () {
            this.element.prop('disabled', true);
            this.element.trigger({
                type: 'disable',
                value: this.getValue()
            });
            this.element.trigger({
                type: 'state',
                state: 'disable',
                value: this.getValue()
            });
            return true;
        },
        refresh: function () {
            this.update(this.element);
        }
    }
    $.togglebutton = ToggleButton;
    $.fn.togglebutton = function (option) {
        var markerArgs = arguments,
          rv;

        var $returnValue = this.each(function () {
            var $this = $(this),
              inst = $this.data('togglebutton'),
              options = ((typeof option === 'object') ? option : {});
            if ((!inst) && (typeof option !== 'string')) {
                $this.data('togglebutton', new ToggleButton(this, options));
            } else {
                if (typeof option === 'string') {
                    rv = inst[option].apply(inst, Array.prototype.slice.call(markerArgs, 1));
                }
            }
        });
        if (option === 'getValue') {
            return rv;
        }
        return $returnValue;
    };
    $.fn.togglebutton.constructor = ToggleButton;
})(jQuery);