(function ($) {

    $(document).ready(function () {
        // Save-Continue
        $(".options button[value=save-continue]").click(function () {
            var btn = $(this);
            btn.closest("form").append('<input type="hidden" name="save-continue" value="true" />');
        });
    });
});
