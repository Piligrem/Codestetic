// --- Spinner ---
; (function ($) {
    var defaults = {
        min: 0,
        max: 999,
        templateContainer: '<div class="spinner" role="container">' +
            '<div class="spinner" role="control">' +
            '<button class="btn btn-default spinner" role="up"><i class="icon-caret-up"></i></span>' +
            '<button class="btn btn-default spinner" role="down"><i class="icon-caret-down"></i></span>' +
            '</div>'
    };

    var Spinner = function (element, options) {
        this.input = $(element)
            .addClass('form-control')
            .addClass('spinner')
            .attr('role', 'input');
        this.options = $.extend(true, {}, defaults, this.input.data(), options);

        // --- Input element ---
        this.input
            .attr('type', 'text')
            .attr('min', this.options.min)
            .attr('max', this.options.max);
        // --- end Input element ---

        // --- Create container ---
        this.container = $(this.options.templateContainer);
        this.container.insertBefore(this.input);
        this.input.insertBefore(this.container.children('.spinner[role="control"]'));
        // --- end Create container ---

        this.controls = this.container.children('[role="control"]');

        // --- Set events ---
        this.input.on('keypress.spinner', $.proxy(this.keypress, this));
        this.input.on('input.spinner', $.proxy(this.change, this));
        this.controls.on('mousedown.spinner touchstart.spinner', $.proxy(this.mousedown, this));
        // --- end Set events on button ---

        $($.proxy(function () {
            this.container.trigger('create');
        }, this));
    };

    Spinner.prototype = {
        constructor: Spinner,
        destroy: function() {},
        getValue: function () {
            return parseInt(this.input.val(), 10);
        },
        mousedown: function (e) {
            //if (!e.pageX && !e.pageY && e.originalEvent) {
            //    e.pageX = e.originalEvent.touches[0].pageX;
            //    e.pageY = e.originalEvent.touches[0].pageY;
            //}
            //e.stopPropagation();
            //e.preventDefault();
            if (this.isDisabled()) { return false; }

            var parentTarget = $(e.currentTarget), target = $(e.target),
                parentRole = parentTarget.attr('role'), role = target.attr('role');

            if (target.attr('role') != 'up' || 'down') {
                target = target.parent();
                role = target.attr('role');
            }

            var value = this._checkValue();

            switch (parentRole) {
                case 'control':
                    if (role == 'up' && value < this.options.max) {
                        this.input.val(value + 1);
                    } else if (role == 'down' && value > this.options.min) {
                        this.input.val(parseInt(this.input.val(), 10) - 1);
                    }
                    break;
            }
            return false;
        },
        change: function (e) {
            $(e.target).val(this._checkValue());
            return false;
        },
        keypress: function (e) {
            var verified = (e.which == 8 || e.which == undefined || e.which == 0) ? null : String.fromCharCode(e.which).match(/[^0-9]/);
            if (verified) {
                e.preventDefault();
            }
        },
        keyup: function (e) { },
        update: function (value) { },
        isDisabled: function () {
            return (this.input.prop('disabled') === true);
        },
        enable: function () {
            this.input.prop('disabled', false);
            this.controls.children().prop('disabled', false);
            this.container.trigger({
                type: 'enable',
                value: this.getValue()
            });
            this.container.trigger({
                type: 'state',
                state: 'enable',
                value: this.getValue()
            });
            return true;
        },
        disable: function () {
            this.input.prop('disabled', true);
            this.controls.children().prop('disabled', true);
            this.container.trigger({
                type: 'disable',
                value: this.getValue()
            });
            this.container.trigger({
                type: 'state',
                state: 'enable',
                value: this.getValue()
            });
            return true;
        },
        _checkValue: function () {
            var value = parseInt(this.input.val(), 10);

            if (value > this.options.max) {
                value = this.options.max;
            } else if (value < this.options.min) {
                value = this.options.min;
            } //else if (!target.val()) { target.val(0); }

            return value;
        }
    }


    $.spinner = Spinner;
    $.fn.spinner = function (option) {
        var markerArgs = arguments,
        rv;

        var $returnValue = this.each(function () {
            var $this = $(this),
            inst = $this.data('spinner'),
            options = ((typeof option === 'object') ? option : {});
            if ((!inst) && (typeof option !== 'string')) {
                $this.data('spinner', new Spinner(this, options));
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
    $.fn.spinner.constructor = Spinner;
})(jQuery);
