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
    if (data.statusCode == 200) {
        $('.loading').hide();
        if (data.statusCode == 200) {
            $("#AddQuestion").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> پرسش شما ثبت شد و پس از تایید در سایت نمایش داده می شود. </p>").fadeIn(300);
            $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
            $("#Message").val("");
            $("#ProductQuestionId").val("");
        }
        else {
            $("#AddQuestion").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
            $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
        }
        $(".captcha").html(data.data);
        $("#AddQuestion button").text("ثبت پرسش");
    }
    else if(data.statusCode==403) {
        $("#Message").val("");
        $("#ProductQuestionId").val("");
        $("#AddQuestion button").text("ثبت پرسش");
        $('.loading').hide();
        $("#ModalLogin").modal();
    }
}
function ShowCommentFailure(data) {
    $('.loading').hide();
    $("#AddQuestion").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
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