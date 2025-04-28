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
                $('#FilesList > div.col-lg-3:first').remove();

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

function OnDeleteAttachmentSuccess(data) {

    if (data.ID && data.ID != "") {
        $('#Attachment_' + data.ID).fadeOut('slow');
    }
    else {
        alter("Unable to Delete");
        console.log(data.message);
    }
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
 
    $("body").on("change", "#compressionLevel", function () {
        $('#compressionLevelCurrent').text($('#compressionLevel').val());
    });
  $("body").on("input", "#compressionLevel", function () {
      $('#compressionLevelCurrent').text($('#compressionLevel').val());
    });
