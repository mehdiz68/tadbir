var selectedFiles;
var DataURLFileReader = {
    read: function (file, callback) {
        var reader = new FileReader();
        var fileInfo = {
            name: file.name,
            type: file.type,
            fileContent: null,
            size: function () {
                var FileSize = 0;
                if (file.size > 1048576) {
                    FileSize = Math.round(file.size * 100 / 1048576) / 100 + " MB";
                }
                else if (file.size > 1024) {
                    FileSize = Math.round(file.size * 100 / 1024) / 100 + " KB";
                }
                else {
                    FileSize = file.size + " bytes";
                }
                return FileSize;
            }
        };
        if (file.size >= 105906176) {
            callback("حجم فایل بیش از 200 مگابایت است", fileInfo);
            return;
        }
        reader.onload = function () {
            fileInfo.fileContent = reader.result;
            callback(null, fileInfo);
        };
        reader.onerror = function () {
            callback(reader.error, fileInfo);
        };
        reader.readAsDataURL(file);
    }
};

function multipleFiles_addMode() {
    $('.CreateLink').hide();
    Init_Multiple_Upload();
    $('[data-toggle="tooltip"]').tooltip()
}


function Init_Multiple_Upload() {
    //Select Category Of Selected Tree
    $("#Stage1 .tree #tree2 li a").each(function () {
        if ($(this).next().hasClass("subItem")) {
            $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom:0;cursor:pointer'>   [+]   <span>");
            $(this).parent().find(".subItem").hide();
        }
    });
    $("#Stage1 .tree #tree2").on("click", ".PlusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='MinusTree' style='position: relative;bottom:0;cursor:pointer'>   [-]   <span>");
        $(this).parent().find(".subItem").first().show();
        $(this).parent().find(".PlusTree").first().remove();

    });
    $("#Stage1 .tree #tree2").on("click", ".MinusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom:0;cursor:pointer'>   [+]   <span>");
        $(this).parent().find(".subItem").first().hide();
        $(this).parent().find(".MinusTree").first().remove();

    });

    $("#UseWaterMark").click(function () {
        $("#WaterMarkContainer").slideToggle();
    });

    $("#HasMultiSize").click(function () {
        $("#HasMultiSizeContainer").slideToggle();
    });

    $("#UseCompression").click(function () {
        $("#UseCompressionContainer").slideToggle();
    });


    $("#createView .tree ul li a").click(function (e) {
        e.preventDefault();
        $(".tree ul li a").removeClass("SelectTree");
        $(this).addClass("SelectTree");
        //$(this).parent().find("a").addClass("SelectTree");
        var selectedTree = $(this).closest(".tree").find("#SelectedTreeItem");
        selectedTree.text($(this).text());
        selectedTree.attr("data-id", $(this).attr("data-id"));
    });

    $("#UploadedFiles").change(function (evt) {
        MultiplefileSelected(evt);
    });
    $("#FormMultipleUpload button[id=Cancel_btn]").click(function () {
        Cancel_btn_handler()
    });
    $('#FormMultipleUpload button[id=Submit_btn]').click(function () {

        $('#UploadedFiles').remove();
        UploadMultipleFiles($(this).attr("data-url"));
    });
    var dropZone = document.getElementById('drop_zone');
    dropZone.addEventListener('dragover', handleDragOver, false);
    dropZone.addEventListener('drop', MultiplefileSelected, false);
    dropZone.addEventListener('dragenter', dragenterHandler, false);
    dropZone.addEventListener('dragleave', dragleaveHandler, false);
    $.blockUI.defaults.overlayCSS = {
        backgroundColor: '#000',
        opacity: 0.6
    };
    $.blockUI.defaults.css = {
        padding: 0,
        margin: 5,
        width: '90%',
        top: '10%',
        left: '5%',
        color: '#000',
        border: '3px solid #aaa',
        backgroundColor: '#fff'

    };
    $.blockUI({ message: $('#createView') });
}

function MultiplefileSelected(evt) {
    selectedFiles=null;
    evt.stopPropagation();
    evt.preventDefault();
    $('#drop_zone').removeClass('hover');
    selectedFiles = evt.target.files || evt.dataTransfer.files;
    if (selectedFiles) {
        $('#Files').empty();
        for (var i = 0; i < selectedFiles.length; i++) {
            DataURLFileReader.read(selectedFiles[i], function (err, fileInfo) {
                if (err != null) {
                    var RowInfo = '<div id="File_' + i + '" class="info"><div class="InfoContainer">' +
                                   '<div class="Error">' + err + '</div>' +
                                  '<div data-name="FileName" class="info">' + fileInfo.name + '</div>' +
                                  '<div data-type="FileType" class="info">' + fileInfo.type + '</div>' +
                                  '<div data-size="FileSize" class="info">' + fileInfo.size() + '</div></div><hr/></div>';
                    $('#Files').append(RowInfo);
                }
                else {
                    var image = '<img src="' + fileInfo.fileContent + '" class="thumb" title="' + fileInfo.name + '" />';
                    var RowInfo = '<div id="File_' + i + '" class="info"><div class="InfoContainer">' +
                                  '<div data_img="Imagecontainer">' + image + '</div>' +
                                  '<div data-name="FileName" class="info">' + fileInfo.name + '</div>' +
                                  '<div data-type="FileType" class="info">' + fileInfo.type + '</div>' +
                                  '<div data-size="FileSize" class="info">' + fileInfo.size() + '</div></div><hr/></div>';
                    $('#Files').append(RowInfo);
                }
            });
        }
    }
    else
    {
        $('#Files').empty();
    }
}

function UploadMultipleFiles(ajaxPostUrl) {
    
    if ($("#createView #Title").val()) {
        if (selectedFiles != null) {
            // here we will create FormData manually to prevent sending mon image files
            var form = $('#FormMultipleUpload')[0];

            var dataString = new FormData(form);
            //Add FolderId TreeView
            var FolderId = $("#SelectedTreeItem").attr("data-id");
            dataString.append("FolderId", FolderId);
            //Add LanguageId DropDownList
            var LanguageId = $("#AddAttachementLanguageId option:selected").val();
            dataString.append("LanguageId", LanguageId);
            dataString.append("PopUpAttachements", false);
            dataString.append("compressionLevel", $("#compressionLevel").val());
            //Get Current Controller and Pass it
            var url = window.location.pathname.split("/");
            var controller = url[2];
            dataString.append("controllerName", controller.toLowerCase());

            //var files = document.getElementById("UploadedFiles").files;
            //if (form[6] == null) {
            for (var i = 0; i < selectedFiles.length; i++) {
                //if (!selectedFiles[i].type.match('image.*')) {
                //    continue;
                //}

                dataString.append("uploadedFiles", selectedFiles[i]);
            }
            //}
            $.ajax({
                url: ajaxPostUrl,  //Server script to process data
                type: 'POST',
                xhr: function () {  // Custom XMLHttpRequest
                    var myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) { // Check if upload property exists
                        myXhr.upload.addEventListener('progress', progressHandlingFunction, false); // For handling the progress of the upload
                    }
                    return myXhr;
                },
                //Ajax events

                success: successHandler,
                error: errorHandler,
                complete: completeHandler,
                // Form data
                data: dataString,
                //Options to tell jQuery not to process data or worry about content-type.
                cache: false,
                contentType: false,
                processData: false
            });
        }
        else
            alert(" فایل انتخاب نشده است ");
    }
    else {
        alert("عنوان فایل را وارد نمایید");
        $("#createView #Title").focus();
    }
}


// Drag and Drop Events
function handleDragOver(evt) {
    evt.preventDefault();
    evt.dataTransfer.effectAllowed = 'copy';
    evt.dataTransfer.dropEffect = 'copy';
}

function dragenterHandler() {
    //$('#drop_zone').removeClass('drop_zone');
    $('#drop_zone').addClass('hover');
}

function dragleaveHandler() {
    $('#drop_zone').removeClass('hover');
}


function ShowAddMultiAttachment(ajaxPostUrl, LanguageId) {
    //Keep Form Data
    var currentTitle = $("#Title").val();
    var currentUseWatermarkOption = $("#WaterMarkType").val();

    var curretUseWatermark = "";
    if ($('#UseWaterMark').is(":checked"))
        curretUseWatermark = "True";
    else
        curretUseWatermark = "False";

    var currentHasMultiSize = "";
    if ($('#HasMultiSize').is(":checked"))
        currentHasMultiSize = "True";
    else
        currentHasMultiSize = "False";

    var currentUseCompression = "";
    if ($('#UseCompression').is(":checked"))
        currentUseCompression = "True";
    else
        currentUseCompression = "False";

    //Send Ajax
    $('.loading').show();
    $.ajax({
        type: 'GET',
        cache: false,
        async: false,
        url: ajaxPostUrl,
        data: "LanguageId=" + LanguageId.value,
        UpdateTargetId: "createView",
        success: function (html) {
            $('.loading').hide();
            $('#createView').empty();
            $('.CreateLink').show();
            $('#createView').html(html);
            Init_Multiple_Upload();
            //Return Form Data
            $("#AddAttachementLanguageId").val(LanguageId.value);
            $("#Title").val(currentTitle);
            $("#WaterMarkType").val(currentUseWatermarkOption);

            if (curretUseWatermark == "True") {
                $("#WaterMarkContainer").show();
                $("#UseWaterMark").prop('checked', true);
            }
            else {
                $("#UseWaterMark").prop('checked', false);
                $("#WaterMarkContainer").hide();
            }

            if (currentHasMultiSize == "True") {
                $("#HasMultiSizeContainer").show();
                $("#HasMultiSize").prop('checked', true);
            }
            else {
                $("#HasMultiSize").prop('checked', false);
                $("#HasMultiSizeContainer").hide();
            }

            if (currentUseCompression == "True") {
                $("#UseCompression").prop('checked', true);
                $("#UseCompressionContainer").show();
            }
            else {
                $("#UseCompression").prop('checked', false);
                $("#UseCompressionContainer").hide();
            }

        },
        error: function () { $('.loading').hide(); alert("error"); }
    });
}