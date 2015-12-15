////name: Core
//(function () {
//	this.MooTools = {
//		version: '1.5.1',
//		build: '0542c135fdeb7feed7d9917e01447a408f22c876'
//	};
//	// typeOf, instanceOf
//	var typeOf = this.typeOf = function (item) {
//		if (item === null) return 'null';
//		if (item.$family !== null) return item.$family();

//		if (item.nodeName) {
//			if (item.nodeType === 1) return 'element';
//			if (item.nodeType === 3) return (/\S/).test(item.nodeValue) ? 'textnode' : 'whitespace';
//		} else if (typeof item.length === 'number') {
//			if ('callee' in item) return 'arguments';
//			if ('item' in item) return 'collection';
//		}

//		return typeof item;
//	};
//	var instanceOf = this.instanceOf = function (item, object) {
//		if (item === null) return false;
//		var constructor = item.$constructor || item.constructor;
//		while (constructor) {
//			if (constructor === object) return true;
//			constructor = constructor.parent;
//		}
//		/*<ltIE8>*/
//		if (!item.hasOwnProperty) return false;
//		/*</ltIE8>*/
//		return item instanceof object;
//	};

//	// Function overloading
//	var Function = this.Function;

	var enumerables = true;
	for (var i in { toString: 1 }) { enumerables = null; }
	if (enumerables) enumerables = ['hasOwnProperty', 'valueOf', 'isPrototypeOf', 'propertyIsEnumerable', 'toLocaleString', 'toString', 'constructor'];

	Function.prototype.overloadSetter = function (usePlural) {
		var self = this;
		return function (a, b) {
			if (a === null) return this;
			if (usePlural || typeof a !== 'string') {
			    for (var k in a) { self.call(this, k, a[k]); }

			    if (enumerables) {
			        for (var i = enumerables.length; i--;) {
			            k = enumerables[i];
			            if (a.hasOwnProperty(k)) self.call(this, k, a[k]);
			        }
			    }
			} else { self.call(this, a, b); }
			return this;
		};
	};

	var Dictionary = (function () {
	    function Dictionary() {
	        if (!(this instanceof Dictionary))
	            return new Dictionary();
	    }

	    Dictionary.prototype.Count = function () {
	        var key,
                count = 0;

	        for (key in this) {
	            if (this.hasOwnProperty(key))
	                count += 1;
	        }
	        return count;
	    };

	    Dictionary.prototype.Keys = function () {
	        var key,
                keys = [];

	        for (key in this) {
	            if (this.hasOwnProperty(key))
	                keys.push(key);
	        }
	        return keys;
	    };

	    Dictionary.prototype.Values = function () {
	        var key,
                values = [];

	        for (key in this) {
	            if (this.hasOwnProperty(key))
	                values.push(this[key]);
	        }
	        return values;
	    };

	    Dictionary.prototype.KeyValuePairs = function () {
	        var key,
                keyValuePairs = [];

	        for (key in this) {
	            if (this.hasOwnProperty(key))
	                keyValuePairs.push({
	                    Key: key,
	                    Value: this[key]
	                });
	        }
	        return keyValuePairs;
	    };

	    Dictionary.prototype.Add = function (key, value) {
	        this[key] = value;
	    }

	    Dictionary.prototype.Clear = function () {
	        var key,
                dummy;

	        for (key in this) {
	            if (this.hasOwnProperty(key))
	                dummy = delete this[key];
	        }
	    }

	    Dictionary.prototype.ContainsKey = function (key) {
	        return this.hasOwnProperty(key);
	    }

	    Dictionary.prototype.ContainsValue = function (value) {
	        var key;

	        for (key in this) {
	            if (this.hasOwnProperty(key) && this[key] === value)
	                return true;
	        }
	        return false;
	    }

	    Dictionary.prototype.Remove = function (key) {
	        var dummy;

	        if (this.hasOwnProperty(key)) {
	            dummy = delete this[key];
	            return true;
	        } else
	            return false;
	    }

	    return Dictionary;
	}());

	jQuery.form = {
	    set: function (form, name, values) {
	        var selector = "." + form + "[name=" + name + "]";
	        if ($(selector).is(':checkbox')) {
	            if (!$.isArray(values)) values = new Array(values);
	            $(selector).removeAttr("checked");
	            for (var i = 0; i < values.length; i++)
	                $(selector + "[value='" + values[i] + "']").attr("checked", "checked");
	            return;
	        }
	        if ($(selector).is(':radio')) {
	            $(selector).removeAttr("checked");
	            $(selector + "[value='" + values + "']").attr("checked", "checked"); return;
	        }
	        $(selector).val(values);
	        return;
	    },
	    save: function (params)//name,url,key,callback,params,multi
	    {
	        var p = {
	            name: 'form',
	            url: '',
	            key: null,
	            test: false,
	            callback: function () { },
	            params: {},
	            multi: false
	        }
	        p = $.extend(p, params);
	        var form = $.form.get(p.name, false, p.multi);
	        if (p.key != null) {
	            var c = {};
	            c[p.key] = form;
	            form = c;
	        }
	        form = $.toJSON(form);
	        if (p.params == null) p.params = { p: form };
	        else p.params.p = form;
	        $.post(p.url, p.params, function (data) {
	            if (p.test)
	                alert(data);
	            data = $.parseJSON(data);
	            if (data.code == 1)
	                $.form.ok(p.name, data.descr, data.title);
	            else $.form.error(p.name, data.descr, data.title);
	            p.callback(data);
	        }, 'html');
	    },
	    get: function (form, json, multi) {
	        if (multi == null) multi = false;
	        if (json == null) json = true;
	        if (form == null || form.length == 0) {
	            if (!json) return {};
	            else return $.toJSON({});
	        }
	        var selector = "input." + form + ":radio:checked,input." + form + ":checkbox:checked,input." + form + ":text,input." + form + ":hidden,input." + form + ":file,input." + form + ":password,textarea." + form + ",select." + form;
	        var inputs = $(selector);
	        var values = {};
	        $.each(inputs, function () {
	            var name = $(this).attr("name");
	            var value = $(this).val();

	            if (($.isArray(value) && value[0] == null) || value == null)
	                return;
	            if ($(this).is(':file')) {
	                var name = $(this).attr("name");

	                var key = $(this).attr("key");
	                var s = "input[type=hidden][key=" + key + "]";
	                var v = {};
	                v.directory = $(s + "[name=" + name + "_directory]").attr("value");
	                v.serverFileName = $(s + "[name=" + name + "_serverFileName]").attr("value");
	                v.path = $(s + "[name=" + name + "_path]").attr("value");
	                v.size = $(s + "[name=" + name + "_size]").attr("value");
	                v.isUploaded = $(s + "[name=" + name + "_isUploaded]").attr("value");
	                if (v.isUploaded == false || v.isUploaded == 0 || v.isUploaded == "0") return;
	                if (!multi && !$.form.intervals[key].wasMulti) values[name] = v;
	                else {
	                    if (values[name] == null) values[name] = new Array();
	                    values[name][values[name].length] = v;
	                }
	                return;
	            }
	            if (multi || $(this).is(':checkbox')) {
	                if ($(this).is(":checkbox") && !$(this).attr("checked")) return;
	                if (values[name] == null) values[name] = new Array();
	                values[name][values[name].length] = value;
	            }
	            else {
	                values[name] = value;
	            }
	        });

	        if (json == false)
	            return values;
	        else
	            return $.toJSON(values);
	    },
	    addInputFile: function (form, name, container) {
	        var key = $('.' + form + '[name=' + name + ']').attr("key");

	        var p = {};
	        var oldparams = $.form.intervals[key];
	        p.upload = oldparams.upload;
	        p.progress = oldparams.progress;
	        p.serverFileName = oldparams.serverFileName_def;
	        p.directory = oldparams.directory;
	        p.autoUpload = oldparams.autoUpload;
	        p.multi = false;
	        p.wasMulti = oldparams.wasMulti;
	        p.onSelect = oldparams.onSelect;
	        p.onStart = oldparams.onStart;
	        p.onComplete = oldparams.onComplete;

	        $(container).append("<input type=file name=" + name + " class=" + form + ">");
	        $.form.makeUpload('.' + form + '[name=' + name + ']', p, 'multi');
	    },
	    uploadChange: function (key, param, value) {
	        $("input[key='" + key + "'][name='" + param + "']").attr("value", value);
	    },
	    makeUpload: function (selector, params, add) {

	        var elements = $(selector);
	        $.each(elements, function () {
	            var p = {
	                upload: "/upload.php?action=uploadFile",
	                progress: "/upload.php?action=progress",
	                serverFileName: "%real%",
	                directory: "",
	                userParam: "",
	                autoUpload: false,
	                multi: false,
	                wasMulti: false,
	                onSelect: function () { },
	                onStart: function () { },
	                onComplete: function () { },
	                name: $(selector).attr("name")
	            }
	            if ($(this).attr("key") != null) return;
	            var date = new Date();
	            var key = date.getMilliseconds().toString() + date.getMinutes().toString() + date.getSeconds().toString() + Math.round(Math.random() * (1000000 - 0)).toString();
	            p = $.extend(p, params);
	            if (add == null)
	                p.wasMulti = p.multi;
	            else (p.wasMulti = true);
	            p.selector = "[type=file][key=" + key + "]";
	            p.key = key;
	            p.serverFileName_def = p.serverFileName;
	            $(this).attr("key", key).change(function () {
	                if ($("#submit_" + key).length == 0) {
	                    $.form.intervals[key].onSelect($.form.intervals[key]);
	                    $(this).after("<input type=submit value='Загрузить' id=submit_" + key + ">");
	                    if ($.form.intervals[key].autoUpload == true)
	                        $("#submit_" + key).click();
	                }
	            });
	            $(this).wrap("<form key='" + key + "' onsubmit=\"return $.form.uploadSelectedFile(this)\" name=wfUpload_" + key + " action='" + p.upload + "' target=iframe_" + key + " enctype='multipart/form-data' method=post></form>")
	            $(this).parent().append("<input type=hidden name=key value='" + key + "'>");
	            $(this).before('<input type="hidden" name="UPLOAD_IDENTIFIER" value="' + key + '">');
	            var span = $(this).parent();
	            var html =
                    "<input type=hidden name='" + p.name + "_directory' key='" + key + "' value='" + p.directory + "'>" +
                    "<input type=hidden name='" + p.name + "_serverFileName' key='" + key + "' value='" + p.serverFileName + "'>" +
                    "<input type=hidden name='" + p.name + "_serverFileName_def' key='" + key + "' value='" + p.serverFileName_def + "'>" +
                    "<input type=hidden name='" + p.name + "_path' key='" + key + "' value=''>" +
                    "<input type=hidden name='inputName' key='" + key + "' value='" + p.name + "'>" +
                    "<input type=hidden name='userParam' key='" + key + "' value='" + p.userParam + "'>" +
                    "<input type=hidden name='" + p.name + "_isUploaded' key='" + key + "' value='0'>" +
                    "<input type=hidden name='" + p.name + "_size' key='" + key + "' value=''>" +
                    "<span uploaded=0 class=uploadDescr style='display:none' key='" + key + "'></span>" +
                    "<iframe style='display:none;' onLoad=$.form.uploadComplete('" + key + "') name=iframe_" + key + " id=iframe_" + key + "></iframe>";
	            $(span).append(html);
	            if ($.form.intervals[key] == null)
	                $.form.intervals[key] = p;
	        });
	        if (params.multi == true) {
	            var l = elements.length;
	            if (l > 0 && $('#more_files_' + $(elements[l - 1]).attr("key")).length == 0)
	                $(elements[l - 1]).parent('form').after("<br><input type=submit value='Добавить еще один файл' onclick=$.form.addInputFile('" + $(elements[l - 1]).attr("class") + "','" + $(elements[l - 1]).attr("name") + "','#more_files_" + $(elements[l - 1]).attr("key") + "')><br>").after('<span id=more_files_' + $(elements[l - 1]).attr("key") + '></span>');
	        }
	    },
	    uploadSelectedFile: function (s, params) {
	        var p = {
	            width: 100,
	            height: 15
	        }
	        p = $.extend(p, params);
	        var key = $(s).attr("key");
	        $("#submit_" + key).attr("disabled", "disabled");
	        var span = $(s).children(".uploadDescr");
	        var style = { display: 'none', margin: "10px" };
	        $(span).html("<div class=progressbar></div> <span style='text-align:right;' class=uploadedFileDetailInfo>Загрузка началась. Пожалуйста, подождите...</span>").css(style);
	        var bar = $(span).children('div.progressbar');
	        bar.progressbar({ value: 0 });
	        //alert($.form.intervals[key].selector);
	        var fname = $($.form.intervals[key].selector).attr("value");
	        fname = fname.split('\\');
	        $.form.intervals[key].realFileName = fname[fname.length - 1];
	        $.form.intervals[key].key = key;
	        $.form.intervals[key].bar = bar;
	        $.form.intervals[key].span = span;
	        $($.form.intervals[key].span).attr("uploaded", 0);
	        $.form.intervals[key].onStart($.form.intervals[key]);
	        //$.form.uploadProgress(key);
	        $.form.intervals[key].interval = setInterval('$.form.uploadProgress("' + key + '")', 1000);
	    },
	    uploadProgress: function (key) {
	        $.ajax({
	            error: function (XMLHttpRequest, textStatus, errorThrown) {
	            },
	            start: function () { },
	            beforeSend: function (request) {
	                request.setRequestHeader('Cookie', document.cookie);
	            },
	            url: $.form.intervals[key].progress,
	            data: {
	                key: key,
	                file: $.form.intervals[key].directory + "/" + $.form.intervals[key].realFileName
	            },
	            complete: function () { },
	            dataType: 'html',
	            type: 'post',
	            async: true, // изменил - было false
	            success: function (data) {
	                data = $.parseJSON(data);
	                if (data.result == 1) {
	                    var timelast = data.time_last;
	                    var total = data.bytes_total;
	                    var speed = data.speed_average;
	                    var bytes = data.bytes_uploaded;
	                    var eta = data.est_sec;
	                    var min = Math.round(eta / 60);
	                    var sec = eta - min * 60;
	                    if (min == 0) { var time = sec + " сек." } else { var time = min + " мин." + sec + " сек." }
	                    var speeds = $.form.speeds(speed);
	                    var percents = Math.round(bytes * 100 / total);
	                    $.form.intervals[key].size = total;
	                    $($.form.intervals[key].span).children('.uploadedFileDetailInfo').html("<b>" + percents + "%</b>, <i>скорость:</i> <b>" + speeds + "</b>, <i>загружено</i> <b>" + $.form.fsize(bytes) + "</b> <i>из</i> <b>" + $.form.fsize(total) + "</b>");
	                    $.form.intervals[key].bar.progressbar('value', percents);
	                }
	                if (data.result == -1)
	                    $.form.intervals[key].size = data.size;
	                if (data.result == 0) {
	                    $.form.intervals[key].size = data.size;
	                    //alert(data.size);
	                    if ($($.form.intervals[key].span).attr("uploaded") == 0)
	                        $($.form.intervals[key].span).html("Сервер не поддерживает отображение процесса загрузки. Подождите завершения загрузки файла...");
	                }

	                $($.form.intervals[key].span).slideDown();
	            }
	        });
	    },
	    uploadCompleteAfterTimer: function (key) {
	        if ($.form.intervals[key].realFileName == null) return;
	        clearInterval($.form.intervals[key].interval);
	        var size = "false";
	        if ($.form.intervals[key].size != null) size = $.form.intervals[key].size;
	        var serverFileName = $.form.str_replace('%real%', $.form.intervals[key].realFileName, $.form.intervals[key].serverFileName);
	        var value = $.form.intervals[key].directory + "/" + serverFileName;
	        $("input[key=" + key + "][name=" + $.form.intervals[key].name + "_path]").attr("value", value);
	        $("input[key=" + key + "][name=" + $.form.intervals[key].name + "_isUploaded]").attr("value", 1);
	        $("input[key=" + key + "][name=" + $.form.intervals[key].name + "_size]").attr("value", size);
	        $("input[key=" + key + "][name=" + $.form.intervals[key].name + "_serverFileName]").attr("value", serverFileName);
	        $($.form.intervals[key].span).attr("uploaded", 1);
	        $("#submit_" + key).remove();
	        $(".uploadDescr[key=" + key + "]").html("Загрузка завершена!!!");
	        var opt = {
	            path: value,
	            size: size,
	            name: $.form.intervals[key].realFileName
	        };
	        $.form.intervals[key].onComplete(opt);
	    },
	    uploadComplete: function (key) {
	        setTimeout("$.form.uploadCompleteAfterTimer('" + key + "')", 500);
	    },
	    intervals: {},
	    "fsize": function (x) {
	        x = Math.round(x / 1024);
	        if (x < 1000) {
	            return x + " " + "Кб";
	        }
	        x = Math.round(x * 100 / 1024) / 100;
	        return x + " " + "Мб";
	    },
	    "speeds": function (x) {
	        x = Math.round(x / 1024);
	        if (x < 1000) {
	            return x + " " + "Кб/сек";
	        }
	        x = Math.round(x * 100 / 1024) / 100;
	        return x + " " + "Мб/сек";
	    },
	    "str_replace": function (search, replace, subject, count) {
	        var i = 0, j = 0, temp = '', repl = '', sl = 0, fl = 0,
                    f = [].concat(search),
                    r = [].concat(replace),
                    s = subject,
                    ra = r instanceof Array, sa = s instanceof Array;
	        s = [].concat(s);
	        if (count) {
	            this.window[count] = 0;
	        }

	        for (i = 0, sl = s.length; i < sl; i++) {
	            if (s[i] === '') {
	                continue;
	            }
	            for (j = 0, fl = f.length; j < fl; j++) {
	                temp = s[i] + '';
	                repl = ra ? (r[j] !== undefined ? r[j] : '') : r[0];
	                s[i] = (temp).split(f[j]).join(repl);
	                if (count && s[i] !== temp) {
	                    this.window[count] += (temp.length - s[i].length) / f[j].length;
	                }
	            }
	        }
	        return sa ? s : s[0];
	    }
	}

	jQuery.dialog = {
	    dispatch: function (dialog) {
	        $(dialog).dialog("close");
	        $(".ui-dialog").remove();
	        $("#modalDialog").remove();
	        //$(dialog).html("");
	    }
	}

	// region Helpers
	function getPhoto(device) {
	    var src = $.ajax({
	        type: "POST",
	        url: "/Media/GetPhotoById",
	        data: JSON.stringify({ photoId: device.Settings.PhotoId }),
	        async: false,
	        dataType: "json",
	        contentType: "application/json",
	        traditional: true,
	        success: function (data) { return data; },
	        error: function (xhr) { $.log("Error: {0}, Message: {1}".format(xhr.error, xhr.statusText), "/Media/GetPhotoById"); }
	    });
	    var result = {};
	    result.imageUrl = src.responseJSON.imageUrl;
	    result.imageUrlBigSize = src.responseJSON.imageUrlBigSize;
	    return result;
	}
	function refreshIndicators(device) {
	    var itemsInfo = $("#d-{0}".format(device.Settings.DeviceId)).find(".item-tools").find(".item-tools-info").children(),
            iconSatellites,
            iconGsm,
            iconBattery;
	    itemsInfo.each(function () {
	        var iconName = $(this).attr("class").match(/(icon-)(\S+)(_\S+)/);
	        if (iconName.length > 2) {
	            switch (iconName[2]) {
	                case "satellites":
	                    if (device.Settings.ControlSatellites) {
	                        $(this).attr("class", "icon-satellites_{0}".format(device.Indicator.Satellites.IconValue));
	                    } else {
	                        $(this).attr("class", "icon-satellites_00");
	                    }
	                    break;
	                case "battery":
	                    if (device.Settings.ControlBattery) {
	                        $(this).attr("class", "icon-battery_{0}".format(device.Indicator.Battery.IconValue));
	                    } else {
	                        $(this).attr("class", "icon-battery_00");
	                    }
	                    break;
	                case "gsm":
	                    if (device.Settings.ControlGSM) {
	                        $(this).attr("class", "icon-gsm_{0}".format(device.Indicator.GSM.IconValue));
	                    } else {
	                        $(this).attr("class", "icon-gsm_00");
	                    }
	                    break;
	            }
	        }
	    });
	    var a = 1;
	}
    // end region Helpers


//	Function.prototype.overloadGetter = function (usePlural) {
//		var self = this;
//		return function (a) {
//			var args, result;
//			if (typeof a !== 'string') args = a;
//			else if (arguments.length > 1) args = arguments;
//			else if (usePlural) args = [a];
//			if (args) {
//				result = {};
//				for (var i = 0; i < args.length; i++) { result[args[i]] = self.call(this, args[i]); }
//			} else {
//				result = self.call(this, a);
//			}
//			return result;
//		};
//	};
//	Function.prototype.extend = function (key, value) {
//		this[key] = value;
//	}.overloadSetter();
	Function.prototype.implement = function (key, value) {
		this.prototype[key] = value;
	}.overloadSetter();

//	// From
//	var slice = Array.prototype.slice;
//	Function.from = function (item) {
//		return (typeOf(item) === 'function') ? item : function () {
//			return item;
//		};
//	};
//	Array.from = function (item) {
//		if (item === null) return [];
//		return (Type.isEnumerable(item) && typeof item !== 'string') ? (typeOf(item) === 'array') ? item : slice.call(item) : [item];
//	};
//	Number.from = function (item) {
//		var number = parseFloat(item);
//		return isFinite(number) ? number : null;
//	};
//	String.from = function (item) {
//		return item + '';
//	};
//	// hide, protect
//	Function.implement({
//		hide: function () {
//			this.$hidden = true;
//			return this;
//		},
//		protect: function () {
//			this.$protected = true;
//			return this;
//		}
//	});

//	// Type
//	var Type = this.Type = function (name, object) {
//		if (name) {
//			var lower = name.toLowerCase();
//			var typeCheck = function (item) {
//				return (typeOf(item) === lower);
//			};

//			Type['is' + name] = typeCheck;
//			if (object !== null) {
//				object.prototype.$family = (function () {
//					return lower;
//				}).hide();

//			}
//		}

//		if (object === null) return null;

//		object.extend(this);
//		object.$constructor = Type;
//		object.prototype.$constructor = object;

//		return object;
//	};
//	var toString = Object.prototype.toString;
//	Type.isEnumerable = function (item) {
//		return (item !== null && typeof item.length === 'number' && toString.call(item) !== '[object Function]');
//	};

//	var hooks = {};
//	var hooksOf = function (object) {
//		var type = typeOf(object.prototype);
//		return hooks[type] || (hooks[type] = []);
//	};

//	var implement = function (name, method) {
//		if (method && method.$hidden) return;

//		var hooks = hooksOf(this);

//		for (var i = 0; i < hooks.length; i++) {
//			var hook = hooks[i];
//			if (typeOf(hook) === 'type') { implement.call(hook, name, method); } else { hook.call(this, name, method); }
//		}

//		var previous = this.prototype[name];
//		if (previous === null || !previous.$protected) { this.prototype[name] = method; }

//		if (this[name] === null && typeOf(method) === 'function') {
//		    extend.call(this, name, function (item) {
//		        return method.apply(item, slice.call(arguments, 1));
//		    });
//		}
//	};
//	var extend = function (name, method) {
//		if (method && method.$hidden) return;
//		var previous = this[name];
//		if (previous == null || !previous.$protected) this[name] = method;
//	};

//	Type.implement({
//		implement: implement.overloadSetter(),
//		extend: extend.overloadSetter(),
//		alias: function (name, existing) {
//			implement.call(this, name, this.prototype[existing]);
//		}.overloadSetter(),
//		mirror: function (hook) {
//			hooksOf(this).push(hook);
//			return this;
//		}
//	});

//	new Type('Type', Type);

//	// Default Types
//	var force = function (name, object, methods) {
//		var isType = (object !== Object),
//			prototype = object.prototype;

//		if (isType) object = new Type(name, object);

//		for (var i = 0, l = methods.length; i < l; i++) {
//			var key = methods[i],
//				generic = object[key],
//				proto = prototype[key];

//			if (generic) generic.protect();
//			if (isType && proto) object.implement(key, proto.protect());
//		}

//		if (isType) {
//			var methodsEnumerable = prototype.propertyIsEnumerable(methods[0]);
//			object.forEachMethod = function (fn) {
//				if (!methodsEnumerable) for (var i = 0, l = methods.length; i < l; i++) {
//					fn.call(prototype, prototype[methods[i]], methods[i]);
//				}
//				for (var key in prototype) { fn.call(prototype, prototype[key], key); }
//			};
//		}

//		return force;
//	};
//	force('String', String, [
//		'charAt', 'charCodeAt', 'concat', 'contains', 'indexOf', 'lastIndexOf', 'match', 'quote', 'replace', 'search',
//		'slice', 'split', 'substr', 'substring', 'trim', 'toLowerCase', 'toUpperCase'
//	])('Array', Array, [
//		'pop', 'push', 'reverse', 'shift', 'sort', 'splice', 'unshift', 'concat', 'join', 'slice',
//		'indexOf', 'lastIndexOf', 'filter', 'forEach', 'every', 'map', 'some', 'reduce', 'reduceRight'
//	])('Number', Number, [
//		'toExponential', 'toFixed', 'toLocaleString', 'toPrecision'
//	])('Function', Function, [
//		'apply', 'call', 'bind'
//	])('RegExp', RegExp, [
//		'exec', 'test'
//	])('Object', Object, [
//		'create', 'defineProperty', 'defineProperties', 'keys',
//		'getPrototypeOf', 'getOwnPropertyDescriptor', 'getOwnPropertyNames',
//		'preventExtensions', 'isExtensible', 'seal', 'isSealed', 'freeze', 'isFrozen'
//	])('Date', Date, ['now']);

//	Object.extend = extend.overloadSetter();
//	Date.extend('now', function () {
//		return +(new Date);
//	});

//	new Type('Boolean', Boolean);

//	// fixes NaN returning as Number
//	Number.prototype.$family = function () {
//		return isFinite(this) ? 'number' : 'null';
//	}.hide();

//	// Number.random
//	Number.extend('random', function (min, max) {
//		return Math.floor(Math.random() * (max - min + 1) + min);
//	});

//	// forEach, each
//	var hasOwnProperty = Object.prototype.hasOwnProperty;
//	Object.extend('forEach', function (object, fn, bind) {
//		for (var key in object) {
//			if (hasOwnProperty.call(object, key)) fn.call(bind, object[key], key, object);
//		}
//	});

//	Object.each = Object.forEach;
//	Array.implement({
//		/*<!ES5>*/
//		forEach: function (fn, bind) {
//			for (var i = 0, l = this.length; i < l; i++) {
//				if (i in this) fn.call(bind, this[i], i, this);
//			}
//		},
//		/*</!ES5>*/
//		each: function (fn, bind) {
//			Array.forEach(this, fn, bind);
//			return this;
//		}
//	});

//	// Array & Object cloning, Object merging and appending
//	var cloneOf = function (item) {
//		switch (typeOf(item)) {
//			case 'array': return item.clone();
//			case 'object': return Object.clone(item);
//			default: return item;
//		}
//	};
//	Array.implement('clone', function () {
//		var i = this.length, clone = new Array(i);
//		while (i--) clone[i] = cloneOf(this[i]);
//		return clone;
//	});
//	var mergeOne = function (source, key, current) {
//		switch (typeOf(current)) {
//			case 'object':
//			    if (typeOf(source[key]) === 'object') { Object.merge(source[key], current); } else { source[key] = Object.clone(current); }
//				break;
//			case 'array': source[key] = current.clone(); break;
//			default: source[key] = current;
//		}
//		return source;
//	};

//	Object.extend({
//		merge: function (source, k, v) {
//			if (typeOf(k) === 'string') return mergeOne(source, k, v);
//			for (var i = 1, l = arguments.length; i < l; i++) {
//				var object = arguments[i];
//				for (var key in object) { mergeOne(source, key, object[key]); }
//			}
//			return source;
//		},

//		clone: function (object) {
//			var clone = {};
//			for (var key in object) { clone[key] = cloneOf(object[key]); }
//			return clone;
//		},

//		append: function (original) {
//			for (var i = 1, l = arguments.length; i < l; i++) {
//				var extended = arguments[i] || {};
//				for (var key in extended) { original[key] = extended[key]; }
//			}
//			return original;
//		}
//	});

//	// Object-less types
//	['Object', 'WhiteSpace', 'TextNode', 'Collection', 'Arguments'].each(function (name) { new Type(name); });

//	// Unique ID
//	var UID = Date.now();

//	String.extend('uniqueID', function () { return (UID++).toString(36); });
//})();

///*---
//name: Array
//description: Contains Array Prototypes like each, contains, and erase.
//license: MIT-style license.
//requires: [Type]
//provides: Array
//*/
//Array.implement({
//	/*<!ES5>*/
//	every: function (fn, bind) {
//		for (var i = 0, l = this.length >>> 0; i < l; i++) {
//			if ((i in this) && !fn.call(bind, this[i], i, this)) return false;
//		}
//		return true;
//	},
//	filter: function (fn, bind) {
//		var results = [];
//		for (var value, i = 0, l = this.length >>> 0; i < l; i++) {
//		    if (i in this) {
//		        value = this[i];
//		        if (fn.call(bind, value, i, this)) results.push(value);
//		    }
//		}
//		return results;
//	},
//	indexOf: function (item, from) {
//		var length = this.length >>> 0;
//		for (var i = (from < 0) ? Math.max(0, length + from) : from || 0; i < length; i++) {
//			if (this[i] === item) return i;
//		}
//		return -1;
//	},
//	map: function (fn, bind) {
//		var length = this.length >>> 0, results = Array(length);
//		for (var i = 0; i < length; i++) {
//			if (i in this) results[i] = fn.call(bind, this[i], i, this);
//		}
//		return results;
//	},
//	some: function (fn, bind) {
//		for (var i = 0, l = this.length >>> 0; i < l; i++) {
//			if ((i in this) && fn.call(bind, this[i], i, this)) return true;
//		}
//		return false;
//	},
//	/*</!ES5>*/
//	clean: function () {
//		return this.filter(function (item) {
//			return item !== null;
//		});
//	},
//	invoke: function (methodName) {
//		var args = Array.slice(arguments, 1);
//		return this.map(function (item) {
//			return item[methodName].apply(item, args);
//		});
//	},
//	associate: function (keys) {
//		var obj = {}, length = Math.min(this.length, keys.length);
//		for (var i = 0; i < length; i++) { obj[keys[i]] = this[i]; }
//		return obj;
//	},
//	link: function (object) {
//		var result = {};
//		for (var i = 0, l = this.length; i < l; i++) {
//			for (var key in object) {
//				if (object[key](this[i])) {
//					result[key] = this[i];
//					delete object[key];
//					break;
//				}
//			}
//		}
//		return result;
//	},
//	contains: function (item, from) {
//		return this.indexOf(item, from) !== -1;
//	},
//	append: function (array) {
//		this.push.apply(this, array);
//		return this;
//	},
//	getLast: function () {
//		return (this.length) ? this[this.length - 1] : null;
//	},
//	getRandom: function () {
//		return (this.length) ? this[Number.random(0, this.length - 1)] : null;
//	},
//	include: function (item) {
//		if (!this.contains(item)) this.push(item);
//		return this;
//	},
//	combine: function (array) {
//	    for (var i = 0, l = array.length; i < l; i++) { this.include(array[i]); }
//		return this;
//	},
//	erase: function (item) {
//		for (var i = this.length; i--;) {
//			if (this[i] === item) this.splice(i, 1);
//		}
//		return this;
//	},
//	empty: function () {
//		this.length = 0;
//		return this;
//	},
//	flatten: function () {
//		var array = [];
//		for (var i = 0, l = this.length; i < l; i++) {
//			var type = typeOf(this[i]);
//			if (type === 'null') continue;
//			array = array.concat((type === 'array' || type === 'collection' || type === 'arguments' || instanceOf(this[i], Array)) ? Array.flatten(this[i]) : this[i]);
//		}
//		return array;
//	},
//	pick: function () {
//		for (var i = 0, l = this.length; i < l; i++) {
//			if (this[i] !== null) return this[i];
//		}
//		return null;
//	},
//	hexToRgb: function (array) {
//		if (this.length !== 3) return null;
//		var rgb = this.map(function (value) {
//			if (value.length === 1) value += value;
//			return parseInt(value, 16);
//		});
//		return (array) ? rgb : 'rgb(' + rgb + ')';
//	},
//	rgbToHex: function (array) {
//		if (this.length < 3) return null;
//		if (this.length === 4 && this[3] === 0 && !array) return 'transparent';
//		var hex = [];
//		for (var i = 0; i < 3; i++) {
//			var bit = (this[i] - 0).toString(16);
//			hex.push((bit.length === 1) ? '0' + bit : bit);
//		}
//		return (array) ? hex : '#' + hex.join('');
//	}
//});

///*---
//name: String
//description: Contains String Prototypes like camelCase, capitalize, test, and toInt.
//license: MIT-style license.
//requires: [Type, Array]
//provides: String
//*/
//String.implement({
//	//<!ES6>
//	contains: function (string, index) {
//		return (index ? String(this).slice(index) : String(this)).indexOf(string) > -1;
//	},
//	//</!ES6>
//	test: function (regex, params) {
//		return ((typeOf(regex) == 'regexp') ? regex : new RegExp('' + regex, params)).test(this);
//	},
//	trim: function () {
//		return String(this).replace(/^\s+|\s+$/g, '');
//	},
//	clean: function () {
//		return String(this).replace(/\s+/g, ' ').trim();
//	},
//	camelCase: function () {
//		return String(this).replace(/-\D/g, function (match) {
//			return match.charAt(1).toUpperCase();
//		});
//	},
//	hyphenate: function () {
//		return String(this).replace(/[A-Z]/g, function (match) {
//			return ('-' + match.charAt(0).toLowerCase());
//		});
//	},
//	capitalize: function () {
//		return String(this).replace(/\b[a-z]/g, function (match) {
//			return match.toUpperCase();
//		});
//	},
//	escapeRegExp: function () {
//		return String(this).replace(/([-.*+?^${}()|[\]\/\\])/g, '\\$1');
//	},
//	toInt: function (base) {
//		return parseInt(this, base || 10);
//	},
//	toFloat: function () {
//		return parseFloat(this);
//	},
//	hexToRgb: function (array) {
//		var hex = String(this).match(/^#?(\w{1,2})(\w{1,2})(\w{1,2})$/);
//		return (hex) ? hex.slice(1).hexToRgb(array) : null;
//	},
//	rgbToHex: function (array) {
//		var rgb = String(this).match(/\d{1,3}/g);
//		return (rgb) ? rgb.rgbToHex(array) : null;
//	},
//	substitute: function (object, regexp) {
//		return String(this).replace(regexp || (/\\?\{([^{}]+)\}/g), function (match, name) {
//			if (match.charAt(0) == '\\') return match.slice(1);
//			return (object[name] !== null) ? object[name] : '';
//		});
//	}
//});

///*---
//name: Function
//description: Contains Function Prototypes like create, bind, pass, and delay.
//license: MIT-style license.
//requires: Type
//provides: Function
//*/
//Function.extend({
//	attempt: function () {
//		for (var i = 0, l = arguments.length; i < l; i++) {
//			try {
//				return arguments[i]();
//			} catch (e) { }
//		}
//		return null;
//	}
//});

//Function.implement({
//	attempt: function (args, bind) {
//		try {
//			return this.apply(bind, Array.from(args));
//		} catch (e) { }

//		return null;
//	},
//	/*<!ES5-bind>*/
//	bind: function (that) {
//		var self = this,
//			args = arguments.length > 1 ? Array.slice(arguments, 1) : null,
//			F = function () { };

//		var bound = function () {
//			var context = that, length = arguments.length;
//			if (this instanceof bound) {
//				F.prototype = self.prototype;
//				context = new F;
//			}
//			var result = (!args && !length)
//				? self.call(context)
//				: self.apply(context, args && length ? args.concat(Array.slice(arguments)) : args || arguments);
//			return context == that ? result : context;
//		};
//		return bound;
//	},
//	/*</!ES5-bind>*/
//	pass: function (args, bind) {
//		var self = this;
//		if (args !== null) args = Array.from(args);
//		return function () {
//			return self.apply(bind, args || arguments);
//		};
//	},
//	delay: function (delay, bind, args) {
//		return setTimeout(this.pass((args == null ? [] : args), bind), delay);
//	},
//	periodical: function (periodical, bind, args) {
//		return setInterval(this.pass((args == null ? [] : args), bind), periodical);
//	}
//});

///*---
//name: Number
//description: Contains Number Prototypes like limit, round, times, and ceil.
//license: MIT-style license.
//requires: Type
//provides: Number
//*/
//Number.implement({
//	limit: function (min, max) {
//		return Math.min(max, Math.max(min, this));
//	},
//	round: function (precision) {
//		precision = Math.pow(10, precision || 0).toFixed(precision < 0 ? -precision : 0);
//		return Math.round(this * precision) / precision;
//	},
//	times: function (fn, bind) {
//	    for (var i = 0; i < this; i++) { fn.call(bind, i, this); }
//	},
//	toFloat: function () {
//		return parseFloat(this);
//	},
//	toInt: function (base) {
//		return parseInt(this, base || 10);
//	}
//});
//Number.alias('each', 'times');
//(function (math) {
//	var methods = {};
//	math.each(function (name) {
//		if (!Number[name]) methods[name] = function () {
//			return Math[name].apply(null, [this].concat(Array.from(arguments)));
//		};
//	});
//	Number.implement(methods);
//})(['abs', 'acos', 'asin', 'atan', 'atan2', 'ceil', 'cos', 'exp', 'floor', 'log', 'max', 'min', 'pow', 'sin', 'sqrt', 'tan']);

///*---
//name: Class
//description: Contains the Class Function for easily creating, extending, and implementing reusable Classes.
//license: MIT-style license.
//requires: [Array, String, Function, Number]
//provides: Class
//*/
//(function () {
//	var Class = this.Class = new Type('Class', function (params) {
//		if (instanceOf(params, Function)) params = { initialize: params };

//		var newClass = function () {
//			reset(this);
//			if (newClass.$prototyping) return this;
//			this.$caller = null;
//			var value = (this.initialize) ? this.initialize.apply(this, arguments) : this;
//			this.$caller = this.caller = null;
//			return value;
//		}.extend(this).implement(params);

//		newClass.$constructor = Class;
//		newClass.prototype.$constructor = newClass;
//		newClass.prototype.parent = parent;

//		return newClass;
//	});
//	var parent = function () {
//		if (!this.$caller) throw new Error('The method "parent" cannot be called.');
//		var name = this.$caller.$name,
//			parent = this.$caller.$owner.parent,
//			previous = (parent) ? parent.prototype[name] : null;
//		if (!previous) throw new Error('The method "' + name + '" has no parent.');
//		return previous.apply(this, arguments);
//	};
//	var reset = function (object) {
//		for (var key in object) {
//			var value = object[key];
//			switch (typeOf(value)) {
//				case 'object':
//					var F = function () { };
//					F.prototype = value;
//					object[key] = reset(new F);
//					break;
//				case 'array': object[key] = value.clone(); break;
//			}
//		}
//		return object;
//	};
//	var wrap = function (self, key, method) {
//		if (method.$origin) method = method.$origin;
//		var wrapper = function () {
//			if (method.$protected && this.$caller == null) throw new Error('The method "' + key + '" cannot be called.');
//			var caller = this.caller, current = this.$caller;
//			this.caller = current; this.$caller = wrapper;
//			var result = method.apply(this, arguments);
//			this.$caller = current; this.caller = caller;
//			return result;
//		}.extend({ $owner: self, $origin: method, $name: key });
//		return wrapper;
//	};
//	var implement = function (key, value, retain) {
//		if (Class.Mutators.hasOwnProperty(key)) {
//			value = Class.Mutators[key].call(this, value);
//			if (value == null) return this;
//		}

//		if (typeOf(value) == 'function') {
//			if (value.$hidden) return this;
//			this.prototype[key] = (retain) ? value : wrap(this, key, value);
//		} else {
//			Object.merge(this.prototype, key, value);
//		}

//		return this;
//	};
//	var getInstance = function (klass) {
//		klass.$prototyping = true;
//		var proto = new klass;
//		delete klass.$prototyping;
//		return proto;
//	};
//	Class.implement('implement', implement.overloadSetter());
//	Class.Mutators = {
//		Extends: function (parent) {
//			this.parent = parent;
//			this.prototype = getInstance(parent);
//		},
//		Implements: function (items) {
//			Array.from(items).each(function (item) {
//				var instance = new item;
//				for (var key in instance) { implement.call(this, key, instance[key], true); }
//			}, this);
//		}
//	};
//})();

/*---
name: ClassSingleton
description: Contains the Class Function for easily creating, extending, and implementing reusable Classes.
license: MIT-style license.
requires: [Class]
provides: ClassSingleton
*/
//var ClassSingleton = new Class({
//	initialize: function (classDefinition, options) {
//		var singletonClass = new Class(classDefinition);
//		return new singletonClass(options);
//	}
//});

(function ($, window, undefined) {
    $.log = function (message, stack) {
        $.ajax({
            url: "/logging/errorlog",
            type: "POST",
            data: JSON.stringify({ message: message, stack: stack }),
            async: true, // изменил - было false
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            success: function (msg) {
            },
            error: function (xhr) {
            }
        });
    };

    $.activityLog = function (systemKeyword, message) {
        $.ajax({
            url: "/logging/activitylog",
            type: "POST",
            data: JSON.stringify({ systemKeyword: systemKeyword, message: message }),
            async: true, // изменил - было false
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            success: function (msg) {
            },
            error: function (xhr) {
            }
        });
    };
}(window.jQuery, window));
(function ($) {
    $.widget("custom.combobox", {
        _create: function () {
            this.wrapper = $("<span>")
              .addClass("custom-combobox")
              .insertAfter(this.element);

            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
        },
        _createAutocomplete: function () {
            var selected = this.element.children(":selected"),
              value = selected.val() ? selected.text() : "";

            this.input = $("<input>")
              .appendTo(this.wrapper)
              .val(value)
              .attr("title", "")
              .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
              .autocomplete({
                  delay: 0,
                  minLength: 0,
                  source: $.proxy(this, "_source")
              })
              .tooltip({
                  tooltipClass: "ui-state-highlight"
              });

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                },
                autocompletechange: "_removeIfInvalid"
            });
        },
        _createShowAllButton: function () {
            var input = this.input,
              wasOpen = false;

            $("<a>")
              .attr("tabIndex", -1)
              .attr("title", "Show All Items")
              .tooltip()
              .appendTo(this.wrapper)
              .button({
                  icons: {
                      primary: "ui-icon-triangle-1-s"
                  },
                  text: false
              })
              .removeClass("ui-corner-all")
              .addClass("custom-combobox-toggle ui-corner-right")
              .mousedown(function () {
                  wasOpen = input.autocomplete("widget").is(":visible");
              })
              .click(function () {
                  input.focus();

                  // Close if already visible
                  if (wasOpen) {
                      return;
                  }

                  // Pass empty string as value to search for, displaying all results
                  input.autocomplete("search", "");
              });
        },
        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
        },
        _removeIfInvalid: function (event, ui) {
            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }
            // Search for a match (case-insensitive)
            var value = this.input.val(),
              valueLowerCase = value.toLowerCase(),
              valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });

            // Found a match, nothing to do
            if (valid) {
                return;
            }

            // Remove invalid value
            this.input
              .val("")
              .attr("title", value + " didn't match any item")
              .tooltip("open");
            this.element.val("");
            this._delay(function () {
                this.input.tooltip("close").attr("title", "");
            }, 2500);
            this.input.autocomplete("instance").term = "";
        },
        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });
})(jQuery);
; (function () {
    Object.fn.isEmpty = function (object) {
        var hasOwnProperty = Object.prototype.hasOwnProperty;
        // null and undefined are "empty"
        if (obj == null) return true;

        // Assume if it has a length property with a non-zero value
        // that that property is correct.
        if (obj.length > 0) return false;
        if (obj.length === 0) return true;

        // Otherwise, does it have any properties of its own?
        // Note that this doesn't handle
        // toString and valueOf enumeration bugs in IE < 9
        for (var key in obj) {
            if (hasOwnProperty.call(obj, key)) return false;
        }
        return true;
    }
});
function isEmpty(obj) {
    var hasOwnProperty = Object.prototype.hasOwnProperty;
    // null and undefined are "empty"
    if (obj == null) return true;

    // Assume if it has a length property with a non-zero value
    // that that property is correct.
    if (obj.length > 0) return false;
    if (obj.length === 0) return true;

    // Otherwise, does it have any properties of its own?
    // Note that this doesn't handle
    // toString and valueOf enumeration bugs in IE < 9
    for (var key in obj) {
        return false;
    }
    return true;
}

Array.prototype.findByElement = function (attribute, value) {
    var result = null,
        elementValue;
    this.forEach(function (element, index, array) {
        if (attribute != null) {
            elementValue = $(element).attr(attribute);
        } else {
            elementValue = $(element).text();
        }
        if (elementValue == value) {
            result = element;
        }
    });
    return result;
};
Array.prototype.elementContains = function (attribute, value) {
    var result = this.findByElement(attribute, value);
    return result != null;
};
function objectToArray(obj) {
    var array = $.map($("#GeoZoneSelected").children("li"), function (value, index) {
        return [value];
    });
    return array;
}

/*!
 * jQuery UI Touch Punch 0.2.3
 *
 * Copyright 2011–2014, Dave Furfero
 * Dual licensed under the MIT or GPL Version 2 licenses.
 *
 * Depends:
 *  jquery.ui.widget.js
 *  jquery.ui.mouse.js
 */
(function ($) {

    // Detect touch support
    $.support.touch = 'ontouchend' in document;

    // Ignore browsers without touch support
    if (!$.support.touch) {
        return;
    }

    var mouseProto = $.ui.mouse.prototype,
        _mouseInit = mouseProto._mouseInit,
        _mouseDestroy = mouseProto._mouseDestroy,
        touchHandled;

    /**
     * Simulate a mouse event based on a corresponding touch event
     * @param {Object} event A touch event
     * @param {String} simulatedType The corresponding mouse event
     */
    function simulateMouseEvent(event, simulatedType) {

        // Ignore multi-touch events
        if (event.originalEvent.touches.length > 1) {
            return;
        }

        event.preventDefault();

        var touch = event.originalEvent.changedTouches[0],
            simulatedEvent = document.createEvent('MouseEvents');

        // Initialize the simulated mouse event using the touch event's coordinates
        simulatedEvent.initMouseEvent(
          simulatedType,    // type
          true,             // bubbles                    
          true,             // cancelable                 
          window,           // view                       
          1,                // detail                     
          touch.screenX,    // screenX                    
          touch.screenY,    // screenY                    
          touch.clientX,    // clientX                    
          touch.clientY,    // clientY                    
          false,            // ctrlKey                    
          false,            // altKey                     
          false,            // shiftKey                   
          false,            // metaKey                    
          0,                // button                     
          null              // relatedTarget              
        );

        // Dispatch the simulated event to the target element
        event.target.dispatchEvent(simulatedEvent);
    }

    /**
     * Handle the jQuery UI widget's touchstart events
     * @param {Object} event The widget element's touchstart event
     */
    mouseProto._touchStart = function (event) {

        var self = this;

        // Ignore the event if another widget is already being handled
        if (touchHandled || !self._mouseCapture(event.originalEvent.changedTouches[0])) {
            return;
        }

        // Set the flag to prevent other widgets from inheriting the touch event
        touchHandled = true;

        // Track movement to determine if interaction was a click
        self._touchMoved = false;

        // Simulate the mouseover event
        simulateMouseEvent(event, 'mouseover');

        // Simulate the mousemove event
        simulateMouseEvent(event, 'mousemove');

        // Simulate the mousedown event
        simulateMouseEvent(event, 'mousedown');
    };

    /**
     * Handle the jQuery UI widget's touchmove events
     * @param {Object} event The document's touchmove event
     */
    mouseProto._touchMove = function (event) {

        // Ignore event if not handled
        if (!touchHandled) {
            return;
        }

        // Interaction was not a click
        this._touchMoved = true;

        // Simulate the mousemove event
        simulateMouseEvent(event, 'mousemove');
    };

    /**
     * Handle the jQuery UI widget's touchend events
     * @param {Object} event The document's touchend event
     */
    mouseProto._touchEnd = function (event) {

        // Ignore event if not handled
        if (!touchHandled) {
            return;
        }

        // Simulate the mouseup event
        simulateMouseEvent(event, 'mouseup');

        // Simulate the mouseout event
        simulateMouseEvent(event, 'mouseout');

        // If the touch interaction did not move, it should trigger a click
        if (!this._touchMoved) {

            // Simulate the click event
            simulateMouseEvent(event, 'click');
        }

        // Unset the flag to allow other widgets to inherit the touch event
        touchHandled = false;
    };

    /**
     * A duck punch of the $.ui.mouse _mouseInit method to support touch events.
     * This method extends the widget with bound touch event handlers that
     * translate touch events to mouse events and pass them to the widget's
     * original mouse event handling methods.
     */
    mouseProto._mouseInit = function () {

        var self = this;

        // Delegate the touch handlers to the widget's element
        self.element.bind({
            touchstart: $.proxy(self, '_touchStart'),
            touchmove: $.proxy(self, '_touchMove'),
            touchend: $.proxy(self, '_touchEnd')
        });

        // Call the original $.ui.mouse init method
        _mouseInit.call(self);
    };

    /**
     * Remove the touch event handlers
     */
    mouseProto._mouseDestroy = function () {

        var self = this;

        // Delegate the touch handlers to the widget's element
        self.element.unbind({
            touchstart: $.proxy(self, '_touchStart'),
            touchmove: $.proxy(self, '_touchMove'),
            touchend: $.proxy(self, '_touchEnd')
        });

        // Call the original $.ui.mouse destroy method
        _mouseDestroy.call(self);
    };
})(jQuery);

//$(function () {
//    $('body').on('mousedown', 'div', function (e) {
//        console.info('--- pageX: %s, pageY: %s, clientX: %s, client: %s', e.pageX, e.pageY, e.clientX, e.clientY);
//        var parent = $('.form-modal'),
//            clientX = e.clientX,
//            clientY = e.clientY;
//        parent.addClass('draggable');
//        $(this)/*.addClass('draggable')*/.parents().on('mousemove', move);
//        //e.preventDefault();
//    }).on('mouseup', function () {
//        $(this).parents().off('mousemove', move);
//        $('.draggable').removeClass('draggable');
//    });
//    function move(e) {
//        var parent = $('.form-modal');
//        var left = e.clientX, //pageX, // - parent/*$(this)*/.outerWidth(),
//            top = e.clientY; //pageY; // - /*$('.draggable')*/$(this).outerHeight() / 2;
//        console.info('left: %s, top: %s, pageX: %s, pageY: %s, clientX: %s, client: %s', left, top, e.pageX, e.pageY, e.clientX, e.clientY);
//        $('.draggable').offset({
//            top: top,
//            left: left
//        }).on('mouseup', function () {
//            $(this).parents().off('mousemove', move);
//            parent.off('mousemove', move);
//            /*$(this)*/parent.removeClass('draggable');
//        });
//    }
//});

jQuery.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
                                                $(window).scrollTop()) + "px");
    this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
                                                $(window).scrollLeft()) + "px");
    return this;
}

function loadScript(url, callback) {
    var script = document.createElement('script');
    script.type = 'text/javascript';

    if (script.readyState) {
        script.onreadystatechange = function () {
            if (script.readyState == 'loaded' || script.readyState == 'complete') {
                script.onreadystatechange = null;
                //callback();
            }
        }
    } else {
        script.onload = function () {
            //callback();
        }
    }

    script.src = url;
    document.getElementsByTagName('head')[0].appendChild(script);
}

function getData(el, name) {
    var result = el.getAttribute('data-' + name)
    return result ? result : '';
}
function setData(el, name, value) {
    el.setAttribute('data-' + name, value);
}
