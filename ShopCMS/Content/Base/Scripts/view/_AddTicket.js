
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
        swal({ title: "", text: data.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

        $("#FullName").val("");
        $("#Email").val("");
        $("#Message").val("");
        $("#Tele").val("");
        $("#CommentParrentId").val(null);
        $("#AddContactUs .AnswerTo").remove();
    }
    else {
        swal({ title: "", text: data.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
    }

    $(".captcha").html(data.data);
}
function ShowContactUsFailure(data) {
    $('.loading').hide();
    swal({ title: "", text: data.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
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

