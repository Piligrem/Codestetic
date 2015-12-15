var titles = {};

$.fn.extend({
    initNotificationHeders: function (info, success, warning, error) {
        titles = {
            info: info,
            success: success,
            warning: warning,
            error: error
        };
    },
    showNotifications: function () {
        var notification = $("#notification").data("kendoNotification");
        var items = $("#notification-list").find("> .msg");
        
        items.each(function (i, el) {
            el = $(el);
            var type = el.data("type");

            notification.show({
                type: type,
                title: titles[type],
                message: el.html(),
            }, type);
        });
    },
    clearMessage: function (e) {
        $("#notification-list").empty();
    },
    notify: function (type, message) {
        var notificationList = $("#notification-list");
        var element = notificationList[0].appendChild(document.createElement("div"));
        $(element).addClass("msg").attr("data-type", type).append(message);
        $(this).showNotifications();
        $(element).remove();
    }
});
