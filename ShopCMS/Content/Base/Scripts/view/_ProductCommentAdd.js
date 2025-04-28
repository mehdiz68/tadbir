function ShowCommentSuccess(data) {
    $('.loading').hide();
    if (data.statusCode == 200) {
        $("#comments").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> نظر شما ثبت شد و پس از تایید در سایت نمایش داده می شود. </p>").fadeIn(300);
        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
        $("#Title").val("");
        $("#Text").val("");
        $(".txbAdvantage").val("");
        $(".txbDisadvantage").val("");
        $("input[type='submit']").prop("disabled", true);
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