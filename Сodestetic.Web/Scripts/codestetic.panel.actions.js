function doAction(id, action, title, context) {
    var html, include, javascript, script, commands;

    switch (action.type) {
        case 'url':
            $.ajax({
                type: "POST",
                url: "/ControlPanel/" + action.name,
                data: JSON.stringify({ id: id }),
                async: false,
                dataType: "json",
                contentType: "application/json",
                traditional: true,
                success: function (data) {
                    html = data.html;
                    include = data.include;
                    javascript = data.script;
                    commands = data.commands;
                    return data;
                },
                error: function (xhr) { }
            });
            var form = L.form({
                title: title,
                modal: true,
                action: action,
                commands: commands
            });
            form._contentContainer.innerHTML = html;
            //if (include) {
            //    for (var i = 0; i < include.length; i++) {
            //        loadScript(include[i]);
            //        //script = document.createElement('script');
            //        //script.setAttribute('src', include[i]);
            //        //script.defer = true;
            //        //form._contentContainer.appendChild(script);
            //    }
            //}
            if (javascript) {
                script = document.createElement('script');
                script.defer = true;
                script.text = javascript;
                form._contentContainer.appendChild(script);
            }
            //for (var a in actions) {
            //    form._actions[actions[a]] = action;
            //}
            form.parent = context;
            form.open(id);
            break;
        default:
            internalActions(id, action.name, title, context);
    }
}

// --- External actions ---
function userinfo(element, action, id) {
    var container = $(element._contentContainer),
        name = container.find('#Name').val(),
        photo = container.find('.photoupload[role="container"]').attr('src'),
        data = {
            id: id,
            name: name,
            photoId: container.find('.ui-photoupload').data('id'),
            markerId: container.find('.ui-markerselector').data('id')
        }
    $.ajax({
        type: "POST",
        url: "/ControlPanel/userInfo" + action,
        data: JSON.stringify(data),
        async: false,
        dataType: "json",
        contentType: "application/json",
        traditional: true,
        success: function (result) {
            if (result.success && action == 'apply') {
                var panel = element.parent,
                    objects = panel.objectPanel,
                    activeItem = objects._currentItem,
                    itemName = activeItem.querySelector('.item-name'),
                    itemPhoto = activeItem.querySelector('.item-photo');
                itemName.innerHTML = name;
                itemPhoto.src = photo;
                // ??? Need add to Device
            }
            element.close();
        },
        error: function (xhr) { }
    });
}

function tolerance(element, action, id) {
    var container = $(element._contentContainer);
    var    data = {
            id: id,
            enable: container.find('#Enable').togglebutton('getValue'),
            radius: container.find('#Radius').spinner('getValue'),
            strokeWidth: container.find('#Width').spinner('getValue'),
            fillColor: container.find('#FillColor').colorpicker('getValueHexA'),
            strokeColor: container.find('#StrokeColor').colorpicker('getValueHexA')
        };
    $.ajax({
        type: "POST",
        url: "/ControlPanel/tolerance" + action,
        data: JSON.stringify(data),
        async: false,
        dataType: "json",
        contentType: "application/json",
        traditional: true,
        success: function (result) {
            if (result.success && action == 'apply') {
                // ??? Need add to Device
            }
            element.close();
        },
        error: function (xhr) { }
    });
}

function alarm(element, action, id) {
    var container = $(element._contentContainer);
    var data = {
        id: id,
        sos: container.find('#Sos').togglebutton('getValue'),
        battery: container.find('#Battery').togglebutton('getValue'),
        batteryLevel: container.find('#MinBatteryLevel').spinner('getValue'),
        inZone: container.find('#InZone').togglebutton('getValue'),
        outZone: container.find('#OutGeoZone').togglebutton('getValue'),
    };
    $.ajax({
        type: "POST",
        url: "/ControlPanel/alarm" + action,
        data: JSON.stringify(data),
        async: false,
        dataType: "json",
        contentType: "application/json",
        traditional: true,
        success: function (result) {
            if (result.success && action == 'apply') {
                // ??? Need add to Device
            }
            element.close();
        },
        error: function (xhr) { }
    });
}

function track(element, action, id) {
    var container = $(element._contentContainer);
    var data = {
        id: id,
        enable: container.find('#Enable').togglebutton('getValue'),
        length: container.find('#Length').spinner('getValue'),
        strokeWidth: container.find('#Width').spinner('getValue'),
        strokeColor: container.find('#StrokeColor').colorpicker('getValueHexA')
    };
    $.ajax({
        type: "POST",
        url: "/ControlPanel/track" + action,
        data: JSON.stringify(data),
        async: false,
        dataType: "json",
        contentType: "application/json",
        traditional: true,
        success: function (result) {
            if (result.success && action == 'apply') {
                // ??? Need add to Device
            }
            element.close();
        },
        error: function (xhr) { }
    });
}
// --- end External actions ---

// --- Internal actions ---
function internalActions(id, func, title, context) {
    var shape,
        defaultSettings = sc.defaultSettings(),
        drawColor = defaultSettings.strokeColor.toColor('hex'),
        drawFill = defaultSettings.fillColor.toColor('hex'),
        options = {
            error: '<strong>Error:</strong> shape edges cannot cross!',
            drawError: {
                color: defaultSettings.drawErrorColor.toColor(), //'#b00b00',
                timeout: defaultSettings.drawErrorTimeout //2500
            },
            allowIntersection: false,
            repeatMode: false,
            drawShapeOptions: {
                dashArray: defaultSettings.strokeDash,
                stroke: true,
                color: drawColor.color, // '#FFBE60', //'#6495ED',
                weight: defaultSettings.strokeWidth, //2,
                opacity: drawColor.opacity, //0.5,
                fill: true,
                fillColor: drawFill.color, // null //same as color by default
                fillOpacity: drawFill.opacity //0.2,
            },
            shapeOptions: {
                stroke: true,
                color: '#6495ED',
                weight: 2,
                opacity: 0.5,
                fill: true,
                fillColor: null, //same as color by default
                fillOpacity: 0.2,
                clickable: true,
            },
            icons: {
                moveIcon: new L.DivStyledIcon({
                    style: 'background-color:' + defaultSettings.fillColor.toColor('lum'),
                    iconSize: new L.Point(12, 12),
                    iconAnchor: new L.Point(7, 7),
                    className: 'leaflet-div-icon leaflet-editing-icon leaflet-edit-move',
                }),
                resizeIcon: new L.DivStyledIcon({
                    style: 'background-color:' + defaultSettings.fillColor.toColor('lum'),
                    iconSize: new L.Point(12, 12),
                    iconAnchor: new L.Point(7, 7),
                    className: 'leaflet-div-icon leaflet-editing-icon leaflet-edit-resize',
                })
            }
        };

    //options.icons.moveIcon.style = 'background-color:' + defaultSettings.fillColor.toColor('lum');
    //options.icons.resizeIcon.style = 'background-color:' + defaultSettings.fillColor.toColor('lum');

    if (func == 'polyline') {
        options.allowIntersection = true;
        options.guidelineDistance = 7;
        options.maxGuideLineLength = 4000;
        options.metric = true; // Whether to use the metric meaurement system or imperial
        options.showLength = true; // Whether to display distance in the tooltip
        options.zIndexOffset = 2000; // This should be > than the highest z-index any map layers
        options.drawShapeOptions.fill = false;
        options.shapeOptions.fill = false;
    }

    shape = new L.Draw[func.camelCase()](this.mapControl.Map, options);
    shape.enable();

    this.mapControl.Map.on('draw:created', function (e) {
        mapControl.geoZones.addLayer(e.layer);
        shape.disable();
        mapControl.Map.off('draw:created');
    });
    return true;
}
// --- end Internal actions ---