$(document).ready(function () {
    $("#Cat").change(function () {
        if ($(this).val() != "0")
            window.location.href = $(this).val();
    });

    $("#isExist").change(function () {
        var $this = $(this);
        $this.parent().find("#has_selling_stock").remove();
        if ($this.is(":checked")) {
            $this.parent().append("<input id='has_selling_stock' value='1' name='has_selling_stock' type='hidden' />");
        }
    });
    if ($(".js-switch")[0]) {
        var elems = Array.prototype.slice.call(document.querySelectorAll('.js-switch'));
        elems.forEach(function (html) {
            var switchery = new Switchery(html, {
                color: '#26B99A'
            });
        });
    }

    $(".js-search-sort").click(function () {
        $(".c-listing__header").addClass("right-search-options-open");
    });
    $(".js-search-advanced").click(function () {
        $(".right-search-options").addClass("right-search-options-open");
    });
    $("#Close-right-search-options").click(function () {
        $(".right-search-options").removeClass("right-search-options-open");
    });
    $("#Close-listing__header").click(function () {
        $(".c-listing__header").removeClass("right-search-options-open");
    });

});
