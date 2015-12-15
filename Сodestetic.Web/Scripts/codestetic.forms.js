// Specter forms

// --- region Modal form ---
L.Form = (L.Layer || L.Class).extend({
    statics: {
        baseClass: 'form-modal'
    },
    includes: L.Mixin.Events,
    options: {
        className: '',
        title: '',
        modal: false,
        width: 'auto',
        filter: function () { return true; },
        action: false,
        commands: []
    },
    initialize: function(options) {
        this._initialize(options);
    },
    _initialize: function (options) {
        options = L.setOptions(this, options);
        var baseClass = L.Form.baseClass,
            classNames = options.className ? baseClass + ' panel-style ' + options.className : baseClass + ' panel-style',
            screenMore = document.body.offsetWidth >= 780;

        this._form = form = L.DomUtil.create('div', classNames);

        if (options.modal) {
            this._overlay = overlay = L.DomUtil.create('div', 'overlay');
            //this._overlay.style.display = 'none';
            //form.style.position = 'absolute';
            overlay.appendChild(form);
        } else { document.body.appendChild(form); }
        form.style.width = options.width != 'auto' ?  + options.width + 'px' : 'auto';

        if (L.Browser.touch) {
            L.DomUtil.addClass(form, 'ui-touch');
        }
        if (L.Browser.mobile) {
            L.DomUtil.addClass(form, 'ui-mobile');
            // --- To listen to the event screen rotation ---
            window.addEventListener("orientationchange", changeOrientation, false);
        }

        // --- Title form
        if (options.title) {
            this._title = title = L.DomUtil.create('div', baseClass + '-title', form);
            var text = L.DomUtil.create('h5', '', title);
            text.innerHTML = options.title;

            $(title).scrollText();

            var closeButtom = L.DomUtil.create('span', 'form-close-button icon-close', title);
            //closeButtom.onclick = this.close;
            //closeButtom.ontouchmove = this.close;
            L.DomEvent.on(closeButtom, 'click', this.close, this);
        }
        // --- end Title form ---

        // --- Container for content ---
        this._contentContainer = L.DomUtil.create('div', 'form-content-container', form);
        // --- end Container for content ---

        // --- Panel buttons ---
        var buttonPanel = L.DomUtil.create('div', 'form-buttons', form),
            applyCancel = L.DomUtil.create('div', 'form-buttons-standart', buttonPanel),
            apply = createButton('Common.Button.Apply', 'btn-default', '', 'apply', applyCancel),
            cancel = createButton('Common.Button.Cancel', 'btn-default', '', 'cancel', applyCancel);
        // --- end Panel buttons ---

        L.DomEvent.on(apply, 'click', this._clickOnButton, this);
        L.DomEvent.on(cancel, 'click', this._clickOnButton, this);

        // --- Listeners and actions ---
        this._listeners = {};
        this._actions = actions = options.actions;
        // --- end Listeners and actions ---

        // --- Draggable form on title bar ---
        if (!L.Browser.touch) {
            L.DomEvent.stopPropagation(form);
            var draggable = $(form).draggable({
                containment: '#map',
                handle: this._title,
            });
            //if (this._title) {
            //    $(form).draggable("option", "handle", this._title);
            //}
        }
    },
    _clickOnButton: function (e) {
        var command = getData(e.currentTarget, 'action');
        if (this.options.commands[command]) {
            window[this.options.action.name](this, this.options.commands[command], this.id);
        } else {
            this.close();
            action = 'cancel';
        }
        //this._actions[action].call(this);
        this._fire('action', command);
    },
    open: function (id) {
        var overlay = this.options.modal ? this._overlay : null,
            parent = document.body, form = this._form,
            node = L.DomUtil.get('map');

        this.id = id;
        parent.insertBefore(overlay ? overlay : form, node);
        this.show();
        return this;
    },
    close: function () {
        if (L.mobile) {
            window.removeEventListener('orientationchange', changeOrientation());
        }
        var parent = document.body,
            element = this.options.modal ? parent.querySelector('.overlay') : this._form;
        this.hide(function () { element.remove(); });
    },
    show: function () {
        var form = this._form;
        if (this.options.modal) {
            $(this._overlay).show('fade', {}, 300, function () {
                L.DomUtil.addClass(form, 'show');
            }, 300);
        } else {
            L.DomUtil.addClass(form, 'show');
        }
        var top = (Math.max(document.body.scrollHeight, this.options.modal ? this._overlay.scrollHeight : 0) - form.offsetHeight) / 2;
        form.style.top = top + 'px';
        return this;
    },
    hide: function (close) {
        if (this.options.modal) {
            var overlay = this._overlay;
            L.DomUtil.removeClass(form, 'show');
            setTimeout(function () {
                $(overlay).hide('fade', {}, 300, close);
            }, 300);
        } else {
            L.DomUtil.removeClass(form, 'show');
        }
        return this;
    },
    parent: this._parent,
    addOptionButtons: function (buttons) {
        this._buttonPanel = panelButton = L.DomUtil.create('div', 'form-buttons-options', buttonPanel);
        //apply = createButton('Common.Button.Apply', 'btn-default', '', 'apply', panelButton),
        //cancel = createButton('Common.Button.Cancel', 'btn-default', '', '', panelButton);

        return this;
    },
    addListener: function (type, listener) {
        if (typeof this._listeners[type] == "undefined") {
            this._listeners[type] = [];
        }

        this._listeners[type].push(listener);
    },
    contentContainer: this._contentContainer,
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
});
L.form = function (options) {
    return new L.Form(options);
};

// --- Photo and name ---
L.SetName = L.Form.extend({
    options: {
        className: '',
        title: '',
        modal: false,
        width: 'auto',
    },
    initialize: function (options) {
        options = L.setOptions(this, options);
        this._initialize(options);
        var container = this.contentContainer;

        //var nameField = createField('name', 'SettingControl.Name'.locale(), '3', 'SettingControl.Name.Tooltip'.locale());
    },
});
// --- end Photo and name ---

// --- Helper ---
function createButton(text, className, classIcon, action, owner) {
    var baseClass = 'btn', baseClassIcon = 'btn-icon', icon,
        button = L.DomUtil.create('button', className ? baseClass + ' ' + className : baseClass, owner);
    button.setAttribute('data-action', action);

    if (classIcon) {
        icon = L.DomUtil.create('span', classIcon ? baseClassIcon + ' ' + classIcon : baseClassIcon, button);
        L.DomUtil.addClass(button, 'btn-with-icon');
    }
    L.DomUtil.create('span', 'btn-text', button).innerHTML = text.locale();
    return button;
}
function createField(name, lable, value, title) {
    var baseClass = 'form-field',
        field = L.DomUtil.create('div', baseClass),
        lable = L.DomUtil.create('span'),
        input = L.DomUtil.create('input');

    label.innerHTML = name;
    input.setAttribute('type', 'text');
    input.setAttribute('name', name);
    input.setAttribule('value', value);
    input.innerHTML = value;

    if (title) { input.setAttribute('title', title); }

    field.appendChild(lable);
    field.appendChild(input);
    return field;
}
function changeOrientation() {
    var orientation = Math.abs(window.orientation) == 90 ? 'landscape' : 'portrait',
                        metaResult = document.head.querySelectorAll('meta'), meta,
                        form = document.body.querySelector('.' + L.Form.baseClass);

    for (var i in metaResult) {
        meta = metaResult[i];
        if (meta.getAttribute('lang')) {
            break;
        }
    }
    if (meta) {
        meta.setAttribute('orientation', orientation);
    }

    //form.styleText = 'margin:auto;position:absolute;top:0;left:0;bottom:0;right:0;';
}
// --- end Modal form ---
