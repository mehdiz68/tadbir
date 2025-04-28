$(document).ready(function () {
    $("#View2 #tree li a").click(function (e) {
        e.preventDefault();
    });

    $("#tree li a").each(function () {
        var $this = $(this);
        $(this).parent().attr("data-type", $this.attr("data-type"));
    });

    $("#SearchPageShow").click(function () {
        $("#SearchPage").slideToggle();
        $("#SearchPageShow").toggleClass("hilightSearch");
    });

    var CurrentRootId = "";
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
        $(this).find("> a").addClass("hilight");
        CurrentRootId = $(this).find("a").attr('data-id');
        //if ($(this).find("a").attr("data-type") == 8 || $(this).find("a").attr("data-type") == 10) {
        //    if ($(this).hasClass("allowProduct")) {
        //        $("#ContexMenu #EditItem").show();
        //        $("#ContexMenu #NewSubMenu").hide();
        //        $("#ContexMenu #DeleteItem").show();
        //    }
        //    else {
        //        $("#ContexMenu #EditItem").hide();
        //        $("#ContexMenu #NewSubMenu").show();
        //        $("#ContexMenu #DeleteItem").hide();
        //    }
        //}
        //else {

            $("#ContexMenu #NewSubMenu").show();
            $("#ContexMenu #EditItem").show();
            $("#ContexMenu #DeleteItem").show();
        //}

        return false;
    });
    $("#NewSubMenu").click(function () {
        window.location.href = "http://" + window.location.host + "/admin/Menus/SubCreate/" + CurrentRootId;
    });
    $("#EditItem").click(function () {
        window.location.href = "http://" + window.location.host + "/admin/Menus/Edit/" + CurrentRootId;
    });
    $("#DeleteItem").click(function () {
        window.location.href = "http://" + window.location.host + "/admin/Menus/Delete/" + CurrentRootId;
    });


    //Select Category Of Selected Tree
    $("#tree li a").each(function () {
        if ($(this).next().hasClass("subItem")) {
            $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: -6px;cursor:pointer'>   [+]   <span>");
            $(this).parent().find(".subItem").hide();
        }
    });
    $("#tree").on("click", ".PlusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='MinusTree' style='position: relative;bottom: -6px;cursor:pointer'>   [-]   <span>");
        $(this).parent().find(".subItem").first().show();
        $(this).parent().find(".PlusTree").first().remove();

    });
    $("#tree").on("click", ".MinusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: -6px;cursor:pointer'>   [+]   <span>");
        $(this).parent().find(".subItem").first().hide();
        $(this).parent().find(".MinusTree").first().remove();

    });


    //---------------------------------
    //Sort
    //$("#tree li a").each(function () {
    //    var $this = $(this);
    //    $(this).parent().attr("id", $this.attr("data-id"));
    //});

    //$("#tree").sortable({
    //    items: "> li",
    //    cursor: 'move',
    //    opacity: 0.6,
    //    placeholder: "ui-state-highlight",
    //    update: function () {
    //        sendPageOrderToServer();
    //    }
    //});
    //function sendPageOrderToServer() {
    //    var order = $("#tree").sortable("toArray");
    //    $.ajax({
    //        crossDomain: true,
    //        type: "POST",
    //        url: "/Admin/Menus/Sort",
    //        data: '{ids:"' + order + '"}',
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        success: function (response) {
    //            if (response.statusCode == 400) {
    //                $(".tree").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> انجام شد . </p>").fadeIn(300);
    //                $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
    //            } else {
    //                $(".tree").before("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> خطا </p>").fadeIn(300);;
    //                $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
    //            }
    //        }
    //    });
    //}

});