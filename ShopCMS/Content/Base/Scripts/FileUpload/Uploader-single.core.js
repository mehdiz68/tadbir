function createMode() {
    $('.CreateLink').hide();
    Init_Upload();
  $('[data-toggle="tooltip"]').tooltip()
     
}
function Init_Upload() {

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
        var selectedTree= $(this).closest(".tree").find("#SelectedTreeItem");
        selectedTree.text($(this).text());
        selectedTree.attr("data-id", $(this).attr("data-id"));

    });

    $('#FormUpload input[name=UploadedFile]').change(function (evt) { singleFileSelected(evt); });
    $("#FormUpload button[id=Cancel_btn]").click(function () {
        Cancel_btn_handler()
    });
    $('#FormUpload button[id=Submit_btn]').click(function () {
        UploadFile($(this).attr("data-url"));
    });
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

function singleFileSelected(evt) {
    //var selectedFile = evt.target.files can use this  or select input file element and access it's files object
    var selectedFile = ($("#UploadedFile"))[0].files[0];//FileControl.files[0];
    if (selectedFile) {
        var FileSize = 0;
        if (selectedFile.size > 1048576) {
            FileSize = Math.round(selectedFile.size * 100 / 1048576) / 100 + " MB";
        }
        else if (selectedFile.size > 1024) {
            FileSize = Math.round(selectedFile.size * 100 / 1024) / 100 + " KB";
        }
        else {
            FileSize = selectedFile.size + " Bytes";
        }
        
        if (selectedFile.size < 105906176) {

            var reader = new FileReader();
            reader.onload = function (e) {
                
                $("#Imagecontainer").empty();
                var dataURL = reader.result;
                var img = new Image()
                img.src = dataURL;
                img.className = "thumb";
                $("#Imagecontainer").append(img);
            };
            reader.readAsDataURL(selectedFile);

            $("#FileName").text("Name : " + selectedFile.name);
            $("#FileType").text("type : " + selectedFile.type);
            $("#FileSize").text("Size : " + FileSize);

            //Show Watermark Position For Images
            //var imageType = /image.*/;
            //if (selectedFile.type.match(imageType)) {
            //    $("#WatermarkPosition").show();
            //}
            //else
            //{
            //    $("#WatermarkPosition").hide();
            //}
        }
        else
        {
            $("#Imagecontainer").empty();
            $("#FileName").text("");
            $("#FileType").text("");
            $("#FileSize").text("");
            alert("حجم فایل بیش از 200 مگابایت است. فایل دیگری انتخاب نمایید");
        }
    }
    else
    {

        $("#Imagecontainer").empty();
        $("#FileName").text("");
        $("#FileType").text("");
        $("#FileSize").text("");
    }
}

function UploadFile(ajaxPostUrl) {
    // we can create form by passing the form to Constructor of formData obeject
    //or creating it manually using append function  but please note file file name should be same like the action Paramter
    //var dataString = new FormData();
    //dataString.append("UploadedFile", selectedFile);
    if ($("#createView #Title").val()) {
        if (($("#UploadedFile"))[0].files[0] != undefined) {
            var form = $('#FormUpload')[0];
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

            $.ajax({
                url: ajaxPostUrl,  //Server script to process data
                type: 'POST',
                xhr: function () {  // Custom XMLHttpRequest
                    var myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) { // Check if upload property exists
                        //myXhr.upload.onprogress = progressHandlingFunction
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
            alert("فایل را انتخاب نمایید");
    }
    else {
        alert("عنوان فایل را وارد نمایید");
            $("#createView #Title").focus();
    }
}

function ShowAddAttachment(ajaxPostUrl, LanguageId)
{
    //Keep Form Data
    var currentTitle = $("#Title").val();
    var currentUseWatermarkOption= $("#WaterMarkType").val();

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

    var currentUseCompression="";
    if ($('#UseCompression').is(":checked"))
        currentUseCompression = "True";
    else
        currentUseCompression = "False";

    //Send Ajax
    $('.loading').show();
    $.ajax({
        type: 'GET',
        cache: false,
        async:false,
        url: ajaxPostUrl,
        data: "LanguageId=" + LanguageId.value,
        UpdateTargetId: "createView",
        success: function (html) {
            $('.loading').hide();
            $('#createView').empty();
            $('.CreateLink').show();
            $('#createView').html(html);
            Init_Upload();
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