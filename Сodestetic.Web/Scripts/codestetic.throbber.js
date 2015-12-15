(function (global, factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        module.exports = factory(global);
    } else if (typeof define == 'function' && define.amd) {
        define("throbber", [], function () {
            return factory(global);
        });
    } else {
        global.ThrobberAnimation = factory(global);
    }
}(window || this, function (window) {
    var document = window.document,
        M = Math,
        setTimeout = window.setTimeout,
        support = ('getContext' in document.createElement('canvas')),
        _extend = function (defaults, obj) {
            defaults = defaults || {};
            for (var i in obj) {
                defaults[i] = obj[i];
            }
            return defaults;
        },
        _animate = (function () {

            var loops = [];
            var animating = false;

            var requestFrame = (function () {
                var r = 'RequestAnimationFrame';
                return window.requestAnimationFrame ||
                  window['webkit' + r] ||
                  window['moz' + r] ||
                  window['o' + r] ||
                  window['ms' + r] ||
                  function (callback) {
                      window.setTimeout(callback, 1000 / 60);
                  };
            }());

            function tick() {

                requestFrame(tick);
                var now = +(new Date());

                for (var i = 0; i < loops.length; i++) {
                    var loop = loops[i];
                    loop.elapsed = now - loop.then;
                    if (loop.elapsed > loop.fpsInterval) {
                        loop.then = now - (loop.elapsed % loop.fpsInterval);
                        loop.fn();
                    }
                }
            }

            return function animate(fps, draw) {

                var now = +(new Date());
                loops.push({
                    fpsInterval: 1000 / fps,
                    then: now,
                    startTime: now,
                    elapsed: 0,
                    fn: draw
                });
                if (!animating) {
                    animating = true;
                    tick();
                }
            };
        }()),
        // convert any color to RGB array
        _getRGB = function (color) {
            if (!support) { return { rgb: false, alpha: 1 }; }

            var t = document.createElement('i'), rgb;

            t.style.display = 'none';
            t.style.color = color;
            document.body.appendChild(t);

            rgb = window.getComputedStyle(t, null)
                .getPropertyValue('color')
                .replace(/^rgba?\(([^\)]+)\)/, '$1').replace(/\s/g, '').split(',').splice(0, 4);

            document.body.removeChild(t);
            t = null;

            return {
                alpha: rgb.length == 4 ? rgb.pop() : 1,
                rgb: rgb
            };
        },
        // used when rotating
        _restore = function (ctx, size, back) {
            var n = back ? -2 : 2;
            ctx.translate(size / n, size / n);
        },
        // locar vars
        fade, i, l, ad, rd,
        // draw the frame
        _draw = function (alpha, o, ctx, step) {

            fade = 1 - alpha || 0;
            ad = 1; rd = -1;

            var size = o.size;

            if (o.clockwise === false) {
                ad = -1;
                rd = 1;
            }

            ctx.clearRect(0, 0, size, size);
            ctx.globalAlpha = o.alpha;
            ctx.lineWidth = o.strokewidth;

            for (i = 0; i < o.lines; i++) {

                l = i + step >= o.lines ? i - o.lines + step : i + step;

                ctx.strokeStyle = 'rgba(' + o.color.join(',') + ',' + M.max(0, ((l / o.lines) - fade)).toFixed(2) + ')';
                ctx.beginPath();

                ctx.moveTo(size / 2, size / 2 - o.padding / 2);
                ctx.lineTo(size / 2, 0);
                ctx.lineWidth = o.strokewidth;
                ctx.stroke();
                _restore(ctx, size, false);
                ctx.rotate(ad * (360 / o.lines) * M.PI / 180);
                _restore(ctx, size, true);
            }

            if (o.rotationspeed) {
                ctx.save();
                _restore(ctx, size, false);

                ctx.rotate(rd * (360 / o.lines / (20 - o.rotationspeed * 2)) * M.PI / 180); //rotate in origin
                _restore(ctx, size, true);
            }
        };

    // ThrobberAnimation constructor
    function ThrobberAnimation(options) {
        if (!(this instanceof ThrobberAnimation)) {
            return new ThrobberAnimation(options);
        }

        var elem = this.elem = document.createElement('canvas'),
            scope = this;

        if (!isNaN(options)) {
            options = { size: options };
        }

        // default options
        // note that some of these are placeholder and calculated against size if not defined
        this.o = {
            size: 34,           // diameter of loader
            rotationspeed: 6,   // rotation speed (1-10)
            clockwise: true,    // direction, set to false for counter clockwise
            color: '#fff',      // color of the spinner, can be any CSS compatible value
            fade: 300,          // duration of fadein/out when calling .start() and .stop()
            fallback: false,    // a gif fallback for non-supported browsers
            alpha: 1            // global alpha, can be defined using rgba as color or separatly
        };

        /*
        // more options, but these are calculated from size if not defined:

        fps                     // frames per second (~size)
        padding                 // diameter of clipped inner area (~size/2)
        strokewidth             // width of the lines (~size/30)
        lines                   // number of lines (~size/2+4)

        */

        // _extend options
        this.configure(options);

        // fade phase
        // 0 = idle
        // 1 = fadein
        // 2 = running
        // 3 = fadeout
        this.phase = -1;

        // references
        if (support) {
            this.ctx = elem.getContext('2d');
            elem.width = elem.height = this.o.size;
        } else if (this.o.fallback) {
            elem = this.elem = new Image();
            elem.src = this.o.fallback;
        }

        ///////////////////
        // the loop

        this.loop = (function () {
            var o = scope.o,
                alpha = 0,
                fade = 1000 / o.fade / o.fps,
                interval = 1000 / o.fps,
                step = scope.step,

                style = elem.style,
                currentStyle = elem.currentStyle,
                filter = currentStyle && currentStyle.filter || style.filter,
                ie = 'filter' in style && o.fallback && !support;

            // the canvas loop
            return function () {

                if (scope.phase == 3) {

                    // fadeout
                    alpha -= fade;
                    if (alpha <= 0) {
                        scope.phase = 0;
                    }

                }

                if (scope.phase == 1) {

                    // fadein
                    alpha += fade;
                    if (alpha >= 1) {
                        scope.phase = 2;
                    }
                }

                if (ie) {
                    style.filter = 'alpha(opacity=' + M.min(o.alpha * 100, M.max(0, Math.round(alpha * 100))) + ')';
                } else if (!support && o.fallback) {
                    style.opacity = alpha;
                } else if (support) {
                    _draw(alpha, o, scope.ctx, step);
                    step = step === 0 ? scope.o.lines : step - 1;
                }
            };
        }());
        _animate(this.o.fps, this.loop);
    }

    // ThrobberAnimation prototypes
    ThrobberAnimation.prototype = {
        constructor: ThrobberAnimation,

        // append the loader to a HTML element
        appendTo: function (elem) {
            this.elem.style.display = 'none';
            elem.appendChild(this.elem);

            return this;
        },
        // _extend options and apply calculate meassures
        configure: function (options) {
            var o = this.o, color;

            _extend(o, options || {});
            color = _getRGB(o.color);

            // do some sensible calculations if not defined
            _extend(o, _extend({
                padding: o.size / 2,
                strokewidth: M.max(1, M.min(o.size / 30, 3)),
                lines: M.min(30, o.size / 2 + 4),
                alpha: color.alpha || 1,
                fps: M.min(30, o.size + 4)
            }, options));

            // grab the rgba array
            o.color = color.rgb;

            // copy the amount of lines into steps
            this.step = o.lines;

            // double-up for retina screens
            if (!!window.devicePixelRatio) {
                // lock element into desired end size
                this.elem.style.width = o.size + 'px';
                this.elem.style.height = o.size + 'px';

                o.size *= window.devicePixelRatio;
                o.padding *= window.devicePixelRatio;
                o.strokewidth *= window.devicePixelRatio;
            }

            return this;
        },
        // starts the animation
        start: function () {
            this.elem.style.display = 'block';
            if (this.phase == -1) {
                this.loop();
            }
            this.phase = 1;

            return this;
        },
        // stops the animation
        stop: function () {
            this.phase = 3;
            return this;
        },
        toggle: function () {
            if (this.phase == 2) {
                this.stop();
            } else {
                this.start();
            }
        }
    };
    return ThrobberAnimation;
}));

;(function ($, window, document, undefined) {
    var pluginName = 'throbber';

    // element: the DOM element
    // options: the instance specific options
    function Throbber(element, options) {
        var self = this;

        this.element = element;
        var el = this.el = $(element),
            opts = this.options = options,
            throbber = this.throbber = null,
            throbberContent = this.throbberContent = null;

        this.visible = false;

        this.init = function () {
            throbbers.push(self);
            if (opts.show) {
                self.show();
            }
        };

        this.initialized = false;
        this.init();
    }

    Throbber.prototype = {
        _reposition: function () {
            var self = this,
                size = null;
            if (self.throbber.hasClass("global")) {
                size = {
                    left: (self.el.width() - self.throbberContent.outerWidth()) / 2,
                    top: (self.el.height() - self.throbberContent.outerHeight()) / 2
                }
            }
            else {
                // not global
                var pos = self.el.position();
                self.throbber.find(".throbber-overlay").css({
                    left: pos.left + parseInt(self.el.css("margin-left")),
                    top: pos.top + parseInt(self.el.css("margin-top")),
                    width: self.el.outerWidth(),
                    height: self.el.outerHeight()
                });
                size = {
                    left: pos.left + ((self.el.outerWidth() - self.throbberContent.outerWidth()) / 2),
                    top: pos.top + ((self.el.outerHeight() - self.throbberContent.outerHeight()) / 2)
                }
            }
            self.throbberContent.css(size);
        },
        show: function (o) {

            if (this.visible)
                return;

            var self = this,
                opts = $.extend({}, this.options, o);

            // create throbber if not avail
            if (!self.throbber) {
                self.throbber = $('<div class="throbber"><div class="throbber-overlay"><span class="throbber-animation"></span></div><div class="throbber-content"></div></div>')
                                 .addClass(opts.cssClass)
                                 .addClass(opts.small ? "small" : "large")
                                 .appendTo(opts._global ? 'body' : self.el);
                if (opts.white) {
                    self.throbber.addClass("white");
                }
                if (opts._global) {
                    self.throbber.addClass("global");
                }

                self.throbberContent = self.throbber.find(".throbber-content");
                self.initialized = true;
            }

            // set text and reposition
            self.throbber.css({ visibility: 'hidden', display: 'block' });
            self.throbberContent.html(opts.message);
            self._reposition();
            self.throbber.css({ visibility: 'visible', display: 'none' });
            //throb.start();

            var show = function () {
                if (_.isFunction(opts.callback)) {
                    opts.callback.apply(this);
                }
                if (!self.visible) {
                    // could have been set to false in 'hide'.
                    // this can happen in very short running processes.
                    //throb.stop();
                    self.hide();
                }
            }

            self.visible = true;
            opts.speed
                ? self.throbber.delay(opts.delay).fadeIn(opts.speed, show)
                : self.throbber.delay(opts.delay).fadeIn(0, show);

            if (opts.timeout) {
                window.setTimeout(self.hide, opts.timeout + opts.delay);
            }
            ThrobberAnimation(opts).appendTo($(".throbber .throbber-content")[0]).start();
        },
        hide: function (immediately) {
            var self = this, opts = this.options;

            if (self.throbber && self.visible) {

                var hide = function () {
                    // [...] 
                }

                self.visible = false;

                defaults.speed && _.isFalse(immediately)
                    ? self.throbber.stop(true).fadeOut(opts.speed, hide)
                    : self.throbber.stop(true).hide(0, hide);
            }

        }
    }

    // A really lightweight plugin wrapper around the constructor, 
    // preventing against multiple instantiations
    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, pluginName)) {
                options = $.extend({}, $[pluginName].defaults, options);
                $.data(this, pluginName, new Throbber(this, options));
            }
        });
    }

    // global (window)-throbber
    var globalThrobber = null,
        throbbers = [],
        defaults = {
            delay: 0,
            speed: 150,
            timeout: 0,
            white: false,
            small: false,
            message: "Please wait...",
            cssClass: null,
            callback: null,
            show: true,
            // internal
            _global: false
        };

    // global resize event
    $(window).on('resize.throbber', function () {
        // resize all active/visible throbbers
        $.each(throbbers, function (i, throbber) {
            if (throbber.initialized && throbber.visible) {
                throbber._reposition();
            }
        })
    });

    $[pluginName] = {
        // the global, default plugin options
        defaults: defaults,

        // options: a message string || options object
        show: function (options) {
            var opts = $.extend(defaults, _.isString(options) ? { message: options } : options, { show: false, _global: true });

            if (!globalThrobber) {
                globalThrobber = $(window).throbber(opts).data("throbber");
            }

            globalThrobber.show(opts);

        },
        hide: function () {
            if (globalThrobber) {
                globalThrobber.hide();
            }
        }
    } // $.throbber
})(jQuery, window, document);
