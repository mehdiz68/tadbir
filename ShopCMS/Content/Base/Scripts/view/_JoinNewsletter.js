function ShowJoinNewsletterSuccess(data) {
    $('.loading').hide();
    if (data.statusCode == 200) {
        $("#JoinNewsLetter").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
        $(".catmsg").delay(5000).fadeOut(300, function () { $(this).remove(); });
        $("#EmailNewsletter").val("");
    }
    else {
        $("#JoinNewsLetter").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
        $(".catmsg").delay(5000).fadeOut(300, function () { $(this).remove(); });
    }
}
function ShowJoinNewsletterFailure(data) {
    $('.loading').hide();
    $("#JoinNewsLetter").append("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> " + data.Message + " </p>").fadeIn(300);
    $(".catmsg").delay(5000).fadeOut(300, function () { $(this).remove(); });

}
function onBeginSubmit() {
    $('.loading').show();
    //var v = grecaptcha.getResponse();
    //if (v.length == 0) {
    //    alert("کپچا انتخاب نشده است");
    //    $('.loading').hide();
    //    return false;
    //}
}

