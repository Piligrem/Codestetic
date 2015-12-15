;(function ($) {
	$.fn.MarkerEditor = function (options) {
		var settings = $.extend(true, {
			icon: {
				classes: "marker-icon",
				url: "",
				id: 0,
				anchor: { x: 0, y: 0 }
			},
			shadow: {
				classes: "marker-shadow",
				url: "",
				id: 0,
				anchor: { x: 0, y: 0 }
			},
			anchor: {
				classes: "anchor-icon"
			}
		}, options),
		    iconEditor = { },
		    container = this,
            // Initialize
            init = function () {
			// Add icon image
			var image = document.createElement("img");
			//image.src = settings.icon.url;
			iconEditor.icon = $(image);
			iconEditor.icon.addClass(settings.icon.classes);
				//.data("id", settings.icon.id);

			container.append(iconEditor.icon);

			setItem(iconEditor.icon, null, {
			    iconId: settings.icon.id,
			    imageUrl: settings.icon.url,
			    anchorX: settings.icon.anchor.x,
			    anchorY: settings.icon.anchor.y
			}, true, false);

		    // Add marker anchor
			var anchor = document.createElement("span");
			iconEditor.anchor = $(anchor);
			iconEditor.anchor
				.addClass(settings.anchor.classes)
				.data("icon", settings.icon.classes.split(" ")[0])
				.data("shadow", settings.shadow.classes.split(" ")[0]);

			container.append(iconEditor.anchor);
			iconEditor.anchor
				.draggable({
				    containment: "." + container.attr("class"),
				    scroll: false,
				});
			setAnchorPosition(iconEditor.icon, settings.icon.anchor.x, settings.icon.anchor.y);

			// Add shadow image
			image = document.createElement("img");
			//image.src = settings.shadow.url;
			iconEditor.shadow = $(image);
			iconEditor.shadow.addClass(settings.shadow.classes);
				//.data("id", settings.shadow.id);

			container.append(iconEditor.shadow);

			setItem(iconEditor.shadow, null, {
			    iconId: settings.shadow.id,
			    imageUrl: settings.shadow.url,
			    anchorX: settings.shadow.anchor.x,
			    anchorY: settings.shadow.anchor.y
			}, true, true);

			iconEditor.shadow
				.draggable({
					containment: "." + container.attr("class"),
					scroll: false,
				});
		}

		// Methods
		container.setMarker = function (button, value) {
			setItem(iconEditor.icon, button, value);
		}
		container.setShadow = function (button, value) {
			setItem(iconEditor.shadow, button, value, true, true);
		}
		container.show = function (el) {
			el.checked ? $("." + $(el).data("child")).show() : $("." + $(el).data("child")).hide();
		}
		container.clearItem = function (element, e) {
			//var item = {};
			$(element).data("selector") == "icon" ? item = iconEditor.icon : item = iconEditor.shadow;
			item.attr("src", "")
				.attr("alt", "")
				.data("id", "")
				.data("anchor-x", "")
				.data("anchor-y", "")
				.hide();
			$(element).hide();
			e.preventDefault();
		}
		container.deleteMarker = function(e)
		{
			var icon = $(".slide-edit .marker-icon"),
				id = icon.data("id");
			if (id != 0) {
			    var form = $(e).find("form")[0],
			        input = document.createElement("input"),
                    action = form.action;

			    action = action.replace(/\/0/i, "/" + id);
			    form.action = action;
			}
		}
		// End methods

		// Functions
		container.setValuesToForm = function (form) {
		    var anchor = iconEditor.anchor.position(),
                input = document.createElement("input"),
                id = iconEditor.icon.data("id"),
                x = anchor.left - iconEditor.icon.position().left + 10,
                y = anchor.top - iconEditor.icon.position().top + 10,
		        aid = 0, ax = 0, ay = 0;

		    input.name = "id";
		    input.type = "hidden";
		    input.value = id;
		    input.style.display = "none";
		    form.append(input);

		    input = document.createElement("input");
		    input.name = "anchor[x]";
		    input.type = "hidden";
		    input.value = x;
		    input.style.display = "none";
		    form.append(input);

		    input = document.createElement("input");
		    input.name = "anchor[y]";
		    input.type = "hidden";
		    input.value = y;
		    input.style.display = "none";
		    form.append(input);


			var value = {
				Id: id,
				Anchor: {
					X: x,
					Y: y
				},
			};
			if (iconEditor.shadow.data("id") != 0) {
			    aid = iconEditor.shadow.data("id");
			    x = anchor.left - iconEditor.shadow.position().left + 10;
			    y = anchor.top - iconEditor.shadow.position().top + 10;

			    input = document.createElement("input");
			    input.name = "shadowMarker[id]";
			    input.type = "hidden";
			    input.value = aid;
			    input.style.display = "none";
			    form.append(input);

			    input = document.createElement("input");
			    input.name = "shadowMarker[anchor][x]";
			    input.type = "hidden";
			    input.value = x;
			    input.style.display = "none";
			    form.append(input);

			    input = document.createElement("input");
			    input.name = "shadowMarker[anchor][y]";
			    input.type = "hidden";
			    input.value = y;
			    input.style.display = "none";
			    form.append(input);

			    value.ShadowMarker = {
					Id: aid,
					Anchor: {
						X: x,
						Y: y
					}
				};
			}
			return value;
		}
		// End functions

		// Utilities
		function setItem(item, button, value, fixAnchorPosition, isShadow) {
		    item.attr("src", value.imageUrl != undefined ? value.imageUrl : "");
		    var width = value.hasOwnProperty("size") ? value.size.Width : item.width(),
                height = value.hasOwnProperty("size") ? value.size.Height : item.width(),
		        ax = value.hasOwnProperty("anchorX") ? value.anchorX : width / 2,
				ay = value.hasOwnProperty("anchorY") ? value.anchorY : height,
                x = 0, y = 0;

		    if (!isShadow && item.attr("src") != "") {
                x = (container.width() - width) / 2;
	            y = (container.height() - height) / 2;
		    } else if (item.attr("src") != "") {
                x = (iconEditor.anchor.position().left - ax + 10);
	            y = (iconEditor.anchor.position().top - ay + 10);
		    }

		    item.data("id", value.iconId != undefined ? value.iconId : 0)
				.css("left", x)
				.css("top", y);
				
		    if (item.attr("src") != "") {
		        item.show();
		    } else {
		        item.hide();
		    }
			if (!fixAnchorPosition)
				setAnchorPosition(item, ax, ay);
			if (button != null) {
				button.show();
			}
		}
		function setAnchorPosition(item, x, y) {
		    if (item.attr("src") != "") {
		        iconEditor.anchor
                    .css("left", item.position().left + x - 10)
                    .css("top", item.position().top + y - 10);
		    } else {
		        iconEditor.anchor
                    .css("left", 0)
                    .css("top", 0);
		    }
		}

		init();
		return this;
	}
})(jQuery);