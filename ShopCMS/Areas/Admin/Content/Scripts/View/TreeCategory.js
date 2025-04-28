$(document).ready(function () {
    //Filter By Language
    $("#LanguageFilter").change(function () {
        window.location.href =  "/Admin/Categories?Type=Tree&&LanguageId=" + $(this).val()+"&&ContentTypeId="+$("#ContentTypeId").val();
    });

    $('#tree li').on('click', function (e) {
        e.preventDefault();
    });
    $("#tree li").contextmenu({
        menu: "#ContexMenu",
        preventSelect: true,
        taphold: true,
        hide: function (event, ui) {
            alert(ui.target.text());
        }
    });
    $('#tree li').on('contextmenu', function (e) {
        $("#tree li a").removeClass("hilight");
        $(this).find("a").addClass("hilight");
        CurrentRootId = $(this).find("a").attr('data-id');

        return false;
    });
    $("#NewSubMenu").click(function () {
        window.location.href = "/admin/Categories/SubCreate/" + CurrentRootId
    });
    $("#EditItem").click(function () {
        window.location.href = "/admin/Categories/Edit/" + CurrentRootId ;
    });
    $("#DeleteItem").click(function () {
        window.location.href =  "/admin/Categories/Delete/" + CurrentRootId ;
    });

    //---------------------------------
    //Sort
    $("#tree li a").each(function () {
        var $this = $(this);
        $(this).parent().attr("id", $this.attr("data-id"));
    });

    $("#tree").sortable({
        items: "li",
        cursor: 'move',
        opacity: 0.6,
        placeholder: "ui-state-highlight",
        update: function () {
            sendPageOrderToServer();
        }
    });
    function sendPageOrderToServer() {
        var order = $("#tree").sortable("toArray");
        $.ajax({
            crossDomain: true,
            type: "POST",
            url:  "/Admin/Categories/Sort",
            data: '{ids:"' + order + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".tree").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> انجام شد . </p>").fadeIn(300);
                    $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                } else {
                    $(".tree").before("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> خطا </p>").fadeIn(300);;
                    $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                }
            }
        });
    }

});