$(document).ready(function () {
    $("#SearchPageShow").click(function () {
        $("#SearchPage").slideToggle();
        $("#SearchPageShow").toggleClass("hilightSearch");
    });
    $("#ShowAddAttache").click(function () {
        $("#AddAttacheContainer").slideToggle();
    });
    $("#CloseAddAttacheContainer").click(function () {
        $("#AddAttacheContainer").slideUp();
    });
    $("#ShowAddFolder").click(function () {
        $("#AddFolder").slideToggle();
    });

    //Select Category Of Selected Tree
    $("#tree li a").each(function () {
        if ($(this).next().hasClass("subItem")) {
            $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 6px;cursor:pointer'>   [+]   <span>");
            $(this).parent().find(".subItem").hide();
        }
    });
    $("#tree").on("click", ".PlusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='MinusTree' style='position: relative;bottom: 6px;cursor:pointer'>   [-]   <span>");
        $(this).parent().find(".subItem").first().show();
        $(this).parent().find(".PlusTree").first().remove();

    });
    $("#tree").on("click", ".MinusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 6px;cursor:pointer'>   [+]   <span>");
        $(this).parent().find(".subItem").first().hide();
        $(this).parent().find(".MinusTree").first().remove();

    });

    //Set Selected Tree
    var CurrentFolderId = $("#HdnFolderId").val();
    if(CurrentFolderId!="")
    {
        if (CurrentFolderId > 0) {
            $(".tree ul > li a").removeClass("treeActive");
            $("#tree li a").removeClass("treeActive");
            $("#tree li a[data-id='" + CurrentFolderId + "']").addClass("treeActive");

            $("#tree .subItem").hide();
            $("#tree li a[data-id='" + CurrentFolderId + "']").parents().each(function () {
                if ($(this).is(".subItem")) {
                    $(this).show();
                    $(this).prev().parent().prepend("<span class='MinusTree' style='position: relative;bottom: 6px;cursor:pointer'>   [-]   <span>");
                    $(this).prev().parent().find(".PlusTree").first().remove();
                }
            });
        }
        else
        {
            $(".tree ul > li a").removeClass("treeActive");
            $(".tree ul > li a[data-id='" + CurrentFolderId + "']").addClass("treeActive");
        }
    }
});