function progressHandlingFunction2(e) {
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
    //$(".loading").hide();
}

function UploadcompleteHandler2() {
    // $('#Upload').empty();
    //Init_Upload();
}

function successHandler2(data) {
    //$(".loading").hide();
    $(".CustomProgress").addClass("hide");
    $("#UploadedFiles2").val("");
    $(".Upload-container li").removeClass("open");
    if (data.statusCode == 200) {
        if (data.successCounter > 0) {
            $('#janebi-images').prepend(data.NewRow);

            alert(data.status);
        }
        else {
            alert(data.status);
        }
    }
    else {
        alert(data.status);
    }
}

function errorHandler2(xhr, ajaxOptions, thrownError) {
    $(".CustomProgress").addClass("hide");
    $("#UploadedFiles2").val("");
    alert("There was an error attempting to upload the file. (" + thrownError + ")");
}


$(document).ready(function () {
    

    //janebi------------------------------------------------------------------------------------


    //Upload From Computer
    $(".Upload-container #UploadedFiles2").change(function (evt) {

        selectedFiles = null;
        evt.stopPropagation();
        evt.preventDefault();
        selectedFiles = evt.target.files || evt.dataTransfer.files;

        //$('#FormMultipleUpload2').submit();
        // here we will create FormData manually to prevent sending mon image files
        var form = $('#FormMultipleUpload2')[0];

        var dataString = new FormData(form);
        //Add Watermark
        dataString.append("UseWaterMark", "on");
        if ($("#ProductCatId").val() != "" && $("#ProductCatId").val()  != undefined)
            dataString.append("ProductCatId", parseInt($("#ProductCatId").val()));
        else
            dataString.append("ProductCatId", 0);
        //Get Current Controller and Pass it
        var url = window.location.pathname.split("/");
        var controller = url[2];
        dataString.append("controllerName", controller.toLowerCase());

        //var files = document.getElementById("UploadedFiles2").files;
        //if (form[6] == null) {
        for (var i = 0; i < selectedFiles.length; i++) {
            //if (!selectedFiles[i].type.match('image.*')) {
            //    continue;
            //}

            dataString.append("UploadedFiles", selectedFiles[i]);
        }
        //}
        $.ajax({
            url: "/Admin/FileManager/UplodMultipleFileFromComputer",  //Server script to process data
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
    });

    //sort 
    $('#janebi-images').sortable({
        opacity: 0.8,
        revert: true,
        forceHelperSize: true,
        forcePlaceholderSize: true,
        placeholder: 'draggable-placeholder',
        tolerance: 'pointer'
    });
    //add
    $("#Addjanebi").click(function () {
        var id = $("#AttachementContainer2 input").val();
        var $this = $(this);
        $("#janebi-images").append("<li  data-id='" + id + "'><span class='glyphicon glyphicon-remove remove-janebi' data-id='" + id + "'></span><a class='dettail' href='/Admin/FileManager/Edit/"+id+"' target='_blank'><span class='glyphicon glyphicon-list' title='جزئیات'></span></a><input type='checkbox' title='تصویر پیش فرض' data-id='"+id+"' /><br/><a href='" + $("#AttachementContainer2 img").attr("src") + "' target='_blank'><img src='" + $("#AttachementContainer2 img").attr("src") + "' width='84' height='78' /></a><input type='hidden' name='janebi' value='" + id + "' /></li>");
    });
    $("body").on("click", ".remove-janebi", function () {
        var $this = $(this);
        $("#janebi-images li[data-id='" + $this.attr("data-id") + "']").remove();
    });

    //Add Default Image
    $("#janebi-images").on("click","li input[type=checkbox]",function () {
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

