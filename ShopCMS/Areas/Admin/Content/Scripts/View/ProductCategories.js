$(document).ready(function () {
    //Filter By Language--------------------------------
    $("#LanguageFilter").change(function () {
        window.location.href = "/Admin/ProductCategoryAttribute?Type=Tree&&LanguageId=" + $(this).val();
    });
    //tree----------------------------------------------
    $("#tree li a").each(function () {
        if ($(this).next().hasClass("subItem")) {
            $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 0;cursor:pointer'>   [+]   <span>")
        }
    });
    $("#tree").on("click", ".PlusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='MinusTree' style='position: relative;bottom: 0;cursor:pointer'>   [-]   <span>");
        $(this).parent().find(".subItem").first().show();
        $(this).parent().find(".PlusTree").first().remove();

    });
    $("#tree").on("click", ".MinusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 0;cursor:pointer'>   [+]   <span>");
        $(this).parent().find(".subItem").first().hide();
        $(this).parent().find(".MinusTree").first().remove();

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
        window.location.href = "http://" + window.location.host + "/admin/ProductCategories/Create/" + CurrentRootId
    });
    $("#EditItem").click(function () {
        window.location.href = "http://" + window.location.host + "/admin/ProductCategories/Edit/" + CurrentRootId;
    });
    $("#DeleteItem").click(function () {
        window.location.href = "http://" + window.location.host + "/admin/ProductCategories/Delete/" + CurrentRootId;
    });
    //Sort---------------------------------------------
    $("#tree li a").each(function () {
        var $this = $(this);
        $(this).parent().attr("id", $this.attr("data-id"));
    });
    //تب کمیسیون-------------------------------------
    $("#Update-Commission").click(function () {
        if ($("#txt-Commission").val() && $("#Update-Commission").attr("data-id") > 0) {
            $("#txt-Commission").removeClass("input-validation-error");
            $(".loading").show(); NProgress.start();
            $.ajax({
                crossDomain: true,
                type: "POST",
                url: '/Admin/ProductCategories/UpdateCommision/',
                data: '{CatId:"' + parseInt($("#Update-Commission").attr("data-id")) + '",value:"' + parseInt($("#txt-Commission").val()) + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        $(".loading").hide(); NProgress.done();
                        swal({ title: "بروزرسانی کمیسیون", text: "با موفقیت بروز شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                    }
                    else {
                        $(".loading").hide(); NProgress.done();
                        swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }

                }
            });
        }
        else {
            $("#txt-Commission").addClass("input-validation-error");
        }
    });

    //افزودن خصوصیت به گروه
    $(".AddAttribute").click(function (e) {
        e.preventDefault();
        var $this = $(this);
        $(".loading").show(); NProgress.start();
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: $this.attr("href"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "افزودن خصوصیت به گروه", text: "با موفقیت اضافه شد ", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                }
                else {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }

            }
        });
    });
    //حذف از گروه

    $(".DeleteAttribute").click(function (e) {
        e.preventDefault();
        var $this = $(this);
        $(".loading").show(); NProgress.start();
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: $this.attr("href"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "حذف خصوصیت از گروه", text: "با موفقیت حذف شد ", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                }
                else {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }

            }
        });
    });

    //خصوصیات قابل سرچ گروه
    $(".att input[type='checkbox']").click(function () {
        var $this = $(this);
        $this.parent().find("input[type='hidden']").remove();
        if ($this.is(":checked")) {
            $this.parent().prepend("<input type='hidden' value='" + $this.attr("data-id") + "' name='Attid'>");
        }
    });


    //گروه خصوصییت های گروه
    $(".grp input[type='checkbox']").click(function () {
        var $this = $(this);
        $this.parent().find("input[type='hidden']").remove();
        if ($this.is(":checked")) {
            $this.parent().prepend("<input type='hidden' value='" + $this.attr("data-id") + "' name='grpId'>");
        }
    });
});



function ShowCommentSuccess(data) {
    $('.loading').hide();
    if (data.statusCode == 200) {
        $("#tab_atts").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> تغییرات ثبت شد. </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });

    }
    else {
        $("#tab_atts").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
    }
}
function ShowCommentFailure(data) {
    $('.loading').hide();
    $("#tab_atts").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
    $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });

}

function onBeginSubmitAddComment() {
    $('.loading').show();
    //var v = grecaptcha.getResponse();
    //if (v.length == 0) {
    //    alert("کپچا انتخاب نشده است");
    //    $('.loading').hide();
    //    return false;
    //}
}


function ShowCommentSuccess2(data) {
    $('.loading').hide();
    if (data.statusCode == 200) {
        $("#tab_Groups").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> تغییرات ثبت شد. </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });

    }
    else {
        $("#tab_Groups").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
    }
}
function ShowCommentFailure2(data) {
    $('.loading').hide();
    $("#tab_Groups").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
    $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });

}


function onBeginSubmitAddComment2() {
    $('.loading').show();
    //var v = grecaptcha.getResponse();
    //if (v.length == 0) {
    //    alert("کپچا انتخاب نشده است");
    //    $('.loading').hide();
    //    return false;
    //}
}