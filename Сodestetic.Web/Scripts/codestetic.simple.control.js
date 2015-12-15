// --- Scroll text ---
jQuery.fn.scrollText = function (handler) {
    //$(this).addClass('scroll-text');
    //if (handler) {
    //    $(handler).hover(_runScroll, _resetScroll);
    //} else {
    $(this).hover(_runScroll, _resetScroll);
    //}
    function _runScroll() {
        var el = this /*_getElement(this)*/, target;

        if (!el) { return; }

        var target = el.scrollWidth - $(el).parent().width();
        if (target > 0) {
            $(el).stop().animate({
                'margin-left': -target
            }, {
                queue: false,
                duration: 20 * target
            }, 'linear');
        }
    }
    function _resetScroll() {
        var el = this /*_getElement(this)*/;
        $(el).stop().animate({ 'margin-left': 0 }, 250);
    }
    function _getElement(el) {
        var el = $(el), element, selector = '.scroll-text';

        if (!el.hasClass(selector)) {
            element = el.closest(selector);
            if (element.length == 0) {
                element = el.parent().find(selector);
            }
            if (element.length == 0) { return undefined; }
            el = element[0];
        }
        return el;
    }
}
// --- end Scroll text ---


// --- end Toggle button ---

// --- Spinner ---
//jQuery.fn.spinner = function () {
//    var element = $(this);

//    element.find('.btn:first-of-type').on('click', function (e) {
//        var parent = $(e.currentTarget).parents('.ui-spinner'),
//            input = parent.children('input').first();
//        input.val(parseInt(input.val(), 10) + 1);
//    });
//    element.find('.btn:last-of-type').on('click', function (e) {
//        var parent = $(e.currentTarget).parents('.ui-spinner'),
//            input = parent.children('input').first();
//        input.val(parseInt(input.val(), 10) - 1);
//    });
//}
// --- end Spinner ---