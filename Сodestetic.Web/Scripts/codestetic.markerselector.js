; (function ($) {
    var defaults = {
        currentIndex: 0,
        iconSelected: {
            width: 64,
            height: 64,
        },
        iconSetting: {
            width: 64,
            height: 64,
            padding: 1,
            margin: 1,
            horizontalNumber: 2,
            verticalNumber: 3,
            backgroud: false
        },
        scrollbar: {
            handler: false,
            options: {}
        },
        ajaxQuery: {
            action: '', 
            controller: '', 
            route: ''
        },
        container: false, // container of list items
        downdrop: false, // click on element to expand of list 
        target: false, // target for the result
        inline: false, //forces to show the colorpicker as an inline element
        templateContainer: '<div class="markerselector" role="container"></div>',
        templateList: '<div class="markerselector" role="list">' +
        '</div>',
        templateMarker: '<div class="markerselector" role="item">' +
            '<img class="markerselector" role="marker" />' +
            '<img class="markerselector" role="shadow" />' +
            '</div>',
    };
    var MarkerSelector = function(element, options) {
        this.element = $(element)
            .addClass('markerselector-element')
            .attr('role', 'markerselector');

        this.options = $.extend(true, {}, defaults, this.element.data(), options);

        this.container = (this.options.container === true) ? this.element : this.options.container;
        this.container = (this.container !== false) ? $(this.container) : false;

        // --- Create container if not it's present ---
        if (this.container == false) {
            this.container = $(this.options.templateContainer);
            this.container.appendTo(this.element);
        }
        // --- end Create container if not it's present ---

        // --- List of markers ---
        this.itemRole = ['list', 'item', 'marker', 'shadow'];
        this.markerlist = $(this.options.templateList);
        var size = this.options.iconSetting,
            width = (size.width + size.margin) * size.horizontalNumber - size.margin,
        height = (size.height + size.margin) * size.verticalNumber - size.margin;
        this.markerlist
            .css('width', width)
            .css('height', height)
            .css('display', 'flex');

        if (this.options.inline) {
            this.container.show();
        } else {
            this.container.hide();
            this.container.css('position', 'absolute');
            // ???
            //this.container
            //    .addClass('expanded')
            //    .show();
        }

        if (this.options.inline === false) {
            this.element.on({
                'focusout.markerselector': $.proxy(this.hide, this)
            });
        }

        this.markerlist.appendTo(this.container ? this.container : $('body'));
        this.markerlist.on('mousedown.markerselector touchend.markerselector', $.proxy(this._select, this));
        // --- end List of markers ---

        // --- Target ---
        this.target = this.options.target !== false ? 
            (typeof this.options.target == 'string' ? $(this.options.target) : this.options.target) : false;
        if (this.target) {
            this.target.css('width', this.options.iconSelected.width)
                .css('height', this.options.iconSelected.height);
        }
        // --- end Target ---

        // --- DropDown ---
        this.dropdown = this.options.dropdown !== false ?
            (typeof this.options.dropdown == 'string' ? $(this.options.dropdown) : this.options.dropdown) : false;
        if (this.dropdown) {
            this.dropdown.attr('role', 'dropdown')
                .on('click.markerselector touchend.markerselector', $.proxy(this.show, this));

        }
        // --- end DropDown ---

        // --- Load data via ajax ---
        if (this.options.ajaxQuery.controller.length > 0) {
            this.ajaxLoad();
        }
        // --- end Load data via ajax ---

        // --- Scrollbar ---
        this.scrollbar = this.options.scrollbar.handler != false ? this.options.scrollbar.handler : false;
        if (this.scrollbar) {
            var wrapper = this.markerlist.wrap('<div class="wrapper-scrollbar"></div>').parent();
            this.scrollbar = new this.scrollbar(wrapper[0], this.options.scrollbar.options);
        }
        // --- end Scrollbar ---

        //this.update();

        $($.proxy(function() {
            this.element.trigger('create');
        }, this));

        return this;
    };
    var AjaxLoad = function () {
        var that = this,
            query = this.options.ajaxQuery,
            url = "/" + query.controller + "/" + query.action;
        if (query.route != undefined && query.route.length > 0) {
            url = "/" + query.route + url;
        }
        $.ajax({
            url: url,
            type: "POST",
            success: function (result) {
                if (result.length > 0) {
                    that.loadMarkers(result);
                }
            },
            error: function (xhr) {
                $.log("Error: {0}, Message: {1}".format(xhr.error, xhr.statusText), form.attr('action'));
            }
        });
    };
    var LoadMarkers = function (markers) {
        var options = this.options,
            size = options.iconSetting, data = options.data,
            template = options.templateMarker,
            i, count = markers.length - 1,
            currentId = this.element.data('id');

        for (i = 0; i < markers.length; i++) {
            var marker = $(template);
            var id = markers[i].id;

            marker.css('width', size.width)
                .css('height', size.height)
                .data('id', id);

            if ((i + 1) % size.horizontalNumber != 0) {
                marker.css('margin', '0 ' + size.margin + 'px ' + ((i < count) ? size.margin + 'px' : '0') + ' 0');
            }
            marker.appendTo(this.markerlist);

            var m = marker.find('.markerselector[role="marker"]'),
                s = marker.find('.markerselector[role="shadow"]');

            var x = markers[i].marker.x,
                y = markers[i].marker.y;

            m.attr('src', markers[i].marker.url)
                .attr('data-width', markers[i].marker.width)
                .attr('data-height', markers[i].marker.height)
                .attr('data-x', x)
                .attr('data-y', y);

            if (markers[i].shadow) {
                s.attr('src', markers[i].shadow.url)
                    .css('left', (marker.width() - markers[i].marker.width) / 2 + (x - markers[i].shadow.x))
                    .css('top', (marker.height() - markers[i].marker.height) / 2 + (y - markers[i].shadow.y))
                    .attr('data-width', markers[i].shadow.width)
                    .attr('data-height', markers[i].shadow.height)
                    .attr('data-x', markers[i].shadow.x)
                    .attr('data-y', markers[i].shadow.y);
            }

            if (currentId == id) {
                marker.addClass('actived');
                this.update(marker);
            }
        }
        this.scrollbar.refresh();
    };

    MarkerSelector.prototype = {
        constructor: MarkerSelector,
        destroy: function () {
            this.dropdown.off();
            this.markerlist.off();
            this.element.off();
            if (this.scrollbar) {
                this.scrollbar.destroy();
            }

            this.markerlist.remove();
            this.element.removeData('markerselector').off('.markerselector');
            if (this.component !== false) {
                this.component.off('.markerselector');
            }
            this.element.removeClass('markerselector-element');
            this.element.trigger({
                type: 'destroy'
            });
        },
        show: function () {
            this.container
                .addClass('expanded')
                .show();

            this.scrollbar.refresh();

            //$(window).on('resize.markerselector', $.proxy(this.reposition, this));
            if (this.options.inline == false) {
                //$(window.document).on({ 'mousedown.markerselector': $.proxy(this.hide, this) });
                $(window.document).on({ 'click.markerselector': $.proxy(this.hide, this) });
                if (this.container.hasClass('expanded')) {
                    this.dropdown.off('click.markerselector touchend.markerselector', this.show);
                    this.dropdown.on('click.markerselector touchend.markerselector', $.proxy(this.hide, this));
                    console.log('%cshow...dropdown', 'color: green');
                }
            }

            this.element.trigger({
                type: 'showList',
            });
            return false;
        },
        hide: function (e) {
            if (e == undefined) return;
            var target = $(e.target),
                parent = target.parent(),
                role = target.attr('role'),
                parentRole = parent.attr('role');

            console.log('%ctype: %s, target: %s, parent: %s, role: %s, parentRole: %s', 'color: blue', e.type, target[0], parent[0], role, parentRole);

            if (target.hasClass('markerselector') &&
                (this.itemRole.indexOf(role) != -1) &&
                parentRole != 'dropdown') { return; }
            //if (parentRole != 'dropdown' && !this.container.hasClass('expanded')) { return; }

            this.container
                .removeClass('expanded')
                .hide();

            //$(window).off('resize.markerselector', this.reposition);
            //$(window.document).off({ 'mousemove.markerselector': this.hide });
            this.element.trigger({
                type: 'hideList'
            });

            $(window.document).off({ 'click.markerselector': this.hide });
            this.dropdown.off('click.markerselector touchend.markerselector', this.hide);
            this.dropdown.on('click.markerselector touchend.markerselector', $.proxy(this.show, this));
            console.log('%chide...dropdown', 'color: red');

            return false;
        },
        _select: function (e) {
            var parentTarget = $(e.currentTarget), target = $(e.target),
                parentRole = parentTarget.attr('role'), role = target.attr('role');

            this.container.find('.markerselector[role="item"].actived').removeClass('actived')
            element = role == 'item' ? target : target.parent();

            this.update(element);
            element.addClass('actived');
            //this.hide();

            this.element.trigger({
                type: 'changed',
                value: this.getValue
            });
        },
        getValue: function (e) {
            switch (e) {
                case 'marker':
                    var marker = this.target.find('.markerselector[role="marker"]'),
                        shadow = this.target.find('.markerselector[role="shadow"]');
                    return {
                        id: this.element.data('id'),
                        iconUrl: marker.attr('src'),
                        iconSize: [marker.data('width'), marker.data('height')],
                        iconAnchor: [marker.data('x'), marker.data('y')],
                        shadowUrl: shadow ? shadow.attr('src') : '',
                        shadowSize: shadow ? [shadow.data('width'), shadow.data('height')] : [],
                        shadowAnchor: shadow? [shadow.data('x'), shadow.data('y')] : []
                    };
                    break;
                default:
                    return this.element.data('id');
            }
        },
        mousedown: function (e) {
            //if (!e.pageX && !e.pageY && e.originalEvent) {
            //    e.pageX = e.originalEvent.touches[0].pageX;
            //    e.pageY = e.originalEvent.touches[0].pageY;
            //}
            this._actionMouseTouch(e);
            return false;
        },
        mouseup: function () {},
        change: function(e) {},
        keyup: function (e) {},
        ajaxLoad: AjaxLoad,
        loadMarkers: LoadMarkers,
        update: function (value) {
            var elements = value.children().clone(),
                marker, shadow;
            
            this.element.data('id', value.data('id'));

            elements.each(function () {
                switch ($(this).attr('role')) {
                    case 'marker':
                        marker = $(this);
                        break;
                    case 'shadow':
                        shadow = $(this);
                        break;
                }
            });
            shadow
                .css('left', (this.target.width() - marker.data('width')) / 2 + (marker.data('x') - shadow.data('x')))
                .css('top', (this.target.height() - marker.data('height')) / 2 + (marker.data('y') - shadow.data('y')));

            this.target.html('');
            elements.appendTo(this.target);
        },
        _actionMouseTouch: function (e) {
            if (e.type = 'touchstart') {
                this.touchStart = true;
                this.touchTime = Date.now();
                console.log('start...');
            }
            if (e.type = 'touchend') {
                var end = Date.now();
                if ((end - this.touchTime) > 300) {
                    console.log('%cend...%s', 'color: green', (end - this.touchTime));
                }
                console.log('end...%s', (end - this.touchTime));
                return;
            }
            e.stopPropagation();
            e.preventDefault();

            if (e.button != 0 && e.type == 'mousedown') { return false; }

            var parentTarget = $(e.currentTarget), target = $(e.target),
                parentRole = parentTarget.attr('role'), role = target.attr('role');

            switch (parentRole) {
                case 'dropdown':
                    if (this.container && parentRole == 'dropdown') {
                        if (!this.container.hasClass('expanded')) {
                            this.show();
                        } else {
                            this.hide();
                        }
                    }
                    break;
                case 'list':
                    if (parentRole == role) { return false; }
                    this.container.find('.markerselector[role="item"].actived').removeClass('actived')
                    element = role == 'item' ? target : target.parent();

                    this.update(element);
                    element.addClass('actived');
                    //this.hide();

                    this.element.trigger({
                        type: 'changed',
                        value: this.getValue
                    });
                    break;
            }
        }
    };

    $.markerselector = MarkerSelector;
    $.fn.markerselector = function (option) {
        var markerArgs = arguments,
          rv;

        var $returnValue = this.each(function () {
            var $this = $(this),
              inst = $this.data('markerselector'),
              options = ((typeof option === 'object') ? option : {});
            if ((!inst) && (typeof option !== 'string')) {
                $this.data('markerselector', new MarkerSelector(this, options));
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
    $.fn.markerselector.constructor = MarkerSelector;
})(jQuery);
