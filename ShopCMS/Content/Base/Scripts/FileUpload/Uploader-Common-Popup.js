function progressHandlingFunction(e) {
    if (e.lengthComputable) {
        var percentComplete = Math.round(e.loaded * 100 / e.total);
        $("#FileProgress").css("width", percentComplete + '%').attr('aria-valuenow', percentComplete);
        $('#FileProgress span').text(percentComplete + "%");
    }
    else {
        $('#FileProgress span').text('unable to compute');
    }
}

function completeHandler() {
    $(".loading").hide();
    $('#createView').empty();
    $('.CreateLink').show();
    $.unblockUI();
}

function UploadcompleteHandler() {
    $('#Upload').empty();
    Init_Upload();
    //SelectAttachement();
}

function successHandler(data) {
    $(".loading").hide();
    if (data.statusCode == 200) {
        if (data.successCounter > 1) {
            if ($("#FilesList > div.col-lg-3").size() >= 8) {
                for (var i = 0; i < data.successCounter; i++) {
                    $("#FilesList > div.col-lg-3:nth-child(" + (i + 1) + ")").remove();
                }
            }
        }
        else
        {
            if ($("#FilesList > div.col-lg-3").size() >= 8)
                $('#FilesList > div.col-lg-3:last').remove();

        }
        $('#FilesList > div.col-lg-3:first').before(data.NewRow);
        alert(data.status);
    }
    else {
        alert(data.status);
    }
}

function errorHandler(xhr, ajaxOptions, thrownError) {
    $(".loading").hide();
    alert("There was an error attempting to upload the file. (" + thrownError + ")");
}


function Cancel_btn_handler() {
    $('#createView').empty();
    $('.CreateLink').show();
    $.unblockUI();
}

$(document).on('keydown', function (e) {
    if (e.keyCode === 27) { // ESC
        Cancel_btn_handler();
    }
});

function SelectAttachement()
{
    $("#SelectAttachement").hide();


    $("#FilesList").on("click", ".col-lg-3",function () {
        $("#SelectAttachement").fadeIn();
        $("#FilesList > .col-lg-3").css("background-color", "#ffffff").css("border", "1px dotted #ccc").removeClass("SelectAttachement");
        $(this).css("background-color", "#ededed").css("border", "1px solid #ffcc00").addClass("SelectAttachement");
        if (!$(this).find(".HasMultiSize input").is(":checked"))
            $("#Sizes").hide();
        else
            $("#Sizes").show();

    });
    $("#CloseSelectAttachement").click(function () {
        $("#SelectAttachement").fadeOut();
        $("#FilesList > .col-lg-3").css("background-color", "#ffffff").css("border", "1px dotted #ccc").removeClass("SelectAttachement");
    });

    $("#BtnAddToCover").click(function () {
        //Get Selected Div
        var selectedAttachement = $("#FilesList .SelectAttachement");
        var ID = $(selectedAttachement).attr("data-id");
        var src = $(selectedAttachement).attr("data-src");
        //Use Its
        $("#AttachementContainer input").val(ID);
        $("#AttachementContainer img").attr("src","/Content/UploadFiles/"+ src);
        $("#AttachementContainer a").attr("href", src);

        $("#SelectAttachement").hide();
        alert("انجام شد");

    });
}
$("body").on("change", "#compressionLevel", function () {
    $('#compressionLevelCurrent').text($('#compressionLevel').val());
});
$("body").on("input", "#compressionLevel", function () {
    $('#compressionLevelCurrent').text($('#compressionLevel').val());
});
