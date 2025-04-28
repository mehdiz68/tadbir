function progressHandlingFunction2(e) {
    $(".loading").show();
    $(".CustomProgress").removeClass("hide");
    if (e.lengthComputable) {
        var percentComplete = Math.round(e.loaded * 100 / e.total);
        $("#FileProgress2").css("width", percentComplete + '%').attr('aria-valuenow', percentComplete);
        $('#FileProgress2 span').text(percentComplete + "%");
    }
    else {
        $('#FileProgress2 span').text('unable to compute');
    }
}

function completeHandler2() {
    $(".loading").hide();
    $(".CustomProgress").addClass("hide");
    $("#UploadedFiles2").val("");
    $(".Upload-container li").removeClass("open");
    if (data.statusCode == 200) {
        if (data.successCounter > 0) {
            $('#janebi-images').prepend(data.NewRow);
            swal({ title: "", text: data.status, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

        }
        else {
            swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
        }
    }
    else {
        swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
    }
}

function UploadcompleteHandler2() {
    // $('#Upload').empty();
    //Init_Upload();
}

function successHandler2(data) {
    $(".loading").hide();
    $(".CustomProgress").addClass("hide");
    $("#UploadedFiles2").val("");
    $(".Upload-container li").removeClass("open");
    if (data.statusCode == 200) {
        if (data.successCounter > 0) {
            $('#janebi-images').prepend(data.NewRow);
            swal({ title: "", text: data.status, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

        }
        else {
            swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
        }
    }
    else {
        swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
    }
}

function errorHandler2(xhr, ajaxOptions, thrownError) {
    $(".CustomProgress").addClass("hide");
    $("#UploadedFiles2").val("");
    swal({ title: "", text: "There was an error attempting to upload the file. (" + thrownError + ")", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
}


$(document).ready(function () {


    //janebi------------------------------------------------------------------------------------


    //Upload From Computer
    $(".Upload-container #UploadedFiles2").change(function (evt) {
        selectedFiles = null;
        evt.stopPropagation();
        evt.preventDefault();
        selectedFiles = evt.target.files || evt.dataTransfer.files;
        if ($("#janebi-images li").length + selectedFiles.length  < 6) {
            var form = $('#FormMultipleUpload2')[0];

            var dataString = new FormData(form);
            dataString.append("ProductId", $("#ProductId").val());

            for (var i = 0; i < selectedFiles.length; i++) {
                if (!selectedFiles[i].type.match('image.*')) {
                    continue;
                }
                if (i < 5)
                    dataString.append("UploadedFiles", selectedFiles[i]);
            }
            //}
            $.ajax({
                url: "/Profile/UplodMultipleFileFromComputer",  //Server script to process data
                type: 'POST',
                xhr: function () {  // Custom XMLHttpRequest
                    var myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) { // Check if upload property exists
                        myXhr.upload.addEventListener('progress', progressHandlingFunction2, false); // For handling the progress of the upload
                    }
                    return myXhr;
                },
                //Ajax events

                success: successHandler2,
                error: errorHandler2,
                complete: UploadcompleteHandler2,
                // Form data
                data: dataString,
                //Options to tell jQuery not to process data or worry about content-type.
                cache: false,
                contentType: false,
                processData: false
            });
        }
        else {
            swal({ title: "", text: "حداکثر 5 عکس می توانید انتخاب نمایید !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

            $("#open-file-attachement").trigger("click");
        }
    });

    //add
    $("body").on("click", ".remove-janebi", function () {
        var $this = $(this);

        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Profile/DeleteProductCommentAttachement?AttachementId=" + $this.attr("data-id") + "&ProductId=" + $("#ProductId").val(),
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#janebi-images li[data-id='" + $this.attr("data-id") + "']").remove();

                }
                else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }

                $(".loading").hide();
            },
            error: function (e) {
                swal({ title: "", text: e, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                $(".loading").hide();

            }
        });

    });

    //Add Default Image
    $("#janebi-images").on("click", "li input[type=checkbox]", function () {
        var $this = $(this);

      
        if ($this.is(":checked")) {

            $this.parent().parent().find(".DefaultImage").remove();
            $this.parent().parent().find("input[type='checkbox']").prop("checked", false);
            $this.prop("checked", true);
            $this.parent().append("<input type='hidden' class='DefaultImage' name='mainJanebi' value='" + $this.attr("data-id") + "'>");
        }
        else {
            $this.parent().find("input.DefaultImage").remove();
        }
    });
});

