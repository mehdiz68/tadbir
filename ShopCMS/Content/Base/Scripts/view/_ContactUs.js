
function RefreshCaptcha()
{
    $.ajax({
        type: "POST",
        url: "/Home/RefreshCaptcha",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $(".captcha").html(response.data);
        }
    });
}
function ShowContactUsSuccess(data) {
    $('.loading').hide();
    if (data.statusCode == 200) {
        $("#AddContactUs").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
        $("#FullName").val("");
        $("#Email").val("");
        $("#Message").val("");
        $("#Tele").val("");
        $("#CommentParrentId").val(null);
        $("#AddContactUs .AnswerTo").remove();
    }
    else {
        $("#AddContactUs").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
    }

    $(".captcha").html(data.data);
}
function ShowContactUsFailure(data) {
    $('.loading').hide();
    $("#AddContactUs").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
    $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
    $(".captcha").html(data.data);

}
function onBeginSubmitContactUs() {
    $('.loading').show();
    //var v = grecaptcha.getResponse();
    //if (v.length == 0) {
    //    alert("کپچا انتخاب نشده است");
    //    $('.loading').hide();
    //    return false;
    //}
}

