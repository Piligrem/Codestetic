var patternTooltip = /(img:)(\S+)/;
var someNumericValue = null;
var mapControl = null;
var dialogs = new Dictionary();

(function ($) {
    // Browser supports HTML5 multiple file?
    var multipleSupport = typeof $('<input/>')[0].multiple !== 'undefined',
        isIE = /msie/i.test(navigator.userAgent);

    $.fn.customFile = function () {
        return this.each(function () {

            var $file = $(this).addClass('customfile'), // the original file input
                $wrap = $('<div class="customfile-wrap">'),
                //$input = $('<input type="text" class="customfile-filename" />'),
                // Button that will be used in non-IE browsers
                $button = $('<button type="button" class="customfile-upload">Open</button>');
                // Hack for IE
                $label = $('<label class="customfile-upload" for="' + $file[0].id + '">Open</label>');

            // Hide by shifting to the left so we
            // can still trigger events
            $file.css({
                position: 'absolute',
                left: '-9999px'
            });

            $wrap.insertAfter($file)
              .append($file, /*$input,*/ (isIE ? $label : $button));

            // Prevent focus
            $file.attr('tabIndex', -1);
            $button.attr('tabIndex', -1);

            $button.click(function () {
                $file.focus().click(); // Open dialog
            });

            $file.change(function () {

                var files = [], fileArr, filename;

                // If multiple is supported then extract
                // all filenames from the file array
                if (multipleSupport) {
                    fileArr = $file[0].files;
                    for (var i = 0, len = fileArr.length; i < len; i++) {
                        files.push(fileArr[i].name);
                    }
                    filename = files.join(', ');

                    // If not supported then just take the value
                    // and remove the path to just show the filename
                } else {
                    filename = $file.val().split('\\').pop();
                }

                //$input.val(filename) // Set the value
                //  .attr('title', filename) // Show filename in title tootlip
                //  .focus(); // Regain focus

            });

            //$input.on({
            //    blur: function () { $file.trigger('blur'); },
            //    keydown: function (e) {
            //        if (e.which === 13) { // Enter
            //            if (!isIE) { $file.trigger('click'); }
            //        } else if (e.which === 8 || e.which === 46) { // Backspace & Del
            //            // On some browsers the value is read-only
            //            // with this trick we remove the old input and add
            //            // a clean clone with all the original events attached
            //            $file.replaceWith($file = $file.clone(true));
            //            $file.trigger('change');
            //            $input.val('');
            //        } else if (e.which === 9) { // TAB
            //            return;
            //        } else { // All other keys
            //            return false;
            //        }
            //    }
            //});

        });
    };
    // Old browser fallback
    if (!multipleSupport) {
        $(document).on('change', 'input.customfile', function () {

            var $this = $(this),
                // Create a unique ID so we
                // can attach the label to the input
                uniqId = 'customfile_' + (new Date()).getTime(),
                $wrap = $this.parent(),

                // Filter empty input
                $inputs = $wrap.siblings().find('.customfile-filename')
                  .filter(function () { return !this.value }),

                $file = $('<input type="file" id="' + uniqId + '" name="' + $this.attr('name') + '"/>');

            // 1ms timeout so it runs after all other events
            // that modify the value have triggered
            setTimeout(function () {
                // Add a new input
                if ($this.val()) {
                    // Check for empty fields to prevent
                    // creating new inputs when changing files
                    if (!$inputs.length) {
                        $wrap.after($file);
                        $file.customFile();
                    }
                    // Remove and reorganize inputs
                } else {
                    $inputs.parent().remove();
                    // Move the input so it's always last on the list
                    $wrap.appendTo($wrap.parent());
                    $wrap.find('input').focus();
                }
            }, 1);

        });
    }

}(jQuery));

(function ($) {
	$.fn.onPressedTrigger = function () {
		$(".item").each(function () {
			if ($(this).hasClass('item-pressed')) {
				$(this).removeClass("item-pressed");
				$(this).find('.item-tools').removeClass("item-tools-pressed");
			}
		});
		var tools = this.find('.item-tools');
		if (!this.hasClass('item-pressed')) {
			this.addClass("item-pressed");
			tools.addClass("item-tools-pressed");
		}
		// else {
		//	this.removeClass("item-pressed");
		//	tools.removeClass("item-tools-pressed");
		//}
		//this.fadeIn('normal', function () {
		//});
	};
})(jQuery);

function create(name, attributes) {
    var el = document.createElement(name);
    if (typeof attributes == 'object') {
        for (var i in attributes) {
            el.setAttribute(i, attributes[i]);

            if (i.toLowerCase() == 'class') {
                el.className = attributes[i]; // for IE compatibility

            } else if (i.toLowerCase() == 'style') {
                el.style.cssText = attributes[i]; // for IE compatibility
            }
        }
    }
    for (var i = 2; i < arguments.length; i++) {
        var val = arguments[i];
        if (typeof val == 'string') { val = document.createTextNode(val) };
        el.appendChild(val);
    }
    return el;
};

$(document).ready(function () {
	$(function () {
		$('.item').click(function (e) {
			$(this).onPressedTrigger();
		});
		$(".icon-gear_off").bind("click", function () {
		    var deviceId = mapControl.Devices.GetDeviceId($(this));
		    if (dialogs.ContainsKey("SettingControl") && deviceId != 0) {
		        // Dialog Settings - Save
		        dialogs["SettingControl"].save = function () {
		            var form = $("#modalDialog").find("form");
		            $.ajax({
		                type: form.attr('method'),
		                async: true, // изменил - было false
		                url: form.attr('action'),
		                data: form.serialize(),
		                success: function (result) {
		                    if (result.success) {
		                        var deviceId = $("#modalDialog").find("#DeviceId").val(),
                                    device = mapControl.Devices[deviceId],
                                    prevToleranceEnable = device.Settings.ToleranceEnable,
                                    photo = $("#d-{0}".format(deviceId)).find("img");

		                        device.Position.Name = $("#modalDialog").find("#Name").val();
		                        device.Settings.IconId = $("#modalDialog").find("#IconId").val();
		                        device.Settings.PhotoId = $("#modalDialog").find("#PhotoId").val();

		                        device.Settings.ToleranceEnable = $("#modalDialog").find("#ToleranceEnable").prop("checked");
		                        device.Settings.Tolerance = $("#modalDialog").find("#Tolerance").val();

		                        device.Settings.StrokeColor = $("#modalDialog").find("#StrokeColor").val();
		                        device.Settings.FillColor = $("#modalDialog").find("#FillColor").val();
		                        device.Settings.StrokeWidth = $("#modalDialog").find("#StrokeWidth").val();

		                        device.Settings.TrackEnable = $("#modalDialog").find("#TrackEnable").prop("checked");
		                        device.Settings.TrackStrokeColor = $("#modalDialog").find("#TrackStrokeColor").val();
		                        device.Settings.TrackStrokeWidth = $("#modalDialog").find("#TrackStrokeWidth").val();

		                        device.Settings.ControlSatellites = $("#modalDialog").find("#ControlSatellites").prop("checked");
		                        device.Settings.ControlBattery = $("#modalDialog").find("#ControlBattery").prop("checked");
		                        device.Settings.ControlGSM = $("#modalDialog").find("#ControlGSM").prop("checked");
		                        device.Settings.ControlInGeoZone = $("#modalDialog").find("#ControlInGeoZone").prop("checked");
		                        device.Settings.ControlOutGeoZone = $("#modalDialog").find("#ControlOutGeoZone").prop("checked");

		                        $.dialog.dispatch("#modalDialog");

		                        if (!prevToleranceEnable && device.Settings.ToleranceEnable) {
		                            mapControl.addTolerance(device);
		                        } else if (prevToleranceEnable && device.Settings.ToleranceEnable && device.Circle.hasOwnProperty("geometry")) {
		                            device.Circle.geometry.setRadius(device.Settings.Tolerance);
		                            device.Circle.options.set("strokeColor", device.Settings.StrokeColor);
		                            device.Circle.options.set("fillColor", device.Settings.FillColor);
		                            device.Circle.options.set("strokeWidth", device.Settings.FillColor);
		                        } else if (prevToleranceEnable && !device.Settings.ToleranceEnable) {
		                            mapControl.removeTolerance(device);
		                        }

		                        if (device.Placemark.hasOwnProperty("geometry")) {
		                            mapControl.setIcon(device);
		                        }
		                        var image = getPhoto(device);
		                        photo.attr("src", image.imageUrl);
		                        photo.parent(".item-photo").attr("resource", "img:{0}".format(image.imageUrlBigSize));

		                        if (device.Settings.hasOwnProperty("Id")) {
		                            refreshIndicators(device);
		                        }
		                    }
		                },
		                error: function (xhr) {
		                    $.log("Error: {0}, Message: {1}".format(xhr.error, xhr.statusText), form.attr('action'));
		                }
		            });
		        }

		        prepareDialog("SettingControl", dialogs["SettingControl"], { deviceId: deviceId });
		        $("#modalDialog").dialog("open");
		    }
		});
		$(".icon-geozone_info").bind("click", function () {
		    var deviceId = mapControl.Devices.GetDeviceId($(this));
		    if (dialogs.ContainsKey("GeoZoneControl") && deviceId != 0) {
                // Dialog GeoZone - Save
		        dialogs["GeoZoneControl"].save = function () {
		            var form = $("#modalDialog").find("form"),
                        selected = new Array();
                        //deviceId = $("#DeviceId");
		            getSelectedElements().each(function () {
		                selected.push($(this).attr("index-id"));
		            });
		            var zones = mapControl.getAllZones("Polygon", selected);
		            $.ajax({
		                type: form.attr('method'),
		                async: true, // изменил - было false
		                url: form.attr('action'),
		                data: JSON.stringify({
		                    deviceId: deviceId,
		                    zones: zones,
		                    selected: selected
		                }),
		                async: true,
		                dataType: "json",
		                contentType: "application/json",
		                traditional: true,
		                success: function (data) {
		                    $.dialog.dispatch("#modalDialog");
		                },
		                error: function (xhr) {
		                    $.log("Error: {0}, Message: {1}".format(xhr.error, xhr.statusText), form.attr('action'));
		                }
		            });
		        }
		        prepareDialog("GeoZoneControl", dialogs["GeoZoneControl"], { deviceId: deviceId });
		        $("#modalDialog").dialog("open");
		    }
		});
		$(".icon-date").bind("click", function () {
		    var button = $(this);

		    if (!button.hasClass("item-button-on")) {
		        var deviceId = mapControl.Devices.GetDeviceId($(this));
		        if (dialogs.ContainsKey("TrackControl") && deviceId != 0) {
		            prepareDialog("TrackControl", dialogs["TrackControl"], { deviceId: deviceId });
		            $("#modalDialog").dialog("open");
		        }
		    } else {
		        mapControl.timer.stop();
		        mapControl.clearTrack();
		        button.removeClass("item-button-on");
		    }
		});
	});
	$(function () {
		$(document).tooltip({
		    items: "[resource]",
		    content: function () {
				var element = $(this);
				if (element.is("[resource]")) {
				    var resValue = element.attr("resource");
				    if ($.isArray(resValue.match(patternTooltip))) {
				        var value = resValue.replace(patternTooltip, '$2')
				        return "<img src='" + value + "' width=250px>";
				    } else {
				        var tooltip = mapControl.Devices.GetTooltip($(this));
				        element.attr("title", tooltip);
				        return tooltip;
				    }
				}
			},
			show: { effect: "puff", delay: 250 },
			hide: { effect: "puff", delay: 250 },
			track: true
		});
	});
	function prepareDialog(name, setting, parameters)
	{
	    var buttons = new Array();
	    if (setting.saveButton != undefined) {
	        buttons.push({
	            text: setting.saveButton,
	            click: setting.save
	        });
	    }
	    if (setting.applyButton != undefined) {
	        buttons.push({
	            text: setting.applyButton,
	            click: setting.apply
	        });
	    }
	    if (setting.cancelButton != undefined) {
	        buttons.push({
	            text: setting.cancelButton,
	            click: function () {
	                $.dialog.dispatch("#modalDialog");
	            }
	        });
	    }

	    $("body").append("<div id='modalDialog'></div>");

	    $("#modalDialog").dialog({
	        autoOpen: false,
	        title: setting.title,
	        width: 'auto',
	        modal: true,
	        resizable: false,
	        draggable: true,
	        //show: { effect: 'clip', duration: 500 },
	        hide: { effect: 'clip', duration: 500 },
	        position: {
	            my: "left+20 top+20", at: "left top", of: "#content-center"
	        },
	        open: function () {
	            $(this).load(dialogs[name].open, parameters);
	            $(".ui-dialog").position().left = 100;
	            //setTimeout(function () {
	            //    $(".ui-dialog").position().left = $(".ui-dialog").position().left - $(".ui-dialog").width() / 2;
	            //}, 100);
	        },
	        close: function () {
	            $(".ui-dialog").remove();
	            $("#modalDialog").remove();
	        },
	        buttons: buttons,
	    });
	};

	$(function () {
		var dataZone;

		$(".icon-geozone").click(function () {
			var element = $(this);
			if (element.hasClass("item-button-off")) {
				DialogGeoZone(element);
			}
			else {
				EndEditPolygon($(this));
			}
		});
	});
	$(function () {
		$(".item-name").click(function () {
			SetPoint($(this));
		})
	});
	initMap();
});

function initTrackUI ()
{
    //Date.parseDate = function (input, format) {
    //    return moment(input, format).toDate();
    //};
    //Date.prototype.dateFormat = function (format) {
    //    return moment(this).format(format);
    //};

    $("input[name='Show']").bind("click", function () {
        $.ajax({
            type: "POST",
            url: "/ControlPanel/GetTrack",
            data: JSON.stringify({
                deviceId: $("#DeviceId").val(),
                dateStart: moment($("#DateStart").val(), "DD.MM.YYYY HH:mm").toDate(),
                dateEnd: moment($("#DateEnd").val(), "DD.MM.YYYY HH:mm").toDate()
            }),
            async: true,
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            success: function (result) {
                var strokeColor = $("#TrackStrokeColor").val(),
                    strokeWidth = $("#TrackStrokeWidth").val(),
                    deviceId = $("#DeviceId").val(),
                    button = $("#d-" + deviceId).find("#date-button");

                button.addClass("item-button-on");
                mapControl.drawTrack({ deviceId: deviceId, strokeColor: strokeColor, strokeWidth: strokeWidth }, result.data.Position);
                $.dialog.dispatch("#modalDialog");
            },
            error: function (xhr) {
                $.log("Error: {0}, Message: {1}".format(xhr.error, xhr.statusText), "/ControlPanel/GetTrack");
            }
        });
    });
    $("input[name='Play']").bind("click", function () {
        $.ajax({
            type: "POST",
            url: "/ControlPanel/GetTrack",
            data: JSON.stringify({
                deviceId: $("#DeviceId").val(),
                dateStart: moment($("#DateStart").val(), "DD.MM.YYYY HH:mm").toDate(),
                dateEnd: moment($("#DateEnd").val(), "DD.MM.YYYY HH:mm").toDate()
            }),
            async: true,
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            success: function (result) {
                var strokeColor = $("#TrackStrokeColor").val(),
                    strokeWidth = $("#TrackStrokeWidth").val(),
                    speedPlay = $("#SpeedPlay").val(),
                    deviceId = $("#DeviceId").val(),
                    button = $("#d-" + deviceId).find("#date-button");

                button.addClass("item-button-on");
                mapControl.playTrack({ deviceId: deviceId, strokeColor: strokeColor, strokeWidth: strokeWidth, speedPlay: speedPlay }, result.data.Position);
                $.dialog.dispatch("#modalDialog");
            },
            error: function (xhr) {
                $.log("Error: {0}, Message: {1}".format(xhr.error, xhr.statusText), "/ControlPanel/GetTrack");
            }
        });
    });
}