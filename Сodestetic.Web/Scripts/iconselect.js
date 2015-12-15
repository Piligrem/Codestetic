; (function ($) {
    $.fn.MarkerDropDown = function (options) {
        // Check arguments
        if (options.listMarker.width == undefined && options.listMarker.height != undefined) {
            options.listMarker.width = options.listMarker.height;
        }
        if (options.listMarker.height == undefined && options.listMarker.width != undefined) {
            options.listMarker.height = options.listMarker.width;
        }
        var settings = $.extend(true, {
            currentIndex: 0,
            selectedMarker: {
                width: 64,
                height: 64,
                padding: 1,
            },
            listMarker: {
                width: 32,
                height: 32,
                margin: 1,
                horizontalNumber: 2,
                verticalNumber: 3
            },
            scroll: {
                cursorColor: "#000",
                cursorBorder: "1px solid #FFF",
                cursorBorderRadius: 5,
                cursorWidth: 4,
                railOffset: {
                    top: 0,
                    left: 0,
                }
            },
            data: {
                key: "",
                value: ""
            }
        }, options),
        container = this,
        dropList = {},
        selectedBox = {},
        iconFirstLine = settings.listMarker.margin + "px " + "0 " + settings.listMarker.margin + "px " + settings.listMarker.margin + "px",
        iconMargin = "0 0 " + settings.listMarker.margin + "px " + settings.listMarker.margin + "px",
        init = function () {
            var widthSelectedBox = settings.selectedMarker.width, //-settings.selectedMarker.padding * 2 - 2,
                heightSelectedBox = settings.selectedMarker.height, // - settings.selectedMarker.padding * 2 - 2,
                widthWrapDropList = (settings.listMarker.width + settings.listMarker.margin + 2) * settings.listMarker.horizontalNumber + 2,
                //heightWrapDropList = (settings.listMarker.height + settings.listMarker.margin + 2) * settings.listMarker.verticalNumber + 1;
                heightWrapDropList = (settings.listMarker.height + settings.listMarker.margin) * settings.listMarker.verticalNumber + 1;

            settings.selectedMarker.iconWidth = settings.selectedMarker.width - settings.selectedMarker.padding * 2 - 2;
            settings.selectedMarker.iconHeight = settings.selectedMarker.height - settings.selectedMarker.padding * 2 - 2;
            settings.currentIndex = settings.currentIndex == 0 ? 1 : settings.currentIndex;

            // Container
            container.addClass("droplist-icon");

            // ------------ Selected box ------------ 
            selectedBox = $(document.createElement("div"));
            selectedBox.addClass("selected-box")
                .css("width", widthSelectedBox).css("height", heightSelectedBox)
                .css("padding", settings.selectedMarker.padding)
                .data("index", 0);
            selectedBox.click(function () { trigger(); });
            container.append(selectedBox);

            // ------ Selected icon ------ 
            var wrapSelectedMarker = $(document.createElement("div"));
            wrapSelectedMarker.addClass("selected-icon");

            var image = $(document.createElement("img"));
            wrapSelectedMarker.append(image);
            selectedBox.append(wrapSelectedMarker);
            wrapSelectedMarker.icon = image;
            selectedBox.wrapSelectedMarker = wrapSelectedMarker;

            image = $(document.createElement("img"));
            image.addClass("shadow");
            wrapSelectedMarker.append(image);
            wrapSelectedMarker.shadow = image;
            // ------ End selected icon ------ 
            // ------------ End selected box ------------ 

            // ------------ DropList box ------------ 
            dropList = $(document.createElement("div"));
            dropList.addClass("wrap-droplist-box");
            //container.append(dropList);
            $("body").append(dropList);
            var pos = getPosition($(".icon-slider"));
            dropList
                .css("left", pos.x)
                .css("top", pos.y);

                // ------ Wrap icons ------ 
                var wrapMarkers = $(document.createElement("div"));
                wrapMarkers.addClass("wrap-icons")
                    .css("width", widthWrapDropList).css("height", heightWrapDropList)
                    //.css("z-index", 10001)
                    .hide();
                dropList.append(wrapMarkers);
                dropList.icons = wrapMarkers
                // ------ End wrap icons ------ 

                // ------ Button DropList ------ 
                var wrapButton = $(document.createElement("div"));
                wrapButton.addClass("droplist-button");
                //var pos = getPosition(dropList);
                wrapButton
                    .css("left", pos.x)
                    .css("top", pos.y);
                dropList.append(wrapButton);
                wrapButton.click(function () {
                    trigger();
                });
                dropList.button = wrapButton;
                // ------ End button DropList ------ 

                // ------ Nice scroll ------ 
                var scrollWrapMarkers = wrapMarkers.niceScroll({
                    cursorcolor: settings.scroll.cursorColor,
                    cursorborder: settings.scroll.cursorBorder,
                    cursorborderradius: settings.scroll.cursorBorderRadius,
                    cursorwidth: settings.scroll.cursorWidth,
                    oneaxismousemode: true,
                    railoffset: {
                        top: settings.scroll.railOffset.top,
                        left: settings.scroll.railOffset.left,
                    }
                });

                //scrollWrapMarkers.hide();
                dropList.scroll = scrollWrapMarkers
                // ------ End nice scroll ------ 
            // ------------ End dropList box ------------
        },
        addMarker = function (icon, firstLine) {
            var wrapMarker = $(document.createElement("div")),
                shadowImage = $(document.createElement("img")),
                iconImage = $(document.createElement("img"));

            wrapMarker.addClass("icon")
                .css("width", settings.listMarker.width)
                .css("height", settings.listMarker.height)
                .css("margin", firstLine ? iconFirstLine : iconMargin)
                .attr("icon-index", icon[settings.data.key]);

            iconImage.attr("src", icon[settings.data.value]);
            wrapMarker.append(iconImage);
            var iconImagePos = setMarkerPos(iconImage, icon.Size);

            shadowImage.attr("src", icon["Shadow" + settings.data.value])
                .addClass("shadow");
            wrapMarker.append(shadowImage);
            setShadowPos(shadowImage, iconImagePos, icon.Size);

            dropList.icons.append(wrapMarker);
            wrapMarker.bind("click", function (e) { iconSelect($(e.currentTarget)); });
        };

        // ------------ Methods ------------ 
        function setMarkerPos(item, size) {
            var x = (settings.listMarker.width - size.width) / 2,
                y = (settings.listMarker.height - size.height) / 2;

            item.css("left", x).css("top", y);
            return { x: x, y: y };
        }
        function setShadowPos(item, iconImagePos, size) {
            item.css("left", iconImagePos.x + size.x).css("top", iconImagePos.y + size.y);
        }
        function findMarkerByIndex(index) {
            var item = dropList.icons.find("div[icon-index=" + index + "]");
            return item.length == 0 ? dropList.icons.children().first() : item;
        }
        function getPosition(element) {
            return {
                x: element.offset().left,
                y: element.offset().top
            };
        }
        // ------------ End methods ------------ 

        // ------------ Events ------------ 
        function trigger() {
            // Hide
            if (dropList.button.hasClass("il-droplist")) {
                dropList.scroll.hide();

                options = { to: { width: settings.listMarker.width, height: settings.listMarker.height } }
                dropList.icons.hide("Fold", null, 1000);
                
                setTimeout(function () {
                    dropList.button.removeClass("droplist-shadow")
                        .removeClass("il-droplist");
                    selectedBox.removeClass("il-droplist");
                }, 150);
            } else {
                //"left", container.offsetLeft).css("top", container.offsetTop)
                options = { to: { width: settings.listMarker.width, height: settings.listMarker.height } }
                var pos = getPosition(container);
                dropList
                    .css("left", pos.x)
                    .css("top", pos.y);
                dropList.icons.show("Fold", null, 1000);
                setTimeout(function () {
                    dropList.button
                        .addClass("droplist-shadow")
                        .addClass("il-droplist");
                    selectedBox.addClass("il-droplist");
                    dropList.scroll.show();
                }, 300);
            }
        }
        function iconSelect(item, localCall) {
            var dx = (settings.selectedMarker.iconWidth - settings.listMarker.width) / 2,
                dy = (settings.selectedMarker.iconHeight - settings.listMarker.height) / 2,
                children = item.children(),
                icon = children.first(),
                shadow = children.last();

            if (localCall == undefined) {
                trigger();
            }

            var value = item.attr("icon-index");
            selectedBox.data("index", value);
            container.val(value);

            selectedBox.wrapSelectedMarker.icon.attr("src", icon.attr("src"))
                .css("left", parseInt(icon.css("left")) + dx).css("top", parseInt(icon.css("top") + dy));

            selectedBox.wrapSelectedMarker.shadow.attr("src", shadow.attr("src"))
                .css("left", parseInt(shadow.css("left")) + dx).css("top", parseInt(shadow.css("top") + dy));
        }
        // ------------ End events ------------ 

        container.load = function (array) {
            var col = 0, firstLine = true;
            for (var i = 0; i < array.length; i++) {
                if (col == settings.listMarker.horizontalNumber) { firstLine = false; }
                addMarker(array[i], firstLine);
                col++;
            }
            var currentMarker = findMarkerByIndex(settings.currentIndex);
            iconSelect(currentMarker, false);
        };
        container.ajaxLoad = function (action, controller, route) {
            var url = "/" + controller + "/" + action;
            if (route != undefined) {
                url = "/" + route + url;
            }
            $.ajax({
                url: url,
                type: "POST",
                success: function (result) {
                    if (settings.data.key != "" || settings.data.value != "") {
                        if (result.length > 0 &&
                            result[0].hasOwnProperty(settings.data.key) && result[0].hasOwnProperty(settings.data.value)) {
                            container.load(result);
                        }
                    }
                },
                error: function (xhr) {
                    $.log("Error: {0}, Message: {1}".format(xhr.error, xhr.statusText), form.attr('action'));
                }
            });
        };

        init();

        return this;
    }
})(jQuery);
