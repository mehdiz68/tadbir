

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
function ShowCommentSuccess(data) {
    $('.loading').hide();
    if (data.statusCode == 200) {
        $("#comments").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> نظر شما ثبت شد و پس از تایید در سایت نمایش داده می شود. </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
        $("#FullName").val("");
        $("#Email").val("");
        $("#Message").val("");
        $("#CommentParrentId").val(null);
        $("#AddComment .AnswerTo").remove();
    }
    else {
        $("#comments").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
    }
    $(".captcha").html(data.data);
}
function ShowCommentFailure(data) {
    $('.loading').hide();
    $("#comments").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
    $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
    $(".captcha").html(data.data);

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