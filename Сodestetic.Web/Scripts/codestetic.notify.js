var logobar = $("#logobar").find("img");
var oldpos = [];
var clientStoped = false;

Date.prototype.toTime = function () {
    return this.toTimeString().match(/\d{2}:\d{2}:\d{2}/)[0]
};
String.prototype.format = function () {
    var formatted = this;
    for (var arg in arguments) {
        formatted = formatted.replace("{" + arg + "}", arguments[arg]);
    }
    return formatted;
};
nowTime = function () {
    var time = new Date();
    return time.toTime() + "." + time.getMilliseconds();
}
function PositionFormat(position) {
    return "Date: {0}, Lon: {2}, Lat: {1}".format(position.TimestampOnUtc.replace("T", " "), position.Longitude, position.Latitude);
}
function IndicatorFormat(indicator) {
    return "Date: {0}, Sat: {1}, GSM: {2}, Bat: {3}".format(indicator.Info.TooltipValue, indicator.Satellites.TooltipValue, indicator.GSM.TooltipValue, indicator.Battery.TooltipValue);
}
function setConnectionState(state) {
    switch (state.newState) {
        case $.connection.connectionState.connected:
            logobar.attr("class", "");
            break;
        case $.connection.connectionState.disconnected:
            logobar.attr("class", "");
            logobar.addClass("state-diconnected");
            break;
        case $.connection.connectionState.reconnecting:
            logobar.attr("class", "");
            logobar.addClass("state-reconnecting");
            break;
    }
}

function initNotification() {
    setConnectionState($.connection.connectionState.disconnected);

    $.connection.transports.longPolling.supportsKeepAlive = function () { return false; };
    var notify = $.connection.notificationHub;
    $.connection.hub.logging = true;
    $.connection.hub.disconnected(function () {
        if (!clientStoped) {
            setTimeout(function () { $.connection.hub.start(); }, 5000); // Restart connection after 5 seconds.
        }
    });
    $.connection.hub.error(function (error) {
        console.log('SignalR error: ' + error);
        $.log(error.message, error.stack);
    });
    $.connection.hub.stateChanged(function (state) { setConnectionState(state); });
    notify.client.connected = function () { $.activityLog("WebSocketConected", "Succeeded"); };
    notify.client.reconnected = function () { $.activityLog("WebSocketReconnecting", "Succeeded"); };
    notify.client.disconnected = function () { $.activityLog("WebSocketClosed", "Succeeded"); };

    notify.client.sendPosition = function (positions, user) {
        if ($.isArray(positions)) {
            var i = 0;
            $.each(positions, function () {
                //if (oldpos.length == 2)
                //    console.log("#{0} - Position: {1}, User: {2}, --".format(nowTime, PositionFormat(oldpos[i]), user));

                console.log("#{0} - Position: {1}, User: {2}".format(nowTime, PositionFormat($(this)[0]), user));
                //oldpos[i] = $(this)[0];
                //i++;
                mapControl.Devices.SetPosition($(this)[0].Id, $(this)[0]);
            });
        }
    };
    notify.client.sendIndicator = function (indicators, user) {
        if ($.isArray(indicators)) {
            $.each(indicators, function () {
                console.log("#{0} - Indicator: {1}, User: {2}".format(nowTime, IndicatorFormat($(this)[0]), user));
                mapControl.Devices.SetIndicator($(this)[0].Id, $(this)[0]);
            });
        }
    };

    // Start the connection.
    $.connection.hub.start().done(function () {
    }).fail(function (error) {
        setConnectionState($.connection.connectionState.disconnected);
        $.log(error.message, error.stack);
    });
    return notify;
}

function stopNotification() {
    clientStoped = true;
    $.connection.hub.stop();
}