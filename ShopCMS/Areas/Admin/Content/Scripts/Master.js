$(document).ready(function () {

        $(".deleteCover").click(function () {
        $(this).parent().find("img").removeAttr("src");
        $(this).parent().find("input").val("");
    });
});