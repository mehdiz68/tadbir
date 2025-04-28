$(document).ready(function () {
    //Filter By Language
    $("#LanguageFilter").change(function () {
        window.location.href ="/Admin/Folders?Type=Tree&&LanguageId=" + $(this).val();
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
        window.location.href = "/admin/Folders/SubCreate/" + CurrentRootId;
    });
    $("#EditItem").click(function () {
        window.location.href =  "/admin/Folders/Edit/" + CurrentRootId;
    });
    $("#DeleteItem").click(function () {
        window.location.href = "/admin/Folders/Delete/" + CurrentRootId;
    });
});