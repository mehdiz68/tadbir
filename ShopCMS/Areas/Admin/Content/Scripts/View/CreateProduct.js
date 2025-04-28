
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

    //Add Advantage
    $(".advantages").on("click", ".glyphicon-plus-sign", function () {
        if ($(".txbAdvantage").length < 10) {
            $(".advantages").append("<input type='text' class='form-control txbAdvantage' /> <span class='glyphicon glyphicon-plus-sign'></span> &nbsp; <span class='glyphicon glyphicon-minus-sign'></span>");
        }
    });
    $(".advantages").on("click", ".glyphicon-minus-sign", function () {
        if ($(".txbAdvantage").length > 1) {
            $(this).prev().remove();
            $(this).prev().remove();
            $(this).remove();
        }
    });
    //Add Disadvantages
    $(".Disadvantages").on("click", ".glyphicon-plus-sign", function () {
        if ($(".txbDisadvantage").length < 10) {
            $(".Disadvantages").append("<input type='text' class='form-control txbDisadvantage' /> <span class='glyphicon glyphicon-plus-sign'></span> &nbsp; <span class='glyphicon glyphicon-minus-sign'></span>");
        }
    });
    $(".Disadvantages").on("click", ".glyphicon-minus-sign", function () {
        if ($(".txbDisadvantage").length > 1) {
            $(this).prev().remove();
            $(this).prev().remove();
            $(this).remove();
        }
    });


    var advantages = [];
    var Disadvantages = [];
    $(".txbDisadvantage").each(function () {
        Disadvantages.push($(this).val())
    });
    $(".txbAdvantage").each(function () {
        advantages.push($(this).val())
    });
    //Add Point
    $("#Add-Point").click(function () {
        var ProductId = $(this).attr("data-id");

        var advantages = [];
        var Disadvantages = [];
        $(".txbDisadvantage").each(function () {
            if ($(this).val())
                Disadvantages.push($(this).val())
        });
        $(".txbAdvantage").each(function () {
            if ($(this).val())
                advantages.push($(this).val())
        });

        if (advantages.length > 0 || Disadvantages.length > 0) {
            $(".loading").show(); NProgress.start();
            $.ajax({
                type: "POST",
                traditional: true,
                url: '/Admin/Products/AddPoint/',
                data: JSON.stringify({ advantages: advantages, Disadvantages: Disadvantages, ProductId: ProductId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {

                        swal({ title: "", text: response.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                    }
                    else {
                        swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide(); NProgress.done();
                }
            });

        }
        else {

            swal({ title: "", text: "مقداری وارد نشده است !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
        }
    });

    $(".checkbox-selector").change(function () {
        if ($(this).is(":checked")) {
            $(this).parent().next().find("input").prop("disabled", false);
        }
        else {
            $(this).parent().next().find("input").prop("disabled", true);

        }
    });

    //$(".ProductAttributeGroups input,.ProductAttributeGroups textarea,.ProductAttributeGroups select option").prop("disabled", true);
    $(".AttributeHeader").click(function () {

        if ($(this).hasClass("open")) {
            $(".AttributeHeader").removeClass("open");
            $(".ProductAttributeGroups").addClass("hide");
            $(this).removeClass("open");
            $(this).next().addClass("hide");
        }
        else {
            $(".AttributeHeader").removeClass("open");
            $(".ProductAttributeGroups").addClass("hide");
            $(this).addClass("open");
            $(this).next().removeClass("hide");

            $('html, body').animate({
                scrollTop: $(this).offset().top
            }, 500);
        }

        //$(".ProductAttributeGroups input,.ProductAttributeGroups textarea,.ProductAttributeGroups select option").prop("disabled", true);
        //$(this).next().find("input").prop("disabled", false);
        //$(this).next().find("textarea").prop("disabled", false);
        //$(this).next().find("select option").prop("disabled", false);
    });
    //change product type-----------------------------------------------------------------------
    $("#ProductTypeId").change(function () {
        if ($(this).val() == "1") {
            $("#prtabs li a").removeClass("hide");
            $("#prtabs li a[href='#pack'],#prtabs li a[href='#files'],#prtabs li a[href='#courses']").addClass("hide");
            $("#Width,#Height,#Lenght,#ProductWeight").val("");
            $("#ProductSendWayId").attr("data-val", "true").attr("data-val-required", "روش های ارسال باید انتخاب شود");
            $('#ProductSendWayId').selectpicker('refresh');
        }
        else if ($(this).val() == "2") {
            $("#prtabs li a").removeClass("hide");
            $("#prtabs li a[href='#files'],#prtabs li a[href='#courses']").addClass("hide");
            $("#Width,#Height,#Lenght,#ProductWeight").val("");
            $("#ProductSendWayId").attr("data-val", "true").attr("data-val-required", "روش های ارسال باید انتخاب شود");
            $('#ProductSendWayId').selectpicker('refresh');

        }
        else if ($(this).val() == "3") {
            $("#prtabs li a").removeClass("hide");
            $("#prtabs li a[href='#pack'],#prtabs li a[href='#package'],#prtabs li a[href='#courses']").addClass("hide");
            $("#Width,#Height,#Lenght,#ProductWeight,#ExtraSendPrice").val("0");

            $("#ProductSendWayId").removeAttr("data-val"); $("#ProductSendWayId").removeAttr("data-val-required");
            $('#ProductSendWayId').selectpicker('refresh');
            $("#AddForm").removeData('unobtrusiveValidation');
            $("#AddForm").removeData('validator');
            $.validator.unobtrusive.parse("#AddForm");

        }
        else if ($(this).val() == "4") {
            $("#prtabs li a").removeClass("hide");
            $("#prtabs li a[href='#pack'],#prtabs li a[href='#package'],#prtabs li a[href='#files']").addClass("hide");
            $("#Width,#Height,#Lenght,#ProductWeight,#ExtraSendPrice").val("0");

            $("#ProductSendWayId").removeAttr("data-val"); $("#ProductSendWayId").removeAttr("data-val-required");
            $('#ProductSendWayId').selectpicker('refresh');
            $("#AddForm").removeData('unobtrusiveValidation');
            $("#AddForm").removeData('validator');
            $.validator.unobtrusive.parse("#AddForm");


        }
        else {
            $("#prtabs li a").removeClass("hide");
            $("#prtabs li a[href='#pack'],#prtabs li a[href='#files'],#prtabs li a[href='#courses']").addClass("hide");
            $("#Width,#Height,#Lenght,#ProductWeight,#ExtraSendPrice").val("0");

            $("#ProductSendWayId").removeAttr("data-val"); $("#ProductSendWayId").removeAttr("data-val-required");
            $('#ProductSendWayId').selectpicker('refresh');
            $("#AddForm").removeData('unobtrusiveValidation');
            $("#AddForm").removeData('validator');
            $.validator.unobtrusive.parse("#AddForm");
        }
    });

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
        if ($("#ProductCatId").val() != "")
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
        $("#janebi-images").append("<li  data-id='" + id + "'><span class='glyphicon glyphicon-remove remove-janebi' data-id='" + id + "'></span><a class='dettail' href='/Admin/FileManager/Edit/" + id + "' target='_blank'><span class='glyphicon glyphicon-list' title='جزئیات'></span></a><input type='checkbox' title='تصویر پیش فرض' data-id='" + id + "' /><br/><a href='" + $("#AttachementContainer2 img").attr("src") + "' target='_blank'><img src='" + $("#AttachementContainer2 img").attr("src") + "' width='84' height='78' /></a><input type='hidden' name='janebi' value='" + id + "' /></li>");
    });
    $("body").on("click", ".remove-janebi", function () {
        var $this = $(this);
        $("#janebi-images li[data-id='" + $this.attr("data-id") + "']").remove();
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

    //Courses-----------------------------------------------------------------------------------
    $("#AddNewCourse").click(function () {
        if ($("#txtNewLesson").val()) {
            $(".loading").show(); NProgress.start();
            if ($(this).attr("data-id") != undefined && parseInt($(this).attr("data-id")) > 0)//edit
            {
                var id = $(this).attr("data-id");
                $.ajax({
                    async: true,
                    type: "POST",
                    url: '/Admin/Products/UpdateCourse/',
                    data: '{Id:"' + parseInt(id) + '",Title:"' + $("#txtNewLesson").val() + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 200) {
                            $("#CourseId option[value='" + id + "']").text($("#txtNewLesson").val());
                            $(".CourseList[data-id='" + id + "']").find("h3 label").text($("#txtNewLesson").val());
                            $("#CourseId").val(response.Id);
                            $("#CourseId").selectpicker('refresh');
                            $("#txtNewLesson").val("");
                            $("#AddNewCourse").val("افزودن سرفصل جدید");
                            $("#AddNewCourse").attr("data-id", "");
                            swal({ title: "", text: "با موفقیت ویرایش شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                            $(".loading").hide(); NProgress.done();
                            $("#txtNewLesson").val("");
                        }
                        else {
                            swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        }
                        $(".loading").hide(); NProgress.done();
                    },
                    error: function (data) {
                        swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $(".loading").hide(); NProgress.done();
                    }
                });
            }
            else {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: '/Admin/Products/AddCourse/',
                    data: '{ProductId:"' + parseInt($("#ProductId").val()) + '",Title:"' + $("#txtNewLesson").val() + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 200) {
                            $("#CourseLists").append(" <div class='CourseList' data-id='" + response.Id + "'><h3> <span class='glyphicon glyphicon-remove' data-id='" + response.Id + "'></span> <span class='glyphicon glyphicon-edit' data-id='" + response.Id + "'></span> <label>" + $("#txtNewLesson").val() + "</label></h3><ul></ul></div>");
                            $("#CourseId").append("<option value='" + response.Id + "'>" + $("#txtNewLesson").val() + "</option>");
                            $("#CourseId").val(response.Id);
                            $("#CourseId").selectpicker('refresh')
                            swal({ title: "", text: "با موفقیت اضافه شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                            $("#txtNewLesson").val("");
                        }
                        else {
                            swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        }
                        $(".loading").hide(); NProgress.done();
                    },
                    error: function (data) {
                        swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $(".loading").hide(); NProgress.done();
                    }
                });//add
            }
        }
        else {
            swal({ title: "", text: "نام دوره را وارد نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
        }
    });
    $("body").on("click", ".CourseList h3 .glyphicon-remove", function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا اطمینان دارید؟",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "بلی",
            cancelButtonText: "خیر",
            cancelButtonClass: "btn-danger",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {

                    $(".loading").show(); NProgress.start();
                    $.ajax({
                        async: true,
                        type: "POST",
                        url: '/Admin/Products/RemoveCourse/',
                        data: '{Id:"' + parseInt($this.attr("data-Id")) + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                $("#CourseId option[value='" + $this.attr("data-id") + "']").remove();
                                $("#CourseId").selectpicker('refresh')
                                $this.parent().parent().remove();
                                swal({ title: "", text: "با موفقیت حذف شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                            }
                            else {
                                swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            }
                            $(".loading").hide(); NProgress.done();
                        },
                        error: function (data) {
                            swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            $(".loading").hide(); NProgress.done();
                        }
                    });
                }
                else {
                    swal.close();
                }
            }
        );
    });
    $("body").on("click", ".CourseList h3 .glyphicon-edit", function () {
        var $this = $(this);
        $("#AddNewCourse").focus();
        $("#txtNewLesson").val($this.parent().find("label").text());
        $("#AddNewCourse").val("ویرایش");
        $("#AddNewCourse").attr("data-id", $this.attr("data-id"));
        $("html, body").animate({ scrollTop: 0 }, "slow");
    });
    $("#AddLesson").click(function () {
        var isvalid = true;
        if (!$("#LessonTitle").val()) {
            isvalid = false;
            $("#LessonTitle").addClass("input-validation-error");
        }
        else
            $("#LessonTitle").removeClass("input-validation-error");
        if (!$("#LessonDescription").val()) {
            isvalid = false;
            $("#LessonDescription").addClass("input-validation-error");
        }
        else
            $("#LessonDescription").removeClass("input-validation-error");
        if (!$("#LessonCapacity").val()) {
            isvalid = false;
            $("#LessonCapacity").addClass("input-validation-error");
        }
        else
            $("#LessonCapacity").removeClass("input-validation-error");
        if (!$("#LessonDuration").val()) {
            isvalid = false;
            $("#LessonDuration").addClass("input-validation-error");
        }
        else
            $("#LessonDuration").removeClass("input-validation-error");
        if (!$("#LessonPrice").val()) {
            isvalid = false;
            $("#LessonPrice").addClass("input-validation-error");
        }
        else
            $("#LessonPrice").removeClass("input-validation-error");
        if (!$("#CourseLessonVideo").val()) {
            isvalid = false;
            swal({ title: "", text: "ویدئو انتخاب نشده است !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
        }
        if (isvalid) {
            if ($(this).attr("data-id") != undefined && parseInt($(this).attr("data-id")) > 0)//edit
            {
                var id = $(this).attr("data-id");
                $.ajax({
                    async: true,
                    type: "POST",
                    url: '/Admin/Products/UpdateCourseLesson/',
                    data: '{Id:"' + parseInt(id) + '",CourseId:"' + parseInt($("#CourseId").val()) + '",Title:"' + $("#LessonTitle").val() + '",description:"' + $("#LessonDescription").val() + '",capacity:"' + parseFloat($("#LessonCapacity").val()) + '",duration:"' + parseInt($("#LessonDuration").val()) + '",price:"' + parseInt($("#LessonPrice").val()) + '",video:"' + $("#CourseLessonVideo").val() + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 200) {
                            var li = $(".CourseList ul li[data-id='" + id + "']");
                            li.find(".Title").text(response.data.Title);
                            li.find(".Duration").text(response.data.Duration);
                            li.find(".Video").attr("href", "/Content/UploadFiles/" + response.data.video);
                            $("#AddLesson").val("افزودن درس");
                            $("#AddLesson").attr("data-id", "");
                            swal({ title: "", text: "با موفقیت ویرایش شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                            $(".loading").hide(); NProgress.done();
                            $("#LessonTitle").val(""); $("#LessonDescription").val(""); $("#LessonCapacity").val(""); $("#LessonDuration").val(""); $("#LessonPrice").val(""); $("#CourseLessonVideo").val(""); $("#CourseLessonVideo").prev().attr("href", ""); $("#CourseLessonVideo").prev().text("");
                        }
                        else {
                            swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        }
                        $(".loading").hide(); NProgress.done();
                    },
                    error: function (data) {
                        swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $(".loading").hide(); NProgress.done();
                    }
                });
            }
            else {
                $.ajax({
                    async: true,
                    type: "POST",
                    url: '/Admin/Products/AddLesson/',
                    data: '{CourseId:"' + parseInt($("#CourseId").val()) + '",Title:"' + $("#LessonTitle").val() + '",description:"' + $("#LessonDescription").val() + '",capacity:"' + parseFloat($("#LessonCapacity").val()) + '",duration:"' + parseInt($("#LessonDuration").val()) + '",price:"' + parseInt($("#LessonPrice").val()) + '",video:"' + $("#CourseLessonVideo").val() + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 200) {
                            $(".CourseList[data-id='" + parseInt($("#CourseId").val()) + "']").find("ul").append(" <li data-id=" + response.Id + "><div class='Operation col-md-3'><span class='glyphicon glyphicon-remove' data-id=" + response.Id + "></span> <span class='glyphicon glyphicon-edit' data-id=" + response.Id + "></span></div><div class='Title col-md-3'>" + $("#LessonTitle").val() + "</div><div class='Duration col-md-3'>" + $("#LessonDuration").val() + "</div><div class='Video col-md-3'><a href='" + $("#CourseLessonVideo").parent().find("a").attr("href") + "'><span class='fa fa-download'></span></a></div><div class='clearfix'></div></li>");
                            swal({ title: "", text: "با موفقیت اضافه شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                            $("#LessonTitle").val(""); $("#LessonDescription").val(""); $("#LessonCapacity").val(""); $("#LessonDuration").val(""); $("#LessonPrice").val(""); $("#CourseLessonVideo").val("");
                        }
                        else {
                            swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        }
                        $(".loading").hide(); NProgress.done();
                    },
                    error: function (data) {
                        swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $(".loading").hide(); NProgress.done();
                    }
                });//add
            }
        }
    });
    $("body").on("click", ".CourseList ul li .glyphicon-remove", function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا اطمینان دارید؟",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "بلی",
            cancelButtonText: "خیر",
            cancelButtonClass: "btn-danger",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {

                    $(".loading").show(); NProgress.start();
                    $.ajax({
                        async: true,
                        type: "POST",
                        url: '/Admin/Products/RemoveCourseLesson/',
                        data: '{Id:"' + parseInt($this.attr("data-Id")) + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                $this.parent().parent().remove();
                                swal({ title: "", text: "با موفقیت حذف شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                            }
                            else {
                                swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            }
                            $(".loading").hide(); NProgress.done();
                        },
                        error: function (data) {
                            swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            $(".loading").hide(); NProgress.done();
                        }
                    });
                }
                else {
                    swal.close();
                }
            }
        );
    });
    $("body").on("click", ".CourseList  ul li  .glyphicon-edit", function () {
        var $this = $(this);

        $(".loading").show(); NProgress.start();
        $.ajax({
            async: true,
            type: "GET",
            url: '/Admin/Products/GetCourseLesson/' + parseInt($this.attr("data-Id")),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#CourseId").val(response.data.ProductCourseId);
                    $("#CourseId").selectpicker('refresh');
                    $("#LessonTitle").val(response.data.Title);
                    $("#LessonDescription").val(response.data.Description);
                    $("#LessonCapacity").val(response.data.Capacity);
                    $("#LessonDuration").val(response.data.Duration);
                    $("#LessonPrice").val(response.data.price);
                    $("#CourseLessonVideo").val(response.data.AttachementId);
                    $("#CourseLessonVideo").prev().attr("href", "/Content/UploadFiles/" + response.data.video);
                    $("#CourseLessonVideo").prev().text("دانلو ویدئو");
                    $("#AddLesson").attr("data-id", $this.attr("data-Id"));
                    $("#AddLesson").val("ویرایش");
                    $("html, body").animate({ scrollTop: $("#Courses").position().top }, "slow");

                }
                else {
                    swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }
                $(".loading").hide(); NProgress.done();
            },
            error: function (data) {
                swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                $(".loading").hide(); NProgress.done();
            }
        });

    });

    //Rank--------------------------------------------------------------------------------------
    jQuery.ajaxSettings.traditional = true;
    $(".RankGroup h3").on("click", ".fa-plus", function () {
        $(this).parent().parent().find("ul").fadeIn();
        $(this).addClass("fa-minus");
        $(this).removeClass("fa-plus");
    });
    $(".RankGroup h3").on("click", ".fa-minus", function () {
        $(this).parent().parent().find("ul").fadeOut();
        $(this).addClass("fa-plus");
        $(this).removeClass("fa-minus");
    });
    $(".RankGroup h3 input").click(function () {
        var $this = $(this);
        if ($this.is(":checked")) {
            $(".loading").show(); NProgress.start();
            $.ajax({
                async: true,
                type: "POST",
                url: '/Admin/Products/AddProductRankSelect/',
                data: '{ProductId:"' + parseInt($("#ProductId").val()) + '",GroupId:"' + parseInt($this.attr("data-Id")) + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        $this.parent().parent().find("ul li").each(function () {
                            $("#CurrentRanks ul").prepend("<li data-id='" + parseInt($(this).attr("data-Id")) + "'><span class='fa fa-remove' data-id='" + parseInt($(this).attr("data-Id")) + "'></span>&nbsp;<label class='CurrentRank'>" + $(this).text() + "</label><br /><select><option selected='true'>--انتخاب نشده--</option><option>1</option><option>2</option><option>3</option><option>4</option><option>5</option></select></li>");
                            $(this).find("input").prop("checked", true);
                        });
                        $this.parent().parent().find("ul").show();
                        swal({ title: "", text: "با موفقیت اضافه شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    }
                    $(".loading").hide(); NProgress.done();
                },
                error: function (data) {
                    swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    $(".loading").hide(); NProgress.done();
                }
            });
        }
    });
    $(".RankGroup ul li input").change(function () {
        var $this = $(this);
        if ($this.is(":checked")) {
            $(".loading").show(); NProgress.start();
            $.ajax({
                async: true,
                type: "POST",
                url: '/Admin/Products/AddProductRankSelect/',
                data: '{ProductId:"' + parseInt($("#ProductId").val()) + '",Id:"' + parseInt($this.attr("data-Id")) + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        $("#CurrentRanks ul").prepend("<li data-id='" + parseInt($this.attr("data-Id")) + "'><span class='fa fa-remove' data-id='" + parseInt($this.attr("data-Id")) + "'></span>&nbsp;<label class='CurrentRank'>" + $this.parent().text() + "</label><br /><select><option selected='true'>--انتخاب نشده--</option><option>1</option><option>2</option><option>3</option><option>4</option><option>5</option></select></li>");
                        swal({ title: "", text: "با موفقیت اضافه شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    }
                    $(".loading").hide(); NProgress.done();
                },
                error: function (data) {
                    swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    $(".loading").hide(); NProgress.done();
                }
            });
        }
    });
    $("body").on("click", "#CurrentRanks ul li .fa-remove", function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا اطمینان دارید؟",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "بلی",
            cancelButtonText: "خیر",
            cancelButtonClass: "btn-danger",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {

                    $(".loading").show(); NProgress.start();
                    $.ajax({
                        async: true,
                        type: "POST",
                        url: '/Admin/Products/RemoveProductRankSelect/',
                        data: '{ProductId:"' + parseInt($("#ProductId").val()) + '",Id:"' + parseInt($this.attr("data-Id")) + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                $this.parent().remove();
                                swal({ title: "", text: "با موفقیت حذف شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                            }
                            else {
                                swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            }
                            $(".loading").hide(); NProgress.done();
                        },
                        error: function (data) {
                            swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            $(".loading").hide(); NProgress.done();
                        }
                    });
                }
                else {
                    swal.close();
                }
            }
        );
    });
    $("#Update-Primary-Rank").click(function () {
        var ids = [];
        var values = [];
        $("#CurrentRanks ul li").each(function () {
            ids.push($(this).attr("data-id"));
            values.push($(this).find("select").val());
        });


        $(".loading").show(); NProgress.start();
        $.ajax({
            async: true,
            type: "POST",
            url: '/Admin/Products/ProductRankSelectValue/',
            data: '{ProductId:"' + parseInt($("#ProductId").val()) + '",ids:"' + ids + '",values:"' + values + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    swal({ title: "", text: "با موفقیت ذخیره شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                }
                else {
                    swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }
                $(".loading").hide(); NProgress.done();
            },
            error: function (data) {
                swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                $(".loading").hide(); NProgress.done();
            }
        });
    });

    //Tag----------------------------------------------------------------------------------------
    $('#SearchTag').keyup(function (e) {
        clearTimeout($.data(this, 'timer'));
        if (e.keyCode == 13)
            search(true);
        else
            $(this).data('timer', setTimeout(search, 680));
    });
    function search(force) {
        $(".loading").show();
        var existingString = $("#SearchTag").val();
        if (!force && existingString.length < 2) {
            $(".loading").hide();
            return; //wasn't enter, not > 1 char
        }
        $.get('/Contents/SearchTag/?TagName=' + existingString, function (data) {
            $("#TagContainer .Tagcontent").html("");
            for (var i = 0; i < data.data.length; i++) {
                var finder = $("#SelectedTags input[value='" + data.data[i].Id + "']");
                if (finder.length>0)
                    $("#TagContainer .Tagcontent").append("<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12' style='padding:0'><input data-id='" + data.data[i].Id + "' name='" + data.data[i].TagName + "' type='checkbox' value='true' checked><input name='" + data.data[i].TagName + "' type='hidden' value='false'><label style='font-weight:normal'>" + data.data[i].TagName + "</label></div>");
                else
                    $("#TagContainer .Tagcontent").append("<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12' style='padding:0'><input data-id='" + data.data[i].Id + "' name='" + data.data[i].TagName + "' type='checkbox' value='true'><input name='" + data.data[i].TagName + "' type='hidden' value='false'><label style='font-weight:normal'>" + data.data[i].TagName + "</label></div>");
            }

            $("#TagContainer .Tagcontent").append("<div class='clearfix'></div>");

            $(".loading").hide();
        });
    }
    /* Show All */
    $("#ResetTag").click(function () {
        $.get('/Contents/AllTag/', function (data) {
            $("#TagContainer .Tagcontent").html("");
            for (var i = 0; i < data.data.length; i++) {
                $("#TagContainer .Tagcontent").append("<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12' style='padding:0'><input data-id='" + data.data[i].Id + "' name='" + data.data[i].TagName + "' type='checkbox' value='true'><input name='" + data.data[i].TagName + "' type='hidden' value='false'><label style='font-weight:normal'>" + data.data[i].TagName + "</label></div>");
            }
            $("#TagContainer .Tagcontent").append("<div class='clearfix'></div>");
            $("#SearchTag").val("");
            $("#AddTagText").val("");
            //$("#SelectedTags").html("");
            $("#SelectedTagsForEdit").html("");
            $("#SelectedTags .RemoveTagSelection").each(function () {
                $(".Tagcontent input[data-id='" + $(this).attr("data-id") + "']").prop("checked", true);
            });
            $(".loading").hide();
        });

    });
    /*Select For Content*/
    $("#SelectForContent").click(function () {
        //$("#SelectedTagsForEdit").html("");
        var ids = [];
        var checkedInputs = $(".Tagcontent input:checked");
        if (checkedInputs.length > 0) {
            //$("#SelectedTags").html("<p> برچسب های انتخابی :  </p><hr/>");
            for (var i = 0; i < checkedInputs.length; i++) {
                if (!$("#SelectedTags input[value='" + checkedInputs.eq(i).attr("data-id") + "']").length)
                    ids.push(checkedInputs.eq(i).attr("data-id"));
            }
            if (ids.length > 0) {
                $(".loading").show(); NProgress.start();
                $.ajax({
                    async: true,
                    type: "POST",
                    url: '/Admin/Products/AddTag/',
                    data: '{ProductId:"' + parseInt($("#ProductId").val()) + '",ids:"' + ids + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.statusCode == 200) {
                            for (var i = 0; i < checkedInputs.length; i++) {
                                if (!$("#SelectedTags input[value='" + checkedInputs.eq(i).attr("data-id") + "']").length)
                                    $("#SelectedTags").append("<span><input type='hidden' name='TagId' id='Tag-" + checkedInputs.eq(i).attr("data-id") + "' value='" + checkedInputs.eq(i).attr("data-id") + "' /><span data-id='" + checkedInputs.eq(i).attr("data-id") + "' style='cursor:pointer' class='RemoveTagSelection glyphicon glyphicon-remove'></span> " + checkedInputs.eq(i).attr("name") + " </span>&nbsp;&nbsp;");

                            }
                            swal({ title: "", text: "با موفقیت ذخیره شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                        }
                        else {
                            swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                        }
                        $(".loading").hide(); NProgress.done();
                    },
                    error: function (data) {
                        swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $(".loading").hide(); NProgress.done();
                    }
                });
            }

        }
        else
            $("#SelectedTags").html("");
    });
    /*Deelte From Selection */
    $("#SelectedTags").on("click", ".RemoveTagSelection", function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا مطمئن هستید؟",
            type: "success",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "بلی",
            cancelButtonText: "خیر",
            cancelButtonClass: "btn-danger",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {
                    $(".loading").show(); NProgress.start();
                    $.ajax({
                        async: true,
                        type: "POST",
                        url: '/Admin/Products/RemoveTag/',
                        data: '{ProductId:"' + parseInt($("#ProductId").val()) + '",Id:"' + parseInt($this.attr("data-Id")) + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                $this.parent().remove();
                                $(".Tagcontent input[data-id='" + $this.attr("data-Id") + "']").prop("checked", false);
                                if ($("#SelectedTags > span").size() == 0)
                                    $("#SelectedTags").html("");
                                swal.close();
                                swal({ title: "", text: "با موفقیت حذف شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                            }
                            else {
                                swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            }
                            $(".loading").hide(); NProgress.done();
                        },
                        error: function (data) {
                            swal({ title: "", text: data.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            $(".loading").hide(); NProgress.done();
                        }
                    });

                } else {
                    swal.close();
                }
            });


    });
    /*Select For Edit*/
    $("#EditTag").click(function () {
        $("#SelectedTags").html("");
        var checkedInputs = $(".Tagcontent input:checked");
        if (checkedInputs.length > 0) {

            var html = "<p><span id='closeEditTag' class='glyphicon glyphicon-remove' style='color:red;cursor:pointer'></span> برچسب های انتخابی برای ویرایش:  </p>";
            $("#SelectedTagsForEdit").html(html);
            html = "<div class='table-responsive'><table class='table'><tr><th>برچسب</th><th></th>";
            for (var i = 0; i < checkedInputs.length; i++) {
                html += "<tr><td><input type='text' placeholder='عنوان جدید را وارد نمایید...' value='" + checkedInputs.eq(i).attr("Name") + "'></td><td><span data-id='" + checkedInputs.eq(i).attr("data-id") + "' style='cursor:pointer' title='ویرایش' class='EditTag glyphicon glyphicon-edit'></span></td></tr>";
            }
            html += "</table></div>";
            $("#SelectedTagsForEdit").append(html);

            $("#closeEditTag").click(function () {
                $("#SelectedTagsForEdit").html("");
                $(".Tagcontent input").prop("checked", false);
            });

        }
        else
            $("#SelectedTagsForEdit").html("");
    });
    /* Edit Tag */
    $("#SelectedTagsForEdit").on("click", ".EditTag", function () {
        var text = $(this).parent().prev().find("input").val();
        var id = $(this).attr("data-id");
        $(".loading").show();
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Contents/EditTags/',
            data: '{id:"' + id + '",tagName:"' + text + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    swal({ title: "", text: "ویرایش انجام شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }
                else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }
                $(".loading").hide();
            }
        });
    });
    $("#DeleteTag").click(function () {
        swal({
            title: "",
            text: "آیا مطمئن هستید؟ تگ های انتخابی به طور کلی حذف خواهند شد",
            type: "success",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "بلی",
            cancelButtonText: "خیر",
            cancelButtonClass: "btn-danger",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {
                    var checkedInputs = $(".Tagcontent input:checked");
                    if (checkedInputs.length > 0) {
                        var ids = "";
                        for (var i = 0; i < checkedInputs.length; i++) {
                            ids += checkedInputs.eq(i).attr("data-id") + ",";
                        }
                        ids = ids.substring(0, ids.length - 1);
                        var id = $(this).attr("data-id");
                        $(".loading").show();
                        $.ajax({
                            crossDomain: true,
                            type: "POST",
                            url: '/Contents/DeleteTags/',
                            data: '{ids:"' + ids + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response.statusCode == 200) {
                                    for (var i = 0; i < response.deletedTagId.length; i++) {
                                        $(".Tagcontent input[data-id=" + response.deletedTagId[i] + "]").parent().remove();
                                    }

                                    $("#tag-Message").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> " + response.Message + " </p>").fadeIn(1000);
                                    $(".catmsg").delay(8000).fadeOut(1000, function () { $(this).remove(); });
                                }
                                else {

                                    $("#tag-Message").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> " + response.Message + " </p>").fadeIn(1000);
                                    $(".catmsg").delay(8000).fadeOut(1000, function () { $(this).remove(); });
                                }
                                $(".loading").hide();
                                swal.close();
                            }
                        });
                    }
                    else
                        swal({ title: "", text: "برچسبی انتخاب نشده است", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                } else {
                    swal.close();
                }
            });


    });
    /*Add Tag*/
    $("#AddNewTag").click(function () {
        if ($("#AddTagText").val()) {
            var languageid = 1;
            if ($("#LanguageId option:selected").val())
                languageid = parseInt($("#LanguageId option:selected").val());
            else if ($("#LanguageId").val())
                languageid = parseInt($("#LanguageId").val());

            $(".loading").show();
            $.ajax({
                crossDomain: true,
                type: "POST",
                url: '/Contents/AddTags/',
                data: '{TagName:"' + $("#AddTagText").val() + '",LanguageId:"' + languageid + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        $(".Tagcontent").prepend("<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12' style='padding:0'><input data-id='" + response.Id + "' name='" + $("#AddTagText").val() + "' type='checkbox' value='true'><input name='" + $("#AddTagText").val() + "' type='hidden' value='false'><label style='font-weight:normal'>" + $("#AddTagText").val() + "</label></div>");

                        swal({ title: "", text: "ثبت شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $("#AddTagText").val("");
                    }
                    else {
                        swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide();
                }
            });
        }
        else
            swal({ title: "", text: "نام برچسب را وارد نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

    });

    //Product Attribute Select---------------------------------------------------------------------

    $("#Update-Attribute").click(function () {

        var AttributeSelects = [];
        var $this = $(this);
        isValid = true;
        //Add Single AND Not Relational Values
        $(".ProductAttribute").each(function () {
            var ProductAttribute = $(this);
            if (ProductAttribute.attr("data-hasmultiplevalue") == "False") {
                var DataType = parseInt(ProductAttribute.attr("data-type-id"));
                var GroupCatAttributeId = parseInt(ProductAttribute.attr("data-id"));
                var AttributeId = parseInt(ProductAttribute.attr("data-attribute-id"));
                var value = GetValue(AttributeId, DataType, ProductAttribute, "false");
                if (value == "nok")
                    isValid = false;
                else if (value != "") {
                    var obj = {
                        GroupCatAttributeId: GroupCatAttributeId,
                        AttributeId: AttributeId,
                        Value: value
                    };
                    AttributeSelects.push(obj);

                }
            }
        });
        //Add Multiple AND Not Relational Values
        $(".MultipleValues").each(function () {
            var ProductAttribute = $(this).parent().parent();
            var DataType = parseInt(ProductAttribute.attr("data-type-id"));
            var GroupCatAttributeId = parseInt(ProductAttribute.attr("data-id"));
            var AttributeId = parseInt(ProductAttribute.attr("data-attribute-id"));
            if ($(this).find(".selectedValue").length == 0 && ProductAttribute.attr("data-isrequired") == "True") {
                $(ProductAttribute).after("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='attmsg'> مقداری انتخاب نشده است </p>").fadeIn(300);
                $(".attmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                isValid = false;
            }
            else {
                $(this).find(".selectedValue").each(function () {

                    var obj = {
                        GroupCatAttributeId: GroupCatAttributeId,
                        AttributeId: AttributeId,
                        Value: GetRawAttributeValue(DataType, $(this))
                    };
                    AttributeSelects.push(obj);
                });
            }

        });

        //Add If Form is Valid
        if (isValid) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/Products/AddProductAttributeSelects/',
                data: JSON.stringify({ productid: parseInt($("#ProductId").val()), AttributeSelects: AttributeSelects }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        swal({ title: "", text: "خصوصیات با موفقیت ثبت شدند.", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                        $('html, body').animate({
                            scrollTop: $("#ErrorSummary").offset().top
                        }, 500);



                    }

                    else {
                        swal({ title: "", text: "خطایی رخ داد", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        $('html, body').animate({
                            scrollTop: $("#ErrorSummary").offset().top
                        }, 500);
                    }

                    $(".loading").hide();
                }
            });

        }
        else {
            $('html, body').animate({
                scrollTop: $("#ErrorSummary").offset().top
            }, 500);
        }



    });
});

